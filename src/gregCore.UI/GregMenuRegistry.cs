/// <file-summary>
/// Schicht:      UI
/// Zweck:        Zentrale Menue-Registry (gregCore.UI Baukasten). Mods
///               registrieren ihre Menues mit GregMenuOptions und melden
///               Offen/Geschlossen. Daraus steuert das Framework Input-Lock
///               (GregInputLock) und Panel-Ticks (GregPanel) - ref-counted,
///               d.h. der Lock faellt erst wenn KEIN sperrendes Menue mehr
///               offen ist. Tick() laeuft ueber GregCoreMod.OnUpdate.
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

    // Oeffner fuer das F1-Hub: wird aufgerufen wenn der Benutzer im Hub
    // auf "Oeffnen" klickt. Optional (null = nur Anzeige).
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
            MelonLogger.Warning($"[gregCore][UI] Menue-Oeffner fehlgeschlagen ({menuId}): {ex.Message}");
            return false;
        }
    }

    // Schliesser fuer das F1-Hub: wird aufgerufen wenn der Benutzer im Hub
    // auf "Schliessen" klickt. Nur wenn ein Closer registriert ist, zeigt der
    // Hub "Schliessen" an — sonst bleibt es bei ehrlichem "Oeffnen".
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
            MelonLogger.Warning($"[gregCore][UI] Menue-Schliesser fehlgeschlagen ({menuId}): {ex.Message}");
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

    // Aggregierter Lock-Zustand ueber alle offenen Menues (ODER-Verknuepfung).
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

    // Pro Frame aus GregCoreMod.OnUpdate. Steuert Input-Lock und Panels.
    public static void Tick(float dt)
    {
        try { GregInputLock.Refresh(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] InputLock-Tick fehlgeschlagen: {ex.Message}");
        }
        try { GregPanel.TickAll(dt); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Panel-Tick fehlgeschlagen: {ex.Message}");
        }
        try { GregHud.Tick(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] HUD-Tick fehlgeschlagen: {ex.Message}");
        }
        try { GregModHub.PollClicks(); }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Hub-Klicks fehlgeschlagen: {ex.Message}");
        }
    }
}
