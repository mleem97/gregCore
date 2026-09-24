/// <file-summary>
/// Schicht:      GameLayer (Patches/Hardware)
/// Design:       Eigene GregCore-Implementierung (gregID-Schema).
/// Zweck:        Stabile Geraete-Identitaet fuer NetworkSwitch/PatchPanel/
///               Server. Vanilla haengt GetInstanceID-Suffixe an
///               (pro Session anders) — ohne Bereinigung zeigen
///               Kabel-Endpunkte nach Save/Load auf tote IDs.
///               Single-Scheme-System: Es gibt genau ein stabiles Schema
///               (gregID:...). Alles ohne gregID-Praefix — leer,
///               vanilla-generiert oder fremd — wird exakt einmal auf
///               gregID ueberfuehrt (Live-Objekte bei Start/Awake,
///               Save-Daten beim Laden inkl. Kabel-Endpunkten).
///               Screens bleiben frei von ID-Tokens. Alles best-effort.
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

    // Eigene IDs (beliebiges Praefix, case-insensitiv).
    internal static bool HasPrefix(string prefix, string deviceId)
    {
        return !string.IsNullOrEmpty(deviceId)
            && deviceId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    // Entfernt GetInstanceID-Suffixe ("Name_123456", auch negativ "-123").
    // Strikter als Split('_')[0]: Suffixe mit Buchstaben (Nutzer-Benennung
    // wie "Core_Switch_A") bleiben erhalten.
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
            MelonLogger.Msg($"[gregCore][HwId] Suffix entfernt: {deviceId} -> {cleanId}");
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
                    // Single-Scheme: jede Nicht-gregID wird exakt einmal
                    // ueberfuehrt (leer, vanilla oder fremd).
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
                    // Single-Scheme: jede Nicht-gregID wird exakt einmal
                    // ueberfuehrt (leer, vanilla oder fremd).
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
                    // Single-Scheme: jede Nicht-gregID wird exakt einmal
                    // ueberfuehrt (leer, vanilla oder fremd).
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

            MelonLogger.Msg("[gregCore][HwId] Pruefe Kartendaten auf Legacy-IDs...");

            // Geheilt wird jede ID ohne gregID-Praefix (leer ausgenommen —
            // leere IDs vergibt der Start-Patch). Fremde Schemata werden
            // dabei wie Vanilla behandelt: einmalig auf gregID ueberfuehrt,
            // Kabel-Endpunkte wandern mit. Null-Guards pro Eintrag: Ein
            // kaputter Eintrag darf nie das gesamte Healing abbrechen.
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
                        MelonLogger.Msg($"[gregCore][HwId] Legacy-Mapping | Switch: {oldId} -> {newGuid} | Kabel: {healedCables}");
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
                        MelonLogger.Msg($"[gregCore][HwId] Legacy-Mapping | PatchPanel: {oldId} -> {newGuid} | Kabel: {healedCables}");
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
                        MelonLogger.Msg($"[gregCore][HwId] Legacy-Mapping | Server: {oldId} -> {newGuid} | Kabel: {healedCables}");
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
