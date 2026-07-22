using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VM = VerlaufsakteApp.Models;
using VS = VerlaufsakteApp.Services;

namespace XHub.Scola;

/// <summary>
/// Batch-Eintraege (BU/BI/LB). Bekommt die aktiven TN aus dem Tray des
/// <see cref="ScolaPanel"/> und setzt pro TN eine Zeile in die Tabellen-Bookmark
/// der Akte (Scolas Word-Engine ueber <see cref="BatchEntryService"/>).
/// </summary>
public partial class BatchPanel : UserControl
{
    private const string BuTableBookmark = "BU_BILDUNG_TABELLE";
    private const string BiTableBookmark = "BI_BERUFSINTEGRATION_TABELLE";
    private const string LbTableBookmark = "LB_LEHRBEGLEITUNG_TABELLE";
    private const string BiTodoBookmark = "BI_BERUFSINTEGRATION_TODO";

    // App-weiter Scola-Host (wird beim Beenden zentral entsorgt), kein eigener STA-Thread pro Panel.
    private readonly VS.WordStaHost _host = App.ScolaWordStaHost;
    private readonly BatchEntryService _service;

    private readonly ObservableCollection<BatchResultLine> _buResults = new();
    private readonly ObservableCollection<BatchResultLine> _biResults = new();
    private readonly ObservableCollection<BatchResultLine> _lbResults = new();
    private readonly ObservableCollection<VM.BiTodoCollectResult> _biTodoResults = new();

    private IReadOnlyList<VM.Participant> _participants = Array.Empty<VM.Participant>();
    private bool _busy;

    public BatchPanel()
    {
        InitializeComponent();
        _service = new BatchEntryService(_host);
        BuResults.ItemsSource = _buResults;
        BiResults.ItemsSource = _biResults;
        LbResults.ItemsSource = _lbResults;
        BiTodoResults.ItemsSource = _biTodoResults;

        // Beim Einfuegen (z. B. aus einem ChatGPT-Code-Fenster) Markdown-Code-Fences
        // (```text, ```, ```csv …) direkt entfernen, damit sie gar nicht erst im Feld
        // erscheinen – so wie Word sie beim Einfuegen weglaesst.
        DataObject.AddPastingHandler(BuInput, OnBatchInputPaste);
        DataObject.AddPastingHandler(BiInput, OnBatchInputPaste);
        DataObject.AddPastingHandler(LbInput, OnBatchInputPaste);
    }

    private void OnBatchInputPaste(object sender, DataObjectPastingEventArgs e)
    {
        var text = e.DataObject.GetData(DataFormats.UnicodeText) as string
                   ?? e.DataObject.GetData(DataFormats.Text) as string;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var cleaned = SeparateEntries(StripCodeFences(text));
        if (string.Equals(cleaned, text, StringComparison.Ordinal))
        {
            return;
        }

        var replacement = new DataObject();
        replacement.SetData(DataFormats.UnicodeText, cleaned);
        e.DataObject = replacement;
    }

    /// <summary>Entfernt Zeilen, die (getrimmt) mit ``` beginnen (Markdown-Code-Fences).</summary>
    private static string StripCodeFences(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var kept = lines.Where(l => !l.Trim().StartsWith("```", StringComparison.Ordinal));
        return string.Join(Environment.NewLine, kept);
    }

    private static readonly Regex EntryDateLine = new(@"^\s*\d{1,2}\.\d{1,2}\.\d{2,4}", RegexOptions.Compiled);

    /// <summary>
    /// Minimale Lesbarkeits-Formatierung: vor jedem neuen Eintrag (Zeile, die mit einem
    /// Datum beginnt) eine Leerzeile einfuegen. Leerzeilen ignoriert der Batch-Parser,
    /// die Zeilenzahl bleibt also korrekt.
    /// </summary>
    private static string SeparateEntries(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n')
            .Select(l => l.TrimEnd())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        var builder = new StringBuilder();
        for (var i = 0; i < lines.Count; i++)
        {
            if (i > 0 && EntryDateLine.IsMatch(lines[i]))
            {
                builder.Append(Environment.NewLine);
            }

            builder.Append(lines[i]);
            if (i < lines.Count - 1)
            {
                builder.Append(Environment.NewLine);
            }
        }

        return builder.ToString();
    }

    /// <summary>Aktuelle TN aus dem Tray (wird vom Hauptfenster gesetzt).</summary>
    public void SetParticipants(IReadOnlyList<VM.Participant> participants)
    {
        _participants = participants ?? Array.Empty<VM.Participant>();
    }

