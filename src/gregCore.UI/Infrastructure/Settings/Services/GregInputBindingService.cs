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
            _logger.Error($"Rebind failed: Keybind {modId}.{actionId} not found.");
            return false;
        }

        var oldKey = entry.CurrentKey;
        entry.CurrentKey = newKey;
        
        _logger.Info($"Keybind changed: {modId}.{actionId} from {oldKey} to {newKey}");
        
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static Key? KeyCodeToKey(KeyCode keyCode)
    {
        try
        {
            return MapFunctionKeys(keyCode)
                ?? MapSystemKeys(keyCode)
                ?? MapNavigationKeys(keyCode)
                ?? MapLettersAtoI(keyCode)
                ?? MapLettersJtoR(keyCode)
                ?? MapLettersStoZ(keyCode)
                ?? MapDigits(keyCode);
        }
        catch { return null; }
    }

    private static Key? MapFunctionKeys(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.F1: return Key.F1;
                case KeyCode.F2: return Key.F2;
                case KeyCode.F3: return Key.F3;
                case KeyCode.F4: return Key.F4;
                case KeyCode.F5: return Key.F5;
                case KeyCode.F6: return Key.F6;
                default: return MapFunctionMid(keyCode);
            }
        }
        catch { return null; }
    }

    private static Key? MapFunctionMid(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.F7: return Key.F7;
                case KeyCode.F8: return Key.F8;
                case KeyCode.F9: return Key.F9;
                case KeyCode.F10: return Key.F10;
                default: return MapFunctionHigh(keyCode);
            }
        }
        catch { return null; }
    }

    private static Key? MapFunctionHigh(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.F11: return Key.F11;
                case KeyCode.F12: return Key.F12;
                case KeyCode.Escape: return Key.Escape;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapSystemKeys(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.Return: return Key.Enter;
                case KeyCode.KeypadEnter: return Key.Enter;
                case KeyCode.Space: return Key.Space;
                case KeyCode.LeftShift: return Key.LeftShift;
                case KeyCode.RightShift: return Key.RightShift;
                default: return MapSystemMid(keyCode);
            }
        }
        catch { return null; }
    }

    private static Key? MapSystemMid(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.LeftControl: return Key.LeftCtrl;
                case KeyCode.RightControl: return Key.RightCtrl;
                case KeyCode.LeftAlt: return Key.LeftAlt;
                case KeyCode.RightAlt: return Key.RightAlt;
                default: return MapSystemTail(keyCode);
            }
        }
        catch { return null; }
    }

    private static Key? MapSystemTail(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.Tab: return Key.Tab;
                case KeyCode.Backspace: return Key.Backspace;
                case KeyCode.Delete: return Key.Delete;
                case KeyCode.Home: return Key.Home;
                case KeyCode.End: return Key.End;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapNavigationKeys(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.PageUp: return Key.PageUp;
                case KeyCode.PageDown: return Key.PageDown;
                case KeyCode.UpArrow: return Key.UpArrow;
                case KeyCode.DownArrow: return Key.DownArrow;
                case KeyCode.LeftArrow: return Key.LeftArrow;
                case KeyCode.RightArrow: return Key.RightArrow;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersAtoI(KeyCode keyCode)
    {
        try
        {
            return MapLettersAtoD(keyCode) ?? MapLettersEtoI(keyCode);
        }
        catch { return null; }
    }

    private static Key? MapLettersAtoD(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.A: return Key.A;
                case KeyCode.B: return Key.B;
                case KeyCode.C: return Key.C;
                case KeyCode.D: return Key.D;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersEtoI(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.E: return Key.E;
                case KeyCode.F: return Key.F;
                case KeyCode.G: return Key.G;
                case KeyCode.H: return Key.H;
                case KeyCode.I: return Key.I;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersJtoR(KeyCode keyCode)
    {
        try
        {
            return MapLettersJtoM(keyCode) ?? MapLettersNtoR(keyCode);
        }
        catch { return null; }
    }

    private static Key? MapLettersJtoM(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.J: return Key.J;
                case KeyCode.K: return Key.K;
                case KeyCode.L: return Key.L;
                case KeyCode.M: return Key.M;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersNtoR(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.N: return Key.N;
                case KeyCode.O: return Key.O;
                case KeyCode.P: return Key.P;
                case KeyCode.Q: return Key.Q;
                case KeyCode.R: return Key.R;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersStoZ(KeyCode keyCode)
    {
        try
        {
            return MapLettersStoV(keyCode) ?? MapLettersWtoZ(keyCode);
        }
        catch { return null; }
    }

    private static Key? MapLettersStoV(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.S: return Key.S;
                case KeyCode.T: return Key.T;
                case KeyCode.U: return Key.U;
                case KeyCode.V: return Key.V;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapLettersWtoZ(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.W: return Key.W;
                case KeyCode.X: return Key.X;
                case KeyCode.Y: return Key.Y;
                case KeyCode.Z: return Key.Z;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapDigits(KeyCode keyCode)
    {
        try
        {
            return MapDigitsLow(keyCode) ?? MapDigitsHigh(keyCode);
        }
        catch { return null; }
    }

    private static Key? MapDigitsLow(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.Alpha0: return Key.Digit0;
                case KeyCode.Alpha1: return Key.Digit1;
                case KeyCode.Alpha2: return Key.Digit2;
                case KeyCode.Alpha3: return Key.Digit3;
                case KeyCode.Alpha4: return Key.Digit4;
                default: return null;
            }
        }
        catch { return null; }
    }

    private static Key? MapDigitsHigh(KeyCode keyCode)
    {
        try
        {
            switch (keyCode)
            {
                case KeyCode.Alpha5: return Key.Digit5;
                case KeyCode.Alpha6: return Key.Digit6;
                case KeyCode.Alpha7: return Key.Digit7;
                case KeyCode.Alpha8: return Key.Digit8;
                case KeyCode.Alpha9: return Key.Digit9;
                default: return null;
            }
        }
        catch { return null; }
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
