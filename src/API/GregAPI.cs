using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnityEngine;
using gregCore.PublicApi;
using gregCore.Core.Models;

namespace gregCore.API;

public enum GregEventId : uint
{
    MoneyChanged = 100, XpChanged = 101, ReputationChanged = 102,
    ServerPowered = 200, ServerBroken = 201, ServerRepaired = 202,
    ServerInstalled = 203, CableConnected = 204, CableDisconnected = 205,
    ServerCustomerChanged = 206, ServerAppChanged = 207, RackUnmounted = 208,
    SwitchBroken = 209, SwitchRepaired = 210, ObjectSpawned = 211,
    ObjectPickedUp = 212, ObjectDropped = 213,
    DayEnded = 300, MonthEnded = 301,
    CustomerAccepted = 400, CustomerSatisfied = 401, CustomerUnsatisfied = 402,
    ShopCheckout = 500, ShopItemAdded = 501, ShopCartCleared = 502, ShopItemRemoved = 503,
    EmployeeHired = 600, EmployeeFired = 601,
    GameSaved = 700, GameLoaded = 701, GameAutoSaved = 702,
    WallPurchased = 800,
    CustomEmployeeHired = 1000, CustomEmployeeFired = 1001,
}

public static class GregAPI
{
    private static readonly Dictionary<GregEventId, string> _eventIdToHook = new()
    {
        { GregEventId.MoneyChanged, "greg.economy.PlayerCoinUpdated" },
        { GregEventId.XpChanged, "greg.economy.PlayerXpUpdated" },
        { GregEventId.ReputationChanged, "greg.economy.PlayerReputationUpdated" },
        { GregEventId.ServerPowered, "greg.hardware.ServerPowered" },
        { GregEventId.ServerBroken, "greg.hardware.ServerBroken" },
        { GregEventId.ServerRepaired, "greg.hardware.ServerRepaired" },
        { GregEventId.ServerInstalled, "greg.hardware.ServerInstalled" },
        { GregEventId.DayEnded, "greg.lifecycle.DayEnded" },
        { GregEventId.MonthEnded, "greg.lifecycle.MonthEnded" },
        { GregEventId.GameLoaded, "greg.lifecycle.GameLoaded" },
        { GregEventId.GameSaved, "greg.lifecycle.GameSaved" },
    };

    private static readonly Dictionary<GregEventId, List<Action<ulong>>> _subscriptions = new();

    public static void Initialize()
    {
        LogInfo("GregAPI initializing...");
    }

    // internal DI container hooks for new services
    internal static gregCore.Infrastructure.Settings.GregKeybindRegistry _keybindReg = null!;
    internal static gregCore.Infrastructure.Settings.GregModSettingsService _modSettingsService = null!;
    private static gregCore.Sdk.IGregAPI? _sdkApi;

    public static gregCore.Sdk.IGregAPI GetSdkApi()
    {
        if (_sdkApi == null)
        {
            _sdkApi = gregCore.GameLayer.Bootstrap.GregServiceContainer.Get<gregCore.Sdk.IGregAPI>();
        }
        return _sdkApi ?? throw new Exception("SDK API not initialized");
    }

    public static void RegisterMod(string modId, string name, string version, object? apiObject = null)
    {
        GetSdkApi().RegisterMod(modId, name, version, apiObject);
    }

    public static class Settings
    {
        public static void RegisterToggle(string modId, string settingId, string displayName, bool defaultValue, Action<bool>? onChanged = null, string category = "General", string description = "")
        {
            GetSdkApi().RegisterToggle(modId, settingId, displayName, defaultValue, onChanged, category, description);
        }

        public static void RegisterSlider(string modId, string settingId, string displayName, float defaultValue, Action<float>? onChanged = null, string category = "General", string description = "")
        {
            GetSdkApi().RegisterSlider(modId, settingId, displayName, defaultValue, onChanged, category, description);
        }
    }

