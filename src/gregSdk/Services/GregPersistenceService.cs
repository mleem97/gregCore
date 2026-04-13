using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MelonLoader;
using MelonLoader.Utils;

namespace greg.Sdk.Services;

/// <summary>
/// JSON-based key-value persistence per mod.
/// Path: UserData/{modName}/{key}.json
/// Thread-safe with lock().
/// </summary>
public static class GregPersistenceService
{
    private static readonly object _lock = new();
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static string GetModDir(string modName)
    {
        return Path.Combine(MelonEnvironment.UserDataDirectory, "gregCore", "Data", modName);
    }

    private static string GetFilePath(string modName, string key)
    {
        return Path.Combine(GetModDir(modName), $"{key}.json");
    }

    public static void Save<T>(string modName, string key, T data)
    {
        lock (_lock)
        {
            try
            {
                var dir = GetModDir(modName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var path = GetFilePath(modName, key);
                var json = JsonSerializer.Serialize(data, _jsonOpts);
                File.WriteAllText(path, json);
                MelonLogger.Msg($"[Persistence] Saved: {modName}/{key} ({json.Length} bytes)");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Persistence] Save failed ({modName}/{key}): {ex.Message}");
            }
        }
    }

    public static T Load<T>(string modName, string key, T defaultValue = default)
    {
        lock (_lock)
        {
            try
            {
                var path = GetFilePath(modName, key);
                if (!File.Exists(path)) return defaultValue;

                var json = File.ReadAllText(path);
                var result = JsonSerializer.Deserialize<T>(json, _jsonOpts);
                MelonLogger.Msg($"[Persistence] Loaded: {modName}/{key}");
                return result ?? defaultValue;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Persistence] Load failed ({modName}/{key}): {ex.Message}");
                return defaultValue;
            }
        }
    }

    public static bool Exists(string modName, string key)
    {
        return File.Exists(GetFilePath(modName, key));
    }

    public static void Delete(string modName, string key)
    {
        lock (_lock)
        {
            try
            {
                var path = GetFilePath(modName, key);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    MelonLogger.Msg($"[Persistence] Deleted: {modName}/{key}");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Persistence] Delete failed ({modName}/{key}): {ex.Message}");
            }
        }
    }

    public static List<string> ListKeys(string modName)
    {
        var result = new List<string>();
        try
        {
            var dir = GetModDir(modName);
            if (!Directory.Exists(dir)) return result;

            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                result.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[Persistence] ListKeys failed ({modName}): {ex.Message}");
        }
        return result;
    }
}

