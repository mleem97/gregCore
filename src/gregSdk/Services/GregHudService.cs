using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Sdk.Services;

/// <summary>
/// SDK Service for managing the JADE-style HUD overlay.
/// Allows any mod (C#, Lua, TS, Rust) to display information onto a unified telemetry box.
/// </summary>
public static class GregHudService
{
    private static GameObject _hudRoot;
    private static TextMeshProUGUI _titleText;
    private static TextMeshProUGUI _subHeaderText;
    private static GameObject _entriesContainer;
    private static CanvasGroup _canvasGroup;

    public static void Initialize()
    {
        if (_hudRoot != null) return;

        var canvas = GregUiService.CreateCanvas("gregCore_HUD", 1000100);
        _hudRoot = new GameObject("JadeBox");
        _hudRoot.transform.SetParent(canvas.transform, false);

        var rt = _hudRoot.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, -20);
        rt.sizeDelta = new Vector2(320, 0); // Height driven by layout

        var img = _hudRoot.AddComponent<Image>();
        img.color = GregUiService.LuminescentArchitect.Surface;
        
        // Add a subtle border
        var outline = _hudRoot.AddComponent<Outline>();
        outline.effectColor = new Color(0.38f, 0.96f, 0.85f, 0.3f);
        outline.effectDistance = new Vector2(1, -1);

        var vlg = GregUiService.AddVerticalLayout(_hudRoot, 8, 15);
        vlg.childAlignment = TextAnchor.UpperLeft;

        // Title
        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(_hudRoot.transform, false);
        _titleText = titleGo.AddComponent<TextMeshProUGUI>();
        _titleText.fontSize = 20;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.color = GregUiService.LuminescentArchitect.PrimaryTeal;
        _titleText.text = "GREG_CORE";

        // Subheader
        var subGo = new GameObject("SubHeader");
        subGo.transform.SetParent(_hudRoot.transform, false);
        _subHeaderText = subGo.AddComponent<TextMeshProUGUI>();
        _subHeaderText.fontSize = 12;
        _subHeaderText.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
        _subHeaderText.text = "SYSTEM TELEMETRY";

        GregUiService.AddSeparator(_hudRoot.transform);

        // Container for dynamic entries
        _entriesContainer = new GameObject("Entries");
        _entriesContainer.transform.SetParent(_hudRoot.transform, false);
        GregUiService.AddVerticalLayout(_entriesContainer, 4, 0);

        _canvasGroup = _hudRoot.AddComponent<CanvasGroup>();
        _hudRoot.SetActive(false);
    }

    private static string _lastUpdateSnapshot = "";

    public static void UpdateJadeBox(string title, string subHeader, List<GregMetadataEntry> entries)
    {
        if (_hudRoot == null) Initialize();

        _hudRoot.SetActive(true);
        _titleText.text = title.ToUpper();
        _subHeaderText.text = subHeader.ToUpper();

        // Simple snapshot to avoid unnecessary UI churn
        string snapshot = title + subHeader + entries.Count;
        foreach (var e in entries) snapshot += e.Label + e.Value;

        if (snapshot == _lastUpdateSnapshot) return;
        _lastUpdateSnapshot = snapshot;

        // Clear existing entries
        for (int i = _entriesContainer.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(_entriesContainer.transform.GetChild(i).gameObject);
        }

        foreach (var entry in entries)
        {
            var row = new GameObject("Entry_" + entry.Label);
            row.transform.SetParent(_entriesContainer.transform, false);
            var hlg = GregUiService.AddHorizontalLayout(row, 10);
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = false;

            var label = GregUiService.CreateLabel(row.transform, "Label", $"<color=#61F5DA88>{entry.Label}:</color>", 14);
            label.alignment = TextAlignmentOptions.Left;
            
            var value = GregUiService.CreateLabel(row.transform, "Value", entry.Value, 14);
            value.color = entry.ValueColor;
            value.alignment = TextAlignmentOptions.Right;
            value.fontStyle = FontStyles.Bold;
        }
    }

    public static void HideJadeBox()
    {
        if (_hudRoot != null) _hudRoot.SetActive(false);
    }
}

