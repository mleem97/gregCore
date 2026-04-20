/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Löst Mod-Abhängigkeiten auf und bestimmt Load-Order.
/// Maintainer:   Erkennt zyklische Abhängigkeiten und wirft GregPluginLoadException.
/// </file-summary>

namespace gregCore.Infrastructure.Plugins;

public sealed class GregDependencyResolver
{
    public IReadOnlyList<PluginInfo> Resolve(IReadOnlyList<PluginInfo> plugins)
    {
        // Topological Sort Placeholder
        return plugins.ToList();
    }
}
