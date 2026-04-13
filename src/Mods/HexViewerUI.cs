using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System.Linq;
using Il2Cpp;
using Il2CppTMPro;

using greg.Core.UI;
using greg.Core.UI.Components;
using greg.Sdk.Services;
using greg.Sdk;

namespace greg.Mods;

/// <summary>
/// A refined HexViewer telemetry overlay integrated into gregCore.
/// </summary>
public static class HexViewerUI
{
    private const string ModId = "HexViewer";
    
    // Aesthetic Colors from Screenshot
    private static readonly Color ColorHeader = new Color(0.8f, 1f, 1f); // Light Teal
    private static readonly Color ColorSubHeader = new Color(0f, 0.9f, 0.8f); // HUD Teal
    private static readonly Color ColorValueYellow = new Color(1f, 0.98f, 0f); // #FFF900
    private static readonly Color ColorPanelBg = new Color(0.02f, 0.08f, 0.08f, 0.92f); // Very dark teal

    private static GregPanel _inspectorPanel;
    private static GregPanel _hookMonitorPanel;
    private static GregPanel _uiTreePanel;
    private static GregPanel _jadeOverlay;

    private static List<string> _hookLogs = new();
    private const int MaxHookLogs = 15;

    public static void Init()
    {
        _uiTreePanel = GregUIBuilder.Panel("hexviewer.uitree").Title("⬡ UI TREE").Position(GregUIAnchor.TopLeft).Size(280, 450).Build();
        _hookMonitorPanel = GregUIBuilder.Panel("hexviewer.hookmonitor").Title("⬡ HOOK LOG").Position(GregUIAnchor.BottomRight).Size(320, 250).Build();
        _inspectorPanel = GregUIBuilder.Panel("hexviewer.inspector").Title("⬡ INSPECTOR").Position(GregUIAnchor.BottomLeft).Size(280, 200).Build();

        // JADE Live Overlay (Primary Telemetry - HUD Style)
        _jadeOverlay = GregUIBuilder.Panel("hexviewer.jade")
            .Title("SCANNING...")
            .Position(GregUIAnchor.TopRight)
            .Size(350, 180)
            .Build();

        _jadeOverlay.Show();
        gregEventDispatcher.On(gregNativeEventHooks.WorldInteractionHovered, OnHookDetected, ModId);
        
        MelonLogger.Msg("[gregCore] HexViewer Subsystem initialized.");
    }

    public static void Shutdown()
    {
        gregEventDispatcher.UnregisterAll(ModId);
    }

    public static void OnUpdate()
    {
        if (UnityEngine.InputSystem.Keyboard.current == null) return;

        if (UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame) { _uiTreePanel?.Toggle(); if (_uiTreePanel != null && _uiTreePanel.IsVisible) RefreshUITree(); }
        if (UnityEngine.InputSystem.Keyboard.current.f2Key.wasPressedThisFrame) { _hookMonitorPanel?.Toggle(); }
        if (UnityEngine.InputSystem.Keyboard.current.f3Key.wasPressedThisFrame) { _inspectorPanel?.Toggle(); }
        
        UpdateJadeOverlay();
        if (_hookMonitorPanel?.IsVisible == true) UpdateHookMonitorUI();
    }

    private static void RefreshUITree()
    {
        var logs = new List<string>();
        logs.Add("SCANNING ACTIVE CANVASES...");
        
        var canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
        foreach (var c in canvases)
        {
            if (!c.gameObject.activeInHierarchy) continue;
            logs.Add($"■ {c.name} (Sorting: {c.sortingOrder})");
            for (int i = 0; i < c.transform.childCount; i++)
            {
                var child = c.transform.GetChild(i);
                if (child.gameObject.activeInHierarchy)
                    logs.Add($"  ∟ {child.name}");
            }
        }
        UpdateStandardPanel("UI TREE", logs, _uiTreePanel);
    }

    private static void OnHookDetected(object payload)
    {
        try {
            string hookName = gregPayload.Get(payload, "HookName", "unknown");
            _hookLogs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {hookName}");
            if (_hookLogs.Count > MaxHookLogs) _hookLogs.RemoveAt(MaxHookLogs);
        } catch {}
    }

    private static void UpdateHookMonitorUI()
    {
        UpdateStandardPanel("HOOK LOG", _hookLogs, _hookMonitorPanel);
    }

