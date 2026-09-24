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
            // Backspace incl. hold-to-repeat.
            try
            {
                if (kb.backspaceKey.wasReleasedThisFrame) _backspaceHeldSince = -1f;
                else if (kb.backspaceKey.wasPressedThisFrame)
                {
                    if (BackspaceOnce(ref buffer)) changed = true;
                    _backspaceHeldSince = Time.realtimeSinceStartup;
                    _lastBackspaceRepeat = Time.realtimeSinceStartup;
                }
                else if (_backspaceHeldSince >= 0f && kb.backspaceKey.isPressed)
                {
                    float held = Time.realtimeSinceStartup - _backspaceHeldSince;
                    if (held >= BackspaceInitialDelay && !string.IsNullOrEmpty(buffer))
                    {
                        float interval = held >= 1.2f ? BackspaceFastInterval : BackspaceSlowInterval;
                        if (Time.realtimeSinceStartup - _lastBackspaceRepeat >= interval)
                        {
                            _lastBackspaceRepeat = Time.realtimeSinceStartup;
                            if (BackspaceOnce(ref buffer)) changed = true;
                        }
                    }
                    else if (string.IsNullOrEmpty(buffer)) _backspaceHeldSince = -1f;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            // Ctrl+Backspace = clear whole field.
            try
            {
                if ((kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed) && kb.backspaceKey.wasPressedThisFrame)
                {
                    if (!string.IsNullOrEmpty(buffer)) { buffer = ""; changed = true; }
                    return changed;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            if (buffer != null && buffer.Length >= Math.Max(1, maxLength)) return changed;

            bool shift = false;
            try { shift = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed; } catch { /* ignored */ }

            if (allowNewline)
            {
                try
                {
                    if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
                    {
                        buffer = (buffer ?? "") + "\n";
                        return true;
                    }
                }
                catch { /* ignored */ }
                try
                {
                    if (kb.tabKey.wasPressedThisFrame)
                    {
                        buffer = (buffer ?? "") + "  ";
                        changed = true;
                    }
                }
                catch { /* ignored */ }
            }

            int cap = Math.Max(1, maxLength);
            try { if (kb.spaceKey.wasPressedThisFrame) AppendChar(' ', ref buffer, ref changed, cap); } catch { /* ignored */ }

            PumpLetter(kb.aKey, shift ? 'A' : 'a', ref buffer, ref changed, cap);
            PumpLetter(kb.bKey, shift ? 'B' : 'b', ref buffer, ref changed, cap);
            PumpLetter(kb.cKey, shift ? 'C' : 'c', ref buffer, ref changed, cap);
            PumpLetter(kb.dKey, shift ? 'D' : 'd', ref buffer, ref changed, cap);
            PumpLetter(kb.eKey, shift ? 'E' : 'e', ref buffer, ref changed, cap);
            PumpLetter(kb.fKey, shift ? 'F' : 'f', ref buffer, ref changed, cap);
            PumpLetter(kb.gKey, shift ? 'G' : 'g', ref buffer, ref changed, cap);
            PumpLetter(kb.hKey, shift ? 'H' : 'h', ref buffer, ref changed, cap);
            PumpLetter(kb.iKey, shift ? 'I' : 'i', ref buffer, ref changed, cap);
            PumpLetter(kb.jKey, shift ? 'J' : 'j', ref buffer, ref changed, cap);
            PumpLetter(kb.kKey, shift ? 'K' : 'k', ref buffer, ref changed, cap);
            PumpLetter(kb.lKey, shift ? 'L' : 'l', ref buffer, ref changed, cap);
            PumpLetter(kb.mKey, shift ? 'M' : 'm', ref buffer, ref changed, cap);
            PumpLetter(kb.nKey, shift ? 'N' : 'n', ref buffer, ref changed, cap);
            PumpLetter(kb.oKey, shift ? 'O' : 'o', ref buffer, ref changed, cap);
            PumpLetter(kb.pKey, shift ? 'P' : 'p', ref buffer, ref changed, cap);
            PumpLetter(kb.qKey, shift ? 'Q' : 'q', ref buffer, ref changed, cap);
            PumpLetter(kb.rKey, shift ? 'R' : 'r', ref buffer, ref changed, cap);
            PumpLetter(kb.sKey, shift ? 'S' : 's', ref buffer, ref changed, cap);
            PumpLetter(kb.tKey, shift ? 'T' : 't', ref buffer, ref changed, cap);
            PumpLetter(kb.uKey, shift ? 'U' : 'u', ref buffer, ref changed, cap);
            PumpLetter(kb.vKey, shift ? 'V' : 'v', ref buffer, ref changed, cap);
            PumpLetter(kb.wKey, shift ? 'W' : 'w', ref buffer, ref changed, cap);
            PumpLetter(kb.xKey, shift ? 'X' : 'x', ref buffer, ref changed, cap);
            PumpLetter(kb.yKey, shift ? 'Y' : 'y', ref buffer, ref changed, cap);
            PumpLetter(kb.zKey, shift ? 'Z' : 'z', ref buffer, ref changed, cap);

            PumpKey(kb.digit1Key, shift ? '!' : '1', ref buffer, ref changed, cap);
            PumpKey(kb.digit2Key, shift ? '@' : '2', ref buffer, ref changed, cap);
            PumpKey(kb.digit3Key, shift ? '#' : '3', ref buffer, ref changed, cap);
            PumpKey(kb.digit4Key, shift ? '$' : '4', ref buffer, ref changed, cap);
            PumpKey(kb.digit5Key, shift ? '%' : '5', ref buffer, ref changed, cap);
            PumpKey(kb.digit6Key, shift ? '^' : '6', ref buffer, ref changed, cap);
            PumpKey(kb.digit7Key, shift ? '&' : '7', ref buffer, ref changed, cap);
            PumpKey(kb.digit8Key, shift ? '*' : '8', ref buffer, ref changed, cap);
            PumpKey(kb.digit9Key, shift ? '(' : '9', ref buffer, ref changed, cap);
            PumpKey(kb.digit0Key, shift ? ')' : '0', ref buffer, ref changed, cap);
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

            PumpKey(kb.minusKey, shift ? '_' : '-', ref buffer, ref changed, cap);
            PumpKey(kb.equalsKey, shift ? '+' : '=', ref buffer, ref changed, cap);
            PumpKey(kb.leftBracketKey, shift ? '{' : '[', ref buffer, ref changed, cap);
            PumpKey(kb.rightBracketKey, shift ? '}' : ']', ref buffer, ref changed, cap);
            PumpKey(kb.semicolonKey, shift ? ':' : ';', ref buffer, ref changed, cap);
            PumpKey(kb.quoteKey, shift ? '"' : '\'', ref buffer, ref changed, cap);
            PumpKey(kb.commaKey, shift ? '<' : ',', ref buffer, ref changed, cap);
            PumpKey(kb.periodKey, shift ? '>' : '.', ref buffer, ref changed, cap);
            PumpKey(kb.slashKey, shift ? '?' : '/', ref buffer, ref changed, cap);
            PumpKey(kb.backslashKey, shift ? '|' : '\\', ref buffer, ref changed, cap);
            PumpKey(kb.backquoteKey, shift ? '~' : '`', ref buffer, ref changed, cap);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return changed;
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
