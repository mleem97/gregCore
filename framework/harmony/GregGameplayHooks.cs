using System;
using HarmonyLib;
using greg.Core;
using greg.Sdk;
using Il2Cpp;
using MelonLoader;

namespace gregFramework.Hooks;

/// <summary>
/// Harmony hooks for domain Gameplay (generated from Il2Cpp unpack).
/// All patches are defensive: null-checks, try-catch, and NativePointer validation.
/// </summary>
internal static class GregGameplayHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try { gregEventDispatcher.Emit(hookName, payload); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Event dispatch failed for {hookName}: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.Awake))]
    [HarmonyPostfix]
    private static void OnObjectivesAwake(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "ComponentInitialized"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesAwake failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.Start))]
    [HarmonyPostfix]
    private static void OnObjectivesStart(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "ComponentInitialized"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesStart failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.GetTimedObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesGetTimedObjective(Objectives __instance, int objectiveUID)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "GetTimedObjective"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesGetTimedObjective failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.IsTutorialInProgress))]
    [HarmonyPostfix]
    private static void OnObjectivesIsTutorialInProgress(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "IsTutorialInProgress"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesIsTutorialInProgress failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.CreateAppObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesCreateAppObjective(Objectives __instance, int customerID, int appID, int time, int requiredIOPS)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "CreateAppObjective"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesCreateAppObjective failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.ObjectiveTimedText))]
    [HarmonyPostfix]
    private static void OnObjectivesObjectiveTimedText(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "ObjectiveTimedText"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesObjectiveTimedText failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.DestroyObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesDestroyObjective(Objectives __instance, int _objectiveUID)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "DestroyObjective"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesDestroyObjective failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.ClearObjectives))]
    [HarmonyPostfix]
    private static void OnObjectivesClearObjectives(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "ClearObjectives"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesClearObjectives failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.StartObjective))]
    [HarmonyPostfix]
    private static void OnObjectivesStartObjective(Objectives __instance, int _objectiveUID, Vector3 objectivePosition, bool _loadSave)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "StartObjective"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesStartObjective failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.InstantiateObjectiveSign))]
    [HarmonyPostfix]
    private static void OnObjectivesInstantiateObjectiveSign(Objectives __instance, int objectiveUID, Vector3 objectPos)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "InstantiateObjectiveSign"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesInstantiateObjectiveSign failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.RemoveObjectiveSign))]
    [HarmonyPostfix]
    private static void OnObjectivesRemoveObjectiveSign(Objectives __instance, int objectiveUID)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "ObjectiveSignRemoved"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesRemoveObjectiveSign failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.OnDestroy))]
    [HarmonyPostfix]
    private static void OnObjectivesOnDestroy(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "OnDestroy"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesOnDestroy failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Objectives), nameof(Objectives.OnLoad))]
    [HarmonyPostfix]
    private static void OnObjectivesOnLoad(Objectives __instance)
    {
        try { if (__instance == null || __instance.NativePointer == IntPtr.Zero) return; SafeEmit(gregHookName.Create(GregDomain.Gameplay, "OnLoad"), new { instance = __instance }); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnObjectivesOnLoad failed: {ex.Message}"); }
    }
}
