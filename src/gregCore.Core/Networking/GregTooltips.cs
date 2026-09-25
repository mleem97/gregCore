/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Vanilla tooltip bridge: show and
///               hide overlay/world tooltips (for mod UIs), drive interact tooltips.
///               All best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregTooltips
{
    public static global::Il2Cpp.Tooltip FindTooltip()
    {
        global::Il2Cpp.Tooltip found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Tooltip>();
            if (all == null) return;
            foreach (var t in all)
            {
                if (t == null) continue;
                try
                {
                    var go = t.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = t;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool ShowOverlay(string text, Vector3 position, int xOffset)
    {
        var tooltip = FindTooltip();
        if (tooltip == null || string.IsNullOrEmpty(text)) return false;
        try
        {
            _ = tooltip.gameObject; // liveness
            tooltip.ShowTooltipOverlayCanvas(text, position, xOffset);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShowTooltipOverlayCanvas failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ShowWorld(string text, RectTransform transform, Camera cam)
    {
        var tooltip = FindTooltip();
        if (tooltip == null || string.IsNullOrEmpty(text) || transform == null) return false;
        try
        {
            _ = tooltip.gameObject; // liveness
            tooltip.ShowTooltipWorldCanvas(text, transform, cam);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShowTooltipWorldCanvas failed: {Base(ex)}");
            return false;
        }
    }

    public static bool Hide()
    {
        var tooltip = FindTooltip();
        if (tooltip == null) return false;
        try
        {
            _ = tooltip.gameObject; // liveness
            tooltip.HideTooltip();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"HideTooltip failed: {Base(ex)}");
            return false;
        }
    }

    public static global::Il2Cpp.ToolTipInteract FindInteractTooltip()
    {
        global::Il2Cpp.ToolTipInteract found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.ToolTipInteract>();
            if (all == null) return;
            foreach (var t in all)
            {
                if (t == null) continue;
                try
                {
                    var go = t.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = t;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool ShowInteract(string text)
    {
        var tooltip = FindInteractTooltip();
        if (tooltip == null || string.IsNullOrEmpty(text)) return false;
        try
        {
            _ = tooltip.gameObject; // liveness
            tooltip.ShowTooltipForInteract(text, null);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShowTooltipForInteract failed: {Base(ex)}");
            return false;
        }
    }

    public static bool HideInteract()
    {
        var tooltip = FindInteractTooltip();
        if (tooltip == null) return false;
        try
        {
            _ = tooltip.gameObject; // liveness
            tooltip.HideTooltipForInteract();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"HideTooltipForInteract failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Tooltips: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Tooltips field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
