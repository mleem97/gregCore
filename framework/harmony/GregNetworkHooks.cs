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
/// Harmony hooks for domain Network (generated from Il2Cpp unpack).
/// </summary>
internal static class GregNetworkHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try
        {
            gregEventDispatcher.Emit(hookName, payload);
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] SafeEmit failed for '{hookName}': {ex.Message}");
        }
    }


    // CableLink.Start
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.Start))]
    [HarmonyPostfix]
    private static void OnCableLinkStart(CableLink __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkStart failed: {ex.Message}");
        }
    }

    // CableLink.SetConnectionSpeed
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.SetConnectionSpeed))]
    [HarmonyPostfix]
    private static void OnCableLinkSetConnectionSpeed(CableLink __instance, float speed)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ConnectionSpeedSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkSetConnectionSpeed failed: {ex.Message}");
        }
    }

    // CableLink.InsertSFP
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.InsertSFP))]
    [HarmonyPostfix]
    private static void OnCableLinkInsertSFP(CableLink __instance, float speed, int type, SFPModule module)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "InsertSFP"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkInsertSFP failed: {ex.Message}");
        }
    }

    // CableLink.RemoveSFP
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.RemoveSFP))]
    [HarmonyPostfix]
    private static void OnCableLinkRemoveSFP(CableLink __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "SFPRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkRemoveSFP failed: {ex.Message}");
        }
    }

    // CableLink.CollectPatchPanelChainCables
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.CollectPatchPanelChainCables))]
    [HarmonyPostfix]
    private static void OnCableLinkCollectPatchPanelChainCables(CableLink __instance, int startCableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CollectPatchPanelChainCables"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkCollectPatchPanelChainCables failed: {ex.Message}");
        }
    }

    // CableLink.CreateRopeAttachPoint
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.CreateRopeAttachPoint))]
    [HarmonyPostfix]
    private static void OnCableLinkCreateRopeAttachPoint(CableLink __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CreateRopeAttachPoint"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkCreateRopeAttachPoint failed: {ex.Message}");
        }
    }

    // CableLink.GetRopeAttachPoint
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.GetRopeAttachPoint))]
    [HarmonyPostfix]
    private static void OnCableLinkGetRopeAttachPoint(CableLink __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetRopeAttachPoint"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCableLinkGetRopeAttachPoint failed: {ex.Message}");
        }
    }

    // CablePositions.Awake
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.Awake))]
    [HarmonyPostfix]
    private static void OnCablePositionsAwake(CablePositions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsAwake failed: {ex.Message}");
        }
    }

    // CablePositions.Start
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.Start))]
    [HarmonyPostfix]
    private static void OnCablePositionsStart(CablePositions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsStart failed: {ex.Message}");
        }
    }

    // CablePositions.ClearAllCables
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.ClearAllCables))]
    [HarmonyPostfix]
    private static void OnCablePositionsClearAllCables(CablePositions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ClearAllCables"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsClearAllCables failed: {ex.Message}");
        }
    }

    // CablePositions.LoadCable
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.LoadCable))]
    [HarmonyPostfix]
    private static void OnCablePositionsLoadCable(CablePositions __instance, CableSaveData cableData)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CableLoaded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsLoadCable failed: {ex.Message}");
        }
    }

    // CablePositions.CreateNewCable - Prefix to override original that returns 0 (causes ID collision)
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.CreateNewCable))]
    [HarmonyPrefix]
    private static bool PrefixCablePositionsCreateNewCable(CablePositions __instance, ref int __result)
    {
        try
        {
            // Generate unique cable ID to prevent collisions
            int newCableId = (__instance.cablePoints != null) ? __instance.cablePoints.Count : 0;
            __result = newCableId;

            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CreateNewCable"),
                new
                {
                    instance = __instance,
                    cableId = newCableId
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixCablePositionsCreateNewCable failed: {ex.Message}");
            __result = -1;
        }
        return false; // Skip original method
    }

    // CablePositions.CreateNewReverseCable
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.CreateNewReverseCable))]
    [HarmonyPostfix]
    private static void OnCablePositionsCreateNewReverseCable(CablePositions __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CreateNewReverseCable"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsCreateNewReverseCable failed: {ex.Message}");
        }
    }

    // CablePositions.GenerateFinalPath
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.GenerateFinalPath))]
    [HarmonyPostfix]
    private static void OnCablePositionsGenerateFinalPath(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GenerateFinalPath"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsGenerateFinalPath failed: {ex.Message}");
        }
    }

    // CablePositions.RedrawCable
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.RedrawCable))]
    [HarmonyPostfix]
    private static void OnCablePositionsRedrawCable(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "RedrawCable"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsRedrawCable failed: {ex.Message}");
        }
    }

    // CablePositions.CreateTubeMesh
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.CreateTubeMesh))]
    [HarmonyPostfix]
    private static void OnCablePositionsCreateTubeMesh(CablePositions __instance, Il2CppSystem.Collections.Generic.List<Vector3> path)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CreateTubeMesh"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsCreateTubeMesh failed: {ex.Message}");
        }
    }

    // CablePositions.RemovePosition
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.RemovePosition))]
    [HarmonyPostfix]
    private static void OnCablePositionsRemovePosition(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "PositionRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsRemovePosition failed: {ex.Message}");
        }
    }

    // CablePositions.RemoveLastPosition
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.RemoveLastPosition))]
    [HarmonyPostfix]
    private static void OnCablePositionsRemoveLastPosition(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "LastPositionRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsRemoveLastPosition failed: {ex.Message}");
        }
    }

    // CablePositions.GetCablePositions
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.GetCablePositions))]
    [HarmonyPostfix]
    private static void OnCablePositionsGetCablePositions(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetCablePositions"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsGetCablePositions failed: {ex.Message}");
        }
    }

    // CablePositions.GetRawCablePositions
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.GetRawCablePositions))]
    [HarmonyPostfix]
    private static void OnCablePositionsGetRawCablePositions(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetRawCablePositions"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsGetRawCablePositions failed: {ex.Message}");
        }
    }

    // CablePositions.GetRawLinkTransforms
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.GetRawLinkTransforms))]
    [HarmonyPostfix]
    private static void OnCablePositionsGetRawLinkTransforms(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetRawLinkTransforms"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsGetRawLinkTransforms failed: {ex.Message}");
        }
    }

    // CablePositions.IsCableComplete
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.IsCableComplete))]
    [HarmonyPostfix]
    private static void OnCablePositionsIsCableComplete(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsCableComplete"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsIsCableComplete failed: {ex.Message}");
        }
    }

    // CablePositions.GetCableMaterial
    [HarmonyPatch(typeof(CablePositions), nameof(CablePositions.GetCableMaterial))]
    [HarmonyPostfix]
    private static void OnCablePositionsGetCableMaterial(CablePositions __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetCableMaterial"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnCablePositionsGetCableMaterial failed: {ex.Message}");
        }
    }

    // NetworkMap.Awake
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.Awake))]
    [HarmonyPostfix]
    private static void OnNetworkMapAwake(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapAwake failed: {ex.Message}");
        }
    }

    // NetworkMap.ClearMap
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.ClearMap))]
    [HarmonyPostfix]
    private static void OnNetworkMapClearMap(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ClearMap"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapClearMap failed: {ex.Message}");
        }
    }

    // NetworkMap.RegisterCustomerBase
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RegisterCustomerBase))]
    [HarmonyPostfix]
    private static void OnNetworkMapRegisterCustomerBase(NetworkMap __instance, CustomerBase customerBase)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "RegisterCustomerBase"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRegisterCustomerBase failed: {ex.Message}");
        }
    }

    // NetworkMap.GetCustomerBase
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetCustomerBase))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetCustomerBase(NetworkMap __instance, int customerId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetCustomerBase"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetCustomerBase failed: {ex.Message}");
        }
    }

    // NetworkMap.RegisterServer
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RegisterServer))]
    [HarmonyPostfix]
    private static void OnNetworkMapRegisterServer(NetworkMap __instance, Server server)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "RegisterServer"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRegisterServer failed: {ex.Message}");
        }
    }

    // NetworkMap.RegisterSwitch
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RegisterSwitch))]
    [HarmonyPostfix]
    private static void OnNetworkMapRegisterSwitch(NetworkMap __instance, NetworkSwitch networkSwitch)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "RegisterSwitch"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRegisterSwitch failed: {ex.Message}");
        }
    }

    // NetworkMap.GetServer
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetServer))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetServer(NetworkMap __instance, string serverId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetServer"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetServer failed: {ex.Message}");
        }
    }

    // NetworkMap.GetSwitchById
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetSwitchById))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetSwitchById(NetworkMap __instance, string switchId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetSwitchById"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetSwitchById failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllServers
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllServers))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllServers(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllServers"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllServers failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllNetworkSwitches
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllNetworkSwitches))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllNetworkSwitches(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllNetworkSwitches"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllNetworkSwitches failed: {ex.Message}");
        }
    }

    // NetworkMap.UpdateDeviceCustomerID
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.UpdateDeviceCustomerID))]
    [HarmonyPostfix]
    private static void OnNetworkMapUpdateDeviceCustomerID(NetworkMap __instance, string deviceName, int customerID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DeviceCustomerIDChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapUpdateDeviceCustomerID failed: {ex.Message}");
        }
    }

    // NetworkMap.AddDevice
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddDevice))]
    [HarmonyPostfix]
    private static void OnNetworkMapAddDevice(NetworkMap __instance, string name, CableLink.TypeOfLink type, int customerID)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DeviceAdded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapAddDevice failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveDevice
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveDevice))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveDevice(NetworkMap __instance, string name)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DeviceRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveDevice failed: {ex.Message}");
        }
    }

    // NetworkMap.Connect
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.Connect))]
    [HarmonyPostfix]
    private static void OnNetworkMapConnect(NetworkMap __instance, string from, string to)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "Connect"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapConnect failed: {ex.Message}");
        }
    }

    // NetworkMap.Disconnect
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.Disconnect))]
    [HarmonyPostfix]
    private static void OnNetworkMapDisconnect(NetworkMap __instance, string from, string to)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "Disconnect"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapDisconnect failed: {ex.Message}");
        }
    }

    // NetworkMap.FindAllRoutes
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.FindAllRoutes))]
    [HarmonyPostfix]
    private static void OnNetworkMapFindAllRoutes(NetworkMap __instance, string baseName, string serverName)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "FindAllRoutes"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapFindAllRoutes failed: {ex.Message}");
        }
    }

    // NetworkMap.FindAllReachablePathsFrom
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.FindAllReachablePathsFrom))]
    [HarmonyPostfix]
    private static void OnNetworkMapFindAllReachablePathsFrom(NetworkMap __instance, string startDevice)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "FindAllReachablePathsFrom"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapFindAllReachablePathsFrom failed: {ex.Message}");
        }
    }

    // NetworkMap.FindPhysicalPath
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.FindPhysicalPath))]
    [HarmonyPostfix]
    private static void OnNetworkMapFindPhysicalPath(NetworkMap __instance, string start, string target)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "FindPhysicalPath"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapFindPhysicalPath failed: {ex.Message}");
        }
    }

    // NetworkMap.GetDevice
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetDevice))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetDevice(NetworkMap __instance, string name)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetDevice"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetDevice failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllDevices
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllDevices))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllDevices(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllDevices"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllDevices failed: {ex.Message}");
        }
    }

    // NetworkMap.GenerateDeviceName
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GenerateDeviceName))]
    [HarmonyPostfix]
    private static void OnNetworkMapGenerateDeviceName(NetworkMap __instance, CableLink.TypeOfLink type, Vector3 position)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GenerateDeviceName"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGenerateDeviceName failed: {ex.Message}");
        }
    }

    // NetworkMap.AddSwitchConnection
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddSwitchConnection))]
    [HarmonyPostfix]
    private static void OnNetworkMapAddSwitchConnection(NetworkMap __instance, string switchName, string deviceName)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "SwitchConnectionAdded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapAddSwitchConnection failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveCableConnection
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveCableConnection))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveCableConnection(NetworkMap __instance, int cableId, bool preserveLACP)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CableConnectionRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveCableConnection failed: {ex.Message}");
        }
    }

    // NetworkMap.PrintNetworkMap
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.PrintNetworkMap))]
    [HarmonyPostfix]
    private static void OnNetworkMapPrintNetworkMap(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "PrintNetworkMap"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapPrintNetworkMap failed: {ex.Message}");
        }
    }

    // NetworkMap.IsIpAddressDuplicate
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.IsIpAddressDuplicate))]
    [HarmonyPostfix]
    private static void OnNetworkMapIsIpAddressDuplicate(NetworkMap __instance, string ip, Server serverToExclude)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsIpAddressDuplicate"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapIsIpAddressDuplicate failed: {ex.Message}");
        }
    }

    // NetworkMap.AddBrokenServer
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddBrokenServer))]
    [HarmonyPostfix]
    private static void OnNetworkMapAddBrokenServer(NetworkMap __instance, Server server)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "BrokenServerAdded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapAddBrokenServer failed: {ex.Message}");
        }
    }

    // NetworkMap.AddBrokenSwitch
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddBrokenSwitch))]
    [HarmonyPostfix]
    private static void OnNetworkMapAddBrokenSwitch(NetworkMap __instance, NetworkSwitch networkSwitch)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "BrokenSwitchAdded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapAddBrokenSwitch failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveBrokenServer
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveBrokenServer))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveBrokenServer(NetworkMap __instance, string serverId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "BrokenServerRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveBrokenServer failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveBrokenSwitch
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveBrokenSwitch))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveBrokenSwitch(NetworkMap __instance, string switchId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "BrokenSwitchRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveBrokenSwitch failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllBrokenServers
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllBrokenServers))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllBrokenServers(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllBrokenServers"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllBrokenServers failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllBrokenSwitches
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllBrokenSwitches))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllBrokenSwitches(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllBrokenSwitches"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllBrokenSwitches failed: {ex.Message}");
        }
    }

    // NetworkMap.IsPatchPanelPort
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.IsPatchPanelPort))]
    [HarmonyPostfix]
    private static void OnNetworkMapIsPatchPanelPort(NetworkMap __instance, string deviceName)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsPatchPanelPort"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapIsPatchPanelPort failed: {ex.Message}");
        }
    }

    // NetworkMap.ResolveThroughPatchPanel
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.ResolveThroughPatchPanel))]
    [HarmonyPostfix]
    private static void OnNetworkMapResolveThroughPatchPanel(NetworkMap __instance, string patchPanelPort, string fromDevice)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ResolveThroughPatchPanel"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapResolveThroughPatchPanel failed: {ex.Message}");
        }
    }

    // NetworkMap.CreateLACPGroup
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.CreateLACPGroup))]
    [HarmonyPostfix]
    private static void OnNetworkMapCreateLACPGroup(NetworkMap __instance, string deviceA, string deviceB, Il2CppSystem.Collections.Generic.List<int> cableIds)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CreateLACPGroup"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapCreateLACPGroup failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveLACPGroup
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveLACPGroup))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveLACPGroup(NetworkMap __instance, int groupId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "LACPGroupRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveLACPGroup failed: {ex.Message}");
        }
    }

    // NetworkMap.RemoveCableFromLACPGroups
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveCableFromLACPGroups))]
    [HarmonyPostfix]
    private static void OnNetworkMapRemoveCableFromLACPGroups(NetworkMap __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CableFromLACPGroupsRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapRemoveCableFromLACPGroups failed: {ex.Message}");
        }
    }

    // NetworkMap.GetLACPGroupForCable
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetLACPGroupForCable))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetLACPGroupForCable(NetworkMap __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetLACPGroupForCable"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetLACPGroupForCable failed: {ex.Message}");
        }
    }

    // NetworkMap.GetLACPGroupBetween
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetLACPGroupBetween))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetLACPGroupBetween(NetworkMap __instance, string deviceA, string deviceB)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetLACPGroupBetween"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetLACPGroupBetween failed: {ex.Message}");
        }
    }

    // NetworkMap.GetAllLACPGroups
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetAllLACPGroups))]
    [HarmonyPostfix]
    private static void OnNetworkMapGetAllLACPGroups(NetworkMap __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetAllLACPGroups"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapGetAllLACPGroups failed: {ex.Message}");
        }
    }

    // NetworkMap.SetLACPGroups
    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.SetLACPGroups))]
    [HarmonyPostfix]
    private static void OnNetworkMapSetLACPGroups(NetworkMap __instance, Il2CppSystem.Collections.Generic.Dictionary<int, NetworkMap.LACPGroup> groups)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "LACPGroupsSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkMapSetLACPGroups failed: {ex.Message}");
        }
    }

    // NetworkSwitch.Start
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.Start))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchStart(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchStart failed: {ex.Message}");
        }
    }

    // NetworkSwitch.PowerButton
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.PowerButton))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchPowerButton(NetworkSwitch __instance, bool forceState)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "PowerButton"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchPowerButton failed: {ex.Message}");
        }
    }

    // NetworkSwitch.TurnOffCommonFunctions
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.TurnOffCommonFunctions))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchTurnOffCommonFunctions(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "TurnOffCommonFunctions"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchTurnOffCommonFunctions failed: {ex.Message}");
        }
    }

    // NetworkSwitch.TurnOnCommonFunction
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.TurnOnCommonFunction))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchTurnOnCommonFunction(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "TurnOnCommonFunction"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchTurnOnCommonFunction failed: {ex.Message}");
        }
    }

    // NetworkSwitch.IsAnyCableConnected
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.IsAnyCableConnected))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchIsAnyCableConnected(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsAnyCableConnected"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchIsAnyCableConnected failed: {ex.Message}");
        }
    }

    // NetworkSwitch.SwitchInsertedInRack
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SwitchInsertedInRack))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchSwitchInsertedInRack(NetworkSwitch __instance, SwitchSaveData switchSaveData)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "SwitchInsertedInRack"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchSwitchInsertedInRack failed: {ex.Message}");
        }
    }

    // NetworkSwitch.GenerateUniqueSwitchId
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.GenerateUniqueSwitchId))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchGenerateUniqueSwitchId(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GenerateUniqueSwitchId"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchGenerateUniqueSwitchId failed: {ex.Message}");
        }
    }

    // NetworkSwitch.DisconnectCablesWhenSwitchIsOff
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.DisconnectCablesWhenSwitchIsOff))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchDisconnectCablesWhenSwitchIsOff(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DisconnectCablesWhenSwitchIsOff"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchDisconnectCablesWhenSwitchIsOff failed: {ex.Message}");
        }
    }

    // NetworkSwitch.HandleNewCableWhileOff
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.HandleNewCableWhileOff))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchHandleNewCableWhileOff(NetworkSwitch __instance, int cableId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "HandleNewCableWhileOff"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchHandleNewCableWhileOff failed: {ex.Message}");
        }
    }

    // NetworkSwitch.GetConnectedDevices
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.GetConnectedDevices))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchGetConnectedDevices(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetConnectedDevices"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchGetConnectedDevices failed: {ex.Message}");
        }
    }

    // NetworkSwitch.GetSwitchId
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.GetSwitchId))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchGetSwitchId(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetSwitchId"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchGetSwitchId failed: {ex.Message}");
        }
    }

    // NetworkSwitch.UpdateScreenUI
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.UpdateScreenUI))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchUpdateScreenUI(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ScreenUIChanged"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchUpdateScreenUI failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ItIsBroken
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ItIsBroken))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchItIsBroken(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ItIsBroken"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchItIsBroken failed: {ex.Message}");
        }
    }

    // NetworkSwitch.DisconnectCables
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.DisconnectCables))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchDisconnectCables(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DisconnectCables"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchDisconnectCables failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ReconnectCables
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ReconnectCables))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchReconnectCables(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ReconnectCables"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchReconnectCables failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ValidateRackPosition
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ValidateRackPosition))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchValidateRackPosition(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ValidateRackPosition"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchValidateRackPosition failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ButtonShowNetworkSwitchConfig
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ButtonShowNetworkSwitchConfig))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchButtonShowNetworkSwitchConfig(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ButtonShowNetworkSwitchConfig"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchButtonShowNetworkSwitchConfig failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ClearWarningSign
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ClearWarningSign))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchClearWarningSign(NetworkSwitch __instance, bool isPreserved)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ClearWarningSign"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchClearWarningSign failed: {ex.Message}");
        }
    }

    // NetworkSwitch.ClearErrorSign
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ClearErrorSign))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchClearErrorSign(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ClearErrorSign"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchClearErrorSign failed: {ex.Message}");
        }
    }

    // NetworkSwitch.SetPowerLightMaterial
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SetPowerLightMaterial))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchSetPowerLightMaterial(NetworkSwitch __instance, Material material)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "PowerLightMaterialSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchSetPowerLightMaterial failed: {ex.Message}");
        }
    }

    // NetworkSwitch.RepairDevice
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.RepairDevice))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchRepairDevice(NetworkSwitch __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "DeviceRepaired"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchRepairDevice failed: {ex.Message}");
        }
    }

    // NetworkSwitch.IsVlanAllowedOnPort
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.IsVlanAllowedOnPort))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchIsVlanAllowedOnPort(NetworkSwitch __instance, int portIndex, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsVlanAllowedOnPort"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchIsVlanAllowedOnPort failed: {ex.Message}");
        }
    }

    // NetworkSwitch.IsVlanAllowedOnCable
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.IsVlanAllowedOnCable))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchIsVlanAllowedOnCable(NetworkSwitch __instance, int cableId, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsVlanAllowedOnCable"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchIsVlanAllowedOnCable failed: {ex.Message}");
        }
    }

    // NetworkSwitch.SetVlanDisallowed
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SetVlanDisallowed))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchSetVlanDisallowed(NetworkSwitch __instance, int portIndex, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "VlanDisallowedSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchSetVlanDisallowed failed: {ex.Message}");
        }
    }

    // NetworkSwitch.SetVlanAllowed
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SetVlanAllowed))]
    [HarmonyPostfix]
    private static void OnNetworkSwitchSetVlanAllowed(NetworkSwitch __instance, int portIndex, int vlanId)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "VlanAllowedSet"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnNetworkSwitchSetVlanAllowed failed: {ex.Message}");
        }
    }

    // SFPBox.InsertSFPBackIntoBox
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.InsertSFPBackIntoBox))]
    [HarmonyPostfix]
    private static void OnSFPBoxInsertSFPBackIntoBox(SFPBox __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "InsertSFPBackIntoBox"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxInsertSFPBackIntoBox failed: {ex.Message}");
        }
    }

    // SFPBox.GetFreeSpaceInTheBox
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.GetFreeSpaceInTheBox))]
    [HarmonyPostfix]
    private static void OnSFPBoxGetFreeSpaceInTheBox(SFPBox __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "GetFreeSpaceInTheBox"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxGetFreeSpaceInTheBox failed: {ex.Message}");
        }
    }

    // SFPBox.RemoveSFPFromBox
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.RemoveSFPFromBox))]
    [HarmonyPostfix]
    private static void OnSFPBoxRemoveSFPFromBox(SFPBox __instance, int position)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "SFPFromBoxRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxRemoveSFPFromBox failed: {ex.Message}");
        }
    }

    // SFPBox.LoadSFPsFromSave
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.LoadSFPsFromSave))]
    [HarmonyPostfix]
    private static void OnSFPBoxLoadSFPsFromSave(SFPBox __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "SFPsFromSaveLoaded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxLoadSFPsFromSave failed: {ex.Message}");
        }
    }

    // SFPBox.TakeSFPFromBox
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.TakeSFPFromBox))]
    [HarmonyPostfix]
    private static void OnSFPBoxTakeSFPFromBox(SFPBox __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "TakeSFPFromBox"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxTakeSFPFromBox failed: {ex.Message}");
        }
    }

    // SFPBox.CanAcceptSFP
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.CanAcceptSFP))]
    [HarmonyPostfix]
    private static void OnSFPBoxCanAcceptSFP(SFPBox __instance, int sfpType)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "CanAcceptSFP"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxCanAcceptSFP failed: {ex.Message}");
        }
    }

    // SFPBox.ReturnSFPDirectly
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.ReturnSFPDirectly))]
    [HarmonyPostfix]
    private static void OnSFPBoxReturnSFPDirectly(SFPBox __instance, SFPModule sfpmodule)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "ReturnSFPDirectly"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPBoxReturnSFPDirectly failed: {ex.Message}");
        }
    }

    // SFPModule.IsAnyCableConnected
    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.IsAnyCableConnected))]
    [HarmonyPostfix]
    private static void OnSFPModuleIsAnyCableConnected(SFPModule __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "IsAnyCableConnected"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPModuleIsAnyCableConnected failed: {ex.Message}");
        }
    }

    // SFPModule.InsertedInSFPPort
    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.InsertedInSFPPort))]
    [HarmonyPostfix]
    private static void OnSFPModuleInsertedInSFPPort(SFPModule __instance, CableLink _link, bool immediate)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "InsertedInSFPPort"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPModuleInsertedInSFPPort failed: {ex.Message}");
        }
    }

    // SFPModule.InsertDirectlyIntoPort
    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.InsertDirectlyIntoPort))]
    [HarmonyPostfix]
    private static void OnSFPModuleInsertDirectlyIntoPort(SFPModule __instance, CableLink _link)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "InsertDirectlyIntoPort"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPModuleInsertDirectlyIntoPort failed: {ex.Message}");
        }
    }

    // SFPModule.RemoveFromPort
    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.RemoveFromPort))]
    [HarmonyPostfix]
    private static void OnSFPModuleRemoveFromPort(SFPModule __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            SafeEmit(
                gregHookName.Create(GregDomain.Network, "FromPortRemoved"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSFPModuleRemoveFromPort failed: {ex.Message}");
        }
    }

}
