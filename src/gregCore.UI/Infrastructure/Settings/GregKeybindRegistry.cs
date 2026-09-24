using System;
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

    /// <summary>
    /// Keys the framework/game owns: never assigned, never taken from the
    /// requester silently — a request for one of these is auto-resolved away.
    /// Escape cancels edits and closes menus game-wide; F1 opens the gregCore hub.
    /// </summary>
    public static readonly HashSet<KeyCode> GameReservedKeys = new()
    {
        KeyCode.Escape,
        KeyCode.F1,
    };

    /// <summary>
    /// Preferred fallback order when the requested key is taken or reserved.
    /// High F-keys first (least likely bound by the game), then navigation keys.
    /// Note: only registered entries and <see cref="GameReservedKeys"/> are
    /// visible to the allocator — mods that never register stay invisible
    /// (see docs/modding/keybinds.md for the adoption roadmap).
    /// </summary>
    public static readonly IReadOnlyList<KeyCode> FallbackPool = new[]
    {
        KeyCode.F12, KeyCode.F11, KeyCode.F10, KeyCode.F9, KeyCode.F8,
        KeyCode.F7, KeyCode.F6, KeyCode.F5, KeyCode.F4, KeyCode.F3, KeyCode.F2,
        KeyCode.Insert, KeyCode.Home, KeyCode.End, KeyCode.Delete,
        KeyCode.PageUp, KeyCode.PageDown,
    };

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
            entry = existing;
        }
        else
        {
            _keybinds[id] = entry;
            if (entry.CurrentKey == KeyCode.None)
            {
                entry.CurrentKey = entry.DefaultKey;
            }
        }

        _logger.Info($"Keybind registered: {entry.DisplayName} [Mod: {entry.ModId}, Key: {entry.CurrentKey}]");
        AutoResolve(entry);
        CheckConflicts();
    }

    /// <summary>
    /// Moves the entry off its requested key when that key is game-reserved
    /// or already taken by another entry: first free key from
    /// <see cref="FallbackPool"/> wins, loudly logged and persisted like any
    /// CurrentKey. No-op when the requested key is free. When the pool is
    /// exhausted the entry keeps its key and <see cref="CheckConflicts"/>
    /// flags it as before.
    /// </summary>
    public void AutoResolve(KeybindEntry entry)
    {
        if (entry == null) return;
        if (entry.CurrentKey == KeyCode.None) return;
        if (!GameReservedKeys.Contains(entry.CurrentKey) && !IsTakenByOther(entry))
            return;

        string reason = GameReservedKeys.Contains(entry.CurrentKey)
            ? "game-reserved"
            : "already taken";
        var free = FindFreeKey(entry);
        if (!free.HasValue) return; // pool exhausted: CheckConflicts flags it

        var old = entry.CurrentKey;
        entry.CurrentKey = free.Value;
        entry.AutoResolved = true;
        _logger.Warning($"Keybind auto-resolved ({reason}): {entry.GetFullId()} {old} -> {free.Value}. " +
            "Change it back in the settings if you prefer the collision.");
    }

    /// <summary>
    /// First free key from <see cref="FallbackPool"/>: not game-reserved and
    /// not taken by any entry except <paramref name="except"/> (which keeps
    /// its own key). Null when the pool is exhausted.
    /// </summary>
    public KeyCode? FindFreeKey(KeybindEntry? except = null)
    {
        string? exceptId = except != null ? except.GetFullId() : null;
        foreach (var candidate in FallbackPool)
        {
            if (GameReservedKeys.Contains(candidate)) continue;
            bool taken = false;
            foreach (var kv in _keybinds)
            {
                if (exceptId != null && string.Equals(kv.Key, exceptId, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (kv.Value != null && kv.Value.CurrentKey == candidate) { taken = true; break; }
            }
            if (!taken) return candidate;
        }
        return null;
    }

    private bool IsTakenByOther(KeybindEntry entry)
    {
        string id = entry.GetFullId();
        foreach (var kv in _keybinds)
        {
            if (string.Equals(kv.Key, id, StringComparison.OrdinalIgnoreCase)) continue;
            if (kv.Value != null && kv.Value.CurrentKey == entry.CurrentKey) return true;
        }
        return false;
    }

    public void Unregister(string modId, string actionId)
    {
        var id = $"{modId}.{actionId}";
        if (_keybinds.Remove(id))
        {
            _logger.Info($"Keybind removed: {id}");
            CheckConflicts();
        }
    }

    public KeybindEntry? Get(string modId, string actionId)
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
            _logger.Warning("Keybind conflicts detected! Please check the Settings menu.");
        }
    }
}
