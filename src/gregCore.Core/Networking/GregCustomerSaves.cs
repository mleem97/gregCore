/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Save-DTOs für Kunden (CustomerBase, CustomerRecord):
///               Erzeugen, Fuellen, Lesen, Upsert per customerBaseID
///               (Liste: NetworkSaveData.customerBases). Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using gregCore.Core.Mods;
using MelonLoader;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregCustomerSaves
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class CustomerBase
    {
        public int CustomerBaseID { get; set; }
        public int CustomerID { get; set; }
        public Dictionary<int, string> SubnetsPerApp { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, int> VlanIdsPerApp { get; set; } = new Dictionary<int, int>();
        public float[] AppsSpeedRequirements { get; set; } = Array.Empty<float>();
        public int[] AppTypes { get; set; } = Array.Empty<int>();
        public int Difficulty { get; set; }
        public Dictionary<int, int> AppObjectiveIDs { get; set; } = new Dictionary<int, int>();
        public int[] AppsTimeBelowRequirements { get; set; } = Array.Empty<int>();
        public bool[] AppReputationAwarded { get; set; } = Array.Empty<bool>();
        public bool WantsInternet { get; set; }
        public List<int> InternetApps { get; set; } = new List<int>();
    }

    public sealed class CustomerRecord
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = "";
        public float Revenue { get; set; }
        public float Penalties { get; set; }
    }

    // ── CustomerBase ─────────────────────────────────────────────────────────

    public static global::Il2Cpp.CustomerBaseSaveData CreateBase(CustomerBase dto)
    {
        global::Il2Cpp.CustomerBaseSaveData entry = null;
        try { entry = new global::Il2Cpp.CustomerBaseSaveData(); } catch { return null; }
        FillBase(entry, dto);
        return entry;
    }

    public static void FillBase(global::Il2Cpp.CustomerBaseSaveData entry, CustomerBase dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.customerBaseID = dto.CustomerBaseID);
        Try(() => entry.customerID = dto.CustomerID);
        Try(() => entry.subnetsPerApp = ToStringDict(dto.SubnetsPerApp));
        Try(() => entry.vlanIdsPerApp = ToIntDict(dto.VlanIdsPerApp));
        Try(() => entry.appsSpeedRequirements = GregModPack.ToFloatArray(dto.AppsSpeedRequirements));
        Try(() => entry.appTypes = GregModPack.ToIntArray(dto.AppTypes));
        Try(() => entry.difficulty = dto.Difficulty);
        Try(() => entry.appObjectiveIDs = ToIntDict(dto.AppObjectiveIDs));
        Try(() => entry.appsTimeBelowRequirements = GregModPack.ToIntArray(dto.AppsTimeBelowRequirements));
        Try(() => entry.appReputationAwarded = GregModPack.ToBoolArray(dto.AppReputationAwarded));
        Try(() => entry.wantsInternet = dto.WantsInternet);
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<int>();
            foreach (var id in dto.InternetApps ?? new List<int>())
            {
                try { list.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.internetApps = list;
        });
    }

    public static CustomerBase ReadBase(global::Il2Cpp.CustomerBaseSaveData entry)
    {
        var dto = new CustomerBase();
        if (entry == null) return dto;
        Try(() => dto.CustomerBaseID = entry.customerBaseID);
        Try(() => dto.CustomerID = entry.customerID);
        Try(() => dto.SubnetsPerApp = FromStringDict(entry.subnetsPerApp));
        Try(() => dto.VlanIdsPerApp = FromIntDict(entry.vlanIdsPerApp));
        Try(() => dto.AppsSpeedRequirements = GregModPack.FromFloatArray(entry.appsSpeedRequirements));
        Try(() => dto.AppTypes = GregModPack.FromIntArray(entry.appTypes));
        Try(() => dto.Difficulty = entry.difficulty);
        Try(() => dto.AppObjectiveIDs = FromIntDict(entry.appObjectiveIDs));
        Try(() => dto.AppsTimeBelowRequirements = GregModPack.FromIntArray(entry.appsTimeBelowRequirements));
        Try(() => dto.AppReputationAwarded = GregModPack.FromBoolArray(entry.appReputationAwarded));
        Try(() => dto.WantsInternet = entry.wantsInternet);
        Try(() =>
        {
            var list = entry.internetApps;
            if (list == null) return;
            foreach (var id in list)
            {
                try { dto.InternetApps.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return dto;
    }

    public static List<CustomerBase> ReadAllBases(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.CustomerBaseSaveData> list)
    {
        var result = new List<CustomerBase>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(ReadBase(entry)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.CustomerBaseSaveData UpsertBase(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.CustomerBaseSaveData> list, CustomerBase dto)
    {
        if (list == null || dto == null) return null;
        global::Il2Cpp.CustomerBaseSaveData found = null;
        Try(() =>
        {
            foreach (var entry in list)
            {
                if (entry == null) continue;
                int id = -1;
                try { id = entry.customerBaseID; } catch { continue; }
                if (id == dto.CustomerBaseID) { found = entry; break; }
            }
        });
        if (found != null) { FillBase(found, dto); return found; }
        var created = CreateBase(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    // ── CustomerRecord ───────────────────────────────────────────────────────

    public static global::Il2Cpp.CustomerRecordSaveData CreateRecord(CustomerRecord dto)
    {
        global::Il2Cpp.CustomerRecordSaveData entry = null;
        try { entry = new global::Il2Cpp.CustomerRecordSaveData(); } catch { return null; }
        FillRecord(entry, dto);
        return entry;
    }

    public static void FillRecord(global::Il2Cpp.CustomerRecordSaveData entry, CustomerRecord dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.customerID = dto.CustomerID);
        Try(() => entry.customerName = dto.CustomerName ?? "");
        Try(() => entry.revenue = dto.Revenue);
        Try(() => entry.penalties = dto.Penalties);
    }

    public static CustomerRecord ReadRecord(global::Il2Cpp.CustomerRecordSaveData entry)
    {
        var dto = new CustomerRecord();
        if (entry == null) return dto;
        Try(() => dto.CustomerID = entry.customerID);
        Try(() => dto.CustomerName = entry.customerName ?? "");
        Try(() => dto.Revenue = entry.revenue);
        Try(() => dto.Penalties = entry.penalties);
        return dto;
    }

    // ── Dict-Brücken ─────────────────────────────────────────────────────────

    private static Il2CppSystem.Collections.Generic.Dictionary<int, string> ToStringDict(Dictionary<int, string> src)
    {
        var dst = new Il2CppSystem.Collections.Generic.Dictionary<int, string>();
        if (src == null) return dst;
        foreach (var kv in src)
        {
            try { dst.Add(kv.Key, kv.Value ?? ""); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return dst;
    }

    private static Il2CppSystem.Collections.Generic.Dictionary<int, int> ToIntDict(Dictionary<int, int> src)
    {
        var dst = new Il2CppSystem.Collections.Generic.Dictionary<int, int>();
        if (src == null) return dst;
        foreach (var kv in src)
        {
            try { dst.Add(kv.Key, kv.Value); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return dst;
    }

    private static Dictionary<int, string> FromStringDict(Il2CppSystem.Collections.Generic.Dictionary<int, string> src)
    {
        var dst = new Dictionary<int, string>();
        if (src == null) return dst;
        try
        {
            foreach (var kv in src)
            {
                try { dst[kv.Key] = kv.Value ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return dst;
    }

    private static Dictionary<int, int> FromIntDict(Il2CppSystem.Collections.Generic.Dictionary<int, int> src)
    {
        var dst = new Dictionary<int, int>();
        if (src == null) return dst;
        try
        {
            foreach (var kv in src)
            {
                try { dst[kv.Key] = kv.Value; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return dst;
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] CustomerSaves-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
