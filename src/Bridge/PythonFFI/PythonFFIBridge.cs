using System;
using System.Collections.Generic;
using System.IO;
using Python.Runtime;
using MelonLoader;
using gregCore.API;

namespace gregCore.Bridge.PythonFFI;

public static class PythonFFIBridge
{
    private static readonly List<PythonPlugin> _plugins = new();
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        GregAPI.LogInfo("PythonFFIBridge initializing...");
        
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string pythonDir = Path.Combine(gameRoot, "Plugins", "Python");
        if (!Directory.Exists(pythonDir)) Directory.CreateDirectory(pythonDir);

        try
        {
            Runtime.PythonDLL = GregAPI.ConfigGetString("gregCore", "PythonDLL", "python310.dll");
            
            PythonEngine.Initialize();
            _isInitialized = true;
            
            LoadPlugins(pythonDir);
        }
        catch (Exception ex)
        {
            GregAPI.LogError($"Failed to initialize Python engine: {ex.Message}");
        }
    }

    private static void LoadPlugins(string pythonDir)
    {
        foreach (string dir in Directory.GetDirectories(pythonDir))
        {
            string mainFile = Path.Combine(dir, "main.py");
            if (!File.Exists(mainFile)) continue;

            try
            {
                using (Py.GIL())
                {
                    string id = Path.GetFileName(dir);
                    var scope = Py.CreateScope();
                    
                    scope.Set("greg", new GregPythonApi());
                    
                    string code = File.ReadAllText(mainFile);
                    scope.Exec(code);

                    var plugin = new PythonPlugin
                    {
                        Id = id,
                        Scope = scope,
                        OnInit = GetMethod(scope, "on_init"),
                        OnUpdate = GetMethod(scope, "on_update"),
                        OnEvent = GetMethod(scope, "on_event"),
                        OnSceneLoaded = GetMethod(scope, "on_scene_loaded"),
                        OnShutdown = GetMethod(scope, "on_shutdown")
                    };

                    plugin.OnInit?.Invoke();
                    _plugins.Add(plugin);
                    GregAPI.LogInfo($"Python Plugin loaded: {id}");
                }
            }
            catch (Exception ex)
            {
                GregAPI.LogError($"Error loading Python Mod in {dir}: {ex.Message}");
            }
        }
    }

    public static void OnUpdate(float dt)
    {
        if (!_isInitialized) return;
        using (Py.GIL())
        {
            foreach (var p in _plugins) p.OnUpdate?.Invoke(dt.ToPython());
        }
    }

    public static void OnSceneLoaded(string name)
    {
        if (!_isInitialized) return;
        using (Py.GIL())
        {
            foreach (var p in _plugins) p.OnSceneLoaded?.Invoke(name.ToPython());
        }
    }

    public static void Shutdown()
    {
        if (!_isInitialized) return;
        using (Py.GIL())
        {
            foreach (var p in _plugins) p.OnShutdown?.Invoke();
        }
        PythonEngine.Shutdown();
    }

    private static PyObject? GetMethod(PyModule scope, string name)
    {
        if (scope.HasAttr(name))
        {
            var attr = scope.GetAttr(name);
            if (attr.IsCallable()) return attr;
        }
        return null;
    }

    class PythonPlugin
    {
        public string Id = "";
        public PyModule Scope = null!;
        public PyObject? OnInit, OnUpdate, OnEvent, OnSceneLoaded, OnShutdown;
    }
}

public class GregPythonApi
{
    public void log_info(string msg) => GregAPI.LogInfo(msg);
    public void log_warning(string msg) => GregAPI.LogWarning(msg);
    public void log_error(string msg) => GregAPI.LogError(msg);
    
    public double get_player_money() => GregAPI.GetPlayerMoney();
    public void set_player_money(double amount) => GregAPI.SetPlayerMoney(amount);
    public double get_player_xp() => GregAPI.GetPlayerXp();
    public void set_player_xp(double amount) => GregAPI.SetPlayerXp(amount);
    public double get_player_reputation() => GregAPI.GetPlayerReputation();
    public void set_player_reputation(double amount) => GregAPI.SetPlayerReputation(amount);
    
    public uint get_server_count() => GregAPI.GetServerCount();
    public uint get_rack_count() => GregAPI.GetRackCount();
    public uint get_switch_count() => GregAPI.GetSwitchCount();
    public uint get_broken_server_count() => GregAPI.GetBrokenServerCount();
    public uint get_broken_switch_count() => GregAPI.GetBrokenSwitchCount();
    
    public uint get_free_technician_count() => GregAPI.GetFreeTechnicianCount();
    public uint get_total_technician_count() => GregAPI.GetTotalTechnicianCount();
    public int dispatch_repair_server() => GregAPI.DispatchRepairServer();
    public int dispatch_repair_switch() => GregAPI.DispatchRepairSwitch();
    
    public float get_time_of_day() => GregAPI.GetTimeOfDay();
    public uint get_day() => GregAPI.GetDay();
    
    public string get_current_scene() => GregAPI.GetCurrentScene();
    public bool is_game_paused() => GregAPI.IsGamePaused();
    public void set_game_paused(bool val) => GregAPI.SetGamePaused(val);
    public float get_time_scale() => GregAPI.GetTimeScale();
    public void set_time_scale(float val) => GregAPI.SetTimeScale(val);
    public int trigger_save() => GregAPI.TriggerSave();
    
    public object get_player_position()
    {
        var p = GregAPI.GetPlayerPosition();
        return new { x = p.Item1, y = p.Item2, z = p.Item3, ry = p.Item4 };
    }
    
    public void show_notification(string text) => GregAPI.ShowNotification(text);
    
    public void subscribe_event(uint id, PyObject callback)
    {
        GregAPI.Subscribe((GregEventId)id, data => {
            using (Py.GIL()) { callback.Invoke(data.ToPython()); }
        });
    }
    
    public void fire_event(uint id, ulong data) => GregAPI.FireEvent((GregEventId)id, data);

    // Hook API (New)
    public void on(string hookName, PyObject callback)
    {
        GregAPI.Hooks.On(hookName, payload => {
            using (Py.GIL())
            {
                var dict = new PyDict();
                dict.SetItem("hook_name", payload.HookName.ToPython());
                dict.SetItem("trigger", payload.Trigger.ToPython());
                var dataDict = new PyDict();
                foreach (var kvp in payload.Data) dataDict.SetItem(kvp.Key, kvp.Value.ToPython());
                dict.SetItem("data", dataDict);
                callback.Invoke(dict);
            }
        });
    }

    public void fire(string hookName, PyObject dataDict)
    {
        var payload = new gregCore.Sdk.Models.GregPayload(hookName, "PythonMod");
        using (Py.GIL())
        {
            var dict = new PyDict(dataDict);
            foreach (var key in dict.Keys())
            {
                var k = key.ToString();
                var v = dict.GetItem(key)?.AsManagedObject(typeof(object));
                if (k != null && v != null)
                {
                    payload.Data[k] = v;
                }
            }
        }
        GregAPI.Hooks.Fire(hookName, payload);
    }
}
