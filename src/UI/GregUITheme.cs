using UnityEngine;

namespace greg.Core.UI;

/// <summary>
/// Maps "The Luminescent Architect" design tokens to Unity Color/Font objects.
/// All values from the gregCore Design System Specification.
/// </summary>
public static class GregUITheme
{
    // ── Surface Hierarchy ──────────────────────────────────────────
    public static readonly Color Surface              = HexColor("#001110");
    public static readonly Color SurfaceContainerLow  = HexColor("#001715");
    public static readonly Color SurfaceContainer     = HexColor("#001E1C");
    public static readonly Color SurfaceContainerHigh = HexColor("#002422");
    public static readonly Color SurfaceContainerHighest = HexColor("#002B29");

    // ── Primary ────────────────────────────────────────────────────
    public static readonly Color Primary          = HexColor("#61F4D8");
    public static readonly Color PrimaryContainer = HexColor("#08C1A6");
    public static readonly Color OnPrimary        = HexColor("#001110");

    // ── Glassmorphism ──────────────────────────────────────────────
    public static readonly Color GlassBackground = new Color(0.0f, 0.12f, 0.11f, 0.80f); // surface-container at 80%
    public const float GlassBlur = 12f;

    // ── Secondary ──────────────────────────────────────────────────
    public static readonly Color Secondary   = HexColor("#1CEDE1");
    public static readonly Color Tertiary    = HexColor("#64D0FF");
    public static readonly Color OnSurface   = HexColor("#C0FCF6");
    public static readonly Color OnSurfaceVariant = HexColor("#80B0AB");

    // ── Error ──────────────────────────────────────────────────────
    public static readonly Color Error    = HexColor("#ED4245");
    public static readonly Color ErrorDim = HexColor("#D7383B");

    // ── Ambient Glow (for box shadows / outlines) ──────────────────
    public static readonly Color PrimaryGlow = new Color(0.38f, 0.96f, 0.85f, 0.10f);

    // ── Ghost Border ───────────────────────────────────────────────
    public static readonly Color GhostBorder = new Color(0.38f, 0.96f, 0.85f, 0.15f);

    // ── Spacing Scale ──────────────────────────────────────────────
    public const float SpaceXS  = 4f;
    public const float SpaceSM  = 8f;
    public const float SpaceMD  = 16f;
    public const float SpaceLG  = 24f;
    public const float SpaceXL  = 32f;
    public const float Space2XL = 48f;

    // ── Corner Radius ──────────────────────────────────────────────
    public const float RadiusSM = 4f;
    public const float RadiusMD = 8f;
    public const float RadiusLG = 16f;

    private static Color HexColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var c))
            return c;
        return Color.white;
    }
}
