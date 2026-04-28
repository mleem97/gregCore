using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI;

public sealed class GregUIController
{
    private readonly IGregLogger _logger;
    private readonly Dictionary<string, VisualElement> _activeElements = new();

    public GregUIController(IGregLogger logger)
    {
        _logger = logger.ForContext("UIController");
    }

    public void Initialize()
    {
        try
        {
            GregUIManager.Initialize();
            _logger.Info("UI Toolkit Controller initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to initialize UI Toolkit Controller.", ex);
        }
    }

    public void AddElement(string id, VisualElement element)
    {
        if (_activeElements.ContainsKey(id))
        {
            GregUIManager.Root.Remove(_activeElements[id]);
        }
        
        _activeElements[id] = element;
        GregUIManager.Root.Add(element);
    }

    public void RemoveElement(string id)
    {
        if (_activeElements.TryGetValue(id, out var element))
        {
            GregUIManager.Root.Remove(element);
            _activeElements.Remove(id);
        }
    }
}
