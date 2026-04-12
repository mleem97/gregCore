using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using HarmonyLib;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Service for handling mod-specific save data to bypass IL2CPP serialization limits.
/// </summary>
public static class GregSaveService
{
    private static readonly string SaveDir = Path.Combine(MelonEnvironment.UserDataDirectory, "gregFramework", "Saves");
    private static readonly Dictionary<string, Dictionary<string, object>> _modData = new();

    public static void Init()
    {
        if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
    }

    /// <summary>
    /// Stores data for a specific mod and key.
    /// </summary>
    public static void SetData(string modId, string key, object data)
    {
        if (!_modData.TryGetValue(modId, out var dict))
        {
            dict = new Dictionary<string, object>();
            _modData[modId] = dict;
        }
        dict[key] = data;
    }

    /// <summary>
    /// Retrieves data for a specific mod and key.
    /// </summary>
    public static T GetData<T>(string modId, string key, T defaultValue = default)
    {
        if (_modData.TryGetValue(modId, out var dict) && dict.TryGetValue(key, out var val))
        {
            try
            {
                if (val is T typedVal) return typedVal;
                // Handle JSON deserialization if it was loaded from file as JObject/JArray
                string json = JsonConvert.SerializeObject(val);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return defaultValue; }
        }
        return defaultValue;
    }

    /// <summary>
    /// Saves all mod data to a file corresponding to the game's current save slot.
    /// </summary>
    public static void Save(string slotName)
    {
        try
        {
            string path = Path.Combine(SaveDir, $"{slotName}_greg.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(_modData, Formatting.Indented));
            MelonLogger.Msg($"[GregSaveService] Saved mod data for slot: {slotName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregSaveService] Failed to save: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads mod data for a specific save slot.
    /// </summary>
    public static void Load(string slotName)
    {
        try
        {
            string path = Path.Combine(SaveDir, $"{slotName}_greg.json");
            if (!File.Exists(path))
            {
                _modData.Clear();
                return;
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(path));
            if (data != null)
            {
                _modData.Clear();
                foreach (var kvp in data) _modData[kvp.Key] = kvp.Value;
            }
            MelonLogger.Msg($"[GregSaveService] Loaded mod data for slot: {slotName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregSaveService] Failed to load: {ex.Message}");
        }
    }
}

/// <summary>
/// Harmony patches to trigger GregSaveService during game save/load.
/// </summary>
[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.SaveGame))]
public static class SaveSystem_SavePatch
{
    public static void Postfix()
    {
        // Data Center doesn't seem to expose the slot name easily in the call, 
        // we might need to find where the current slot is stored.
        // For now, use "default".
        GregSaveService.Save("auto");
    }
}

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.LoadGame))]
public static class SaveSystem_LoadPatch
{
    public static void Postfix()
    {
        GregSaveService.Load("auto");
    }
}
