/// <file-summary>
/// Layer:   GameLayer (Patches/Hardware)
/// Design:  Own GregCore implementation (gregID schema).
/// Purpose: Stable device identity for NetworkSwitch/PatchPanel/Server.
///          Vanilla appends GetInstanceID suffixes (different every session)
///          which kills cable endpoints after save/load.
///          Single-scheme: exactly one stable schema (gregID:...). Anything
///          without it is converted to gregID exactly once (live objects at
///          Start/Awake, save data on load incl. cable endpoints).
///          Screens stay ID-token-free. All best-effort.
/// </file-summary>

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
    private static readonly System.Collections.Generic.HashSet<string> _loggedIds = new();
    private const string SwitchPrefix = "gregID:Switch:";
    private const string PatchPanelPrefix = "gregID:PatchPanel:";
    private const string ServerPrefix = "gregID:Server:";

    // Own IDs (any prefix, case-insensitive).
    internal static bool HasPrefix(string prefix, string deviceId)
    {
        return !string.IsNullOrEmpty(deviceId)
            && deviceId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    // Strips GetInstanceID suffixes ("Name_123456", incl. negative).
    // Stricter than Split('_')[0]: letter suffixes ("Core_Switch_A") survive.
    private static string CleanId(string prefix, string deviceId)
    {
        if (!HasPrefix(prefix, deviceId)) return deviceId;
        int cut = deviceId.LastIndexOf('_');
        if (cut < 0 || cut + 1 >= deviceId.Length) return deviceId;
        string suffix = deviceId.Substring(cut + 1);
        if (!int.TryParse(suffix, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            return deviceId;

        string cleanId = deviceId.Substring(0, cut);
        if (_loggedIds.Add(prefix + "|" + cleanId))
        {
            MelonLogger.Msg($"[gregCore][HwId] Unsuffixed: {deviceId} -> {cleanId}");
        }
        return cleanId;
    }

    public static string GenerateGregId(string prefix)
    {
        return $"{prefix}{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
    }

    #region SWITCH ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.Start))]
    internal static class GregSwitchIdAssignPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.switchId;
                if (HasPrefix(SwitchPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once.
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(SwitchPrefix);
                    __instance.switchId = uniqueId!;
                    // Display separation: gameObject.name stays vanilla.

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }

                    __instance.UpdateScreenUI();
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregSwitchIdAssignPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.GenerateUniqueSwitchId))]
    internal static class GregSwitchIdCleanPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.NetworkSwitch __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                __result = CleanId(SwitchPrefix, __result);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregSwitchIdCleanPatch), ex); }
        }
    }

    #endregion

    #region PATCH PANEL ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.Awake))]
    internal static class GregPatchPanelIdAssignPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.patchPanelId;
                if (HasPrefix(PatchPanelPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once.
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(PatchPanelPrefix);
                    __instance.patchPanelId = uniqueId!;
                    // Display separation: gameObject.name stays vanilla.

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregPatchPanelIdAssignPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.PatchPanel), nameof(global::Il2Cpp.PatchPanel.GenerateUniquePatchPanelId))]
    internal static class GregPatchPanelIdCleanPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.PatchPanel __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                __result = CleanId(PatchPanelPrefix, __result);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregPatchPanelIdCleanPatch), ex); }
        }
    }

    #endregion

    #region SERVER ID PERSISTENCE

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.Start))]
    internal static class GregServerIdAssignPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.ServerID;
                if (HasPrefix(ServerPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once.
                    string uniqueId = HardwareIdPersistencePatch.GenerateGregId(ServerPrefix);
                    __instance.ServerID = uniqueId!;

                    // Display separation: gameObject.name stays vanilla.

                    if (global::Il2Cpp.SaveSystem.displayToRawMap != null)
                    {
                        global::Il2Cpp.SaveSystem.displayToRawMap[uniqueId] = uniqueId;
                        if (!string.IsNullOrEmpty(currentId))
                            global::Il2Cpp.SaveSystem.displayToRawMap.Remove(currentId);
                    }

                    __instance.UpdateServerScreenUI();
                }
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregServerIdAssignPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.GenerateUniqueServerId))]
    internal static class GregServerIdCleanPatch
    {
        [HarmonyPostfix]
        internal static void Postfix(global::Il2Cpp.Server __instance, ref string __result)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (string.IsNullOrEmpty(__result)) return;
                __result = CleanId(ServerPrefix, __result);
            }
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregServerIdCleanPatch), ex); }
        }
    }

    #endregion

    #region DISPLAY SCRUB (Vanilla-Bezeichnung auf Screens)

    // Screens must never show gregIDs: if a screen text contains one,
    // only the token is replaced with the vanilla designation.
    // Vanilla texts without tokens: no-op.

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
    internal static class GregServerScreenScrubPatch
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
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregServerScreenScrubPatch), ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.NetworkSwitch), nameof(global::Il2Cpp.NetworkSwitch.UpdateScreenUI))]
    internal static class GregSwitchScreenScrubPatch
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
            catch (Exception ex) { HookIntegration.LogPatchError(nameof(GregSwitchScreenScrubPatch), ex); }
        }
    }

    #endregion
}

