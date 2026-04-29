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
/// Harmony hooks for domain Employee (generated from Il2Cpp unpack).
/// </summary>
internal static class GregEmployeeHooks
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

    // HRSystem.OnEnable
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.OnEnable))]
    [HarmonyPostfix]
    private static void OnHRSystemOnEnable(HRSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemOnEnable failed: {ex.Message}");
        }
    }

    // HRSystem.ButtonHireEmployee
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonHireEmployee))]
    [HarmonyPostfix]
    private static void OnHRSystemButtonHireEmployee(HRSystem __instance, int i)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ButtonHireEmployee"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemButtonHireEmployee failed: {ex.Message}");
        }
    }

    // HRSystem.ButtonCancelBuying
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonCancelBuying))]
    [HarmonyPostfix]
    private static void OnHRSystemButtonCancelBuying(HRSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ButtonCancelBuying"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemButtonCancelBuying failed: {ex.Message}");
        }
    }

    // HRSystem.ButtonConfirmHire
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmHire))]
    [HarmonyPostfix]
    private static void OnHRSystemButtonConfirmHire(HRSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ButtonConfirmHire"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemButtonConfirmHire failed: {ex.Message}");
        }
    }

    // HRSystem.ButtonFireEmployee
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonFireEmployee))]
    [HarmonyPostfix]
    private static void OnHRSystemButtonFireEmployee(HRSystem __instance, int i)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ButtonFireEmployee"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemButtonFireEmployee failed: {ex.Message}");
        }
    }

    // HRSystem.ButtonConfirmFireEmployee
    [HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmFireEmployee))]
    [HarmonyPostfix]
    private static void OnHRSystemButtonConfirmFireEmployee(HRSystem __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ButtonConfirmFireEmployee"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnHRSystemButtonConfirmFireEmployee failed: {ex.Message}");
        }
    }

    // Technician.Awake
    [HarmonyPatch(typeof(Technician), nameof(Technician.Awake))]
    [HarmonyPostfix]
    private static void OnTechnicianAwake(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianAwake failed: {ex.Message}");
        }
    }

    // Technician.Start
    [HarmonyPatch(typeof(Technician), nameof(Technician.Start))]
    [HarmonyPostfix]
    private static void OnTechnicianStart(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianStart failed: {ex.Message}");
        }
    }

    // Technician.AssignJob
    [HarmonyPatch(typeof(Technician), nameof(Technician.AssignJob))]
    [HarmonyPostfix]
    private static void OnTechnicianAssignJob(Technician __instance, TechnicianManager.RepairJob job)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "AssignJob"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianAssignJob failed: {ex.Message}");
        }
    }

    // Technician.GetCurrentDevicePrefabID
    [HarmonyPatch(typeof(Technician), nameof(Technician.GetCurrentDevicePrefabID))]
    [HarmonyPostfix]
    private static void OnTechnicianGetCurrentDevicePrefabID(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "GetCurrentDevicePrefabID"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianGetCurrentDevicePrefabID failed: {ex.Message}");
        }
    }

    // Technician.RepairDevice
    [HarmonyPatch(typeof(Technician), nameof(Technician.RepairDevice))]
    [HarmonyPostfix]
    private static void OnTechnicianRepairDevice(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "DeviceRepaired"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianRepairDevice failed: {ex.Message}");
        }
    }

    // Technician.GetCorrectDevicePrefab
    [HarmonyPatch(typeof(Technician), nameof(Technician.GetCorrectDevicePrefab))]
    [HarmonyPostfix]
    private static void OnTechnicianGetCorrectDevicePrefab(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "GetCorrectDevicePrefab"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianGetCorrectDevicePrefab failed: {ex.Message}");
        }
    }

    // Technician.RotateTowardsGoal
    [HarmonyPatch(typeof(Technician), nameof(Technician.RotateTowardsGoal))]
    [HarmonyPostfix]
    private static void OnTechnicianRotateTowardsGoal(Technician __instance, Vector3 goal)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "RotateTowardsGoal"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianRotateTowardsGoal failed: {ex.Message}");
        }
    }

    // Technician.PositionHandTargetsOnDevice
    [HarmonyPatch(typeof(Technician), nameof(Technician.PositionHandTargetsOnDevice))]
    [HarmonyPostfix]
    private static void OnTechnicianPositionHandTargetsOnDevice(Technician __instance, GameObject device)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "PositionHandTargetsOnDevice"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianPositionHandTargetsOnDevice failed: {ex.Message}");
        }
    }

    // Technician.OnLoadingStarted
    [HarmonyPatch(typeof(Technician), nameof(Technician.OnLoadingStarted))]
    [HarmonyPostfix]
    private static void OnTechnicianOnLoadingStarted(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "OnLoadingStarted"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianOnLoadingStarted failed: {ex.Message}");
        }
    }

    // Technician.OnDestroy
    [HarmonyPatch(typeof(Technician), nameof(Technician.OnDestroy))]
    [HarmonyPostfix]
    private static void OnTechnicianOnDestroy(Technician __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "OnDestroy"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianOnDestroy failed: {ex.Message}");
        }
    }

    // TechnicianManager.Awake
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.Awake))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerAwake(TechnicianManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "ComponentInitialized"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerAwake failed: {ex.Message}");
        }
    }

    // TechnicianManager.AddTechnician
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.AddTechnician))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerAddTechnician(TechnicianManager __instance, Technician technician)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "Hired"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerAddTechnician failed: {ex.Message}");
        }
    }

    // TechnicianManager.SendTechnician
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.SendTechnician))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerSendTechnician(TechnicianManager __instance, NetworkSwitch networkSwitch, Server server)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "Dispatched"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerSendTechnician failed: {ex.Message}");
        }
    }

    // TechnicianManager.RequestNextJob
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.RequestNextJob))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerRequestNextJob(TechnicianManager __instance, Technician technician)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "NextJobRequested"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerRequestNextJob failed: {ex.Message}");
        }
    }

    // TechnicianManager.EnqueueDispatch
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.EnqueueDispatch))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerEnqueueDispatch(TechnicianManager __instance, TechnicianManager.RepairJob job)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "JobQueued"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerEnqueueDispatch failed: {ex.Message}");
        }
    }

    // TechnicianManager.IsDeviceAlreadyAssigned
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.IsDeviceAlreadyAssigned))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerIsDeviceAlreadyAssigned(TechnicianManager __instance, NetworkSwitch networkSwitch, Server server)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "IsDeviceAlreadyAssigned"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerIsDeviceAlreadyAssigned failed: {ex.Message}");
        }
    }

    // TechnicianManager.RestoreJobQueue
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.RestoreJobQueue))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerRestoreJobQueue(TechnicianManager __instance, Il2CppSystem.Collections.Generic.List<RepairJobSaveData> savedJobs)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "JobQueueLoaded"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerRestoreJobQueue failed: {ex.Message}");
        }
    }

    // TechnicianManager.FireTechnician
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.FireTechnician))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerFireTechnician(TechnicianManager __instance, int technicianID)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "Fired"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerFireTechnician failed: {ex.Message}");
        }
    }

    // TechnicianManager.OpenDumpsterArea
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.OpenDumpsterArea))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerOpenDumpsterArea(TechnicianManager __instance, int areaID)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "OpenDumpsterArea"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerOpenDumpsterArea failed: {ex.Message}");
        }
    }

    // TechnicianManager.GetClosestOpenedDumpsterIndex
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.GetClosestOpenedDumpsterIndex))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerGetClosestOpenedDumpsterIndex(TechnicianManager __instance, Vector3 position)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "GetClosestOpenedDumpsterIndex"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerGetClosestOpenedDumpsterIndex failed: {ex.Message}");
        }
    }

    // TechnicianManager.OnDestroy
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.OnDestroy))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerOnDestroy(TechnicianManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "OnDestroy"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerOnDestroy failed: {ex.Message}");
        }
    }

    // TechnicianManager.OnLoadingStarted
    [HarmonyPatch(typeof(TechnicianManager), nameof(TechnicianManager.OnLoadingStarted))]
    [HarmonyPostfix]
    private static void OnTechnicianManagerOnLoadingStarted(TechnicianManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;

            SafeEmit(
                gregHookName.Create(GregDomain.Employee, "OnLoadingStarted"),
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnTechnicianManagerOnLoadingStarted failed: {ex.Message}");
        }
    }

}
