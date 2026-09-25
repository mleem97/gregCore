/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Patch panel bridge: save DTO (PatchPanelSaveData) plus
///               create/fill/read/upsert in the game's own list
///               (NetworkSaveData.patchPanels, key patchPanelID) as well as
///               safe runtime helpers (Capture, InsertIntoRack, FindAll,
///               IsAnyCableConnected, ValidateRackPosition). All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregPatchPanels
{
    // ── Managed DTO ──────────────────────────────────────────────────────────

    public sealed class PatchPanelSave
    {
        public string PatchPanelID { get; set; } = "";
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int RackPositionUID { get; set; } = -1;
        public int PatchPanelType { get; set; }
    }

    // ── Create / fill / read ─────────────────────────────────────────────────

    public static global::Il2Cpp.PatchPanelSaveData Create(PatchPanelSave dto)
    {
        global::Il2Cpp.PatchPanelSaveData entry = null;
        try { entry = new global::Il2Cpp.PatchPanelSaveData(); } catch { return null; }
        Fill(entry, dto);
        return entry;
    }

    public static void Fill(global::Il2Cpp.PatchPanelSaveData entry, PatchPanelSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.patchPanelID = dto.PatchPanelID ?? "");
        Try(() => entry.position = dto.Position);
        Try(() => entry.rotation = dto.Rotation);
        Try(() => entry.rackPositionUID = dto.RackPositionUID);
        Try(() => entry.patchPanelType = dto.PatchPanelType);
    }

    public static PatchPanelSave Read(global::Il2Cpp.PatchPanelSaveData entry)
    {
        var dto = new PatchPanelSave();
        if (entry == null) return dto;
        Try(() => dto.PatchPanelID = entry.patchPanelID ?? "");
        Try(() => dto.Position = entry.position);
        Try(() => dto.Rotation = entry.rotation);
        Try(() => dto.RackPositionUID = entry.rackPositionUID);
        Try(() => dto.PatchPanelType = entry.patchPanelType);
        return dto;
    }

    public static List<PatchPanelSave> ReadAll(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.PatchPanelSaveData> list)
    {
        var result = new List<PatchPanelSave>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(Read(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── Upsert / remove (key: patchPanelID; empty ID -> always append) ─────────

    public static global::Il2Cpp.PatchPanelSaveData Upsert(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.PatchPanelSaveData> list, PatchPanelSave dto)
    {
        if (list == null || dto == null) return null;
        string key = dto.PatchPanelID ?? "";
        global::Il2Cpp.PatchPanelSaveData found = null;
        if (!string.IsNullOrEmpty(key))
        {
            Try(() =>
            {
                foreach (var entry in list)
                {
                    if (entry == null) continue;
                    string id = null;
                    try { id = entry.patchPanelID; } catch { continue; }
                    if (string.Equals(id, key, StringComparison.OrdinalIgnoreCase))
                    {
                        found = entry;
                        break;
                    }
                }
            });
        }
        if (found != null)
        {
            Fill(found, dto);
            return found;
        }
        var created = Create(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    public static bool Remove(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.PatchPanelSaveData> list, string patchPanelID)
    {
        if (list == null || string.IsNullOrEmpty(patchPanelID)) return false;
        bool removed = false;
        Try(() =>
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                global::Il2Cpp.PatchPanelSaveData entry = null;
                try { entry = list[i]; } catch { continue; }
                if (entry == null) continue;
                string id = null;
                try { id = entry.patchPanelID; } catch { continue; }
                if (string.Equals(id, patchPanelID, StringComparison.OrdinalIgnoreCase))
                {
                    try { list.RemoveAt(i); removed = true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        });
        return removed;
    }

    // ── Runtime bridge (live patch panel) ────────────────────────────────────

    public static List<global::Il2Cpp.PatchPanel> FindAll()
    {
        var result = new List<global::Il2Cpp.PatchPanel>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.PatchPanel>();
            if (all == null) return;
            foreach (var p in all)
            {
                if (p == null) continue;
                try
                {
                    var go = p.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(p);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.PatchPanel FindById(string patchPanelID)
    {
        if (string.IsNullOrEmpty(patchPanelID)) return null;
        global::Il2Cpp.PatchPanel found = null;
        Try(() =>
        {
            foreach (var p in FindAll())
            {
                string id = null;
                try { id = p.patchPanelId; } catch { continue; }
                if (string.Equals(id, patchPanelID, StringComparison.OrdinalIgnoreCase))
                {
                    found = p;
                    break;
                }
            }
        });
        return found;
    }

    // Live panel -> DTO (RackPositionUID is not readable runtime-side -> -1).
    public static PatchPanelSave Capture(global::Il2Cpp.PatchPanel panel)
    {
        var dto = new PatchPanelSave();
        if (panel == null) return dto;
        Try(() => dto.PatchPanelID = panel.patchPanelId ?? "");
        Try(() => dto.PatchPanelType = panel.patchPanelType);
        Try(() => dto.Position = panel.transform.position);
        Try(() => dto.Rotation = panel.transform.rotation);
        return dto;
    }

    // Vanilla restore path: insert panel into the rack via save entry.
    public static bool InsertIntoRack(global::Il2Cpp.PatchPanel panel, global::Il2Cpp.PatchPanelSaveData entry)
    {
        if (panel == null || entry == null) return false;
        try
        {
            _ = panel.gameObject; // liveness
            panel.InsertedInRack(entry);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"InsertedInRack failed: {Base(ex)}");
            return false;
        }
    }

    public static bool InsertIntoRack(global::Il2Cpp.PatchPanel panel, PatchPanelSave dto)
    {
        if (panel == null || dto == null) return false;
        var entry = Create(dto);
        if (entry == null) return false;
        return InsertIntoRack(panel, entry);
    }

    public static bool IsAnyCableConnected(global::Il2Cpp.PatchPanel panel)
    {
        if (panel == null) return false;
        try { return panel.IsAnyCableConnected(); } catch { return false; }
    }

    public static bool ValidateRackPosition(global::Il2Cpp.PatchPanel panel)
    {
        if (panel == null) return false;
        try { return panel.ValidateRackPosition(); } catch { return false; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] PatchPanels: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] PatchPanels field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
