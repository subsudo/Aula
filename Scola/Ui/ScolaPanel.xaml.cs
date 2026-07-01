using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VM = VerlaufsakteApp.Models;

namespace XHub.Scola;

/// <summary>
/// Rechte Arbeitsflaeche von Aula (heller, schlichter Look): klappbarer
/// Klassen-Import mit mitwachsendem Textfeld, kompakter TN-Tray und oben ein
/// Umschalter, der das separate Batch-Panel ein-/ausblendet.
/// </summary>
public partial class ScolaPanel : UserControl
{
    private readonly ClassImportService _import = new();
    private readonly ObservableCollection<VM.Participant> _tray = new();

    public ScolaPanel()
    {
        InitializeComponent();
        TrayItems.ItemsSource = _tray;
        Loaded += ScolaPanel_OnLoaded;
    }

    private void ScolaPanel_OnLoaded(object sender, RoutedEventArgs e)
    {
#if DEBUG
        if (App.Config?.EnableDummyParticipants == true && _tray.Count == 0)
        {
            SeedDummyParticipants();
        }
#endif
    }

#if DEBUG
    private void SeedDummyParticipants()
    {
        _tray.Add(MakeDummy("Mustermann, Maximilian", "MaMu", true, ("#3CB371", "Anwesenheit i.O."), ("#D17878", "Offen")));
        _tray.Add(MakeDummy("Roth, Lea", "LeRo", true, ("#C8A96C", "Hinweis")));
        _tray.Add(MakeDummy("Abebe, Yonas", "YoAb", true));
        _tray.Add(MakeDummy("Schneider, Anna-Sophie", "AnSc", false, ("#3CB371", "a"), ("#D17878", "b"), ("#C8A96C", "c")));
        _tray.Add(MakeDummy("Li, Wei", "WeLi", true));
        UpdateTrayStatus();
        TrayChanged?.Invoke(this, EventArgs.Empty);
    }

    private static VM.Participant MakeDummy(string fullName, string initials, bool present,
        params (string Color, string Text)[] hints)
    {
        return new VM.Participant
        {
            FullName = fullName,
            Initials = initials,
            IsPresent = present,
            MatchStatus = VM.MatchStatus.Found,
            DocumentPath = string.Empty,
            MatchedFolderPath = string.Empty,
            ActiveHints = hints
                .Select(h => new VM.ParticipantHintDisplay { MarkerColor = h.Color, Text = h.Text })
                .ToList()
        };
    }
#endif

    /// <summary>Wird ausgeloest, wenn der Nutzer den Batch-Umschalter oben betaetigt.</summary>
    public event EventHandler? BatchToggleRequested;

    /// <summary>Wird ausgeloest, wenn ein TN im Tray angeklickt wird (zeigt ihn im Acta-Detail).</summary>
    public event EventHandler<VM.Participant>? ParticipantActivated;

    /// <summary>Wird ausgeloest, wenn sich der Tray-Inhalt aendert (Import / Entfernen).</summary>
    public event EventHandler? TrayChanged;

    /// <summary>Aktuelle TN im Tray (fuer den Batch-Bereich).</summary>
    public IReadOnlyList<VM.Participant> TrayParticipants => _tray;

    /// <summary>Wird ausgeloest, wenn am Tray-Chip eine Word-Aktion angefordert wird.</summary>
    public event EventHandler<TrayQuickActionEventArgs>? QuickActionRequested;

