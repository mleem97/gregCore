/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Adapter für MelonLoader's Logger.
/// Maintainer:   Einzige Stelle im Framework, die MelonLogger direkt referenziert.
/// </file-summary>

using MelonLoader;

namespace gregCore.Infrastructure.Logging;

public sealed class MelonLoggerAdapter : IGregLogger
{
    private readonly MelonLogger.Instance _melonLogger;
    private readonly string _prefix;

    public MelonLoggerAdapter(MelonLogger.Instance melonLogger, string prefix = "")
    {
        ArgumentNullException.ThrowIfNull(melonLogger);
        _melonLogger = melonLogger;
        _prefix = string.IsNullOrEmpty(prefix) ? "" : $"[{prefix}] ";
    }

    public void Debug(string message) => _melonLogger.Msg(ConsoleColor.Gray, $"{_prefix}{message}");
    public void Info(string message) => _melonLogger.Msg(ConsoleColor.White, $"{_prefix}{message}");
    public void Warning(string message) => _melonLogger.Warning($"{_prefix}{message}");
    public void Error(string message, Exception? ex = null)
    {
        if (ex != null) _melonLogger.Error($"{_prefix}{message}\n{ex}");
        else _melonLogger.Error($"{_prefix}{message}");
    }

    public IGregLogger ForContext(string context) => 
        new MelonLoggerAdapter(_melonLogger, string.IsNullOrEmpty(_prefix) ? context : $"{_prefix.Trim('[', ']')}::{context}");
}