    private static void UpdateJadeOverlay()
    {
        if (_jadeOverlay == null) return;

        var hitResult = greg.Core.gregGameHooks.RaycastForward(10.0f);
        if (hitResult == null || hitResult.Value.Entity == null)
        {
            _jadeOverlay.Hide();
            return;
        }

        GameObject entity = hitResult.Value.Entity;
        var rack = entity.GetComponentInParent<Rack>();
        var cable = entity.GetComponentInParent<CableLink>();
        bool isSpool = entity.name.ToLower().Contains("spool") || entity.name.ToLower().Contains("spinner");

        if (rack == null && cable == null && !isSpool)
        {
            _jadeOverlay.Hide();
            return;
        }

        _jadeOverlay.Show();
        string typeLabel = "OBJECT";
        string subHeader = "TELEMETRY";
        var details = new Dictionary<string, (string value, Color color)>();

        if (rack != null)
        {
            typeLabel = "SERVER RACK";
            subHeader = "RACK TELEMETRY";
            int deviceCount = 0;
            float totalIops = 0f;

            if (rack.isPositionUsed != null)
            {
                foreach (int used in rack.isPositionUsed) if (used != 0) deviceCount++;
            }

            var servers = rack.GetComponentsInChildren<Server>();
            foreach (var s in servers) totalIops += s.currentProcessingSpeed;

            var renderer = rack.GetComponentInChildren<MeshRenderer>();
            string hexStr = renderer != null ? $"#{ColorUtility.ToHtmlStringRGB(renderer.material.color)}" : "#FFFFFF";

            details.Add("ID", (rack.gameObject.name, Color.white));
            details.Add("DEVICES", (deviceCount.ToString(), Color.white));
            details.Add("IOPS", (totalIops.ToString("F1"), ColorHeader));
            details.Add("HEX", (hexStr, ColorValueYellow));
            details.Add("STATUS", ("RACK TELEMETRY", ColorSubHeader));
        }
        else if (cable != null || isSpool)
        {
            typeLabel = cable != null ? "CABLE LINK" : "CABLE SPOOL";
            subHeader = "LINK TELEMETRY";
            string type = cable != null ? cable.typeOfLink.ToString().ToUpper() : "SPOOL";
            
            var renderer = entity.GetComponentInChildren<MeshRenderer>();
            string hexStr = renderer != null ? $"#{ColorUtility.ToHtmlStringRGB(renderer.material.color)}" : "#FFFFFF";

            details.Add("TYPE", (type, ColorHeader));
            details.Add("HEX", (hexStr, ColorValueYellow));
            
            if (cable != null)
            {
                details.Add("SPEED", (cable.connectionSpeed.ToString() + " Gbps", Color.white));
            }
            details.Add("STATUS", ("LINK ACTIVE", ColorSubHeader));
        }

        DrawHudOverlay(typeLabel, subHeader, details);

        if (_inspectorPanel?.IsVisible == true)
        {
            var components = entity.GetComponents<Component>();
            var compLogs = new List<string>();
            compLogs.Add($"INSPECTING: {entity.name}");
            foreach (var comp in components) if (comp != null) compLogs.Add($"• {comp.GetType().Name}");
            UpdateStandardPanel("INSPECTOR", compLogs, _inspectorPanel);
        }
    }

    private static void DrawHudOverlay(string title, string subHeader, Dictionary<string, (string value, Color color)> details)
    {
        var panelGO = _jadeOverlay.PanelRoot;
        if (panelGO == null) return;

        var bg = panelGO.GetComponent<Image>();
        if (bg != null) bg.color = ColorPanelBg;

        var header = panelGO.transform.Find("Header");
        if (header != null)
        {
             var hImg = header.GetComponent<Image>();
             if (hImg != null) hImg.enabled = false;
             
             var titleText = header.Find("Title")?.GetComponent<TextMeshProUGUI>();
             if (titleText != null) 
             {
                 titleText.text = $"<size=120%>□</size> {title.ToUpper()}";
                 titleText.color = Color.white;
                 titleText.fontStyle = FontStyles.Bold;
             }
        }

        var content = panelGO.transform.Find("Content");
        if (content != null)
        {
            for (int i = content.childCount - 1; i >= 0; i--) UnityEngine.Object.Destroy(content.GetChild(i).gameObject);

            AddHudLabel(content, subHeader.ToUpper(), ColorSubHeader, 12, FontStyles.Bold);
            new GameObject("Spacer").transform.SetParent(content, false);

            foreach (var kv in details)
            {
                AddHudLabel(content, $"{kv.Key}: <color=#{ColorUtility.ToHtmlStringRGB(kv.Value.color)}>{kv.Value.value}</color>", Color.white, 11, FontStyles.Bold);
            }
        }
    }

    private static void AddHudLabel(Transform parent, string text, Color color, float size, FontStyles style)
    {
        var go = new GameObject("HudLabel");
        go.transform.SetParent(parent, false);
        var tm = go.AddComponent<TextMeshProUGUI>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = size;
        tm.fontStyle = style;
        tm.alignment = TextAlignmentOptions.Left;
    }

    private static void UpdateStandardPanel(string title, List<string> details, GregPanel panel)
    {
        if (panel == null) return;
        var panelGO = panel.PanelRoot;
        if (panelGO == null) return;

        var titleText = panelGO.transform.Find("Header/Title")?.GetComponent<TextMeshProUGUI>();
        if (titleText != null) titleText.text = $"⬡ {title}";

        var content = panelGO.transform.Find("Content");
        if (content != null)
        {
            for (int i = content.childCount - 1; i >= 0; i--) UnityEngine.Object.Destroy(content.GetChild(i).gameObject);

            foreach (var detail in details)
            {
                var detailGO = new GameObject("Detail");
                detailGO.transform.SetParent(content, false);
                var text = detailGO.AddComponent<TextMeshProUGUI>();
                text.text = $"> {detail}";
                text.fontSize = 11;
                text.fontStyle = FontStyles.Bold;
                text.color = GregUITheme.OnSurface;
            }
        }
    }
}
