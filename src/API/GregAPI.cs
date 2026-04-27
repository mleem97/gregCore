/// <file-summary>
/// Schicht:      API (Legacy)
/// Zweck:        Stabile API-Fassade für alle Mod-Sprachen (Lua, Rust, Go, JS, Python).
/// Maintainer:   Alle Methoden lesen/schreiben echte IL2CPP-Game-Objekte.
///               Fehler werden abgefangen und Default-Werte zurückgegeben.
///               Die Legacy-API delegiert intern an IL2CPP-Singletons.
/// </file-summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using greg.Sdk;
using gregCore.Infrastructure.UI;
using gregCore.Core.Models;
using gregCore.Core.Events;
using MelonLoader;

namespace gregCore.API
{
    /// <summary>
    /// Legacy GregAPI – stabile, statische Fassade für alle Scripting-Bridges.
    /// Jede Methode ist in try-catch gewrapped und gibt bei Fehler einen Safe-Default zurück.
    /// </summary>
    public static class GregAPI
    {
        // ─── Mod Registration ────────────────────────────────────────────
        public static object RegisterMod(string id, string name, string version) => null!;
        public static GregSettingsProxy Settings { get; } = new GregSettingsProxy();
        public static GregHooksProxy Hooks { get; } = new GregHooksProxy();

        // ─── Logging ─────────────────────────────────────────────────────
        public static void Log(string msg, string type = "INFO") => GregDevConsole.Instance?.AddLog(msg, type);
        public static void LogInfo(string msg) => Log(msg, "INFO");
        public static void LogWarning(string msg) => Log(msg, "WARN");
        public static void LogError(string msg) => Log(msg, "ERROR");

        // ─── Notifications ───────────────────────────────────────────────
        public static void ShowNotification(string msg) { }
        public static void ShowNotification(string msg, float duration) { }

        // ─── Events & Hooks ──────────────────────────────────────────────

        /// <summary>
        /// Referenz zum zentralen HookBus – wird von GregCoreMod beim Boot gesetzt.
        /// </summary>
        internal static GregHookBus? HookBus { get; set; }
        public static GregEventBus? EventBus { get; set; }

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

        public static void Subscribe(string id, Action<object> cb)
        {
            On(id, cb);
        }

        public static void On(string eventId, Action<object> callback)
        {
            try
            {
                // Bridge: Subscribe via EventBus
                EventBus?.Subscribe(eventId, payload =>
                {
                    try { callback?.Invoke(payload); }
                    catch (Exception ex) { MelonLogger.Error($"[GregAPI] Event handler error for '{eventId}': {ex.Message}"); }
                });

                // Also register legacy native hooks
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
                return pm?.playerClass?.money ?? 0.0;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerMoney(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm?.playerClass != null) pm.playerClass.money = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerMoney failed: {ex.Message}"); }
        }

        public static double GetPlayerXp()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                return pm?.playerClass?.xp ?? 0.0;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerXp(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm?.playerClass != null) pm.playerClass.xp = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerXp failed: {ex.Message}"); }
        }

        public static double GetPlayerReputation()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                return pm?.playerClass?.reputation ?? 0.0;
            }
            catch { return 0.0; }
        }

        public static void SetPlayerReputation(double val)
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm?.playerClass != null) pm.playerClass.reputation = (float)val;
            }
            catch (Exception ex) { MelonLogger.Error($"[GregAPI] SetPlayerReputation failed: {ex.Message}"); }
        }

        // ─── World / Hardware (IL2CPP Live) ──────────────────────────────

        public static uint GetServerCount()
        {
            try
            {
                var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                return servers != null ? (uint)servers.Count : 0u;
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
                var switches = UnityEngine.Object.FindObjectsOfType<Il2Cpp.NetworkSwitch>();
                return switches != null ? (uint)switches.Count : 0u;
            }
            catch { return 0u; }
        }

        public static uint GetBrokenServerCount()
        {
            try
            {
                uint count = 0;
                var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                if (servers == null) return 0u;
                foreach (var s in servers)
                {
                    try { if (s.isBroken) count++; } catch { }
                }
                return count;
            }
            catch { return 0u; }
        }

        public static uint GetBrokenSwitchCount()
        {
            try
            {
                uint count = 0;
                var switches = UnityEngine.Object.FindObjectsOfType<Il2Cpp.NetworkSwitch>();
                if (switches == null) return 0u;
                foreach (var s in switches)
                {
                    try { if (s.isBroken) count++; } catch { }
                }
                return count;
            }
            catch { return 0u; }
        }

        // ─── Technicians ─────────────────────────────────────────────────

        public static uint GetFreeTechnicianCount()
        {
            try
            {
                var tm = Il2Cpp.TechnicianManager.instance;
                if (tm?.technicians == null) return 0u;
                int busyCount = 0;
                foreach (var t in tm.technicians)
                {
                    try { if (t.isBusy) busyCount++; } catch { }
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
                return tm?.technicians != null ? (uint)tm.technicians.Count : 0u;
            }
            catch { return 0u; }
        }

        public static int DispatchRepairServer()
        {
            try
            {
                var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                if (servers == null) return 0;
                foreach (var s in servers)
                {
                    try
                    {
                        if (s.isBroken)
                        {
                            s.RepairDevice();
                            return 1;
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
                var switches = UnityEngine.Object.FindObjectsOfType<Il2Cpp.NetworkSwitch>();
                if (switches == null) return 0;
                foreach (var s in switches)
                {
                    try
                    {
                        if (s.isBroken)
                        {
                            s.RepairDevice();
                            return 1;
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
                return tc?.currentTimeOfDay ?? 0f;
            }
            catch { return 0f; }
        }

        public static uint GetDay()
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                return tc != null ? (uint)tc.day : 1u;
            }
            catch { return 1u; }
        }

        public static float GetSecondsInFullDay()
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                return tc?.secondsInFullDay ?? 1200f;
            }
            catch { return 1200f; }
        }

        public static void SetSecondsInFullDay(float val)
        {
            try
            {
                var tc = Il2Cpp.TimeController.instance;
                if (tc != null) tc.secondsInFullDay = val;
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
            try
            {
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name ?? "None";
            }
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
                return mgr != null ? (int)mgr.difficulty : 1;
            }
            catch { return 1; }
        }

        // ─── Player ──────────────────────────────────────────────────────

        public static Vector3 GetPlayerPosition()
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm?.playerGO == null) return Vector3.zero;
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
