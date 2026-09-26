using System.Collections.Generic;
using UnityEngine;

namespace gregCore.UI
{
    /// <summary>
    /// Canonical IMGUI theme (sky blue/atoll blue, modern minimal).
    /// Counterpart to GregUITheme (UI Toolkit) for IMGUI mods.
    /// Dummy-safe factories (parameterless ctors + field assignment).
    /// Mod-local mirrors (ModImGuiTheme.cs) are standalone fallbacks
    /// without DLL dependency and are kept in sync from here.
    /// </summary>
    public static class GregImGuiTheme
    {
        public static readonly Color PanelBg = new Color(0.08f, 0.10f, 0.13f, 0.98f);
        public static readonly Color TitleBg = new Color(0.06f, 0.08f, 0.11f, 0.90f);
        public static readonly Color Border = new Color(0.14f, 0.17f, 0.22f, 0.80f);
        public static readonly Color Text = new Color(0.92f, 0.94f, 0.96f);
        public static readonly Color Dim = new Color(0.62f, 0.66f, 0.72f);
        public static readonly Color Atoll = new Color(0.04f, 0.64f, 0.75f);
        public static readonly Color AtollText = new Color(0.02f, 0.07f, 0.12f);
        public static readonly Color Sky = new Color(0.53f, 0.81f, 0.92f);
        public static readonly Color Gold = new Color(1.00f, 0.85f, 0.28f);
        public static readonly Color Danger = new Color(1.00f, 0.32f, 0.32f);
        public static readonly Color Success = new Color(0.30f, 0.70f, 0.35f);

        public static GUIStyle Box()
        {
            return Box(null);
        }

        public static GUIStyle Box(Texture2D bg)
        {
            var s = new GUIStyle();
            s.normal.background = bg;
            s.normal.textColor = Text;
            s.border = Inset(8);
            s.padding = Inset(12);
            return s;
        }

        public static GUIStyle Label()
        {
            return Label(13);
        }

        public static GUIStyle Label(int size)
        {
            return Label(size, false);
        }

        public static GUIStyle Label(int size, bool bold)
        {
            return Label(size, bold, null);
        }

        public static GUIStyle Label(int size, bool bold, Color? color)
        {
            var s = new GUIStyle();
            s.fontSize = size;
            s.normal.textColor = color ?? Text;
            try { s.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return s;
        }

        public static GUIStyle Button()
        {
            return Button(false);
        }

        public static GUIStyle Button(bool primary)
        {
            var s = new GUIStyle();
            s.fontSize = 13;
            try { s.fontStyle = FontStyle.Bold; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            s.alignment = TextAnchor.MiddleCenter;
            s.border = Inset(6);
            s.padding = Inset(8);
            if (primary)
            {
                s.normal.background = Tex(Atoll);
                s.normal.textColor = AtollText;
                s.hover.background = Tex(Sky);
                s.hover.textColor = AtollText;
            }
            else
            {
                s.normal.background = Tex(new Color(0.11f, 0.13f, 0.17f));
                s.normal.textColor = Text;
                s.hover.background = Tex(new Color(0.16f, 0.19f, 0.25f));
                s.hover.textColor = Color.white;
            }
            return s;
        }

        public static RectOffset Inset(int v)
        {
            var r = new RectOffset();
            r.left = v; r.right = v; r.top = v; r.bottom = v;
            return r;
        }

        private static readonly Dictionary<Color, Texture2D> _texCache =
            new Dictionary<Color, Texture2D>();

        public static Texture2D Tex(Color c)
        {
            Texture2D t;
            if (_texCache.TryGetValue(c, out t) && t != null) return t;
            t = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            try
            {
                t.SetPixel(0, 0, c);
                t.Apply();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { UnityEngine.Object.DontDestroyOnLoad(t); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            _texCache[c] = t;
            return t;
        }
    }
}
