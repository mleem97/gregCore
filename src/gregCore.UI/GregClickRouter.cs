using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    /// <summary>
    /// Manuelles Klick-Routing für UI-Toolkit-Panels ohne EventSystem.
    /// Hit-Test über worldBound + Maus-Polling (InputSystem, bewährt).
    /// Mods registrieren (Element, Aktion)-Paare; echte Toolkit-Callbacks
    /// haben per Zeitstempel Vorrang (kein Doppelfeuer).
    /// </summary>
    public static class GregClickRouter
    {
        public sealed class Clickable
        {
            public VisualElement Element;
            public Action Action;
        }

        // Ein Klick-Durchgang. Gibt true zurück wenn eine Aktion feuerte.
        // lastRealClickUtc: Mod setzt ihn bei echten Callbacks; Routing
        // schweigt 500ms danach (Dedup).
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
                catch { }
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
                            try { c.Action(); } catch { }
                            return true;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return false;
        }

        public static void MarkRealClick(ref DateTime lastRealClickUtc)
        {
            try { lastRealClickUtc = DateTime.UtcNow; } catch { }
        }
    }
}
