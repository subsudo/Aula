using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VM = VerlaufsakteApp.Models;
using VS = VerlaufsakteApp.Services;

namespace XHub.Scola;

/// <summary>Ergebnis eines einzelnen Batch-Eintrags.</summary>
public sealed class BatchResultLine
{
    public string Name { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Orchestriert die Batch-Eintraege (BU/BI/LB) ueber Scolas Word-Engine:
/// pro TN eine normalisierte Zeile in die Tabellen-Bookmark der Akte einfuegen.
/// Nachbau von Scolas ExecuteEntryBatchAsync, entkoppelt von der UI.
/// </summary>
public sealed class BatchEntryService
{
    private static readonly Regex NonBreakingSpaces = new("[   ]", RegexOptions.Compiled);

    private readonly VS.WordStaHost _host;

    public BatchEntryService(VS.WordStaHost host)
    {
        _host = host;
    }

    public async Task<IReadOnlyList<BatchResultLine>> RunAsync(
        string bookmarkName,
        IReadOnlyList<(VM.Participant Participant, string Row)> mapping,
        IProgress<string>? progress,
        CancellationToken token)
    {
        var results = new List<BatchResultLine>();

        foreach (var (participant, row) in mapping)
        {
            token.ThrowIfCancellationRequested();
            progress?.Report($"Verarbeite {participant.FullName}…");

            var docPath = participant.DocumentPath;
            if (string.IsNullOrWhiteSpace(docPath))
            {
                results.Add(new BatchResultLine { Name = participant.FullName, IsSuccess = false, Message = "keine Akte" });
                continue;
            }

            try
            {
                await _host.RunAsync(
                    $"Batch {bookmarkName}",
                    service => service.InsertTextRowToTable(docPath, bookmarkName, row, bringToForeground: false));
                results.Add(new BatchResultLine { Name = participant.FullName, IsSuccess = true, Message = "Eintrag eingefügt" });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (VS.WordTemplateValidationException ex) when (ex.Kind == VS.WordTemplateValidationErrorKind.BookmarkMissing)
            {
                results.Add(new BatchResultLine { Name = participant.FullName, IsSuccess = false, Message = "Bookmark fehlt" });
            }
            catch (VS.WordTemplateValidationException ex)
            {
                results.Add(new BatchResultLine { Name = participant.FullName, IsSuccess = false, Message = ex.UserMessage });
            }
            catch (Exception ex)
            {
                results.Add(new BatchResultLine { Name = participant.FullName, IsSuccess = false, Message = ex.Message });
            }
        }

        return results;
    }

    /// <summary>
    /// Normalisiert eine Roh-Zeile zu "Datum\tKuersel\tUnterrichtsart\tText".
    /// Portiert aus Scolas TryNormalizeBatchRow.
    /// </summary>
    public static bool TryNormalizeRow(string rawRow, out string normalizedRow, out string error)
    {
        normalizedRow = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(rawRow))
        {
            error = "Zeile ist leer.";
            return false;
        }

        var line = NonBreakingSpaces.Replace(rawRow, " ").Trim();

        if (line.Contains('\t'))
        {
            var parts = line.Split('\t').Select(p => p.Trim()).ToList();
            if (parts.Count < 4)
            {
                error = "Zu wenige Spalten (mindestens 4 erwartet).";
                return false;
            }

            var fourth = string.Join(" ", parts.Skip(3).Where(p => !string.IsNullOrWhiteSpace(p)));
            if (string.IsNullOrWhiteSpace(fourth))
            {
                fourth = "-";
            }

            normalizedRow = $"{parts[0]}\t{parts[1]}\t{parts[2]}\t{fourth}";
            return true;
        }

        var tokens = Regex.Split(line, @"\s+").Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        if (tokens.Count < 4)
        {
            error = "Nicht genügend Felder (Format: Datum Kürzel Unterrichtsart Text).";
            return false;
        }

        var rest = string.Join(" ", tokens.Skip(3)).Trim();
        if (string.IsNullOrWhiteSpace(rest))
        {
            error = "Spalte 4 (Eintragstext) fehlt.";
            return false;
        }

        normalizedRow = $"{tokens[0]}\t{tokens[1]}\t{tokens[2]}\t{rest}";
        return true;
    }
}
