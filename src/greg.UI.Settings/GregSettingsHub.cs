using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MelonLoader;
using gregCore.API;
using gregCore.UI;
using gregCore.Infrastructure.Social;

namespace greg.UI.Settings
{
    public class GregSettingsHub : MonoBehaviour
    {
        public GregSettingsHub(IntPtr ptr) : base(ptr) { }

        private static GregSettingsHub? _instance;
        private GregUIBuilder? _uiBuilder;
        private int _selectedTabIndex = 0;
        private Vector2 _sidebarScroll;

        private class TabData
        {
            public string Id = string.Empty;
            public string Label = string.Empty;
            public Action<GregUIBuilder>? BuildFn;
        }
        
        private static readonly List<TabData> _tabs = new();

        public static void Initialize()
        {
            if (_instance == null)
            {
                var go = new GameObject("GregSettingsHub_Host");
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
            RegisterTab("greg.core", "Core Framework", builder =>
            {
                builder.AddHeadline("GREGCORE SETTINGS")
                       .AddLabel("v1.1.0-F7 | Premium IMGUI Native")
                       .AddSpacer(15)
                       
                       .AddHeadline("Social & Integration")
                       .AddSwitch("Discord Rich Presence", true, v => {
                           if (v) DiscordService.Initialize();
                           else DiscordService.Shutdown();
                       })
                       
                       .AddSpacer(10)
                       .AddHeadline("Logging & Debug")
                       .AddSwitch("Verbose Console Output", false, v => { })
                       .AddSwitch("Detailed Hook Tracing", false, v => { })
                       
                       .AddSpacer(10)
                       .AddHeadline("UI Configuration")
                       .AddSlider("UI Scale (Beta)", 0.5f, 2.0f, 1.0f, v => { })
                       .AddButton("Reset UI State", () => { })
                       .AddButton("Dump Scene Hierarchy", () => { });
            });
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.f8Key.wasPressedThisFrame)
            {
                Toggle();
            }
            if (_uiBuilder != null && _uiBuilder.IsVisible && keyboard.escapeKey.wasPressedThisFrame)
            {
                _uiBuilder.IsVisible = false;
            }
        }

        private void OnGUI()
        {
            if (_uiBuilder == null || !_uiBuilder.IsVisible) return;

            GregImGui.EnsureInitialized();

            // Window Layout
            float width = 850;
            float height = 600;
            var winRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

            // 1. Frame & Header
            GregImGui.DrawWindowFrame(winRect, "Mod Configuration Hub");

            // 2. Sidebar (Full Height under header)
            float sidebarWidth = 220;
            var sidebarRect = new Rect(winRect.x, winRect.y + 50, sidebarWidth, winRect.height - 50);
            
            // Sidebar Background
            GUI.Box(sidebarRect, "", GregImGui.stSidebar);

            // Scrollable List
            _sidebarScroll = GUI.BeginScrollView(
                sidebarRect,
                _sidebarScroll,
                new Rect(0, 0, sidebarWidth - 10, _tabs.Count * 50 + 20),
                GUIStyle.none,
                GUIStyle.none // No scrollbars for cleaner sidebar
            );

            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                var tabRect = new Rect(0, i * 50, sidebarWidth, 50);
                
                var isSelected = (i == _selectedTabIndex);
                
                // Sidebar button style (tab-like)
                var style = isSelected ? GregImGui.stTabActive : GregImGui.stTab;
                if (GUI.Button(tabRect, "     " + tab.Label, style))
                {
                    if (_selectedTabIndex != i)
                    {
                        _selectedTabIndex = i;
                        BuildTabContent();
                    }
                }

                // Active Indicator (Blue vertical bar)
                if (isSelected)
                {
                    GregImGui.DrawRect(new Rect(tabRect.x, tabRect.y, 4, tabRect.height), GregImGui.ColorAccent);
                }
            }
            GUI.EndScrollView();

            // 3. Content Area
            var contentRect = new Rect(winRect.x + sidebarWidth + 20, winRect.y + 70, winRect.width - sidebarWidth - 40, winRect.height - 90);
            
            if (_selectedTabIndex >= 0 && _selectedTabIndex < _tabs.Count)
            {
                _uiBuilder.SetContentArea(contentRect);
                _uiBuilder.DrawContent();
            }
        }

        public void Toggle()
        {
            if (_uiBuilder == null) BuildUI();
            _uiBuilder?.Toggle();
            
            if (_uiBuilder != null && _uiBuilder.IsVisible)
            {
                BuildTabContent();
            }
        }

        private void BuildUI()
        {
            _uiBuilder = GregUIBuilder.CreateTablet("Settings Hub").Build();
        }

        private void BuildTabContent()
        {
            if (_uiBuilder == null) return;
            if (_selectedTabIndex < 0 || _selectedTabIndex >= _tabs.Count) return;

            _uiBuilder.ResetActions();
            _tabs[_selectedTabIndex].BuildFn?.Invoke(_uiBuilder);
        }
    }
}
