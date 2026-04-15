using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MelonLoader;

namespace greg.Sdk.Services;

public sealed class GregHotkeyInfo
{
    public string ModId { get; set; }
    public string ActionName { get; set; }
    public string KeyName { get; set; }
    public Key Key { get; set; }
    public bool Ctrl { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    public Action OnPressed { get; set; }
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Centralized Input Manager to handle mod hotkeys and prevent overlaps.
/// Allows users to disable specific tool hotkeys via the gregModConfigManager.
/// </summary>
public static class GregInputManagerService
{
    private static List<GregHotkeyInfo> _registeredHotkeys = new();

    public static void RegisterHotkey(string modId, string actionName, Key key, Action onPressed, bool ctrl = false, bool shift = false, bool alt = false)
    {
        // Check if already registered
        var existing = _registeredHotkeys.Find(h => h.ModId == modId && h.ActionName == actionName);
        if (existing != null)
        {
            existing.Key = key;
            existing.KeyName = key.ToString();
            existing.Ctrl = ctrl;
            existing.Shift = shift;
            existing.Alt = alt;
            existing.OnPressed = onPressed;
            return;
        }

        // Load enabled state from config
        bool isEnabled = GregConfigService.Get(modId, $"Hotkey_{actionName}_Enabled", true);

        _registeredHotkeys.Add(new GregHotkeyInfo
        {
            ModId = modId,
            ActionName = actionName,
            Key = key,
            KeyName = (ctrl ? "Ctrl+" : "") + (shift ? "Shift+" : "") + (alt ? "Alt+" : "") + key.ToString(),
            Ctrl = ctrl,
            Shift = shift,
            Alt = alt,
            OnPressed = onPressed,
            IsEnabled = isEnabled
        });

        MelonLogger.Msg($"[GregInput] Registered hotkey '{actionName}' for mod '{modId}' on key {key}.");
    }

    public static void Update()
    {
        if (Keyboard.current == null) return;

        foreach (var hotkey in _registeredHotkeys)
        {
            if (hotkey.IsEnabled && Keyboard.current[hotkey.Key].wasPressedThisFrame)
            {
                bool modifiersMatch = true;
                if (hotkey.Ctrl && !Keyboard.current.ctrlKey.isPressed) modifiersMatch = false;
                if (hotkey.Shift && !Keyboard.current.shiftKey.isPressed) modifiersMatch = false;
                if (hotkey.Alt && !Keyboard.current.altKey.isPressed) modifiersMatch = false;

                if (modifiersMatch)
                {
                    try
                    {
                        hotkey.OnPressed?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"[GregInput] Error executing hotkey '{hotkey.ActionName}' for {hotkey.ModId}: {ex.Message}");
                    }
                }
            }
        }
    }

    public static List<GregHotkeyInfo> GetHotkeys() => _registeredHotkeys;

    public static void SetHotkeyEnabled(string modId, string actionName, bool enabled)
    {
        var hotkey = _registeredHotkeys.Find(h => h.ModId == modId && h.ActionName == actionName);
        if (hotkey != null)
        {
            hotkey.IsEnabled = enabled;
            GregConfigService.Set(modId, $"Hotkey_{actionName}_Enabled", enabled);
        }
    }
}
