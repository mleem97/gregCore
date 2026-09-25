/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for tablets/widgets (GregUIBuilder behind handles).
///               Callbacks run as Lua closures (errors -> log, no crash).
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
        RegisterTabletOpen(greg, modId);
        RegisterWidgetOpen(greg, modId);
        RegisterPanelAddButton(greg, modId);
        RegisterPanelAddSlider(greg, modId);
        RegisterPanelAddSection(greg);
        RegisterPanelVisible(greg);
    }

    private static void RegisterTabletOpen(Table greg, string modId)
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
    }

    private static void RegisterWidgetOpen(Table greg, string modId)
    {

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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPanelAddButton(Table greg, string modId)
    {

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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPanelAddSlider(Table greg, string modId)
    {

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
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPanelAddSection(Table greg)
    {

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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPanelVisible(Table greg)
    {

        // greg.panel_visible(id) → bool
        greg["panel_visible"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var b = Lookup(id);
                return b != null && b.IsVisible;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });

        // greg.panel_close(id) → bool (hide + forget handle)
        greg["panel_close"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var b = Lookup(id);
                if (b == null) return false;
                try { b.IsVisible = false; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  /* ignored: hide best-effort */ }
                lock (_gate) { _panels.Remove(id ?? ""); }
                return true;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    /// <summary>
    /// Opens a tracked tablet for a computer app page. Returns the handle
    /// id ("" on failure). The handle works with all panel_add_* calls.
    /// </summary>
    internal static string OpenTabletForApp(string title)
    {
        try
        {
            var builder = gregCore.UI.GregUIBuilder.CreateTablet(title ?? "App");
            builder.Build();
            return Track(builder);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return ""; }
    }

    /// <summary>Hides and forgets a tablet handle (best-effort).</summary>
    internal static void CloseTablet(string handle)
    {
        try
        {
            var b = Lookup(handle);
            if (b == null) return;
            try { b.IsVisible = false; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            lock (_gate) { _panels.Remove(handle ?? ""); }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static string Track(gregCore.UI.GregUIBuilder builder)
    {
        try
        {
            string id = "pnl_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            lock (_gate) { _panels[id] = builder; }
            return id;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return ""; }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
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
