namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Konfiguration für das gregCore Logging-System.
/// </summary>
public sealed class ConsoleConfig
{
    public bool ShowTimestamps { get; set; } = true;
    public bool UseBoxDrawing { get; set; } = true;
    public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;
}

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Status
}
