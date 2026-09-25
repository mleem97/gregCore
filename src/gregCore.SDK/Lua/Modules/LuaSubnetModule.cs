/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for subnet math (pure functions, no game).
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
        RegisterMaskFromCidr(subnetTable, modId);
        RegisterUsableIps(subnetTable, script, modId);
        RegisterFirstUsable(subnetTable, modId);
        greg["subnet"] = subnetTable;

        var setipTable = new Table(script);
        RegisterShowFor(setipTable, modId);
        RegisterCancel(setipTable);

        greg["setip"] = setipTable;
    }

    private static void RegisterMaskFromCidr(Table t, string modId)
    {

        // greg.subnet.mask_from_cidr(cidr) → string ("" on failure)
        t["mask_from_cidr"] = (Func<int, string>)((cidr) =>
        {
            try { return gregCore.Core.Networking.GregSetIP.MaskFromCidrManaged(cidr) ?? ""; }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] subnet.mask_from_cidr() failed: {ex.Message}");
                return "";
            }
        });
    }

    private static void RegisterUsableIps(Table t, Script script, string modId)
    {

        // greg.subnet.usable_ips(subnet) → array (caution with large networks!)
        t["usable_ips"] = (Func<string, Table>)((subnet) =>
        {
            try
            {
                var result = new Table(script);
                if (string.IsNullOrWhiteSpace(subnet)) return result;
                int i = 1;
                foreach (var ip in gregCore.Core.Networking.GregSetIP.GetUsableIPs(subnet))
                {
                    result[i++] = ip ?? "";
                    if (i > 65536) break; // hard cap against giant networks
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] subnet.usable_ips() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterFirstUsable(Table t, string modId)
    {

        // greg.subnet.first_usable(subnet) → string ("" when none)
        t["first_usable"] = (Func<string, string>)((subnet) =>
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
    }

    private static void RegisterShowFor(Table t, string modId)
    {

        // greg.setip.show_for(serverId) → bool (opens the vanilla keypad)
        t["show_for"] = (Func<string, bool>)((serverId) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(serverId);
                return s != null && gregCore.Core.Networking.GregSetIP.ShowFor(s);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] setip.show_for() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterCancel(Table t)
    {

        // greg.setip.cancel() → bool
        t["cancel"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregSetIP.Cancel(); }
            catch { return false; }
        });
    }
}
