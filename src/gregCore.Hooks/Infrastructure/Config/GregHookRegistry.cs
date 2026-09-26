using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;

namespace gregCore.Infrastructure.Config;

/// <summary>
/// Registry that loads and manages greg_hooks.json at runtime.
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
                _logger.Debug($"Loading hooks from: {hooksFile}");
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

                        // Generate a consistent hash-based ID for FFI
                        int eventId = GetStableHashCode(hook.Name);
                        _hookToId[hook.Name] = eventId;
                        _idToHook[eventId] = hook.Name;
                    }
                    
                    _logger.Info($"Successfully {_hooks.Count} Hooks aus greg_hooks.json loaded.");
                }
            }
            else
            {
                _logger.Warning($"greg_hooks.json not found bei: {hooksFile}. Hook registry remains empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error loading greg_hooks.json", ex);
        }
    }

    private string ResolveHooksFilePath()
    {
        // 1st priority: MelonLoader mods folder
        var modsDir = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        if (!string.IsNullOrEmpty(modsDir))
        {
            var path = Path.Combine(modsDir, "greg_hooks.json");
            if (File.Exists(path)) return path;
        }

        // 2nd priority: assembly location
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
        // Guarantees a stable, positive 32-bit hash across different runtimes
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
