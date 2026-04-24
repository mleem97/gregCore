using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.API;

namespace gregCore.Bridge.LuaFFI;

public static class LuaFFIBridge
{
    private static readonly List<LuaPlugin> _plugins = new();

    public static void Initialize()
    {
        GregAPI.LogInfo("LuaFFIBridge initializing...");
        
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string luaDir = Path.Combine(gameRoot, "Plugins", "Lua");
        
        if (!Directory.Exists(luaDir)) Directory.CreateDirectory(luaDir);
        
        LoadPlugins(luaDir);
    }

    private static void LoadPlugins(string luaDir)
    {
        foreach (string dir in Directory.GetDirectories(luaDir))
        {
            string mainFile = Path.Combine(dir, "main.lua");
            if (!File.Exists(mainFile)) continue;

            try
            {
                string id = Path.GetFileName(dir);
                var script = new Script(CoreModules.Preset_SoftSandbox);
                var gregTable = new Table(script);
                
                RegisterApi(gregTable, script);
                script.Globals["greg"] = gregTable;

                script.DoFile(mainFile);

                var plugin = new LuaPlugin
                {
                    Id = id,
                    Script = script,
                    OnInit = script.Globals.Get("on_init").Type == DataType.Function ? script.Globals.Get("on_init").Function : null,
                    OnUpdate = script.Globals.Get("on_update").Type == DataType.Function ? script.Globals.Get("on_update").Function : null,
                    OnEvent = script.Globals.Get("on_event").Type == DataType.Function ? script.Globals.Get("on_event").Function : null,
                    OnSceneLoaded = script.Globals.Get("on_scene_loaded").Type == DataType.Function ? script.Globals.Get("on_scene_loaded").Function : null,
                    OnShutdown = script.Globals.Get("on_shutdown").Type == DataType.Function ? script.Globals.Get("on_shutdown").Function : null
                };

                SafeCall(plugin, plugin.OnInit);
                _plugins.Add(plugin);
                GregAPI.LogInfo($"Lua Plugin loaded: {id}");
            }
            catch (Exception ex)
            {
                GregAPI.LogError($"Error loading Lua Mod in {dir}: {ex.Message}");
            }
        }
    }

