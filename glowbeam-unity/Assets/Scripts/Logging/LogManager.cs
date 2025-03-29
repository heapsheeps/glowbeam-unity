using UnityEngine;

public static class LogManager
{
    public static LogLevel MinimumLogLevel = LogLevel.Debug;

    public static void Log(LogLevel level, string message)
    {
        if (level < MinimumLogLevel) return;

        switch (level)
        {
            case LogLevel.Debug:
                Debug.Log($"[DEBUG] {message}");
                break;
            case LogLevel.Info:
                Debug.Log($"[INFO] {message}");
                break;
            case LogLevel.Warning:
                Debug.LogWarning($"[WARNING] {message}");
                break;
            case LogLevel.Error:
                Debug.LogError($"[ERROR] {message}");
                break;
        }
    }
}
