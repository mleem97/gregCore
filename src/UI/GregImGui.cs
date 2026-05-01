using UnityEngine;
using MelonLoader;
using System.Collections.Generic;

namespace gregCore.UI
{
    /// <summary>
    /// Premium Native GregCore IMGUI styling engine.
    /// Replicates modern UI Toolkit aesthetics using procedural IMGUI textures.
    /// Supports dynamic font application and custom scrollbar styling.
    /// </summary>
    public static class GregImGui
    {
        public static GUIStyle stWindow = new();
        public static GUIStyle stHeader = new();
        public static GUIStyle stLabel = new();
        public static GUIStyle stLabelSmall = new();
        public static GUIStyle stButton = new();
        public static GUIStyle stButtonHover = new();
        public static GUIStyle stButtonActive = new();
        public static GUIStyle stToggle = new();
        public static GUIStyle stCheckbox = new();
        public static GUIStyle stSwitchBgOff = new();
        public static GUIStyle stSwitchBgOn = new();
        public static GUIStyle stSwitchKnob = new();
        public static GUIStyle stSidebar = new();
        public static GUIStyle stTab = new();
        public static GUIStyle stTabActive = new();
        
        // Custom Scrollbar Styles
        public static GUIStyle stScrollbarVTrack = new();
        public static GUIStyle stScrollbarVThumb = new();

        // Theme Colors (Fluent Dark Palette)
        public static readonly Color ColorBg = new Color(0.12f, 0.12f, 0.14f, 0.98f);
        public static readonly Color ColorSidebar = new Color(0.08f, 0.08f, 0.1f, 1f);
        public static readonly Color ColorAccent = new Color(0.38f, 0.65f, 0.95f, 1f);
        public static readonly Color ColorAccentDim = new Color(0.38f, 0.65f, 0.95f, 0.4f);
        public static readonly Color ColorText = new Color(0.95f, 0.95f, 0.97f, 1f);
        public static readonly Color ColorTextDim = new Color(0.65f, 0.65f, 0.7f, 1f);
        public static readonly Color ColorBorder = new Color(0.25f, 0.25f, 0.28f, 1f);
        public static readonly Color ColorButton = new Color(0.18f, 0.18f, 0.22f, 1f);
        public static readonly Color ColorButtonHover = new Color(0.24f, 0.24f, 0.3f, 1f);
        public static readonly Color ColorSwitchOff = new Color(0.25f, 0.25f, 0.27f, 1f);
        public static readonly Color ColorScrollTrack = new Color(0.05f, 0.05f, 0.06f, 0.5f);
        public static readonly Color ColorScrollThumb = new Color(0.3f, 0.3f, 0.35f, 1f);

        private static bool _initialized;
        private static readonly Dictionary<string, Texture2D> _texCache = new();
        private static Font? _gameFont;

        public static void EnsureInitialized()
        {
            if (_initialized && stWindow != null) 
            {
                ApplyDynamicFont();
                return;
            }

            // Generate Procedural Textures
            var texBg = GetTexture("win_bg", ColorBg);
            var texSidebar = GetTexture("sidebar_bg", ColorSidebar);
            var texBtn = GetTexture("btn_normal", ColorButton);
            var texBtnHover = GetTexture("btn_hover", ColorButtonHover);
            var texAccent = GetTexture("btn_active", ColorAccent);
            var texSwitchOff = GetTexture("switch_off", ColorSwitchOff);
            var texTabActive = GetTexture("tab_active_bg", new Color(0.38f, 0.65f, 0.95f, 0.15f));
            var texScrollTrack = GetTexture("scroll_track", ColorScrollTrack);
            var texScrollThumb = GetTexture("scroll_thumb", ColorScrollThumb);

            // Initialize Styles manually to avoid copy constructor issues in IL2CPP
            stWindow.normal.background = texBg;
            stWindow.padding = CreateRectOffset(0, 0, 0, 0);

            stSidebar.normal.background = texSidebar;
            stSidebar.padding = CreateRectOffset(0, 0, 0, 0);

            stHeader.normal.textColor = ColorAccent;
            stHeader.fontSize = 20;
            stHeader.fontStyle = FontStyle.Bold;
            stHeader.padding = CreateRectOffset(20, 20, 15, 10);

            stLabel.normal.textColor = ColorText;
            stLabel.fontSize = 15;
            stLabel.alignment = TextAnchor.MiddleLeft;
            stLabel.padding = CreateRectOffset(5, 5, 2, 2);

            stLabelSmall.normal.textColor = ColorTextDim;
            stLabelSmall.fontSize = 12;
            stLabelSmall.alignment = TextAnchor.MiddleLeft;
            stLabelSmall.padding = CreateRectOffset(5, 5, 2, 2);

            stButton.normal.background = texBtn;
            stButton.normal.textColor = ColorText;
            stButton.hover.background = texBtnHover;
            stButton.hover.textColor = Color.white;
            stButton.active.background = texAccent;
            stButton.alignment = TextAnchor.MiddleCenter;
            stButton.fontSize = 14;
            stButton.margin = CreateRectOffset(5, 5, 5, 5);
            stButton.padding = CreateRectOffset(10, 10, 8, 8);

            stButtonActive.normal.background = texAccent;
            stButtonActive.normal.textColor = Color.white;
            stButtonActive.alignment = TextAnchor.MiddleCenter;
            stButtonActive.fontSize = 14;
            stButtonActive.margin = CreateRectOffset(5, 5, 5, 5);
            stButtonActive.padding = CreateRectOffset(10, 10, 8, 8);

            stTab.normal.textColor = ColorTextDim;
            stTab.hover.textColor = Color.white;
            stTab.hover.background = GetTexture("tab_hover", new Color(1, 1, 1, 0.05f));
            stTab.fontSize = 14;
            stTab.alignment = TextAnchor.MiddleLeft;
            stTab.padding = CreateRectOffset(25, 10, 10, 10);

            stTabActive.normal.textColor = ColorAccent;
            stTabActive.normal.background = texTabActive;
            stTabActive.fontStyle = FontStyle.Bold;
            stTabActive.fontSize = 14;
            stTabActive.alignment = TextAnchor.MiddleLeft;
            stTabActive.padding = CreateRectOffset(25, 10, 10, 10);

            stToggle.normal.textColor = ColorText;
            stToggle.fontSize = 15;
            stToggle.padding = CreateRectOffset(30, 0, 0, 0);

            stSwitchBgOff.normal.background = texSwitchOff;
            stSwitchBgOn.normal.background = texAccent;
            stSwitchKnob.normal.background = GetTexture("knob", Color.white);
            
            stScrollbarVTrack.normal.background = texScrollTrack;
            stScrollbarVTrack.fixedWidth = 8;
            
            stScrollbarVThumb.normal.background = texScrollThumb;
            stScrollbarVThumb.fixedWidth = 8;

            ApplyDynamicFont();

            _initialized = true;
            MelonLoader.MelonLogger.Msg("[GregImGui] Native framework styles initialized (v2).");
        }

