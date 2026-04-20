/// <file-summary>
/// Schicht:      Core
/// Zweck:        Basis-Exception für alle frameworkeigenen Exceptions.
/// Maintainer:   Alle eigenen Exceptions sollten hiervon erben.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public abstract class GregCoreException : Exception
{
    protected GregCoreException(string message) : base(message) { }
    protected GregCoreException(string message, Exception inner) : base(message, inner) { }
}
