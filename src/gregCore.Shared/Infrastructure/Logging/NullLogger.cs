/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Null logger implementation for tests.
/// Maintainer:  Discards all logs.
/// </file-summary>

namespace gregCore.Infrastructure.Logging;

public sealed class NullLogger : IGregLogger
{
    public void Debug(string message) { /* Intentionally empty: null logger discards all output (tests). */ }
    public void Info(string message) { /* Intentionally empty: see Debug. */ }
    public void Success(string message) { /* Intentionally empty: see Debug. */ }
    public void Warning(string message) { /* Intentionally empty: see Debug. */ }
    public void Error(string message, Exception? ex = null) { /* Intentionally empty: see Debug. */ }
    public IGregLogger ForContext(string context) => this;
}
