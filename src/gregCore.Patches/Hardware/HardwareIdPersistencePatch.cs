using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using System;
using MelonLoader;
using gregCore.GameLayer.Hooks;
using gregCore.Infrastructure.Persistence;

namespace gregCore.GameLayer.Patches.Hardware;

public static class HardwareIdPersistencePatch
{
    private static readonly System.Collections.Generic.HashSet<int> PatchedDevices = new();
    private const string SwitchPrefix = "gregID:Switch:";
    private const string PatchPanelPrefix = "gregID:PatchPanel:";
    private const string ServerPrefix = "gregID:Server:";

    private static string CleanId(string prefix, string deviceId, int hashCode)
    {
        if (deviceId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            if (deviceId.Contains('_'))
            {
                string cleanId = deviceId.Split('_')[0];
                if (!PatchedDevices.Contains(hashCode))
                {
                    MelonLogger.Msg($"Unity Id Removal | Cleaned Device Id | {deviceId} -> {cleanId}");
                }
                return cleanId;
            }
        }
        return deviceId;
    }

    public static string GenerateGregId(string prefix)
    {
        return $"{prefix}{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
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
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.switchId;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(SwitchPrefix);
                    __instance.switchId = uniqueId!;
                    // Display-Trennung: gameObject.name bleibt Vanilla (Persistenz
                    // steckt in switchId + Inventar-UID, unsichtbar dahinter).

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

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.GenerateUniqueSwitchId))]
    internal static class UniqueSwitchIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                int instanceKey = __instance.GetHashCode();
                __result = CleanId(SwitchPrefix, __result, instanceKey);
                if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(UniqueSwitchIdPatch), ex); }
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
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.patchPanelId;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(PatchPanelPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(PatchPanelPrefix);
                    __instance.patchPanelId = uniqueId!;
                    // Display-Trennung: gameObject.name bleibt Vanilla (siehe Switch).

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

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.GenerateUniquePatchPanelId))]
    internal static class UniquePatchPanelIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                int instanceKey = __instance.GetHashCode();
                __result = CleanId(PatchPanelPrefix, __result, instanceKey);
                if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(UniquePatchPanelIdPatch), ex); }
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
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.ServerID;
                if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(ServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(ServerPrefix);
                    __instance.ServerID = uniqueId!;

                    // Display-Trennung: gameObject.name bleibt Vanilla (siehe Switch).

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

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.GenerateUniqueServerId))]
    internal static class UniqueServerIdPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                int instanceKey = __instance.GetHashCode();
                __result = CleanId(ServerPrefix, __result, instanceKey);
                if (!PatchedDevices.Contains(instanceKey)) PatchedDevices.Add(instanceKey);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(UniqueServerIdPatch), ex); }
        }
    }

    #endregion

    #region DISPLAY SCRUB (Vanilla-Bezeichnung auf Screens)

    // Screens duerfen nie gregIDs zeigen: Falls ein Screen-Text ein gregID-
    // Token enthaelt (ServerID-Durchgriff), wird nur das Token durch die
    // Vanilla-Bezeichnung ersetzt. Vanilla-Texte ohne Token: No-Op.
    // HINWEIS: Aeltere Saves/Sessions mit umbenannten Objekten (Name =
    // gregID) fallen auf "Server"/"Switch" zurueck - einmalig, danach ist
    // der Name wieder Vanilla (Namen werden nicht gespeichert).

    private static string VanillaDesignation(global::UnityEngine.GameObject go, string fallback)
    {
        try
        {
            string n = null;
            try { n = go != null ? go.name : null; } catch { }
            string clean = GregEntityInventory.CleanDisplayName(n);
            return string.IsNullOrEmpty(clean) ? fallback : clean;
        }
        catch { return fallback; }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.UpdateServerScreenUI))]
    internal static class ServerScreenScrubPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                var txt = __instance.txtServerScreen;
                if (txt == null || txt.Pointer == IntPtr.Zero) return;
                string cur = null;
                try { cur = txt.text; } catch { return; }
                if (string.IsNullOrEmpty(cur)
                    || cur.IndexOf("gregID:", StringComparison.OrdinalIgnoreCase) < 0) return;
                string designation = VanillaDesignation(__instance.gameObject, "Server");
                try { txt.text = GregEntityInventory.ScrubGregIds(cur, designation); } catch { }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(ServerScreenScrubPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.UpdateScreenUI))]
    internal static class SwitchScreenScrubPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                var txt = __instance.txtScreen;
                if (txt == null || txt.Pointer == IntPtr.Zero) return;
                string cur = null;
                try { cur = txt.text; } catch { return; }
                if (string.IsNullOrEmpty(cur)
                    || cur.IndexOf("gregID:", StringComparison.OrdinalIgnoreCase) < 0) return;
                string designation = VanillaDesignation(__instance.gameObject, "Switch");
                try { txt.text = GregEntityInventory.ScrubGregIds(cur, designation); } catch { }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(SwitchScreenScrubPatch), ex); }
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
    public static void Prefix(
        global::Il2Cpp.NetworkSaveData networkData,
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.RackPosition> allRackPositions,
        int saveVersion)
    {
        try
        {
            if (networkData == null || networkData.Pointer == IntPtr.Zero) return;
            var data = networkData;

            MelonLogger.Msg("Checking for legacy IDs in map data...");

            foreach (var swData in data.switches)
            {
                string oldId = swData.switchID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(SwitchPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string newGuid = HardwareIdPersistencePatch.GenerateGregId(SwitchPrefix);
                    swData.switchID = newGuid;
                    int healedCables = 0;
                    foreach (var cable in data.cables)
                    {
                        if (cable.startPoint.switchID == oldId) { cable.startPoint.switchID = newGuid; healedCables++; }
                        if (cable.endPoint.switchID == oldId) { cable.endPoint.switchID = newGuid; healedCables++; }
                    }
                    MelonLogger.Msg($"Legacy Mapping | Switch: {oldId} -> {newGuid} | Healed Cables: {healedCables}");
                }
            }

            foreach (var ppData in data.patchPanels)
            {
                string oldId = ppData.patchPanelID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(PatchPanelPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string newGuid = HardwareIdPersistencePatch.GenerateGregId(PatchPanelPrefix);
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
                    MelonLogger.Msg($"Legacy Mapping | Patch Panel: {oldId} -> {newGuid} | Healed Cables: {healedCables}");
                }
            }

            foreach (var serverData in data.servers)
            {
                string oldId = serverData.serverID;
                if (!string.IsNullOrEmpty(oldId) && !oldId.StartsWith(ServerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    string newGuid = HardwareIdPersistencePatch.GenerateGregId(ServerPrefix);
                    serverData.serverID = newGuid;
                    int healedCables = 0;
                    foreach (var cable in data.cables)
                    {
                        if (cable.startPoint.serverID == oldId) { cable.startPoint.serverID = newGuid; healedCables++; }
                        if (cable.endPoint.serverID == oldId) { cable.endPoint.serverID = newGuid; healedCables++; }
                    }
                    MelonLogger.Msg($"Legacy Mapping | Server: {oldId} -> {newGuid} | Healed Cables: {healedCables}");
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
