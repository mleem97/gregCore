/// <file-summary>
/// Schicht:      UI
/// Zweck:        Generalisierter Input-Lock (gregCore.UI Baukasten, ersetzt
///               mod-lokale Locks). Wird pro Frame ueber GregMenuRegistry.Tick
///               getrieben: Solange ein registriertes Menue mit Lock-Flags
///               offen ist, bleiben Cursor frei und Spiel-Inputs (PlayerManager
///               + PlayerInput-Komponenten) deaktiviert. Ref-counted ueber die
///               Registry: Erst wenn kein Menue mehr sperrt, wird alles
///               zurueckgesetzt.
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Runtime input manipulation over live game state; needs running game.")]
public static class GregInputLock
{
    private static readonly List<PlayerInput> Suspended = new List<PlayerInput>();
    private static bool _applied;
    private static float _nextRescanRealtime;

    public static bool IsApplied => _applied;

    // Pro Frame aufrufen (via GregMenuRegistry.Tick).
    public static void Refresh()
    {
        try
        {
            if (!GregMenuRegistry.IsLockActive)
            {
                if (_applied) Restore();
                return;
            }
            GregMenuRegistry.GetLockState(out bool cam, out bool move, out bool interact, out bool cursor);
            if (!_applied)
            {
                Apply(cam, move, interact, cursor);
            }
            else
            {
                if (cursor) ForceCursor();
                SuspendNew();
            }
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] InputLock-Refresh fehlgeschlagen: {ex.Message}");
        }
    }

    private static void Apply(bool cam, bool move, bool interact, bool cursor)
    {
        _applied = true;
        if (cursor) ForceCursor();
        SetPlayerManager(!cam, !move, !interact);
        SuspendAll();
    }

    private static void Restore()
    {
        _applied = false;
        ResumeAll();
        SetPlayerManager(true, true, true);
        try
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        catch { }
    }

    private static void ForceCursor()
    {
        try
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        catch { }
    }

    private static void SetPlayerManager(bool mouse, bool movement, bool rayInteract)
    {
        try
        {
            var pm = global::Il2Cpp.PlayerManager.instance;
            if (pm == null) return;
            try { pm.enabledMouseMovement = mouse; } catch { }
            try { pm.enabledPlayerMovement = movement; } catch { }
            try { pm.enabledRayLookInteract = rayInteract; } catch { }
        }
        catch { }
    }

    private static void SuspendAll()
    {
        Suspended.Clear();
        SuspendNew();
    }

    // Auch spaeter gespawnte PlayerInputs einsammeln (wiederholt aufrufen).
    // Objekt-Vollsuche ist teuer: max. 1x/2s (kein Main-Thread-Starving).
    private static void SuspendNew()
    {
        float now = 0f;
        try { now = Time.realtimeSinceStartup; } catch { return; }
        if (now < _nextRescanRealtime) return;
        _nextRescanRealtime = now + 2f;
        PlayerInput[] all = null;
        try { all = Resources.FindObjectsOfTypeAll<PlayerInput>(); } catch { }
        if (all == null) return;
        foreach (var pi in all)
        {
            if (pi == null || Suspended.Contains(pi)) continue;
            try
            {
                var go = pi.gameObject;
                if (go == null || !go.scene.IsValid() || !go.scene.isLoaded) continue;
            }
            catch { continue; }
            try
            {
                try
                {
                    var asset = pi.actions;
                    if (asset != null && asset.enabled) asset.Disable();
                }
                catch { }
                pi.DeactivateInput();
                Suspended.Add(pi);
            }
            catch { }
        }
    }

    private static void ResumeAll()
    {
        foreach (var pi in Suspended)
        {
            if (pi == null) continue;
            try { pi.ActivateInput(); } catch { }
            try
            {
                InputActionAsset asset = null;
                try { asset = pi.actions; } catch { }
                if (asset != null && !asset.enabled) asset.Enable();
            }
            catch { }
        }
        Suspended.Clear();
    }
}
