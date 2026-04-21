using System;
using UnityEngine;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.Settings.Models;

namespace gregCore.Infrastructure.Settings.Services;

public class GregInputBindingService
{
    private readonly IGregLogger _logger;
    private readonly GregKeybindRegistry _keybindRegistry;
    private GregSettingsPersistenceService _persistence;

    public GregInputBindingService(IGregLogger logger, GregKeybindRegistry keybindRegistry)
    {
        _logger = logger.ForContext("InputBindingService");
        _keybindRegistry = keybindRegistry;
    }

    public void SetPersistence(GregSettingsPersistenceService persistence)
    {
        _persistence = persistence;
    }

    public bool Rebind(string modId, string actionId, KeyCode newKey)
    {
        var entry = _keybindRegistry.Get(modId, actionId);
        if (entry == null)
        {
            _logger.Error($"Rebind fehlgeschlagen: Keybind {modId}.{actionId} nicht gefunden.");
            return false;
        }

        var oldKey = entry.CurrentKey;
        entry.CurrentKey = newKey;
        
        _logger.Info($"Keybind geändert: {modId}.{actionId} von {oldKey} zu {newKey}");
        
        _keybindRegistry.CheckConflicts();
        _persistence?.SaveAll();
        
        return true;
    }

    public void ResetToDefault(string modId, string actionId)
    {
        var entry = _keybindRegistry.Get(modId, actionId);
        if (entry != null)
        {
            Rebind(modId, actionId, entry.DefaultKey);
        }
    }

    public void OnUpdate()
    {
        try
        {
            foreach (var keybind in _keybindRegistry.GetAll())
            {
                if (keybind.CurrentKey != KeyCode.None && keybind.OnPress != null)
                {
                    // Fallback using standard input manager as configured in gregCore.
                    // If the game restricts legacy input entirely, this would be swapped to InputSystem mapping.
                    if (Input.GetKeyDown(keybind.CurrentKey))
                    {
                        keybind.OnPress.Invoke();
                    }
                }
            }
        }
        catch (Exception)
        {
            // _logger.Error("Error checking keybinds", ex); // Too spammy for Update loop
        }
    }
}
