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
/// Harmony hooks for domain Server (generated from Il2Cpp unpack).
/// </summary>
[HarmonyPatch]
internal static class GregServerHooks
{
    // Server.Start
    [HarmonyPatch(typeof(Server), nameof(Server.Start))]
    [HarmonyPostfix]
    private static void OnServerStart(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerStart failed: {ex.Message}");
        }
    }

    // Server.OnLoadingStarted
    [HarmonyPatch(typeof(Server), nameof(Server.OnLoadingStarted))]
    [HarmonyPostfix]
    private static void OnServerOnLoadingStarted(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "OnLoadingStarted"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerOnLoadingStarted failed: {ex.Message}");
        }
    }

    // Server.OnLoadingComplete
    [HarmonyPatch(typeof(Server), nameof(Server.OnLoadingComplete))]
    [HarmonyPostfix]
    private static void OnServerOnLoadingComplete(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "OnLoadingComplete"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerOnLoadingComplete failed: {ex.Message}");
        }
    }

    // Server.PowerButton
    [HarmonyPatch(typeof(Server), nameof(Server.PowerButton))]
    [HarmonyPostfix]
    private static void OnServerPowerButton(Server __instance, bool forceState)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "PowerButton"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerPowerButton failed: {ex.Message}");
        }
    }

    // Server.TurnOffCommonFunctions
    [HarmonyPatch(typeof(Server), nameof(Server.TurnOffCommonFunctions))]
    [HarmonyPostfix]
    private static void OnServerTurnOffCommonFunctions(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "TurnOffCommonFunctions"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerTurnOffCommonFunctions failed: {ex.Message}");
        }
    }

    // Server.TurnOnCommonFunction
    [HarmonyPatch(typeof(Server), nameof(Server.TurnOnCommonFunction))]
    [HarmonyPostfix]
    private static void OnServerTurnOnCommonFunction(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "TurnOnCommonFunction"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerTurnOnCommonFunction failed: {ex.Message}");
        }
    }

    // Server.IsAnyCableConnected
    [HarmonyPatch(typeof(Server), nameof(Server.IsAnyCableConnected))]
    [HarmonyPostfix]
    private static void OnServerIsAnyCableConnected(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "IsAnyCableConnected"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerIsAnyCableConnected failed: {ex.Message}");
        }
    }

    // Server.GenerateUniqueServerId
    [HarmonyPatch(typeof(Server), nameof(Server.GenerateUniqueServerId))]
    [HarmonyPostfix]
    private static void OnServerGenerateUniqueServerId(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "GenerateUniqueServerId"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerGenerateUniqueServerId failed: {ex.Message}");
        }
    }

    // Server.ServerInsertedInRack
    [HarmonyPatch(typeof(Server), nameof(Server.ServerInsertedInRack))]
    [HarmonyPostfix]
    private static void OnServerServerInsertedInRack(Server __instance, ServerSaveData serverSaveData)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ServerInsertedInRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerServerInsertedInRack failed: {ex.Message}");
        }
    }

    // Server.RegisterLink
    [HarmonyPatch(typeof(Server), nameof(Server.RegisterLink))]
    [HarmonyPostfix]
    private static void OnServerRegisterLink(Server __instance, CableLink link)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "RegisterLink"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerRegisterLink failed: {ex.Message}");
        }
    }

    // Server.UnregisterLink
    [HarmonyPatch(typeof(Server), nameof(Server.UnregisterLink))]
    [HarmonyPostfix]
    private static void OnServerUnregisterLink(Server __instance, CableLink link)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "UnregisterLink"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerUnregisterLink failed: {ex.Message}");
        }
    }

    // Server.UpdateServerScreenUI
    [HarmonyPatch(typeof(Server), nameof(Server.UpdateServerScreenUI))]
    [HarmonyPostfix]
    private static void OnServerUpdateServerScreenUI(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ServerScreenUIChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerUpdateServerScreenUI failed: {ex.Message}");
        }
    }

    // Server.ButtonClickChangeCustomer
    [HarmonyPatch(typeof(Server), nameof(Server.ButtonClickChangeCustomer))]
    [HarmonyPostfix]
    private static void OnServerButtonClickChangeCustomer(Server __instance, bool forward)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ButtonClickChangeCustomer"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerButtonClickChangeCustomer failed: {ex.Message}");
        }
    }

    // Server.GetNextCustomerID
    [HarmonyPatch(typeof(Server), nameof(Server.GetNextCustomerID))]
    [HarmonyPostfix]
    private static void OnServerGetNextCustomerID(Server __instance, int currentCustomerID, bool forward)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "GetNextCustomerID"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerGetNextCustomerID failed: {ex.Message}");
        }
    }

    // Server.ButtonClickChangeIP
    [HarmonyPatch(typeof(Server), nameof(Server.ButtonClickChangeIP))]
    [HarmonyPostfix]
    private static void OnServerButtonClickChangeIP(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ButtonClickChangeIP"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerButtonClickChangeIP failed: {ex.Message}");
        }
    }

    // Server.SetIP
    [HarmonyPatch(typeof(Server), nameof(Server.SetIP))]
    [HarmonyPostfix]
    private static void OnServerSetIP(Server __instance, string _ip)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "IPSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerSetIP failed: {ex.Message}");
        }
    }

    // Server.GetCustomerID
    [HarmonyPatch(typeof(Server), nameof(Server.GetCustomerID))]
    [HarmonyPostfix]
    private static void OnServerGetCustomerID(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "GetCustomerID"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerGetCustomerID failed: {ex.Message}");
        }
    }

    // Server.UpdateCustomer
    [HarmonyPatch(typeof(Server), nameof(Server.UpdateCustomer))]
    [HarmonyPostfix]
    private static void OnServerUpdateCustomer(Server __instance, int newCustomerID)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "CustomerChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerUpdateCustomer failed: {ex.Message}");
        }
    }

    // Server.UpdateAppID
    [HarmonyPatch(typeof(Server), nameof(Server.UpdateAppID))]
    [HarmonyPostfix]
    private static void OnServerUpdateAppID(Server __instance, int _appID)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "AppIDChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerUpdateAppID failed: {ex.Message}");
        }
    }

    // Server.ItIsBroken
    [HarmonyPatch(typeof(Server), nameof(Server.ItIsBroken))]
    [HarmonyPostfix]
    private static void OnServerItIsBroken(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ItIsBroken"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerItIsBroken failed: {ex.Message}");
        }
    }

    // Server.ValidateRackPosition
    [HarmonyPatch(typeof(Server), nameof(Server.ValidateRackPosition))]
    [HarmonyPostfix]
    private static void OnServerValidateRackPosition(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ValidateRackPosition"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerValidateRackPosition failed: {ex.Message}");
        }
    }

    // Server.ClearWarningSign
    [HarmonyPatch(typeof(Server), nameof(Server.ClearWarningSign))]
    [HarmonyPostfix]
    private static void OnServerClearWarningSign(Server __instance, bool isPreserved)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ClearWarningSign"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerClearWarningSign failed: {ex.Message}");
        }
    }

    // Server.ClearErrorSign
    [HarmonyPatch(typeof(Server), nameof(Server.ClearErrorSign))]
    [HarmonyPostfix]
    private static void OnServerClearErrorSign(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "ClearErrorSign"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerClearErrorSign failed: {ex.Message}");
        }
    }

    // Server.SetPowerLightMaterial
    [HarmonyPatch(typeof(Server), nameof(Server.SetPowerLightMaterial))]
    [HarmonyPostfix]
    private static void OnServerSetPowerLightMaterial(Server __instance, Material material)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "PowerLightMaterialSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerSetPowerLightMaterial failed: {ex.Message}");
        }
    }

    // Server.RepairDevice
    [HarmonyPatch(typeof(Server), nameof(Server.RepairDevice))]
    [HarmonyPostfix]
    private static void OnServerRepairDevice(Server __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Server, "DeviceRepaired"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnServerRepairDevice failed: {ex.Message}");
        }
    }

}
