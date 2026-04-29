using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.Infrastructure.Settings.Models;
using gregCore.Infrastructure.Plugins;
using gregCore.Core.Abstractions;
using Il2CppTMPro;

namespace gregCore.Infrastructure.Settings.Services
{
    public class GregSettingsUiBridge
    {
        private readonly IGregLogger _logger;
        private readonly GregModSettingsService _settingsService;
        private readonly GregKeybindRegistry _keybindRegistry;
        private readonly GregInputBindingService _inputBindingService;
        private readonly GregPluginRegistry _pluginRegistry;
        private VisualElement? _root;
        private TextField? _searchInput;
        private VisualElement? _contentContainer;

        public GregSettingsUiBridge(
            IGregLogger logger,
            GregModSettingsService settingsService,
            GregKeybindRegistry keybindRegistry,
            GregInputBindingService inputBindingService,
            GregPluginRegistry pluginRegistry)
        {
            _logger = logger.ForContext("SettingsUiBridge");
            _settingsService = settingsService;
            _keybindRegistry = keybindRegistry;
            _inputBindingService = inputBindingService;
            _pluginRegistry = pluginRegistry;
        }

        public void BuildModSettingsPanel(GameObject panelGo)
        {
            var doc = panelGo.GetComponent<UIDocument>();
            if (doc == null) doc = panelGo.AddComponent<UIDocument>();
            if (doc.panelSettings == null)
            {
                var settings = ScriptableObject.CreateInstance<PanelSettings>();
                settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                settings.referenceResolution = new Vector2Int(1920, 1080);
                settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                settings.match = 0.5f;
                doc.panelSettings = settings;
            }

            _root = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Column,
                    backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.96f),
                    paddingTop = 10,
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10
                }
            };
            doc.rootVisualElement.Add(_root);

            // Scroll view
            var scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    marginTop = 10
                }
            };
            _root.Add(scrollView);
            _contentContainer = scrollView;

            // Search bar
            AddSearchBar();
            
            // Populate mods
            RefreshUi();
        }

        private void AddSearchBar()
        {
            _searchInput = new TextField
            {
                placeholderText = "Suche nach Mods oder Keybinds...",
                style =
                {
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                    color = Color.white,
                    height = 30,
                    marginBottom = 10
                }
            };
            _searchInput.RegisterValueChangedCallback<string>(evt => RefreshUi(evt.newValue));
            _root!.Add(_searchInput);
        }

        public void RefreshUi(string query = "")
        {
            if (_contentContainer == null) return;

            _contentContainer.Clear();

            var mods = _pluginRegistry.GetAllRegisteredMods();
            foreach (var mod in mods)
            {
                if (!string.IsNullOrEmpty(query) && !mod.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                    continue;

                AddModHeader(mod.Name, mod.Version);

                // Add Settings
                var settings = _settingsService.GetByMod(mod.ModId);
                foreach (var setting in settings)
                {
                    AddSettingEntry(setting);
                }

                // Add Keybinds
                var keybinds = _keybindRegistry.GetByMod(mod.ModId);
                foreach (var keybind in keybinds)
                {
                    AddKeybindEntry(keybind);
                }
            }
        }

        private void AddModHeader(string name, string version)
        {
            var header = new Label($"{name} (v{version})")
            {
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = Color.white,
                    marginTop = 10,
                    marginBottom = 6
                }
            };
            _contentContainer!.Add(header);
        }

        private void AddSettingEntry(BaseSettingEntry setting)
        {
            var entry = new Label($"  {setting.DisplayName}: {GetValue(setting)}")
            {
                style =
                {
                    fontSize = 14,
                    color = Color.cyan,
                    marginBottom = 3,
                    marginLeft = 10
                }
            };
            _contentContainer!.Add(entry);
        }

        private void AddKeybindEntry(KeybindEntry keybind)
        {
            var conflictText = keybind.HasConflict ? " [KONFLIKT]" : "";
            var entry = new Label($"  {keybind.DisplayName}: {keybind.CurrentKey}{conflictText}")
            {
                style =
                {
                    fontSize = 14,
                    color = keybind.HasConflict ? Color.red : Color.yellow,
                    marginBottom = 3,
                    marginLeft = 10
                }
            };
            _contentContainer!.Add(entry);
        }

        private string GetValue(BaseSettingEntry entry)
        {
            var type = entry.GetType();
            var prop = type.GetProperty("Value");
            return prop?.GetValue(entry)?.ToString() ?? "N/A";
        }
    }
}
