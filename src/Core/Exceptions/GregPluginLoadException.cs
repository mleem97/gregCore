/// <file-summary>
/// Schicht:      Core
/// Zweck:        Exception für Fehler beim Laden von Plugins oder Auflösen von Abhängigkeiten.
/// Maintainer:   Wird geworfen bei zyklischen oder fehlenden Abhängigkeiten.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregPluginLoadException : GregCoreException
{
    public GregPluginLoadException(string message) : base(message) { }
    public GregPluginLoadException(string message, Exception inner) : base(message, inner) { }
}
