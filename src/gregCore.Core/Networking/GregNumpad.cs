/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Numpad bridge (number pad): find instance, read typed
///               number, press keys (digits, OK, Del, Clear,
///               Copy, Paste). All best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregNumpad
{
    public static global::Il2Cpp.Numpad FindInstance()
    {
        global::Il2Cpp.Numpad found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Numpad>();
            if (all == null) return;
            foreach (var n in all)
            {
                if (n == null) continue;
                try
                {
                    var go = n.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = n;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool IsActive()
    {
        var inst = FindInstance();
        if (inst == null) return false;
        try { return inst.isActive; } catch { return false; }
    }

    public static string GetWrittenNumber()
    {
        var inst = FindInstance();
        if (inst == null) return "";
        try { return inst.writtenNumber ?? ""; } catch { return ""; }
    }

    public static string GetCopiedNumber()
    {
        var inst = FindInstance();
        if (inst == null) return "";
        try { return inst.copiedNumber ?? ""; } catch { return ""; }
    }

    public static bool PressNumber(string number)
    {
        var inst = FindInstance();
        if (inst == null || string.IsNullOrEmpty(number)) return false;
        try
        {
            _ = inst.gameObject; // liveness
            inst.ClickNumber(number);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ClickNumber failed: {Base(ex)}");
            return false;
        }
    }

    public static bool PressOK()
    {
        return PressSimple("OK", p => p.ClickButtonOK());
    }

    public static bool PressDelete()
    {
        return PressSimple("Del", p => p.ClickButtonDel());
    }

    public static bool PressClear()
    {
        return PressSimple("Clear", p => p.ClickButtonClear());
    }

    public static bool PressCopy()
    {
        return PressSimple("Copy", p => p.ClickButtonCopy());
    }

    public static bool PressPaste()
    {
        return PressSimple("Paste", p => p.ClickButtonPaste());
    }

    private static bool PressSimple(string label, Action<global::Il2Cpp.Numpad> action)
    {
        var inst = FindInstance();
        if (inst == null || action == null) return false;
        try
        {
            _ = inst.gameObject; // liveness
            action(inst);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"{label} failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Numpad: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Numpad field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
