/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for UI interaction (notifications).
/// Maintainer:   greg.ui.notify(), greg.ui.log()
///               Later extension: greg.ui.show_panel() for IMGUI panels.
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.UI;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaUiModule
{
    // Set by the main composition: hooks Lua config tabs into
    // the game settings hub. Without wiring, register_mod_config_tab
    // is silently ignored (no hard reference to the main project).
    public static Action<string, string, Action<GregUIBuilder>>? RegisterTabHandler { get; set; }

    public static void Register(Table greg, Script script, string modId)
    {
        var uiTable = new Table(script);
        RegisterNotify(uiTable, modId);
        RegisterRegisterModConfigTab(uiTable, script, modId);

        greg["ui"] = uiTable;
    }

    private static void RegisterNotify(Table t, string modId)
    {

        // greg.ui.notify(message, duration?)
        t["notify"] = (Action<string, double?>)((message, duration) =>
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
        t["log"] = (Action<string, string?>)((message, type) =>
        {
            try { API.GregAPI.Log(message, type ?? "INFO"); }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        });

        // greg.ui.log_info(message)
        t["log_info"] = (Action<string>)((msg) => API.GregAPI.LogInfo($"[{modId}] {msg}"));

        // greg.ui.log_warning(message)
        t["log_warning"] = (Action<string>)((msg) => API.GregAPI.LogWarning($"[{modId}] {msg}"));

        // greg.ui.log_error(message)
        t["log_error"] = (Action<string>)((msg) => API.GregAPI.LogError($"[{modId}] {msg}"));
    }

    private static void RegisterRegisterModConfigTab(Table t, Script script, string modId)
    {

        // greg.ui.register_mod_config_tab(tab_id, label, builder_fn)
        t["register_mod_config_tab"] = (Action<string, string, Closure>)((tabId, label, builderFn) =>
        {
            try
            {
                Action<GregUIBuilder> registerTab = builder =>
                {
                    try
                    {
                        builderFn.Call(DynValue.FromObject(script, builder));
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"[LuaMod:{modId}] ui.register_mod_config_tab callback failed: {ex.Message}");
                    }
                };

                if (RegisterTabHandler == null)
                {
                    MelonLogger.Warning($"[LuaMod:{modId}] ui.register_mod_config_tab ohne UI-Anbindung ignoriert.");
                    return;
                }
                RegisterTabHandler(tabId, label, registerTab);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] ui.register_mod_config_tab failed: {ex.Message}");
            }
        });
    }
}
