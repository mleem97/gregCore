using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using System;
using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Hardware;

public static class HardwareIdPersistencePatch
{
    private static readonly System.Collections.Generic.HashSet<int> PatchedDevices = new();
    private const string SwitchPrefix = "gregID:Switch:";
    private const string PatchPanelPrefix = "gregID:PatchPanel:";
    private const string ServerPrefix = "gregID:Server:";

    private static string CleanId(string prefix, string deviceId, int hashCode)
    {
        if (deviceId != null && deviceId.StartsWith(prefix) && deviceId.Contains('_'))
        {
            string cleanId = deviceId.Split('_')[0];
            if (!PatchedDevices.Contains(hashCode))
            {
                greg.Logging.GregLogger.Msg($"Unity Id Removal | Cleaned Device Id | {deviceId} -> {cleanId}", "PersistentID");
            }
            return cleanId;
        }
        return deviceId;
    }

    #region SWITCH ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.Start))]
    internal static class NewSwitchIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance)
        {
            try
            {
                string currentId = __instance.switchId;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(SwitchPrefix))
                {
                    string uniqueId = $"{SwitchPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    __instance.switchId = uniqueId;
                    __instance.gameObject.name = uniqueId;

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }

                    __instance.UpdateScreenUI();
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(NewSwitchIdPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.switchId), MethodType.Getter)]
    internal static class SwitchIdPropertyPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(SwitchPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.GenerateUniqueSwitchId))]
    internal static class UniqueSwitchIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(SwitchPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    #endregion

    #region PATCH PANEL ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.Awake))]
    internal static class NewPatchPanelIdpatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance)
        {
            try
            {
                string currentId = __instance.patchPanelId;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(PatchPanelPrefix))
                {
                    string uniqueId = $"{PatchPanelPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    __instance.patchPanelId = uniqueId;
                    __instance.gameObject.name = uniqueId;

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(NewPatchPanelIdpatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.patchPanelId), MethodType.Getter)]
    internal static class PatchPanelIdPropertyPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(PatchPanelPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.GenerateUniquePatchPanelId))]
    internal static class UniquePatchPanelIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(PatchPanelPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    #endregion

    #region SERVER ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.Start))]
    internal static class NewServerIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance)
        {
            try
            {
                string currentId = __instance.ServerID;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(ServerPrefix))
                {
                    string uniqueId = $"{ServerPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    __instance.ServerID = uniqueId;
                    __instance.gameObject.name = uniqueId;

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }

                    __instance.UpdateServerScreenUI();
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(NewServerIdPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.ServerID), MethodType.Getter)]
    internal static class ServerIdPropertyPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(ServerPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.GenerateUniqueServerId))]
    internal static class UniqueServerIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance, ref string __result)
        {
            if (string.IsNullOrEmpty(__result)) return;
            int instanceKey = __instance.GetHashCode();
            __result = CleanId(ServerPrefix, __result, instanceKey);
            if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
        }
    }

    #endregion
}

[HarmonyPatch(typeof(global::Il2Cpp.WaypointInitializationSystem), nameof(global::Il2Cpp.WaypointInitializationSystem.LoadNetworkState))]
public static class MapDataHealing
{
    private const string SwitchPrefix = "gregID:Switch:";
    private const string PatchPanelPrefix = "gregID:PatchPanel:";
    private const string ServerPrefix = "gregID:Server:";

    [HarmonyPrefix]
    public static void Prefix(object[] __args)
    {
        try
        {
            if (__args == null || __args.Length < 3) return;
            global::Il2Cpp.NetworkSaveData data = (global::Il2Cpp.NetworkSaveData)__args[0];

            greg.Logging.GregLogger.Msg("Checking for legacy IDs in map data...", "PersistentID");

            foreach (var swData in data.switches)
            {
                string oldId = swData.switchID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(SwitchPrefix))
                {
                    string newGuid = $"{SwitchPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    swData.switchID = newGuid;
                    int healedCables = 0;
                    foreach (var cable in data.cables)
                    {
                        if (cable.startPoint.switchID == oldId) { cable.startPoint.switchID = newGuid; healedCables++; }
                        if (cable.endPoint.switchID == oldId) { cable.endPoint.switchID = newGuid; healedCables++; }
                    }
                    greg.Logging.GregLogger.Msg($"Legacy Mapping | Switch: {oldId} -> {newGuid} | Healed Cables: {healedCables}", "PersistentID");
                }
            }

            foreach (var ppData in data.patchPanels)
            {
                string oldId = ppData.patchPanelID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(PatchPanelPrefix))
                {
                    string newGuid = $"{PatchPanelPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    ppData.patchPanelID = newGuid;
                    int healedCables = 0;
                    foreach (var cable in data.cables)
                    {
                        if (cable.startPoint.switchID != null && cable.startPoint.switchID.StartsWith(oldId))
                        {
                            cable.startPoint.switchID = cable.startPoint.switchID.Replace(oldId, newGuid);
                            healedCables++;
                        }
                        if (cable.endPoint.switchID != null && cable.endPoint.switchID.StartsWith(oldId))
                        {
                            cable.endPoint.switchID = cable.endPoint.switchID.Replace(oldId, newGuid);
                            healedCables++;
                        }
                    }
                    greg.Logging.GregLogger.Msg($"Legacy Mapping | Patch Panel: {oldId} -> {newGuid} | Healed Cables: {healedCables}", "PersistentID");
                }
            }

            foreach (var serverData in data.servers)
            {
                string oldId = serverData.serverID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(ServerPrefix))
                {
                    string newGuid = $"{ServerPrefix}{Guid.NewGuid().ToString().Substring(0, 8)}";
                    serverData.serverID = newGuid;
                    int healedCables = 0;
                    foreach (var cable in data.cables)
                    {
                        if (cable.startPoint.serverID == oldId) { cable.startPoint.serverID = newGuid; healedCables++; }
                        if (cable.endPoint.serverID == oldId) { cable.endPoint.serverID = newGuid; healedCables++; }
                    }
                    greg.Logging.GregLogger.Msg($"Legacy Mapping | Server: {oldId} -> {newGuid} | Healed Cables: {healedCables}", "PersistentID");
                }
            }
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(MapDataHealing), ex);
        }
    }

    [HarmonyPostfix]
    public static void Postfix(global::Il2Cpp.WaypointInitializationSystem __instance)
    {
        try
        {
            __instance.RequestRouteEvaluation();
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(MapDataHealing), ex);
        }
    }
}
