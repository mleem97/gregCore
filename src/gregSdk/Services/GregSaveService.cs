using System;
using System.IO;
using UnityEngine;
using MelonLoader;
using Newtonsoft.Json;

namespace greg.Sdk.Services;

/// <summary>
/// Service for handling mod-specific save data.
/// Implements Task 1.2: gregSaveService.
/// </summary>
public static class GregSaveService
{
    public static event Action OnBeforeSave;
    public static event Action OnAfterLoad;

    internal static void TriggerOnBeforeSave() => OnBeforeSave?.Invoke();
    internal static void TriggerOnAfterLoad() => OnAfterLoad?.Invoke();

    internal static string BaseSavePathOverride { get; set; }
    private static string BaseSavePath => BaseSavePathOverride ?? Path.Combine(Application.persistentDataPath, "gregFramework");

    private static void EnsureDirectory()
    {
        if (!Directory.Exists(BaseSavePath))
            Directory.CreateDirectory(BaseSavePath);
    }

    private static string GetPath(string modId) => Path.Combine(BaseSavePath, $"{modId}.json");

    /// <summary>
    /// Gets the native mod save data from the game's new SaveSystem if available.
    /// </summary>
    public static Il2Cpp.ModItemSaveData GetNativeModData(string modId)
    {
        var currentSave = Il2Cpp.SaveData._current;
        if (currentSave == null || currentSave.modItemData == null) return null;

        foreach (var data in currentSave.modItemData)
        {
            if (data.modFolderName == modId)
                return data;
        }
        return null;
    }

    /// <summary>
    /// Creates or updates native save data for a mod.
    /// </summary>
    public static Il2Cpp.ModItemSaveData GetOrCreateNativeModData(string modId)
    {
        var currentSave = Il2Cpp.SaveData._current;
        if (currentSave == null) return null;

        if (currentSave.modItemData == null)
            currentSave.modItemData = new Il2CppSystem.Collections.Generic.List<Il2Cpp.ModItemSaveData>();

        var existing = GetNativeModData(modId);
        if (existing != null) return existing;

        var newData = new Il2Cpp.ModItemSaveData
        {
            modFolderName = modId,
            saveValue = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float>(0),
            saveIntArray = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int>(0),
            saveIntArray2 = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<int>(0)
        };

        currentSave.modItemData.Add(newData);
        return newData;
    }

    /// <summary>
    /// Implements Task 1.2 sub-goal: Initialization.
    /// </summary>
    public static void Init() 
    {
        EnsureDirectory();
        MelonLogger.Msg("[GregSaveService] Initialized.");
    }

    /// <summary>
    /// Stores data for a specific mod and key.
    /// </summary>
    public static void SetData<T>(string modId, string key, T data)
    {
        try
        {
            EnsureDirectory();
            var wrapper = new SaveWrapper<T> { key = key, data = data };
            var json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
            File.WriteAllText(GetPath(modId), json);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregSaveService] Failed to set data for {modId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves data for a specific mod and key.
    /// </summary>
    public static T GetData<T>(string modId, string key, T defaultValue = default)
    {
        try
        {
            string path = GetPath(modId);
            if (!File.Exists(path)) return defaultValue;

            string json = File.ReadAllText(path);
            var wrapper = JsonConvert.DeserializeObject<SaveWrapper<T>>(json);
            
            if (wrapper != null && wrapper.key == key)
                return wrapper.data;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[GregSaveService] Failed to get data for {modId}: {ex.Message}");
        }
        return defaultValue;
    }

    private class SaveWrapper<T>
    {
        public string key;
        public T data;
    }
}



