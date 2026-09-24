/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Internet-Endpunkte und Command Center.
/// Maintainer:   greg.internet.endpoints(), command_center_level(),
///               auto_repair_mode(), set_auto_repair_mode()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaInternetModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var net = new Table(script);

        // greg.internet.endpoints() → array of {server, ip, type, app, max, current}
        net["endpoints"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var ep in gregCore.Core.Networking.GregInternet.ReadAllEndpoints())
                {
                    try
                    {
                        if (ep == null) continue;
                        var t = new Table(script);
                        t["server"] = ep.ServerID ?? "";
                        t["ip"] = ep.IP ?? "";
                        t["type"] = ep.ServerType;
                        t["app"] = ep.AppID;
                        t["max"] = (double)ep.MaxProcessingSpeed;
                        t["current"] = (double)ep.CurrentProcessingSpeed;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] internet.endpoints() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.internet.command_center_level() → number
        net["command_center_level"] = (Func<int>)(() =>
        {
            try { return gregCore.Core.Networking.GregInternet.GetCommandCenterLevel(); }
            catch { return 0; }
        });

        // greg.internet.auto_repair_mode() → number
        net["auto_repair_mode"] = (Func<int>)(() =>
        {
            try { return gregCore.Core.Networking.GregInternet.GetAutoRepairMode(); }
            catch { return -1; }
        });

        // greg.internet.set_auto_repair_mode(mode) → bool
        net["set_auto_repair_mode"] = (Func<int, bool>)((mode) =>
        {
            try { return gregCore.Core.Networking.GregInternet.SetAutoRepairMode(mode); }
            catch { return false; }
        });

        greg["internet"] = net;
    }
}
