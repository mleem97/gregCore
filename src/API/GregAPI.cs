using System;
using System.Collections.Generic;
using UnityEngine;
using greg.Sdk;
using gregCore.Infrastructure.UI;
using gregCore.Core.Models;

namespace gregCore.API
{
    public static class GregAPI
    {
        public static object RegisterMod(string id, string name, string version) => null!;
        public static GregSettingsProxy Settings { get; } = new GregSettingsProxy();
        public static GregHooksProxy Hooks { get; } = new GregHooksProxy();

        public static void On(string eventId, Action<object> callback)
        {
            switch (eventId)
            {
                case "OnCoinsChanged": gregNativeEventHooks.OnCoinsChanged += callback; break;
                case "OnXpChanged": gregNativeEventHooks.OnXpChanged += callback; break;
                case "OnReputationChanged": gregNativeEventHooks.OnReputationChanged += callback; break;
                default: break;
            }
        }

        public static void On(HookName hookName, Action<object> callback) => On(hookName.Full, callback);

        public static void Log(string msg, string type = "INFO") => GregDevConsole.Instance?.AddLog(msg, type);
        public static void LogInfo(string msg) => Log(msg, "INFO");
        public static void LogWarning(string msg) => Log(msg, "WARN");
        public static void LogError(string msg) => Log(msg, "ERROR");
        
        public static void ShowNotification(string msg) { } 
        public static void ShowNotification(string msg, float duration) { }
        
        public static void FireEvent(string id, object? data = null) { }
        public static void Subscribe(string id, Action<object> cb) => On(id, cb);

        public static double GetPlayerMoney() => 0.0;
        public static void SetPlayerMoney(double val) { }
        public static double GetPlayerXp() => 0.0;
        public static void SetPlayerXp(double val) { }
        public static double GetPlayerReputation() => 0.0;
        public static void SetPlayerReputation(double val) { }
        
        public static uint GetServerCount() => 0;
        public static uint GetRackCount() => 0;
        public static uint GetSwitchCount() => 0;
        public static uint GetBrokenServerCount() => 0;
        public static uint GetBrokenSwitchCount() => 0;
        public static uint GetFreeTechnicianCount() => 0;
        public static uint GetTotalTechnicianCount() => 0;
        
        public static int DispatchRepairServer() => 0;
        public static int DispatchRepairSwitch() => 0;
        
        public static float GetTimeOfDay() => 0f;
        public static uint GetDay() => 1;
        public static float GetSecondsInFullDay() => 1200f;
        public static void SetSecondsInFullDay(float val) { }
        public static float GetTimeScale() => 1f;
        public static void SetTimeScale(float val) { }
        
        public static string GetCurrentScene() => "None";
        public static bool IsGamePaused() => false;
        public static void SetGamePaused(bool val) { }
        public static int TriggerSave() => 0;
        public static int GetDifficulty() => 1;
        
        public static Vector3 GetPlayerPosition() => Vector3.zero;

        public static void ConfigSetBool(string m, string k, bool v) { }
        public static bool ConfigGetBool(string m, string k, bool d) => d;
        public static void ConfigSetInt(string m, string k, int v) { }
        public static int ConfigGetInt(string m, string k, int d) => d;
        public static void ConfigSetString(string m, string k, string v) { }
        public static string ConfigGetString(string m, string k, string d) => d;

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
        public void Fire(string id, object? data = null) { }
        public void On(string id, Action<object> cb) { }
    }

    public class HookEventArgs
    {
        public string HookName = "";
        public string Trigger = "";
        public object? Data;
    }

    public enum GregEventId { None, OnCoinsChanged, system_GameLoaded, ServerBroken, ServerRepaired }
}
