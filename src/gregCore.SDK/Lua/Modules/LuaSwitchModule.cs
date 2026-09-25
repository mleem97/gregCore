/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for switch management (mirror of LuaServerModule).
/// Maintainer:   greg.switch.get_all(), count(), broken_count(), repair(),
///               repair_all(), find_by_id()
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaSwitchModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var switchTable = new Table(script);
        RegisterGetAll(switchTable, script, modId);
        RegisterGetList(switchTable, script);
        RegisterCount(switchTable);
        RegisterFindById(switchTable, script);
        RegisterRepair(switchTable);

        greg["switch"] = switchTable;
    }

    private static void RegisterGetAll(Table t, Script script, string modId)
    {

        // greg.switch.get_all() → table of switch info
        t["get_all"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var sw in FindAllSwitches())
                {
                    try
                    {
                        var info = SwitchToTable(script, sw);
                        if (info != null) result[i++] = info;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] switch.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterGetList(Table t, Script script)
    {

        // greg.switch.get_list() → array of switch IDs (alias-friendly)
        t["get_list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var sw in FindAllSwitches())
                {
                    try
                    {
                        string id = null;
                        try { id = sw.switchId; } catch { continue; }
                        if (!string.IsNullOrEmpty(id)) result[i++] = id;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch { return new Table(script); }
        });
    }

    private static void RegisterCount(Table t)
    {

        // greg.switch.count() → number
        t["count"] = (Func<int>)(() =>
        {
            try
            {
                int n = 0;
                foreach (var sw in FindAllSwitches()) n++;
                return n;
            }
            catch { return 0; }
        });

        // greg.switch.broken_count() → number
        t["broken_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetBrokenSwitchCount(); }
            catch { return 0; }
        });
    }

    private static void RegisterFindById(Table t, Script script)
    {

        // greg.switch.find_by_id(id) → info table or nil
        t["find_by_id"] = (Func<string, DynValue>)((id) =>
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return DynValue.Nil;
                foreach (var sw in FindAllSwitches())
                {
                    string sid = null;
                    try { sid = sw.switchId; } catch { continue; }
                    if (string.Equals(sid, id, StringComparison.OrdinalIgnoreCase))
                    {
                        var info = SwitchToTable(script, sw);
                        return info != null ? DynValue.FromObject(script, info) : DynValue.Nil;
                    }
                }
                return DynValue.Nil;
            }
            catch { return DynValue.Nil; }
        });
    }

    private static void RegisterRepair(Table t)
    {

        // greg.switch.repair(id) → bool
        t["repair"] = (Func<string, bool>)((id) =>
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return false;
                foreach (var sw in FindAllSwitches())
                {
                    string sid = null;
                    try { sid = sw.switchId; } catch { continue; }
                    if (!string.Equals(sid, id, StringComparison.OrdinalIgnoreCase)) continue;
                    try
                    {
                        if (sw.isBroken) { sw.RepairDevice(); return true; }
                        return false;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return false;
            }
            catch { return false; }
        });

        // greg.switch.repair_all() → number of repaired
        t["repair_all"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.DispatchRepairSwitch(); }
            catch { return 0; }
        });
    }

    internal static System.Collections.Generic.List<Il2Cpp.NetworkSwitch> FindAllSwitches()
    {
        var list = new System.Collections.Generic.List<Il2Cpp.NetworkSwitch>();
        try
        {
            var nm = Il2Cpp.NetworkMap.instance;
            if (nm != null && nm.switches != null)
            {
                foreach (var kvp in nm.switches)
                {
                    try { if (kvp.Value != null) list.Add(kvp.Value); }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                if (list.Count > 0) return list;
            }
            var found = UnityEngine.Object.FindObjectsOfType<Il2Cpp.NetworkSwitch>();
            if (found != null)
            {
                foreach (var sw in found)
                {
                    try { if (sw != null) list.Add(sw); }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return list;
    }

    internal static Table SwitchToTable(Script script, Il2Cpp.NetworkSwitch sw)
    {
        try
        {
            if (sw == null) return null;
            var info = new Table(script);
            string id = "";
            try { id = sw.switchId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            info["id"] = string.IsNullOrEmpty(id) ? sw.GetHashCode().ToString() : id;
            info["hash"] = sw.GetHashCode();
            try { info["is_on"] = sw.isOn; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { info["is_broken"] = sw.isBroken; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try
            {
                var pos = sw.transform != null ? sw.transform.position : UnityEngine.Vector3.zero;
                info["x"] = (double)pos.x;
                info["y"] = (double)pos.y;
                info["z"] = (double)pos.z;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return info;
        }
        catch { return null; }
    }
}
