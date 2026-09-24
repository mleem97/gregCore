/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Techniker (Bestand, Dispatch).
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

        // greg.tech.free_count() → number of idle technicians
        techTable["free_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetFreeTechnicianCount(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.free_count() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.total_count() → number of technicians
        techTable["total_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetTotalTechnicianCount(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.total_count() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.dispatch_server() → 1 if a repair was dispatched, else 0
        techTable["dispatch_server"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.DispatchRepairServer(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.dispatch_server() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.dispatch_switch() → 1 if a repair was dispatched, else 0
        techTable["dispatch_switch"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.DispatchRepairSwitch(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.dispatch_switch() failed: {ex.Message}");
                return 0;
            }
        });

        // greg.tech.list() → array of {id, name, salary, state, busy}
        techTable["list"] = (Func<Table>)(() =>
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

        // greg.tech.send_to_server(technicianId, serverId) → bool
        techTable["send_to_server"] = (Func<int, string, bool>)((techId, serverId) =>
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

        // greg.tech.send_to_switch(technicianId, switchId) → bool
        techTable["send_to_switch"] = (Func<int, string, bool>)((techId, switchId) =>
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

        // greg.tech.hire(index) → bool
        techTable["hire"] = (Func<int, bool>)((index) =>
        {
            try { return gregCore.Core.Networking.GregTechnicians.HireEmployee(index); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.hire() failed: {ex.Message}");
                return false;
            }
        });

        // greg.tech.fire(technicianId) → bool
        techTable["fire"] = (Func<int, bool>)((techId) =>
        {
            try { return gregCore.Core.Networking.GregTechnicians.FireTechnician(techId); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] tech.fire() failed: {ex.Message}");
                return false;
            }
        });

        greg["tech"] = techTable;
    }
}
