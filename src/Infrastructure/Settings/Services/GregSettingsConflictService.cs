using System.Collections.Generic;
using System.Linq;
using gregCore.Infrastructure.Settings.Models;
using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Settings.Services;

public class GregSettingsConflictService
{
    private readonly IGregLogger _logger;
    private readonly GregKeybindRegistry _keybindRegistry;
    private readonly GregModSettingsService _modSettingsService;

    public GregSettingsConflictService(IGregLogger logger, GregKeybindRegistry keybindRegistry, GregModSettingsService modSettingsService)
    {
        _logger = logger.ForContext("SettingsConflictService");
        _keybindRegistry = keybindRegistry;
        _modSettingsService = modSettingsService;
    }

    public void ValidateAll()
    {
        _logger.Info("Validiere Einstellungen und Keybinds...");
        _keybindRegistry.CheckConflicts();

        // 1. Keybind Conflicts
        var conflicts = _keybindRegistry.GetAll().Where(k => k.HasConflict).ToList();
        if (conflicts.Any())
        {
            var groups = conflicts.GroupBy(k => k.CurrentKey);
            foreach (var group in groups)
            {
                var actions = string.Join(", ", group.Select(x => x.GetFullId()));
                _logger.Warning($"[Keybind-Konflikt] Taste '{group.Key}' wird mehrfach verwendet: {actions}");
            }
        }

        // 2. Missing Defaults or IDs
        foreach (var keybind in _keybindRegistry.GetAll())
        {
            if (string.IsNullOrEmpty(keybind.ModId) || string.IsNullOrEmpty(keybind.ActionId))
            {
                _logger.Error($"[Registrierungsfehler] Keybind ohne ModId oder ActionId gefunden: {keybind.DisplayName}");
            }
        }

        foreach (var setting in _modSettingsService.GetAll())
        {
            if (string.IsNullOrEmpty(setting.ModId) || string.IsNullOrEmpty(setting.SettingId))
            {
                _logger.Error($"[Registrierungsfehler] Setting ohne ModId oder SettingId gefunden: {setting.DisplayName}");
            }
        }
        
        _logger.Info("Validierung abgeschlossen.");
    }
}
