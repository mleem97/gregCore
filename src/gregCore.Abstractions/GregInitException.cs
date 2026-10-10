/// <file-summary>
/// Layer:       Core
/// Purpose:      Exception for framework initialization failures.
/// Maintainer:   Framework-owned exception, no Unity dependency.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregInitException : GregCoreException
{
    public GregInitException(string message) : base(message) { }
    public GregInitException(string message, Exception inner) : base(message, inner) { }
}
