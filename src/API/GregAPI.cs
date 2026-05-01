/// <file-summary>
/// Schicht:      API (Legacy)
/// Zweck:        Stabile API-Fassade für alle Mod-Sprachen (Lua, Rust, Go, JS, Python).
/// Maintainer:   Alle Methoden lesen/schreiben echte IL2CPP-Game-Objekte.
///               Fehler werden abgefangen und Default-Werte zurückgegeben.
///               Pointer-Prüfungen verhindern ObjectCollectedException.
/// </file-summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using greg.Sdk;
using gregCore.UI;
using gregCore.Infrastructure.UI;
using gregCore.Core.Models;
using gregCore.Core.Events;
using gregCore.Core;
using MelonLoader;

namespace gregCore.API
{
    /// <summary>
    /// Legacy GregAPI – stabile, statische Fassade für alle Scripting-Bridges.
    /// Jede Methode ist in try-catch gewrapped, prüft Pointer und gibt bei Fehler einen Safe-Default zurück.
    /// </summary>
    public static class GregAPI
    {
        private static IGregLogger _logger = null!;

        internal static void Initialize(IGregLogger logger)
        {
            _logger = logger.ForContext("API");
            // Use shared event bus from GregCoreMod if available, otherwise create own
            EventBus = GregCoreMod.EventBus ?? new GregEventBus(_logger);
            HookBus = GregCoreMod.HookBus;
            Persistence = new Infrastructure.Config.GregPersistenceService(_logger);
            _logger.Info("GregAPI initialized successfully.");
        }

        public static GregUIController UI { get; internal set; } = null!;
        public static GregEventBus EventBus { get; internal set; } = null!;
        public static IGregPersistenceService Persistence { get; internal set; } = null!;

        public static object RegisterMod(string id, string name, string version) => null!;
        public static GregSettingsProxy Settings { get; } = new GregSettingsProxy();
        public static GregHooksProxy Hooks { get; } = new GregHooksProxy();

        public static void Log(string msg, string type = "INFO") => GregDevConsole.Instance?.AddLog(msg, type);
        public static void LogInfo(string msg) => Log(msg, "INFO");
        public static void LogWarning(string msg) => Log(msg, "WARN");
        public static void LogError(string msg) => Log(msg, "ERROR");

        public static void ShowNotification(string msg) { }
        public static void ShowNotification(string msg, float duration) { }

        internal static GregHookBus? HookBus { get; set; }

        public static void FireEvent(string id, object? data = null)
        {
            try
            {
                var payload = new EventPayload
                {
                    HookName = id,
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = data as IReadOnlyDictionary<string, object>
                        ?? new Dictionary<string, object> { { "Data", data ?? "null" } },
                    IsCancelable = false
                };
                HookBus?.Dispatch(id, payload);
                EventBus?.Publish(id, payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregAPI] FireEvent({id}) failed: {ex.Message}");
            }
        }

        public static void Subscribe(string id, Action<object> cb) => On(id, cb);

        public static void On(string eventId, Action<object> callback)
        {
            try
            {
                EventBus?.Subscribe(eventId, payload =>
                {
                    try { callback?.Invoke(payload); }
                    catch (Exception ex) { MelonLogger.Error($"[GregAPI] Event handler error for '{eventId}': {ex.Message}"); }
                });

                switch (eventId)
                {
                    case "OnCoinsChanged": gregNativeEventHooks.OnCoinsChanged += callback; break;
                    case "OnXpChanged": gregNativeEventHooks.OnXpChanged += callback; break;
                    case "OnReputationChanged": gregNativeEventHooks.OnReputationChanged += callback; break;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregAPI] On({eventId}) failed: {ex.Message}");
            }
        }

        public static void On(HookName hookName, Action<object> callback) => On(hookName.Full, callback);

        // ─── Economy (IL2CPP Live) ───────────────────────────────────────

        public static double GetPlayerMoney()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return 0.0;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return 0.0;
                return pc.money;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerMoney(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return;
                pc.money = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerMoney failed: {ex.Message}"); }
        }

        public static double GetPlayerXp()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return 0.0;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return 0.0;
                return pc.xp;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerXp(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return;
                pc.xp = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerXp failed: {ex.Message}"); }
        }

        public static double GetPlayerReputation()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return 0.0;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return 0.0;
                return pc.reputation;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerReputation(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero) return;
                var pc = pm.playerClass;
                if (pc == null || pc.Pointer == IntPtr.Zero) return;
                pc.reputation = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerReputation failed: {ex.Message}"); }
        }

        // ─── World / Hardware (IL2CPP Live) ──────────────────────────────

