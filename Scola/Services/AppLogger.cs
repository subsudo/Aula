namespace VerlaufsakteApp.Services;

public static class AppLogger
{
    private static bool _debugEnabled;

    public static string LogDirectoryPath => XHub.Services.AppLogger.LogDirectoryPath;
    public static string CurrentLogFilePath => XHub.Services.AppLogger.CurrentLogFilePath;

    public static void Info(string message)
    {
        XHub.Services.AppLogger.Info($"Scola.{message}");
    }

    public static void Debug(string message)
    {
        if (_debugEnabled)
        {
            XHub.Services.AppLogger.Info($"Scola.DEBUG {message}");
        }
    }

    public static void Warn(string message)
    {
        XHub.Services.AppLogger.Warn($"Scola.{message}");
    }

    public static void Error(string message, Exception? exception = null)
    {
        XHub.Services.AppLogger.Error($"Scola.{message}", exception);
    }

    public static void SetDebugEnabled(bool enabled)
    {
        _debugEnabled = enabled;
        XHub.Services.AppLogger.Info($"Scola.Debug-Logging {(enabled ? "aktiviert" : "deaktiviert")}. ");
    }
}
