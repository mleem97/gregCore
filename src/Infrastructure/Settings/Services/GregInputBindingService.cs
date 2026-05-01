using System;
using UnityEngine;
using UnityEngine.InputSystem;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.Settings.Models;

namespace gregCore.Infrastructure.Settings.Services;

public class GregInputBindingService
{
    private readonly IGregLogger _logger;
    private readonly GregKeybindRegistry _keybindRegistry;
    private GregSettingsPersistenceService _persistence = null!;

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

    private static bool WasKeyPressedThisFrame(KeyCode keyCode)
    {
        var kb = Keyboard.current;
        if (kb == null) return false;

        try
        {
            var key = KeyCodeToKey(keyCode);
            if (key.HasValue)
                return kb[key.Value].wasPressedThisFrame;
        }
        catch { }
        return false;
    }

    private static Key? KeyCodeToKey(KeyCode keyCode)
    {
        return keyCode switch
        {
            KeyCode.F1 => Key.F1,
            KeyCode.F2 => Key.F2,
            KeyCode.F3 => Key.F3,
            KeyCode.F4 => Key.F4,
            KeyCode.F5 => Key.F5,
            KeyCode.F6 => Key.F6,
            KeyCode.F7 => Key.F7,
            KeyCode.F8 => Key.F8,
            KeyCode.F9 => Key.F9,
            KeyCode.F10 => Key.F10,
            KeyCode.F11 => Key.F11,
            KeyCode.F12 => Key.F12,
            KeyCode.Escape => Key.Escape,
            KeyCode.Return or KeyCode.KeypadEnter => Key.Enter,
            KeyCode.Space => Key.Space,
            KeyCode.LeftShift => Key.LeftShift,
            KeyCode.RightShift => Key.RightShift,
            KeyCode.LeftControl => Key.LeftCtrl,
            KeyCode.RightControl => Key.RightCtrl,
            KeyCode.LeftAlt => Key.LeftAlt,
            KeyCode.RightAlt => Key.RightAlt,
            KeyCode.Tab => Key.Tab,
            KeyCode.Backspace => Key.Backspace,
            KeyCode.Delete => Key.Delete,
            KeyCode.Home => Key.Home,
            KeyCode.End => Key.End,
            KeyCode.PageUp => Key.PageUp,
            KeyCode.PageDown => Key.PageDown,
            KeyCode.UpArrow => Key.UpArrow,
            KeyCode.DownArrow => Key.DownArrow,
            KeyCode.LeftArrow => Key.LeftArrow,
            KeyCode.RightArrow => Key.RightArrow,
            KeyCode.A => Key.A,
            KeyCode.B => Key.B,
            KeyCode.C => Key.C,
            KeyCode.D => Key.D,
            KeyCode.E => Key.E,
            KeyCode.F => Key.F,
            KeyCode.G => Key.G,
            KeyCode.H => Key.H,
            KeyCode.I => Key.I,
            KeyCode.J => Key.J,
            KeyCode.K => Key.K,
            KeyCode.L => Key.L,
            KeyCode.M => Key.M,
            KeyCode.N => Key.N,
            KeyCode.O => Key.O,
            KeyCode.P => Key.P,
            KeyCode.Q => Key.Q,
            KeyCode.R => Key.R,
            KeyCode.S => Key.S,
            KeyCode.T => Key.T,
            KeyCode.U => Key.U,
            KeyCode.V => Key.V,
            KeyCode.W => Key.W,
            KeyCode.X => Key.X,
            KeyCode.Y => Key.Y,
            KeyCode.Z => Key.Z,
            KeyCode.Alpha0 => Key.Digit0,
            KeyCode.Alpha1 => Key.Digit1,
            KeyCode.Alpha2 => Key.Digit2,
            KeyCode.Alpha3 => Key.Digit3,
            KeyCode.Alpha4 => Key.Digit4,
            KeyCode.Alpha5 => Key.Digit5,
            KeyCode.Alpha6 => Key.Digit6,
            KeyCode.Alpha7 => Key.Digit7,
            KeyCode.Alpha8 => Key.Digit8,
            KeyCode.Alpha9 => Key.Digit9,
            _ => null
        };
    }

    public void OnUpdate()
    {
        try
        {
            foreach (var keybind in _keybindRegistry.GetAll())
            {
                if (keybind.CurrentKey != KeyCode.None && keybind.OnPress != null)
                {
                    if (WasKeyPressedThisFrame(keybind.CurrentKey))
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
