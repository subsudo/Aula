using System;
using System.Collections.Generic;
using System.Linq;
using VM = VerlaufsakteApp.Models;
using VS = VerlaufsakteApp.Services;

namespace XHub.Scola;

/// <summary>
/// Aula-seitiger Klassen-Import: nimmt eingefuegten Text/Klasse, parst die Namen
/// (Scolas <see cref="VS.ParticipantParser"/>) und matcht sie gegen den
/// Server-TN-Pool (Scolas <see cref="VS.FolderMatcher"/>).
/// </summary>
public sealed class ClassImportService
{
    // Marker wie in Scolas Einstellungen, damit Anwesenheit automatisch aus der
    // eingefuegten Liste abgeleitet wird (zusaetzlich zur eingebauten Parser-Logik).
    private static readonly string[] DefaultAbsenceValues =
    {
        "abwesend",
        "abwesend entschuldigt",
        "abwesend unentschuldigt",
        "abwesend (entschuldigt)",
        "abwesend (unentschuldigt)"
    };

    private static readonly string[] DefaultPresenceValues = { "anwesend" };

    private readonly VS.ParticipantParser _parser;
    private readonly VS.ParticipantHintsService _hints = new(null);

    public ClassImportService(IEnumerable<string>? absenceValues = null, IEnumerable<string>? presenceValues = null)
    {
        _parser = new VS.ParticipantParser(
            absenceValues ?? DefaultAbsenceValues,
            presenceValues ?? DefaultPresenceValues);
    }

    /// <summary>Laedt die aktiven Bemerkungs-Anzeigen (farbige Punkte) fuer eine Akte.</summary>
    public IReadOnlyList<VM.ParticipantHintDisplay> LoadHints(string documentPath)
    {
        return string.IsNullOrWhiteSpace(documentPath)
            ? Array.Empty<VM.ParticipantHintDisplay>()
            : _hints.LoadActiveDisplays(documentPath);
    }

    /// <summary>
    /// Parst und matcht in einem Rutsch. Laeuft potenziell laenger (Ordner-Scan),
    /// daher vom Aufrufer auf einem Hintergrund-Thread aufrufen.
    /// </summary>
    public List<VM.Participant> Import(
        string text,
        string serverBasePath,
        bool useSecondaryBasePath,
        string? secondaryBasePath,
        string? verlaufsakteKeyword)
    {
        var matcher = new VS.FolderMatcher(
            serverBasePath,
            useSecondaryBasePath,
            secondaryBasePath,
            useTertiaryServerBasePath: false,
            tertiaryServerBasePath: null,
            verlaufsakteKeyword: verlaufsakteKeyword,
            initialsResolver: new VS.InitialsResolver());

        matcher.BuildIndex();

        var participants = _parser.Parse(text, rawLine => matcher.ResolveLikelyNameFromRawLine(rawLine));
        foreach (var participant in participants)
        {
            matcher.MatchParticipant(participant);
        }

        ApplyHints(participants);
        return participants;
    }

    private void ApplyHints(IReadOnlyList<VM.Participant> participants)
    {
        var docPaths = participants
            .Where(p => !string.IsNullOrWhiteSpace(p.DocumentPath))
            .Select(p => p.DocumentPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (docPaths.Count == 0)
        {
            return;
        }

        var byPath = _hints.LoadActiveDisplaysForDocuments(docPaths);
        foreach (var participant in participants)
        {
            participant.ActiveHints =
                !string.IsNullOrWhiteSpace(participant.DocumentPath) &&
                byPath.TryGetValue(participant.DocumentPath, out var hints)
                    ? hints
                    : Array.Empty<VM.ParticipantHintDisplay>();
        }
    }
}
