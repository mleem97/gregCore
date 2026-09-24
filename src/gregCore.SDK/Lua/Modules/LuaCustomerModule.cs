/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for customers (reading bases, IP lookup, subnet routes).
/// Maintainer:   greg.customer.bases(), is_ip_present(), app_id_for_ip(),
///               register_subnet(), unregister_subnet()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaCustomerModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var customerTable = new Table(script);
        RegisterBases(customerTable, script, modId);
        RegisterIsIpPresent(customerTable);
        RegisterRegisterSubnet(customerTable, modId);
        RegisterUnregisterSubnet(customerTable, modId);

        greg["customer"] = customerTable;
    }

    private static void RegisterBases(Table t, Script script, string modId)
    {

        // greg.customer.bases() → array of customer base info
        t["bases"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var dto in gregCore.Core.Networking.GregCustomers.ReadAllBases())
                {
                    try
                    {
                        if (dto == null) continue;
                        result[i++] = BaseToTable(script, dto);
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] customer.bases() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.customer.find_base(baseId) → info table or nil
        t["find_base"] = (Func<int, DynValue>)((baseId) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                if (cb == null) return DynValue.Nil;
                var dto = gregCore.Core.Networking.GregCustomers.ReadBase(cb);
                if (dto == null) return DynValue.Nil;
                return DynValue.FromObject(script, BaseToTable(script, dto));
            }
            catch { return DynValue.Nil; }
        });
    }

    private static void RegisterIsIpPresent(Table t)
    {

        // greg.customer.is_ip_present(baseId, ip) → bool
        t["is_ip_present"] = (Func<int, string, bool>)((baseId, ip) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                return cb != null && gregCore.Core.Networking.GregCustomers.IsIPPresent(cb, ip);
            }
            catch { return false; }
        });

        // greg.customer.app_id_for_ip(baseId, ip) → number (-1 when unknown)
        t["app_id_for_ip"] = (Func<int, string, int>)((baseId, ip) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                if (cb == null) return -1;
                return gregCore.Core.Networking.GregCustomers.GetAppIDForIP(cb, ip);
            }
            catch { return -1; }
        });
    }

    private static void RegisterRegisterSubnet(Table t, string modId)
    {

        // greg.customer.register_subnet(baseId, vlanId, routeKey, ips) → bool
        t["register_subnet"] = (Func<int, int, string, DynValue, bool>)((baseId, vlanId, routeKey, ips) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                if (cb == null || string.IsNullOrEmpty(routeKey)) return false;
                var list = new System.Collections.Generic.List<string>();
                if (ips != null && ips.Type == DataType.Table)
                {
                    foreach (var pair in ips.Table.Pairs)
                    {
                        try
                        {
                            string s = pair.Value.Type == DataType.String
                                ? pair.Value.String
                                : pair.Value.ToString();
                            if (!string.IsNullOrEmpty(s)) list.Add(s);
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                return gregCore.Core.Networking.GregCustomers.TryRegisterRoutedSubnet(cb, vlanId, routeKey, list);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] customer.register_subnet() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterUnregisterSubnet(Table t, string modId)
    {

        // greg.customer.unregister_subnet(baseId, routeKey) → bool
        t["unregister_subnet"] = (Func<int, string, bool>)((baseId, routeKey) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                return cb != null && gregCore.Core.Networking.GregCustomers.TryUnregisterRoutedSubnet(cb, routeKey);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] customer.unregister_subnet() failed: {ex.Message}");
                return false;
            }
        });
    }

    internal static Table BaseToTable(Script script,
        gregCore.Core.Networking.GregCustomers.BaseInfo dto)
    {
        try
        {
            if (dto == null) return null;
            var t = new Table(script);
            t["base_id"] = dto.CustomerBaseID;
            t["customer_id"] = dto.CustomerID;
            t["money_speed"] = (double)dto.EffectiveMoneySpeed;
            t["all_met"] = dto.AllRequirementsMet;
            t["wants_internet"] = dto.WantsInternet;
            t["satisfied"] = dto.WasFullySatisfied;
            return t;
        }
        catch { return null; }
    }

    internal static Il2Cpp.CustomerBase FindBaseById(int baseId)
    {
        try
        {
            foreach (var cb in gregCore.Core.Networking.GregCustomers.FindAllBases())
            {
                if (cb == null) continue;
                int id = -1;
                try { id = cb.customerBaseID; } catch { continue; }
                if (id == baseId) return cb;
            }
            return null;
        }
        catch { return null; }
    }
}
