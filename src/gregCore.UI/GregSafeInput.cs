/// <file-summary>
/// Layer:      UI
/// Purpose:    IL2CPP-safe text input (gregCore.UI kit). The game strips the
///              TextEditor APIs used by GUI.TextField and UIToolkit TextField
///              (hard crash) — so this renders text in a Label and pumps
///              UnityEngine.InputSystem.Keyboard manually. GregKeyPump holds
///              per-field state (backspace repeat); GregSafeTextField is the
///              drop-in VisualElement (focus, caret, Changed event). The owning
///              mod calls PumpFrame() once per frame while the field is
///              focused (see GregPanelBuilder.AddSafeInputField).
///              Needs the running game (excluded from coverage).
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Live InputSystem/UIToolkit input over running game; needs running game.")]
public sealed class GregKeyPump
{
    private const float BackspaceInitialDelay = 0.45f;
    private const float BackspaceSlowInterval = 0.07f;
    private const float BackspaceFastInterval = 0.03f;

    private float _backspaceHeldSince = -1f;
    private float _lastBackspaceRepeat;

    public void Reset()
    {
        _backspaceHeldSince = -1f;
    }

    /// <summary>
    /// Pumps one frame of keyboard input into <paramref name="buffer"/>.
    /// Returns true when the text changed. Never throws. Tab inserts two
    /// spaces when <paramref name="allowNewline"/> is true — callers that
    /// use Tab for focus-switching must intercept it before calling.
    /// </summary>
    public bool PumpFrame(ref string? buffer, int maxLength, bool allowNewline)
    {
        bool changed = false;
        Keyboard? kb;
        try { kb = Keyboard.current; } catch { return false; }
        if (kb == null) return false;
        try
        {
            PumpBackspace(kb, ref buffer, ref changed);
            if (TryClearAll(kb, ref buffer, ref changed)) return changed;
            if (IsFull(buffer, maxLength)) return changed;
            bool shift = ReadShift(kb);
            if (TryConsumeNewline(kb, ref buffer, ref changed, allowNewline)) return true;
            int cap = Math.Max(1, maxLength);
            PumpAll(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return changed;
    }

    private static bool IsFull(string? buffer, int maxLength)
    {
        try { return buffer != null && buffer.Length >= Math.Max(1, maxLength); }
        catch { return false; }
    }

    private static bool TryConsumeNewline(Keyboard? kb, ref string? buffer, ref bool changed, bool allowNewline)
    {
        try
        {
            if (!allowNewline) return false;
            if (TryNewline(kb, ref buffer)) return true;
            PumpTab(kb, ref buffer, ref changed);
            return false;
        }
        catch { return false; }
    }

    private static void PumpAll(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpSpace(kb, ref buffer, ref changed, cap);
            PumpAlphabet(kb, shift, ref buffer, ref changed, cap);
            PumpDigits(kb, shift, ref buffer, ref changed, cap);
            PumpSymbols(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private void PumpBackspace(Keyboard? kb, ref string? buffer, ref bool changed)
    {
        try
        {
            if (kb.backspaceKey.wasReleasedThisFrame) _backspaceHeldSince = -1f;
            else if (kb.backspaceKey.wasPressedThisFrame) PressBackspace(ref buffer, ref changed);
            else if (_backspaceHeldSince >= 0f && kb.backspaceKey.isPressed) RepeatBackspace(ref buffer, ref changed);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private void PressBackspace(ref string? buffer, ref bool changed)
    {
        try
        {
            if (BackspaceOnce(ref buffer)) changed = true;
            _backspaceHeldSince = Time.realtimeSinceStartup;
            _lastBackspaceRepeat = Time.realtimeSinceStartup;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private void RepeatBackspace(ref string? buffer, ref bool changed)
    {
        try
        {
            float held = Time.realtimeSinceStartup - _backspaceHeldSince;
            if (held < BackspaceInitialDelay)
            {
                if (string.IsNullOrEmpty(buffer)) _backspaceHeldSince = -1f;
                return;
            }
            if (string.IsNullOrEmpty(buffer)) { _backspaceHeldSince = -1f; return; }
            float interval = held >= 1.2f ? BackspaceFastInterval : BackspaceSlowInterval;
            if (Time.realtimeSinceStartup - _lastBackspaceRepeat < interval) return;
            _lastBackspaceRepeat = Time.realtimeSinceStartup;
            if (BackspaceOnce(ref buffer)) changed = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static bool TryClearAll(Keyboard? kb, ref string? buffer, ref bool changed)
    {
        try
        {
            if ((kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed) && kb.backspaceKey.wasPressedThisFrame)
            {
                if (!string.IsNullOrEmpty(buffer)) { buffer = ""; changed = true; }
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    private static bool ReadShift(Keyboard? kb)
    {
        try { return kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed; } catch { return false; }
    }

    private static bool TryNewline(Keyboard? kb, ref string? buffer)
    {
        try
        {
            if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
            {
                buffer = (buffer ?? "") + "\n";
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    private static void PumpTab(Keyboard? kb, ref string? buffer, ref bool changed)
    {
        try
        {
            if (!kb.tabKey.wasPressedThisFrame) return;
            buffer = (buffer ?? "") + "  ";
            changed = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpSpace(Keyboard? kb, ref string? buffer, ref bool changed, int cap)
    {
        try { if (kb.spaceKey.wasPressedThisFrame) AppendChar(' ', ref buffer, ref changed, cap); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpAlphabet(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpLettersAtoM(kb, shift, ref buffer, ref changed, cap);
            PumpLettersNtoZ(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static char Pick(bool shift, char lower, char upper)
    {
        try { return shift ? upper : lower; }
        catch { return lower; }
    }

    private static void PumpLettersAtoM(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpLettersAtoG(kb, shift, ref buffer, ref changed, cap);
            PumpLettersHtoM(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpLettersAtoG(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpLetter(kb.aKey, Pick(shift, 'a', 'A'), ref buffer, ref changed, cap);
            PumpLetter(kb.bKey, Pick(shift, 'b', 'B'), ref buffer, ref changed, cap);
            PumpLetter(kb.cKey, Pick(shift, 'c', 'C'), ref buffer, ref changed, cap);
            PumpLetter(kb.dKey, Pick(shift, 'd', 'D'), ref buffer, ref changed, cap);
            PumpLetter(kb.eKey, Pick(shift, 'e', 'E'), ref buffer, ref changed, cap);
            PumpLetter(kb.fKey, Pick(shift, 'f', 'F'), ref buffer, ref changed, cap);
            PumpLetter(kb.gKey, Pick(shift, 'g', 'G'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpLettersHtoM(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpLetter(kb.hKey, Pick(shift, 'h', 'H'), ref buffer, ref changed, cap);
            PumpLetter(kb.iKey, Pick(shift, 'i', 'I'), ref buffer, ref changed, cap);
            PumpLetter(kb.jKey, Pick(shift, 'j', 'J'), ref buffer, ref changed, cap);
            PumpLetter(kb.kKey, Pick(shift, 'k', 'K'), ref buffer, ref changed, cap);
            PumpLetter(kb.lKey, Pick(shift, 'l', 'L'), ref buffer, ref changed, cap);
            PumpLetter(kb.mKey, Pick(shift, 'm', 'M'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpLettersNtoZ(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpLettersNtoS(kb, shift, ref buffer, ref changed, cap);
            PumpLettersTtoZ(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpLettersNtoS(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpLetter(kb.nKey, Pick(shift, 'n', 'N'), ref buffer, ref changed, cap);
            PumpLetter(kb.oKey, Pick(shift, 'o', 'O'), ref buffer, ref changed, cap);
            PumpLetter(kb.pKey, Pick(shift, 'p', 'P'), ref buffer, ref changed, cap);
            PumpLetter(kb.qKey, Pick(shift, 'q', 'Q'), ref buffer, ref changed, cap);
            PumpLetter(kb.rKey, Pick(shift, 'r', 'R'), ref buffer, ref changed, cap);
            PumpLetter(kb.sKey, Pick(shift, 's', 'S'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpLettersTtoZ(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpLetter(kb.tKey, Pick(shift, 't', 'T'), ref buffer, ref changed, cap);
            PumpLetter(kb.uKey, Pick(shift, 'u', 'U'), ref buffer, ref changed, cap);
            PumpLetter(kb.vKey, Pick(shift, 'v', 'V'), ref buffer, ref changed, cap);
            PumpLetter(kb.wKey, Pick(shift, 'w', 'W'), ref buffer, ref changed, cap);
            PumpLetter(kb.xKey, Pick(shift, 'x', 'X'), ref buffer, ref changed, cap);
            PumpLetter(kb.yKey, Pick(shift, 'y', 'Y'), ref buffer, ref changed, cap);
            PumpLetter(kb.zKey, Pick(shift, 'z', 'Z'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpDigits(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpTopDigits(kb, shift, ref buffer, ref changed, cap);
            PumpNumpad(kb, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpTopDigits(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpTopDigitsLow(kb, shift, ref buffer, ref changed, cap);
            PumpTopDigitsHigh(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpTopDigitsLow(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpKey(kb.digit1Key, Pick(shift, '1', '!'), ref buffer, ref changed, cap);
            PumpKey(kb.digit2Key, Pick(shift, '2', '@'), ref buffer, ref changed, cap);
            PumpKey(kb.digit3Key, Pick(shift, '3', '#'), ref buffer, ref changed, cap);
            PumpKey(kb.digit4Key, Pick(shift, '4', '$'), ref buffer, ref changed, cap);
            PumpKey(kb.digit5Key, Pick(shift, '5', '%'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpTopDigitsHigh(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            if (kb == null) return;
            PumpKey(kb.digit6Key, Pick(shift, '6', '^'), ref buffer, ref changed, cap);
            PumpKey(kb.digit7Key, Pick(shift, '7', '&'), ref buffer, ref changed, cap);
            PumpKey(kb.digit8Key, Pick(shift, '8', '*'), ref buffer, ref changed, cap);
            PumpKey(kb.digit9Key, Pick(shift, '9', '('), ref buffer, ref changed, cap);
            PumpKey(kb.digit0Key, Pick(shift, '0', ')'), ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpNumpad(Keyboard? kb, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpKey(kb.numpad1Key, '1', ref buffer, ref changed, cap);
            PumpKey(kb.numpad2Key, '2', ref buffer, ref changed, cap);
            PumpKey(kb.numpad3Key, '3', ref buffer, ref changed, cap);
            PumpKey(kb.numpad4Key, '4', ref buffer, ref changed, cap);
            PumpKey(kb.numpad5Key, '5', ref buffer, ref changed, cap);
            PumpKey(kb.numpad6Key, '6', ref buffer, ref changed, cap);
            PumpKey(kb.numpad7Key, '7', ref buffer, ref changed, cap);
            PumpKey(kb.numpad8Key, '8', ref buffer, ref changed, cap);
            PumpKey(kb.numpad9Key, '9', ref buffer, ref changed, cap);
            PumpKey(kb.numpad0Key, '0', ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpSymbols(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpSymbolsA(kb, shift, ref buffer, ref changed, cap);
            PumpSymbolsB(kb, shift, ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpSymbolsA(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpKey(kb.minusKey, shift ? '_' : '-', ref buffer, ref changed, cap);
            PumpKey(kb.equalsKey, shift ? '+' : '=', ref buffer, ref changed, cap);
            PumpKey(kb.leftBracketKey, shift ? '{' : '[', ref buffer, ref changed, cap);
            PumpKey(kb.rightBracketKey, shift ? '}' : ']', ref buffer, ref changed, cap);
            PumpKey(kb.semicolonKey, shift ? ':' : ';', ref buffer, ref changed, cap);
            PumpKey(kb.quoteKey, shift ? '"' : '\'', ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void PumpSymbolsB(Keyboard? kb, bool shift, ref string? buffer, ref bool changed, int cap)
    {
        try
        {
            PumpKey(kb.commaKey, shift ? '<' : ',', ref buffer, ref changed, cap);
            PumpKey(kb.periodKey, shift ? '>' : '.', ref buffer, ref changed, cap);
            PumpKey(kb.slashKey, shift ? '?' : '/', ref buffer, ref changed, cap);
            PumpKey(kb.backslashKey, shift ? '|' : '\\', ref buffer, ref changed, cap);
            PumpKey(kb.backquoteKey, shift ? '~' : '`', ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static bool BackspaceOnce(ref string? buffer)
    {
        try
        {
            if (string.IsNullOrEmpty(buffer)) return false;
            buffer = buffer.Substring(0, buffer.Length - 1);
            return true;
        }
        catch { return false; }
    }

    private static void PumpLetter(KeyControl? key, char c, ref string? buffer, ref bool changed, int maxLen)
    {
        PumpKey(key, c, ref buffer, ref changed, maxLen);
    }

    private static void PumpKey(KeyControl? key, char c, ref string? buffer, ref bool changed, int maxLen)
    {
        try
        {
            if (key != null && key.wasPressedThisFrame)
                AppendChar(c, ref buffer, ref changed, maxLen);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void AppendChar(char c, ref string? buffer, ref bool changed, int maxLen)
    {
        try
        {
            buffer ??= "";
            if (buffer.Length >= maxLen) return;
            buffer += c;
            changed = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}

[ExcludeFromCodeCoverage(Justification = "Live UIToolkit rendering over running game; needs running game.")]
public sealed class GregSafeTextField : VisualElement
{
    private readonly GregKeyPump _pump = new();
    private readonly Label _label;
    private string _value = "";
    private bool _focused;
    private float _caretAt;
    private bool _caretOn = true;

    public event Action<string>? Changed;

    public GregSafeTextField(string initialValue = "", int maxLength = 512, bool multiline = false)
    {
        MaxLength = Math.Max(1, maxLength);
        Multiline = multiline;
        _value = Sanitize(initialValue ?? "");

        _label = new Label();
        _label.style.whiteSpace = WhiteSpace.Normal;
        _label.style.flexGrow = 1f;
        Add(_label);
        RefreshLabel();
    }

    public string Value
    {
        get => _value;
        set
        {
            try
            {
                var clean = Sanitize(value ?? "");
                if (!string.Equals(clean, _value, StringComparison.Ordinal))
                {
                    _value = clean;
                    RefreshLabel();
                    try { Changed?.Invoke(_value); } catch { /* ignored */ }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    public int MaxLength { get; set; } = 512;
    public bool Multiline { get; set; }
    public bool HasFocus => _focused;
    public Label InnerLabel => _label;

    /// <summary>Keyboard-pump focus (independent of UIToolkit visual focus).</summary>
    public void FocusField()
    {
        _focused = true;
        try { _pump.Reset(); } catch { /* ignored */ }
        RefreshLabel();
    }

    public void BlurField()
    {
        _focused = false;
        RefreshLabel();
    }

    /// <summary>
    /// Call once per frame while the field is focused. Returns true on change.
    /// </summary>
    public bool PumpFrame()
    {
        if (!_focused) return false;
        try
        {
            string? buf = _value;
            if (!_pump.PumpFrame(ref buf, MaxLength, Multiline)) return TickCaret();
            Value = Sanitize(buf ?? "");
            return true;
        }
        catch { return false; }
    }

    private bool TickCaret()
    {
        try
        {
            float now = Time.realtimeSinceStartup;
            if (now - _caretAt > 0.53f)
            {
                _caretAt = now;
                _caretOn = !_caretOn;
                RefreshLabel();
            }
        }
        catch { /* ignored */ }
        return false;
    }

    private void RefreshLabel()
    {
        try
        {
            string text = string.IsNullOrEmpty(_value) ? "" : _value.Replace("\t", "  ");
            if (_focused && _caretOn) text += "▌";
            _label.text = text;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private string Sanitize(string value)
    {
        try
        {
            var s = (value ?? "").Replace("\r", "");
            if (!Multiline) s = s.Replace("\n", " ");
            if (s.Length > MaxLength) s = s.Substring(0, MaxLength);
            return s;
        }
        catch { return ""; }
    }
}
