using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    /// <summary>
    /// Manual click routing for UI Toolkit panels without EventSystem.
    /// Hit test via worldBound + mouse polling (InputSystem, proven).
    /// Mods register (element, action) pairs; real Toolkit callbacks
    /// take precedence by timestamp (no double firing).
    /// </summary>
    public static class GregClickRouter
    {
        public sealed class Clickable
        {
            public VisualElement Element { get; set; }
            public Action Action { get; set; }
        }

        // One click pass. Returns true if an action fired.
        // lastRealClickUtc: mod sets it on real callbacks; routing
        // stays silent for 500ms afterwards (dedup).
        public static bool RouteClicks(List<Clickable> clickables, ref DateTime lastRealClickUtc)
        {
            if (clickables == null || clickables.Count == 0) return false;
            try
            {
                var mouse = UnityEngine.InputSystem.Mouse.current;
                if (mouse == null) return false;
                if (!mouse.leftButton.wasPressedThisFrame) return false;
                try
                {
                    if ((DateTime.UtcNow - lastRealClickUtc).TotalMilliseconds < 500.0) return false;
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                Vector2 pos = mouse.position.ReadValue();
                pos.y = Screen.height - pos.y;
                for (int i = clickables.Count - 1; i >= 0; i--)
                {
                    var c = clickables[i];
                    if (c == null || c.Element == null || c.Action == null) continue;
                    try
                    {
                        if (!c.Element.visible) continue;
                        Rect b = c.Element.worldBound;
                        if (b.width <= 0f || b.height <= 0f) continue;
                        if (b.Contains(pos))
                        {
                            try { c.Action(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                            return true;
                        }
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return false;
        }

        public static DateTime MarkRealClick()
        {
            try { return DateTime.UtcNow; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return DateTime.UtcNow; }
        }
    }
}
