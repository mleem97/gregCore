using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using MelonLoader;
using gregCore.Infrastructure.Logging;
using gregCore.UI;
using gregCore.Infrastructure.Social;

namespace greg.UI.Settings
{
    public class GregSettingsHub : MonoBehaviour
    {
        public GregSettingsHub(IntPtr ptr) : base(ptr) { }

        private static GregSettingsHub? _instance;
        private GregPanelBuilder? _panelBuilder;
        private VisualElement? _sidebar;
        private VisualElement? _sidebarContent;
        private VisualElement? _contentArea;
        private int _selectedTabIndex = 0;
        private bool _isVisible;

        private class TabData
        {
            public string Id = string.Empty;
            public string Label = string.Empty;
            public Action<GregPanelBuilder>? BuildFn;
        }

        private static readonly List<TabData> _tabs = new();

        public static void Initialize()
        {
            if (_instance == null)
            {
                var go = new GameObject("GregSettingsHub_Host");
                _instance = go.AddComponent(Il2CppInterop.Runtime.Il2CppType.Of<GregSettingsHub>()).Cast<GregSettingsHub>();
                UnityEngine.Object.DontDestroyOnLoad(go);

                RegisterStandardTabs();
            }
        }

        public static GregSettingsHub? Instance => _instance;

        public static bool IsVisibleNow()
        {
            try { return _instance != null && _instance._isVisible; }
            catch { return false; }
        }

        // Find settings tab by mod menu ID or mod name
        // (e.g. menu "backplanes" -> tab "backplanes.settings").
        public static string FindTabForMenu(string menuId, string modName)
        {
            try
            {
                if (_tabs == null || _tabs.Count == 0) return null;
                foreach (var tab in _tabs)
                {
                    if (tab == null || string.IsNullOrEmpty(tab.Id)) continue;
                    if (!string.IsNullOrEmpty(menuId)
                        && (string.Equals(tab.Id, menuId, StringComparison.OrdinalIgnoreCase)
                            || tab.Id.StartsWith(menuId + ".", StringComparison.OrdinalIgnoreCase)))
                        return tab.Id;
                }

                if (!string.IsNullOrEmpty(modName))
                {
                    foreach (var tab in _tabs)
                    {
                        if (tab == null) continue;
                        if (string.Equals(tab.Label, modName, StringComparison.OrdinalIgnoreCase))
                            return tab.Id;
                    }
                }
            }
            catch { }
            return null;
        }

        // Open settings hub, optionally directly on a tab.
        public static bool ShowTab(string tabId)
        {
            try
            {
                if (_instance == null) Initialize();
                if (_instance == null) return false;
                if (!string.IsNullOrEmpty(tabId))
                {
                    for (int i = 0; i < _tabs.Count; i++)
                    {
                        if (_tabs[i] != null && string.Equals(_tabs[i].Id, tabId, StringComparison.OrdinalIgnoreCase))
                        {
                            _instance._selectedTabIndex = i;
                            break;
                        }
                    }
                }

                _instance.Show();
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregSettingsHub] ShowTab failed: {ex.GetBaseException().Message}");
                return false;
            }
        }

        public static void RegisterTab(string tabId, string label, Action<GregPanelBuilder> buildFn)
        {
            var existing = _tabs.FindIndex(t => t.Id == tabId);
            if (existing >= 0)
            {
                _tabs[existing] = new TabData { Id = tabId, Label = label, BuildFn = buildFn };
                return;
            }

            _tabs.Add(new TabData { Id = tabId, Label = label, BuildFn = buildFn });
        }

        public static void RegisterTab(string tabId, string label, Action<GregUIBuilder> buildFn)
        {
            RegisterTab(tabId, label, panelBuilder =>
            {
                var legacyBuilder = GregUIBuilder.Create(label);
                InjectPanelBuilder(legacyBuilder, panelBuilder);
                buildFn(legacyBuilder);
            });
        }

