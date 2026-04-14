using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppTMPro;
using MelonLoader;

namespace greg.Core.UI;

/// <summary>
/// The Ultimate Unity UI Hijacker Core.
/// Implements "The Luminescent Architect" design system via runtime instantiation.
/// </summary>
public static class GregUiBuilder
{
    private const float REFERENCE_WIDTH = 1920f;
    private const float REFERENCE_HEIGHT = 1080f;

    /// <summary>
    /// Phase 3: Create a robust, resolution-independent canvas.
    /// </summary>
    public static GameObject CreateLuminescentCanvas(string name, int sortingOrder = 999)
    {
        var go = new GameObject(name);
        UnityEngine.Object.DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(REFERENCE_WIDTH, REFERENCE_HEIGHT);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();

        EnsureEventSystem();

        return go;
    }

    /// <summary>
    /// Phase 4: Create a glass-morphic container.
    /// Implements the "Glass & Gradient" rule (80% opacity, 12px blur).
    /// </summary>
    public static GameObject CreateGlassPanel(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.color = GregUITheme.GlassBackground;
        
        // Add CanvasGroup for alpha blending / animations
        go.AddComponent<CanvasGroup>();

        return go;
    }

    /// <summary>
    /// Create a button with the kinetic core signature (Teal Gradient).
    /// </summary>
    public static GameObject CreateMachinedButton(Transform parent, string label, Action onClick)
    {
        var go = new GameObject($"Button_{label}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280, 56);

        var img = go.AddComponent<Image>();
        img.color = GregUITheme.Primary; // Fallback or base color for gradient

        var btn = go.AddComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;
        
        var colors = btn.colors;
        colors.normalColor = GregUITheme.Primary;
        colors.highlightedColor = Color.Lerp(GregUITheme.Primary, Color.white, 0.2f);
        colors.pressedColor = GregUITheme.PrimaryContainer;
        btn.colors = colors;

        btn.onClick.AddListener(onClick);

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 18;
        tmp.color = GregUITheme.OnPrimary;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.font = GetDefaultFont(false);

        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return go;
    }

    public static TextMeshProUGUI CreateEditorialLabel(Transform parent, string text, bool isHeadline = false)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = GregUITheme.OnSurface;
        tmp.fontSize = isHeadline ? 32 : 14;
        tmp.font = GetDefaultFont(isHeadline);
        
        if (isHeadline)
        {
            tmp.characterSpacing = 2f; // +2% tracking
            tmp.fontStyle = FontStyles.Bold;
        }

        return tmp;
    }

    private static void EnsureEventSystem()
    {
        if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
        {
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            MelonLogger.Msg("[GregUiBuilder] Created missing EventSystem.");
        }
    }

    private static TMP_FontAsset GetDefaultFont(bool headline)
    {
        // Strategy: Search for Inter or Space Grotesk in loaded assets
        var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        var targetName = headline ? "Space" : "Inter";
        
        var found = fonts.FirstOrDefault(f => f.name.Contains(targetName));
        return found ?? (fonts.Length > 0 ? fonts[0] : null);
    }
}