    public static class Input
    {
        public static void RegisterKeybind(string modId, string actionId, string displayName, KeyCode defaultKey, Action onPress, string category = "Controls", string description = "")
        {
            GetSdkApi().RegisterKeybind(modId, actionId, displayName, defaultKey, onPress, category, description);
        }
    }

    public static class Hooks
    {
        public static void On(string hookName, Action<gregCore.Sdk.Models.GregPayload> handler)
        {
            GetSdkApi().On(hookName, handler);
        }

        public static void Fire(string hookName, gregCore.Sdk.Models.GregPayload payload)
        {
            GetSdkApi().Fire(hookName, payload);
        }
    }

    // --- Economy ---
    public static double GetPlayerMoney() => gregCore.PublicApi.greg.Economy.GetBalance();
    public static void SetPlayerMoney(double amount) => gregCore.PublicApi.greg.Economy.SetBalance((float)amount);
    public static double GetPlayerXp() => gregCore.PublicApi.greg.Economy.GetXP();
    public static void SetPlayerXp(double amount) => gregCore.PublicApi.greg.Economy.AddXP((float)(amount - GetPlayerXp()));
    public static double GetPlayerReputation() => gregCore.PublicApi.greg.Economy.GetReputation();
    public static void SetPlayerReputation(double amount) => gregCore.PublicApi.greg.Economy.AddReputation((float)(amount - GetPlayerReputation()));

    // --- World ---
    public static uint GetServerCount() => (uint)gregCore.PublicApi.greg.Server.GetCount();
    public static uint GetRackCount() => (uint)gregCore.PublicApi.greg.Facility.GetRackCount();
    public static uint GetSwitchCount() => (uint)gregCore.PublicApi.greg.Network.GetSwitchCount();
    public static uint GetBrokenServerCount() => (uint)gregCore.PublicApi.greg.Server.GetBrokenCount();
    public static uint GetBrokenSwitchCount() => (uint)gregCore.PublicApi.greg.Network.GetBrokenSwitchCount();

    // --- Technicians ---
    public static uint GetFreeTechnicianCount() => (uint)gregCore.PublicApi.greg.Npc.GetFreeTechnicianCount();
    public static uint GetTotalTechnicianCount() => (uint)gregCore.PublicApi.greg.Npc.GetTotalTechnicianCount();
    public static int DispatchRepairServer() => gregCore.PublicApi.greg.Npc.DispatchRepairServer(null) ? 0 : -1;
    public static int DispatchRepairSwitch() => gregCore.PublicApi.greg.Npc.DispatchRepairSwitch(null) ? 0 : -1;

    // --- Time ---
    public static float GetTimeOfDay() => gregCore.PublicApi.greg.Time.GetTimeOfDay();
    public static uint GetDay() => (uint)gregCore.PublicApi.greg.Time.GetDay();
    public static float GetSecondsInFullDay() => gregCore.PublicApi.greg.Time.GetSecondsInFullDay();
    public static void SetSecondsInFullDay(float s) => gregCore.PublicApi.greg.Time.SetSecondsInFullDay(s);

    // --- Game ---
    public static string GetCurrentScene() => gregCore.PublicApi.greg.Save.GetCurrentScene();
    public static bool IsGamePaused() => gregCore.PublicApi.greg.Time.IsPaused();
    public static void SetGamePaused(bool paused) => gregCore.PublicApi.greg.Time.SetPaused(paused);
    public static float GetTimeScale() => gregCore.PublicApi.greg.Time.GetTimeScale();
    public static void SetTimeScale(float scale) => gregCore.PublicApi.greg.Time.SetTimeScale(scale);
    public static int TriggerSave() { gregCore.PublicApi.greg.Save.TriggerSave(); return 0; }
    public static int GetDifficulty() => gregCore.PublicApi.greg.Save.GetDifficulty();

    // --- Player ---
    public static (float x, float y, float z, float ry) GetPlayerPosition()
    {
        var pos = gregCore.PublicApi.greg.Player.GetPosition();
        var rot = gregCore.PublicApi.greg.Player.GetRotation();
        return (pos.x, pos.y, pos.z, rot.y);
    }

