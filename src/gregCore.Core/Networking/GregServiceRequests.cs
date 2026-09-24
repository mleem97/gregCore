/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        ServiceRequests-Brücke: Save-DTO (ServiceRequestsSaveData +
///               ServiceRequest), Lesen/Auffüllen, Runtime-Passthroughs
///               (AddRequest, RebuildUI, GetSaveData/LoadFromSave) sowie
///               ServiceRequestRow-Helfer (Bind, Separator, Click, BoundSR).
///               Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregServiceRequests
{
    // ── Verwaltete DTOs ──────────────────────────────────────────────────────

    public sealed class RequestInfo
    {
        public int SrNumber;
        public string State = "";
        public string ShortDescription = "";
        public string LongDescription = "";
        public bool RewardGranted;
        public bool Completed;
        public string ProgressText = "";
    }

    public sealed class SaveSnapshot
    {
        public int CurrentSRNumber;
        public List<RequestInfo> Requests = new List<RequestInfo>();
    }

    // ── Instanz ──────────────────────────────────────────────────────────────

    public static global::Il2Cpp.ServiceRequests GetInstance()
    {
        try { return global::Il2Cpp.ServiceRequests.instance; }
        catch { return null; }
    }

    // ── Lesen ────────────────────────────────────────────────────────────────

    public static RequestInfo Read(global::Il2Cpp.ServiceRequest sr)
    {
        var dto = new RequestInfo();
        if (sr == null) return dto;
        Try(() => dto.SrNumber = sr.srNumber);
        Try(() => dto.State = sr.state.ToString());
        Try(() => dto.ShortDescription = sr.shortDescription ?? "");
        Try(() => dto.LongDescription = sr.longDescription ?? "");
        Try(() => dto.RewardGranted = sr.rewardGranted);
        Try(() => dto.Completed = sr.IsCompleted());
        Try(() => dto.ProgressText = sr.GetProgressText() ?? "");
        return dto;
    }

    public static List<RequestInfo> GetRequests()
    {
        var result = new List<RequestInfo>();
        var inst = GetInstance();
        if (inst == null) return result;
        Try(() =>
        {
            var _ = inst.gameObject; // liveness
            var list = inst.requests;
            if (list == null) return;
            foreach (var sr in list)
            {
                try { result.Add(Read(sr)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static int GetCurrentSRNumber()
    {
        var inst = GetInstance();
        if (inst == null) return -1;
        try
        {
            var _ = inst.gameObject; // liveness
            return inst.currentSRNumber;
        }
        catch { return -1; }
    }

    public static SaveSnapshot ReadSave(global::Il2Cpp.ServiceRequestsSaveData data)
    {
        var snap = new SaveSnapshot();
        if (data == null) return snap;
        Try(() => snap.CurrentSRNumber = data.currentSRNumber);
        Try(() =>
        {
            var list = data.requests;
            if (list == null) return;
            foreach (var sr in list)
            {
                try { snap.Requests.Add(Read(sr)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return snap;
    }

    public static SaveSnapshot GetSaveSnapshot()
    {
        var inst = GetInstance();
        if (inst == null) return new SaveSnapshot();
        global::Il2Cpp.ServiceRequestsSaveData data = null;
        try
        {
            var _ = inst.gameObject; // liveness
            data = inst.GetSaveData();
        }
        catch { data = null; }
        return ReadSave(data);
    }

    // ── Schreiben / Aktionen (Vanilla-Pfade) ─────────────────────────────────

    public static bool AddRequest(global::Il2Cpp.ServiceRequest sr)
    {
        var inst = GetInstance();
        if (inst == null || sr == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.AddRequest(sr);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AddRequest fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool LoadFromSave(global::Il2Cpp.ServiceRequestsSaveData data)
    {
        var inst = GetInstance();
        if (inst == null || data == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.LoadFromSave(data);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadFromSave fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool RebuildUI()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.RebuildUI();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RebuildUI fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── ServiceRequestRow-Helfer ─────────────────────────────────────────────

    public static List<global::Il2Cpp.ServiceRequestRow> FindRows()
    {
        var result = new List<global::Il2Cpp.ServiceRequestRow>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.ServiceRequestRow>();
            if (all == null) return;
            foreach (var r in all)
            {
                if (r == null) continue;
                try
                {
                    var go = r.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(r);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static bool BindRow(global::Il2Cpp.ServiceRequestRow row, global::Il2Cpp.ServiceRequest sr)
    {
        var inst = GetInstance();
        if (row == null || inst == null || sr == null) return false;
        try
        {
            var _ = row.gameObject; // liveness
            row.Bind(inst, sr);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"BindRow fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool SetRowSeparator(global::Il2Cpp.ServiceRequestRow row, string label)
    {
        if (row == null) return false;
        try
        {
            var _ = row.gameObject; // liveness
            row.SetAsSeparator(label ?? "");
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetRowSeparator fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClickRow(global::Il2Cpp.ServiceRequestRow row)
    {
        if (row == null) return false;
        try
        {
            var _ = row.gameObject; // liveness
            row.OnClickServiceRequestRow();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ClickRow fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static RequestInfo ReadBoundSR(global::Il2Cpp.ServiceRequestRow row)
    {
        if (row == null) return new RequestInfo();
        global::Il2Cpp.ServiceRequest sr = null;
        try
        {
            var _ = row.gameObject; // liveness
            sr = row.boundSR;
        }
        catch { sr = null; }
        return Read(sr);
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] ServiceRequests: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] ServiceRequests-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
