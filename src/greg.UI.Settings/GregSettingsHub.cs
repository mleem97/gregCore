using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.API;

namespace greg.UI.Settings
{
    public class GregUIBuilder
    {
        public GregUIBuilder AddLabel(string text)
        {
            GUILayout.Label(text);
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            bool newValue = GUILayout.Toggle(currentValue, label);
            if (newValue != currentValue)
            {
                onChanged?.Invoke(newValue);
            }
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            float newValue = GUILayout.HorizontalSlider(currentValue, min, max);
            GUILayout.Label(newValue.ToString("F1"), GUILayout.Width(40));
            GUILayout.EndHorizontal();
            
            if (Math.Abs(newValue - currentValue) > 0.01f)
            {
                onChanged?.Invoke(newValue);
            }
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick)
        {
            if (GUILayout.Button(label))
            {
                onClick?.Invoke();
            }
            return this;
        }
    }

    public class GregSettingsHub : MonoBehaviour
    {
        private static GregSettingsHub? _instance;
        private bool _isVisible = false;
        private int _selectedTab = 0;
        
        private class TabData
        {
            public string Id = string.Empty;
            public string Label = string.Empty;
            public Action<GregUIBuilder>? BuildFn;
        }
        
        private static readonly List<TabData> _tabs = new();
        private GUIStyle? _windowStyle;
        private GUIStyle? _tabStyle;
        private GregUIBuilder _builder = new GregUIBuilder();

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

        public static void RegisterTab(string tabId, string label, Action<GregUIBuilder> buildFn)
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
                builder.AddLabel("gregCore v1.0.0.33-pre")
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
                           .AddToggle("Build Mode Key: B", true, v => { })
                           .AddLabel($"Placed Racks: [unknown]")
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
                       .AddToggle("Disable Vanilla Save (expert!)", false, v => { })
                       .AddToggle("Save Grid State", true, v => { })
                       .AddToggle("Save Server State", true, v => { })
                       .AddToggle("Save Network State", true, v => { })
                       .AddToggle("Save Cable State", true, v => { });
                       
                var engine = greg.SaveEngine.GregSaveEngine.Instance;
                builder.AddLabel($"Last Save: [unknown]")
                       .AddLabel($"DB File: {(engine != null ? engine.DbPath : "None")}")
                       .AddButton("Save Now", () => engine?.SaveAll())
                       .AddButton("Open Save Folder", () => {
                           if (engine != null && !string.IsNullOrEmpty(engine.DbPath))
                           {
                               System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{engine.DbPath}\"");
                           }
                       });
            });

            RegisterTab("greg.lang", "Languages", builder =>
            {
                builder.AddLabel("Languages Registry (Lua, JS, Python, Go, Rust)");
            });

            RegisterTab("greg.debug", "Debug", builder =>
            {
                builder.AddLabel("Debug & Diagnose");
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                _isVisible = !_isVisible;
            }
            if (_isVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                _isVisible = false;
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            if (_windowStyle == null)
            {
                _windowStyle = GUI.skin.window;
                _tabStyle = GUI.skin.button;
            }

            GUI.Window(999, new Rect((Screen.width - 480) / 2, 100, 480, 500), (GUI.WindowFunction)DrawWindow, "gregCore Settings Hub");
        }

        private void DrawWindow(int id)
        {
            if (_tabs.Count == 0) return;

            GUILayout.BeginHorizontal();
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (GUILayout.Toggle(_selectedTab == i, _tabs[i].Label, _tabStyle))
                {
                    _selectedTab = i;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            if (_selectedTab >= 0 && _selectedTab < _tabs.Count)
            {
                _tabs[_selectedTab].BuildFn?.Invoke(_builder);
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
