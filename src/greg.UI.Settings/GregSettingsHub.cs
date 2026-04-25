using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.API;
using gregCore.UI;

namespace greg.UI.Settings
{
    public class GregSettingsHub : MonoBehaviour
    {
        private static GregSettingsHub? _instance;
        private bool _isVisible = false;
        private int _selectedTab = 0;
        private GameObject? _uiPanel;
        
        private class TabData
        {
            public string Id = string.Empty;
            public string Label = string.Empty;
            public Action<gregCore.UI.GregUIBuilder>? BuildFn;
        }
        
        private static readonly List<TabData> _tabs = new();

        public static void Initialize()
        {
            if (_instance == null)
            {
                var go = new GameObject("GregSettingsHub");
                _instance = go.AddComponent<GregSettingsHub>();
                UnityEngine.Object.DontDestroyOnLoad(go);
                
                RegisterStandardTabs();
            }
        }

        public static void RegisterTab(string tabId, string label, Action<gregCore.UI.GregUIBuilder> buildFn)
        {
            if (!_tabs.Exists(t => t.Id == tabId))
            {
                _tabs.Add(new TabData { Id = tabId, Label = label, BuildFn = buildFn });
            }
        }

        public static void UnregisterTab(string tabId)
        {
            _tabs.RemoveAll(t => t.Id == tabId);
        }

        private static void RegisterStandardTabs()
        {
            RegisterTab("greg.core", "Framework", builder =>
            {
                builder.AddLabel("gregCore v1.0.0.40-pre")
                       .AddLabel("MelonLoader v0.6+")
                       .AddLabel($"Save Mode: {(frameworkSdk.GregFeatureGuard.IsVanillaSave ? "Vanilla" : "Greg")}")
                       .AddToggle("Verbose Startup Log", false, v => { })
                       .AddToggle("Debug Mode (alle Hooks loggen)", false, v => { })
                       .AddButton("Run Language Scan now", () => { })
                       .AddButton("Show Missing.md Status", () => { });
            });

            RegisterTab("greg.grid", "Grid", builder =>
            {
                var grid = greg.GridPlacement.GregGridManager.Instance;
                if (frameworkSdk.GregFeatureGuard.IsVanillaSave)
                {
                    builder.AddLabel("⚠ Vanilla Save detected — Grid Placement disabled");
                }
                
                builder.AddToggle("Grid Placement Active", frameworkSdk.GregFeatureGuard.IsEnabled("GridPlacement"), v => {
                    if (v) frameworkSdk.GregFeatureGuard.EnableFeature("GridPlacement");
                    else frameworkSdk.GregFeatureGuard.DisableFeature("GridPlacement");
                });
                
                if (grid != null)
                {
                    builder.AddToggle("Show Grid Lines", grid.ShowGridLines, v => grid.ShowGridLines = v)
                           .AddToggle("Show Sub-Grid", grid.ShowSubGrid, v => grid.ShowSubGrid = v)
                           .AddSlider("Sub-Grid Zoom Threshold", 1.0f, 10.0f, 5.0f, v => { })
                           .AddLabel($"Grid Size: 50x50")
                           .AddButton("Clear All Greg Racks", () => grid.ClearAll());
                }
            });

            RegisterTab("greg.save", "SaveEngine", builder =>
            {
                if (frameworkSdk.GregFeatureGuard.IsVanillaSave)
                {
                    builder.AddLabel("⚠ Vanilla Save detected — SaveEngine write disabled");
                }
                
                builder.AddToggle("greg.SaveEngine Active", frameworkSdk.GregFeatureGuard.IsEnabled("SaveEngine.Write"), v => {
                    if (v) frameworkSdk.GregFeatureGuard.EnableFeature("SaveEngine.Write");
                    else frameworkSdk.GregFeatureGuard.DisableFeature("SaveEngine.Write");
                });
                
                builder.AddSlider("Auto-Save Interval (seconds)", 10f, 300f, greg.SaveEngine.GregSaveScheduler.AutoSaveIntervalSeconds, v => greg.SaveEngine.GregSaveScheduler.AutoSaveIntervalSeconds = v)
                       .AddToggle("Save Grid State", true, v => { })
                       .AddToggle("Save Server State", true, v => { });
                       
                var engine = greg.SaveEngine.GregSaveEngine.Instance;
                builder.AddLabel($"DB File: {(engine != null ? engine.DbPath : "None")}")
                       .AddButton("Save Now", () => engine?.SaveAll());
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Toggle();
            }
            if (_isVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            _isVisible = !_isVisible;
            if (_isVisible && _uiPanel == null)
            {
                BuildUI();
            }
            GregUIManager.SetPanelActive("SettingsHub", _isVisible);
        }

        private void BuildUI()
        {
            var builder = GregUIBuilder.CreateTablet("Settings Hub")
                .SetSize(500, 600)
                .AddHeadline("GREGCORE SETTINGS");

            // Build UI for each registered mod
            foreach (var category in gregCore.Core.Config.ModMenu.Categories)
            {
                category.BuildUI(builder);
            }

            _uiPanel = builder.Build();
        }

        public void OnGUI()
        {
            // IMGUI disabled
        }
    }
}
