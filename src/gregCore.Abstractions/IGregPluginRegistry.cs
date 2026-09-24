/// <file-summary>
/// Layer:       Core
/// Purpose:     Interface for registering mods/plugins.
/// Maintainer:  Manages the lifecycle of all loaded plugins.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregPluginRegistry
{
    void LoadAll();
    IReadOnlyList<PluginInfo> GetLoadedPlugins();
}
