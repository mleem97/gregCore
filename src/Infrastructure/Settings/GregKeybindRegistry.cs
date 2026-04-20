using System.Collections.Generic;
using System.Linq;
using gregCore.Infrastructure.Settings.Models;
using UnityEngine;

namespace gregCore.Infrastructure.Settings;

public class GregKeybindRegistry
{
    // ModId.ActionId -> KeybindEntry
    private readonly Dictionary<string, KeybindEntry> _keybinds = new();
    private readonly IGregLogger _logger;

    public GregKeybindRegistry(IGregLogger logger)
    {
        _logger = logger.ForContext("KeybindRegistry");
    }

    public void Register(KeybindEntry entry)
    {
        var id = entry.GetFullId();

        // If it already exists, meaning it was loaded from persistence earlier,
        // we just update its callbacks and metadata, but preserve CurrentKey.
        if (_keybinds.TryGetValue(id, out var existing))
        {
            existing.DisplayName = entry.DisplayName;
            existing.Description = entry.Description;
            existing.OnPress = entry.OnPress;
            existing.DefaultKey = entry.DefaultKey;
            existing.Category = entry.Category;
        }
        else
        {
            _keybinds[id] = entry;
            if (entry.CurrentKey == KeyCode.None)
            {
                entry.CurrentKey = entry.DefaultKey;
            }
        }

        _logger.Info($"Keybind registriert: {entry.DisplayName} [Mod: {entry.ModId}, Key: {entry.CurrentKey}]");
        CheckConflicts();
    }

    public void Unregister(string modId, string actionId)
    {
        var id = $"{modId}.{actionId}";
        if (_keybinds.Remove(id))
        {
            _logger.Info($"Keybind entfernt: {id}");
            CheckConflicts();
        }
    }

    public KeybindEntry Get(string modId, string actionId)
    {
        _keybinds.TryGetValue($"{modId}.{actionId}", out var entry);
        return entry;
    }

    public IEnumerable<KeybindEntry> GetAll() => _keybinds.Values;

    public IEnumerable<KeybindEntry> GetByMod(string modId) => _keybinds.Values.Where(k => k.ModId == modId);

    public void CheckConflicts()
    {
        // Reset conflict status
        foreach (var entry in _keybinds.Values)
        {
            entry.HasConflict = false;
        }

        // Group by CurrentKey (excluding KeyCode.None)
        var groups = _keybinds.Values
            .Where(k => k.CurrentKey != KeyCode.None)
            .GroupBy(k => k.CurrentKey)
            .Where(g => g.Count() > 1);

        bool foundConflict = false;
        foreach (var group in groups)
        {
            foundConflict = true;
            foreach (var entry in group)
            {
                entry.HasConflict = true;
            }
        }

        if (foundConflict)
        {
            _logger.Warning("Keybind-Konflikte erkannt! Bitte im Settings-Menü prüfen.");
        }
    }
}
