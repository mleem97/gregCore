using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.UI;

/// <summary>
/// Central UI controller for GregCore.
/// Provides initialization and management for the UI Toolkit-based interface.
/// </summary>
public sealed class GregUIController
{
    private readonly IGregLogger _logger;

    public GregUIController(IGregLogger logger)
    {
        _logger = logger.ForContext("UIController");
    }

    public void Initialize()
    {
        try
        {
            GregCanvasManager.Instance.Initialize();
            GregUILayerManager.Instance.Initialize();
            GregUIManager.Initialize();
            GregNotificationManager.Initialize();
            GregTooltipManager.Initialize();
            _logger?.Info("UI Controller (UI Toolkit) initialized.");
        }
        catch (Exception ex)
        {
            _logger?.Error($"UI Controller initialization failed: {ex.Message}");
        }
    }

    public void AddElement(string id, object element)
    {
        // Legacy shim - UI Toolkit elements are added directly to VisualElement trees
    }

    public void RemoveElement(string id)
    {
        // Legacy shim
    }
}
