/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Subnetz-Mathematik (reine Funktionen, kein Spiel).
/// Maintainer:   greg.subnet.mask_from_cidr(), usable_ips(), first_usable()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaSubnetModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var subnetTable = new Table(script);

        // greg.subnet.mask_from_cidr(cidr) → string ("" on failure)
        subnetTable["mask_from_cidr"] = (Func<int, string>)((cidr) =>
        {
            try { return gregCore.Core.Networking.GregSetIP.MaskFromCidrManaged(cidr) ?? ""; }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] subnet.mask_from_cidr() failed: {ex.Message}");
                return "";
            }
        });

        // greg.subnet.usable_ips(subnet) → array (Vorsicht bei grossen Netzen!)
        subnetTable["usable_ips"] = (Func<string, Table>)((subnet) =>
        {
            try
            {
                var result = new Table(script);
                if (string.IsNullOrWhiteSpace(subnet)) return result;
                int i = 1;
                foreach (var ip in gregCore.Core.Networking.GregSetIP.GetUsableIPs(subnet))
                {
                    result[i++] = ip ?? "";
                    if (i > 65536) break; // harter Deckel gegen Riesennetze
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] subnet.usable_ips() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.subnet.first_usable(subnet) → string ("" when none)
        subnetTable["first_usable"] = (Func<string, string>)((subnet) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subnet)) return "";
                return gregCore.Core.Networking.GregSetIP.GetFirstUsableIP(subnet) ?? "";
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] subnet.first_usable() failed: {ex.Message}");
                return "";
            }
        });

        greg["subnet"] = subnetTable;
    }
}
