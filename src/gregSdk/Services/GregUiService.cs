using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppTMPro;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Service for creating and managing uGUI elements with the Luminescent Architect branding.
/// </summary>
public static class GregUiService
{
    private static GameObject _clickBlocker;

    public static class LuminescentArchitect
    {
        public static readonly Color Surface = new Color(0.00f, 0.07f, 0.06f, 0.85f); // #001110
        public static readonly Color SurfaceContainer = new Color(0.00f, 0.12f, 0.11f, 0.90f); // #001E1C
        public static readonly Color PrimaryTeal = new Color(0.38f, 0.96f, 0.85f, 1f); // #61F4D8
        public static readonly Color OnSurface = new Color(0.75f, 0.99f, 0.96f, 1f); // #C0FCF6
    }

    /// <summary>
    /// Creates a new Screen Space Overlay Canvas.
    /// </summary>
    public static Canvas CreateCanvas(string name, int sortingOrder = 1000000)
    {
        var go = new GameObject(name);
        UnityEngine.Object.DontDestroyOnLoad(go);
        
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        canvas.overrideSorting = true;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    /// <summary>
    /// Creates a Luminescent Architect styled panel.
    /// </summary>
    public static GameObject CreatePanel(Transform parent, string name, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = size;

        var img = go.AddComponent<Image>();
        img.color = LuminescentArchitect.Surface;
        img.raycastTarget = true;

        return go;
    }

    /// <summary>
    /// Creates a TextMeshPro label.
    /// </summary>
    public static TextMeshProUGUI CreateLabel(Transform parent, string name, string text, int fontSize = 14)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = LuminescentArchitect.OnSurface;
        tmp.alignment = TextAlignmentOptions.Left;

        return tmp;
    }

    /// <summary>
    /// Blocks or unblocks game input.
    /// </summary>
    public static void SetInputBlocking(bool block)
    {
        if (block)
        {
            if (_clickBlocker == null)
            {
                var canvas = CreateCanvas("greg_InputBlocker", 2000000);
                _clickBlocker = canvas.gameObject;
                
                var plate = new GameObject("Plate");
                plate.transform.SetParent(_clickBlocker.transform, false);
                var rt = plate.AddComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                var image = plate.AddComponent<Image>();
                image.color = new Color(0f, 0f, 0f, 0f);
                image.raycastTarget = true;
            }
            _clickBlocker.SetActive(true);
        }
        else if (_clickBlocker != null)
        {
            _clickBlocker.SetActive(false);
        }
    }
}
