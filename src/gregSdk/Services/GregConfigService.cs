using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;

namespace greg.Sdk.Services;

/// <summary>
/// Handles live mod configurations and persistence.
/// </summary>
public static class GregConfigService
{
    private static readonly string ConfigDir = Path.Combine(MelonEnvironment.UserDataDirectory, "gregFramework", "configs");
    private static readonly Dictionary<string, Dictionary<string, object>> _settings = new();
    private static readonly Dictionary<string, Action<string, object>> _listeners = new();

    static GregConfigService()
    {
        if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
    }

    public static void Set<T>(string modId, string key, T value)
    {
        if (!_settings.ContainsKey(modId)) _settings[modId] = new Dictionary<string, object>();
        
        bool changed = !_settings[modId].ContainsKey(key) || !EqualityComparer<T>.Default.Equals((T)_settings[modId][key], value);
        _settings[modId][key] = value;

        if (changed && _listeners.TryGetValue(modId, out var listener))
        {
            listener.Invoke(key, value);
        }
    }

    public static T Get<T>(string modId, string key, T defaultValue)
    {
        if (_settings.TryGetValue(modId, out var modSettings) && modSettings.TryGetValue(key, out var val))
        {
            return (T)val;
        }
        return defaultValue;
    }

    public static void Subscribe(string modId, Action<string, object> onChanged)
    {
        if (!_listeners.ContainsKey(modId)) _listeners[modId] = null;
        _listeners[modId] += onChanged;
    }

    public static void Save()
    {
        foreach (var mod in _settings)
        {
            string path = Path.Combine(ConfigDir, $"{mod.Key}.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(mod.Value, Formatting.Indented));
        }
    }

    public static void Load(string modId)
    {
        string path = Path.Combine(ConfigDir, $"{modId}.json");
        if (File.Exists(path))
        {
            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(path));
                if (data != null) _settings[modId] = data;
            }
            catch { }
        }
    }
}

