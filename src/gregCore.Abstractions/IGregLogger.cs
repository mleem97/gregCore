/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for logging.
/// Maintainer:   Sole contract for logs, decouples MelonLogger from the framework.
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
