namespace XHub.Shared;

/// <summary>
/// Schlanke, app-uebergreifende TN-Referenz. Der Schluessel ist der Pfad der
/// Verlaufsakte-.docx, weil daran auch die Bemerkungen haengen (gemeinsamer
/// Identifikator zwischen Acta-Teil und Scola-Teil).
/// </summary>
public sealed class SharedParticipant
{
    public SharedParticipant(string documentPath, string displayName, string initials = "")
    {
        DocumentPath = documentPath ?? string.Empty;
        DisplayName = displayName ?? string.Empty;
        Initials = initials ?? string.Empty;
    }

    /// <summary>Stabiler, app-uebergreifender Schluessel = Pfad der Verlaufsakte.</summary>
    public string DocumentPath { get; }

    public string DisplayName { get; }

    public string Initials { get; }

    public override bool Equals(object? obj) =>
        obj is SharedParticipant other &&
        string.Equals(DocumentPath, other.DocumentPath, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(DocumentPath);
}
