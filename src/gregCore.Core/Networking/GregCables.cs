/// <file-summary>
/// Layer:       Core (Networking)
<<<<<<< HEAD
/// Purpose:     Cable bridge (live CableLink): find (all/by switchId/
///               by type), read state (speed, type, parents, SFP),
///               actions (set speed, insert/remove SFP, second/label action,
///               rope anchor). All best-effort.
/// Note:        CableIDComponent has no public members (only private
///               CableId/SwitchId) — intentionally omitted.
=======
/// Purpose:     Cable bridge (live CableLink): find (all/per switchId/
///               per type), read state (speed, type, parents, SFP),
///               actions (set speed, insert/remove SFP, second/label action,
///               rope anchors). All best-effort.
/// Note:        CableIDComponent has no public members (only private
///               CableId/SwitchId) — deliberately omitted.
>>>>>>> agent/gregcore-integration
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregCables
{
    // ── DTO ──────────────────────────────────────────────────────────────────

    public sealed class LinkInfo
    {
        public string SwitchID { get; set; } = "";
        public string Type { get; set; } = "";
        public float ConnectionSpeed { get; set; }
        public int CustomerID { get; set; } = -1;
        public bool IsStartOrEnd { get; set; }
        public bool IsEndPoint { get; set; }
        public bool IsSFPPort { get; set; }
        public int SfpTypeInserted { get; set; } = -1;
        public int SfpTypeSupported { get; set; } = -1;
        public bool IsFibrePort { get; set; }
        public int CableIDsOnLink { get; set; }
        public string ParentServerID { get; set; } = "";
        public string ParentSwitchID { get; set; } = "";
        public string ParentPatchPanelID { get; set; } = "";
    }

<<<<<<< HEAD
    // ── Find ───────────────────────────────────────────────────────────────
=======
    // ── Find ─────────────────────────────────────────────────────────────────
>>>>>>> agent/gregcore-integration

    public static List<global::Il2Cpp.CableLink> FindAll()
    {
        var result = new List<global::Il2Cpp.CableLink>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.CableLink>();
            if (all == null) return;
            foreach (var l in all)
            {
                if (l == null) continue;
                try
                {
                    var go = l.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(l);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static List<global::Il2Cpp.CableLink> FindBySwitchId(string switchID)
    {
        var result = new List<global::Il2Cpp.CableLink>();
        if (string.IsNullOrEmpty(switchID)) return result;
        Try(() =>
        {
            foreach (var l in FindAll())
            {
                string id = null;
                try { id = l.switchID; } catch { continue; }
                if (string.Equals(id, switchID, StringComparison.OrdinalIgnoreCase))
                {
                    try { result.Add(l); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        });
        return result;
    }

    public static List<global::Il2Cpp.CableLink> FindByType(string typeOfLink)    {
        var result = new List<global::Il2Cpp.CableLink>();
        if (string.IsNullOrWhiteSpace(typeOfLink)) return result;
        Try(() =>
        {
            foreach (var l in FindAll())
            {
                string t = null;
                try { t = l.typeOfLink.ToString(); } catch { continue; }
                if (string.Equals(t, typeOfLink, StringComparison.OrdinalIgnoreCase))
                {
                    try { result.Add(l); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        });
        return result;
    }

<<<<<<< HEAD
    // ── Read ────────────────────────────────────────────────────────────────
=======
    public static List<global::Il2Cpp.CableLink> FindByServer(global::Il2Cpp.Server server)
    {
        var result = new List<global::Il2Cpp.CableLink>();
        if (server == null) return result;
        Try(() =>
        {
            foreach (var l in FindAll())
            {
                if (l == null) continue;
                global::Il2Cpp.Server parent = null;
                try { parent = l.parentServer; } catch { continue; }
                if (parent == null) continue;
                bool same = false;
                try { same = parent == server; } catch { continue; }
                if (same)
                {
                    try { result.Add(l); } catch { }
                }
            }
        });
        return result;
    }
>>>>>>> agent/gregcore-integration

    // ── Read ─────────────────────────────────────────────────────────────────
    public static LinkInfo Read(global::Il2Cpp.CableLink link)
    {
        var dto = new LinkInfo();
        if (link == null) return dto;
        Try(() => dto.SwitchID = link.switchID ?? "");
        Try(() => dto.Type = link.typeOfLink.ToString());
        Try(() => dto.ConnectionSpeed = link.connectionSpeed);
        Try(() => dto.CustomerID = link.CustomerID);
        Try(() => dto.IsStartOrEnd = link.isStartOrEnd);
        Try(() => dto.IsEndPoint = link.isEndPoint);
        Try(() => dto.IsSFPPort = link.isSFPPort);
        Try(() => dto.SfpTypeInserted = link.sfpTypeInserted);
        Try(() => dto.SfpTypeSupported = link.sfpTypeSupported);
        Try(() => dto.IsFibrePort = link.isFibrePort);
        Try(() => dto.CableIDsOnLink = link.cableIDsOnLink);
        Try(() => dto.ParentServerID = link.parentServer != null ? link.parentServer.ServerID ?? "" : "");
        Try(() => dto.ParentSwitchID = link.parentSwitch != null ? link.parentSwitch.switchId ?? "" : "");
        Try(() => dto.ParentPatchPanelID = link.parentPatchPanel != null ? link.parentPatchPanel.patchPanelId ?? "" : "");
        return dto;
    }

    public static List<LinkInfo> ReadAll()
    {
        var result = new List<LinkInfo>();
        Try(() =>
        {
            foreach (var l in FindAll())
            {
                try { result.Add(Read(l)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

<<<<<<< HEAD
    // ── Actions ─────────────────────────────────────────────────────────────
=======
    // ── Actions ──────────────────────────────────────────────────────────────
>>>>>>> agent/gregcore-integration

    public static bool SetConnectionSpeed(global::Il2Cpp.CableLink link, float speed)
    {
        if (link == null) return false;
        try
        {
            var _ = link.gameObject; // liveness
            link.SetConnectionSpeed(speed);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetConnectionSpeed failed: {Base(ex)}");
            return false;
        }
    }

    public static bool InsertSFP(global::Il2Cpp.CableLink link, float speed, int type, global::Il2Cpp.SFPModule module)
    {
        if (link == null || module == null) return false;
        try
        {
            var _ = link.gameObject; // liveness
            link.InsertSFP(speed, type, module);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"InsertSFP failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RemoveSFP(global::Il2Cpp.CableLink link)
    {
        if (link == null) return false;
        try
        {
            var _ = link.gameObject; // liveness
            link.RemoveSFP();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveSFP failed: {Base(ex)}");
            return false;
        }
    }

    public static bool SecondAction(global::Il2Cpp.CableLink link)
    {
        if (link == null) return false;
        try
        {
            var _ = link.gameObject; // liveness
            if (!link.IsAllowedToDoSecondAction()) return false;
            link.SecondActionOnClick();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SecondAction failed: {Base(ex)}");
            return false;
        }
    }

    public static bool LabelAction(global::Il2Cpp.CableLink link)
    {
        if (link == null) return false;
        try
        {
            var _ = link.gameObject; // liveness
            link.LabelActionOnClick();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LabelAction failed: {Base(ex)}");
            return false;
        }
    }

    public static Transform GetRopeAttachPoint(global::Il2Cpp.CableLink link, bool createIfMissing)
    {
        if (link == null) return null;
        try
        {
            var _ = link.gameObject; // liveness
            if (createIfMissing)
            {
                try { link.CreateRopeAttachPoint(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            return link.GetRopeAttachPoint();
        }
        catch (Exception ex)
        {
            Warn($"RopeAttachPoint failed: {Base(ex)}");
            return null;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Cables: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Cables field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
