/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Objectives + subnet helpers + device timers: create/
///               start/delete objectives, static CIDR checks
///               (InternetAccessSR), register/unregister ITimedDevice.
///               All best-effort.
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
    // ── Instance ─────────────────────────────────────────────────────────────

    public static global::Il2Cpp.Objectives GetInstance()
    {
        try { return global::Il2Cpp.Objectives.instance; }
        catch { return null; }
    }

    // ── Create/start objectives ──────────────────────────────────────────────

    public static bool CreateObjective(int localisationUID, int objectiveUID, Vector3 position,
        int xpReward, int reputationReward, bool isSub)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            _ = inst.gameObject; // liveness
            inst.CreateNewObjective(localisationUID, objectiveUID, position, xpReward, reputationReward, isSub);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"CreateNewObjective failed: {Base(ex)}");
            return false;
        }
    }

    public static int CreateAppObjective(int customerID, int appID, int time, int requiredIOPS)
    {
        var inst = GetInstance();
        if (inst == null) return -1;
        try
        {
            _ = inst.gameObject; // liveness
            return inst.CreateAppObjective(customerID, appID, time, requiredIOPS);
        }
        catch (Exception ex)
        {
            Warn($"CreateAppObjective failed: {Base(ex)}");
            return -1;
        }
    }

    public static bool StartObjective(int objectiveUID, Vector3 position)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            _ = inst.gameObject; // liveness
            inst.StartObjective(objectiveUID, position, false);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"StartObjective failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ClearObjectives()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            _ = inst.gameObject; // liveness
            inst.ClearObjectives();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ClearObjectives failed: {Base(ex)}");
            return false;
        }
    }

    public static bool IsTutorialInProgress()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            _ = inst.gameObject; // liveness
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
            _ = inst.gameObject; // liveness
            var set = inst.activeObjectives;
            if (set == null) return;
            foreach (var uid in set)
            {
                try { result.Add(uid); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
            _ = timed.gameObject; // liveness
            timed.SetupObjectiveTimed(maxTime, text ?? "", customerID, appID, requiredIOPS);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetupObjectiveTimed failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ClaimReward(global::Il2Cpp.ObjectiveObject objective)
    {
        if (objective == null) return false;
        try
        {
            _ = objective.gameObject; // liveness
            objective.GetReward();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"GetReward failed: {Base(ex)}");
            return false;
        }
    }

    // ── CIDR helpers (InternetAccessSR, static) ──────────────────────────────

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
            _ = mgr.gameObject; // liveness
            mgr.Register(device);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Timer-Register failed: {Base(ex)}");
            return false;
        }
    }

    public static bool UnregisterTimer(global::Il2Cpp.ITimedDevice device)
    {
        var mgr = GetTimerManager();
        if (mgr == null || device == null) return false;
        try
        {
            _ = mgr.gameObject; // liveness
            mgr.Unregister(device);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Timer-Unregister failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Objectives: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Objectives field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
