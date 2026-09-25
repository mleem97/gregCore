/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for computer shortcuts + apps (greg.computer).
///               Shortcuts inject buttons into the ComputerShop main screen;
///               apps open tablet pages built with the familiar panel_add_*
///               calls. Callbacks run as Lua closures (errors -> log).
/// Maintainer:  Registry: gregCore.UI.GregComputer. Tablets:
///              LuaTabletModule.OpenTabletForApp / CloseTablet.
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.UI;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaComputerModule
{
    private sealed class LuaApp
    {
        public string ModId = "";
        public Closure? OnOpen;
        public Closure? OnClose;
        public string Handle = "";
    }

    private static readonly Dictionary<string, LuaApp> _luaApps =
        new Dictionary<string, LuaApp>(StringComparer.Ordinal);
    private static readonly Dictionary<string, Action<string>> _openedHandlers =
        new Dictionary<string, Action<string>>(StringComparer.Ordinal);
    private static readonly Dictionary<string, Action<string>> _closedHandlers =
        new Dictionary<string, Action<string>>(StringComparer.Ordinal);
    private static readonly object _gate = new object();

    public static void Register(Table greg, Script script, string modId)
    {
        var t = new Table(script);
        RegisterShortcuts(t, script, modId);
        RegisterApps(t, script, modId);
        RegisterAppState(t, script, modId);
        RegisterListings(t, script);
        greg["computer"] = t;
    }

    private static void RegisterShortcuts(Table t, Script script, string modId)
    {
        // greg.computer.register_shortcut(id, label, fn_or_appid)
        // fn (Closure) runs on click; string opens the app with that id.
        t["register_shortcut"] = (Func<string, string, DynValue, bool>)((id, label, target) =>
        {
            try
            {
                return TryRegisterShortcut(modId, id, label, target);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] computer.register_shortcut failed: {ex.Message}");
                return false;
            }
        });

        // greg.computer.unregister_shortcut(id) -> bool
        t["unregister_shortcut"] = (Func<string, bool>)((id) =>
        {
            try { return GregComputer.UnregisterShortcut(modId, id ?? ""); }
            catch { return false; }
        });
    }

    private static bool TryRegisterShortcut(string modId, string id, string label, DynValue target)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(label)) return false;
            if (target.Type == DataType.Function)
            {
                var fn = target.Function;
                return GregComputer.RegisterShortcut(modId, id, label,
                    () => SafeCall(modId, fn));
            }
            if (target.Type == DataType.String && !string.IsNullOrWhiteSpace(target.String))
            {
                return GregComputer.RegisterShortcut(modId, id, label, null, target.String);
            }
            MelonLogger.Warning($"[LuaMod:{modId}] computer.register_shortcut needs a function or app id.");
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{modId}] computer.register_shortcut failed: {ex.Message}");
            return false;
        }
    }

    private static void RegisterApps(Table t, Script script, string modId)
    {
        // greg.computer.register_app(appId, title, on_open_fn[, on_close_fn]) -> bool
        // on_open_fn(handleId): build content with panel_add_* calls.
        t["register_app"] = (Func<string, string, Closure, DynValue, bool>)((appId, title, onOpen, onClose) =>
        {
            try
            {
                return TryRegisterApp(modId, appId, title, onOpen, onClose);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] computer.register_app failed: {ex.Message}");
                return false;
            }
        });

        // greg.computer.unregister_app(appId) -> bool
        t["unregister_app"] = (Func<string, bool>)((appId) =>
        {
            try
            {
                lock (_gate) { _luaApps.Remove(modId + "\u0000" + (appId ?? "")); }
                return GregComputer.UnregisterApp(modId, appId ?? "");
            }
            catch { return false; }
        });
    }

    private static bool TryRegisterApp(string modId, string appId, string title, Closure onOpen, DynValue onClose)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(appId) || onOpen == null) return false;
            Closure? onCloseFn = onClose.Type == DataType.Function ? onClose.Function : null;
            var entry = new LuaApp { ModId = modId, OnOpen = onOpen, OnClose = onCloseFn };
            lock (_gate) { _luaApps[modId + "\u0000" + appId] = entry; }
            EnsureHandlers(modId);
            return GregComputer.RegisterApp(modId, appId, title ?? appId,
                build: null, onClosed: null, framePage: false);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{modId}] computer.register_app failed: {ex.Message}");
            return false;
        }
    }

    private static void RegisterAppState(Table t, Script script, string modId)
    {
        // greg.computer.open_app(appId) -> bool
        t["open_app"] = (Func<string, bool>)((appId) =>
        {
            try { return GregComputer.TryOpenApp(appId ?? ""); }
            catch { return false; }
        });

        // greg.computer.close_app() -> bool (true when an app was open)
        t["close_app"] = (Func<bool>)(() =>
        {
            try
            {
                bool was = !string.IsNullOrEmpty(GregComputer.CurrentAppId);
                GregComputer.CloseApp();
                return was;
            }
            catch { return false; }
        });

        // greg.computer.current_app() -> string ("" when none)
        t["current_app"] = (Func<string>)(() =>
        {
            try { return GregComputer.CurrentAppId ?? ""; }
            catch { return ""; }
        });
    }

    private static void RegisterListings(Table t, Script script)
    {
        // greg.computer.list_shortcuts() -> array of {mod, id, label, app}
        t["list_shortcuts"] = (Func<Table>)(() => ListShortcuts(script));

        // greg.computer.list_apps() -> array of {mod, app, title}
        t["list_apps"] = (Func<Table>)(() => ListApps(script));
    }

    private static Table ListShortcuts(Script script)
    {
        var out_ = new Table(script);
        try
        {
            int i = 1;
            foreach (var s in GregComputer.Shortcuts())
            {
                if (s == null) continue;
                var row = new Table(script);
                row["mod"] = s.ModId; row["id"] = s.Id;
                row["label"] = s.Label; row["app"] = s.AppId;
                out_[i++] = row;
            }
        }
        catch { }
        return out_;
    }

    private static Table ListApps(Script script)
    {
        var out_ = new Table(script);
        try
        {
            int i = 1;
            foreach (var a in GregComputer.Apps())
            {
                if (a == null) continue;
                var row = new Table(script);
                row["mod"] = a.ModId; row["app"] = a.AppId; row["title"] = a.Title;
                out_[i++] = row;
            }
        }
        catch { }
        return out_;
    }

    public static void UnregisterAll(string modId)
    {
        try
        {
            lock (_gate)
            {
                CloseOwnedHandles(modId);
                RemoveOwnedEntries(modId);
                DetachHandlers(modId);
            }
            GregComputer.UnregisterAll(modId);
        }
        catch { }
    }

    private static void CloseOwnedHandles(string modId)
    {
        try
        {
            foreach (var k in new List<string>(_luaApps.Keys))
            {
                try
                {
                    if (_luaApps.TryGetValue(k, out var entry) && entry != null
                        && entry.ModId == modId && !string.IsNullOrEmpty(entry.Handle))
                    {
                        try { LuaTabletModule.CloseTablet(entry.Handle); } catch { }
                        entry.Handle = "";
                    }
                }
                catch { }
            }
        }
        catch { }
    }

    private static void RemoveOwnedEntries(string modId)
    {
        try
        {
            foreach (var k in new List<string>(_luaApps.Keys))
            {
                try
                {
                    if (k.StartsWith(modId + "\u0000", StringComparison.Ordinal))
                        _luaApps.Remove(k);
                }
                catch { }
            }
        }
        catch { }
    }

    private static void DetachHandlers(string modId)
    {
        try
        {
            if (_openedHandlers.TryGetValue(modId, out var opened) && opened != null)
            {
                try { GregComputer.AppOpened -= opened; } catch { }
                _openedHandlers.Remove(modId);
            }
        }
        catch { }
        try
        {
            if (_closedHandlers.TryGetValue(modId, out var closed) && closed != null)
            {
                try { GregComputer.AppClosed -= closed; } catch { }
                _closedHandlers.Remove(modId);
            }
        }
        catch { }
    }

    private static void EnsureHandlers(string modId)
    {
        lock (_gate)
        {
            if (_openedHandlers.ContainsKey(modId)) return;
            Action<string> opened = (appId) => OnAppOpened(modId, appId);
            Action<string> closed = (appId) => OnAppClosed(modId, appId);
            _openedHandlers[modId] = opened;
            _closedHandlers[modId] = closed;
            try { GregComputer.AppOpened += opened; } catch { }
            try { GregComputer.AppClosed += closed; } catch { }
        }
    }

    private static void OnAppOpened(string modId, string appId)
    {
        try
        {
            LuaApp? entry;
            lock (_gate) { _luaApps.TryGetValue(modId + "\u0000" + appId, out entry); }
            if (entry?.OnOpen == null) return;
            ClosePreviousHandle(entry);
            string handle = OpenHandleForApp(appId);
            entry.Handle = handle ?? "";
            if (string.IsNullOrEmpty(entry.Handle)) return;
            SafeCall(modId, entry.OnOpen, entry.Handle);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{modId}] computer app open failed: {ex.Message}");
        }
    }

    private static void ClosePreviousHandle(LuaApp entry)
    {
        try
        {
            if (!string.IsNullOrEmpty(entry.Handle))
            {
                try { LuaTabletModule.CloseTablet(entry.Handle); } catch { }
                entry.Handle = "";
            }
        }
        catch { }
    }

    private static string OpenHandleForApp(string appId)
    {
        try
        {
            string title = ResolveAppTitle(appId);
            return LuaTabletModule.OpenTabletForApp(title);
        }
        catch { return ""; }
    }

    private static string ResolveAppTitle(string appId)
    {
        try
        {
            if (GregComputer.TryGetApp(appId, out var app) && app != null
                && !string.IsNullOrWhiteSpace(app.Title)) return app.Title;
        }
        catch { }
        return appId;
    }

    private static void OnAppClosed(string modId, string appId)
    {
        try
        {
            LuaApp? entry;
            lock (_gate) { _luaApps.TryGetValue(modId + "\u0000" + appId, out entry); }
            if (entry != null && !string.IsNullOrEmpty(entry.Handle))
            {
                try { LuaTabletModule.CloseTablet(entry.Handle); } catch { }
                entry.Handle = "";
            }
            if (entry?.OnClose != null) SafeCall(modId, entry.OnClose, appId);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{modId}] computer app close failed: {ex.Message}");
        }
    }

    private static void SafeCall(string modId, Closure? fn, params object[] args)
    {
        try
        {
            if (fn == null) return;
            fn.Call(args);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{modId}] computer callback failed: {ex.Message}");
        }
    }
}
