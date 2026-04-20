using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;

namespace gregCore.Infrastructure.Config;

/// <summary>
/// Registry, die greg_hooks.json zur Laufzeit lädt und verwaltet.
/// </summary>
public class GregHookRegistry : IGregHookRegistry
{
    private readonly IGregLogger _logger;
    private readonly List<GregHookDef> _hooks = new();
    private readonly Dictionary<string, GregHookDef> _hookByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _hookToId = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, string> _idToHook = new();

    public GregHookRegistry(IGregLogger logger)
    {
        _logger = logger;
        LoadHooks();
    }

    private void LoadHooks()
    {
        try
        {
            string hooksFile = ResolveHooksFilePath();
            
            if (File.Exists(hooksFile))
            {
                _logger.Debug($"Lade Hooks von: {hooksFile}");
                var content = File.ReadAllText(hooksFile);
                var manifest = JsonConvert.DeserializeObject<GregHooksManifest>(content);
                
                if (manifest?.Hooks != null)
                {
                    _hooks.AddRange(manifest.Hooks);
                    
                    foreach (var hook in manifest.Hooks)
                    {
                        if (string.IsNullOrEmpty(hook.Name)) continue;
                        
                        _hookByName[hook.Name] = hook;
                        
                        if (!string.IsNullOrEmpty(hook.FriendlyAlias))
                        {
                            _hookByName[hook.FriendlyAlias] = hook;
                        }

                        // Generiere konsistente Hash-basierte ID für FFI
                        int eventId = GetStableHashCode(hook.Name);
                        _hookToId[hook.Name] = eventId;
                        _idToHook[eventId] = hook.Name;
                    }
                    
                    _logger.Info($"Erfolgreich {_hooks.Count} Hooks aus greg_hooks.json geladen.");
                }
            }
            else
            {
                _logger.Warning($"greg_hooks.json nicht gefunden bei: {hooksFile}. Hook-Registry bleibt leer.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Fehler beim Laden der greg_hooks.json", ex);
        }
    }

    private string ResolveHooksFilePath()
    {
        // 1. Priorität: MelonLoader Mods Ordner
        var modsDir = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        if (!string.IsNullOrEmpty(modsDir))
        {
            var path = Path.Combine(modsDir, "greg_hooks.json");
            if (File.Exists(path)) return path;
        }

        // 2. Priorität: Assembly Location
        var asmLoc = Assembly.GetExecutingAssembly().Location;
        var asmDir = Path.GetDirectoryName(asmLoc);
        if (!string.IsNullOrEmpty(asmDir))
        {
            var path = Path.Combine(asmDir, "greg_hooks.json");
            if (File.Exists(path)) return path;
        }

        // 3. Fallback: Project assets
        return Path.Combine(Environment.CurrentDirectory, "assets", "greg_hooks.json");
    }

    private static int GetStableHashCode(string str)
    {
        // Garantiert einen stabilen, positiven 32-bit Hash über verschiedene Laufzeiten hinweg
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;
            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0') break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }
            return Math.Abs(hash1 + (hash2 * 1566083941));
        }
    }

    public IEnumerable<GregHookDef> GetAllHooks() => _hooks;

    public bool TryGetHook(string name, out GregHookDef hookDef) =>
        _hookByName.TryGetValue(name, out hookDef!);

    public bool TryGetEventId(string hookName, out int eventId) =>
        _hookToId.TryGetValue(hookName, out eventId);

    public bool TryGetHookName(int eventId, out string hookName) =>
        _idToHook.TryGetValue(eventId, out hookName!);
}
