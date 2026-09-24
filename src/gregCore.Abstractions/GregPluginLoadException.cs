/// <file-summary>
/// Layer:       Core
/// Purpose:      Exception for plugin load failures or unresolvable dependencies.
/// Maintainer:   Thrown on cyclic or missing dependencies.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public class GregPluginLoadException : GregCoreException
{
    public GregPluginLoadException(string message) : base(message) { }
    public GregPluginLoadException(string message, Exception inner) : base(message, inner) { }
}
