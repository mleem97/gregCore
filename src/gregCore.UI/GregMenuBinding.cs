/// <file-summary>
/// Layer:      UI
/// Purpose:    One-call F1-hub wiring for toggle menus. Replaces the
///             repeated opener + closer + SetOpen block every mod
///             hand-rolls (see docs/modding/menu-binding.md).
///             Best-effort; never throws.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Thin registry wiring over live game UI; needs running game.")]
public static class GregMenuBinding
{
    /// <summary>
    /// Binds a toggle-style menu to the F1 hub: opener toggles and reports
    /// the live state, closer hides (toggles only if open) and reports
    /// closed. Equivalent to what mods previously wrote by hand.
    /// </summary>
    public static void BindToggle(string menuId, Action toggle, Func<bool> isOpen)
    {
        if (string.IsNullOrEmpty(menuId) || toggle == null) return;
        GregMenuRegistry.RegisterOpener(menuId, () =>
        {
            try { toggle(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            Report(menuId, SafeIsOpen(isOpen));
        });
        GregMenuRegistry.RegisterCloser(menuId, () =>
        {
            try { if (isOpen == null || SafeIsOpen(isOpen)) toggle(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            Report(menuId, false);
        });
    }

    /// <summary>
    /// Reports the live open state (e.g. from a hotkey toggle path).
    /// One line — replaces per-mod ReportOpenState methods.
    /// </summary>
    public static void Report(string menuId, bool open)
    {
        try { GregMenuRegistry.SetOpen(menuId, open); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static bool SafeIsOpen(Func<bool> isOpen)
    {
        try { return isOpen != null && isOpen(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }
}
