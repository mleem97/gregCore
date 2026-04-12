using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2Cpp;
using greg.Sdk.Services;

namespace greg.Core.UI;

public static class gregModConfigManager
{
    private static bool _isOpen;
    private static Vector2 _scrollPos;
    private static GUIStyle _headerStyle;
    private static GUIStyle _buttonStyle;
    private static GUIStyle _boxStyle;
    private static GUIStyle _tabStyle;
    
    private static int _activeTab = 0;
    private static string[] _tabs = { "MODS", "CONTROLS" };

    // Rebinding state
    private static bool _isRebinding = false;
    private static InputActionRebindingExtensions.RebindingOperation _rebindOperation;
    private static string _rebindingActionName = "";

    public static void Toggle(bool open)
    {
        _isOpen = open;
        if (!open) 
        {
            GregConfigService.Save();
            if (_isRebinding) CancelRebind();
        }
    }

    public static void DrawConfigUI()
    {
        if (!_isOpen) return;

        InitStyles();

        float width = 700f;
        float height = 600f;
        float x = (Screen.width / 2f) - (width / 2f);
        float y = (Screen.height / 2f) - (height / 2f);

        GUI.backgroundColor = new Color(0.00f, 0.07f, 0.06f, 0.95f);
        GUI.Box(new Rect(x, y, width, height), "");

        GUI.Label(new Rect(x, y + 20f, width, 40f), "<b>GREG FRAMEWORK CONFIGURATION</b>", _headerStyle);

        // Tabs
        _activeTab = GUI.Toolbar(new Rect(x + 20f, y + 70f, width - 40f, 30f), _activeTab, _tabs, _tabStyle);

        _scrollPos = GUI.BeginScrollView(new Rect(x + 20f, y + 110f, width - 40f, height - 160f), _scrollPos, new Rect(0, 0, width - 60f, 1500f));
        GUILayout.BeginArea(new Rect(0, 0, width - 60f, 1500f));

        if (_activeTab == 0)
        {
            DrawModSettings("gregCore", "Core Framework Settings");
            DrawModSettings("HexViewer", "JADE HUD Settings");
            DrawModSettings("greg.AutoEmployees", "Automation Settings");
        }
        else if (_activeTab == 1)
        {
            DrawControlsTab();
        }

        GUILayout.EndArea();
        GUI.EndScrollView();

        if (GUI.Button(new Rect(x + width - 120f, y + height - 40f, 100f, 30f), "CLOSE", _buttonStyle))
        {
            Toggle(false);
        }
    }

    private static void DrawControlsTab()
    {
        GUILayout.Label("<b>KEYBINDINGS</b>", _headerStyle);
        GUILayout.Space(10);

        if (_isRebinding)
        {
            GUILayout.BeginVertical(_boxStyle);
            GUILayout.Label($"<color=#61F4D8>Waiting for input...</color>", _headerStyle);
            GUILayout.Label($"Press any key for: {_rebindingActionName}", _headerStyle);
            if (GUILayout.Button("CANCEL", _buttonStyle, GUILayout.Width(100)))
            {
                CancelRebind();
            }
            GUILayout.EndVertical();
            return;
        }

        // Fetch game's RebindUIv2 elements
        var rebindUIs = RebindUIv2.s_RebindActionUIs;
        if (rebindUIs == null || rebindUIs.Count == 0)
        {
            GUILayout.Label("No bindings available or game controls not loaded yet.");
            return;
        }

        for (int i = 0; i < rebindUIs.Count; i++)
        {
            var ui = rebindUIs[i];
            if (ui == null || ui.m_Action == null) continue;

            string actionName = ui.m_ActionLabel != null ? ui.m_ActionLabel.text : ui.m_Action.action.name;
            string currentBinding = ui.m_BindingText != null ? ui.m_BindingText.text : "?";

            GUILayout.BeginHorizontal(_boxStyle);
            GUILayout.Label(actionName, GUILayout.Width(250));
            GUILayout.Label($"<color=#C0FCF6>{currentBinding}</color>", GUILayout.Width(150));
            
            if (GUILayout.Button("REBIND", _buttonStyle, GUILayout.Width(100)))
            {
                StartRebind(ui, actionName);
            }
            GUILayout.EndHorizontal();
        }
    }

