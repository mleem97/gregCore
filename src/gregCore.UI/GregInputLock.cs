/// <file-summary>
/// Layer:      UI
/// Purpose:     Generalized input lock (gregCore.UI kit, replaces
///              mod-local locks). Driven per frame via GregMenuRegistry.Tick:
///              as long as a registered menu with lock flags is open,
///              the cursor stays free and game inputs (PlayerManager
///              + PlayerInput components) stay disabled. Ref-counted via the
///              registry: everything is restored only once no menu
///              locks anymore.
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

    // Call per frame (via GregMenuRegistry.Tick).
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
            MelonLogger.Warning($"[gregCore][UI] InputLock-Refresh failed: {ex.Message}");
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void ForceCursor()
    {
        try
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SetPlayerManager(bool mouse, bool movement, bool rayInteract)
    {
        try
        {
            var pm = global::Il2Cpp.PlayerManager.instance;
            if (pm == null) return;
            try { pm.enabledMouseMovement = mouse; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { pm.enabledPlayerMovement = movement; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { pm.enabledRayLookInteract = rayInteract; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void SuspendAll()
    {
        Suspended.Clear();
        SuspendNew();
    }

    // Also collect later-spawned PlayerInputs (call repeatedly).
    // Full object search is expensive: max. 1x/2s (no main-thread starvation).
    private static void SuspendNew()
    {
        float now = 0f;
        try { now = Time.realtimeSinceStartup; } catch { return; }
        if (now < _nextRescanRealtime) return;
        _nextRescanRealtime = now + 2f;
        PlayerInput[] all = null;
        try { all = Resources.FindObjectsOfTypeAll<PlayerInput>(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                pi.DeactivateInput();
                Suspended.Add(pi);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static void ResumeAll()
    {
        foreach (var pi in Suspended)
        {
            if (pi == null) continue;
            try { pi.ActivateInput(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try
            {
                InputActionAsset asset = null;
                try { asset = pi.actions; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                if (asset != null && !asset.enabled) asset.Enable();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        Suspended.Clear();
    }
}
