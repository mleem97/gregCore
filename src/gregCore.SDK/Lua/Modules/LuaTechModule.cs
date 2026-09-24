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

        greg["tech"] = techTable;
    }
}
