/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für Logging.
/// Maintainer:   Einziger Vertrag für Logs, entkoppelt MelonLogger vom Framework.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregLogger
{
    void Debug(string message);
    void Info(string message);
    void Success(string message);
    void Warning(string message);
    void Error(string message, Exception? ex = null);
    IGregLogger ForContext(string context);
}
