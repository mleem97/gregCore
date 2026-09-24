/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for technicians (inventory, dispatch).
/// Maintainer:   greg.tech.free_count(), total_count(), dispatch_server(),
///               dispatch_switch()
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaTechModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var techTable = new Table(script);
        RegisterFreeCount(techTable, modId);
        RegisterDispatchServer(techTable, modId);
        RegisterList(techTable, script, modId);
        RegisterSendToServer(techTable);
        RegisterSendToSwitch(techTable);
        RegisterHire(techTable, modId);
        RegisterRequestNextJob(techTable);

        greg["tech"] = techTable;
    }

    private static void RegisterFreeCount(Table t, string modId)
    {

        // greg.tech.free_count() → number of idle technicians
        t["free_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetFreeTechnicianCount(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.free_count() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.total_count() → number of technicians
        t["total_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetTotalTechnicianCount(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.total_count() failed: {ex.Message}");
                return 0;
            }
        });
    }

    private static void RegisterDispatchServer(Table t, string modId)
    {

        // greg.tech.dispatch_server() → 1 if a repair was dispatched, else 0
        t["dispatch_server"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.DispatchRepairServer(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.dispatch_server() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.dispatch_switch() → 1 if a repair was dispatched, else 0
        t["dispatch_switch"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.DispatchRepairSwitch(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.dispatch_switch() failed: {ex.Message}");
                return 0;
            }
        });
    }

    private static void RegisterList(Table t, Script script, string modId)
    {

        // greg.tech.list() → array of {id, name, salary, state, busy}
        t["list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var info in gregCore.Core.Networking.GregTechnicians.ReadAll())
                {
                    try
                    {
                        if (info == null) continue;
                        var t = new Table(script);
                        t["id"] = info.TechnicianID;
                        t["name"] = info.Name ?? "";
                        t["salary"] = (double)info.Salary;
                        t["state"] = info.State ?? "";
                        t["busy"] = info.IsBusy;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.list() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterSendToServer(Table t)
    {

        // greg.tech.send_to_server(technicianId, serverId) → bool
        t["send_to_server"] = (Func<int, string, bool>)((techId, serverId) =>
        {
            try
            {
                var tech = gregCore.Core.Networking.GregTechnicians.FindByID(techId);
                var server = gregCore.Core.Networking.GregServers.FindById(serverId);
                return tech != null && server != null &&
                    gregCore.Core.Networking.GregTechnicians.SendTechnician(null, server);
            }
            catch { return false; }
        });
    }

    private static void RegisterSendToSwitch(Table t)
    {

        // greg.tech.send_to_switch(technicianId, switchId) → bool
        t["send_to_switch"] = (Func<int, string, bool>)((techId, switchId) =>
        {
            try
            {
                var tech = gregCore.Core.Networking.GregTechnicians.FindByID(techId);
                if (tech == null) return false;
                global::Il2Cpp.NetworkSwitch target = null;
                foreach (var sw in LuaSwitchModule.FindAllSwitches())
                {
                    string sid = null;
                    try { sid = sw.switchId; } catch { continue; }
                    if (string.Equals(sid, switchId, StringComparison.OrdinalIgnoreCase))
                    {
                        target = sw;
                        break;
                    }
                }
                return target != null &&
                    gregCore.Core.Networking.GregTechnicians.SendTechnician(target, null);
            }
            catch { return false; }
        });
    }

    private static void RegisterHire(Table t, string modId)
    {

        // greg.tech.hire(index) → bool
        t["hire"] = (Func<int, bool>)((index) =>
        {
            try { return gregCore.Core.Networking.GregTechnicians.HireEmployee(index); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.hire() failed: {ex.Message}");
                return false;
            }
        });

        // greg.tech.fire(technicianId) → bool
        t["fire"] = (Func<int, bool>)((techId) =>
        {
            try { return gregCore.Core.Networking.GregTechnicians.FireTechnician(techId); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.fire() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterRequestNextJob(Table t)
    {

        // greg.tech.request_next_job(techId) → bool
        t["request_next_job"] = (Func<int, bool>)((techId) =>
        {
            try
            {
                var tech = gregCore.Core.Networking.GregTechnicians.FindByID(techId);
                return tech != null &&
                    gregCore.Core.Networking.GregTechnicians.RequestNextJob(tech);
            }
            catch { return false; }
        });
    }
}
