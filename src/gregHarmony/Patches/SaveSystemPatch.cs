using System;
using HarmonyLib;
using Il2Cpp;
using greg.Sdk.Services;
using MelonLoader;

namespace greg.Harmony.Patches;

/// <summary>
/// Patch for Task 1.2: Integrates GregSaveService with the game's native SaveSystem.
/// </summary>
[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.SaveGame))]
public static class SaveSystem_SavePatch
{
    public static void Postfix(string savename)
    {
        try
        {
            // Detect slot from argument or static property
            string slot = string.IsNullOrEmpty(savename) ? (SaveSystem.loadSaveName ?? "auto") : savename;
            
            // Trigger framework save logic
            MelonLogger.Msg($"[gregCore] Native save triggered for slot: {slot}. Persisting mod data...");
            GregSaveService.TriggerOnBeforeSave();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Failed in Save Postfix: {ex.Message}");
        }
    }
}

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.LoadGame))]
public static class SaveSystem_LoadPatch
{
    public static void Postfix(string savename)
    {
        try
        {
            string slot = string.IsNullOrEmpty(savename) ? (SaveSystem.loadSaveName ?? "auto") : savename;
            MelonLogger.Msg($"[gregCore] Native load triggered for slot: {slot}. Refreshing mod data...");
            GregSaveService.TriggerOnAfterLoad();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Failed in Load Postfix: {ex.Message}");
        }
    }
}

