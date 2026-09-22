/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Objectives + Subnetz-Helper + Device-Timer: Objective
///               anlegen/starten/loeschen, statische CIDR-Prüfung
///               (InternetAccessSR), ITimedDevice an-/abmelden.
///               Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregObjectives
{
    // ── Instanz ──────────────────────────────────────────────────────────────

    public static global::Il2Cpp.Objectives GetInstance()
    {
        try { return global::Il2Cpp.Objectives.instance; }
        catch { return null; }
    }

    // ── Objectives anlegen/starten ───────────────────────────────────────────

    public static bool CreateObjective(int localisationUID, int objectiveUID, Vector3 position,
        int xpReward, int reputationReward, bool isSub)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.CreateNewObjective(localisationUID, objectiveUID, position, xpReward, reputationReward, isSub);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"CreateNewObjective fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static int CreateAppObjective(int customerID, int appID, int time, int requiredIOPS)
    {
        var inst = GetInstance();
        if (inst == null) return -1;
        try
        {
            var _ = inst.gameObject; // liveness
            return inst.CreateAppObjective(customerID, appID, time, requiredIOPS);
        }
        catch (Exception ex)
        {
            Warn($"CreateAppObjective fehlgeschlagen: {Base(ex)}");
            return -1;
        }
    }

    public static bool StartObjective(int objectiveUID, Vector3 position)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.StartObjective(objectiveUID, position, false);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"StartObjective fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClearObjectives()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.ClearObjectives();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ClearObjectives fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool IsTutorialInProgress()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            return inst.IsTutorialInProgress();
        }
        catch { return false; }
    }

    public static List<int> GetActiveObjectiveUIDs()
    {
        var result = new List<int>();
        var inst = GetInstance();
        if (inst == null) return result;
        Try(() =>
        {
            var _ = inst.gameObject; // liveness
            var set = inst.activeObjectives;
            if (set == null) return;
            foreach (var uid in set)
            {
                try { result.Add(uid); } catch { }
            }
        });
        return result;
    }

    // ── ObjectiveTimed / ObjectiveObject ─────────────────────────────────────

    public static bool SetupTimedObjective(global::Il2Cpp.ObjectiveTimed timed,
        int maxTime, string text, int customerID, int appID, int requiredIOPS)
    {
        if (timed == null) return false;
        try
        {
            var _ = timed.gameObject; // liveness
            timed.SetupObjectiveTimed(maxTime, text ?? "", customerID, appID, requiredIOPS);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetupObjectiveTimed fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClaimReward(global::Il2Cpp.ObjectiveObject objective)
    {
        if (objective == null) return false;
        try
        {
            var _ = objective.gameObject; // liveness
            objective.GetReward();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"GetReward fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── CIDR-Helper (InternetAccessSR, statisch) ─────────────────────────────

    public static bool CidrsOverlap(string a, string b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
        try { return global::Il2Cpp.InternetAccessSR.CidrsOverlap(a, b); }
        catch { return false; }
    }

    public static bool TryParseCidr(string text, out uint network, out int prefix)
    {
        network = 0;
        prefix = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        try { return global::Il2Cpp.InternetAccessSR.TryParseCidr(text, out network, out prefix); }
        catch { return false; }
    }

    // ── DeviceTimerManager ───────────────────────────────────────────────────

    public static global::Il2Cpp.DeviceTimerManager GetTimerManager()
    {
        try { return global::Il2Cpp.DeviceTimerManager.instance; }
        catch { return null; }
    }

    public static bool RegisterTimer(global::Il2Cpp.ITimedDevice device)
    {
        var mgr = GetTimerManager();
        if (mgr == null || device == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.Register(device);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Timer-Register fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool UnregisterTimer(global::Il2Cpp.ITimedDevice device)
    {
        var mgr = GetTimerManager();
        if (mgr == null || device == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.Unregister(device);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Timer-Unregister fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Objectives: {message}"); } catch { }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Objectives-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
