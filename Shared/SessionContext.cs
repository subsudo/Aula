using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XHub.Shared;

/// <summary>
/// Einzige Quelle der Wahrheit fuer den app-uebergreifenden Zustand zwischen dem
/// Acta-Rueckgrat und den eingebetteten Scola-Funktionen. Wird in den
/// Integrationsstufen verdrahtet (in Scola markierte TN -> Acta-Detailbereich).
/// </summary>
public sealed class SessionContext : INotifyPropertyChanged
{
    private SharedParticipant? _selectedParticipant;

    /// <summary>Aktuell ausgewaehlter TN (steuert u. a. den Detailbereich).</summary>
    public SharedParticipant? SelectedParticipant
    {
        get => _selectedParticipant;
        set
        {
            if (Equals(_selectedParticipant, value))
            {
                return;
            }

            _selectedParticipant = value;
            OnPropertyChanged();
        }
    }

    /// <summary>In der Scola-Arbeitsliste angehakte/markierte TN.</summary>
    public ObservableCollection<SharedParticipant> MarkedInScola { get; } = new();

    public void MarkInScola(SharedParticipant participant)
    {
        if (participant is null || MarkedInScola.Contains(participant))
        {
            return;
        }

        MarkedInScola.Add(participant);
    }

    public void UnmarkInScola(SharedParticipant participant)
    {
        if (participant is null)
        {
            return;
        }

        MarkedInScola.Remove(participant);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
