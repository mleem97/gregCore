/// <file-summary>
/// Layer:      UI
/// Purpose:     Central menu registry (gregCore.UI kit). Mods
///              register their menus with GregMenuOptions and report
///              open/closed. From this, the framework controls input lock
///              (GregInputLock) and panel ticks (GregPanel) - ref-counted,
///              i.e. the lock is released only once NO locking menu remains
///              open. Tick() runs via GregCoreMod.OnUpdate.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Runtime UI orchestration over live game state; needs running game.")]
public static class GregMenuRegistry
{
    private sealed class Entry
    {
        public GregMenuOptions Options = new GregMenuOptions();
        public bool Open;
        public Action Opener;
        public Action Closer;
    }

    private static readonly Dictionary<string, Entry> _menus = new Dictionary<string, Entry>();

    public static void RegisterMenu(string menuId, GregMenuOptions options)
    {
        if (string.IsNullOrEmpty(menuId)) return;
        lock (_menus)
        {
            if (!_menus.TryGetValue(menuId, out var e))
            {
                e = new Entry();
                _menus[menuId] = e;
            }
            e.Options = options != null ? options.Clone() : new GregMenuOptions();
        }
    }

    public static void UnregisterMenu(string menuId)
    {
        if (string.IsNullOrEmpty(menuId)) return;
        lock (_menus) { _menus.Remove(menuId); }
    }

    // Opener for the F1 hub: called when the user clicks "open" in the hub.
    // Optional (null = display only).
    public static void RegisterOpener(string menuId, Action opener)
    {
        if (string.IsNullOrEmpty(menuId)) return;
        lock (_menus)
        {
            if (!_menus.TryGetValue(menuId, out var e))
            {
                e = new Entry();
                _menus[menuId] = e;
            }
            e.Opener = opener;
        }
    }

    public static bool TryOpen(string menuId)
    {
        Action opener = null;
        lock (_menus)
        {
            if (_menus.TryGetValue(menuId, out var e)) opener = e.Opener;
        }
        if (opener == null) return false;
        try { opener(); return true; }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Menu opener failed ({menuId}): {ex.Message}");
            return false;
        }
    }

    // Closer for the F1 hub: called when the user clicks "Schliessen" in the hub.
    // Only if a closer is registered does the hub show "Schliessen"
    // — otherwise it stays with honest "open".
    public static void RegisterCloser(string menuId, Action closer)
    {
        if (string.IsNullOrEmpty(menuId)) return;
        lock (_menus)
        {
            if (!_menus.TryGetValue(menuId, out var e))
            {
                e = new Entry();
                _menus[menuId] = e;
            }
            e.Closer = closer;
        }
    }

    public static bool TryClose(string menuId)
    {
        Action closer = null;
        lock (_menus)
        {
            if (_menus.TryGetValue(menuId, out var e)) closer = e.Closer;
        }
        if (closer == null) return false;
        try { closer(); return true; }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Menu closer failed ({menuId}): {ex.Message}");
            return false;
        }
    }

    public static IReadOnlyList<MenuInfo> Snapshot()
    {
        var list = new List<MenuInfo>();
        lock (_menus)
        {
            foreach (var kv in _menus)
            {
                if (kv.Value == null) continue;
                list.Add(new MenuInfo
                {
                    MenuId = kv.Key,
                    Open = kv.Value.Open,
                    HasOpener = kv.Value.Opener != null,
                    HasCloser = kv.Value.Closer != null,
                });
            }
        }
        list.Sort((a, b) => string.Compare(a.MenuId, b.MenuId, StringComparison.OrdinalIgnoreCase));
        return list;
    }

    public sealed class MenuInfo
    {
        public string MenuId;
        public bool Open;
        public bool HasOpener;
        public bool HasCloser;
    }

    public static void SetOpen(string menuId, bool open)
    {
        if (string.IsNullOrEmpty(menuId)) return;
        lock (_menus)
        {
            if (!_menus.TryGetValue(menuId, out var e))
            {
                e = new Entry();
                _menus[menuId] = e;
            }
            e.Open = open;
        }
    }

    public static bool IsOpen(string menuId)
    {
        if (string.IsNullOrEmpty(menuId)) return false;
        lock (_menus) { return _menus.TryGetValue(menuId, out var e) && e.Open; }
    }

    public static GregMenuOptions GetOptions(string menuId)
    {
        lock (_menus)
        {
            if (_menus.TryGetValue(menuId, out var e) && e.Options != null)
                return e.Options.Clone();
        }
        return new GregMenuOptions();
    }

    // Aggregated lock state across all open menus (OR combination).
    public static void GetLockState(out bool cam, out bool move, out bool interact, out bool cursor)
    {
        cam = false; move = false; interact = false; cursor = false;
        lock (_menus)
        {
            foreach (var kv in _menus)
            {
                var e = kv.Value;
                if (e == null || !e.Open || e.Options == null) continue;
                cam |= e.Options.LockCamera;
                move |= e.Options.LockMovement;
                interact |= e.Options.LockInteract;
                cursor |= e.Options.ShowCursor;
            }
        }
    }

    public static bool IsLockActive
    {
        get
        {
            GetLockState(out bool cam, out bool move, out bool interact, out bool cursor);
            return cam || move || interact || cursor;
        }
    }

    // Per frame from GregCoreMod.OnUpdate. Controls input lock and panels.
    public static void Tick(float dt)
    {
        try { GregInputLock.Refresh(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] InputLock-Tick failed: {ex.Message}");
        }
        try { GregPanel.TickAll(dt); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Panel-Tick failed: {ex.Message}");
        }
        try { GregHud.Tick(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] HUD-Tick failed: {ex.Message}");
        }
        try { GregModHub.PollClicks(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Hub clicks failed: {ex.Message}");
        }
    }
}
