/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Verwaltet alle registrierten Mods und Plugins.
/// Maintainer:   Verantwortlich für Lifecycle (Load, Initialize, Unload).
/// </file-summary>

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;
using gregCore.PublicApi;
using gregCore.PublicApi.Attributes;
using gregCore.Core.Events;
using System.Reflection;

namespace gregCore.Infrastructure.Plugins;

public sealed class GregPluginRegistry : IGregPluginRegistry
{
    private readonly IAssemblyScanner _scanner;
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;
    private readonly List<PluginInfo> _loadedPlugins = new();
    private readonly Dictionary<string, ModMetadata> _registeredMods = new();
    private readonly Dictionary<string, LoadedMod> _runtimeMods = new(StringComparer.OrdinalIgnoreCase);
    private GregApiContext? _context;

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

        if (string.IsNullOrEmpty(metadata.PersistentId))
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(metadata.ModId));
            var guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);
            metadata.PersistentId = new Guid(guidBytes).ToString();
        }

        _registeredMods[metadata.ModId] = metadata;
        _logger.Info($"Mod registriert: {metadata.Name} ({metadata.Version}) [ID: {metadata.ModId}, PersistentID: {metadata.PersistentId}]");
    }

    public void Configure(GregApiContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    public ModMetadata? GetModMetadata(string modId)
    {
        _registeredMods.TryGetValue(modId, out var metadata);
        return metadata;
    }

    public IEnumerable<ModMetadata> GetAllRegisteredMods() => _registeredMods.Values;

    public void LoadAll()
    {
        _logger.Info("Lade alle Plugins...");
        var path = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory);
        var plugins = new GregDependencyResolver().Resolve(_scanner.ScanDirectory(path)
            .Where(p => !string.IsNullOrWhiteSpace(p.Manifest.Id)).ToArray());
        foreach (var plugin in plugins)
        {
            if (_loadedPlugins.Any(p => string.Equals(p.Manifest.Id, plugin.Manifest.Id, StringComparison.OrdinalIgnoreCase))) continue;
            _loadedPlugins.Add(plugin);
            if (_context != null) LoadRuntimeMod(plugin);
        }
        _logger.Info($"{_loadedPlugins.Count} Plugins geladen.");
    }

    public void Update(float deltaTime)
    {
        foreach (var mod in _runtimeMods.Values.ToArray())
            Safe(mod, () => mod.Instance.OnUpdate(deltaTime), "OnUpdate");
    }

    public void SceneLoaded(string sceneName)
    {
        foreach (var mod in _runtimeMods.Values.ToArray())
            Safe(mod, () => mod.Instance.OnSceneLoaded(sceneName), "OnSceneLoaded");
    }

    public void Shutdown()
    {
        _context?.LifetimeSource?.Cancel();
        foreach (var id in _runtimeMods.Keys.ToArray()) Unload(id);
    }

    public bool Unload(string modId)
    {
        if (!_runtimeMods.Remove(modId, out var mod)) return false;
        Safe(mod, mod.Instance.OnShutdown, "OnShutdown");
        mod.Dispose();
        return true;
    }

    private void LoadRuntimeMod(PluginInfo plugin)
    {
        if (_context == null || string.IsNullOrWhiteSpace(plugin.ModTypeName)) return;
        try
        {
            var assembly = Assembly.LoadFrom(plugin.AssemblyPath);
            var type = assembly.GetType(plugin.ModTypeName, throwOnError: true)!;
            if (!typeof(GregMod).IsAssignableFrom(type))
                throw new GregPluginLoadException($"{plugin.Manifest.Id}: entry type does not derive from GregMod.");
            var instance = (GregMod?)Activator.CreateInstance(type) ?? throw new GregPluginLoadException($"{plugin.Manifest.Id}: could not create entry type.");
            instance.Initialize(_context);
            var loaded = new LoadedMod(plugin.Manifest.Id, instance);
            _runtimeMods.Add(plugin.Manifest.Id, loaded);
            Safe(loaded, instance.OnLoad, "OnLoad");
            Safe(loaded, instance.OnReady, "OnReady");
            _logger.Info($"Mod ready: {plugin.Manifest.Id} ({plugin.Manifest.Version})");
        }
        catch (Exception ex)
        {
            _logger.Error($"Mod load failed: {plugin.Manifest.Id}", ex);
        }
    }

    private void Safe(LoadedMod mod, Action action, string phase)
    {
        try { action(); }
        catch (Exception ex) { _logger.Error($"Mod {mod.Id} {phase} failed", ex); }
    }

    private sealed class LoadedMod : IDisposable
    {
        public string Id { get; }
        public GregMod Instance { get; }
        public LoadedMod(string id, GregMod instance) { Id = id; Instance = instance; }
        public void Dispose()
        {
            Instance.DisposeSubscriptions();
            Instance.DisposeResources();
        }
    }

    public IReadOnlyList<PluginInfo> GetLoadedPlugins() => _loadedPlugins.AsReadOnly();
}
