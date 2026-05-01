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
/// Harmony hooks for domain System (generated from Il2Cpp unpack).
/// </summary>
internal static class GregSystemHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.System, hookName),
                payload);
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook SafeEmit '" + hookName + $"' failed: {ex.Message}");
        }
    }


    // AICharacterControl.OnEnable
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnEnable))]
    [HarmonyPostfix]
    private static void OnAICharacterControlOnEnable(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlOnEnable failed: {ex.Message}");
        }
    }

    // AICharacterControl.OnCreated
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnCreated))]
    [HarmonyPostfix]
    private static void OnAICharacterControlOnCreated(AICharacterControl __instance, UMAData umadata)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnCreated",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlOnCreated failed: {ex.Message}");
        }
    }

    // AICharacterControl.StartingAnimation
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.StartingAnimation))]
    [HarmonyPostfix]
    private static void OnAICharacterControlStartingAnimation(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "StartingAnimation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlStartingAnimation failed: {ex.Message}");
        }
    }

    // AICharacterControl.OnDisable
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnDisable))]
    [HarmonyPostfix]
    private static void OnAICharacterControlOnDisable(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlOnDisable failed: {ex.Message}");
        }
    }

    // AICharacterControl.OnDestroy
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnDestroy))]
    [HarmonyPostfix]
    private static void OnAICharacterControlOnDestroy(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlOnDestroy failed: {ex.Message}");
        }
    }

    // AICharacterControl.SetTarget
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.SetTarget))]
    [HarmonyPostfix]
    private static void OnAICharacterControlSetTarget(AICharacterControl __instance, Vector3 target)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TargetSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlSetTarget failed: {ex.Message}");
        }
    }

    // AICharacterControl.AgentReachTarget
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.AgentReachTarget))]
    [HarmonyPostfix]
    private static void OnAICharacterControlAgentReachTarget(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AgentReachTarget",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlAgentReachTarget failed: {ex.Message}");
        }
    }

    // AICharacterControl.moveBack
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.moveBack))]
    [HarmonyPostfix]
    private static void OnAICharacterControlmoveBack(AICharacterControl __instance, Vector3 direction)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "moveBack",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlmoveBack failed: {ex.Message}");
        }
    }

    // AICharacterControl.GotoNextPoint
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.GotoNextPoint))]
    [HarmonyPostfix]
    private static void OnAICharacterControlGotoNextPoint(AICharacterControl __instance, Il2CppReferenceArray<Transform> _waypoints)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GotoNextPoint",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlGotoNextPoint failed: {ex.Message}");
        }
    }

    // AICharacterControl.SetStopLoopingDestinationPoints
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.SetStopLoopingDestinationPoints))]
    [HarmonyPostfix]
    private static void OnAICharacterControlSetStopLoopingDestinationPoints(AICharacterControl __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "StopLoopingDestinationPointsSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlSetStopLoopingDestinationPoints failed: {ex.Message}");
        }
    }

    // AICharacterControl.AnimSit
    [HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.AnimSit))]
    [HarmonyPostfix]
    private static void OnAICharacterControlAnimSit(AICharacterControl __instance, bool active)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AnimSit",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterControlAnimSit failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.Start
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.Start))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsStart(AICharacterExpressions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsStart failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.OnDestroy
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.OnDestroy))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsOnDestroy(AICharacterExpressions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsOnDestroy failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.OnCreated
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.OnCreated))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsOnCreated(AICharacterExpressions __instance, UMAData umadata)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnCreated",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsOnCreated failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.Talk
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.Talk))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsTalk(AICharacterExpressions __instance, string sentence)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Talk",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsTalk failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_none
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_none))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_none(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_none",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_none failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_A
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_A))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_A(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_A",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_A failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_O
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_O))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_O(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_O",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_O failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_U
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_U))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_U(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_U",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_U failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_BPM
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_BPM))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_BPM(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_BPM",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_BPM failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_FV
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_FV))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_FV(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_FV",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_FV failed: {ex.Message}");
        }
    }

    // AICharacterExpressions.MouthShape_CDG
    [HarmonyPatch(typeof(AICharacterExpressions), nameof(AICharacterExpressions.MouthShape_CDG))]
    [HarmonyPostfix]
    private static void OnAICharacterExpressionsMouthShape_CDG(AICharacterExpressions __instance, float t)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouthShape_CDG",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAICharacterExpressionsMouthShape_CDG failed: {ex.Message}");
        }
    }

    // AssetManagement.OnEnable
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.OnEnable))]
    [HarmonyPostfix]
    private static void OnAssetManagementOnEnable(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementOnEnable failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterAll
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterAll))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterAll(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterAll",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterAll failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterSwitches
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterSwitches))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterSwitches(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterSwitches",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterSwitches failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterServers
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterServers))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterServers(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterServers",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterServers failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterBroken
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterBroken))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterBroken(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterBroken",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterBroken failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterEOL
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterEOL))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterEOL(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterEOL",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterEOL failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonFilterOff
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonFilterOff))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonFilterOff(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonFilterOff",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonFilterOff failed: {ex.Message}");
        }
    }

    // AssetManagement.SendTechnician
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.SendTechnician))]
    [HarmonyPostfix]
    private static void OnAssetManagementSendTechnician(AssetManagement __instance, NetworkSwitch networkSwitch, Server server)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TechnicianDispatched",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementSendTechnician failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonConfirmSendingTechnician
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonConfirmSendingTechnician))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonConfirmSendingTechnician(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonConfirmSendingTechnician",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonConfirmSendingTechnician failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonCancelSendingTechnician
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonCancelSendingTechnician))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonCancelSendingTechnician(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancelSendingTechnician",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonCancelSendingTechnician failed: {ex.Message}");
        }
    }

    // AssetManagement.UpdateTechnicianInformation
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.UpdateTechnicianInformation))]
    [HarmonyPostfix]
    private static void OnAssetManagementUpdateTechnicianInformation(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TechnicianInformationChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementUpdateTechnicianInformation failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonAddAllBrokenDevicesToQueue
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonAddAllBrokenDevicesToQueue))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonAddAllBrokenDevicesToQueue(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonAddAllBrokenDevicesToQueue",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonAddAllBrokenDevicesToQueue failed: {ex.Message}");
        }
    }

    // AssetManagement.ButtonClearAllWarnings
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonClearAllWarnings))]
    [HarmonyPostfix]
    private static void OnAssetManagementButtonClearAllWarnings(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonClearAllWarnings",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementButtonClearAllWarnings failed: {ex.Message}");
        }
    }

    // AssetManagement.PopulateAutoRepairDropdown
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.PopulateAutoRepairDropdown))]
    [HarmonyPostfix]
    private static void OnAssetManagementPopulateAutoRepairDropdown(AssetManagement __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PopulateAutoRepairDropdown",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementPopulateAutoRepairDropdown failed: {ex.Message}");
        }
    }

    // AssetManagement.OnAutoRepairDropdownChanged
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.OnAutoRepairDropdownChanged))]
    [HarmonyPostfix]
    private static void OnAssetManagementOnAutoRepairDropdownChanged(AssetManagement __instance, int index)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnAutoRepairDropdownChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementOnAutoRepairDropdownChanged failed: {ex.Message}");
        }
    }

    // AssetManagementDeviceLine.SetupLine
    [HarmonyPatch(typeof(AssetManagementDeviceLine), nameof(AssetManagementDeviceLine.SetupLine))]
    [HarmonyPostfix]
    private static void OnAssetManagementDeviceLineSetupLine(AssetManagementDeviceLine __instance, AssetManagementDeviceLineData data, int index)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "upLineSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementDeviceLineSetupLine failed: {ex.Message}");
        }
    }

    // AssetManagementDeviceLine.ButtonClearWarningSign
    [HarmonyPatch(typeof(AssetManagementDeviceLine), nameof(AssetManagementDeviceLine.ButtonClearWarningSign))]
    [HarmonyPostfix]
    private static void OnAssetManagementDeviceLineButtonClearWarningSign(AssetManagementDeviceLine __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonClearWarningSign",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementDeviceLineButtonClearWarningSign failed: {ex.Message}");
        }
    }

    // AssetManagementDeviceLine.ButtonSendTechnician
    [HarmonyPatch(typeof(AssetManagementDeviceLine), nameof(AssetManagementDeviceLine.ButtonSendTechnician))]
    [HarmonyPostfix]
    private static void OnAssetManagementDeviceLineButtonSendTechnician(AssetManagementDeviceLine __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonSendTechnician",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAssetManagementDeviceLineButtonSendTechnician failed: {ex.Message}");
        }
    }

    // AudioManager.Awake
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.Awake))]
    [HarmonyPostfix]
    private static void OnAudioManagerAwake(AudioManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerAwake failed: {ex.Message}");
        }
    }

    // AudioManager.SetMusic
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetMusic))]
    [HarmonyPostfix]
    private static void OnAudioManagerSetMusic(AudioManager __instance, int _clipUID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MusicSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerSetMusic failed: {ex.Message}");
        }
    }

    // AudioManager.PlayEffectAudioClip
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.PlayEffectAudioClip))]
    [HarmonyPostfix]
    private static void OnAudioManagerPlayEffectAudioClip(AudioManager __instance, AudioClip audioClip, float volume, float delayed)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayEffectAudioClip",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerPlayEffectAudioClip failed: {ex.Message}");
        }
    }

    // AudioManager.SetMasterVolume
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetMasterVolume))]
    [HarmonyPostfix]
    private static void OnAudioManagerSetMasterVolume(AudioManager __instance, float _volume)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MasterVolumeSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerSetMasterVolume failed: {ex.Message}");
        }
    }

    // AudioManager.SetEffectsVolume
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetEffectsVolume))]
    [HarmonyPostfix]
    private static void OnAudioManagerSetEffectsVolume(AudioManager __instance, float _volume)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "EffectsVolumeSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerSetEffectsVolume failed: {ex.Message}");
        }
    }

    // AudioManager.SetMusicVolume
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetMusicVolume))]
    [HarmonyPostfix]
    private static void OnAudioManagerSetMusicVolume(AudioManager __instance, float _volume)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MusicVolumeSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerSetMusicVolume failed: {ex.Message}");
        }
    }

    // AudioManager.SetRacksVolume
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.SetRacksVolume))]
    [HarmonyPostfix]
    private static void OnAudioManagerSetRacksVolume(AudioManager __instance, float _volume)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RacksVolumeSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerSetRacksVolume failed: {ex.Message}");
        }
    }

    // AudioManager.PlayRandomRJ45Clip
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.PlayRandomRJ45Clip))]
    [HarmonyPostfix]
    private static void OnAudioManagerPlayRandomRJ45Clip(AudioManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayRandomRJ45Clip",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerPlayRandomRJ45Clip failed: {ex.Message}");
        }
    }

    // AudioManager.PlayRandomImpactClip
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.PlayRandomImpactClip))]
    [HarmonyPostfix]
    private static void OnAudioManagerPlayRandomImpactClip(AudioManager __instance, float _volume)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayRandomImpactClip",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerPlayRandomImpactClip failed: {ex.Message}");
        }
    }

    // AudioManager.PlayRackDoorOpen
    [HarmonyPatch(typeof(AudioManager), nameof(AudioManager.PlayRackDoorOpen))]
    [HarmonyPostfix]
    private static void OnAudioManagerPlayRackDoorOpen(AudioManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayRackDoorOpen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAudioManagerPlayRackDoorOpen failed: {ex.Message}");
        }
    }

    // AutoDisable.OnEnable
    [HarmonyPatch(typeof(AutoDisable), nameof(AutoDisable.OnEnable))]
    [HarmonyPostfix]
    private static void OnAutoDisableOnEnable(AutoDisable __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnAutoDisableOnEnable failed: {ex.Message}");
        }
    }

    // CheckIfTouchingWall.Awake
    [HarmonyPatch(typeof(CheckIfTouchingWall), nameof(CheckIfTouchingWall.Awake))]
    [HarmonyPostfix]
    private static void OnCheckIfTouchingWallAwake(CheckIfTouchingWall __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCheckIfTouchingWallAwake failed: {ex.Message}");
        }
    }

    // CheckIfTouchingWall.Start
    [HarmonyPatch(typeof(CheckIfTouchingWall), nameof(CheckIfTouchingWall.Start))]
    [HarmonyPostfix]
    private static void OnCheckIfTouchingWallStart(CheckIfTouchingWall __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCheckIfTouchingWallStart failed: {ex.Message}");
        }
    }

    // CheckIfTouchingWall.OnDestroy
    [HarmonyPatch(typeof(CheckIfTouchingWall), nameof(CheckIfTouchingWall.OnDestroy))]
    [HarmonyPostfix]
    private static void OnCheckIfTouchingWallOnDestroy(CheckIfTouchingWall __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCheckIfTouchingWallOnDestroy failed: {ex.Message}");
        }
    }

    // CheckIfTouchingWall.PerformOverlapCheck
    [HarmonyPatch(typeof(CheckIfTouchingWall), nameof(CheckIfTouchingWall.PerformOverlapCheck))]
    [HarmonyPostfix]
    private static void OnCheckIfTouchingWallPerformOverlapCheck(CheckIfTouchingWall __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PerformOverlapCheck",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCheckIfTouchingWallPerformOverlapCheck failed: {ex.Message}");
        }
    }

    // CheckIfTouchingWall.SetRenderersEnabled
    [HarmonyPatch(typeof(CheckIfTouchingWall), nameof(CheckIfTouchingWall.SetRenderersEnabled))]
    [HarmonyPostfix]
    private static void OnCheckIfTouchingWallSetRenderersEnabled(CheckIfTouchingWall __instance, bool isEnabled)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RenderersEnabledSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCheckIfTouchingWallSetRenderersEnabled failed: {ex.Message}");
        }
    }

    // CommandCenter.Awake
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.Awake))]
    [HarmonyPostfix]
    private static void OnCommandCenterAwake(CommandCenter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterAwake failed: {ex.Message}");
        }
    }

    // CommandCenter.OnDestroy
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.OnDestroy))]
    [HarmonyPostfix]
    private static void OnCommandCenterOnDestroy(CommandCenter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterOnDestroy failed: {ex.Message}");
        }
    }

    // CommandCenter.ButtonUpgradeCommandCenter
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.ButtonUpgradeCommandCenter))]
    [HarmonyPostfix]
    private static void OnCommandCenterButtonUpgradeCommandCenter(CommandCenter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonUpgradeCommandCenter",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterButtonUpgradeCommandCenter failed: {ex.Message}");
        }
    }

    // CommandCenter.ButtonDowngradeCommandCenter
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.ButtonDowngradeCommandCenter))]
    [HarmonyPostfix]
    private static void OnCommandCenterButtonDowngradeCommandCenter(CommandCenter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonDowngradeCommandCenter",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterButtonDowngradeCommandCenter failed: {ex.Message}");
        }
    }

    // CommandCenter.SpawnOperatorsForLevel
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.SpawnOperatorsForLevel))]
    [HarmonyPostfix]
    private static void OnCommandCenterSpawnOperatorsForLevel(CommandCenter __instance, int level)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OperatorsForLevelSpawned",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterSpawnOperatorsForLevel failed: {ex.Message}");
        }
    }

    // CommandCenter.SpawnOperatorsForSingleLevel
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.SpawnOperatorsForSingleLevel))]
    [HarmonyPostfix]
    private static void OnCommandCenterSpawnOperatorsForSingleLevel(CommandCenter __instance, int level)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OperatorsForSingleLevelSpawned",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterSpawnOperatorsForSingleLevel failed: {ex.Message}");
        }
    }

    // CommandCenter.DestroyOperatorsForLevel
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.DestroyOperatorsForLevel))]
    [HarmonyPostfix]
    private static void OnCommandCenterDestroyOperatorsForLevel(CommandCenter __instance, int level)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DestroyOperatorsForLevel",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterDestroyOperatorsForLevel failed: {ex.Message}");
        }
    }

    // CommandCenter.ToggleClearWarningAuto
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.ToggleClearWarningAuto))]
    [HarmonyPostfix]
    private static void OnCommandCenterToggleClearWarningAuto(CommandCenter __instance, bool isOn)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ToggleClearWarningAuto",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterToggleClearWarningAuto failed: {ex.Message}");
        }
    }

    // CommandCenter.SetAutoRepairMode
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.SetAutoRepairMode))]
    [HarmonyPostfix]
    private static void OnCommandCenterSetAutoRepairMode(CommandCenter __instance, int mode)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AutoRepairModeSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterSetAutoRepairMode failed: {ex.Message}");
        }
    }

    // CommandCenter.OnLoad
    [HarmonyPatch(typeof(CommandCenter), nameof(CommandCenter.OnLoad))]
    [HarmonyPostfix]
    private static void OnCommandCenterOnLoad(CommandCenter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnLoad",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCommandCenterOnLoad failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonShopScreen
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonShopScreen))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonShopScreen(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonShopScreen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonShopScreen failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonNetworkMap
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonNetworkMap))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonNetworkMap(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonNetworkMap",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonNetworkMap failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonAssetManagementScreen
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonAssetManagementScreen))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonAssetManagementScreen(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonAssetManagementScreen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonAssetManagementScreen failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonBalanceSheetScreen
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBalanceSheetScreen))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonBalanceSheetScreen(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonBalanceSheetScreen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonBalanceSheetScreen failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonHireScreen
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonHireScreen))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonHireScreen(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonHireScreen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonHireScreen failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonReturnMainScreen
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonReturnMainScreen))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonReturnMainScreen(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonReturnMainScreen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonReturnMainScreen failed: {ex.Message}");
        }
    }

    // ComputerShop.GetNextAvailableSpawnPoint
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.GetNextAvailableSpawnPoint))]
    [HarmonyPostfix]
    private static void OnComputerShopGetNextAvailableSpawnPoint(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetNextAvailableSpawnPoint",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopGetNextAvailableSpawnPoint failed: {ex.Message}");
        }
    }

    // ComputerShop.FreeUpSpawnPoint
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.FreeUpSpawnPoint))]
    [HarmonyPostfix]
    private static void OnComputerShopFreeUpSpawnPoint(ComputerShop __instance, int spawnIndex)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "FreeUpSpawnPoint",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopFreeUpSpawnPoint failed: {ex.Message}");
        }
    }

    // ComputerShop.GetPrefabForItem
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.GetPrefabForItem))]
    [HarmonyPostfix]
    private static void OnComputerShopGetPrefabForItem(ComputerShop __instance, int itemID, PlayerManager.ObjectInHand itemType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetPrefabForItem",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopGetPrefabForItem failed: {ex.Message}");
        }
    }

    // ComputerShop.HandleObjectives
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.HandleObjectives))]
    [HarmonyPostfix]
    private static void OnComputerShopHandleObjectives(ComputerShop __instance, PlayerManager.ObjectInHand itemType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HandleObjectives",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopHandleObjectives failed: {ex.Message}");
        }
    }

    // ComputerShop.RemoveSpawnedItem
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.RemoveSpawnedItem))]
    [HarmonyPostfix]
    private static void OnComputerShopRemoveSpawnedItem(ComputerShop __instance, int uid)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SpawnedItemRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopRemoveSpawnedItem failed: {ex.Message}");
        }
    }

    // ComputerShop.RemoveCartUIItem
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.RemoveCartUIItem))]
    [HarmonyPostfix]
    private static void OnComputerShopRemoveCartUIItem(ComputerShop __instance, ShopCartItem cartItem)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CartUIItemRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopRemoveCartUIItem failed: {ex.Message}");
        }
    }

    // ComputerShop.SelectNextAvailable
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.SelectNextAvailable))]
    [HarmonyPostfix]
    private static void OnComputerShopSelectNextAvailable(ComputerShop __instance, int removedIndex)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectNextAvailable",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopSelectNextAvailable failed: {ex.Message}");
        }
    }

    // ComputerShop.UpdateCartTotal
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.UpdateCartTotal))]
    [HarmonyPostfix]
    private static void OnComputerShopUpdateCartTotal(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CartTotalChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopUpdateCartTotal failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonCheckOut
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonCheckOut))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonCheckOut(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCheckOut",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonCheckOut failed: {ex.Message}");
        }
    }

    // ComputerShop.ClearTrackingWithoutDestroying
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ClearTrackingWithoutDestroying))]
    [HarmonyPostfix]
    private static void OnComputerShopClearTrackingWithoutDestroying(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClearTrackingWithoutDestroying",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopClearTrackingWithoutDestroying failed: {ex.Message}");
        }
    }

    // ComputerShop.OpenColorPicker
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.OpenColorPicker))]
    [HarmonyPostfix]
    private static void OnComputerShopOpenColorPicker(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OpenColorPicker",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopOpenColorPicker failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonChosenColor
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonChosenColor))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonChosenColor(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonChosenColor",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonChosenColor failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonCancelColorPicker
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonCancelColorPicker))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonCancelColorPicker(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancelColorPicker",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonCancelColorPicker failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonClear
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonClear))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonClear(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonClear",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonClear failed: {ex.Message}");
        }
    }

    // ComputerShop.ButtonCancel
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonCancel))]
    [HarmonyPostfix]
    private static void OnComputerShopButtonCancel(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancel",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopButtonCancel failed: {ex.Message}");
        }
    }

    // ComputerShop.DestroyAllSpawnedItems
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.DestroyAllSpawnedItems))]
    [HarmonyPostfix]
    private static void OnComputerShopDestroyAllSpawnedItems(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DestroyAllSpawnedItems",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopDestroyAllSpawnedItems failed: {ex.Message}");
        }
    }

    // ComputerShop.CleanUpShop
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.CleanUpShop))]
    [HarmonyPostfix]
    private static void OnComputerShopCleanUpShop(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CleanUpShop",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopCleanUpShop failed: {ex.Message}");
        }
    }

    // ComputerShop.CloseShop
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.CloseShop))]
    [HarmonyPostfix]
    private static void OnComputerShopCloseShop(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CloseShop",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopCloseShop failed: {ex.Message}");
        }
    }

    // ComputerShop.UnlockFromSave
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.UnlockFromSave))]
    [HarmonyPostfix]
    private static void OnComputerShopUnlockFromSave(ComputerShop __instance, Il2CppSystem.Collections.Generic.Dictionary<string, bool> savedStates)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "UnlockFromSave",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopUnlockFromSave failed: {ex.Message}");
        }
    }

    // ComputerShop.OnLoad
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.OnLoad))]
    [HarmonyPostfix]
    private static void OnComputerShopOnLoad(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnLoad",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopOnLoad failed: {ex.Message}");
        }
    }

    // ComputerShop.OnDestroy
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.OnDestroy))]
    [HarmonyPostfix]
    private static void OnComputerShopOnDestroy(ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnComputerShopOnDestroy failed: {ex.Message}");
        }
    }

    // FootSteps.Awake
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.Awake))]
    [HarmonyPostfix]
    private static void OnFootStepsAwake(FootSteps __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsAwake failed: {ex.Message}");
        }
    }

    // FootSteps.PlayRequestedStepSound
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.PlayRequestedStepSound))]
    [HarmonyPostfix]
    private static void OnFootStepsPlayRequestedStepSound(FootSteps __instance, int _clipArray)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayRequestedStepSound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsPlayRequestedStepSound failed: {ex.Message}");
        }
    }

    // FootSteps.GetRandomFromRequest
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.GetRandomFromRequest))]
    [HarmonyPostfix]
    private static void OnFootStepsGetRandomFromRequest(FootSteps __instance, int _clipArray)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetRandomFromRequest",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsGetRandomFromRequest failed: {ex.Message}");
        }
    }

    // FootSteps.Step
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.Step))]
    [HarmonyPostfix]
    private static void OnFootStepsStep(FootSteps __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Step",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsStep failed: {ex.Message}");
        }
    }

    // FootSteps.GetRandomClip
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.GetRandomClip))]
    [HarmonyPostfix]
    private static void OnFootStepsGetRandomClip(FootSteps __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetRandomClip",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsGetRandomClip failed: {ex.Message}");
        }
    }

    // FootSteps.OnEnable
    [HarmonyPatch(typeof(FootSteps), nameof(FootSteps.OnEnable))]
    [HarmonyPostfix]
    private static void OnFootStepsOnEnable(FootSteps __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFootStepsOnEnable failed: {ex.Message}");
        }
    }

    // GetCurrentVersion.Start
    [HarmonyPatch(typeof(GetCurrentVersion), nameof(GetCurrentVersion.Start))]
    [HarmonyPostfix]
    private static void OnGetCurrentVersionStart(GetCurrentVersion __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnGetCurrentVersionStart failed: {ex.Message}");
        }
    }

    // GODMOD.Awake
    [HarmonyPatch(typeof(GODMOD), nameof(GODMOD.Awake))]
    [HarmonyPostfix]
    private static void OnGODMODAwake(GODMOD __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnGODMODAwake failed: {ex.Message}");
        }
    }

    // GODMOD.OnEnable
    [HarmonyPatch(typeof(GODMOD), nameof(GODMOD.OnEnable))]
    [HarmonyPostfix]
    private static void OnGODMODOnEnable(GODMOD __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnGODMODOnEnable failed: {ex.Message}");
        }
    }

    // GODMOD.OnDisable
    [HarmonyPatch(typeof(GODMOD), nameof(GODMOD.OnDisable))]
    [HarmonyPostfix]
    private static void OnGODMODOnDisable(GODMOD __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnGODMODOnDisable failed: {ex.Message}");
        }
    }

    // GODMOD.StartGodMod
    [HarmonyPatch(typeof(GODMOD), nameof(GODMOD.StartGodMod))]
    [HarmonyPostfix]
    private static void OnGODMODStartGodMod(GODMOD __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "StartGodMod",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnGODMODStartGodMod failed: {ex.Message}");
        }
    }

    // InputManager.Awake
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.Awake))]
    [HarmonyPostfix]
    private static void OnInputManagerAwake(InputManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnInputManagerAwake failed: {ex.Message}");
        }
    }

    // InputManager.OnDestroy
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.OnDestroy))]
    [HarmonyPostfix]
    private static void OnInputManagerOnDestroy(InputManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnInputManagerOnDestroy failed: {ex.Message}");
        }
    }

    // InputManager.CheckCurrentControls
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.CheckCurrentControls))]
    [HarmonyPostfix]
    private static void OnInputManagerCheckCurrentControls(InputManager __instance, InputAction.CallbackContext ctx)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CheckCurrentControls",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnInputManagerCheckCurrentControls failed: {ex.Message}");
        }
    }

    // LocalisedText.Start
    [HarmonyPatch(typeof(LocalisedText), nameof(LocalisedText.Start))]
    [HarmonyPostfix]
    private static void OnLocalisedTextStart(LocalisedText __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnLocalisedTextStart failed: {ex.Message}");
        }
    }

    // LocalisedText.ChangeText
    [HarmonyPatch(typeof(LocalisedText), nameof(LocalisedText.ChangeText))]
    [HarmonyPostfix]
    private static void OnLocalisedTextChangeText(LocalisedText __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ChangeText",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnLocalisedTextChangeText failed: {ex.Message}");
        }
    }

    // LocalisedText.SetText
    [HarmonyPatch(typeof(LocalisedText), nameof(LocalisedText.SetText))]
    [HarmonyPostfix]
    private static void OnLocalisedTextSetText(LocalisedText __instance, int _localisation_uid)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TextSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnLocalisedTextSetText failed: {ex.Message}");
        }
    }

    // LocalisedText.OnDestroy
    [HarmonyPatch(typeof(LocalisedText), nameof(LocalisedText.OnDestroy))]
    [HarmonyPostfix]
    private static void OnLocalisedTextOnDestroy(LocalisedText __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnLocalisedTextOnDestroy failed: {ex.Message}");
        }
    }

    // MainGameManager.Awake
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.Awake))]
    [HarmonyPostfix]
    private static void OnMainGameManagerAwake(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerAwake failed: {ex.Message}");
        }
    }

    // MainGameManager.Start
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.Start))]
    [HarmonyPostfix]
    private static void OnMainGameManagerStart(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerStart failed: {ex.Message}");
        }
    }

    // MainGameManager.ResetTrolleyPosition
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ResetTrolleyPosition))]
    [HarmonyPostfix]
    private static void OnMainGameManagerResetTrolleyPosition(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetTrolleyPosition",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerResetTrolleyPosition failed: {ex.Message}");
        }
    }

    // MainGameManager.GetServerPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetServerPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetServerPrefab(MainGameManager __instance, int serverType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetServerPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetServerPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetSwitchPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSwitchPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetSwitchPrefab(MainGameManager __instance, int switchType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSwitchPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetSwitchPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetPatchPanelPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetPatchPanelPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetPatchPanelPrefab(MainGameManager __instance, int switchType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetPatchPanelPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetPatchPanelPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetCableSpinnerPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCableSpinnerPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetCableSpinnerPrefab(MainGameManager __instance, int prefabID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCableSpinnerPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetCableSpinnerPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetSfpPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetSfpPrefab(MainGameManager __instance, int prefabID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSfpPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetSfpPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetSfpBoxPrefab
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpBoxPrefab))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetSfpBoxPrefab(MainGameManager __instance, int prefabID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSfpBoxPrefab",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetSfpBoxPrefab failed: {ex.Message}");
        }
    }

    // MainGameManager.GetCustomerItemByID
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCustomerItemByID))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetCustomerItemByID(MainGameManager __instance, int customerID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCustomerItemByID",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetCustomerItemByID failed: {ex.Message}");
        }
    }

    // MainGameManager.ShuffleAvailableCustomers
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShuffleAvailableCustomers))]
    [HarmonyPostfix]
    private static void OnMainGameManagerShuffleAvailableCustomers(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShuffleAvailableCustomers",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerShuffleAvailableCustomers failed: {ex.Message}");
        }
    }

    // MainGameManager.ShuffleAvailableSubnets
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShuffleAvailableSubnets))]
    [HarmonyPostfix]
    private static void OnMainGameManagerShuffleAvailableSubnets(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShuffleAvailableSubnets",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerShuffleAvailableSubnets failed: {ex.Message}");
        }
    }

    // MainGameManager.GetAppLogo
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetAppLogo))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetAppLogo(MainGameManager __instance, int customerID, int appID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetAppLogo",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetAppLogo failed: {ex.Message}");
        }
    }

    // MainGameManager.GetCustomerLogo
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCustomerLogo))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetCustomerLogo(MainGameManager __instance, int customerID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCustomerLogo",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetCustomerLogo failed: {ex.Message}");
        }
    }

    // MainGameManager.GetFreeSubnet
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetFreeSubnet))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetFreeSubnet(MainGameManager __instance, float appRequirements)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetFreeSubnet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetFreeSubnet failed: {ex.Message}");
        }
    }

    // MainGameManager.IsSubnetValid
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.IsSubnetValid))]
    [HarmonyPostfix]
    private static void OnMainGameManagerIsSubnetValid(MainGameManager __instance, string subnet)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "IsSubnetValid",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerIsSubnetValid failed: {ex.Message}");
        }
    }

    // MainGameManager.ShowCustomerCardsCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShowCustomerCardsCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerShowCustomerCardsCanvas(MainGameManager __instance, CustomerBaseDoor _door)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowCustomerCardsCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerShowCustomerCardsCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.CreateFallbackCustomer
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.CreateFallbackCustomer))]
    [HarmonyPostfix]
    private static void OnMainGameManagerCreateFallbackCustomer(MainGameManager __instance, CustomerItem original, int customerBaseID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CreateFallbackCustomer",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerCreateFallbackCustomer failed: {ex.Message}");
        }
    }

    // MainGameManager.ButtonCustomerChosen
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCustomerChosen))]
    [HarmonyPostfix]
    private static void OnMainGameManagerButtonCustomerChosen(MainGameManager __instance, int _cardID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCustomerChosen",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerButtonCustomerChosen failed: {ex.Message}");
        }
    }

    // MainGameManager.ButtonCancelCustomerChoice
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCancelCustomerChoice))]
    [HarmonyPostfix]
    private static void OnMainGameManagerButtonCancelCustomerChoice(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancelCustomerChoice",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerButtonCancelCustomerChoice failed: {ex.Message}");
        }
    }

    // MainGameManager.ShowBuyWallCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShowBuyWallCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerShowBuyWallCanvas(MainGameManager __instance, Wall wall)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowBuyWallCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerShowBuyWallCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.ButtonBuyWall
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonBuyWall))]
    [HarmonyPostfix]
    private static void OnMainGameManagerButtonBuyWall(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonBuyWall",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerButtonBuyWall failed: {ex.Message}");
        }
    }

    // MainGameManager.ButtonCancelBuyWall
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCancelBuyWall))]
    [HarmonyPostfix]
    private static void OnMainGameManagerButtonCancelBuyWall(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancelBuyWall",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerButtonCancelBuyWall failed: {ex.Message}");
        }
    }

    // MainGameManager.ShowNetworkConfigCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShowNetworkConfigCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerShowNetworkConfigCanvas(MainGameManager __instance, NetworkSwitch networkSwitch)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowNetworkConfigCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerShowNetworkConfigCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.CloseNetworkConfigCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.CloseNetworkConfigCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerCloseNetworkConfigCanvas(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CloseNetworkConfigCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerCloseNetworkConfigCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.OpenAnyCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.OpenAnyCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerOpenAnyCanvas(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OpenAnyCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerOpenAnyCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.CloseAnyCanvas
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.CloseAnyCanvas))]
    [HarmonyPostfix]
    private static void OnMainGameManagerCloseAnyCanvas(MainGameManager __instance, bool isCustomerChoice)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CloseAnyCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerCloseAnyCanvas failed: {ex.Message}");
        }
    }

    // MainGameManager.RemoveUsedSubnet
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.RemoveUsedSubnet))]
    [HarmonyPostfix]
    private static void OnMainGameManagerRemoveUsedSubnet(MainGameManager __instance, string subnet)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "UsedSubnetRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerRemoveUsedSubnet failed: {ex.Message}");
        }
    }

    // MainGameManager.ReturnSubnet
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnSubnet))]
    [HarmonyPostfix]
    private static void OnMainGameManagerReturnSubnet(MainGameManager __instance, string subnet)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ReturnSubnet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerReturnSubnet failed: {ex.Message}");
        }
    }

    // MainGameManager.OnLoad
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.OnLoad))]
    [HarmonyPostfix]
    private static void OnMainGameManagerOnLoad(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnLoad",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerOnLoad failed: {ex.Message}");
        }
    }

    // MainGameManager.OnDestroy
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.OnDestroy))]
    [HarmonyPostfix]
    private static void OnMainGameManagerOnDestroy(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerOnDestroy failed: {ex.Message}");
        }
    }

    // MainGameManager.SetAutoSaveInterval
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.SetAutoSaveInterval))]
    [HarmonyPostfix]
    private static void OnMainGameManagerSetAutoSaveInterval(MainGameManager __instance, float minutes)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AutoSaveIntervalSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerSetAutoSaveInterval failed: {ex.Message}");
        }
    }

    // MainGameManager.SetAutoSaveEnabled
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.SetAutoSaveEnabled))]
    [HarmonyPostfix]
    private static void OnMainGameManagerSetAutoSaveEnabled(MainGameManager __instance, bool enabled)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AutoSaveEnabledSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerSetAutoSaveEnabled failed: {ex.Message}");
        }
    }

    // MainGameManager.RestartAutoSave
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.RestartAutoSave))]
    [HarmonyPostfix]
    private static void OnMainGameManagerRestartAutoSave(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RestartAutoSave",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerRestartAutoSave failed: {ex.Message}");
        }
    }

    // MainGameManager.ReturnServerNameFromType
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnServerNameFromType))]
    [HarmonyPostfix]
    private static void OnMainGameManagerReturnServerNameFromType(MainGameManager __instance, int type)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ReturnServerNameFromType",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerReturnServerNameFromType failed: {ex.Message}");
        }
    }

    // MainGameManager.ReturnSwitchNameFromType
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnSwitchNameFromType))]
    [HarmonyPostfix]
    private static void OnMainGameManagerReturnSwitchNameFromType(MainGameManager __instance, int type)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ReturnSwitchNameFromType",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerReturnSwitchNameFromType failed: {ex.Message}");
        }
    }

    // MainGameManager.LoadTrolleyPosition
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.LoadTrolleyPosition))]
    [HarmonyPostfix]
    private static void OnMainGameManagerLoadTrolleyPosition(MainGameManager __instance, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TrolleyPositionLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerLoadTrolleyPosition failed: {ex.Message}");
        }
    }

    // MainGameManager.GetCustomerTotalRequirement
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCustomerTotalRequirement))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetCustomerTotalRequirement(MainGameManager __instance, CustomerItem customer)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCustomerTotalRequirement",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetCustomerTotalRequirement failed: {ex.Message}");
        }
    }

    // MainGameManager.IsCustomerSuitableForBase
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.IsCustomerSuitableForBase))]
    [HarmonyPostfix]
    private static void OnMainGameManagerIsCustomerSuitableForBase(MainGameManager __instance, CustomerItem customer, int customerBaseID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "IsCustomerSuitableForBase",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerIsCustomerSuitableForBase failed: {ex.Message}");
        }
    }

    // MainGameManager.InitializeVlanPool
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.InitializeVlanPool))]
    [HarmonyPostfix]
    private static void OnMainGameManagerInitializeVlanPool(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InitializeVlanPool",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerInitializeVlanPool failed: {ex.Message}");
        }
    }

    // MainGameManager.GetFreeVlanId
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetFreeVlanId))]
    [HarmonyPostfix]
    private static void OnMainGameManagerGetFreeVlanId(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetFreeVlanId",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerGetFreeVlanId failed: {ex.Message}");
        }
    }

    // MainGameManager.ReturnVlanId
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnVlanId))]
    [HarmonyPostfix]
    private static void OnMainGameManagerReturnVlanId(MainGameManager __instance, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ReturnVlanId",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerReturnVlanId failed: {ex.Message}");
        }
    }

    // MainGameManager.RemoveUsedVlanId
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.RemoveUsedVlanId))]
    [HarmonyPostfix]
    private static void OnMainGameManagerRemoveUsedVlanId(MainGameManager __instance, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "UsedVlanIdRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerRemoveUsedVlanId failed: {ex.Message}");
        }
    }

    // MainGameManager.OnApplicationQuit
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.OnApplicationQuit))]
    [HarmonyPostfix]
    private static void OnMainGameManagerOnApplicationQuit(MainGameManager __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnApplicationQuit",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainGameManagerOnApplicationQuit failed: {ex.Message}");
        }
    }

    // PatchPanel.GetPairedLink
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.GetPairedLink))]
    [HarmonyPostfix]
    private static void OnPatchPanelGetPairedLink(PatchPanel __instance, CableLink link)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetPairedLink",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPatchPanelGetPairedLink failed: {ex.Message}");
        }
    }

    // PatchPanel.IsAnyCableConnected
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.IsAnyCableConnected))]
    [HarmonyPostfix]
    private static void OnPatchPanelIsAnyCableConnected(PatchPanel __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "IsAnyCableConnected",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPatchPanelIsAnyCableConnected failed: {ex.Message}");
        }
    }

    // PatchPanel.InsertedInRack
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.InsertedInRack))]
    [HarmonyPostfix]
    private static void OnPatchPanelInsertedInRack(PatchPanel __instance, PatchPanelSaveData saveData)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InsertedInRack",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPatchPanelInsertedInRack failed: {ex.Message}");
        }
    }

    // PatchPanel.GenerateUniquePatchPanelId
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.GenerateUniquePatchPanelId))]
    [HarmonyPostfix]
    private static void OnPatchPanelGenerateUniquePatchPanelId(PatchPanel __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GenerateUniquePatchPanelId",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPatchPanelGenerateUniquePatchPanelId failed: {ex.Message}");
        }
    }

    // PatchPanel.ValidateRackPosition
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.ValidateRackPosition))]
    [HarmonyPostfix]
    private static void OnPatchPanelValidateRackPosition(PatchPanel __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ValidateRackPosition",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPatchPanelValidateRackPosition failed: {ex.Message}");
        }
    }

    // PositionIndicator.Awake
    [HarmonyPatch(typeof(PositionIndicator), nameof(PositionIndicator.Awake))]
    [HarmonyPostfix]
    private static void OnPositionIndicatorAwake(PositionIndicator __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnPositionIndicatorAwake failed: {ex.Message}");
        }
    }

    // ShopCartItem.AddSpawnedItem
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.AddSpawnedItem))]
    [HarmonyPostfix]
    private static void OnShopCartItemAddSpawnedItem(ShopCartItem __instance, int uid)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SpawnedItemAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemAddSpawnedItem failed: {ex.Message}");
        }
    }

    // ShopCartItem.RemoveLastSpawnedItem
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.RemoveLastSpawnedItem))]
    [HarmonyPostfix]
    private static void OnShopCartItemRemoveLastSpawnedItem(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "LastSpawnedItemRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemRemoveLastSpawnedItem failed: {ex.Message}");
        }
    }

    // ShopCartItem.ClearAllUIDs
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.ClearAllUIDs))]
    [HarmonyPostfix]
    private static void OnShopCartItemClearAllUIDs(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClearAllUIDs",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemClearAllUIDs failed: {ex.Message}");
        }
    }

    // ShopCartItem.OnAddClicked
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.OnAddClicked))]
    [HarmonyPostfix]
    private static void OnShopCartItemOnAddClicked(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnAddClicked",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemOnAddClicked failed: {ex.Message}");
        }
    }

    // ShopCartItem.OnRemoveClicked
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.OnRemoveClicked))]
    [HarmonyPostfix]
    private static void OnShopCartItemOnRemoveClicked(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnRemoveClicked",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemOnRemoveClicked failed: {ex.Message}");
        }
    }

    // ShopCartItem.UpdateDisplay
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.UpdateDisplay))]
    [HarmonyPostfix]
    private static void OnShopCartItemUpdateDisplay(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DisplayChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemUpdateDisplay failed: {ex.Message}");
        }
    }

    // ShopCartItem.OnDestroy
    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.OnDestroy))]
    [HarmonyPostfix]
    private static void OnShopCartItemOnDestroy(ShopCartItem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnShopCartItemOnDestroy failed: {ex.Message}");
        }
    }

    // StaticUIElements.Awake
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.Awake))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsAwake(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsAwake failed: {ex.Message}");
        }
    }

    // StaticUIElements.Start
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.Start))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsStart(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsStart failed: {ex.Message}");
        }
    }

    // StaticUIElements.SetNotification
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.SetNotification))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsSetNotification(StaticUIElements __instance, int _localisationUID, Sprite _sprite, string _text)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "NotificationSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsSetNotification failed: {ex.Message}");
        }
    }

    // StaticUIElements.ShowStaticCanvas
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ShowStaticCanvas))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsShowStaticCanvas(StaticUIElements __instance, bool active)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowStaticCanvas",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsShowStaticCanvas failed: {ex.Message}");
        }
    }

    // StaticUIElements.RemoveCustomKeyHint
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.RemoveCustomKeyHint))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsRemoveCustomKeyHint(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CustomKeyHintRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsRemoveCustomKeyHint failed: {ex.Message}");
        }
    }

    // StaticUIElements.InstantiateParticleUpgrade
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.InstantiateParticleUpgrade))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsInstantiateParticleUpgrade(StaticUIElements __instance, Transform _transform)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InstantiateParticleUpgrade",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsInstantiateParticleUpgrade failed: {ex.Message}");
        }
    }

    // StaticUIElements.UpdateMessageDisplay
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.UpdateMessageDisplay))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsUpdateMessageDisplay(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MessageDisplayChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsUpdateMessageDisplay failed: {ex.Message}");
        }
    }

    // StaticUIElements.AddMeesageInField
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.AddMeesageInField))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsAddMeesageInField(StaticUIElements __instance, string message)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MeesageInFieldAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsAddMeesageInField failed: {ex.Message}");
        }
    }

    // StaticUIElements.InstantiateErrorWarningSign
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.InstantiateErrorWarningSign))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsInstantiateErrorWarningSign(StaticUIElements __instance, bool isError, Vector3 objectPos)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InstantiateErrorWarningSign",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsInstantiateErrorWarningSign failed: {ex.Message}");
        }
    }

    // StaticUIElements.DestroyErrorWarningSign
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.DestroyErrorWarningSign))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsDestroyErrorWarningSign(StaticUIElements __instance, int errorWarningUID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DestroyErrorWarningSign",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsDestroyErrorWarningSign failed: {ex.Message}");
        }
    }

    // StaticUIElements.ShowSpriteNextToPointer
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ShowSpriteNextToPointer))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsShowSpriteNextToPointer(StaticUIElements __instance, Sprite _sprite)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowSpriteNextToPointer",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsShowSpriteNextToPointer failed: {ex.Message}");
        }
    }

    // StaticUIElements.ClearSpriteNextToPointer
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ClearSpriteNextToPointer))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsClearSpriteNextToPointer(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClearSpriteNextToPointer",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsClearSpriteNextToPointer failed: {ex.Message}");
        }
    }

    // StaticUIElements.ShowTextUnderCursor
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ShowTextUnderCursor))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsShowTextUnderCursor(StaticUIElements __instance, string text)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowTextUnderCursor",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsShowTextUnderCursor failed: {ex.Message}");
        }
    }

    // StaticUIElements.HideTextUnderCursor
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.HideTextUnderCursor))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsHideTextUnderCursor(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HideTextUnderCursor",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsHideTextUnderCursor failed: {ex.Message}");
        }
    }

    // StaticUIElements.UpdateHoldProgress
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.UpdateHoldProgress))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsUpdateHoldProgress(StaticUIElements __instance, float value)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HoldProgressChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsUpdateHoldProgress failed: {ex.Message}");
        }
    }

    // StaticUIElements.SetLoadingInfo
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.SetLoadingInfo))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsSetLoadingInfo(StaticUIElements __instance, string s)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "LoadingInfoSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsSetLoadingInfo failed: {ex.Message}");
        }
    }

    // StaticUIElements.OnLoadingStarted
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.OnLoadingStarted))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsOnLoadingStarted(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnLoadingStarted",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsOnLoadingStarted failed: {ex.Message}");
        }
    }

    // StaticUIElements.ButtonSaveInputTextOverlay
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ButtonSaveInputTextOverlay))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsButtonSaveInputTextOverlay(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonSaveInputTextOverlay",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsButtonSaveInputTextOverlay failed: {ex.Message}");
        }
    }

    // StaticUIElements.ButtonCancelInputTextOverlay
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.ButtonCancelInputTextOverlay))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsButtonCancelInputTextOverlay(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ButtonCancelInputTextOverlay",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsButtonCancelInputTextOverlay failed: {ex.Message}");
        }
    }

    // StaticUIElements.RestorePreviousSelection
    [HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.RestorePreviousSelection))]
    [HarmonyPostfix]
    private static void OnStaticUIElementsRestorePreviousSelection(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RestorePreviousSelection",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnStaticUIElementsRestorePreviousSelection failed: {ex.Message}");
        }
    }

    // TerrainDetector.ConvertToSplatMapCoordinate
    [HarmonyPatch(typeof(TerrainDetector), nameof(TerrainDetector.ConvertToSplatMapCoordinate))]
    [HarmonyPostfix]
    private static void OnTerrainDetectorConvertToSplatMapCoordinate(TerrainDetector __instance, Vector3 worldPosition)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ConvertToSplatMapCoordinate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTerrainDetectorConvertToSplatMapCoordinate failed: {ex.Message}");
        }
    }

    // TerrainDetector.GetActiveTerrainTextureIdx
    [HarmonyPatch(typeof(TerrainDetector), nameof(TerrainDetector.GetActiveTerrainTextureIdx))]
    [HarmonyPostfix]
    private static void OnTerrainDetectorGetActiveTerrainTextureIdx(TerrainDetector __instance, Vector3 position)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetActiveTerrainTextureIdx",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTerrainDetectorGetActiveTerrainTextureIdx failed: {ex.Message}");
        }
    }

    // TerrainDetector.SetCurrentTerrain
    [HarmonyPatch(typeof(TerrainDetector), nameof(TerrainDetector.SetCurrentTerrain))]
    [HarmonyPostfix]
    private static void OnTerrainDetectorSetCurrentTerrain(TerrainDetector __instance, Terrain _terrain)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CurrentTerrainSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTerrainDetectorSetCurrentTerrain failed: {ex.Message}");
        }
    }

    // TimeController.Awake
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.Awake))]
    [HarmonyPostfix]
    private static void OnTimeControllerAwake(TimeController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerAwake failed: {ex.Message}");
        }
    }

    // TimeController.Start
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.Start))]
    [HarmonyPostfix]
    private static void OnTimeControllerStart(TimeController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerStart failed: {ex.Message}");
        }
    }

    // TimeController.TimeIsBetween
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.TimeIsBetween))]
    [HarmonyPostfix]
    private static void OnTimeControllerTimeIsBetween(TimeController __instance, float startHour, float endHour)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TimeIsBetween",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerTimeIsBetween failed: {ex.Message}");
        }
    }

    // TimeController.CurrentTimeInHours
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.CurrentTimeInHours))]
    [HarmonyPostfix]
    private static void OnTimeControllerCurrentTimeInHours(TimeController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CurrentTimeInHours",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerCurrentTimeInHours failed: {ex.Message}");
        }
    }

    // TimeController.HoursFromDate
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.HoursFromDate))]
    [HarmonyPostfix]
    private static void OnTimeControllerHoursFromDate(TimeController __instance, float _time, int _day)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HoursFromDate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerHoursFromDate failed: {ex.Message}");
        }
    }

    // TimeController.OnDisable
    [HarmonyPatch(typeof(TimeController), nameof(TimeController.OnDisable))]
    [HarmonyPostfix]
    private static void OnTimeControllerOnDisable(TimeController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTimeControllerOnDisable failed: {ex.Message}");
        }
    }

    // UsableObject.MoveBetweenPositions
    [HarmonyPatch(typeof(UsableObject), nameof(UsableObject.MoveBetweenPositions))]
    [HarmonyPostfix]
    private static void OnUsableObjectMoveBetweenPositions(UsableObject __instance, Vector3 _position, Vector3 _rotation)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MoveBetweenPositions",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnUsableObjectMoveBetweenPositions failed: {ex.Message}");
        }
    }

    // UsableObject.OnLoadDestroy
    [HarmonyPatch(typeof(UsableObject), nameof(UsableObject.OnLoadDestroy))]
    [HarmonyPostfix]
    private static void OnUsableObjectOnLoadDestroy(UsableObject __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnLoadDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnUsableObjectOnLoadDestroy failed: {ex.Message}");
        }
    }

    // UsableObject.OnCollisionEnter
    [HarmonyPatch(typeof(UsableObject), nameof(UsableObject.OnCollisionEnter))]
    [HarmonyPostfix]
    private static void OnUsableObjectOnCollisionEnter(UsableObject __instance, Collision collision)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnCollisionEnter",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnUsableObjectOnCollisionEnter failed: {ex.Message}");
        }
    }

    // UsableObject.RemoveRigidbody
    [HarmonyPatch(typeof(UsableObject), nameof(UsableObject.RemoveRigidbody))]
    [HarmonyPostfix]
    private static void OnUsableObjectRemoveRigidbody(UsableObject __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RigidbodyRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnUsableObjectRemoveRigidbody failed: {ex.Message}");
        }
    }

    // UsableObject.RestoreRigidbody
    [HarmonyPatch(typeof(UsableObject), nameof(UsableObject.RestoreRigidbody))]
    [HarmonyPostfix]
    private static void OnUsableObjectRestoreRigidbody(UsableObject __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RestoreRigidbody",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnUsableObjectRestoreRigidbody failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.GetCableCurrentSpeed
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.GetCableCurrentSpeed))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemGetCableCurrentSpeed(WaypointInitializationSystem __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCableCurrentSpeed",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemGetCableCurrentSpeed failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.GetAllCables
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.GetAllCables))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemGetAllCables(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetAllCables",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemGetAllCables failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.UpdateCableInfo
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.UpdateCableInfo))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemUpdateCableInfo(WaypointInitializationSystem __instance, int cableId, WaypointInitializationSystem.CableInfo info)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CableInfoChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemUpdateCableInfo failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.ClearNetworkState
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.ClearNetworkState))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemClearNetworkState(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClearNetworkState",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemClearNetworkState failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.CreateCableWithSpawners
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.CreateCableWithSpawners))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemCreateCableWithSpawners(WaypointInitializationSystem __instance, int cableId, Il2CppSystem.Collections.Generic.List<Vector3> positions)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CreateCableWithSpawners",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemCreateCableWithSpawners failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.UpdateServerCustomerID
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.UpdateServerCustomerID))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemUpdateServerCustomerID(WaypointInitializationSystem __instance, string serverID, int customerID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ServerCustomerIDChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemUpdateServerCustomerID failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.RequestRouteEvaluation
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.RequestRouteEvaluation))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemRequestRouteEvaluation(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RequestRouteEvaluation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemRequestRouteEvaluation failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.SetEvaluationCooldown
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.SetEvaluationCooldown))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemSetEvaluationCooldown(WaypointInitializationSystem __instance, float seconds)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "EvaluationCooldownSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemSetEvaluationCooldown failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.GetEvaluationCooldown
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.GetEvaluationCooldown))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemGetEvaluationCooldown(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetEvaluationCooldown",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemGetEvaluationCooldown failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.SetPacketSpawnerEnabled
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.SetPacketSpawnerEnabled))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemSetPacketSpawnerEnabled(WaypointInitializationSystem __instance, bool enabled)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PacketSpawnerEnabledSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemSetPacketSpawnerEnabled failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.EvaluateAllRoutes
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.EvaluateAllRoutes))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemEvaluateAllRoutes(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "EvaluateAllRoutes",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemEvaluateAllRoutes failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.GetServerProcessingSpeed
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.GetServerProcessingSpeed))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemGetServerProcessingSpeed(WaypointInitializationSystem __instance, string serverName)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetServerProcessingSpeed",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemGetServerProcessingSpeed failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.GetCustomerRoutes
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.GetCustomerRoutes))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemGetCustomerRoutes(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetCustomerRoutes",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemGetCustomerRoutes failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.ResetAllSpawners
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.ResetAllSpawners))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemResetAllSpawners(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetAllSpawners",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemResetAllSpawners failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.RegisterCableInNetworkMap
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.RegisterCableInNetworkMap))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemRegisterCableInNetworkMap(WaypointInitializationSystem __instance, WaypointInitializationSystem.CableInfo cableInfo)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RegisterCableInNetworkMap",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemRegisterCableInNetworkMap failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.OnCableRemoved
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.OnCableRemoved))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemOnCableRemoved(WaypointInitializationSystem __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnCableRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemOnCableRemoved failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.DoesCableServeMultipleCustomers
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.DoesCableServeMultipleCustomers))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemDoesCableServeMultipleCustomers(WaypointInitializationSystem __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DoesCableServeMultipleCustomers",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemDoesCableServeMultipleCustomers failed: {ex.Message}");
        }
    }

    // WaypointInitializationSystem.CleanUpSystem
    [HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.CleanUpSystem))]
    [HarmonyPostfix]
    private static void OnWaypointInitializationSystemCleanUpSystem(WaypointInitializationSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CleanUpSystem",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWaypointInitializationSystemCleanUpSystem failed: {ex.Message}");
        }
    }

    // CarController.Start
    [HarmonyPatch(typeof(CarController), nameof(CarController.Start))]
    [HarmonyPostfix]
    private static void OnCarControllerStart(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerStart failed: {ex.Message}");
        }
    }

    // CarController.Move
    [HarmonyPatch(typeof(CarController), nameof(CarController.Move))]
    [HarmonyPostfix]
    private static void OnCarControllerMove(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Move",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerMove failed: {ex.Message}");
        }
    }

    // CarController.Steer
    [HarmonyPatch(typeof(CarController), nameof(CarController.Steer))]
    [HarmonyPostfix]
    private static void OnCarControllerSteer(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Steer",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerSteer failed: {ex.Message}");
        }
    }

    // CarController.BrakeAndDeacceleration
    [HarmonyPatch(typeof(CarController), nameof(CarController.BrakeAndDeacceleration))]
    [HarmonyPostfix]
    private static void OnCarControllerBrakeAndDeacceleration(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "BrakeAndDeacceleration",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerBrakeAndDeacceleration failed: {ex.Message}");
        }
    }

    // CarController.TakeTheWheel
    [HarmonyPatch(typeof(CarController), nameof(CarController.TakeTheWheel))]
    [HarmonyPostfix]
    private static void OnCarControllerTakeTheWheel(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TakeTheWheel",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerTakeTheWheel failed: {ex.Message}");
        }
    }

    // CarController.LeaveTheTrolley
    [HarmonyPatch(typeof(CarController), nameof(CarController.LeaveTheTrolley))]
    [HarmonyPostfix]
    private static void OnCarControllerLeaveTheTrolley(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "LeaveTheTrolley",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerLeaveTheTrolley failed: {ex.Message}");
        }
    }

    // CarController.StopCar
    [HarmonyPatch(typeof(CarController), nameof(CarController.StopCar))]
    [HarmonyPostfix]
    private static void OnCarControllerStopCar(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "StopCar",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerStopCar failed: {ex.Message}");
        }
    }

    // CarController.ResetTrolleyPosition
    [HarmonyPatch(typeof(CarController), nameof(CarController.ResetTrolleyPosition))]
    [HarmonyPostfix]
    private static void OnCarControllerResetTrolleyPosition(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetTrolleyPosition",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerResetTrolleyPosition failed: {ex.Message}");
        }
    }

    // CarController.HandleAudio
    [HarmonyPatch(typeof(CarController), nameof(CarController.HandleAudio))]
    [HarmonyPostfix]
    private static void OnCarControllerHandleAudio(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HandleAudio",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerHandleAudio failed: {ex.Message}");
        }
    }

    // CarController.TurnOffCollidersInTrolley
    [HarmonyPatch(typeof(CarController), nameof(CarController.TurnOffCollidersInTrolley))]
    [HarmonyPostfix]
    private static void OnCarControllerTurnOffCollidersInTrolley(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TurnOffCollidersInTrolley",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerTurnOffCollidersInTrolley failed: {ex.Message}");
        }
    }

    // CarController.TurnBackOnCollidersInTRolley
    [HarmonyPatch(typeof(CarController), nameof(CarController.TurnBackOnCollidersInTRolley))]
    [HarmonyPostfix]
    private static void OnCarControllerTurnBackOnCollidersInTRolley(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TurnBackOnCollidersInTRolley",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerTurnBackOnCollidersInTRolley failed: {ex.Message}");
        }
    }

    // CarController.OnCollisionEnter
    [HarmonyPatch(typeof(CarController), nameof(CarController.OnCollisionEnter))]
    [HarmonyPostfix]
    private static void OnCarControllerOnCollisionEnter(CarController __instance, Collision collision)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnCollisionEnter",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerOnCollisionEnter failed: {ex.Message}");
        }
    }

    // CarController.OnDestroy
    [HarmonyPatch(typeof(CarController), nameof(CarController.OnDestroy))]
    [HarmonyPostfix]
    private static void OnCarControllerOnDestroy(CarController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCarControllerOnDestroy failed: {ex.Message}");
        }
    }

    // Benchmark02.Start
    [HarmonyPatch(typeof(Benchmark02), nameof(Benchmark02.Start))]
    [HarmonyPostfix]
    private static void OnBenchmark02Start(Benchmark02 __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBenchmark02Start failed: {ex.Message}");
        }
    }

    // Benchmark03.Awake
    [HarmonyPatch(typeof(Benchmark03), nameof(Benchmark03.Awake))]
    [HarmonyPostfix]
    private static void OnBenchmark03Awake(Benchmark03 __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBenchmark03Awake failed: {ex.Message}");
        }
    }

    // Benchmark03.Start
    [HarmonyPatch(typeof(Benchmark03), nameof(Benchmark03.Start))]
    [HarmonyPostfix]
    private static void OnBenchmark03Start(Benchmark03 __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBenchmark03Start failed: {ex.Message}");
        }
    }

    // Benchmark04.Start
    [HarmonyPatch(typeof(Benchmark04), nameof(Benchmark04.Start))]
    [HarmonyPostfix]
    private static void OnBenchmark04Start(Benchmark04 __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBenchmark04Start failed: {ex.Message}");
        }
    }

    // CameraController.Awake
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.Awake))]
    [HarmonyPostfix]
    private static void OnCameraControllerAwake(CameraController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCameraControllerAwake failed: {ex.Message}");
        }
    }

    // CameraController.Start
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))]
    [HarmonyPostfix]
    private static void OnCameraControllerStart(CameraController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCameraControllerStart failed: {ex.Message}");
        }
    }

    // CameraController.GetPlayerInput
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.GetPlayerInput))]
    [HarmonyPostfix]
    private static void OnCameraControllerGetPlayerInput(CameraController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetPlayerInput",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCameraControllerGetPlayerInput failed: {ex.Message}");
        }
    }

    // ObjectSpin.Awake
    [HarmonyPatch(typeof(ObjectSpin), nameof(ObjectSpin.Awake))]
    [HarmonyPostfix]
    private static void OnObjectSpinAwake(ObjectSpin __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnObjectSpinAwake failed: {ex.Message}");
        }
    }

    // SimpleScript.Start
    [HarmonyPatch(typeof(SimpleScript), nameof(SimpleScript.Start))]
    [HarmonyPostfix]
    private static void OnSimpleScriptStart(SimpleScript __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSimpleScriptStart failed: {ex.Message}");
        }
    }

    // SkewTextExample.Awake
    [HarmonyPatch(typeof(SkewTextExample), nameof(SkewTextExample.Awake))]
    [HarmonyPostfix]
    private static void OnSkewTextExampleAwake(SkewTextExample __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSkewTextExampleAwake failed: {ex.Message}");
        }
    }

    // SkewTextExample.Start
    [HarmonyPatch(typeof(SkewTextExample), nameof(SkewTextExample.Start))]
    [HarmonyPostfix]
    private static void OnSkewTextExampleStart(SkewTextExample __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSkewTextExampleStart failed: {ex.Message}");
        }
    }

    // SkewTextExample.CopyAnimationCurve
    [HarmonyPatch(typeof(SkewTextExample), nameof(SkewTextExample.CopyAnimationCurve))]
    [HarmonyPostfix]
    private static void OnSkewTextExampleCopyAnimationCurve(SkewTextExample __instance, AnimationCurve curve)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CopyAnimationCurve",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSkewTextExampleCopyAnimationCurve failed: {ex.Message}");
        }
    }

    // TextConsoleSimulator.Awake
    [HarmonyPatch(typeof(TextConsoleSimulator), nameof(TextConsoleSimulator.Awake))]
    [HarmonyPostfix]
    private static void OnTextConsoleSimulatorAwake(TextConsoleSimulator __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextConsoleSimulatorAwake failed: {ex.Message}");
        }
    }

    // TextConsoleSimulator.Start
    [HarmonyPatch(typeof(TextConsoleSimulator), nameof(TextConsoleSimulator.Start))]
    [HarmonyPostfix]
    private static void OnTextConsoleSimulatorStart(TextConsoleSimulator __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextConsoleSimulatorStart failed: {ex.Message}");
        }
    }

    // TextConsoleSimulator.OnEnable
    [HarmonyPatch(typeof(TextConsoleSimulator), nameof(TextConsoleSimulator.OnEnable))]
    [HarmonyPostfix]
    private static void OnTextConsoleSimulatorOnEnable(TextConsoleSimulator __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextConsoleSimulatorOnEnable failed: {ex.Message}");
        }
    }

    // TextConsoleSimulator.OnDisable
    [HarmonyPatch(typeof(TextConsoleSimulator), nameof(TextConsoleSimulator.OnDisable))]
    [HarmonyPostfix]
    private static void OnTextConsoleSimulatorOnDisable(TextConsoleSimulator __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextConsoleSimulatorOnDisable failed: {ex.Message}");
        }
    }

    // TextConsoleSimulator.ON_TEXT_CHANGED
    [HarmonyPatch(typeof(TextConsoleSimulator), nameof(TextConsoleSimulator.ON_TEXT_CHANGED))]
    [HarmonyPostfix]
    private static void OnTextConsoleSimulatorON_TEXT_CHANGED(TextConsoleSimulator __instance, UnityEngine.Object obj)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ON_TEXT_CHANGED",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextConsoleSimulatorON_TEXT_CHANGED failed: {ex.Message}");
        }
    }

    // TextMeshProFloatingText.Awake
    [HarmonyPatch(typeof(TextMeshProFloatingText), nameof(TextMeshProFloatingText.Awake))]
    [HarmonyPostfix]
    private static void OnTextMeshProFloatingTextAwake(TextMeshProFloatingText __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextMeshProFloatingTextAwake failed: {ex.Message}");
        }
    }

    // TextMeshProFloatingText.Start
    [HarmonyPatch(typeof(TextMeshProFloatingText), nameof(TextMeshProFloatingText.Start))]
    [HarmonyPostfix]
    private static void OnTextMeshProFloatingTextStart(TextMeshProFloatingText __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextMeshProFloatingTextStart failed: {ex.Message}");
        }
    }

    // TextMeshSpawner.Awake
    [HarmonyPatch(typeof(TextMeshSpawner), nameof(TextMeshSpawner.Awake))]
    [HarmonyPostfix]
    private static void OnTextMeshSpawnerAwake(TextMeshSpawner __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextMeshSpawnerAwake failed: {ex.Message}");
        }
    }

    // TextMeshSpawner.Start
    [HarmonyPatch(typeof(TextMeshSpawner), nameof(TextMeshSpawner.Start))]
    [HarmonyPostfix]
    private static void OnTextMeshSpawnerStart(TextMeshSpawner __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTextMeshSpawnerStart failed: {ex.Message}");
        }
    }

    // VertexJitter.Awake
    [HarmonyPatch(typeof(VertexJitter), nameof(VertexJitter.Awake))]
    [HarmonyPostfix]
    private static void OnVertexJitterAwake(VertexJitter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexJitterAwake failed: {ex.Message}");
        }
    }

    // VertexJitter.OnEnable
    [HarmonyPatch(typeof(VertexJitter), nameof(VertexJitter.OnEnable))]
    [HarmonyPostfix]
    private static void OnVertexJitterOnEnable(VertexJitter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexJitterOnEnable failed: {ex.Message}");
        }
    }

    // VertexJitter.OnDisable
    [HarmonyPatch(typeof(VertexJitter), nameof(VertexJitter.OnDisable))]
    [HarmonyPostfix]
    private static void OnVertexJitterOnDisable(VertexJitter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexJitterOnDisable failed: {ex.Message}");
        }
    }

    // VertexJitter.Start
    [HarmonyPatch(typeof(VertexJitter), nameof(VertexJitter.Start))]
    [HarmonyPostfix]
    private static void OnVertexJitterStart(VertexJitter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexJitterStart failed: {ex.Message}");
        }
    }

    // VertexJitter.ON_TEXT_CHANGED
    [HarmonyPatch(typeof(VertexJitter), nameof(VertexJitter.ON_TEXT_CHANGED))]
    [HarmonyPostfix]
    private static void OnVertexJitterON_TEXT_CHANGED(VertexJitter __instance, UnityEngine.Object obj)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ON_TEXT_CHANGED",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexJitterON_TEXT_CHANGED failed: {ex.Message}");
        }
    }

    // VertexShakeA.Awake
    [HarmonyPatch(typeof(VertexShakeA), nameof(VertexShakeA.Awake))]
    [HarmonyPostfix]
    private static void OnVertexShakeAAwake(VertexShakeA __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeAAwake failed: {ex.Message}");
        }
    }

    // VertexShakeA.OnEnable
    [HarmonyPatch(typeof(VertexShakeA), nameof(VertexShakeA.OnEnable))]
    [HarmonyPostfix]
    private static void OnVertexShakeAOnEnable(VertexShakeA __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeAOnEnable failed: {ex.Message}");
        }
    }

    // VertexShakeA.OnDisable
    [HarmonyPatch(typeof(VertexShakeA), nameof(VertexShakeA.OnDisable))]
    [HarmonyPostfix]
    private static void OnVertexShakeAOnDisable(VertexShakeA __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeAOnDisable failed: {ex.Message}");
        }
    }

    // VertexShakeA.Start
    [HarmonyPatch(typeof(VertexShakeA), nameof(VertexShakeA.Start))]
    [HarmonyPostfix]
    private static void OnVertexShakeAStart(VertexShakeA __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeAStart failed: {ex.Message}");
        }
    }

    // VertexShakeA.ON_TEXT_CHANGED
    [HarmonyPatch(typeof(VertexShakeA), nameof(VertexShakeA.ON_TEXT_CHANGED))]
    [HarmonyPostfix]
    private static void OnVertexShakeAON_TEXT_CHANGED(VertexShakeA __instance, UnityEngine.Object obj)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ON_TEXT_CHANGED",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeAON_TEXT_CHANGED failed: {ex.Message}");
        }
    }

    // VertexShakeB.Awake
    [HarmonyPatch(typeof(VertexShakeB), nameof(VertexShakeB.Awake))]
    [HarmonyPostfix]
    private static void OnVertexShakeBAwake(VertexShakeB __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeBAwake failed: {ex.Message}");
        }
    }

    // VertexShakeB.OnEnable
    [HarmonyPatch(typeof(VertexShakeB), nameof(VertexShakeB.OnEnable))]
    [HarmonyPostfix]
    private static void OnVertexShakeBOnEnable(VertexShakeB __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeBOnEnable failed: {ex.Message}");
        }
    }

    // VertexShakeB.OnDisable
    [HarmonyPatch(typeof(VertexShakeB), nameof(VertexShakeB.OnDisable))]
    [HarmonyPostfix]
    private static void OnVertexShakeBOnDisable(VertexShakeB __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeBOnDisable failed: {ex.Message}");
        }
    }

    // VertexShakeB.Start
    [HarmonyPatch(typeof(VertexShakeB), nameof(VertexShakeB.Start))]
    [HarmonyPostfix]
    private static void OnVertexShakeBStart(VertexShakeB __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeBStart failed: {ex.Message}");
        }
    }

    // VertexShakeB.ON_TEXT_CHANGED
    [HarmonyPatch(typeof(VertexShakeB), nameof(VertexShakeB.ON_TEXT_CHANGED))]
    [HarmonyPostfix]
    private static void OnVertexShakeBON_TEXT_CHANGED(VertexShakeB __instance, UnityEngine.Object obj)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ON_TEXT_CHANGED",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexShakeBON_TEXT_CHANGED failed: {ex.Message}");
        }
    }

    // VertexZoom.Awake
    [HarmonyPatch(typeof(VertexZoom), nameof(VertexZoom.Awake))]
    [HarmonyPostfix]
    private static void OnVertexZoomAwake(VertexZoom __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexZoomAwake failed: {ex.Message}");
        }
    }

    // VertexZoom.OnEnable
    [HarmonyPatch(typeof(VertexZoom), nameof(VertexZoom.OnEnable))]
    [HarmonyPostfix]
    private static void OnVertexZoomOnEnable(VertexZoom __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexZoomOnEnable failed: {ex.Message}");
        }
    }

    // VertexZoom.OnDisable
    [HarmonyPatch(typeof(VertexZoom), nameof(VertexZoom.OnDisable))]
    [HarmonyPostfix]
    private static void OnVertexZoomOnDisable(VertexZoom __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentDisabled",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexZoomOnDisable failed: {ex.Message}");
        }
    }

    // VertexZoom.Start
    [HarmonyPatch(typeof(VertexZoom), nameof(VertexZoom.Start))]
    [HarmonyPostfix]
    private static void OnVertexZoomStart(VertexZoom __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexZoomStart failed: {ex.Message}");
        }
    }

    // VertexZoom.ON_TEXT_CHANGED
    [HarmonyPatch(typeof(VertexZoom), nameof(VertexZoom.ON_TEXT_CHANGED))]
    [HarmonyPostfix]
    private static void OnVertexZoomON_TEXT_CHANGED(VertexZoom __instance, UnityEngine.Object obj)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ON_TEXT_CHANGED",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnVertexZoomON_TEXT_CHANGED failed: {ex.Message}");
        }
    }

    // WarpTextExample.Awake
    [HarmonyPatch(typeof(WarpTextExample), nameof(WarpTextExample.Awake))]
    [HarmonyPostfix]
    private static void OnWarpTextExampleAwake(WarpTextExample __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWarpTextExampleAwake failed: {ex.Message}");
        }
    }

    // WarpTextExample.Start
    [HarmonyPatch(typeof(WarpTextExample), nameof(WarpTextExample.Start))]
    [HarmonyPostfix]
    private static void OnWarpTextExampleStart(WarpTextExample __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWarpTextExampleStart failed: {ex.Message}");
        }
    }

    // WarpTextExample.CopyAnimationCurve
    [HarmonyPatch(typeof(WarpTextExample), nameof(WarpTextExample.CopyAnimationCurve))]
    [HarmonyPostfix]
    private static void OnWarpTextExampleCopyAnimationCurve(WarpTextExample __instance, AnimationCurve curve)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CopyAnimationCurve",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnWarpTextExampleCopyAnimationCurve failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.IsVisible
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.IsVisible))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleIsVisible(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "IsVisible",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleIsVisible failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.Start
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.Start))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleStart(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleStart failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.LoadAccentMap
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.LoadAccentMap))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleLoadAccentMap(OSK_AccentConsole __instance, OSK_AccentAssetObj accents)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AccentMapLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleLoadAccentMap failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.OnDestroy
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.OnDestroy))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleOnDestroy(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleOnDestroy failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.SetConsole
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.SetConsole))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleSetConsole(OSK_AccentConsole __instance, OSK_LongPressPacket packet)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ConsoleSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleSetConsole failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.Set
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.Set))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleSet(OSK_AccentConsole __instance, OSK_LongPressPacket packet)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Set",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleSet failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.Reset
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.Reset))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleReset(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Reset",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleReset failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.ShowBackground
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.ShowBackground))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleShowBackground(OSK_AccentConsole __instance, bool show)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowBackground",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleShowBackground failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.RemoveConsole
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.RemoveConsole))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleRemoveConsole(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ConsoleRemoved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleRemoveConsole failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.AccentCharClick
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.AccentCharClick))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleAccentCharClick(OSK_AccentConsole __instance, string accentedChar, OSK_Receiver receiver)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AccentCharClick",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleAccentCharClick failed: {ex.Message}");
        }
    }

    // OSK_AccentConsole.Generate
    [HarmonyPatch(typeof(OSK_AccentConsole), nameof(OSK_AccentConsole.Generate))]
    [HarmonyPostfix]
    private static void OnOSK_AccentConsoleGenerate(OSK_AccentConsole __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Generate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_AccentConsoleGenerate failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.GamepadPrep
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.GamepadPrep))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperGamepadPrep(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GamepadPrep",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperGamepadPrep failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.GetSelectedKey
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.GetSelectedKey))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperGetSelectedKey(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSelectedKey",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperGetSelectedKey failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.SetSelectedKey
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.SetSelectedKey))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperSetSelectedKey(OSK_GamepadHelper __instance, OSK_Key k)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectedKeySet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperSetSelectedKey failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.Activate
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.Activate))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperActivate(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Activate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperActivate failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.DeActivate
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.DeActivate))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperDeActivate(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "DeActivate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperDeActivate failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.Start
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.Start))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperStart(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperStart failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.JoystickInput
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.JoystickInput))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperJoystickInput(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "JoystickInput",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperJoystickInput failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.JoystickButtonA
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.JoystickButtonA))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperJoystickButtonA(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "JoystickButtonA",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperJoystickButtonA failed: {ex.Message}");
        }
    }

    // OSK_GamepadHelper.JoystickButtonB
    [HarmonyPatch(typeof(OSK_GamepadHelper), nameof(OSK_GamepadHelper.JoystickButtonB))]
    [HarmonyPostfix]
    private static void OnOSK_GamepadHelperJoystickButtonB(OSK_GamepadHelper __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "JoystickButtonB",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_GamepadHelperJoystickButtonB failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.AutoCorrectLayout
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.AutoCorrectLayout))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardAutoCorrectLayout(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AutoCorrectLayout",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardAutoCorrectLayout failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.AcceptPhysicalKeyboard
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.AcceptPhysicalKeyboard))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardAcceptPhysicalKeyboard(OSK_Keyboard __instance, bool accept)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AcceptPhysicalKeyboard",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardAcceptPhysicalKeyboard failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.Prep
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.Prep))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardPrep(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Prep",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardPrep failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.GetOSKKeyCode
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.GetOSKKeyCode))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardGetOSKKeyCode(OSK_Keyboard __instance, string c)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetOSKKeyCode",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardGetOSKKeyCode failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.GetKeyCode
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.GetKeyCode))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardGetKeyCode(OSK_Keyboard __instance, string c)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetKeyCode",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardGetKeyCode failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.ClickSound
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.ClickSound))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardClickSound(OSK_Keyboard __instance, int keytypecode)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClickSound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardClickSound failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.SelectSound
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.SelectSound))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardSelectSound(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectSound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardSelectSound failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.OutputTextUpdate
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.OutputTextUpdate))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardOutputTextUpdate(OSK_Keyboard __instance, string newchar, OSK_Receiver receiver)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OutputTextUpdate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardOutputTextUpdate failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.GetSelectedKey
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.GetSelectedKey))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardGetSelectedKey(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSelectedKey",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardGetSelectedKey failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.GetOSKKey
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.GetOSKKey))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardGetOSKKey(OSK_Keyboard __instance, string k)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GetOSKKey",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardGetOSKKey failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.InputFromPointerDevice
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.InputFromPointerDevice))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardInputFromPointerDevice(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InputFromPointerDevice",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardInputFromPointerDevice failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.Awake
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.Awake))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardAwake(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardAwake failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.Start
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.Start))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardStart(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardStart failed: {ex.Message}");
        }
    }

    // OSK_Keyboard.OnGUI
    [HarmonyPatch(typeof(OSK_Keyboard), nameof(OSK_Keyboard.OnGUI))]
    [HarmonyPostfix]
    private static void OnOSK_KeyboardOnGUI(OSK_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnGUI",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeyboardOnGUI failed: {ex.Message}");
        }
    }

    // OSK_KeySounds.Start
    [HarmonyPatch(typeof(OSK_KeySounds), nameof(OSK_KeySounds.Start))]
    [HarmonyPostfix]
    private static void OnOSK_KeySoundsStart(OSK_KeySounds __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeySoundsStart failed: {ex.Message}");
        }
    }

    // OSK_KeySounds.PlaySound
    [HarmonyPatch(typeof(OSK_KeySounds), nameof(OSK_KeySounds.PlaySound))]
    [HarmonyPostfix]
    private static void OnOSK_KeySoundsPlaySound(OSK_KeySounds __instance, int k)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlaySound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeySoundsPlaySound failed: {ex.Message}");
        }
    }

    // OSK_KeySounds.PlaySelectKeySound
    [HarmonyPatch(typeof(OSK_KeySounds), nameof(OSK_KeySounds.PlaySelectKeySound))]
    [HarmonyPostfix]
    private static void OnOSK_KeySoundsPlaySelectKeySound(OSK_KeySounds __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlaySelectKeySound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_KeySoundsPlaySelectKeySound failed: {ex.Message}");
        }
    }

    // OSK_UI_InputReceiver.TMPInputFieldReActivate
    [HarmonyPatch(typeof(OSK_UI_InputReceiver), nameof(OSK_UI_InputReceiver.TMPInputFieldReActivate))]
    [HarmonyPostfix]
    private static void OnOSK_UI_InputReceiverTMPInputFieldReActivate(OSK_UI_InputReceiver __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "TMPInputFieldReActivate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_InputReceiverTMPInputFieldReActivate failed: {ex.Message}");
        }
    }

    // OSK_UI_InputReceiver.SelectionEnd
    [HarmonyPatch(typeof(OSK_UI_InputReceiver), nameof(OSK_UI_InputReceiver.SelectionEnd))]
    [HarmonyPostfix]
    private static void OnOSK_UI_InputReceiverSelectionEnd(OSK_UI_InputReceiver __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectionEnd",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_InputReceiverSelectionEnd failed: {ex.Message}");
        }
    }

    // OSK_UI_Keyboard.ShowHideKeyboard
    [HarmonyPatch(typeof(OSK_UI_Keyboard), nameof(OSK_UI_Keyboard.ShowHideKeyboard))]
    [HarmonyPostfix]
    private static void OnOSK_UI_KeyboardShowHideKeyboard(OSK_UI_Keyboard __instance, bool show)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ShowHideKeyboard",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_KeyboardShowHideKeyboard failed: {ex.Message}");
        }
    }

    // OSK_UI_Keyboard.GamepadWrapNavigation
    [HarmonyPatch(typeof(OSK_UI_Keyboard), nameof(OSK_UI_Keyboard.GamepadWrapNavigation))]
    [HarmonyPostfix]
    private static void OnOSK_UI_KeyboardGamepadWrapNavigation(OSK_UI_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "GamepadWrapNavigation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_KeyboardGamepadWrapNavigation failed: {ex.Message}");
        }
    }

    // OSK_UI_Keyboard.SelectedKey
    [HarmonyPatch(typeof(OSK_UI_Keyboard), nameof(OSK_UI_Keyboard.SelectedKey))]
    [HarmonyPostfix]
    private static void OnOSK_UI_KeyboardSelectedKey(OSK_UI_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectedKey",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_KeyboardSelectedKey failed: {ex.Message}");
        }
    }

    // OSK_UI_Keyboard.SetSelectedKey
    [HarmonyPatch(typeof(OSK_UI_Keyboard), nameof(OSK_UI_Keyboard.SetSelectedKey))]
    [HarmonyPostfix]
    private static void OnOSK_UI_KeyboardSetSelectedKey(OSK_UI_Keyboard __instance, OSK_UI_Key k)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SelectedKeySet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_KeyboardSetSelectedKey failed: {ex.Message}");
        }
    }

    // OSK_UI_Keyboard.PrepAssetGroup
    [HarmonyPatch(typeof(OSK_UI_Keyboard), nameof(OSK_UI_Keyboard.PrepAssetGroup))]
    [HarmonyPostfix]
    private static void OnOSK_UI_KeyboardPrepAssetGroup(OSK_UI_Keyboard __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PrepAssetGroup",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnOSK_UI_KeyboardPrepAssetGroup failed: {ex.Message}");
        }
    }

    // viperInput.Start
    [HarmonyPatch(typeof(viperInput), nameof(viperInput.Start))]
    [HarmonyPostfix]
    private static void OnviperInputStart(viperInput __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnviperInputStart failed: {ex.Message}");
        }
    }

    // FirstPersonController.Start
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Start))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerStart(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerStart failed: {ex.Message}");
        }
    }

    // FirstPersonController.HandleZoom
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.HandleZoom))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerHandleZoom(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HandleZoom",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerHandleZoom failed: {ex.Message}");
        }
    }

    // FirstPersonController.UpdateCameraPosition
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.UpdateCameraPosition))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerUpdateCameraPosition(FirstPersonController __instance, float speed)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CameraPositionChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerUpdateCameraPosition failed: {ex.Message}");
        }
    }

    // FirstPersonController.PlayLandingSound
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.PlayLandingSound))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerPlayLandingSound(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayLandingSound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerPlayLandingSound failed: {ex.Message}");
        }
    }

    // FirstPersonController.ProgressStepCycle
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.ProgressStepCycle))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerProgressStepCycle(FirstPersonController __instance, float speed)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ProgressStepCycle",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerProgressStepCycle failed: {ex.Message}");
        }
    }

    // FirstPersonController.RotateView
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.RotateView))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerRotateView(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "RotateView",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerRotateView failed: {ex.Message}");
        }
    }

    // FirstPersonController.OnControllerColliderHit
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.OnControllerColliderHit))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerOnControllerColliderHit(FirstPersonController __instance, ControllerColliderHit hit)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnControllerColliderHit",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerOnControllerColliderHit failed: {ex.Message}");
        }
    }

    // FirstPersonController.GetMouseLook - Prefix to bypass null getter (Look input)
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.GetMouseLook))]
    [HarmonyPrefix]
    private static bool PrefixFirstPersonControllerGetMouseLook(FirstPersonController __instance, ref Vector2 __result)
    {
        try
        {
            // Bypass original that returns null - provide default mouse look
            __result = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            SafeEmit(
                "GetMouseLook",
                new
                {
                    instance = __instance,
                    mouseLook = __result
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixFirstPersonControllerGetMouseLook failed: {ex.Message}");
            __result = Vector2.zero;
        }
        return false; // Skip original method
    }

    // FirstPersonController.GetInput - Prefix to bypass null getter (Move input)
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.GetInput))]
    [HarmonyPrefix]
    private static bool PrefixFirstPersonControllerGetInput(FirstPersonController __instance, float speed, ref Vector3 __result)
    {
        try
        {
            // Bypass original that returns null - provide default movement input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            __result = new Vector3(horizontal * speed, 0, vertical * speed);

            SafeEmit(
                "GetInput",
                new
                {
                    instance = __instance,
                    moveInput = __result
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixFirstPersonControllerGetInput failed: {ex.Message}");
            __result = Vector3.zero;
        }
        return false; // Skip original method
    }

    // FirstPersonController.Crouch
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Crouch))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerCrouch(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Crouch",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerCrouch failed: {ex.Message}");
        }
    }

    // FirstPersonController.StopCrouching
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.StopCrouching))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerStopCrouching(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "StopCrouching",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerStopCrouching failed: {ex.Message}");
        }
    }

    // FirstPersonController.ResetCameraPosition
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.ResetCameraPosition))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerResetCameraPosition(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetCameraPosition",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerResetCameraPosition failed: {ex.Message}");
        }
    }

    // FirstPersonController.UpdateNormalFov
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.UpdateNormalFov))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerUpdateNormalFov(FirstPersonController __instance, float fov)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "NormalFovChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerUpdateNormalFov failed: {ex.Message}");
        }
    }

    // FirstPersonController.OnDestroy
    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.OnDestroy))]
    [HarmonyPostfix]
    private static void OnFirstPersonControllerOnDestroy(FirstPersonController __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnFirstPersonControllerOnDestroy failed: {ex.Message}");
        }
    }

    // MouseLook.Init
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.Init))]
    [HarmonyPostfix]
    private static void OnMouseLookInit(MouseLook __instance, Transform character, Transform camera)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Init",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookInit failed: {ex.Message}");
        }
    }

    // MouseLook.ResetRotation
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.ResetRotation))]
    [HarmonyPostfix]
    private static void OnMouseLookResetRotation(MouseLook __instance, Transform character)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetRotation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookResetRotation failed: {ex.Message}");
        }
    }

    // MouseLook.MouseLookOnDisable
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.MouseLookOnDisable))]
    [HarmonyPostfix]
    private static void OnMouseLookMouseLookOnDisable(MouseLook __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "MouseLookOnDisable",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookMouseLookOnDisable failed: {ex.Message}");
        }
    }

    // MouseLook.SetCursorLock
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.SetCursorLock))]
    [HarmonyPostfix]
    private static void OnMouseLookSetCursorLock(MouseLook __instance, bool value)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CursorLockSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookSetCursorLock failed: {ex.Message}");
        }
    }

    // MouseLook.UpdateCursorLock
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.UpdateCursorLock))]
    [HarmonyPostfix]
    private static void OnMouseLookUpdateCursorLock(MouseLook __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CursorLockChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookUpdateCursorLock failed: {ex.Message}");
        }
    }

    // MouseLook.InternalLockUpdate
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.InternalLockUpdate))]
    [HarmonyPostfix]
    private static void OnMouseLookInternalLockUpdate(MouseLook __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "InternalLockUpdate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookInternalLockUpdate failed: {ex.Message}");
        }
    }

    // MouseLook.ClampRotationAroundXAxis
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.ClampRotationAroundXAxis))]
    [HarmonyPostfix]
    private static void OnMouseLookClampRotationAroundXAxis(MouseLook __instance, Quaternion q)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ClampRotationAroundXAxis",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookClampRotationAroundXAxis failed: {ex.Message}");
        }
    }

    // MouseLook.SittingClampRotation
    [HarmonyPatch(typeof(MouseLook), nameof(MouseLook.SittingClampRotation))]
    [HarmonyPostfix]
    private static void OnMouseLookSittingClampRotation(MouseLook __instance, Vector2 q)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "SittingClampRotation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMouseLookSittingClampRotation failed: {ex.Message}");
        }
    }

    // RayLookAt.Init
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.Init))]
    [HarmonyPostfix]
    private static void OnRayLookAtInit(RayLookAt __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Init",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtInit failed: {ex.Message}");
        }
    }

    // RayLookAt.Cleanup
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.Cleanup))]
    [HarmonyPostfix]
    private static void OnRayLookAtCleanup(RayLookAt __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Cleanup",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtCleanup failed: {ex.Message}");
        }
    }

    // RayLookAt.HandleLookAtRay
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.HandleLookAtRay))]
    [HarmonyPostfix]
    private static void OnRayLookAtHandleLookAtRay(RayLookAt __instance, Transform character)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HandleLookAtRay",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtHandleLookAtRay failed: {ex.Message}");
        }
    }

    // RayLookAt.ResetHold
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.ResetHold))]
    [HarmonyPostfix]
    private static void OnRayLookAtResetHold(RayLookAt __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetHold",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtResetHold failed: {ex.Message}");
        }
    }

    // RayLookAt.HideItemNameOrSiluete
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.HideItemNameOrSiluete))]
    [HarmonyPostfix]
    private static void OnRayLookAtHideItemNameOrSiluete(RayLookAt __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HideItemNameOrSiluete",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtHideItemNameOrSiluete failed: {ex.Message}");
        }
    }

    // RayLookAt.CloseInteractionMenu
    [HarmonyPatch(typeof(RayLookAt), nameof(RayLookAt.CloseInteractionMenu))]
    [HarmonyPostfix]
    private static void OnRayLookAtCloseInteractionMenu(RayLookAt __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "CloseInteractionMenu",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnRayLookAtCloseInteractionMenu failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.Awake
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.Awake))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterAwake(ThirdPersonCharacter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterAwake failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.Move
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.Move))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterMove(ThirdPersonCharacter __instance, Vector3 move, bool crouch, bool jump, bool onlyturn, bool backward)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "Move",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterMove failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.UpdateAnimator
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.UpdateAnimator))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterUpdateAnimator(ThirdPersonCharacter __instance, Vector3 move)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "AnimatorChanged",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterUpdateAnimator failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.HandleGroundedMovement
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.HandleGroundedMovement))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterHandleGroundedMovement(ThirdPersonCharacter __instance, bool crouch, bool jump)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "HandleGroundedMovement",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterHandleGroundedMovement failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.ApplyExtraTurnRotation
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.ApplyExtraTurnRotation))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterApplyExtraTurnRotation(ThirdPersonCharacter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "ApplyExtraTurnRotation",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterApplyExtraTurnRotation failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.OnAnimatorMove
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.OnAnimatorMove))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterOnAnimatorMove(ThirdPersonCharacter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnAnimatorMove",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterOnAnimatorMove failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.PlayStepSound
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.PlayStepSound))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterPlayStepSound(ThirdPersonCharacter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "PlayStepSound",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterPlayStepSound failed: {ex.Message}");
        }
    }

    // ThirdPersonCharacter.OnAnimationEventFootStep
    [HarmonyPatch(typeof(ThirdPersonCharacter), nameof(ThirdPersonCharacter.OnAnimationEventFootStep))]
    [HarmonyPostfix]
    private static void OnThirdPersonCharacterOnAnimationEventFootStep(ThirdPersonCharacter __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                "OnAnimationEventFootStep",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnThirdPersonCharacterOnAnimationEventFootStep failed: {ex.Message}");
        }
    }

}
