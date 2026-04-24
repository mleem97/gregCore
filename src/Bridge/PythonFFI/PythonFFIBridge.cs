using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Python.Runtime;
using UnityEngine;
using gregCore.API;
using gregCore.Core.Models;

namespace gregCore.Bridge.PythonFFI;

public class PythonFFIBridge
{
    public void log_info(string msg) => GregAPI.LogInfo(msg);
    public void log_warning(string msg) => GregAPI.LogWarning(msg);
    public void log_error(string msg) => GregAPI.LogError(msg);

    public double get_player_money() => GregAPI.GetPlayerMoney();
    public void set_player_money(double val) => GregAPI.SetPlayerMoney(val);
    public double get_player_xp() => GregAPI.GetPlayerXp();
    public void set_player_xp(double val) => GregAPI.SetPlayerXp(val);

    public uint get_server_count() => GregAPI.GetServerCount();
    public uint get_broken_server_count() => GregAPI.GetBrokenServerCount();
    public int dispatch_repair_server() => GregAPI.DispatchRepairServer();

    public float get_time_of_day() => GregAPI.GetTimeOfDay();
    public uint get_day() => GregAPI.GetDay();
    public int trigger_save() => GregAPI.TriggerSave();
    
    public PyObject get_player_position()
    {
        var p = GregAPI.GetPlayerPosition();
        using (Py.GIL())
        {
            var dict = new PyDict();
            dict.SetItem("x", p.x.ToPython());
            dict.SetItem("y", p.y.ToPython());
            dict.SetItem("z", p.z.ToPython());
            return dict;
        }
    }

    public void subscribe_event(string eventId, PyObject callback)
    {
        GregAPI.Subscribe(eventId, data => {
            using (Py.GIL()) { callback.Invoke(); }
        });
    }

    public void on(string hookName, PyObject callback)
    {
        GregAPI.Hooks.On(hookName, (Action<object>)(payloadObj => {
            var payload = (gregCore.API.HookEventArgs)payloadObj;
            using (Py.GIL())
            {
                var dict = new PyDict();
                dict.SetItem("hook_name", payload.HookName.ToPython());
                dict.SetItem("trigger", payload.Trigger.ToPython());
                callback.Invoke(dict);
            }
        }));
    }

    public static void Initialize() { }
    public static void OnUpdate(float dt) { }
    public static void OnSceneLoaded(string sceneName) { }
    public static void Shutdown() { }
}
