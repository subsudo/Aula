using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XHub.Models;
using XHub.Services;
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
    private const int MinRobustTokenCount = 2;
    private static readonly Regex TokenRegex = new(@"[\p{L}\p{N}]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ParentheticalRegex = new(@"\([^)]*\)", RegexOptions.Compiled);

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
    /// Parst und matcht gegen denselben bereits aufgebauten Index wie die
    /// Acta-Seite. Namensteile duerfen dabei in anderer Reihenfolge stehen.
    /// </summary>
    public List<VM.Participant> Import(
        string text,
        IReadOnlyList<ParticipantIndexEntry> participantIndex)
    {
        ArgumentNullException.ThrowIfNull(participantIndex);

        var entries = participantIndex
            .Where(entry => !string.IsNullOrWhiteSpace(entry.FolderPath))
            .DistinctBy(entry => entry.FolderPath, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var participants = _parser.Parse(text);
        foreach (var participant in participants)
        {
            MatchParticipant(participant, entries);
        }

        ApplyHints(participants);
        return participants;
    }

    private static void MatchParticipant(
        VM.Participant participant,
        IReadOnlyList<ParticipantIndexEntry> entries)
    {
        var resolution = ResolveParticipant(participant.FullName, entries);
        var candidatePaths = resolution.Matches
            .Select(entry => entry.FolderPath)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
        participant.CandidateFolderPaths = candidatePaths;

        if (resolution.Matches.Count == 0)
        {
            participant.MatchStatus = VM.MatchStatus.NotFound;
            participant.MatchedFolderPath = null;
            participant.SelectedFolderPath = null;
            participant.DocumentPath = string.Empty;
            participant.Initials = string.Empty;
            participant.OdooUrl = string.Empty;
            participant.CounselorInitials = string.Empty;
            participant.IsHeaderMetadataLoaded = false;
            VS.AppLogger.Warn($"ClassImport: Kein Treffer im Acta-Index fuer '{participant.FullName}'.");
            return;
        }

        if (resolution.Matches.Count > 1)
        {
            participant.MatchStatus = VM.MatchStatus.MultipleFound;
            participant.MatchedFolderPath = null;
            participant.SelectedFolderPath = null;
            participant.DocumentPath = string.Empty;
            participant.Initials = string.Empty;
            participant.OdooUrl = string.Empty;
            participant.CounselorInitials = string.Empty;
            participant.IsHeaderMetadataLoaded = false;
            VS.AppLogger.Warn($"ClassImport: Mehrere Treffer im Acta-Index fuer '{participant.FullName}'. Count={resolution.Matches.Count}.");
            return;
        }

        var match = resolution.Matches[0];
        if (resolution.WasTrimmed)
        {
            participant.FullName = resolution.CleanedName;
        }

        participant.MatchStatus = VM.MatchStatus.Found;
        participant.MatchedFolderPath = match.FolderPath;
        participant.SelectedFolderPath = match.FolderPath;
        participant.DocumentPath = match.DocumentPath;
        participant.Initials = match.Initials;
        participant.OdooUrl = match.OdooUrl;
        participant.CounselorInitials = match.CounselorInitials;
        participant.IsHeaderMetadataLoaded = true;
        VS.AppLogger.Info($"ClassImport: '{participant.FullName}' eindeutig ueber Acta-Index aufgeloest zu '{match.DisplayName}'.");
    }

    private static MatchResolution ResolveParticipant(
        string rawName,
        IReadOnlyList<ParticipantIndexEntry> entries)
    {
        var orderedTokens = TokenizeOrdered(rawName);
        for (var end = orderedTokens.Count; end >= MinRobustTokenCount; end--)
        {
            var prefix = orderedTokens.Take(end).ToList();
            var matches = FindMatches(prefix, entries);
            if (matches.Count == 0)
            {
                continue;
            }

            return new MatchResolution(
                matches,
                string.Join(" ", prefix),
                WasTrimmed: end < orderedTokens.Count);
        }

        return new MatchResolution(Array.Empty<ParticipantIndexEntry>(), rawName, WasTrimmed: false);
    }

    private static IReadOnlyList<ParticipantIndexEntry> FindMatches(
        IReadOnlyList<string> tokens,
        IReadOnlyList<ParticipantIndexEntry> entries)
    {
        var required = SearchTextUtility.BuildTokenCounts(tokens);
        if (required.Values.Sum() < MinRobustTokenCount)
        {
            return Array.Empty<ParticipantIndexEntry>();
        }

        var matches = entries
            .Where(entry => SearchTextUtility.HasTokenCountsMatch(required, entry.SearchTokens))
            .ToList();
        if (matches.Count > 0)
        {
            return matches;
        }

        required = SearchTextUtility.BuildTokenCounts(
            tokens.Select(SearchTextUtility.ReplaceUmlauts));
        return entries
            .Where(entry => SearchTextUtility.HasTokenCountsMatch(required, entry.SearchTokensFallback))
            .ToList();
    }

    private static List<string> TokenizeOrdered(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<string>();
        }

        return TokenRegex.Matches(ParentheticalRegex.Replace(value, " "))
            .Select(match => match.Value.Trim())
            .Where(token => token.Length > 0)
            .ToList();
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

    private sealed record MatchResolution(
        IReadOnlyList<ParticipantIndexEntry> Matches,
        string CleanedName,
        bool WasTrimmed);
}
