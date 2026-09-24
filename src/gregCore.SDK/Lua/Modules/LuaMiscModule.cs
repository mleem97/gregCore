/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Kleine, eigenständige Lua-APIs: Steam, Sprache, Numpad, Pause.
/// Maintainer:   greg.steam.*, greg.locale.*, greg.numpad.*, greg.pause.*
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaMiscModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var steam = new Table(script);

        // greg.steam.parse_lobby(connect) → number (0 when unparsable)
        steam["parse_lobby"] = (Func<string, double>)((connect) =>
        {
            try
            {
                if (string.IsNullOrEmpty(connect)) return 0.0;
                return (double)gregCore.Core.Networking.GregSteamLobby.ParseLobbyFromConnect(connect);
            }
            catch { return 0.0; }
        });
        greg["steam"] = steam;

        var locale = new Table(script);

        // greg.locale.text(uid, fallback) → string
        locale["text"] = (Func<int, string, string>)((uid, fallback) =>
        {
            try { return gregCore.Core.Networking.GregLocalisation.GetTextByID(uid, fallback ?? ""); }
            catch { return fallback ?? ""; }
        });

        // greg.locale.change(uid) → bool
        locale["change"] = (Func<int, bool>)((uid) =>
        {
            try { return gregCore.Core.Networking.GregLocalisation.ChangeLanguage(uid); }
            catch { return false; }
        });

        // greg.locale.current() → number
        locale["current"] = (Func<int>)(() =>
        {
            try { return gregCore.Core.Networking.GregLocalisation.GetLoadLanguageUID(); }
            catch { return 0; }
        });
        greg["locale"] = locale;

        var numpad = new Table(script);

        // greg.numpad.is_active() / written() / copied()
        numpad["is_active"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregNumpad.IsActive(); }
            catch { return false; }
        });
        numpad["written"] = (Func<string>)(() =>
        {
            try { return gregCore.Core.Networking.GregNumpad.GetWrittenNumber() ?? ""; }
            catch { return ""; }
        });
        numpad["copied"] = (Func<string>)(() =>
        {
            try { return gregCore.Core.Networking.GregNumpad.GetCopiedNumber() ?? ""; }
            catch { return ""; }
        });

        // greg.numpad.press(digit) / press_ok() / press_delete() → bool
        numpad["press"] = (Func<string, bool>)((digit) =>
        {
            try { return gregCore.Core.Networking.GregNumpad.PressNumber(digit ?? ""); }
            catch { return false; }
        });
        numpad["press_ok"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregNumpad.PressOK(); }
            catch { return false; }
        });
        numpad["press_delete"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregNumpad.PressDelete(); }
            catch { return false; }
        });
        greg["numpad"] = numpad;

        var pause = new Table(script);

        // greg.pause.is_paused() → bool
        pause["is_paused"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregPauseMenu.IsPaused(); }
            catch { return false; }
        });

        // greg.pause.on_open(fn) / on_close(fn) → bool (subscribe; no unsubscribe
        // handle by design — callbacks live as long as the mod)
        pause["on_open"] = (Func<Closure, bool>)((fn) =>
        {
            try
            {
                if (fn == null) return false;
                return gregCore.Core.Networking.GregPauseMenu.SubscribeOpen(() =>
                {
                    try { fn.Call(); }
                    catch (Exception ex)
                    {
                        LuaLog.Error($"[LuaMod:{modId}] pause.on_open callback failed: {ex.Message}");
                    }
                });
            }
            catch { return false; }
        });
        pause["on_close"] = (Func<Closure, bool>)((fn) =>
        {
            try
            {
                if (fn == null) return false;
                return gregCore.Core.Networking.GregPauseMenu.SubscribeClose(() =>
                {
                    try { fn.Call(); }
                    catch (Exception ex)
                    {
                        LuaLog.Error($"[LuaMod:{modId}] pause.on_close callback failed: {ex.Message}");
                    }
                });
            }
            catch { return false; }
        });
        greg["pause"] = pause;
    }
}
