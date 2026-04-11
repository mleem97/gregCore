using System;
using MoonSharp.Interpreter;

namespace gregModLoader.LanguageBridges.LuaModules;

/// <summary>
/// Keyboard/mouse input API for Lua: <c>greg.input.*</c>.
/// Uses Unity InputSystem (Keyboard.current, Mouse.current) via dynamic to avoid CI ref issues.
/// </summary>
public sealed class gregInputLuaModule : iGregLuaModule
{
    public void Register(Script vm, Table greg)
    {
        var input = new Table(vm);
        greg["input"] = input;

        // greg.input.key_pressed(keyName) -> bool (was pressed THIS frame)
        input["key_pressed"] = (Func<string, bool>)(keyName =>
        {
            try
            {
                dynamic kb = GetKeyboard();
                if (kb == null) return false;
                var key = FindKey(kb, keyName);
                return key?.wasPressedThisFrame ?? false;
            }
            catch { return false; }
        });

        // greg.input.key_down(keyName) -> bool (is currently held)
        input["key_down"] = (Func<string, bool>)(keyName =>
        {
            try
            {
                dynamic kb = GetKeyboard();
                if (kb == null) return false;
                var key = FindKey(kb, keyName);
                return key?.isPressed ?? false;
            }
            catch { return false; }
        });

        // greg.input.ctrl() -> bool
        input["ctrl"] = (Func<bool>)(() =>
        {
            try
            {
                dynamic kb = GetKeyboard();
                if (kb == null) return false;
                return kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed;
            }
            catch { return false; }
        });

        // greg.input.shift() -> bool
        input["shift"] = (Func<bool>)(() =>
        {
            try
            {
                dynamic kb = GetKeyboard();
                if (kb == null) return false;
                return kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            }
            catch { return false; }
        });

        // greg.input.alt() -> bool
        input["alt"] = (Func<bool>)(() =>
        {
            try
            {
                dynamic kb = GetKeyboard();
                if (kb == null) return false;
                return kb.leftAltKey.isPressed || kb.rightAltKey.isPressed;
            }
            catch { return false; }
        });
    }

    private static object GetKeyboard()
    {
        try
        {
            var type = Type.GetType("UnityEngine.InputSystem.Keyboard, Unity.InputSystem");
            if (type == null) return null;
            var prop = type.GetProperty("current", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return prop?.GetValue(null);
        }
        catch { return null; }
    }

    private static dynamic FindKey(dynamic kb, string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var upper = name.ToUpperInvariant();
        return upper switch
        {
            "F1" => kb.f1Key,
            "F2" => kb.f2Key,
            "F3" => kb.f3Key,
            "F4" => kb.f4Key,
            "F5" => kb.f5Key,
            "F6" => kb.f6Key,
            "F7" => kb.f7Key,
            "F8" => kb.f8Key,
            "F9" => kb.f9Key,
            "F10" => kb.f10Key,
            "F11" => kb.f11Key,
            "F12" => kb.f12Key,
            "ESCAPE" or "ESC" => kb.escapeKey,
            "SPACE" => kb.spaceKey,
            "ENTER" or "RETURN" => kb.enterKey,
            "TAB" => kb.tabKey,
            "BACKSPACE" => kb.backspaceKey,
            "DELETE" => kb.deleteKey,
            "INSERT" => kb.insertKey,
            "HOME" => kb.homeKey,
            "END" => kb.endKey,
            "PAGEUP" => kb.pageUpKey,
            "PAGEDOWN" => kb.pageDownKey,
            "UP" => kb.upArrowKey,
            "DOWN" => kb.downArrowKey,
            "LEFT" => kb.leftArrowKey,
            "RIGHT" => kb.rightArrowKey,
            _ => TryFindByCharacter(kb, upper)
        };
    }

    private static dynamic TryFindByCharacter(dynamic kb, string upper)
    {
        try
        {
            if (upper.Length == 1)
            {
                char c = upper[0];
                if (c >= 'A' && c <= 'Z')
                {
                    return c switch
                    {
                        'A' => kb.aKey, 'B' => kb.bKey, 'C' => kb.cKey, 'D' => kb.dKey,
                        'E' => kb.eKey, 'F' => kb.fKey, 'G' => kb.gKey, 'H' => kb.hKey,
                        'I' => kb.iKey, 'J' => kb.jKey, 'K' => kb.kKey, 'L' => kb.lKey,
                        'M' => kb.mKey, 'N' => kb.nKey, 'O' => kb.oKey, 'P' => kb.pKey,
                        'Q' => kb.qKey, 'R' => kb.rKey, 'S' => kb.sKey, 'T' => kb.tKey,
                        'U' => kb.uKey, 'V' => kb.vKey, 'W' => kb.wKey, 'X' => kb.xKey,
                        'Y' => kb.yKey, 'Z' => kb.zKey,
                        _ => null
                    };
                }
            }
        }
        catch { }
        return null;
    }
}
