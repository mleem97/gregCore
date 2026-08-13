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
        ArgumentNullException.ThrowIfNull(plugins);
        var byId = new Dictionary<string, PluginInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var plugin in plugins.OrderBy(p => p.Manifest.Id, StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(plugin.Manifest.Id))
                throw new GregPluginLoadException($"Plugin '{plugin.AssemblyPath}' has no manifest id.");
            if (!byId.TryAdd(plugin.Manifest.Id, plugin))
                throw new GregPluginLoadException($"Duplicate plugin id '{plugin.Manifest.Id}'.");
        }
        var state = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        var result = new List<PluginInfo>(plugins.Count);
        var stack = new Stack<string>();
        foreach (var id in byId.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)) Visit(id);
        return result;

        void Visit(string id)
        {
            if (state.TryGetValue(id, out var value))
            {
                if (value == 2) return;
                throw new GregPluginLoadException($"Cyclic plugin dependency: {string.Join(" -> ", stack.Reverse().Append(id))}.");
            }
            if (!byId.TryGetValue(id, out var plugin))
                throw new GregPluginLoadException($"Missing plugin dependency '{id}'.");
            state[id] = 1;
            stack.Push(id);
            foreach (var dependency in plugin.Manifest.Dependencies.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                var dependencyId = dependency.Split('@', 2)[0].Trim();
                if (!byId.ContainsKey(dependencyId))
                    throw new GregPluginLoadException($"Plugin '{id}' requires missing dependency '{dependencyId}'.");
                Visit(dependencyId);
            }
            stack.Pop();
            state[id] = 2;
            result.Add(plugin);
        }
    }
}
