using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace greg.Core.UI.Components;

/// <summary>
/// A base panel component using gregUI theme.
/// </summary>
public class GregPanel : IGregUIElement
{
    public string PanelId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Vector2 Size { get; set; } = new Vector2(400, 300);
    public GregUIAnchor Anchor { get; set; } = GregUIAnchor.Center;
    public bool Draggable { get; set; } = true;
    public bool Closable { get; set; } = true;
    public List<IGregUIComponent> Components { get; set; } = new();

    private GameObject _panelGO;
    private bool _isVisible = false;

    public bool IsVisible => _isVisible;
    public GameObject PanelRoot => _panelGO;

    public void Attach(Canvas rootCanvas)
    {
        if (_panelGO != null) return;

        _panelGO = new GameObject($"gregUI_{PanelId}");
        _panelGO.transform.SetParent(rootCanvas.transform, false);

        var rect = _panelGO.AddComponent<RectTransform>();
        rect.sizeDelta = Size;
        ApplyAnchor(rect, Anchor);

        var img = _panelGO.AddComponent<Image>();
        img.color = GregUITheme.Surface;

        // Add background outline (Ghost Border)
        var outline = _panelGO.AddComponent<Outline>();
        outline.effectColor = GregUITheme.GhostBorder;
        outline.effectDistance = new Vector2(1, 1);

        // Header
        var header = new GameObject("Header");
        header.transform.SetParent(_panelGO.transform, false);
        var headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 32);
        headerRect.anchoredPosition = Vector2.zero;

        var headerImg = header.AddComponent<Image>();
        headerImg.color = GregUITheme.SurfaceContainerHigh;

        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(header.transform, false);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = Title;
        titleText.fontSize = 14;
        titleText.color = GregUITheme.OnSurface;
        titleText.alignment = TextAlignmentOptions.Left;
        
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.sizeDelta = new Vector2(-16, 0);
        titleRect.anchoredPosition = new Vector2(8, 0);

        // Content Area
        var content = new GameObject("Content");
        content.transform.SetParent(_panelGO.transform, false);
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(0, -32);
        contentRect.anchoredPosition = new Vector2(0, -16);

        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset { left = 8, right = 8, top = 8, bottom = 8 };
        layout.spacing = GregUITheme.SpaceSM;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        foreach (var comp in Components)
        {
            comp.Build(content.transform);
        }

        _panelGO.SetActive(_isVisible);
    }

    private void ApplyAnchor(RectTransform rect, GregUIAnchor anchor)
    {
        switch (anchor)
        {
            case GregUIAnchor.TopLeft:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                break;
            case GregUIAnchor.TopRight:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                break;
            case GregUIAnchor.BottomLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
                break;
            case GregUIAnchor.BottomRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
                break;
            case GregUIAnchor.Center:
            default:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
        rect.anchoredPosition = Vector2.zero;
    }

    public void Show()
    {
        _isVisible = true;
        _panelGO?.SetActive(true);
    }

    public void Hide()
    {
        _isVisible = false;
        _panelGO?.SetActive(false);
    }

    public void Toggle()
    {
        _isVisible = !_isVisible;
        _panelGO?.SetActive(_isVisible);
    }

    public void Destroy()
    {
        if (_panelGO != null)
        {
            UnityEngine.Object.Destroy(_panelGO);
            _panelGO = null;
        }
    }
}

public enum GregUIAnchor { TopLeft, TopRight, TopCenter, Center, BottomLeft, BottomRight, BottomCenter }
