namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Configuration for the gregCore logging system.
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
