/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Null-Logger Implementierung für Tests.
/// Maintainer:   Verwirft alle Logs.
/// </file-summary>

namespace gregCore.Infrastructure.Logging;

public sealed class NullLogger : IGregLogger
{
    public void Debug(string message) { }
    public void Info(string message) { }
    public void Warning(string message) { }
    public void Error(string message, Exception? ex = null) { }
    public IGregLogger ForContext(string context) => this;
}
