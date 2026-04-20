using System;
using MelonLoader;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Zentraler statischer Logger für gregCore. 
/// Routet alle Ausgaben einheitlich formatiert an den MelonLogger.
/// </summary>
public static class GregLogger
{
    private static ConsoleConfig _config = new();
    private static bool _isInitialized = false;

    public static void Configure(ConsoleConfig config)
    {
        _config = config;
        _isInitialized = true;
    }

    public static void Info(string component, string message) => Log("info", component, message);
    public static void Success(string component, string message) => Log("success", component, message);
    public static void Warning(string component, string message) => Log("warn", component, message);
    public static void Error(string component, string message) => Log("error", component, message);
    public static void Debug(string component, string message) => Log("debug", component, message);
    
    public static void Status(string message) 
    {
        if (_config.MinLogLevel > LogLevel.Status) return;
        
        // Status wird immer in Magenta ausgegeben
        string formatted = ConsoleFormatters.FormatStatusLine(message);
        MelonLogger.Msg(ConsoleColor.Magenta, formatted);
    }

    public static void BridgeInfo(string bridgeName, string message) => Info(bridgeName, message);
    public static void BridgeError(string bridgeName, string message) => Error(bridgeName, message);

    public static void Box(string[] lines)
    {
        string[] boxed = ConsoleFormatters.CreateBox(lines);
        foreach (var line in boxed)
        {
            MelonLogger.Msg(ConsoleColor.Cyan, line);
        }
    }

    private static void Log(string level, string component, string message)
    {
        // LogLevel check
        if (!IsLevelEnabled(level)) return;

        // Wir nutzen hier eine vereinfachte Version für die MelonLoader Terminal-Farben.
        // Um echte mehrfarbige Zeilen zu haben, müsste man direkt auf System.Console zugreifen,
        // was aber das MelonLoader-Logging (Log-Files) umgehen würde.
        // Daher nutzen wir die Level-Farbe für die gesamte Zeile, wie es in ML üblich ist.
        
        string prefix = ConsoleFormatters.CreatePrefix(level, component, _config.ShowTimestamps);
        string fullMsg = $"{prefix} {message}";
        
        var color = ConsoleTheme.GetLevelColor(level);

        if (level == "error" || level == "err")
            MelonLogger.Error(fullMsg);
        else if (level == "warn" || level == "warning")
            MelonLogger.Warning(fullMsg);
        else
            MelonLogger.Msg(color, fullMsg);
    }

    private static bool IsLevelEnabled(string level)
    {
        var current = level.ToLower() switch
        {
            "debug" or "dbg" => LogLevel.Debug,
            "info" => LogLevel.Info,
            "warn" or "warning" or "wrn" => LogLevel.Warning,
            "error" or "err" => LogLevel.Error,
            "status" => LogLevel.Status,
            _ => LogLevel.Info
        };

        return current >= _config.MinLogLevel;
    }
}
