using System;
using System.Collections.Generic;
using System.Linq;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.Settings.Models;
using gregCore.Infrastructure.Settings.Services;

namespace gregCore.Infrastructure.Settings;

public class GregModSettingsService
{
    private readonly Dictionary<string, BaseSettingEntry> _settings = new();
    private readonly IGregLogger _logger;
    private GregSettingsPersistenceService _persistence;

    public GregModSettingsService(IGregLogger logger)
    {
        _logger = logger.ForContext("ModSettingsService");
    }

    public void SetPersistence(GregSettingsPersistenceService persistence)
    {
        _persistence = persistence;
    }

    public void Register<T>(SettingEntry<T> entry)
    {
        var id = entry.GetFullId();
        
        if (_settings.TryGetValue(id, out var existingBase) && existingBase is SettingEntry<T> existing)
        {
            existing.DisplayName = entry.DisplayName;
            existing.Description = entry.Description;
            existing.Category = entry.Category;
            existing.OnValueChanged = entry.OnValueChanged;
            existing.DefaultValue = entry.DefaultValue;
        }
        else
        {
            _settings[id] = entry;
            // First time registration, so value is default
            if (EqualityComparer<T>.Default.Equals(entry.Value, default(T)))
            {
                entry.Value = entry.DefaultValue;
            }
            
            _persistence?.ApplyLoadedSettingsTo(entry);
        }
        
        _logger.Info($"Setting registriert: {entry.DisplayName} [Mod: {entry.ModId}, Wert: {entry.Value}]");
    }

    public SettingEntry<T> Get<T>(string modId, string settingId)
    {
        if (_settings.TryGetValue($"{modId}.{settingId}", out var entry) && entry is SettingEntry<T> typedEntry)
        {
            return typedEntry;
        }
        return null;
    }

    public void UpdateSetting<T>(string modId, string settingId, T newValue)
    {
        var entry = Get<T>(modId, settingId);
        if (entry != null)
        {
            entry.Value = newValue;
            entry.OnValueChanged?.Invoke(newValue);
            _persistence?.SaveAll();
            _logger.Info($"Setting aktualisiert: {modId}.{settingId} -> {newValue}");
        }
    }

    public void ResetToDefault(string modId, string settingId)
    {
        var id = $"{modId}.{settingId}";
        if (_settings.TryGetValue(id, out var entry))
        {
            // We need to use reflection here because we don't know the type T
            var entryType = entry.GetType();
            var defaultValueField = entryType.GetProperty("DefaultValue");
            var valueField = entryType.GetProperty("Value");
            
            if (defaultValueField != null && valueField != null)
            {
                var defaultValue = defaultValueField.GetValue(entry);
                valueField.SetValue(entry, defaultValue);
                
                var onValueChangedField = entryType.GetProperty("OnValueChanged");
                if (onValueChangedField != null)
                {
                    var callback = onValueChangedField.GetValue(entry) as Delegate;
                    callback?.DynamicInvoke(defaultValue);
                }
                
                _persistence?.SaveAll();
                _logger.Info($"Setting auf Default zurückgesetzt: {id}");
            }
        }
    }

    public IEnumerable<BaseSettingEntry> GetAll() => _settings.Values;

    public IEnumerable<BaseSettingEntry> GetByMod(string modId) => _settings.Values.Where(s => s.ModId == modId);

    public IEnumerable<BaseSettingEntry> Search(string query)
    {
        if (string.IsNullOrEmpty(query)) return _settings.Values;
        
        var q = query.ToLowerInvariant();
        return _settings.Values.Where(s => 
            s.DisplayName.ToLowerInvariant().Contains(q) || 
            s.ModId.ToLowerInvariant().Contains(q) ||
            (s.Category != null && s.Category.ToLowerInvariant().Contains(q)));
    }
}
