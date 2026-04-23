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
/// Harmony hooks for domain Gameplay (generated from Il2Cpp unpack).
/// </summary>
[HarmonyPatch]
internal static class GregGameplayHooks
{
    // Objectives.Awake
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.Awake))]
    [HarmonyPostfix]
    private static void OnObjectivesAwake(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesAwake failed: {ex.Message}");
        }
    }

    // Objectives.Start
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.Start))]
    [HarmonyPostfix]
    private static void OnObjectivesStart(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesStart failed: {ex.Message}");
        }
    }

    // Objectives.GetTimedObjective
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.GetTimedObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesGetTimedObjective(Objectives __instance, int objectiveUID)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "GetTimedObjective"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesGetTimedObjective failed: {ex.Message}");
        }
    }

    // Objectives.IsTutorialInProgress
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.IsTutorialInProgress))]
    [HarmonyPostfix]
    private static void OnObjectivesIsTutorialInProgress(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "IsTutorialInProgress"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesIsTutorialInProgress failed: {ex.Message}");
        }
    }

    // Objectives.CreateAppObjective
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.CreateAppObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesCreateAppObjective(Objectives __instance, int customerID, int appID, int time, int requiredIOPS)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "CreateAppObjective"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesCreateAppObjective failed: {ex.Message}");
        }
    }

    // Objectives.ObjectiveTimedText
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.ObjectiveTimedText))]
    [HarmonyPostfix]
    private static void OnObjectivesObjectiveTimedText(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "ObjectiveTimedText"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesObjectiveTimedText failed: {ex.Message}");
        }
    }

    // Objectives.DestroyObjective
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.DestroyObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesDestroyObjective(Objectives __instance, int _objectiveUID)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "DestroyObjective"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesDestroyObjective failed: {ex.Message}");
        }
    }

    // Objectives.ClearObjectives
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.ClearObjectives))]
    [HarmonyPostfix]
    private static void OnObjectivesClearObjectives(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "ClearObjectives"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesClearObjectives failed: {ex.Message}");
        }
    }

    // Objectives.StartObjective
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.StartObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesStartObjective(Objectives __instance, int _objectiveUID, Vector3 objectivePosition, bool _loadSave)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "StartObjective"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesStartObjective failed: {ex.Message}");
        }
    }

    // Objectives.InstantiateObjectiveSign
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.InstantiateObjectiveSign))]
    [HarmonyPostfix]
    private static void OnObjectivesInstantiateObjectiveSign(Objectives __instance, int objectiveUID, Vector3 objectPos)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "InstantiateObjectiveSign"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesInstantiateObjectiveSign failed: {ex.Message}");
        }
    }

    // Objectives.RemoveObjectiveSign
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.RemoveObjectiveSign))]
    [HarmonyPostfix]
    private static void OnObjectivesRemoveObjectiveSign(Objectives __instance, int objectiveUID)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "ObjectiveSignRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesRemoveObjectiveSign failed: {ex.Message}");
        }
    }

    // Objectives.OnDestroy
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.OnDestroy))]
    [HarmonyPostfix]
    private static void OnObjectivesOnDestroy(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "OnDestroy"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesOnDestroy failed: {ex.Message}");
        }
    }

    // Objectives.OnLoad
    [HarmonyPatch(typeof(Objectives), nameof(Objectives.OnLoad))]
    [HarmonyPostfix]
    private static void OnObjectivesOnLoad(Objectives __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Gameplay, "OnLoad"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectivesOnLoad failed: {ex.Message}");
        }
    }

}
