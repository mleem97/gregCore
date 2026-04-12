using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2Cpp;
using System.Linq;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for the game's SaveSystem.
/// Allows mods to persist custom data within the global game state.
/// </summary>
public static class GregSaveService
{
    private static readonly Dictionary<string, string> _customModData = new();
    
    // Actions for mods to subscribe to
    public static Action OnBeforeSave;
    public static Action OnAfterLoad;

    static GregSaveService()
    {
        // Hook into game's native save events if possible, or use Harmony
        // For now, we provide the manual triggers
    }

    /// <summary>
    /// Stores custom data for a mod.
    /// </summary>
    public static void SetModData(string modId, string key, string value)
    {
        _customModData[$"{modId}_{key}"] = value;
    }

    /// <summary>
    /// Retrieves custom data for a mod.
    /// </summary>
    public static string GetModData(string modId, string key)
    {
        return _customModData.TryGetValue($"{modId}_{key}", out var val) ? val : null;
    }

    /// <summary>
    /// Returns all keys for a specific mod.
    /// </summary>
    public static List<string> GetModKeys(string modId)
    {
        string prefix = $"{modId}_";
        return _customModData.Keys.Where(k => k.StartsWith(prefix)).Select(k => k.Substring(prefix.Length)).ToList();
    }

    // Internal triggers called by gregCore Harmony patches
    internal static void TriggerSave()
    {
        OnBeforeSave?.Invoke();
        // Custom persistence logic (e.g., writing to a sidecar file or injecting into SaveData)
    }

    internal static void TriggerLoad()
    {
        // Custom loading logic
        OnAfterLoad?.Invoke();
    }
}
