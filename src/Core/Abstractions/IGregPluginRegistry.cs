/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für die Registrierung von Mods/Plugins.
/// Maintainer:   Verwaltet den Lifecycle aller geladenen Plugins.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregPluginRegistry
{
    void LoadAll();
    IReadOnlyList<PluginInfo> GetLoadedPlugins();
}
