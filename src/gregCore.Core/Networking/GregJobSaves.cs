/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Save-DTOs für Jobs + Interact-Objekte (Technician,
///               RepairJob, InteractObject): Erzeugen, Fuellen, Lesen;
///               Upsert per uid bei InteractObjects (Listen:
///               SaveData.rackMountObjectData / interactObjectData).
///               Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using gregCore.Core.Mods;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregJobSaves
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class TechnicianSave
    {
        public float[] Position = Array.Empty<float>();
        public int TechnicianID;
    }

    public sealed class RepairJob
    {
        public string ServerID = "";
        public string SwitchID = "";
    }

    public sealed class InteractObject
    {
        public int Uid;
        public float[] Value = Array.Empty<float>();
        public int[] SaveIntArray = Array.Empty<int>();
        public int[] SaveIntArray2 = Array.Empty<int>();
        public Vector3 Position;
        public Quaternion Rotation;
        public string LabelText = "";
        public string CoopLooseId = "";
        public string RackTemplateId = "";
    }

    // ── Technician ───────────────────────────────────────────────────────────

    public static global::Il2Cpp.TechnicianSaveData CreateTechnician(TechnicianSave dto)
    {
        // Vanilla-Ctor braucht ein live Technician-Objekt; null ist
        // best-effort (liefert null, wenn Vanilla dereferenziert).
        global::Il2Cpp.TechnicianSaveData entry = null;
        try { entry = new global::Il2Cpp.TechnicianSaveData(null); } catch { return null; }
        FillTechnician(entry, dto);
        return entry;
    }

    public static void FillTechnician(global::Il2Cpp.TechnicianSaveData entry, TechnicianSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.position = GregModPack.ToFloatArray(dto.Position));
        Try(() => entry.technicianID = dto.TechnicianID);
    }

    public static TechnicianSave ReadTechnician(global::Il2Cpp.TechnicianSaveData entry)
    {
        var dto = new TechnicianSave();
        if (entry == null) return dto;
        Try(() => dto.Position = GregModPack.FromFloatArray(entry.position));
        Try(() => dto.TechnicianID = entry.technicianID);
        return dto;
    }

    public static List<TechnicianSave> ReadAllTechnicians(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.TechnicianSaveData> list)
    {
        var result = new List<TechnicianSave>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(ReadTechnician(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── RepairJob ────────────────────────────────────────────────────────────

    public static global::Il2Cpp.RepairJobSaveData CreateRepairJob(RepairJob dto)
    {
        global::Il2Cpp.RepairJobSaveData entry = null;
        try { entry = new global::Il2Cpp.RepairJobSaveData(); } catch { return null; }
        FillRepairJob(entry, dto);
        return entry;
    }

    public static void FillRepairJob(global::Il2Cpp.RepairJobSaveData entry, RepairJob dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.serverID = dto.ServerID ?? "");
        Try(() => entry.switchID = dto.SwitchID ?? "");
    }

    public static RepairJob ReadRepairJob(global::Il2Cpp.RepairJobSaveData entry)
    {
        var dto = new RepairJob();
        if (entry == null) return dto;
        Try(() => dto.ServerID = entry.serverID ?? "");
        Try(() => dto.SwitchID = entry.switchID ?? "");
        return dto;
    }

    public static List<RepairJob> ReadAllRepairJobs(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.RepairJobSaveData> list)
    {
        var result = new List<RepairJob>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(ReadRepairJob(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── InteractObject (Key: uid) ────────────────────────────────────────────

    public static global::Il2Cpp.InteractObjectData CreateInteractObject(InteractObject dto)
    {
        // Vanilla-Ctor braucht ein live Interact-Objekt; null ist
        // best-effort (liefert null, wenn Vanilla dereferenziert).
        global::Il2Cpp.InteractObjectData entry = null;
        try { entry = new global::Il2Cpp.InteractObjectData(null); } catch { return null; }
        FillInteractObject(entry, dto);
        return entry;
    }

    public static void FillInteractObject(global::Il2Cpp.InteractObjectData entry, InteractObject dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.uid = dto.Uid);
        Try(() => entry.value = GregModPack.ToFloatArray(dto.Value));
        Try(() => entry.saveIntArray = GregModPack.ToIntArray(dto.SaveIntArray));
        Try(() => entry.saveIntArray2 = GregModPack.ToIntArray(dto.SaveIntArray2));
        Try(() => entry.position = dto.Position);
        Try(() => entry.rotation = dto.Rotation);
        Try(() => entry.labelText = dto.LabelText ?? "");
        Try(() => entry.coopLooseId = dto.CoopLooseId ?? "");
        Try(() => entry.rackTemplateId = dto.RackTemplateId ?? "");
    }

    public static InteractObject ReadInteractObject(global::Il2Cpp.InteractObjectData entry)
    {
        var dto = new InteractObject();
        if (entry == null) return dto;
        Try(() => dto.Uid = entry.uid);
        Try(() => dto.Value = GregModPack.FromFloatArray(entry.value));
        Try(() => dto.SaveIntArray = GregModPack.FromIntArray(entry.saveIntArray));
        Try(() => dto.SaveIntArray2 = GregModPack.FromIntArray(entry.saveIntArray2));
        Try(() => dto.Position = entry.position);
        Try(() => dto.Rotation = entry.rotation);
        Try(() => dto.LabelText = entry.labelText ?? "");
        Try(() => dto.CoopLooseId = entry.coopLooseId ?? "");
        Try(() => dto.RackTemplateId = entry.rackTemplateId ?? "");
        return dto;
    }

    public static List<InteractObject> ReadAllInteractObjects(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.InteractObjectData> list)
    {
        var result = new List<InteractObject>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(ReadInteractObject(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.InteractObjectData UpsertInteractObject(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.InteractObjectData> list, InteractObject dto)
    {
        if (list == null || dto == null) return null;
        global::Il2Cpp.InteractObjectData found = null;
        Try(() =>
        {
            foreach (var entry in list)
            {
                if (entry == null) continue;
                int uid = -1;
                try { uid = entry.uid; } catch { continue; }
                if (uid == dto.Uid) { found = entry; break; }
            }
        });
        if (found != null) { FillInteractObject(found, dto); return found; }
        var created = CreateInteractObject(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] JobSaves-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