        private static void ApplyDynamicFont()
        {
            var font = GregFontLoader.GetUGUIFont();
            if (font != null && font != _gameFont)
            {
                _gameFont = font;
                if (stHeader != null) stHeader.font = font;
                if (stLabel != null) stLabel.font = font;
                if (stLabelSmall != null) stLabelSmall.font = font;
                if (stButton != null) stButton.font = font;
                if (stButtonActive != null) stButtonActive.font = font;
                if (stTab != null) stTab.font = font;
                if (stTabActive != null) stTabActive.font = font;
                if (stToggle != null) stToggle.font = font;
            }
        }

        private static Texture2D GetTexture(string id, Color col)
        {
            if (_texCache.TryGetValue(id, out var tex) && tex != null) return tex;
            
            var newTex = new Texture2D(1, 1);
            newTex.SetPixel(0, 0, col);
            newTex.Apply();
            newTex.hideFlags = HideFlags.HideAndDontSave;
            _texCache[id] = newTex;
            return newTex;
        }

        private static RectOffset CreateRectOffset(int l, int r, int t, int b)
        {
            var res = new RectOffset();
            res.left = l; res.right = r; res.top = t; res.bottom = b;
            return res;
        }

        public static void DrawWindowFrame(Rect rect, string title)
        {
            var shadowRect = new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2);
            DrawRect(shadowRect, ColorBorder);
            
            GUI.Box(rect, "", stWindow);
            
            var headerBarRect = new Rect(rect.x, rect.y, rect.width, 50);
            DrawRect(headerBarRect, new Color(0, 0, 0, 0.2f));
            GUI.Label(headerBarRect, "    " + title.ToUpper(), stHeader);

            DrawRect(new Rect(rect.x, rect.y, rect.width, 3), ColorAccent);
        }

        public static bool DrawSwitch(Rect rect, bool value)
        {
            var switchRect = new Rect(rect.x, rect.y + (rect.height - 20) / 2, 44, 20);
            GUI.Box(switchRect, "", value ? stSwitchBgOn : stSwitchBgOff);

            float targetX = value ? switchRect.x + 24 : switchRect.x + 2;
            var knobRect = new Rect(targetX, switchRect.y + 2, 16, 16);
            GUI.Box(knobRect, "", stSwitchKnob);

            if (Event.current.type == EventType.MouseDown && switchRect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return !value;
            }
            return value;
        }

        public static void DrawRect(Rect rect, Color col)
        {
            var oldCol = GUI.color;
            GUI.color = col;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = oldCol;
        }

        public static void DrawBox(Rect rect, string title) => DrawWindowFrame(rect, title);

        public static GUISkin GetCleanSkin()
        {
            var skin = UnityEngine.Object.Instantiate(GUI.skin);
            skin.verticalScrollbar = stScrollbarVTrack;
            skin.verticalScrollbarThumb = stScrollbarVThumb;
            return skin;
        }
    }
}
