using System;
using System.Collections.Generic;
using MelonLoader;

namespace greg.Sdk.Services;

/// <summary>
/// Central registry for greg ecosystem mods.
/// Enables optional cross-mod integration (e.g. gregHexviewer, gregCoreUiExt).
/// </summary>
public static class GregModRegistry
{
    private class ModEntry
    {
        public string Name;
        public string Version;
        public object ApiInstance;
    }

    private static readonly Dictionary<string, ModEntry> _mods = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(string modName, string version, object apiInstance = null)
    {
        if (string.IsNullOrWhiteSpace(modName)) return;

        _mods[modName] = new ModEntry
        {
            Name = modName,
            Version = version ?? "0.0.0",
            ApiInstance = apiInstance
        };
        MelonLogger.Msg($"[ModRegistry] Registered: {modName} v{version}");
    }

    public static void Unregister(string modName)
    {
        if (_mods.Remove(modName))
            MelonLogger.Msg($"[ModRegistry] Unregistered: {modName}");
    }

    public static bool IsLoaded(string modName)
    {
        return _mods.ContainsKey(modName);
    }

    public static string GetVersion(string modName)
    {
        return _mods.TryGetValue(modName, out var entry) ? entry.Version : null;
    }

    public static T GetApi<T>(string modName) where T : class
    {
        if (_mods.TryGetValue(modName, out var entry) && entry.ApiInstance is T api)
        {
            MelonLogger.Msg($"[ModRegistry] GetApi<{typeof(T).Name}>({modName}): found");
            return api;
        }
        MelonLogger.Msg($"[ModRegistry] GetApi<{typeof(T).Name}>({modName}): null");
        return null;
    }

    public static List<string> GetLoadedMods()
    {
        return new List<string>(_mods.Keys);
    }
}

