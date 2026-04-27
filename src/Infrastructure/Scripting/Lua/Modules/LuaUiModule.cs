/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für UI-Interaktion (Notifications).
/// Maintainer:   greg.ui.notify(), greg.ui.log()
///               Spätere Erweiterung: greg.ui.show_panel() für IMGUI-Panels.
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaUiModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var uiTable = new Table(script);

        // greg.ui.notify(message, duration?)
        uiTable["notify"] = (Action<string, double?>)((message, duration) =>
        {
            try
            {
                float dur = (float)(duration ?? 5.0);
                API.GregAPI.ShowNotification(message, dur);
                MelonLogger.Msg($"[LuaMod:{modId}] UI Notification: {message}");
            }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] ui.notify failed: {ex.Message}"); }
        });

        // greg.ui.log(message, type?) – Adds to DevConsole
        uiTable["log"] = (Action<string, string?>)((message, type) =>
        {
            try { API.GregAPI.Log(message, type ?? "INFO"); }
            catch { }
        });

        // greg.ui.log_info(message)
        uiTable["log_info"] = (Action<string>)((msg) => API.GregAPI.LogInfo($"[{modId}] {msg}"));

        // greg.ui.log_warning(message)
        uiTable["log_warning"] = (Action<string>)((msg) => API.GregAPI.LogWarning($"[{modId}] {msg}"));

        // greg.ui.log_error(message)
        uiTable["log_error"] = (Action<string>)((msg) => API.GregAPI.LogError($"[{modId}] {msg}"));

        greg["ui"] = uiTable;
    }
}
