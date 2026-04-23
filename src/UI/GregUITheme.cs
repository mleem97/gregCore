using UnityEngine;

namespace gregCore.UI
{
    /// <summary>
    /// Central theme registry for the Luminescent Architect Design System.
    /// Standardized tokens for colors, spacing, and effects.
    /// </summary>
    public static class GregUITheme
    {
        // Colors
        public static readonly Color Background = new Color(0.00f, 0.07f, 0.07f, 0.93f);
        public static readonly Color Accent = new Color(0.38f, 0.96f, 0.85f, 1f);      // #61F4D8
        public static readonly Color Text = new Color(0.75f, 0.99f, 0.97f, 1f);        // #C0FCF6
        public static readonly Color Warning = new Color(0.93f, 0.25f, 0.27f, 1f);     // #ED4245
        public static readonly Color ButtonNormal = new Color(0.05f, 0.15f, 0.15f, 1f);
        public static readonly Color ButtonHover = new Color(0.10f, 0.25f, 0.25f, 1f);

        // Spacing
        public static readonly float Padding = 10f;
        public static readonly float HeaderHeight = 30f;
        public static readonly int DefaultFontSize = 14;
        public static readonly int HeaderFontSize = 16;

        /// <summary>
        /// Applies the theme colors to an Image component.
        /// </summary>
        public static void ApplyBackground(UnityEngine.UI.Image img) => img.color = Background;
        
        /// <summary>
        /// Applies the theme colors to a Text component.
        /// </summary>
        public static void ApplyText(UnityEngine.UI.Text txt, bool isHeader = false)
        {
            txt.color = Text;
            txt.fontSize = isHeader ? HeaderFontSize : DefaultFontSize;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        /// <summary>
        /// Applies the theme colors to a Button component.
        /// </summary>
        public static void ApplyButton(UnityEngine.UI.Button btn)
        {
            var colors = btn.colors;
            colors.normalColor = ButtonNormal;
            colors.highlightedColor = ButtonHover;
            colors.pressedColor = Accent;
            btn.colors = colors;
        }
    }
}
