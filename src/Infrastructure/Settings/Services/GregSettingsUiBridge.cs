using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using gregCore.Infrastructure.Settings.Models;
using gregCore.Core.Abstractions;
using Il2CppTMPro;

namespace gregCore.Infrastructure.Settings.Services;

public class GregSettingsUiBridge
{
    private readonly IGregLogger _logger;
    private readonly GregModSettingsService _settingsService;
    private readonly GregKeybindRegistry _keybindRegistry;
    private readonly GregInputBindingService _inputBindingService;
    private readonly GregPluginRegistry _pluginRegistry;

    private GameObject _mainPanel;
    private InputField _searchInput;
    private Transform _contentContainer;

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

    public void BuildModSettingsPanel(GameObject panel)
    {
        _mainPanel = panel;
        _logger.Info("Baue Mod-Settings UI...");

        // 1. Setup ScrollView
        var scrollObj = new GameObject("ModSettingsScrollView");
        scrollObj.transform.SetParent(panel.transform, false);
        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        _contentContainer = content.transform;
        
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 10;
        vlg.padding = new RectOffset(20, 20, 20, 20);
        
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = content.GetComponent<RectTransform>();

        // 2. Add Search Bar
        AddSearchBar(content.transform);

        // 3. Populate Mods
        RefreshUi();
    }

    private void AddSearchBar(Transform parent)
    {
        var searchObj = new GameObject("SearchBar");
        searchObj.transform.SetParent(parent, false);
        _searchInput = searchObj.AddComponent<InputField>();
        _searchInput.onValueChanged.AddListener(new Action<string>(query => RefreshUi(query)));
        
        var placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(searchObj.transform, false);
        var placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = "Suche nach Mods oder Keybinds...";
        placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        placeholderText.color = Color.gray;
        
        _searchInput.placeholder = placeholderText;
    }

    public void RefreshUi(string query = "")
    {
        if (_contentContainer == null) return;

        // Clear existing (except search bar)
        foreach (Transform child in _contentContainer)
        {
            if (child.name == "SearchBar") continue;
            UnityEngine.Object.Destroy(child.gameObject);
        }

        var mods = _pluginRegistry.GetAllRegisteredMods();
        foreach (var mod in mods)
        {
            if (!string.IsNullOrEmpty(query) && !mod.Name.ToLowerInvariant().Contains(query.ToLowerInvariant()))
                continue;

            AddModHeader(mod);
            
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

    private void AddModHeader(ModMetadata mod)
    {
        var headerObj = new GameObject($"Header_{mod.ModId}");
        headerObj.transform.SetParent(_contentContainer, false);
        var text = headerObj.AddComponent<Text>();
        text.text = $"{mod.Name} (v{mod.Version})";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
    }

    private void AddSettingEntry(BaseSettingEntry setting)
    {
        var entryObj = new GameObject($"Setting_{setting.GetFullId()}");
        entryObj.transform.SetParent(_contentContainer, false);
        var text = entryObj.AddComponent<Text>();
        text.text = $"  {setting.DisplayName}: {GetValue(setting)}";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 18;
        text.color = Color.cyan;
    }

    private void AddKeybindEntry(KeybindEntry keybind)
    {
        var entryObj = new GameObject($"Keybind_{keybind.GetFullId()}");
        entryObj.transform.SetParent(_contentContainer, false);
        var text = entryObj.AddComponent<Text>();
        var conflictText = keybind.HasConflict ? " <color=red>[KONFLIKT]</color>" : "";
        text.text = $"  {keybind.DisplayName}: {keybind.CurrentKey}{conflictText}";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 18;
        text.color = keybind.HasConflict ? Color.red : Color.yellow;
        text.supportRichText = true;
    }

    private string GetValue(BaseSettingEntry entry)
    {
        var type = entry.GetType();
        var prop = type.GetProperty("Value");
        return prop?.GetValue(entry)?.ToString() ?? "N/A";
    }
}
