/// <file-summary>
/// Layer:       Compatibility
/// Purpose:     Further parts of ModConfigSystem (partial): registry API /
///               JSON codec. Split due to codeline limits.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine.UIElements;

namespace DataCenterModLoader;

public static partial class ModConfigSystem
{
    public static uint RegisterBool(string modId, string key, string displayName, bool defaultValue, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterBool '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Bool,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                BoolDefault = defaultValue,
                BoolValue = defaultValue
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Bool)
            {
                entry.BoolValue = persisted.BoolValue;
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterBool '{modId}/{key}' = {entry.BoolValue} (default={defaultValue})");
            _logger.Msg($"[ModConfig] Registered bool '{modId}/{key}': {entry.BoolValue}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterBool", ex);
            return 0;
        }
    }

    public static uint RegisterInt(string modId, string key, string displayName, int defaultValue, int min, int max, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterInt '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Int,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                IntDefault = defaultValue,
                IntValue = defaultValue,
                IntMin = min,
                IntMax = max
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Int)
            {
                entry.IntValue = Math.Clamp(persisted.IntValue, min, max);
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterInt '{modId}/{key}' = {entry.IntValue} (default={defaultValue}, range={min}-{max})");
            _logger.Msg($"[ModConfig] Registered int '{modId}/{key}': {entry.IntValue}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterInt", ex);
            return 0;
        }
    }

    public static uint RegisterFloat(string modId, string key, string displayName, float defaultValue, float min, float max, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterFloat '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Float,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                FloatDefault = defaultValue,
                FloatValue = defaultValue,
                FloatMin = min,
                FloatMax = max
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Float)
            {
                entry.FloatValue = Math.Clamp(persisted.FloatValue, min, max);
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterFloat '{modId}/{key}' = {entry.FloatValue:F1} (default={defaultValue:F1}, range={min:F1}-{max:F1})");
            _logger.Msg($"[ModConfig] Registered float '{modId}/{key}': {entry.FloatValue:F1}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterFloat", ex);
            return 0;
        }
    }

    public static uint GetBool(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Bool)
                    return entry.BoolValue ? 1u : 0u;
            }
            return 0xFFFFFFFF;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetBool", ex);
            return 0xFFFFFFFF;
        }
    }

    public static int GetInt(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                    return entry.IntValue;
            }
            return 0;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetInt", ex);
            return 0;
        }
    }

    public static float GetFloat(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                    return entry.FloatValue;
            }
            return 0f;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetFloat", ex);
            return 0f;
        }
    }

    public static void SetModInfo(string modId, string author, string version)
    {
        try
        {
            if (string.IsNullOrEmpty(modId)) return;
            var mod = GetOrCreateMod(modId);
            if (!string.IsNullOrEmpty(author)) mod.Author = author;
            if (!string.IsNullOrEmpty(version)) mod.Version = version;
            CrashLog.Log($"ModConfig: set mod info for '{modId}': author='{author}', version='{version}'");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetModInfo", ex);
        }
    }

    public static bool RegisterBoolOption(string modId, string key, string displayName, bool defaultValue)
        => RegisterBoolOption(modId, key, displayName, defaultValue, "");

    public static bool RegisterBoolOption(string modId, string key, string displayName, bool defaultValue, string description)
        => RegisterBool(modId, key, displayName, defaultValue, description) == 1;

    public static bool RegisterIntOption(string modId, string key, string displayName, int defaultValue, int min, int max)
        => RegisterIntOption(modId, key, displayName, defaultValue, min, max, "");

    public static bool RegisterIntOption(string modId, string key, string displayName, int defaultValue, int min, int max, string description)
        => RegisterInt(modId, key, displayName, defaultValue, min, max, description) == 1;

    public static bool RegisterFloatOption(string modId, string key, string displayName, float defaultValue, float min, float max)
        => RegisterFloatOption(modId, key, displayName, defaultValue, min, max, "");

    public static bool RegisterFloatOption(string modId, string key, string displayName, float defaultValue, float min, float max, string description)
        => RegisterFloat(modId, key, displayName, defaultValue, min, max, description) == 1;

    public static bool GetBoolValue(string modId, string key)
        => GetBoolValue(modId, key, false);

    public static bool GetBoolValue(string modId, string key, bool defaultValue)
    {
        uint raw = GetBool(modId, key);
        if (raw == 0xFFFFFFFF) return defaultValue;
        return raw == 1;
    }

    public static int GetIntValue(string modId, string key)
        => GetIntValue(modId, key, 0);

    public static int GetIntValue(string modId, string key, int defaultValue)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                    return entry.IntValue;
            }
            return defaultValue;
        }
        catch { return defaultValue; }
    }

    public static float GetFloatValue(string modId, string key)
        => GetFloatValue(modId, key, 0f);

    public static float GetFloatValue(string modId, string key, float defaultValue)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                    return entry.FloatValue;
            }
            return defaultValue;
        }
        catch { return defaultValue; }
    }

    public static bool SetBoolValue(string modId, string key, bool value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Bool)
                {
                    entry.BoolValue = value;
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetBoolValue", ex);
            return false;
        }
    }

    public static bool SetIntValue(string modId, string key, int value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                {
                    entry.IntValue = Math.Clamp(value, entry.IntMin, entry.IntMax);
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetIntValue", ex);
            return false;
        }
    }

    public static bool SetFloatValue(string modId, string key, float value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                {
                    entry.FloatValue = Math.Clamp(value, entry.FloatMin, entry.FloatMax);
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetFloatValue", ex);
            return false;
        }
    }

    public static bool HasOption(string modId, string key)
    {
        return _mods.TryGetValue(modId, out var mod) && mod.Entries.ContainsKey(key);
    }

    public static void OpenPanel() => ShowPanel();
    public static void ClosePanel() => HidePanel();

    private static ModConfig GetOrCreateMod(string modId)
    {
        if (!_mods.TryGetValue(modId, out var mod))
        {
            mod = new ModConfig { ModId = modId };
            _mods[modId] = mod;
            _modOrder.Add(modId);

            if (_selectedModId == null)
                _selectedModId = modId;

            CrashLog.Log($"ModConfig: created mod config for '{modId}'.");
        }
        return mod;
    }

    private static string GetConfigPath(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
            throw new ArgumentException("modId cannot be null or empty", nameof(modId));

        if (modId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            modId.Contains(Path.DirectorySeparatorChar) ||
            modId.Contains(Path.AltDirectorySeparatorChar) ||
            modId.Contains(".."))
        {
            CrashLog.Log($"[Security] Attempted path traversal detected with modId: {modId}");
            throw new ArgumentException("Invalid characters in modId", nameof(modId));
        }

        return Path.Combine(_configDir, modId + ".json");
    }

    private static void SaveModConfig(ModConfig mod)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"entries\": {");

            bool first = true;
            foreach (var key in mod.EntryOrder)
            {
                if (!mod.Entries.TryGetValue(key, out var entry))
                    continue;

                if (!first) sb.AppendLine(",");
                first = false;

                string escapedKey = EscapeJsonString(key);

                switch (entry.Type)
                {
                    case ConfigEntryType.Bool:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"bool\", \"value\": {(entry.BoolValue ? "true" : "false")} }}");
                        break;
                    case ConfigEntryType.Int:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"int\", \"value\": {entry.IntValue} }}");
                        break;
                    case ConfigEntryType.Float:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"float\", \"value\": {entry.FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture)} }}");
                        break;
                }
            }

            if (!first) sb.AppendLine();
            sb.AppendLine("  }");
            sb.AppendLine("}");

            string path = GetConfigPath(mod.ModId);
            string? dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            CrashLog.Log($"ModConfig: saved config for '{mod.ModId}' to {path}");
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"ModConfigSystem.SaveModConfig({mod.ModId})", ex);
        }
    }

    private static ConfigEntry? LoadPersistedValue(string modId, string key)
    {
        try
        {
            string path = GetConfigPath(modId);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path, Encoding.UTF8);
            var entries = ParseConfigJson(json);

            if (entries != null && entries.TryGetValue(key, out var entry))
                return entry;
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"ModConfigSystem.LoadPersistedValue({modId}/{key})", ex);
        }
        return null;
    }

}
