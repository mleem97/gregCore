/// <file-summary>
/// Layer:       Core
/// Purpose:      Exception for errors inside language bridges (Lua/JS/FFI).
/// Maintainer:   Should be caught and logged in isolation, never crashes the main thread.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregBridgeException : GregCoreException
{
    public GregBridgeException(string message) : base(message) { }
    public GregBridgeException(string message, Exception inner) : base(message, inner) { }
}