    private static void RegisterApi(Table greg, Script script)
    {
        // Logging
        greg["log_info"] = (Action<string>)GregAPI.LogInfo;
        greg["log_warning"] = (Action<string>)GregAPI.LogWarning;
        greg["log_error"] = (Action<string>)GregAPI.LogError;

        // Economy
        greg["get_player_money"] = (Func<double>)GregAPI.GetPlayerMoney;
        greg["set_player_money"] = (Action<double>)GregAPI.SetPlayerMoney;
        greg["get_player_xp"] = (Func<double>)GregAPI.GetPlayerXp;
        greg["set_player_xp"] = (Action<double>)GregAPI.SetPlayerXp;
        greg["get_player_reputation"] = (Func<double>)GregAPI.GetPlayerReputation;
        greg["set_player_reputation"] = (Action<double>)GregAPI.SetPlayerReputation;

        // World
        greg["get_server_count"] = (Func<uint>)GregAPI.GetServerCount;
        greg["get_rack_count"] = (Func<uint>)GregAPI.GetRackCount;
        greg["get_switch_count"] = (Func<uint>)GregAPI.GetSwitchCount;
        greg["get_broken_server_count"] = (Func<uint>)GregAPI.GetBrokenServerCount;
        greg["get_broken_switch_count"] = (Func<uint>)GregAPI.GetBrokenSwitchCount;

        // Technicians
        greg["get_free_technician_count"] = (Func<uint>)GregAPI.GetFreeTechnicianCount;
        greg["get_total_technician_count"] = (Func<uint>)GregAPI.GetTotalTechnicianCount;
        greg["dispatch_repair_server"] = (Func<int>)GregAPI.DispatchRepairServer;
        greg["dispatch_repair_switch"] = (Func<int>)GregAPI.DispatchRepairSwitch;

        // Time
        greg["get_time_of_day"] = (Func<float>)GregAPI.GetTimeOfDay;
        greg["get_day"] = (Func<uint>)GregAPI.GetDay;
        greg["get_seconds_in_full_day"] = (Func<float>)GregAPI.GetSecondsInFullDay;
        greg["set_seconds_in_full_day"] = (Action<float>)GregAPI.SetSecondsInFullDay;

        // Game
        greg["get_current_scene"] = (Func<string>)GregAPI.GetCurrentScene;
        greg["is_game_paused"] = (Func<bool>)GregAPI.IsGamePaused;
        greg["set_game_paused"] = (Action<bool>)GregAPI.SetGamePaused;
        greg["get_time_scale"] = (Func<float>)GregAPI.GetTimeScale;
        greg["set_time_scale"] = (Action<float>)GregAPI.SetTimeScale;
        greg["trigger_save"] = (Func<int>)GregAPI.TriggerSave;
        greg["get_difficulty"] = (Func<int>)GregAPI.GetDifficulty;

        // Player
        greg["get_player_position"] = (Func<Table>)(() => {
            var p = GregAPI.GetPlayerPosition();
            var t = new Table(script);
            t["x"] = p.x; t["y"] = p.y; t["z"] = p.z; t["ry"] = p.y;
            return t;
        });

        // UI
        greg["show_notification"] = (Action<string>)GregAPI.ShowNotification;

        // Events
        greg["subscribe_event"] = (Action<uint, Closure>)((id, callback) => {
            GregAPI.Subscribe(((GregEventId)id).ToString(), data => callback.Call(data));
        });
        greg["fire_event"] = (Action<uint, ulong>)((id, data) => GregAPI.FireEvent(((GregEventId)id).ToString(), data));

        // Hook API (New)
        greg["on"] = (Action<string, Closure>)((hookName, callback) => {
            GregAPI.Hooks.On(hookName, payloadObj => {
                var payload = (gregCore.Sdk.Models.GregPayload)payloadObj;
                var table = new Table(script);
                table["hook_name"] = payload.HookName;
                table["trigger"] = payload.Trigger;
                var dataTable = new Table(script);
                foreach (var kvp in payload.Data) dataTable[kvp.Key] = kvp.Value;
                table["data"] = dataTable;
                callback.Call(table);
            });
        });

        greg["fire"] = (Action<string, Table>)((hookName, dataTable) => {
            var payload = new gregCore.Sdk.Models.GregPayload(hookName, "LuaMod");
            foreach (var pair in dataTable.Pairs)
            {
                payload.Data[pair.Key.String] = pair.Value.ToObject();
            }
            GregAPI.Hooks.Fire(hookName, payload);
        });

        // Config
        greg["config_set_bool"] = (Action<string, string, bool>)GregAPI.ConfigSetBool;
        greg["config_get_bool"] = (Func<string, string, bool, bool>)GregAPI.ConfigGetBool;
        greg["config_set_int"] = (Action<string, string, int>)GregAPI.ConfigSetInt;
        greg["config_get_int"] = (Func<string, string, int, int>)GregAPI.ConfigGetInt;
        greg["config_set_string"] = (Action<string, string, string>)GregAPI.ConfigSetString;
        greg["config_get_string"] = (Func<string, string, string, string>)GregAPI.ConfigGetString;
    }

    public static void OnUpdate(float dt)
    {
        foreach (var p in _plugins) SafeCall(p, p.OnUpdate, dt);
    }

    public static void OnSceneLoaded(string name)
    {
        foreach (var p in _plugins) SafeCall(p, p.OnSceneLoaded, name);
    }

    public static void Shutdown()
    {
        foreach (var p in _plugins) SafeCall(p, p.OnShutdown);
        _plugins.Clear();
    }

    private static void SafeCall(LuaPlugin p, Closure? func, params object[] args)
    {
        if (func == null) return;
        try
        {
            func.Call(args);
        }
        catch (Exception ex)
        {
            GregAPI.LogError($"[LuaMod:{p.Id}] error: {ex.Message}");
        }
    }

    class LuaPlugin
    {
        public string Id = "";
        public Script Script = null!;
        public Closure? OnInit, OnUpdate, OnEvent, OnSceneLoaded, OnShutdown;
    }
}
