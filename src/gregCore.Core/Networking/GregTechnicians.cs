/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Technician bridge: find/read (ID, name, salary, status),
///               have them repair, assign jobs, manager (hire,
///               dispatch, fire, queue) and HR dialogs. All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregTechnicians
{
    // ── DTO ──────────────────────────────────────────────────────────────────

    public sealed class TechnicianInfo
    {
        public int TechnicianID = -1;
        public string Name = "";
        public int Salary;
        public string State = "";
        public bool IsBusy;
        public bool HasDeviceInHand;
        public string CurrentServerID = "";
    }

    // ── Find / read ──────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.Technician> FindAll()
    {
        var result = new List<global::Il2Cpp.Technician>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Technician>();
            if (all == null) return;
            foreach (var t in all)
            {
                if (t == null) continue;
                try
                {
                    var go = t.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(t);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.Technician FindByID(int technicianID)
    {
        global::Il2Cpp.Technician found = null;
        Try(() =>
        {
            foreach (var t in FindAll())
            {
                int id = -1;
                try { id = t.technicianID; } catch { continue; }
                if (id == technicianID) { found = t; break; }
            }
        });
        return found;
    }

    public static TechnicianInfo Read(global::Il2Cpp.Technician tech)
    {
        var dto = new TechnicianInfo();
        if (tech == null) return dto;
        Try(() => dto.TechnicianID = tech.technicianID);
        Try(() => dto.Name = tech.technicianName ?? "");
        Try(() => dto.Salary = tech.salary);
        Try(() => dto.State = tech.currentState.ToString());
        Try(() => dto.IsBusy = tech.isBusy);
        Try(() => dto.HasDeviceInHand = tech.deviceInHand != null);
        Try(() => dto.CurrentServerID = tech.server != null ? tech.server.ServerID ?? "" : "");
        return dto;
    }

    public static List<TechnicianInfo> ReadAll()
    {
        var result = new List<TechnicianInfo>();
        Try(() =>
        {
            foreach (var t in FindAll())
            {
                try { result.Add(Read(t)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── Actions (Technician) ─────────────────────────────────────────────────

    public static bool RepairDevice(global::Il2Cpp.Technician tech)
    {
        if (tech == null) return false;
        try
        {
            var _ = tech.gameObject; // liveness
            tech.RepairDevice();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RepairDevice failed: {Base(ex)}");
            return false;
        }
    }

    public static bool AssignJob(global::Il2Cpp.Technician tech,
        global::Il2Cpp.NetworkSwitch networkSwitch, global::Il2Cpp.Server server)
    {
        if (tech == null) return false;
        try
        {
            var _ = tech.gameObject; // liveness
            var job = new global::Il2Cpp.TechnicianManager.RepairJob();
            try { job.networkSwitch = networkSwitch; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { job.server = server; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            tech.AssignJob(job);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AssignJob failed: {Base(ex)}");
            return false;
        }
    }

    public static bool BeginRequestJob(global::Il2Cpp.Technician tech)
    {
        if (tech == null) return false;
        try
        {
            var _ = tech.gameObject; // liveness
            var routine = tech.RequestJobDelayed();
            return GregRackAndSfp.StartIl2CppRoutine(routine);
        }
        catch (Exception ex)
        {
            Warn($"RequestJobDelayed failed: {Base(ex)}");
            return false;
        }
    }

    // ── Manager ──────────────────────────────────────────────────────────────

    public static global::Il2Cpp.TechnicianManager GetManager()
    {
        try { return global::Il2Cpp.TechnicianManager.instance; }
        catch { return null; }
    }

    public static bool SendTechnician(global::Il2Cpp.NetworkSwitch networkSwitch, global::Il2Cpp.Server server)
    {
        var mgr = GetManager();
        if (mgr == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.SendTechnician(networkSwitch, server);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SendTechnician failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RequestNextJob(global::Il2Cpp.Technician tech)
    {
        var mgr = GetManager();
        if (mgr == null || tech == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.RequestNextJob(tech);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RequestNextJob failed: {Base(ex)}");
            return false;
        }
    }

    public static bool FireTechnician(int technicianID)
    {
        var mgr = GetManager();
        if (mgr == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.FireTechnician(technicianID);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"FireTechnician failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RestoreJobQueue(List<GregJobSaves.RepairJob> jobs)
    {
        var mgr = GetManager();
        if (mgr == null || jobs == null) return false;
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.RepairJobSaveData> list = null;
        try
        {
            list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.RepairJobSaveData>();
            foreach (var j in jobs)
            {
                try
                {
                    var e = GregJobSaves.CreateRepairJob(j);
                    if (e != null) list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { return false; }
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.RestoreJobQueue(list);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RestoreJobQueue failed: {Base(ex)}");
            return false;
        }
    }

    // ── HR dialogs ───────────────────────────────────────────────────────────

    public static global::Il2Cpp.HRSystem FindHR()
    {
        global::Il2Cpp.HRSystem found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.HRSystem>();
            if (all == null) return;
            foreach (var h in all)
            {
                if (h == null) continue;
                try
                {
                    var go = h.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = h;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool HireEmployee(int index)
    {
        var hr = FindHR();
        if (hr == null) return false;
        try
        {
            var _ = hr.gameObject; // liveness
            hr.ButtonHireEmployee(index);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonHireEmployee failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ConfirmHire()
    {
        var hr = FindHR();
        if (hr == null) return false;
        try
        {
            var _ = hr.gameObject; // liveness
            hr.ButtonConfirmHire();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonConfirmHire failed: {Base(ex)}");
            return false;
        }
    }

    public static bool FireEmployee(int index)
    {
        var hr = FindHR();
        if (hr == null) return false;
        try
        {
            var _ = hr.gameObject; // liveness
            hr.ButtonFireEmployee(index);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonFireEmployee failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ConfirmFireEmployee()
    {
        var hr = FindHR();
        if (hr == null) return false;
        try
        {
            var _ = hr.gameObject; // liveness
            hr.ButtonConfirmFireEmployee();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonConfirmFireEmployee failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Technicians: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Technicians field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
