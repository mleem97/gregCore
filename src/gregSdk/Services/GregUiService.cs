using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2Cpp;

namespace greg.Sdk.Services;

public static class GregUiService
{
    public static class LuminescentArchitect
    {
        public static readonly Color Surface = new Color(0.00f, 0.05f, 0.04f, 0.88f); // Slightly more transparent
        public static readonly Color SurfaceContainer = new Color(0.00f, 0.10f, 0.09f, 0.92f);
        public static readonly Color PrimaryTeal = new Color(0.38f, 0.96f, 0.85f, 1f);
        public static readonly Color GlowTeal = new Color(0.38f, 0.96f, 0.85f, 0.25f);
        public static readonly Color OnSurface = new Color(0.85f, 1.00f, 0.98f, 1f); // Brighter text
    }

    private static readonly Dictionary<string, Action<GameObject>> _replacements = new();

    public static void RegisterReplacement(string panelName, Action<GameObject> factory)
    {
        _replacements[panelName] = factory;
        MelonLoader.MelonLogger.Msg($"[gregCore] Registered UI Replacement for: {panelName}");
    }

    public static bool TryGetReplacement(string panelName, out Action<GameObject> factory)
    {
        return _replacements.TryGetValue(panelName, out factory);
    }

    public static float GlobalScale { get; private set; } = 0.85f;
    private static List<CanvasScaler> _trackedScalers = new();

    public static Canvas CreateCanvas(string name, int sortingOrder = 1000000)
    {
        var go = new GameObject(name);
        UnityEngine.Object.DontDestroyOnLoad(go);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920 / GlobalScale, 1080 / GlobalScale);
        _trackedScalers.Add(scaler);
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static GameObject CreatePanel(string title, Vector2 size)
    {
        var canvas = CreateCanvas(title);
        var panelGo = new GameObject("Panel");
        panelGo.transform.SetParent(canvas.transform, false);
        var rect = panelGo.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        return panelGo;
    }

    public static void SetGlobalScale(float scale)
    {
        GlobalScale = Mathf.Clamp(scale, 0.5f, 2.0f);
        foreach (var scaler in _trackedScalers)
        {
            if (scaler != null)
                scaler.referenceResolution = new Vector2(1920 / GlobalScale, 1080 / GlobalScale);
        }
    }

    public static GameObject CreateModernPanel(Transform parent, string name, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = LuminescentArchitect.Surface;
        
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.38f, 0.96f, 0.85f, 0.15f);
        outline.effectDistance = new Vector2(1, -1);
        
        return go;
    }

    public static Button CreateModernButton(Transform parent, string name, string label, System.Action onClick)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 40);

        var img = go.AddComponent<Image>();
        img.color = LuminescentArchitect.SurfaceContainer;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = LuminescentArchitect.SurfaceContainer;
        colors.highlightedColor = LuminescentArchitect.PrimaryTeal;
        colors.pressedColor = new Color(0.03f, 0.75f, 0.65f, 1f);
        btn.colors = colors;

        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.38f, 0.96f, 0.85f, 0.4f);
        outline.effectDistance = new Vector2(1, -1);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 14;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = LuminescentArchitect.OnSurface;

        if (onClick != null) btn.onClick.AddListener((System.Action)onClick);

        return btn;
    }

    public static Il2CppTMPro.TextMeshProUGUI CreateLabel(Transform parent, string name, string text, int fontSize = 14)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);
        var tmp = go.AddComponent<Il2CppTMPro.TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = LuminescentArchitect.OnSurface;
        tmp.alignment = Il2CppTMPro.TextAlignmentOptions.Left;
        return tmp;
    }

    /// <summary>
    /// Adds a VerticalLayoutGroup with Luminescent standard spacing (24px).
    /// </summary>
    public static VerticalLayoutGroup AddVerticalLayout(GameObject target, int spacing = 12, int padding = 20)
    {
        var vlg = target.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = spacing;
        vlg.padding = new RectOffset();
        vlg.padding.left = padding;
        vlg.padding.right = padding;
        vlg.padding.top = padding;
        vlg.padding.bottom = padding;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        var csf = target.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return vlg;
    }

    /// <summary>
    /// Adds a HorizontalLayoutGroup for toolbars or row-based elements.
    /// </summary>
    public static HorizontalLayoutGroup AddHorizontalLayout(GameObject target, int spacing = 10)
    {
        var hlg = target.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = spacing;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        return hlg;
    }

    public static void AddSeparator(Transform parent)
    {
        var go = new GameObject("Separator");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 2);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.38f, 0.96f, 0.85f, 0.2f);
    }

    public static void TakeoverVanillaUi(GameObject vanillaRoot)
    {
        if (vanillaRoot == null) return;
        foreach (var graphic in vanillaRoot.GetComponentsInChildren<Graphic>(true)) graphic.enabled = false;
        foreach (var sel in vanillaRoot.GetComponentsInChildren<Selectable>(true)) { sel.interactable = false; sel.enabled = false; }
        MelonLoader.MelonLogger.Msg($"[gregCore] UI Takeover: {vanillaRoot.name}");
    }
}

