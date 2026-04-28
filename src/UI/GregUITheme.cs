using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace gregCore.UI
{
    public static class GregUITheme
    {
        // --- Color Palette (Design System Specs) ---
        public static readonly Color PrimaryAccent = ParseHex("#00bfa5");   // Striking teal (CTA)
        public static readonly Color SecondaryColor = ParseHex("#10eade"); // Bright cyan (Highlights)
        public static readonly Color TertiaryColor = ParseHex("#0ac4fd");  // Vivid blue (Badges)
        public static readonly Color NeutralBorder = ParseHex("#47817c"); // Desaturated teal (Borders)
        public static Color NeutralPalette => NeutralBorder;
        public static readonly Color BackgroundDark = new Color(0.07f, 0.07f, 0.07f, 0.96f); 
        public static readonly Color SurfaceDark = new Color(0.12f, 0.12f, 0.12f, 0.98f);    
        public static Color PanelBackground => SurfaceDark;

        // --- Layout & Geometry ---
        public static readonly float CornerRadius = 8f; 
        public static readonly float Padding = 16f;
        public static readonly float Spacing = 12f;
        public static readonly float HeaderHeight = 40f;
        public static readonly float BorderWidthTablet = 3f;
        public static readonly float BorderWidthWidget = 1.5f;
        public static readonly float BorderWidth = 2f;

        private static Color ParseHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
                return color;
            return Color.magenta;
        }

        public static void ApplyTextStyle(Label label, bool isHeadline = false)
        {
            label.style.fontSize = isHeadline ? 20 : 14;
            label.style.color = isHeadline ? SecondaryColor : new Color(0.88f, 0.88f, 0.88f);
            label.style.unityFontStyleAndWeight = isHeadline ? FontStyle.Bold : FontStyle.Normal;
        }

        public static void ApplyPrimaryButtonStyle(Button button)
        {
            button.style.backgroundColor = PrimaryAccent;
            button.style.color = Color.black;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = CornerRadius;
            button.style.borderTopRightRadius = CornerRadius;
            button.style.borderBottomLeftRadius = CornerRadius;
            button.style.borderBottomRightRadius = CornerRadius;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        public static void ApplySecondaryButtonStyle(Button button)
        {
            button.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            button.style.color = NeutralBorder;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = CornerRadius;
            button.style.borderTopRightRadius = CornerRadius;
            button.style.borderBottomLeftRadius = CornerRadius;
            button.style.borderBottomRightRadius = CornerRadius;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }
}