        public static void UnregisterTab(string tabId)
        {
            _tabs.RemoveAll(t => t.Id == tabId);
        }

        private static void RegisterStandardTabs()
        {
            RegisterTab("greg.core", "Core Framework", (Action<GregPanelBuilder>)(builder =>
            {
                builder
                    .AddHeadline("GREGCORE SETTINGS")
                    .AddLabel("v1.2.0-UI-Toolkit | Modern Native")
                    .AddSpacer(15)

                    .AddHeadline("Social & Integration")
                    .AddSwitch("Discord Rich Presence", false, v =>
                    {
                        if (v) DiscordService.Initialize();
                        else DiscordService.Shutdown();
                    })

                    .AddSpacer(10)
                    .AddHeadline("Logging & Debug")
                    .AddSwitch("Verbose Console Output", gregCore.Infrastructure.Logging.DevLog.Verbose, v =>
                    {
                        gregCore.Infrastructure.Logging.DevLog.Verbose = v;
                    })
                    .AddSwitch("Detailed Hook Tracing", false, v => { })

                    .AddSpacer(10)
                    .AddHeadline("UI Configuration")
                    .AddSlider("UI Scale (Beta)", 0.5f, 2.0f, 1.0f, v => { })
                    .AddButton("Reset UI State", () => { })
                    .AddButton("Dump Scene Hierarchy", () => { });
                    }));
        }

