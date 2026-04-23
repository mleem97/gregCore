using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Settings.Services;

public class GregHudService
{
    private readonly IGregLogger _logger;
    private readonly GregKeybindRegistry _keybindRegistry;
    private bool _showHud = false;

    public GregHudService(IGregLogger logger, GregKeybindRegistry keybindRegistry)
    {
        _logger = logger.ForContext("HudService");
        _keybindRegistry = keybindRegistry;
    }

    private GameObject? _hudPanel;

    public void Toggle() 
    {
        _showHud = !_showHud;
        if (_showHud && _hudPanel == null)
        {
            BuildUI();
        }
        gregCore.UI.GregUIManager.SetPanelActive("HUD", _showHud);
    }

    private void BuildUI()
    {
        var conflicts = _keybindRegistry.GetAll().Where(k => k.HasConflict).ToList();
        var builder = gregCore.UI.GregUIBuilder.Create("HUD")
            .SetSize(300, 40 + (conflicts.Count * 20));
            
        builder.AddLabel("gregCore: Keybind-Konflikte!");
        foreach (var conflict in conflicts)
        {
            builder.AddLabel($"{conflict.DisplayName} ({conflict.CurrentKey})");
        }
        
        _hudPanel = builder.Build();
        var rt = _hudPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(10, -10);
    }

    public void OnGUI()
    {
        // IMGUI disabled
    }
}
