/// <file-summary>
/// Schicht:      Core
/// Zweck:        Exception für ABI (Application Binary Interface) Mismatches bei nativen Mods.
/// Maintainer:   Wird geworfen, wenn GameAPITable Versionen nicht übereinstimmen.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregAbiException : GregCoreException
{
    public GregAbiException(string message) : base(message) { }
    public GregAbiException(string message, Exception inner) : base(message, inner) { }
}