[HarmonyPatch(typeof(global::Il2Cpp.WaypointInitializationSystem), nameof(global::Il2Cpp.WaypointInitializationSystem.LoadNetworkState))]
public static class GregNetworkIdHealing
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

            MelonLogger.Msg("[gregCore][HwId] Healing map IDs...");

            // Heals every ID without gregID prefix (empty excluded — the
            // Start patch assigns those). Endpoints follow. Per-entry
            // null-guards: one bad entry never aborts the whole healing.
            if (data.switches != null)
            {
                foreach (var swData in data.switches)
                {
                    try
                    {
                        if (swData == null) continue;
                        string oldId = swData.switchID;
                        if (string.IsNullOrEmpty(oldId)
                            || HardwareIdPersistencePatch.HasPrefix(SwitchPrefix, oldId))
                            continue;
                        string newGuid = HardwareIdPersistencePatch.GenerateGregId(SwitchPrefix);
                        swData.switchID = newGuid;
                        int healedCables = 0;
                        if (data.cables != null)
                        {
                            foreach (var cable in data.cables)
                            {
                                if (cable == null) continue;
                                if (cable.startPoint != null && cable.startPoint.switchID == oldId) { cable.startPoint.switchID = newGuid; healedCables++; }
                                if (cable.endPoint != null && cable.endPoint.switchID == oldId) { cable.endPoint.switchID = newGuid; healedCables++; }
                            }
                        }
                        MelonLogger.Msg($"[gregCore][HwId] Remap Switch {oldId} -> {newGuid} ({healedCables} cables)");
                    }
                    catch { }
                }
            }

            if (data.patchPanels != null)
            {
                foreach (var ppData in data.patchPanels)
                {
                    try
                    {
                        if (ppData == null) continue;
                        string oldId = ppData.patchPanelID;
                        if (string.IsNullOrEmpty(oldId)
                            || HardwareIdPersistencePatch.HasPrefix(PatchPanelPrefix, oldId))
                            continue;
                        string newGuid = HardwareIdPersistencePatch.GenerateGregId(PatchPanelPrefix);
                        ppData.patchPanelID = newGuid;
                        int healedCables = 0;
                        if (data.cables != null)
                        {
                            foreach (var cable in data.cables)
                            {
                                if (cable == null) continue;
                                if (cable.startPoint != null && cable.startPoint.switchID != null && cable.startPoint.switchID.StartsWith(oldId))
                                {
                                    cable.startPoint.switchID = cable.startPoint.switchID.Replace(oldId, newGuid);
                                    healedCables++;
                                }
                                if (cable.endPoint != null && cable.endPoint.switchID != null && cable.endPoint.switchID.StartsWith(oldId))
                                {
                                    cable.endPoint.switchID = cable.endPoint.switchID.Replace(oldId, newGuid);
                                    healedCables++;
                                }
                            }
                        }
                        MelonLogger.Msg($"[gregCore][HwId] Remap PatchPanel {oldId} -> {newGuid} ({healedCables} cables)");
                    }
                    catch { }
                }
            }

            if (data.servers != null)
            {
                foreach (var serverData in data.servers)
                {
                    try
                    {
                        if (serverData == null) continue;
                        string oldId = serverData.serverID;
                        if (string.IsNullOrEmpty(oldId)
                            || HardwareIdPersistencePatch.HasPrefix(ServerPrefix, oldId))
                            continue;
                        string newGuid = HardwareIdPersistencePatch.GenerateGregId(ServerPrefix);
                        serverData.serverID = newGuid;
                        int healedCables = 0;
                        if (data.cables != null)
                        {
                            foreach (var cable in data.cables)
                            {
                                if (cable == null) continue;
                                if (cable.startPoint != null && cable.startPoint.serverID == oldId) { cable.startPoint.serverID = newGuid; healedCables++; }
                                if (cable.endPoint != null && cable.endPoint.serverID == oldId) { cable.endPoint.serverID = newGuid; healedCables++; }
                            }
                        }
                        MelonLogger.Msg($"[gregCore][HwId] Remap Server {oldId} -> {newGuid} ({healedCables} cables)");
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(GregNetworkIdHealing), ex);
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
            HookIntegration.LogPatchError(nameof(GregNetworkIdHealing), ex);
        }
    }
}
