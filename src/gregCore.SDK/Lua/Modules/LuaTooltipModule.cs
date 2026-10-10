/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Tooltips/Overlays.
/// Maintainer:   greg.tooltip.overlay(), hide(), interact(), hide_interact()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaTooltipModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var tooltip = new Table(script);

        // greg.tooltip.overlay(text, x, y, z, xOffset?) → bool
        tooltip["overlay"] = (Func<string, double, double, double, int, bool>)((text, x, y, z, xoff) =>
        {
            try
            {
                return gregCore.Core.Networking.GregTooltips.ShowOverlay(
                    text ?? "", new UnityEngine.Vector3((float)x, (float)y, (float)z), xoff);
            }
            catch { return false; }
        });

        // greg.tooltip.hide() → bool
        tooltip["hide"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregTooltips.Hide(); }
            catch { return false; }
        });

        // greg.tooltip.interact(text) → bool
        tooltip["interact"] = (Func<string, bool>)((text) =>
        {
            try { return gregCore.Core.Networking.GregTooltips.ShowInteract(text ?? ""); }
            catch { return false; }
        });

        // greg.tooltip.hide_interact() → bool
        tooltip["hide_interact"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregTooltips.HideInteract(); }
            catch { return false; }
        });

        greg["tooltip"] = tooltip;
    }
}
