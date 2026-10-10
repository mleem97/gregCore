/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Loads GregCore-native DLL plugins from Mods/gregNative.
/// Maintainer:  Cecil discovery (no blind load), then Assembly.LoadFrom per hit.
/// </file-summary>

using System.Reflection;
using Mono.Cecil;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;
using gregCore.PublicApi;

namespace gregCore.Infrastructure.Plugins.Native;

public sealed class GregNativePluginLoader
{
    private const string PluginInterfaceFullName = "gregCore.PublicApi.IGregNativePlugin";

    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;
    private readonly Dictionary<string, Entry> _loaded = new(StringComparer.OrdinalIgnoreCase);

    public GregNativePluginLoader(IGregLogger logger, IGregEventBus eventBus)
    {
        _logger = logger.ForContext("GregNativePlugins");
        _eventBus = eventBus;
    }

    public IReadOnlyList<GregNativePluginSnapshot> GetLoaded() =>
        _loaded.Values
            .OrderBy(e => e.Plugin.Id, StringComparer.OrdinalIgnoreCase)
            .Select(e => new GregNativePluginSnapshot
            {
                Id = e.Plugin.Id,
                Name = e.Plugin.Name,
                Version = e.Plugin.Version,
                AssemblyPath = e.AssemblyPath
            })
            .ToArray();

    public int LoadAll(string nativeDir)
    {
        if (string.IsNullOrWhiteSpace(nativeDir)) return 0;
        if (!Directory.Exists(nativeDir)) return 0;

        var count = 0;
        foreach (var file in IO.GregFileSystem.EnumerateFilesByExtension(nativeDir, ".dll"))
            count += LoadFile(file, nativeDir);
        return count;
    }

    public bool Unload(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        if (!_loaded.Remove(id, out var entry)) return false;
        Safe(entry, static e => e.Plugin.OnNativeUnload(), "OnNativeUnload");
        SafeDispose(entry);
        return true;
    }

    public void Shutdown()
    {
        foreach (var id in _loaded.Keys.ToArray()) Unload(id);
    }

    private int LoadFile(string file, string nativeDir)
    {
        // Guard: never load from `.deactivated`.
        if (IO.GregDeactivatedGuard.IsDeactivatedPath(file)) return 0;
        var typeNames = DiscoverPluginTypes(file);
        if (typeNames.Count == 0) return 0;

        var loaded = 0;
        Assembly? assembly = null;
        foreach (var typeName in typeNames)
        {
            try
            {
                assembly ??= Assembly.LoadFrom(file);
                var type = assembly.GetType(typeName, throwOnError: true)!;
                if (type.IsAbstract || type.IsInterface) continue;
                var instance = Activator.CreateInstance(type) as IGregNativePlugin;
                if (instance == null)
                {
                    _logger.Warning($"{Path.GetFileName(file)}: {typeName} does not implement IGregNativePlugin.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(instance.Id))
                {
                    _logger.Warning($"{Path.GetFileName(file)}: Plugin ID is empty — skipped.");
                    continue;
                }
                if (_loaded.ContainsKey(instance.Id))
                {
                    _logger.Warning($"Duplicate native plugin ID '{instance.Id}' — skipped.");
                    continue;
                }
                var host = new Host(_logger.ForContext(instance.Id), _eventBus, nativeDir);
                instance.OnNativeLoad(host);
                _loaded.Add(instance.Id, new Entry(instance, file));
                _logger.Info($"Native plugin ready: {instance.Id} ({instance.Version})");
                loaded++;
            }
            catch (Exception ex)
            {
                _logger.Error($"Native plugin load failed: {Path.GetFileName(file)}:{typeName}", ex);
            }
        }
        return loaded;
    }

    private List<string> DiscoverPluginTypes(string file)
    {
        var result = new List<string>();
        try
        {
            using var module = ModuleDefinition.ReadModule(file);
            foreach (var type in module.Types.SelectMany(AllTypes))
            {
                try
                {
                    if (type.IsInterface || type.IsAbstract) continue;
                    if (type.Interfaces.Any(i => i.InterfaceType.FullName == PluginInterfaceFullName))
                        result.Add(type.FullName);
                }
                catch { /* skip single type */ }
            }
        }
        catch { /* not a valid .NET assembly */ }
        return result;
    }

    private static IEnumerable<TypeDefinition> AllTypes(TypeDefinition type)
    {
        yield return type;
        foreach (var nested in type.NestedTypes.SelectMany(AllTypes)) yield return nested;
    }

    private void Safe(Entry entry, Action<Entry> action, string phase)
    {
        try { action(entry); }
        catch (Exception ex) { _logger.Error($"Native plugin {entry.Plugin.Id} {phase} failed", ex); }
    }

    private static void SafeDispose(Entry entry)
    {
        try { entry.Plugin.Dispose(); }
        catch { /* Dispose must never break shutdown */ }
    }

    private sealed class Entry
    {
        public IGregNativePlugin Plugin { get; }
        public string AssemblyPath { get; }
        public Entry(IGregNativePlugin plugin, string assemblyPath)
        {
            Plugin = plugin;
            AssemblyPath = assemblyPath;
        }
    }

    private sealed class Host : IGregNativeHost
    {
        private readonly IGregEventBus _bus;
        public IGregLogger Logger { get; }
        public string ModFolderPath { get; }
        public Host(IGregLogger logger, IGregEventBus bus, string modFolderPath)
        {
            Logger = logger;
            _bus = bus;
            ModFolderPath = modFolderPath;
        }
        public void PublishEvent(string hookName, IReadOnlyDictionary<string, object> data)
        {
            try
            {
                _bus.Publish(hookName, new EventPayload
                {
                    HookName = hookName,
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = data
                });
            }
            catch { /* events must never break plugins */ }
        }
    }
}