    private void Section_OnToggle(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Primitives.ToggleButton { Tag: FrameworkElement body } toggle)
        {
            body.Visibility = toggle.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void ClearBatchInput_OnClick(object sender, RoutedEventArgs e)
    {
        if (_busy || sender is not Button { Tag: string kind })
        {
            return;
        }

        var (input, status, results) = kind switch
        {
            "BU" => (BuInput, BuStatus, _buResults),
            "BI" => (BiInput, BiStatus, _biResults),
            "LB" => (LbInput, LbStatus, _lbResults),
            _ => (BuInput, BuStatus, _buResults)
        };

        input.Clear();
        results.Clear();
        status.Text = string.Empty;
    }

    private async void RunBatch_OnClick(object sender, RoutedEventArgs e)
    {
        if (_busy || sender is not Button { Tag: string kind })
        {
            return;
        }

        var (bookmark, input, status, results) = kind switch
        {
            "BU" => (BuTableBookmark, BuInput, BuStatus, _buResults),
            "BI" => (BiTableBookmark, BiInput, BiStatus, _biResults),
            "LB" => (LbTableBookmark, LbInput, LbStatus, _lbResults),
            _ => (string.Empty, BuInput, BuStatus, _buResults)
        };

        if (string.IsNullOrEmpty(bookmark))
        {
            return;
        }

        // Nur angehakte (anwesende) TN kommen in den Batch. Deaktivierte TN
        // (Haken draussen) duerfen nie als aktiv gelten.
        var eligible = _participants
            .Where(p => p.IsPresent && p.MatchStatus == VM.MatchStatus.Found && !string.IsNullOrWhiteSpace(p.DocumentPath))
            .ToList();

        if (eligible.Count == 0)
        {
            status.Text = "Keine angehakten (anwesenden) TN mit Akte im Tray.";
            return;
        }

        var rows = input.Text
            .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
            .Select(l => l.Trim())
            // Markdown-Code-Fences (```text, ```, ```csv …) rausfiltern, die beim
            // Kopieren aus einem ChatGPT-Code-Fenster mitkommen und sonst faelschlich
            // als Zeilen zaehlen.
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("```", StringComparison.Ordinal))
            .ToList();

        if (rows.Count != eligible.Count)
        {
            status.Text = $"Zeilen ({rows.Count}) ≠ aktive TN ({eligible.Count}).";
            return;
        }

        var normalized = new List<string>();
        for (var i = 0; i < rows.Count; i++)
        {
            if (!BatchEntryService.TryNormalizeRow(rows[i], out var row, out var error))
            {
                status.Text = $"Zeile {i + 1}: {error}";
                return;
            }

            normalized.Add(row);
        }

        var mapping = eligible.Zip(normalized, (participant, row) => (participant, row)).ToList();

        // Zuordnung bestaetigen lassen, bevor in die Akten geschrieben wird.
        var confirmRows = mapping.Select((pair, i) =>
        {
            var cols = pair.row.Split('\t');
            return new BatchConfirmRow
            {
                Index = i + 1,
                Name = pair.participant.FullName,
                Date = cols.Length > 0 ? cols[0] : string.Empty,
                Initials = cols.Length > 1 ? cols[1] : string.Empty,
                Type = cols.Length > 2 ? cols[2] : string.Empty,
                Text = cols.Length > 3 ? cols[3] : string.Empty
            };
        }).ToList();

        if (!BatchConfirmWindow.Confirm(Window.GetWindow(this), $"{kind}-Zuordnung prüfen", confirmRows))
        {
            status.Text = "Abgebrochen.";
            return;
        }

        // Auch der Batch respektiert den globalen Word-Sperrmechanismus, damit
        // waehrend eines laufenden Batches keine Einzelaktion dazwischenfunkt.
        if (!XHub.Services.WordBusyGuard.TryEnter())
        {
            status.Text = "Eine Word-Aktion läuft bereits. Bitte kurz warten.";
            return;
        }

        _busy = true;
        results.Clear();
        var progress = new Progress<string>(text => status.Text = text);
        try
        {
            var lines = await _service.RunAsync(bookmark, mapping, progress, CancellationToken.None);
            foreach (var line in lines)
            {
                results.Add(line);
            }

            var success = lines.Count(l => l.IsSuccess);
            var failed = lines.Count - success;
            status.Text = $"Fertig: {success} ✓ / {failed} ✗";
        }
        catch (Exception ex)
        {
            status.Text = $"Fehler: {ex.Message}";
        }
        finally
        {
            _busy = false;
            XHub.Services.WordBusyGuard.Exit();
        }
    }

    private async void RunBiTodo_OnClick(object sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var present = _participants.Where(p => p.IsPresent).ToList();
        if (present.Count == 0)
        {
            BiTodoStatus.Text = "Keine angehakten (anwesenden) TN im Tray.";
            return;
        }

        var requests = present.Select(p =>
        {
            var hasAkte = p.MatchStatus == VM.MatchStatus.Found && !string.IsNullOrWhiteSpace(p.DocumentPath);
            return new VM.BiTodoCollectRequest
            {
                FullName = p.FullName,
                Initials = p.Initials,
                DocumentPath = hasAkte ? p.DocumentPath : string.Empty,
                FailureMessage = hasAkte ? string.Empty : "kein passender Ordner"
            };
        }).ToList();

        if (!XHub.Services.WordBusyGuard.TryEnter())
        {
            BiTodoStatus.Text = "Eine Word-Aktion läuft bereits. Bitte kurz warten.";
            return;
        }

        _busy = true;
        _biTodoResults.Clear();
        BiTodoStatus.Text = $"Sammle {requests.Count} TN…";
        try
        {
            var title = $"BI, {DateTime.Now.ToString("dddd, dd.MM.yy", CultureInfo.GetCultureInfo("de-CH"))}";
            var summary = await _host.RunAsync(
                "CollectBiTodoDocument",
                service => service.CollectBiTodoDocument(requests, BiTodoBookmark, title));

            foreach (var result in summary.Results)
            {
                _biTodoResults.Add(result);
            }

            BiTodoStatus.Text = $"Fertig: {summary.SuccessCount} ✓ / {summary.FailureCount} ✗";
        }
        catch (Exception ex)
        {
            BiTodoStatus.Text = $"Fehler: {ex.Message}";
            var message = $"Der Sammellauf konnte nicht abgeschlossen werden.\n\n{ex.Message}";
            var owner = Window.GetWindow(this);
            if (owner is not null)
            {
                MessageBox.Show(owner, message, "BI: To-dos fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(message, "BI: To-dos fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        finally
        {
            _busy = false;
            XHub.Services.WordBusyGuard.Exit();
        }
    }
}
