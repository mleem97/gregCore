/// <file-summary>
/// Schicht:      Core
/// Zweck:        Exception für Initialisierungsfehler des Frameworks.
/// Maintainer:   Framework-eigene Exception, keine Unity-Abhängigkeit.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregInitException : GregCoreException
{
    public GregInitException(string message) : base(message) { }
    public GregInitException(string message, Exception inner) : base(message, inner) { }
}