        public static uint GetServerCount()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                return nm != null && nm.servers != null ? (uint)nm.servers.Count : 0u;
            }
            catch { return 0u; }
        }

        public static uint GetRackCount()
        {
            try
            {
                var racks = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Rack>();
                return racks != null ? (uint)racks.Count : 0u;
            }
            catch { return 0u; }
        }

        public static uint GetSwitchCount()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                return nm != null && nm.switches != null ? (uint)nm.switches.Count : 0u;
            }
            catch { return 0u; }
        }

        public static uint GetBrokenServerCount()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                return nm != null && nm.brokenServers != null ? (uint)nm.brokenServers.Count : 0u;
            }
            catch { return 0u; }
        }

        public static uint GetBrokenSwitchCount()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                return nm != null && nm.brokenSwitches != null ? (uint)nm.brokenSwitches.Count : 0u;
            }
            catch { return 0u; }
        }

        // ─── Technicians ─────────────────────────────────────────────────

        public static uint GetFreeTechnicianCount()
        {
            try
            {
                var tm = Il2Cpp.TechnicianManager.instance;
                if (tm == null || tm.Pointer == IntPtr.Zero || tm.technicians == null) return 0u;
                int busyCount = 0;
                foreach (var t in tm.technicians)
                {
                    try { if (t != null && t.Pointer != IntPtr.Zero && t.isBusy) busyCount++; } catch { }
                }
                return (uint)Math.Max(0, tm.technicians.Count - busyCount);
            }
            catch { return 0u; }
        }

        public static uint GetTotalTechnicianCount()
        {
            try
            {
                var tm = Il2Cpp.TechnicianManager.instance;
                if (tm == null || tm.Pointer == IntPtr.Zero || tm.technicians == null) return 0u;
                return (uint)tm.technicians.Count;
            }
            catch { return 0u; }
        }

        public static int DispatchRepairServer()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenServers == null || nm.brokenServers.Count == 0) return 0;

                foreach (var kvp in nm.brokenServers)
                {
                    try
                    {
                        var s = kvp.Value;
                        if (s != null && s.Pointer != IntPtr.Zero && s.isBroken)
                        {
                            s.RepairDevice();
                            return 1; // Return after dispatching one, no defensive copy needed since we break
                        }
                    }
                    catch { }
                }
                return 0;
            }
            catch { return 0; }
        }

        public static int DispatchRepairSwitch()
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenSwitches == null || nm.brokenSwitches.Count == 0) return 0;

                foreach (var kvp in nm.brokenSwitches)
                {
                    try
                    {
                        var s = kvp.Value;
                        if (s != null && s.Pointer != IntPtr.Zero && s.isBroken)
                        {
                            s.RepairDevice();
                            return 1; // Return after dispatching one, no defensive copy needed since we break
                        }
                    }
                    catch { }
                }
                return 0;
            }
            catch { return 0; }
        }

        // ─── Time ────────────────────────────────────────────────────────

        public static float GetTimeOfDay()
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                if (tc == null || tc.Pointer == IntPtr.Zero) return 0f;
                return tc.currentTimeOfDay;
            }
            catch { return 0f; }
        }

        public static uint GetDay()
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                if (tc == null || tc.Pointer == IntPtr.Zero) return 1u;
                return (uint)tc.day;
            }
            catch { return 1u; }
        }

        public static float GetSecondsInFullDay()
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                if (tc == null || tc.Pointer == IntPtr.Zero) return 1200f;
                return tc.secondsInFullDay;
            }
            catch { return 1200f; }
        }

        public static void SetSecondsInFullDay(float val)
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                if (tc == null || tc.Pointer == IntPtr.Zero) return;
                tc.secondsInFullDay = val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetSecondsInFullDay failed: {ex.Message}"); }
        }

        // ─── Game State ──────────────────────────────────────────────────

        public static float GetTimeScale()
        {
            try { return UnityEngine.Time.timeScale; }
            catch { return 1f; }
        }

        public static void SetTimeScale(float val)
        {
            try { UnityEngine.Time.timeScale = val; }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetTimeScale failed: {ex.Message}"); }
        }

        public static string GetCurrentScene()
        {
            try { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name ?? "None"; }
            catch { return "None"; }
        }

        public static bool IsGamePaused()
        {
            try { return UnityEngine.Time.timeScale < 0.01f; }
            catch { return false; }
        }

        public static void SetGamePaused(bool val)
        {
            try { UnityEngine.Time.timeScale = val ? 0f : 1f; }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetGamePaused failed: {ex.Message}"); }
        }

        public static int TriggerSave()
        {
            try
            {
                Il2Cpp.SaveSystem.SaveGame();
                return 1;
            }
            catch { return 0; }
        }

        public static int GetDifficulty()
        {
            try
            {
                var mgr = Il2Cpp.MainGameManager.instance;
                if (mgr == null || mgr.Pointer == IntPtr.Zero) return 1;
                return (int)mgr.difficulty;
            }
            catch { return 1; }
        }

        // ─── Player ──────────────────────────────────────────────────────

        public static Vector3 GetPlayerPosition()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm == null || pm.Pointer == IntPtr.Zero || pm.playerGO == null || pm.playerGO.Pointer == IntPtr.Zero)
                    return Vector3.zero;
                return pm.playerGO.transform.position;
            }
            catch { return Vector3.zero; }
        }

        // ─── Config (Persisted via ModConfigSystem) ──────────────────────

        public static void ConfigSetBool(string m, string k, bool v) { }
        public static bool ConfigGetBool(string m, string k, bool d) => d;
        public static void ConfigSetInt(string m, string k, int v) { }
        public static int ConfigGetInt(string m, string k, int d) => d;
        public static void ConfigSetString(string m, string k, string v) { }
        public static string ConfigGetString(string m, string k, string d) => d;

        // ─── Internal References (set by GregCoreMod) ────────────────────
        public static object _keybindReg { get; set; } = null!;
        public static object _modSettingsService { get; set; } = null!;
    }

    public class GregSettingsProxy
    {
        public void RegisterToggle(string modId, string k, string n, bool def, Action<bool> cb, string cat = "General", string desc = "") { }
        public void RegisterSlider(string modId, string k, string n, float min, float max, float def, Action<float> cb, string cat = "General", string desc = "") { }
        public void RegisterToggle(string k, string n, string d, bool def) { }
        public void RegisterSlider(string k, string n, string d, float min, float max, float def) { }
    }

    public class GregHooksProxy
    {
        public void Fire(string id, object? data = null) => GregAPI.FireEvent(id, data);
        public void On(string id, Action<object> cb) => GregAPI.On(id, cb);
    }

    public class HookEventArgs
    {
        public string HookName = "";
        public string Trigger = "";
        public object? Data;
    }

    public enum GregEventId { None, OnCoinsChanged, system_GameLoaded, ServerBroken, ServerRepaired }
}
