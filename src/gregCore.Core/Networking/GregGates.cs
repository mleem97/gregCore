/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Tor- und Wand-Brücke (GateLever, Wall): finden, öffnen/
///               schliessen (lokal + Netzwerk), Status lesen. Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregGates
{
    // ── Finden ───────────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.GateLever> FindAllGates()
    {
        return FindAll<global::Il2Cpp.GateLever>();
    }

    public static List<global::Il2Cpp.Wall> FindAllWalls()
    {
        return FindAll<global::Il2Cpp.Wall>();
    }

    private static List<T> FindAll<T>() where T : UnityEngine.Object
    {
        var result = new List<T>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<T>();
            if (all == null) return;
            foreach (var o in all)
            {
                if (o == null) continue;
                try
                {
                    var go = (o as Component) != null
                        ? ((Component)(object)o).gameObject
                        : (o as GameObject);
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(o);
                }
                catch { }
            }
        });
        return result;
    }

    // ── GateLever ────────────────────────────────────────────────────────────

    public static bool OpenGate(global::Il2Cpp.GateLever gate)
    {
        if (gate == null) return false;
        try
        {
            var _ = gate.gameObject; // liveness
            gate.OpenGate();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"OpenGate fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool CloseGate(global::Il2Cpp.GateLever gate, bool networked)
    {
        if (gate == null) return false;
        try
        {
            var _ = gate.gameObject; // liveness
            if (networked) gate.CloseGateNetworked();
            else gate.CloseGate();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"CloseGate fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ApplyRemoteToggle(global::Il2Cpp.GateLever gate, bool open)
    {
        if (gate == null) return false;
        try
        {
            var _ = gate.gameObject; // liveness
            gate.ApplyRemoteToggle(open);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ApplyRemoteToggle fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool TruckComing(global::Il2Cpp.GateLever gate)
    {
        if (gate == null) return false;
        try
        {
            var _ = gate.gameObject; // liveness
            gate.TruckComing();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"TruckComing fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── Wall ─────────────────────────────────────────────────────────────────

    public static bool IsWallOpened(global::Il2Cpp.Wall wall)
    {
        if (wall == null) return false;
        try { return wall.isWallOpened; } catch { return false; }
    }

    public static bool IsInternetWall(global::Il2Cpp.Wall wall)
    {
        if (wall == null) return false;
        try { return wall.isInternetWall; } catch { return false; }
    }

    public static bool OpenWall(global::Il2Cpp.Wall wall)
    {
        if (wall == null) return false;
        try
        {
            var _ = wall.gameObject; // liveness
            wall.OpenWall();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"OpenWall fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Gates: {message}"); } catch { }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Gates-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
