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



