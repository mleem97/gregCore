using System;
using HarmonyLib;
using greg.Core;
using greg.Sdk;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace gregFramework.Hooks;

/// <summary>
/// Harmony hooks for domain Rack (generated from Il2Cpp unpack).
/// All patches are defensive: null-checks, try-catch, and Pointer validation.
/// </summary>
internal static class GregRackHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try
        {
            gregEventDispatcher.Emit(hookName, payload);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Event dispatch failed for {hookName}: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.Awake))]
    [HarmonyPostfix]
    private static void OnRackAwake(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "ComponentInitialized"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackAwake failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.Start))]
    [HarmonyPostfix]
    private static void OnRackStart(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "ComponentInitialized"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackStart failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.IsPositionAvailable))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool PrefixRackIsPositionAvailable(Rack __instance, int index, int sizeInU, ref bool __result)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero)
            {
                __result = false;
                return false;
            }

            bool isAvailable = true;
            var used = __instance.isPositionUsed;
            if (used != null && used.Pointer != IntPtr.Zero)
            {
                for (int i = index; i < index + sizeInU && i < used.Length; i++)
                {
                    if (used[i])
                    {
                        isAvailable = false;
                        break;
                    }
                }
            }
            __result = isAvailable;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "IsPositionAvailable"), new { instance = __instance, result = isAvailable });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixRackIsPositionAvailable failed: {ex.Message}");
            __result = false;
        }
        return false; // Skip original
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.MarkPositionAsUsed))]
    [HarmonyPostfix]
    private static void OnRackMarkPositionAsUsed(Rack __instance, int index, int sizeInU)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "MarkPositionAsUsed"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMarkPositionAsUsed failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.MarkPositionAsUnused))]
    [HarmonyPostfix]
    private static void OnRackMarkPositionAsUnused(Rack __instance, int index, int sizeInU)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "MarkPositionAsUnused"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMarkPositionAsUnused failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.UpdateAudioVolume))]
    [HarmonyPostfix]
    private static void OnRackUpdateAudioVolume(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "AudioVolumeChanged"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackUpdateAudioVolume failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.ButtonDisablePositionsInRack))]
    [HarmonyPostfix]
    private static void OnRackButtonDisablePositionsInRack(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "ButtonDisablePositionsInRack"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackButtonDisablePositionsInRack failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.SetDisablePositionsButtonMaterial))]
    [HarmonyPostfix]
    private static void OnRackSetDisablePositionsButtonMaterial(Rack __instance, Material material)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "DisablePositionsButtonMaterialSet"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackSetDisablePositionsButtonMaterial failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.ButtonUnmountRack))]
    [HarmonyPostfix]
    private static void OnRackButtonUnmountRack(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "ButtonUnmountRack"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackButtonUnmountRack failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.OnLoad))]
    [HarmonyPostfix]
    private static void OnRackOnLoad(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "OnLoad"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackOnLoad failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(Rack), nameof(Rack.OnDestroy))]
    [HarmonyPostfix]
    private static void OnRackOnDestroy(Rack __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "OnDestroy"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackOnDestroy failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(RackMount), nameof(RackMount.InstantiateRack))]
    [HarmonyPostfix]
    private static void OnRackMountInstantiateRack(RackMount __instance, InteractObjectData saveData)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "InstantiateRack"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountInstantiateRack failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(RackMount), nameof(RackMount.ApplyMaterialToLODs))]
    [HarmonyPostfix]
    private static void OnRackMountApplyMaterialToLODs(RackMount __instance, GameObject rackGO, Material mat)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "ApplyMaterialToLODs"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountApplyMaterialToLODs failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(RackMount), nameof(RackMount.OnLoad))]
    [HarmonyPostfix]
    private static void OnRackMountOnLoad(RackMount __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "OnLoad"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountOnLoad failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(RackMount), nameof(RackMount.OnDestroy))]
    [HarmonyPostfix]
    private static void OnRackMountOnDestroy(RackMount __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "OnDestroy"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountOnDestroy failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(RackMount), nameof(RackMount.CheatInsertRack))]
    [HarmonyPostfix]
    private static void OnRackMountCheatInsertRack(RackMount __instance, GameObject go, int type)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Rack, "CheatInsertRack"), new { instance = __instance });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountCheatInsertRack failed: {ex.Message}");
        }
    }
}
