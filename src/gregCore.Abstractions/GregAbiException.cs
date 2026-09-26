/// <file-summary>
/// Layer:       Core
/// Purpose:      Exception for ABI (Application Binary Interface) mismatches in native mods.
/// Maintainer:   Thrown when GameAPITable versions do not match.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregAbiException : GregCoreException
{
    public GregAbiException(string message) : base(message) { }
    public GregAbiException(string message, Exception inner) : base(message, inner) { }
}
