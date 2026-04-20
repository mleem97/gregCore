/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Verwaltet alle registrierten Mods und Plugins.
/// Maintainer:   Verantwortlich für Lifecycle (Load, Initialize, Unload).
/// </file-summary>

namespace gregCore.Infrastructure.Plugins;

public sealed class GregPluginRegistry : IGregPluginRegistry
{
    private readonly IAssemblyScanner _scanner;
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;
    private readonly List<PluginInfo> _loadedPlugins = new();

    public GregPluginRegistry(IAssemblyScanner scanner, IGregLogger logger, IGregEventBus eventBus)
    {
        _scanner = scanner;
        _logger = logger.ForContext("PluginRegistry");
        _eventBus = eventBus;
    }

    public void LoadAll()
    {
        _logger.Info("Lade alle Plugins...");
        var plugins = _scanner.ScanDirectory("Mods");
        _loadedPlugins.AddRange(plugins);
        _logger.Info($"{_loadedPlugins.Count} Plugins geladen.");
    }

    public IReadOnlyList<PluginInfo> GetLoadedPlugins() => _loadedPlugins.AsReadOnly();
}
