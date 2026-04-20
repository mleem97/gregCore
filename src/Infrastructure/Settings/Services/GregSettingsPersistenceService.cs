using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.Settings.Models;

namespace gregCore.Infrastructure.Settings.Services;

public class GregSettingsPersistenceService
{
    private readonly IGregLogger _logger;
    private readonly GregKeybindRegistry _keybindRegistry;
    private readonly GregModSettingsService _modSettingsService;
    
    private readonly string _keybindsFile;
    private readonly string _settingsFile;

    public GregSettingsPersistenceService(
        IGregLogger logger, 
        GregKeybindRegistry keybindRegistry, 
        GregModSettingsService modSettingsService,
        IGregEventBus eventBus = null)
    {
        _logger = logger.ForContext("SettingsPersistence");
        _keybindRegistry = keybindRegistry;
        _modSettingsService = modSettingsService;

        var userData = global::MelonLoader.Utils.MelonEnvironment.UserDataDirectory;
        _keybindsFile = Path.Combine(userData, "gregCore_Keybinds.json");
        _settingsFile = Path.Combine(userData, "gregCore_ModSettings.json");

        if (eventBus != null)
        {
            eventBus.Subscribe("greg.SYSTEM.SettingsClosed", _ => SaveAll());
        }
    }

    public void SaveAll() => Save();

    public void Load()
    {
        try
        {
            _logger.Info("Lade Einstellungen und Keybinds...");
            if (File.Exists(_keybindsFile))
            {
                var content = File.ReadAllText(_keybindsFile);
                var entries = JsonConvert.DeserializeObject<List<KeybindEntry>>(content);
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        _keybindRegistry.Register(entry);
                    }
                    _logger.Info($"[Settings] {entries.Count} Keybinds geladen.");
                }
            }

            if (File.Exists(_settingsFile))
            {
                // Settings are polymorphic, handled via ApplyLoadedSettingsTo during registration
                _logger.Info("[Settings] Mod-Settings Persistence bereit.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("[Settings] Fehler beim Laden der Persistence.", ex);
        }
    }

    public void Save()
    {
        try
        {
            var keybinds = _keybindRegistry.GetAll().ToList();
            var kbContent = JsonConvert.SerializeObject(keybinds, Formatting.Indented);
            File.WriteAllText(_keybindsFile, kbContent);

            var settings = _modSettingsService.GetAll().ToDictionary(k => k.GetFullId(), v => GetValueObject(v));
            var stContent = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsFile, stContent);

            _logger.Info($"[Settings] {keybinds.Count} Keybinds und {settings.Count} Settings gespeichert.");
        }
        catch (Exception ex)
        {
            _logger.Error("[Settings] Fehler beim Speichern der Persistence.", ex);
        }
    }

    private object GetValueObject(BaseSettingEntry entry)
    {
        var type = entry.GetType();
        var prop = type.GetProperty("Value");
        return prop?.GetValue(entry);
    }

    public void ApplyLoadedSettingsTo(BaseSettingEntry newEntry)
    {
        // When a mod registers a setting AFTER load, we inject the Value from the JSON.
        try 
        {
            if (File.Exists(_settingsFile))
            {
                var content = File.ReadAllText(_settingsFile);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, Newtonsoft.Json.Linq.JToken>>(content);
                if (dict != null && dict.TryGetValue(newEntry.GetFullId(), out var jval))
                {
                    var type = newEntry.GetType();
                    var genericArgs = type.GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        var tgtType = genericArgs[0];
                        var finalVal = jval.ToObject(tgtType);
                        var prop = type.GetProperty("Value");
                        prop?.SetValue(newEntry, finalVal);
                    }
                }
            }
        } 
        catch (Exception ex) 
        {
            _logger.Error($"[Settings] Failed to apply delayed setting: {newEntry?.GetFullId()}", ex);
        }
    }
}
