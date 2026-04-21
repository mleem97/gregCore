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
    private readonly Dictionary<string, ModMetadata> _registeredMods = new();

    public GregPluginRegistry(IAssemblyScanner scanner, IGregLogger logger, IGregEventBus eventBus)
    {
        _scanner = scanner;
        _logger = logger.ForContext("PluginRegistry");
        _eventBus = eventBus;
    }

    public void RegisterMod(ModMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.ModId))
        {
            _logger.Error("Mod-Registrierung fehlgeschlagen: ModId ist leer.");
            return;
        }

        _registeredMods[metadata.ModId] = metadata;
        _logger.Info($"Mod registriert: {metadata.Name} ({metadata.Version}) [ID: {metadata.ModId}]");
    }

    public ModMetadata? GetModMetadata(string modId)
    {
        _registeredMods.TryGetValue(modId, out var metadata);
        return metadata;
    }

    public IEnumerable<ModMetadata> GetAllRegisteredMods() => _registeredMods.Values;

    public void LoadAll()
    {
        _logger.Info("Lade alle Plugins...");
        var plugins = _scanner.ScanDirectory("Mods");
        _loadedPlugins.AddRange(plugins);
        _logger.Info($"{_loadedPlugins.Count} Plugins geladen.");
    }

    public IReadOnlyList<PluginInfo> GetLoadedPlugins() => _loadedPlugins.AsReadOnly();
}
