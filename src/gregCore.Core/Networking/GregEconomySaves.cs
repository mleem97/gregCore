/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Save-DTOs für Wirtschaft (MonthlySnapshot, BalanceSheet):
///               Erzeugen, Fuellen, Lesen (geschachtelte Records inklusive).
///               Quelle: SaveData.balanceSheetData. Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregEconomySaves
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class MonthlySnapshot
    {
        public int Month;
        public int Day;
        public List<GregCustomerSaves.CustomerRecord> Records = new List<GregCustomerSaves.CustomerRecord>();
        public float SalaryExpense;
        public float RepairExpense;
        public float ShopExpense;
    }

    public sealed class BalanceSheet
    {
        public List<MonthlySnapshot> History = new List<MonthlySnapshot>();
        public float TotalMonthlySalary;
    }

    // ── MonthlySnapshot ──────────────────────────────────────────────────────

    public static global::Il2Cpp.MonthlySnapshotSaveData CreateSnapshot(MonthlySnapshot dto)
    {
        global::Il2Cpp.MonthlySnapshotSaveData entry = null;
        try { entry = new global::Il2Cpp.MonthlySnapshotSaveData(); } catch { return null; }
        FillSnapshot(entry, dto);
        return entry;
    }

    public static void FillSnapshot(global::Il2Cpp.MonthlySnapshotSaveData entry, MonthlySnapshot dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.month = dto.Month);
        Try(() => entry.day = dto.Day);
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.CustomerRecordSaveData>();
            foreach (var r in dto.Records ?? new List<GregCustomerSaves.CustomerRecord>())
            {
                try
                {
                    var e = GregCustomerSaves.CreateRecord(r);
                    if (e != null) list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.records = list;
        });
        Try(() => entry.salaryExpense = dto.SalaryExpense);
        Try(() => entry.repairExpense = dto.RepairExpense);
        Try(() => entry.shopExpense = dto.ShopExpense);
    }

    public static MonthlySnapshot ReadSnapshot(global::Il2Cpp.MonthlySnapshotSaveData entry)
    {
        var dto = new MonthlySnapshot();
        if (entry == null) return dto;
        Try(() => dto.Month = entry.month);
        Try(() => dto.Day = entry.day);
        Try(() =>
        {
            var list = entry.records;
            if (list == null) return;
            foreach (var e in list)
            {
                try { dto.Records.Add(GregCustomerSaves.ReadRecord(e)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        Try(() => dto.SalaryExpense = entry.salaryExpense);
        Try(() => dto.RepairExpense = entry.repairExpense);
        Try(() => dto.ShopExpense = entry.shopExpense);
        return dto;
    }

    // ── BalanceSheet ─────────────────────────────────────────────────────────

    public static global::Il2Cpp.BalanceSheetSaveData CreateSheet(BalanceSheet dto)
    {
        global::Il2Cpp.BalanceSheetSaveData entry = null;
        try { entry = new global::Il2Cpp.BalanceSheetSaveData(); } catch { return null; }
        FillSheet(entry, dto);
        return entry;
    }

    public static void FillSheet(global::Il2Cpp.BalanceSheetSaveData entry, BalanceSheet dto)
    {
        if (entry == null || dto == null) return;
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.MonthlySnapshotSaveData>();
            foreach (var s in dto.History ?? new List<MonthlySnapshot>())
            {
                try
                {
                    var e = CreateSnapshot(s);
                    if (e != null) list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.history = list;
        });
        Try(() => entry.totalMonthlySalary = dto.TotalMonthlySalary);
    }

    public static BalanceSheet ReadSheet(global::Il2Cpp.BalanceSheetSaveData entry)
    {
        var dto = new BalanceSheet();
        if (entry == null) return dto;
        Try(() =>
        {
            var list = entry.history;
            if (list == null) return;
            foreach (var e in list)
            {
                try { dto.History.Add(ReadSnapshot(e)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        Try(() => dto.TotalMonthlySalary = entry.totalMonthlySalary);
        return dto;
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] EconomySaves-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
