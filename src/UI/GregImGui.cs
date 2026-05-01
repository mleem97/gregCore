using UnityEngine;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// DEPRECATED: IMGUI is strictly forbidden in IL2CPP / Unity 6000.4+ environments.
    /// This class is kept for binary compatibility but all rendering methods are no-ops.
    /// Use GregPanelBuilder and UI Toolkit instead.
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
        public static GUIStyle stScrollbarVTrack = new();
        public static GUIStyle stScrollbarVThumb = new();

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

        private static bool _warned;

        public static void EnsureInitialized()
        {
            if (!_warned)
            {
                _warned = true;
                MelonLogger.Warning("[GregImGui] DEPRECATED: IMGUI is disabled. Use UI Toolkit (GregPanelBuilder).");
            }
        }

        public static void DrawWindowFrame(Rect rect, string title)
        {
            EnsureInitialized();
        }

        public static bool DrawSwitch(Rect rect, bool value)
        {
            EnsureInitialized();
            return value;
        }

        public static void DrawRect(Rect rect, Color col)
        {
            EnsureInitialized();
        }

        public static void DrawBox(Rect rect, string title) => DrawWindowFrame(rect, title);

        public static GUISkin GetCleanSkin()
        {
            EnsureInitialized();
            return GUI.skin;
        }
    }
}
