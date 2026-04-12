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

    // ── Primary ────────────────────────────────────────────────────
    public static readonly Color Primary          = HexColor("#61F4D8");
    public static readonly Color PrimaryContainer = HexColor("#08C1A6");
    public static readonly Color OnPrimary        = HexColor("#001110");

    // ── Secondary ──────────────────────────────────────────────────
    public static readonly Color Secondary   = HexColor("#1CEDE1");
    public static readonly Color Tertiary    = HexColor("#64D0FF");
    public static readonly Color OnSurface   = HexColor("#C0FCF6");

    // ── Error ──────────────────────────────────────────────────────
    public static readonly Color Error    = HexColor("#ED4245");
    public static readonly Color ErrorDim = HexColor("#D7383B");

    // ── Syntax Highlighting (for HexViewer / Code displays) ───────
    public static readonly Color SyntaxFunction = HexColor("#64D0FF"); // tertiary
    public static readonly Color SyntaxString   = HexColor("#1CEDE1"); // secondary
    public static readonly Color SyntaxKeyword  = HexColor("#D7383B"); // error_dim
    public static readonly Color SyntaxComment  = HexColor("#3A6B66");
    public static readonly Color SyntaxNumber   = HexColor("#08C1A6"); // primary-container

    // ── Ambient Glow (for box shadows / outlines) ──────────────────
    // Use as: outline color with 10% alpha
    public static readonly Color PrimaryGlow = new Color(0.38f, 0.96f, 0.85f, 0.10f);

    // ── Ghost Border ───────────────────────────────────────────────
    public static readonly Color GhostBorder = new Color(0.38f, 0.96f, 0.85f, 0.15f);

    // ── Spacing Scale (from Design System) ─────────────────────────
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
