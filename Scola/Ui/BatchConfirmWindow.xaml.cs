using System.Collections.Generic;
using System.Windows;

namespace XHub.Scola;

/// <summary>Eine Zeile der Batch-Zuordnung fuer die Bestaetigung.</summary>
public sealed class BatchConfirmRow
{
    public int Index { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Date { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}

/// <summary>
/// Modernes Bestaetigungsfenster: zeigt vor dem Schreiben die Zuordnung
/// Zeile -> Teilnehmer, damit Fehlzuordnungen auffallen.
/// </summary>
public partial class BatchConfirmWindow : Window
{
    public BatchConfirmWindow(string title, IReadOnlyList<BatchConfirmRow> rows)
    {
        InitializeComponent();
        TitleText.Text = title;
        Title = title;
        RowsList.ItemsSource = rows;
    }

    /// <summary>Zeigt den Dialog modal und gibt true zurueck, wenn bestaetigt.</summary>
    public static bool Confirm(Window? owner, string title, IReadOnlyList<BatchConfirmRow> rows)
    {
        var dialog = new BatchConfirmWindow(title, rows);
        if (owner is not null)
        {
            dialog.Owner = owner;
        }

        return dialog.ShowDialog() == true;
    }

    private void Confirm_OnClick(object sender, RoutedEventArgs e) => DialogResult = true;

    private void Cancel_OnClick(object sender, RoutedEventArgs e) => DialogResult = false;
}
