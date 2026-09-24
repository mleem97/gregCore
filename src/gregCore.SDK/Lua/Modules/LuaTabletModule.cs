/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Tablets/Widgets (GregUIBuilder hinter Handles).
///               Callbacks laufen als Lua-Closures (Fehler -> Log, kein Crash).
/// Maintainer:   greg.tablet_* / greg.widget_* (open, add_*, toggle, visible, close)
/// </file-summary>

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaTabletModule
{
    private static readonly Dictionary<string, gregCore.UI.GregUIBuilder> _panels =
        new Dictionary<string, gregCore.UI.GregUIBuilder>(StringComparer.Ordinal);
    private static readonly object _gate = new object();

    public static void Register(Table greg, Script script, string modId)
    {
        // greg.tablet_open(title) → handle id ("" on failure)
        greg["tablet_open"] = (Func<string, string>)((title) =>
        {
            try
            {
                var builder = gregCore.UI.GregUIBuilder.CreateTablet(title ?? "Tablet");
                builder.Build();
                return Track(builder);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tablet_open() failed: {ex.Message}");
                return "";
            }
        });

        // greg.widget_open(title, x?, y?) → handle id ("" on failure)
        greg["widget_open"] = (Func<string, double, double, string>)((title, x, y) =>
        {
            try
            {
                var builder = gregCore.UI.GregUIBuilder.CreateWidget(
                    title ?? "Widget", (float)x, (float)y);
                builder.Build();
                return Track(builder);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] widget_open() failed: {ex.Message}");
                return "";
            }
        });

        // greg.panel_add_label(id, text) → bool
        greg["panel_add_label"] = (Func<string, string, bool>)((id, text) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                b.AddLabel(text ?? "");
                return true;
            }
            catch { return false; }
        });

        // greg.panel_add_button(id, label, fn) → bool
        greg["panel_add_button"] = (Func<string, string, Closure, bool>)((id, label, fn) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null || fn == null) return false;
                b.AddButton(label ?? "", () => SafeCall(modId, fn));
                return true;
            }
            catch { return false; }
        });

        // greg.panel_add_toggle(id, label, value, fn) → bool (fn receives bool)
        greg["panel_add_toggle"] = (Func<string, string, bool, Closure, bool>)((id, label, value, fn) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                b.AddToggle(label ?? "", value, (v) => SafeCall(modId, fn, v));
                return true;
            }
            catch { return false; }
        });

        // greg.panel_add_slider(id, label, min, max, value, fn) → bool (fn receives number)
        greg["panel_add_slider"] = (Func<string, string, double, double, double, Closure, bool>)(
            (id, label, min, max, value, fn) =>
            {
                try
                {
                    var b = Lookup(id);
                    if (b == null) return false;
                    b.AddSlider(label ?? "", (float)min, (float)max, (float)value,
                        (v) => SafeCall(modId, fn, (double)v));
                    return true;
                }
                catch { return false; }
            });

        // greg.panel_add_spacer(id, height?) → bool
        greg["panel_add_spacer"] = (Func<string, double, bool>)((id, height) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                b.AddSpacer((float)height);
                return true;
            }
            catch { return false; }
        });

        // greg.panel_add_section(id, title) → bool
        greg["panel_add_section"] = (Func<string, string, bool>)((id, title) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                b.AddSection(title ?? "");
                return true;
            }
            catch { return false; }
        });

        // greg.panel_toggle(id) → bool (new visibility)
        greg["panel_toggle"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                b.Toggle();
                return b.IsVisible;
            }
            catch { return false; }
        });

        // greg.panel_visible(id) → bool
        greg["panel_visible"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var b = Lookup(id);
                return b != null && b.IsVisible;
            }
            catch { return false; }
        });

        // greg.panel_close(id) → bool (hide + forget handle)
        greg["panel_close"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                try { b.IsVisible = false; } catch { /* ignored: hide best-effort */ }
                lock (_gate) { _panels.Remove(id ?? ""); }
                return true;
            }
            catch { return false; }
        });
    }

    private static string Track(gregCore.UI.GregUIBuilder builder)
    {
        try
        {
            string id = "pnl_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            lock (_gate) { _panels[id] = builder; }
            return id;
        }
        catch { return ""; }
    }

    private static gregCore.UI.GregUIBuilder Lookup(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) return null;
            lock (_gate)
            {
                gregCore.UI.GregUIBuilder builder;
                return _panels.TryGetValue(id, out builder) ? builder : null;
            }
        }
        catch { return null; }
    }

    private static void SafeCall(string modId, Closure fn, params object[] args)
    {
        try
        {
            if (fn == null) return;
            fn.Call(args);
        }
        catch (Exception ex)
        {
            LuaLog.Error($"[LuaMod:{modId}] panel callback failed: {ex.Message}");
        }
    }
}
