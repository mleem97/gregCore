using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI;

public sealed class GregUIController
{
    private readonly IGregLogger _logger;
    private UIDocument _uiDocument;
    private VisualElement _root;
    private readonly Dictionary<string, VisualElement> _activeElements = new();

    public GregUIController(IGregLogger logger)
    {
        _logger = logger.ForContext("UIController");
    }

    public void Initialize()
    {
        try
        {
            var go = new GameObject("GregCore_UI");
            UnityEngine.Object.DontDestroyOnLoad(go);
            
            _uiDocument = go.AddComponent<UIDocument>();
            _uiDocument.panelSettings = CreateDefaultPanelSettings();
            _root = _uiDocument.rootVisualElement;
            
            _logger.Info("UI Toolkit initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to initialize UI Toolkit document.", ex);
        }
    }

    public void AddElement(string id, VisualElement element)
    {
        if (_activeElements.ContainsKey(id))
        {
            _root.Remove(_activeElements[id]);
        }
        
        _activeElements[id] = element;
        _root.Add(element);
    }

    public void RemoveElement(string id)
    {
        if (_activeElements.TryGetValue(id, out var element))
        {
            _root.Remove(element);
            _activeElements.Remove(id);
        }
    }

    private PanelSettings CreateDefaultPanelSettings()
    {
        // In a real IL2CPP environment, we might need to load this from a bundle or create it via Reflection if not available directly
        var settings = ScriptableObject.CreateInstance<PanelSettings>();
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        settings.referenceResolution = new Vector2Int(1920, 1080);
        settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        settings.match = 0.5f;
        return settings;
    }
}
