/// <file-summary>
/// Schicht:      Core
/// Zweck:        Exception für Fehler innerhalb von Language Bridges (Lua/JS/FFI).
/// Maintainer:   Sollte abgefangen und isoliert geloggt werden, crasht nie den Main Thread.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregBridgeException : GregCoreException
{
    public GregBridgeException(string message) : base(message) { }
    public GregBridgeException(string message, Exception inner) : base(message, inner) { }
}
