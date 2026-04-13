using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;

namespace greg.Core.Diagnostic;

public static class GregSessionLogger
{
    private static string _sessionFilePath;
    private static readonly object _lock = new();

    public static void Initialize()
    {
        string logDir = Path.Combine(MelonEnvironment.UserDataDirectory, "gregCore", "Logs");
        if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _sessionFilePath = Path.Combine(logDir, $"session_{timestamp}.log");

        Log("=== GREG CORE SESSION START ===");
        Log($"OS: {Environment.OSVersion}");
        Log($"Runtime: {Environment.Version}");
        Log($"Engine: Unity 6 (6000.4.2f1)");
    }

    public static void Log(string message, string level = "INFO")
    {
        if (_sessionFilePath == null) return;

        lock (_lock)
        {
            try
            {
                string line = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
                File.AppendAllText(_sessionFilePath, line + Environment.NewLine);
            }
            catch { /* Ignore logging failures to prevent crashes */ }
        }
    }

    public static void LogError(string message, Exception ex = null)
    {
        Log($"{message} {(ex != null ? "\n" + ex.ToString() : "")}", "ERROR");
    }

    public static void LogVerbose(string message)
    {
        // Only log if verbose is enabled in config (placeholder for now)
        Log(message, "VERBOSE");
    }
}