    private static void StartRebind(RebindUIv2 ui, string actionName)
    {
        _isRebinding = true;
        _rebindingActionName = actionName;

        ui.ResolveActionAndBinding(out var action, out int bindingIndex);
        if (action == null) { CancelRebind(); return; }

        action.Disable();

        _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete((System.Action<InputActionRebindingExtensions.RebindingOperation>)(op => {
                action.Enable();
                ui.UpdateBindingDisplay();
                CleanUpRebind();
            }))
            .OnCancel((System.Action<InputActionRebindingExtensions.RebindingOperation>)(op => {
                action.Enable();
                CleanUpRebind();
            }));

        _rebindOperation.Start();
    }

    private static void CancelRebind()
    {
        if (_rebindOperation != null)
        {
            _rebindOperation.Cancel();
        }
        CleanUpRebind();
    }

    private static void CleanUpRebind()
    {
        if (_rebindOperation != null)
        {
            _rebindOperation.Dispose();
            _rebindOperation = null;
        }
        _isRebinding = false;
    }

    private static void DrawModSettings(string modId, string displayName)
    {
        GUILayout.BeginVertical(_boxStyle);
        GUILayout.Label($"<b>{displayName}</b>", GUILayout.Width(500));
        
        if (modId == "HexViewer")
        {
            bool enabled = GregConfigService.Get(modId, "Enabled", true);
            bool nextEnabled = GUILayout.Toggle(enabled, "Enable JADE HUD");
            if (nextEnabled != enabled) GregConfigService.Set(modId, "Enabled", nextEnabled);
        }
        else if (modId == "greg.AutoEmployees")
        {
            bool autoRepair = GregConfigService.Get(modId, "AutoRepair", true);
            bool nextRepair = GUILayout.Toggle(autoRepair, "Auto-Repair EOL");
            if (nextRepair != autoRepair) GregConfigService.Set(modId, "AutoRepair", nextRepair);

            float threshold = (float)Convert.ToDouble(GregConfigService.Get(modId, "Threshold", 50.0));
            float nextThreshold = GUILayout.HorizontalSlider(threshold, 0f, 100f);
            if (Math.Abs(nextThreshold - threshold) > 0.1f) GregConfigService.Set(modId, "Threshold", nextThreshold);
            GUILayout.Label($"Efficiency Threshold: {nextThreshold:F0}%");
        }
        else
        {
            GUILayout.Label("No configurable settings for this mod version.");
        }

        GUILayout.EndVertical();
        GUILayout.Space(10);
    }

    private static void InitStyles()
    {
        if (_headerStyle != null) return;

        _headerStyle = new GUIStyle();
        _headerStyle.alignment = TextAnchor.MiddleCenter;
        _headerStyle.fontSize = 18;
        _headerStyle.fontStyle = FontStyle.Bold;
        _headerStyle.normal.textColor = new Color(0.38f, 0.96f, 0.85f, 1f);

        _buttonStyle = new GUIStyle();
        _buttonStyle.alignment = TextAnchor.MiddleCenter;
        _buttonStyle.normal.textColor = Color.white;
        _buttonStyle.fontStyle = FontStyle.Bold;

        _tabStyle = new GUIStyle();
        _tabStyle.fontSize = 14;
        _tabStyle.fontStyle = FontStyle.Bold;
        _tabStyle.normal.textColor = new Color(0.75f, 0.99f, 0.96f, 1f);

        _boxStyle = new GUIStyle();
        _boxStyle.padding = new RectOffset();
        _boxStyle.padding.left = 10;
        _boxStyle.padding.right = 10;
        _boxStyle.padding.top = 10;
        _boxStyle.padding.bottom = 10;
        _boxStyle.margin = new RectOffset();
        _boxStyle.margin.left = 5;
        _boxStyle.margin.right = 5;
        _boxStyle.margin.top = 5;
        _boxStyle.margin.bottom = 5;
    }
}
