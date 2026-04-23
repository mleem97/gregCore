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
/// Harmony hooks for domain Customer (generated from Il2Cpp unpack).
/// </summary>
[HarmonyPatch]
internal static class GregCustomerHooks
{
    // CustomerBase.Awake
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.Awake))]
    [HarmonyPostfix]
    private static void OnCustomerBaseAwake(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseAwake failed: {ex.Message}");
        }
    }

    // CustomerBase.Start
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.Start))]
    [HarmonyPostfix]
    private static void OnCustomerBaseStart(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseStart failed: {ex.Message}");
        }
    }

    // CustomerBase.GetEffectiveMoneySpeed
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetEffectiveMoneySpeed))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetEffectiveMoneySpeed(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetEffectiveMoneySpeed"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetEffectiveMoneySpeed failed: {ex.Message}");
        }
    }

    // CustomerBase.AreAllAppRequirementsMet
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AreAllAppRequirementsMet))]
    [HarmonyPostfix]
    private static void OnCustomerBaseAreAllAppRequirementsMet(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "AreAllAppRequirementsMet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseAreAllAppRequirementsMet failed: {ex.Message}");
        }
    }

    // CustomerBase.UpdateCustomerServerCountAndSpeed
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.UpdateCustomerServerCountAndSpeed))]
    [HarmonyPostfix]
    private static void OnCustomerBaseUpdateCustomerServerCountAndSpeed(CustomerBase __instance, int count, float speed)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "CustomerServerCountAndSpeedChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseUpdateCustomerServerCountAndSpeed failed: {ex.Message}");
        }
    }

    // CustomerBase.AddAppPerformance
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AddAppPerformance))]
    [HarmonyPostfix]
    private static void OnCustomerBaseAddAppPerformance(CustomerBase __instance, int appID, float speed)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "AppPerformanceAdded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseAddAppPerformance failed: {ex.Message}");
        }
    }

    // CustomerBase.ResetAllAppSpeeds
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.ResetAllAppSpeeds))]
    [HarmonyPostfix]
    private static void OnCustomerBaseResetAllAppSpeeds(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "ResetAllAppSpeeds"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseResetAllAppSpeeds failed: {ex.Message}");
        }
    }

    // CustomerBase.IsIPPresent
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.IsIPPresent))]
    [HarmonyPostfix]
    private static void OnCustomerBaseIsIPPresent(CustomerBase __instance, string ip)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "IsIPPresent"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseIsIPPresent failed: {ex.Message}");
        }
    }

    // CustomerBase.GetAppIDForIP
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetAppIDForIP))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetAppIDForIP(CustomerBase __instance, string ip)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetAppIDForIP"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetAppIDForIP failed: {ex.Message}");
        }
    }

    // CustomerBase.SetUpBase
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.SetUpBase))]
    [HarmonyPostfix]
    private static void OnCustomerBaseSetUpBase(CustomerBase __instance, CustomerItem customerItem, CustomerBaseSaveData saveData)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "UpBaseSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseSetUpBase failed: {ex.Message}");
        }
    }

    // CustomerBase.SetUpApp
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.SetUpApp))]
    [HarmonyPostfix]
    private static void OnCustomerBaseSetUpApp(CustomerBase __instance, int appID, int difficulty, CustomerBaseSaveData saveData)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "UpAppSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseSetUpApp failed: {ex.Message}");
        }
    }

    // CustomerBase.AppText
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AppText))]
    [HarmonyPostfix]
    private static void OnCustomerBaseAppText(CustomerBase __instance, int lastUsedApp)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "AppText"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseAppText failed: {ex.Message}");
        }
    }

    // CustomerBase.UpdateSpeedOnCustomerBaseApp
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.UpdateSpeedOnCustomerBaseApp))]
    [HarmonyPostfix]
    private static void OnCustomerBaseUpdateSpeedOnCustomerBaseApp(CustomerBase __instance, int appID, float speed)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "SpeedOnCustomerBaseAppChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseUpdateSpeedOnCustomerBaseApp failed: {ex.Message}");
        }
    }

    // CustomerBase.GetSubnetsPerApp
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetSubnetsPerApp))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetSubnetsPerApp(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetSubnetsPerApp"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetSubnetsPerApp failed: {ex.Message}");
        }
    }

    // CustomerBase.GetVlanIdsPerApp
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetVlanIdsPerApp))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetVlanIdsPerApp(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetVlanIdsPerApp"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetVlanIdsPerApp failed: {ex.Message}");
        }
    }

    // CustomerBase.GetServerTypeForIP
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetServerTypeForIP))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetServerTypeForIP(CustomerBase __instance, string ip)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetServerTypeForIP"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetServerTypeForIP failed: {ex.Message}");
        }
    }

    // CustomerBase.GetTotalAppSpeed
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.GetTotalAppSpeed))]
    [HarmonyPostfix]
    private static void OnCustomerBaseGetTotalAppSpeed(CustomerBase __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "GetTotalAppSpeed"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseGetTotalAppSpeed failed: {ex.Message}");
        }
    }

    // CustomerBase.LoadData
    [HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.LoadData))]
    [HarmonyPostfix]
    private static void OnCustomerBaseLoadData(CustomerBase __instance, CustomerBaseSaveData data)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Customer, "DataLoaded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCustomerBaseLoadData failed: {ex.Message}");
        }
    }

}
