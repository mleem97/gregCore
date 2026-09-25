using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Il2CppTMPro;
using MelonLoader;

namespace gregCore.UI;

public static partial class GregFontLoader
{
    /// <summary>
    /// Applies the resolved UI Toolkit font to a VisualElement style.
    /// Children inherit the font definition, so calling this on a root
    /// element is enough for all descendant labels.
    /// </summary>
    public static bool ApplyFontTo(VisualElement element)
    {
        if (element == null) return false;
        if (_defaultUGUIFont == null && _defaultTMPFontAsset == null) SearchFonts();
        bool any = TryApplyFontDefinition(element);
        any = TryApplyLegacyFont(element, any) || any;
        if (!any)
            MelonLogger.Warning("[FontLoader] ApplyFontTo: no font slot could be set.");
        return any;
    }

    private static bool TryApplyFontDefinition(VisualElement element)
    {
        try
        {
            if (!TryResolveFontDefinition(out FontDefinition? fd, out string source)) return false;
            if (fd == null) return false;
            element.style.unityFontDefinition = new StyleFontDefinition(fd);
            MelonLogger.Msg($"[FontLoader] ApplyFontTo: unityFontDefinition set via {source}.");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[FontLoader] ApplyFontTo: unityFontDefinition failed: {ex.Message}");
            return false;
        }
    }

    private static bool TryResolveFontDefinition(out FontDefinition? fd, out string source)
    {
        fd = null;
        source = "";
        try
        {
            if (TryResolveSdfAsset(out fd, out source)) return true;
            if (TryResolveTmpFont(out fd, out source)) return true;
            return TryResolveLegacyFont(out fd, out source);
        }
        catch { return false; }
    }

    private static bool TryResolveSdfAsset(out FontDefinition? fd, out string source)
    {
        fd = null;
        source = "";
        try
        {
            if (_defaultFontAsset == null) return false;
            fd = FontDefinition.FromSDFFont(_defaultFontAsset);
            source = $"SDF FontAsset '{_defaultFontAsset.name}'";
            return true;
        }
        catch { return false; }
    }

    private static bool TryResolveTmpFont(out FontDefinition? fd, out string source)
    {
        fd = null;
        source = "";
        try
        {
            if (_defaultTMPFontAsset == null) return false;
            var asFontAsset = _defaultTMPFontAsset.TryCast<FontAsset>();
            if (asFontAsset == null) return false;
            fd = FontDefinition.FromSDFFont(asFontAsset);
            source = $"TMP cast '{_defaultTMPFontAsset.name}'";
            return true;
        }
        catch { return false; }
    }

    private static bool TryResolveLegacyFont(out FontDefinition? fd, out string source)
    {
        fd = null;
        source = "";
        try
        {
            if (_defaultUGUIFont == null) return false;
            fd = FontDefinition.FromFont(_defaultUGUIFont);
            source = $"legacy Font '{_defaultUGUIFont.name}'";
            return true;
        }
        catch { return false; }
    }

    private static bool TryApplyLegacyFont(VisualElement element, bool already)
    {
        try
        {
            if (_defaultUGUIFont == null) return already;
            element.style.unityFont = new StyleFont(_defaultUGUIFont);
            if (!already) MelonLogger.Msg("[FontLoader] ApplyFontTo: unityFont (legacy) set.");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[FontLoader] ApplyFontTo: unityFont failed: {ex.Message}");
            return already;
        }
    }
}