    // --- UI / Logging ---
    public static void ShowNotification(string message) => gregCore.PublicApi.greg.UI.ShowNotification(message);
    public static void LogInfo(string message) {
        greg.Logging.GregLogger.Msg(message, "API");
        gregCore.Infrastructure.UI.GregDevConsole.Instance.AddLog(message, LogType.Log);
    }
    public static void LogWarning(string message) {
        greg.Logging.GregLogger.Warn(message, "API");
        gregCore.Infrastructure.UI.GregDevConsole.Instance.AddLog(message, LogType.Warning);
    }
    public static void LogError(string message) {
        greg.Logging.GregLogger.Error(message, null, "API");
        gregCore.Infrastructure.UI.GregDevConsole.Instance.AddLog(message, LogType.Error);
    }
    public static void LogSuccess(string message) {
        greg.Logging.GregLogger.Msg(message, "API");
        gregCore.Infrastructure.UI.GregDevConsole.Instance.AddLog(message, LogType.Log);
    }

    // --- Events ---
    public static void FireEvent(GregEventId eventId, ulong data = 0)
    {
        if (gregCore.PublicApi.greg.IsInitialized && _eventIdToHook.TryGetValue(eventId, out string? hookName) && hookName != null)
        {
            var ctx = gregCore.PublicApi.greg._context;
            ctx?.EventBus.Publish(hookName, new EventPayload
            {
                HookName = hookName,
                Data = new Dictionary<string, object> { { "raw_data", data } }
            });
        }

        if (_subscriptions.TryGetValue(eventId, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try { handler(data); }
                catch (Exception ex) { LogError($"Error in Event-Handler for {eventId}: {ex.Message}"); }
            }
        }
    }

    public static void Subscribe(GregEventId eventId, Action<ulong> handler)
    {
        if (!_subscriptions.ContainsKey(eventId))
            _subscriptions[eventId] = new List<Action<ulong>>();

        _subscriptions[eventId].Add(handler);

        if (gregCore.PublicApi.greg.IsInitialized && _eventIdToHook.TryGetValue(eventId, out string? hookName) && hookName != null)
        {
            var ctx = gregCore.PublicApi.greg._context;
            ctx?.EventBus.Subscribe(hookName, payload => {
                ulong data = 0;
                if (payload.Data != null)
                {
                    if (payload.Data.TryGetValue("raw_data", out var d)) data = Convert.ToUInt64(d);
                    else if (payload.Data.TryGetValue("NewValue", out var nv)) data = Convert.ToUInt64(nv);
                }
                handler(data);
            });
        }
    }

    public static void Unsubscribe(GregEventId eventId, Action<ulong> handler)
    {
        if (_subscriptions.TryGetValue(eventId, out var handlers))
        {
            handlers.Remove(handler);
        }
    }

    // --- Config ---
    public static void ConfigSetBool(string modId, string key, bool value) => gregCore.PublicApi.greg.Save.Set($"{modId}.{key}", value);
    public static bool ConfigGetBool(string modId, string key, bool defaultValue = false) => gregCore.PublicApi.greg.Save.Get($"{modId}.{key}", defaultValue);
    public static void ConfigSetInt(string modId, string key, int value) => gregCore.PublicApi.greg.Save.Set($"{modId}.{key}", value);
    public static int ConfigGetInt(string modId, string key, int defaultValue = 0) => gregCore.PublicApi.greg.Save.Get($"{modId}.{key}", defaultValue);
    public static void ConfigSetFloat(string modId, string key, float value) => gregCore.PublicApi.greg.Save.Set($"{modId}.{key}", value);
    public static float ConfigGetFloat(string modId, string key, float defaultValue = 0f) => gregCore.PublicApi.greg.Save.Get($"{modId}.{key}", defaultValue);
    public static void ConfigSetString(string modId, string key, string value) => gregCore.PublicApi.greg.Save.Set($"{modId}.{key}", value);
    public static string ConfigGetString(string modId, string key, string defaultValue = "") => gregCore.PublicApi.greg.Save.Get($"{modId}.{key}", defaultValue);
}