    private void TrayAction_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        if (sender is FrameworkElement { Tag: string actionKey, DataContext: VM.Participant participant })
        {
            QuickActionRequested?.Invoke(this, new TrayQuickActionEventArgs
            {
                Participant = participant,
                ActionKey = actionKey,
                IsNavigationOnly = Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
            });
        }
    }

    /// <summary>True, wenn der Batch-Bereich gerade angefordert (sichtbar) sein soll.</summary>
    public bool IsBatchRequested => BatchToggle.IsChecked == true;

    private void BatchToggle_OnToggle(object sender, RoutedEventArgs e)
    {
        BatchToggleRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ImportToggle_OnToggle(object sender, RoutedEventArgs e)
    {
        if (ImportBody is null)
        {
            return;
        }

        ImportBody.Visibility = ImportToggle.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void Chip_OnClick(object sender, MouseButtonEventArgs e)
    {
        if (IsInteractiveSource(e.OriginalSource))
        {
            return;
        }

        if (sender is FrameworkElement { DataContext: VM.Participant participant })
        {
            ParticipantActivated?.Invoke(this, participant);
        }
    }

    /// <summary>True, wenn der Klick aus einem Button/der Checkbox im Chip kommt (kein Detail/Drag).</summary>
    private static bool IsInteractiveSource(object? originalSource)
    {
        var current = originalSource as DependencyObject;
        while (current is not null)
        {
            if (current is System.Windows.Controls.Primitives.ButtonBase or CheckBox)
            {
                return true;
            }

            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
        }

        return false;
    }

    private Point _dragStart;
    private VM.Participant? _dragCandidate;

    private void Chip_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsInteractiveSource(e.OriginalSource))
        {
            _dragCandidate = null;
            return;
        }

        if (sender is FrameworkElement { DataContext: VM.Participant participant })
        {
            _dragStart = e.GetPosition(null);
            _dragCandidate = participant;
        }
    }

    private void Chip_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || _dragCandidate is null)
        {
            return;
        }

        var position = e.GetPosition(null);
        if (Math.Abs(position.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(position.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        var participant = _dragCandidate;
        _dragCandidate = null;
        var data = new DataObject("AulaScolaParticipant", participant);
        DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
    }

    private void ChipClose_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: VM.Participant participant })
        {
            _tray.Remove(participant);
            UpdateTrayStatus();
            TrayChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Presence_OnChanged(object sender, RoutedEventArgs e) => UpdateTrayStatus();

    private void ClearImport_OnClick(object sender, RoutedEventArgs e) => ImportInput.Clear();

    private void ClearTray_OnClick(object sender, RoutedEventArgs e)
    {
        _tray.Clear();
        UpdateTrayStatus();
        TrayChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateTrayStatus()
    {
        var total = _tray.Count;
        var present = _tray.Count(p => p.IsPresent);
        StatusText.Text = $"Auswertung: {total} Einträge ({present} anwesend, {total - present} abwesend)";
    }

    // --- Drop von der Acta-Seite: TN aus Actas Liste in den Tray ziehen ---

    private void Panel_OnDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent("AulaActaEntry") ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void Panel_OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("AulaActaEntry") is XHub.Models.ParticipantIndexEntry entry)
        {
            AddEntryToTray(entry);
            e.Handled = true;
        }
    }

    /// <summary>Fuegt einen Acta-Index-Eintrag als TN in den Tray ein (dedupliziert, alphabetisch).</summary>
    public void AddEntryToTray(XHub.Models.ParticipantIndexEntry entry)
    {
        var key = string.IsNullOrWhiteSpace(entry.DocumentPath) ? entry.FolderPath : entry.DocumentPath;
        var exists = _tray.Any(p =>
            string.Equals(string.IsNullOrWhiteSpace(p.DocumentPath) ? p.MatchedFolderPath : p.DocumentPath, key,
                StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            return;
        }

        _tray.Add(new VM.Participant
        {
            FullName = entry.DisplayName,
            DocumentPath = entry.DocumentPath,
            Initials = entry.Initials,
            MatchedFolderPath = entry.FolderPath,
            MatchStatus = VM.MatchStatus.Found,
            IsPresent = true,
            ActiveHints = _import.LoadHints(entry.DocumentPath)
        });

        SortTrayByName();
        UpdateTrayStatus();
        TrayChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Aktualisiert die farbigen Hinweis-Punkte der Tray-Kacheln fuer eine Akte,
    /// wenn deren Hinweise auf der Acta-Seite geaendert wurden (Echtzeit-Sync).
    /// </summary>
    public void RefreshHintsForDocument(string? documentPath)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
        {
            return;
        }

        foreach (var participant in _tray)
        {
            if (string.Equals(participant.DocumentPath, documentPath, StringComparison.OrdinalIgnoreCase))
            {
                participant.ActiveHints = _import.LoadHints(documentPath);
            }
        }
    }

    private void SortTrayByName()
    {
        var sorted = _tray.OrderBy(p => p.FullName, StringComparer.CurrentCultureIgnoreCase).ToList();
        _tray.Clear();
        foreach (var participant in sorted)
        {
            _tray.Add(participant);
        }
    }

    private async void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {
        var text = ImportInput.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var config = App.Config;
        var serverBasePath = config.ServerBasePath;
        var useSecondary = config.UseSecondaryServerBasePath;
        var secondary = config.SecondaryServerBasePath;
        var keyword = config.VerlaufsakteKeyword;

        ImportButton.IsEnabled = false;
        ImportButton.Content = "Importiere…";
        try
        {
            var participants = await Task.Run(() =>
                _import.Import(text, serverBasePath, useSecondary, secondary, keyword));

            _tray.Clear();
            foreach (var participant in participants)
            {
                _tray.Add(participant);
            }

            UpdateTrayStatus();
            TrayChanged?.Invoke(this, EventArgs.Empty);

            // Sobald die TN unten erscheinen, das Import-Fenster automatisch einklappen.
            ImportToggle.IsChecked = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Import fehlgeschlagen:\n{ex.Message}",
                "Klassen-Import",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        finally
        {
            ImportButton.IsEnabled = true;
            ImportButton.Content = "Importieren";
        }
    }
}

/// <summary>Daten fuer eine am Tray-Chip angeforderte Word-Aktion.</summary>
public sealed class TrayQuickActionEventArgs : EventArgs
{
    public VM.Participant Participant { get; init; } = null!;
    public string ActionKey { get; init; } = string.Empty;
    public bool IsNavigationOnly { get; init; }
}
