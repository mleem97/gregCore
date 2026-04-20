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

    public void Toggle() => _showHud = !_showHud;

    public void OnGUI()
    {
        if (!_showHud) return;

        var conflicts = _keybindRegistry.GetAll().Where(k => k.HasConflict).ToList();
        if (conflicts.Count == 0) return;

        GUI.Box(new Rect(10, 10, 300, 40 + (conflicts.Count * 20)), "gregCore: Keybind-Konflikte!");
        
        int y = 40;
        foreach (var conflict in conflicts)
        {
            GUI.Label(new Rect(20, y, 280, 20), $"<color=red>{conflict.DisplayName} ({conflict.CurrentKey})</color>");
            y += 20;
        }
    }
}
