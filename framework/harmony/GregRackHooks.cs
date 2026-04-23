using System;
using HarmonyLib;
using greg.Core;
using greg.Sdk;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

namespace gregFramework.Hooks;

/// <summary>
/// Harmony hooks for domain Rack (generated from Il2Cpp unpack).
/// </summary>
[HarmonyPatch]
internal static class GregRackHooks
{
    // Rack.Awake
    [HarmonyPatch(typeof(Rack), nameof(Rack.Awake))]
    [HarmonyPostfix]
    private static void OnRackAwake(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackAwake failed: {ex.Message}");
        }
    }

    // Rack.Start
    [HarmonyPatch(typeof(Rack), nameof(Rack.Start))]
    [HarmonyPostfix]
    private static void OnRackStart(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackStart failed: {ex.Message}");
        }
    }

    // Rack.IsPositionAvailable
    [HarmonyPatch(typeof(Rack), nameof(Rack.IsPositionAvailable))]
    [HarmonyPostfix]
    private static void OnRackIsPositionAvailable(Rack __instance, int index, int sizeInU)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "IsPositionAvailable"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackIsPositionAvailable failed: {ex.Message}");
        }
    }

    // Rack.MarkPositionAsUsed
    [HarmonyPatch(typeof(Rack), nameof(Rack.MarkPositionAsUsed))]
    [HarmonyPostfix]
    private static void OnRackMarkPositionAsUsed(Rack __instance, int index, int sizeInU)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "MarkPositionAsUsed"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMarkPositionAsUsed failed: {ex.Message}");
        }
    }

    // Rack.MarkPositionAsUnused
    [HarmonyPatch(typeof(Rack), nameof(Rack.MarkPositionAsUnused))]
    [HarmonyPostfix]
    private static void OnRackMarkPositionAsUnused(Rack __instance, int index, int sizeInU)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "MarkPositionAsUnused"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMarkPositionAsUnused failed: {ex.Message}");
        }
    }

    // Rack.UpdateAudioVolume
    [HarmonyPatch(typeof(Rack), nameof(Rack.UpdateAudioVolume))]
    [HarmonyPostfix]
    private static void OnRackUpdateAudioVolume(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "AudioVolumeChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackUpdateAudioVolume failed: {ex.Message}");
        }
    }

    // Rack.ButtonDisablePositionsInRack
    [HarmonyPatch(typeof(Rack), nameof(Rack.ButtonDisablePositionsInRack))]
    [HarmonyPostfix]
    private static void OnRackButtonDisablePositionsInRack(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "ButtonDisablePositionsInRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackButtonDisablePositionsInRack failed: {ex.Message}");
        }
    }

    // Rack.SetDisablePositionsButtonMaterial
    [HarmonyPatch(typeof(Rack), nameof(Rack.SetDisablePositionsButtonMaterial))]
    [HarmonyPostfix]
    private static void OnRackSetDisablePositionsButtonMaterial(Rack __instance, Material material)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "DisablePositionsButtonMaterialSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackSetDisablePositionsButtonMaterial failed: {ex.Message}");
        }
    }

    // Rack.ButtonUnmountRack
    [HarmonyPatch(typeof(Rack), nameof(Rack.ButtonUnmountRack))]
    [HarmonyPostfix]
    private static void OnRackButtonUnmountRack(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "ButtonUnmountRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackButtonUnmountRack failed: {ex.Message}");
        }
    }

    // Rack.OnLoad
    [HarmonyPatch(typeof(Rack), nameof(Rack.OnLoad))]
    [HarmonyPostfix]
    private static void OnRackOnLoad(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "OnLoad"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackOnLoad failed: {ex.Message}");
        }
    }

    // Rack.OnDestroy
    [HarmonyPatch(typeof(Rack), nameof(Rack.OnDestroy))]
    [HarmonyPostfix]
    private static void OnRackOnDestroy(Rack __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "OnDestroy"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackOnDestroy failed: {ex.Message}");
        }
    }

    // RackMount.InstantiateRack
    [HarmonyPatch(typeof(RackMount), nameof(RackMount.InstantiateRack))]
    [HarmonyPostfix]
    private static void OnRackMountInstantiateRack(RackMount __instance, InteractObjectData saveData)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "InstantiateRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountInstantiateRack failed: {ex.Message}");
        }
    }

    // RackMount.ApplyMaterialToLODs
    [HarmonyPatch(typeof(RackMount), nameof(RackMount.ApplyMaterialToLODs))]
    [HarmonyPostfix]
    private static void OnRackMountApplyMaterialToLODs(RackMount __instance, GameObject rackGO, Material mat)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "ApplyMaterialToLODs"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountApplyMaterialToLODs failed: {ex.Message}");
        }
    }

    // RackMount.OnLoad
    [HarmonyPatch(typeof(RackMount), nameof(RackMount.OnLoad))]
    [HarmonyPostfix]
    private static void OnRackMountOnLoad(RackMount __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "OnLoad"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountOnLoad failed: {ex.Message}");
        }
    }

    // RackMount.OnDestroy
    [HarmonyPatch(typeof(RackMount), nameof(RackMount.OnDestroy))]
    [HarmonyPostfix]
    private static void OnRackMountOnDestroy(RackMount __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "OnDestroy"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountOnDestroy failed: {ex.Message}");
        }
    }

    // RackMount.CheatInsertRack
    [HarmonyPatch(typeof(RackMount), nameof(RackMount.CheatInsertRack))]
    [HarmonyPostfix]
    private static void OnRackMountCheatInsertRack(RackMount __instance, GameObject go, int type)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Rack, "CheatInsertRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRackMountCheatInsertRack failed: {ex.Message}");
        }
    }

}
