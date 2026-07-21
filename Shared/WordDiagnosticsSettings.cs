namespace XHub.Shared;

public static class WordDiagnosticsSettings
{
    public static bool Enabled { get; private set; }

    public static void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }
}
