using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MelonLoader;

namespace gregModLoader;

internal static class gregModActivationService
{
    private sealed class ActivationState
    {
        public Dictionary<string, bool> ModStates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private static readonly object Sync = new();
    private static readonly Dictionary<string, bool> States = new(StringComparer.OrdinalIgnoreCase);

    private static string _configPath = string.Empty;
    private static MelonLogger.Instance _logger;
    private static bool _initialized;

    public static void Initialize(MelonLogger.Instance logger)
    {
        lock (Sync)
        {
            if (_initialized)
            {
                return;
            }

            _logger = logger;
            _configPath = GameFolderLayout.ResolveModCfgFile("gregcore-mod-activation.json");
            LoadUnsafe();
            _initialized = true;
        }
    }

    public static bool IsEnabled(string modId, bool defaultValue = true)
    {
        if (string.IsNullOrWhiteSpace(modId))
        {
            return defaultValue;
        }

        lock (Sync)
        {
            return States.TryGetValue(modId, out var enabled) ? enabled : defaultValue;
        }
    }

    public static void SetEnabled(string modId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(modId))
        {
            return;
        }

        lock (Sync)
        {
            States[modId] = enabled;
            SaveUnsafe();
        }
    }

    private static void LoadUnsafe()
    {
        if (string.IsNullOrWhiteSpace(_configPath) || !File.Exists(_configPath))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            var state = JsonSerializer.Deserialize<ActivationState>(json);
            if (state?.ModStates is null)
            {
                return;
            }

            States.Clear();
            foreach (var kv in state.ModStates)
            {
                States[kv.Key] = kv.Value;
            }
        }
        catch (Exception ex)
        {
            _logger?.Warning($"[gregCore] Failed to read mod activation config: {ex.Message}");
        }
    }

    private static void SaveUnsafe()
    {
        if (string.IsNullOrWhiteSpace(_configPath))
        {
            return;
        }

        try
        {
            var state = new ActivationState { ModStates = new Dictionary<string, bool>(States, StringComparer.OrdinalIgnoreCase) };
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch (Exception ex)
        {
            _logger?.Warning($"[gregCore] Failed to persist mod activation config: {ex.Message}");
        }
    }
}