        private static void InjectPanelBuilder(GregUIBuilder legacyBuilder, GregPanelBuilder panelBuilder)
        {
            var field = typeof(GregUIBuilder).GetField("_panelBuilder", BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(legacyBuilder, panelBuilder);
        }

        private void Update()
        {
            try
            {
                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                if (keyboard.f8Key.wasPressedThisFrame)
                {
                    Toggle();
                }
                if (_isVisible && keyboard.escapeKey.wasPressedThisFrame)
                {
                    Hide();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregSettingsHub] Update error: {ex.Message}");
            }
        }

        public void Toggle()
        {
            if (_isVisible) Hide();
            else Show();
        }

        public void Show()
        {
            if (_panelBuilder == null) BuildUI();
            _panelBuilder?.Show();
            _isVisible = true;
            BuildTabContent();
            try
            {
                var root = _panelBuilder?.Root;
                if (root != null) root.MarkDirtyRepaint();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        public void Hide()
        {
            _panelBuilder?.Hide();
            _isVisible = false;
        }

        private void BuildUI()
        {
            try
            {
                _panelBuilder = GregPanelBuilder.Create("Mod Configuration Hub")
                    .SetSize(850, 600)
                    .Build(GregUILayerType.Panel);

                var root = _panelBuilder.Root;
                if (root == null) return;

                var defaultScroll = root.Q<ScrollView>("PanelContent");
                if (defaultScroll != null)
                {
                    defaultScroll.RemoveFromHierarchy();
                }

                var body = new VisualElement();
                body.style.flexGrow = 1;
                body.style.flexShrink = 1;
                body.style.minHeight = 0;
                body.style.flexDirection = FlexDirection.Row;
                root.Add(body);

                _sidebar = new VisualElement();
                _sidebar.name = "SettingsSidebar";
                _sidebar.style.width = 220;
                _sidebar.style.flexShrink = 0;
                _sidebar.style.backgroundColor = new Color(0.08f, 0.08f, 0.1f, 1f);
                _sidebar.style.borderRightWidth = 1;
                _sidebar.style.borderRightColor = GregUITheme.NeutralBorder;
                _sidebar.style.paddingTop = 10;
                _sidebar.style.paddingBottom = 10;
                body.Add(_sidebar);

                // Scrollable tab list (many tabs must not push content out).
                var sidebarScroll = new ScrollView(ScrollViewMode.Vertical);
                sidebarScroll.name = "SettingsSidebarScroll";
                sidebarScroll.style.flexGrow = 1;
                sidebarScroll.style.flexShrink = 1;
                sidebarScroll.style.minHeight = 0;
                sidebarScroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
                sidebarScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                _sidebar.Add(sidebarScroll);
                _sidebarContent = sidebarScroll.contentContainer;

                // Scrollable tab content (long settings pages scroll
                // instead of overflowing past the panel edge).
                var contentScroll = new ScrollView(ScrollViewMode.Vertical);
                contentScroll.name = "SettingsContentScroll";
                contentScroll.style.flexGrow = 1;
                contentScroll.style.flexShrink = 1;
                contentScroll.style.minHeight = 0;
                contentScroll.verticalScrollerVisibility = ScrollerVisibility.Auto;
                contentScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                body.Add(contentScroll);

                _contentArea = contentScroll.contentContainer;
                _contentArea.name = "SettingsContent";
                _contentArea.style.paddingLeft = GregUITheme.Padding;
                _contentArea.style.paddingRight = GregUITheme.Padding;
                _contentArea.style.paddingTop = GregUITheme.Padding;
                _contentArea.style.paddingBottom = GregUITheme.Padding;
                // NOTE: no flexGrow here — the ScrollView owns the sizing;
                // the content container must size to its children.

                BuildSidebar();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregSettingsHub] BuildUI failed: {ex.Message}");
            }
        }

        private void BuildSidebar()
        {
            var target = _sidebarContent ?? _sidebar;
            if (target == null) return;
            target.Clear();

            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                var index = i;

                var btn = new Button();
                btn.text = tab.Label;
                btn.style.height = 44;
                btn.style.backgroundColor = index == _selectedTabIndex
                    ? new Color(0.38f, 0.65f, 0.95f, 0.15f)
                    : Color.clear;
                btn.style.color = index == _selectedTabIndex
                    ? GregUITheme.PrimaryAccent
                    : new Color(0.65f, 0.65f, 0.7f);
                btn.style.unityFontStyleAndWeight = index == _selectedTabIndex ? FontStyle.Bold : FontStyle.Normal;
                btn.style.borderLeftWidth = index == _selectedTabIndex ? 4 : 0;
                btn.style.borderLeftColor = GregUITheme.PrimaryAccent;
                btn.style.borderRightWidth = 0;
                btn.style.borderTopWidth = 0;
                btn.style.borderBottomWidth = 0;
                btn.style.paddingLeft = 20;
                btn.style.unityTextAlign = TextAnchor.MiddleLeft;
                btn.style.marginLeft = 0;
                btn.style.marginRight = 0;
                btn.style.marginTop = 0;
                btn.style.marginBottom = 0;
                btn.style.borderTopLeftRadius = 0;
                btn.style.borderTopRightRadius = 0;
                btn.style.borderBottomLeftRadius = 0;
                btn.style.borderBottomRightRadius = 0;

                btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ =>
                {
                    _selectedTabIndex = index;
                    BuildSidebar();
                    BuildTabContent();
                }));

                if (_sidebarContent != null) _sidebarContent.Add(btn);
                else _sidebar?.Add(btn);
            }
        }

        private void BuildTabContent()
        {
            if (_contentArea == null || _panelBuilder == null) return;
            _contentArea.Clear();

            if (_selectedTabIndex < 0 || _selectedTabIndex >= _tabs.Count) return;

            try
            {
                var tabBuilder = GregPanelBuilder.Create(_tabs[_selectedTabIndex].Label);
                var contentField = typeof(GregPanelBuilder).GetField("_contentContainer",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (contentField != null)
                {
                    contentField.SetValue(tabBuilder, _contentArea);
                }

                _tabs[_selectedTabIndex].BuildFn?.Invoke(tabBuilder);

                if (_contentArea.childCount > 0)
                {
                    GregCanvasManager.Instance.ApplyGameFont(_contentArea);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregSettingsHub] BuildTabContent failed: {ex.Message}");
            }
        }
    }
}
