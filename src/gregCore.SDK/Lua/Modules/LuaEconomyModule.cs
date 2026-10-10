/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for economy data (reading balance sheet, read-only).
/// Maintainer:   greg.economy.sheet(), history(). The save graph is resolved via
///               probing (API-drift tolerant), all best-effort.
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaEconomyModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var economyTable = new Table(script);
        RegisterSheet(economyTable, script, modId);
        RegisterHistory(economyTable, script, modId);

        greg["economy"] = economyTable;
    }

    private static void RegisterSheet(Table t, Script script, string modId)
    {

        // greg.economy.sheet() → { total_salary, months } or nil
        t["sheet"] = (Func<DynValue>)(() =>
        {
            try
            {
                var sheet = ReadBalanceSheet();
                if (sheet == null) return DynValue.Nil;
                var t = new Table(script);
                t["total_salary"] = (double)sheet.TotalMonthlySalary;
                int months = 0;
                try { months = sheet.History != null ? sheet.History.Count : 0; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                t["months"] = months;
                return DynValue.FromObject(script, t);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] economy.sheet() failed: {ex.Message}");
                return DynValue.Nil;
            }
        });
    }

    private static void RegisterHistory(Table t, Script script, string modId)
    {

        // greg.economy.history() → array of { month, day, salary, repair, shop }
        t["history"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var sheet = ReadBalanceSheet();
                if (sheet?.History == null) return result;
                int i = 1;
                foreach (var snap in sheet.History)
                {
                    try
                    {
                        if (snap == null) continue;
                        var t = new Table(script);
                        t["month"] = snap.Month;
                        t["day"] = snap.Day;
                        t["salary"] = (double)snap.SalaryExpense;
                        t["repair"] = (double)snap.RepairExpense;
                        t["shop"] = (double)snap.ShopExpense;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] economy.history() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    internal static gregCore.Core.Networking.GregEconomySaves.BalanceSheet ReadBalanceSheet()
    {
        try
        {
            var save = greg.Sdk.GregPublicAPI.GetSaveDataSafe();
            if (save == null) return null;
            // Resolve the save graph via probing (field names vary by build type).
            object sheetData = ProbeMember(save, "balanceSheetData")
                ?? ProbeMember(save, "balanceSheet");
            if (sheetData is global::Il2Cpp.BalanceSheetSaveData typed)
                return gregCore.Core.Networking.GregEconomySaves.ReadSheet(typed);
            return null;
        }
        catch { return null; }
    }

    private static object ProbeMember(object target, string name)
    {
        try
        {
            if (target == null || string.IsNullOrEmpty(name)) return null;
            var t = target.GetType();
            var prop = t.GetProperty(name);
            if (prop != null && prop.CanRead) return prop.GetValue(target);
            var field = t.GetField(name);
            if (field != null) return field.GetValue(target);
            return null;
        }
        catch { return null; }
    }
}
