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
using System.Security.Cryptography;
using System.Text;
using MelonLoader;
using gregCore.Core.Diagnostics;
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
        return $"{prefix}{Guid.NewGuid().ToString("N").ToUpperInvariant().Substring(0, 12)}";
    }

    /// <summary>
    /// Derives a STABLE gregID from a legacy device ID (SHA-256, 12 hex chars).
    /// The same legacy ID always yields the same gregID — on live objects at
    /// Start/Awake AND on save data during healing — so both sides correlate
    /// by construction, regardless of how vanilla binds save entries to live
    /// objects (by ID or by position). Session-stable and machine-stable.
    /// Uniqueness follows from legacy-ID uniqueness (48-bit space; vanilla
    /// guarantees unique device IDs per session).
    /// Empty input falls back to a random ID (nothing can reference it yet).
    /// </summary>
    public static string GenerateStableGregId(string prefix, string legacyId)
    {
        try
        {
            if (string.IsNullOrEmpty(legacyId)) return GenerateGregId(prefix);
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(legacyId));
            var sb = new StringBuilder(12);
            for (int i = 0; i < 6; i++) sb.Append(hash[i].ToString("X2"));
            return prefix + sb.ToString();
        }
        catch
        {
            return GenerateGregId(prefix);
        }
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
                if (!GregGameCompat.NotifyHwIdGate()) return;
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.switchId;
                if (HasPrefix(SwitchPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once.
                    // Stable derivation: the same legacy ID yields the same
                    // gregID here and in save healing, so live objects and
                    // save endpoints correlate by construction.
                    string uniqueId = HardwareIdPersistencePatch.GenerateStableGregId(SwitchPrefix, currentId);
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
                if (!GregGameCompat.NotifyHwIdGate()) return;
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.patchPanelId;
                if (HasPrefix(PatchPanelPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once
                    // (stable derivation, see switch patch).
                    string uniqueId = HardwareIdPersistencePatch.GenerateStableGregId(PatchPanelPrefix, currentId);
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
                if (!GregGameCompat.NotifyHwIdGate()) return;
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                string currentId = __instance.ServerID;
                if (HasPrefix(ServerPrefix, currentId)) return;
                {
                    // Single-scheme: any non-gregID becomes gregID exactly once
                    // (stable derivation, see switch patch).
                    string uniqueId = HardwareIdPersistencePatch.GenerateStableGregId(ServerPrefix, currentId);
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

    #region DISPLAY SCRUB (vanilla designation on screens)

    // Screens must never show gregIDs: if a screen text contains one,
    // only the token is replaced with the vanilla designation.
    // Vanilla texts without tokens: no-op.

    private static string VanillaDesignation(global::UnityEngine.GameObject go, string fallback)
    {
        try
        {
            string n = null;
            try { n = go != null ? go.name : null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
                try { txt.text = GregEntityInventory.ScrubGregIds(cur, designation); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
                try { txt.text = GregEntityInventory.ScrubGregIds(cur, designation); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
            // Route-safe passthrough on unknown/unsupported builds: never
            // renumber devices we cannot verify (broken IDs = broken routes).
            if (!GregGameCompat.NotifyHwIdGate()) return;
            if (networkData == null || networkData.Pointer == IntPtr.Zero) return;
            var data = networkData;

            MelonLogger.Msg("[gregCore][HwId] Healing map IDs...");

            // Heals every ID without gregID prefix (empty excluded — the
            // Start patch assigns those). Endpoints follow. Per-entry
            // null-guards: one bad entry never aborts the whole healing.
            HealSwitches(data);

            HealPatchPanels(data);

            HealServers(data);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(GregNetworkIdHealing), ex);
        }
    }

    private static void HealSwitches(global::Il2Cpp.NetworkSaveData data)
    {
        if (data.switches == null) return;
        foreach (var swData in data.switches)
        {
            try
            {
                if (swData == null) continue;
                string oldId = swData.switchID;
                if (string.IsNullOrEmpty(oldId)
                    || HardwareIdPersistencePatch.HasPrefix(SwitchPrefix, oldId))
                    continue;
                string newGuid = HardwareIdPersistencePatch.GenerateStableGregId(SwitchPrefix, oldId);
                swData.switchID = newGuid;
                int healed = HealSwitchEndpoints(data, oldId, newGuid);
                MelonLogger.Msg($"[gregCore][HwId] Remap Switch {oldId} -> {newGuid} ({healed} cables)");
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static int HealSwitchEndpoints(global::Il2Cpp.NetworkSaveData data, string oldId, string newGuid)
    {
        int healedCables = 0;
        if (data.cables == null) return healedCables;
        foreach (var cable in data.cables)
        {
            if (cable == null) continue;
            if (cable.startPoint != null && cable.startPoint.switchID == oldId) { cable.startPoint.switchID = newGuid; healedCables++; }
            if (cable.endPoint != null && cable.endPoint.switchID == oldId) { cable.endPoint.switchID = newGuid; healedCables++; }
        }
        return healedCables;
    }

    private static void HealPatchPanels(global::Il2Cpp.NetworkSaveData data)
    {
        if (data.patchPanels == null) return;
        foreach (var ppData in data.patchPanels)
        {
            try
            {
                if (ppData == null) continue;
                string oldId = ppData.patchPanelID;
                if (string.IsNullOrEmpty(oldId)
                    || HardwareIdPersistencePatch.HasPrefix(PatchPanelPrefix, oldId))
                    continue;
                string newGuid = HardwareIdPersistencePatch.GenerateStableGregId(PatchPanelPrefix, oldId);
                ppData.patchPanelID = newGuid;
                int healed = HealPatchPanelEndpoints(data, oldId, newGuid);
                MelonLogger.Msg($"[gregCore][HwId] Remap PatchPanel {oldId} -> {newGuid} ({healed} cables)");
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static int HealPatchPanelEndpoints(global::Il2Cpp.NetworkSaveData data, string oldId, string newGuid)
    {
        int healedCables = 0;
        if (data.cables == null) return healedCables;
        foreach (var cable in data.cables)
        {
            if (cable == null) continue;
            if (cable.startPoint != null && cable.startPoint.switchID != null
                && GregPanelEndpointMatcher.Matches(cable.startPoint.switchID, oldId))
            {
                cable.startPoint.switchID = GregPanelEndpointMatcher.Remap(cable.startPoint.switchID, oldId, newGuid);
                healedCables++;
            }
            if (cable.endPoint != null && cable.endPoint.switchID != null
                && GregPanelEndpointMatcher.Matches(cable.endPoint.switchID, oldId))
            {
                cable.endPoint.switchID = GregPanelEndpointMatcher.Remap(cable.endPoint.switchID, oldId, newGuid);
                healedCables++;
            }
        }
        return healedCables;
    }

    private static void HealServers(global::Il2Cpp.NetworkSaveData data)
    {
        if (data.servers == null) return;
        foreach (var serverData in data.servers)
        {
            try
            {
                if (serverData == null) continue;
                string oldId = serverData.serverID;
                if (string.IsNullOrEmpty(oldId)
                    || HardwareIdPersistencePatch.HasPrefix(ServerPrefix, oldId))
                    continue;
                string newGuid = HardwareIdPersistencePatch.GenerateStableGregId(ServerPrefix, oldId);
                serverData.serverID = newGuid;
                int healed = HealServerEndpoints(data, oldId, newGuid);
                MelonLogger.Msg($"[gregCore][HwId] Remap Server {oldId} -> {newGuid} ({healed} cables)");
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static int HealServerEndpoints(global::Il2Cpp.NetworkSaveData data, string oldId, string newGuid)
    {
        int healedCables = 0;
        if (data.cables == null) return healedCables;
        foreach (var cable in data.cables)
        {
            if (cable == null) continue;
            if (cable.startPoint != null && cable.startPoint.serverID == oldId) { cable.startPoint.serverID = newGuid; healedCables++; }
            if (cable.endPoint != null && cable.endPoint.serverID == oldId) { cable.endPoint.serverID = newGuid; healedCables++; }
        }
        return healedCables;
    }

    [HarmonyPostfix]
    public static void Postfix(global::Il2Cpp.WaypointInitializationSystem __instance)
    {
        try
        {
            // Skipped together with healing: pure vanilla passthrough.
            if (!GregGameCompat.HwIdRewritesAllowed) return;
            __instance.RequestRouteEvaluation();
            // Load-time diagnosis for duplicate rack positions (Mantis #19):
            // log-only, never mutates.
            try { GregRackOverlapGuard.ScanAndReport(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(GregNetworkIdHealing), ex);
        }
    }
}
