using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.UI;

public sealed class GregUIController
{
    private readonly IGregLogger _logger;

    public GregUIController(IGregLogger logger)
    {
        _logger = logger.ForContext("UIController");
    }

    public void Initialize()
    {
        GregUIManager.Initialize();
        _logger?.Info("UI Controller (IMGUI) initialized.");
    }

    public void AddElement(string id, object element)
    {
        // Shimm for legacy code
    }

    public void RemoveElement(string id)
    {
        // Shimm for legacy code
    }
}
