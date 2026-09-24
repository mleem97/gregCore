using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace gregCore.UI
{
    /// <summary>
    /// GregUI design foundation: modern minimalist, sky blue/atoll blue.
    /// Deep-navy surfaces, thin slate borders, light text, atoll CTA.
    /// </summary>
    public static class GregUITheme
    {
        // --- Color Palette (Sky/Atoll) ---
        public static readonly Color PrimaryAccent = ParseHex("#0AA2C0"); // Atoll blue (CTA)
        public static readonly Color PrimaryTextOnAccent = ParseHex("#06121F"); // Dark navy on atoll
        public static readonly Color SecondaryColor = ParseHex("#87CEEB"); // Sky blue (highlights)
        public static readonly Color TertiaryColor = new Color(1.00f, 0.85f, 0.28f); // Gold (prices, rewards)
        public static readonly Color NeutralBorder = new Color(0.14f, 0.17f, 0.22f, 0.80f); // Slate (borders, dividers)
        public static Color NeutralPalette => NeutralBorder;
        public static readonly Color BackgroundDark = new Color(0.06f, 0.08f, 0.11f, 0.96f); // Title bars, wells
        public static readonly Color SurfaceDark = new Color(0.08f, 0.10f, 0.13f, 0.98f); // Panel background
        public static Color PanelBackground => SurfaceDark;

        // --- Text ---
        public static readonly Color TextPrimary = new Color(0.92f, 0.94f, 0.96f);
        public static readonly Color TextDim = new Color(0.62f, 0.66f, 0.72f);

        // --- Status ---
        public static readonly Color Success = new Color(0.30f, 0.70f, 0.35f);
        public static readonly Color Warning = new Color(1.00f, 0.69f, 0.00f);
        public static readonly Color Danger = new Color(1.00f, 0.32f, 0.32f);

        // --- Layout & Geometry ---
        public static readonly float CornerRadius = 8f;
        public static readonly float Padding = 16f;
        public static readonly float Spacing = 12f;
        public static readonly float HeaderHeight = 40f;
        public static readonly float TitleBarHeight = 28f;
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
            label.style.color = isHeadline ? TextPrimary : new Color(0.88f, 0.88f, 0.88f);
            label.style.unityFontStyleAndWeight = isHeadline ? FontStyle.Bold : FontStyle.Normal;
        }

        public static void ApplyPrimaryButtonStyle(Button button)
        {
            button.style.backgroundColor = PrimaryAccent;
            button.style.color = PrimaryTextOnAccent;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = CornerRadius;
            button.style.borderTopRightRadius = CornerRadius;
            button.style.borderBottomLeftRadius = CornerRadius;
            button.style.borderBottomRightRadius = CornerRadius;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        public static void ApplySecondaryButtonStyle(Button button)
        {
            button.style.backgroundColor = new Color(0.11f, 0.13f, 0.17f);
            button.style.color = new Color(0.85f, 0.87f, 0.90f);
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = CornerRadius;
            button.style.borderTopRightRadius = CornerRadius;
            button.style.borderBottomLeftRadius = CornerRadius;
            button.style.borderBottomRightRadius = CornerRadius;
            button.style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }
}
