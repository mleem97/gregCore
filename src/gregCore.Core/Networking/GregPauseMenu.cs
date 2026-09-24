/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Pause menu bridge: subscribe to open/close callbacks (vanilla
///               delegates), read pause state. Replaces mod-local
///               canvas lookups (IPAM, NoEOL). All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregPauseMenu
{
    private static readonly Dictionary<Action, global::Il2Cpp.PauseMenu.OnPauseMenuOpen> _openHandlers =
        new Dictionary<Action, global::Il2Cpp.PauseMenu.OnPauseMenuOpen>();

    private static readonly Dictionary<Action, global::Il2Cpp.PauseMenu.OnPauseMenuClose> _closeHandlers =
        new Dictionary<Action, global::Il2Cpp.PauseMenu.OnPauseMenuClose>();

    private static readonly object _lock = new object();

    public static global::Il2Cpp.PauseMenu FindInstance()
    {
        global::Il2Cpp.PauseMenu found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.PauseMenu>();
            if (all == null) return;
            foreach (var p in all)
            {
                if (p == null) continue;
                try
                {
                    var go = p.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = p;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    // True once the pause UI is active (any tab).
    public static bool IsPaused()
    {
        var menu = FindInstance();
        if (menu == null) return false;
        try
        {
            var ui = menu.pauseMenuUI;
            if (ui == null) return false;
            return ui.activeInHierarchy;
        }
        catch { return false; }
    }

    public static bool SubscribeOpen(Action handler)
    {
        if (handler == null) return false;
        lock (_lock)
        {
            try
            {
                if (!_openHandlers.TryGetValue(handler, out var del) || del == null)
                {
                    global::Il2Cpp.PauseMenu.OnPauseMenuOpen converted = handler;
                    del = converted;
                    _openHandlers[handler] = del;
                }
                global::Il2Cpp.PauseMenu.onPauseMenuOpenCallback += del;
                return true;
            }
            catch (Exception ex)
            {
                Warn($"SubscribeOpen failed: {Base(ex)}");
                return false;
            }
        }
    }

    public static bool UnsubscribeOpen(Action handler)
    {
        if (handler == null) return false;
        lock (_lock)
        {
            try
            {
                if (!_openHandlers.TryGetValue(handler, out var del) || del == null)
                    return false;
                global::Il2Cpp.PauseMenu.onPauseMenuOpenCallback -= del;
                _openHandlers.Remove(handler);
                return true;
            }
            catch (Exception ex)
            {
                Warn($"UnsubscribeOpen failed: {Base(ex)}");
                return false;
            }
        }
    }

    public static bool SubscribeClose(Action handler)
    {
        if (handler == null) return false;
        lock (_lock)
        {
            try
            {
                if (!_closeHandlers.TryGetValue(handler, out var del) || del == null)
                {
                    global::Il2Cpp.PauseMenu.OnPauseMenuClose converted = handler;
                    del = converted;
                    _closeHandlers[handler] = del;
                }
                global::Il2Cpp.PauseMenu.onPauseMenuCloseCallback += del;
                return true;
            }
            catch (Exception ex)
            {
                Warn($"SubscribeClose failed: {Base(ex)}");
                return false;
            }
        }
    }

    public static bool UnsubscribeClose(Action handler)
    {
        if (handler == null) return false;
        lock (_lock)
        {
            try
            {
                if (!_closeHandlers.TryGetValue(handler, out var del) || del == null)
                    return false;
                global::Il2Cpp.PauseMenu.onPauseMenuCloseCallback -= del;
                _closeHandlers.Remove(handler);
                return true;
            }
            catch (Exception ex)
            {
                Warn($"UnsubscribeClose failed: {Base(ex)}");
                return false;
            }
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] PauseMenu: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] PauseMenu field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
