/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Kunden (Bases lesen, IP-Lookup, Subnetz-Routen).
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

        // greg.customer.bases() → array of customer base info
        customerTable["bases"] = (Func<Table>)(() =>
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
                        var t = new Table(script);
                        t["base_id"] = dto.CustomerBaseID;
                        t["customer_id"] = dto.CustomerID;
                        t["money_speed"] = (double)dto.EffectiveMoneySpeed;
                        t["all_met"] = dto.AllRequirementsMet;
                        t["wants_internet"] = dto.WantsInternet;
                        t["satisfied"] = dto.WasFullySatisfied;
                        result[i++] = t;
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

        // greg.customer.is_ip_present(baseId, ip) → bool
        customerTable["is_ip_present"] = (Func<int, string, bool>)((baseId, ip) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                return cb != null && gregCore.Core.Networking.GregCustomers.IsIPPresent(cb, ip);
            }
            catch { return false; }
        });

        // greg.customer.app_id_for_ip(baseId, ip) → number (-1 when unknown)
        customerTable["app_id_for_ip"] = (Func<int, string, int>)((baseId, ip) =>
        {
            try
            {
                var cb = FindBaseById(baseId);
                if (cb == null) return -1;
                return gregCore.Core.Networking.GregCustomers.GetAppIDForIP(cb, ip);
            }
            catch { return -1; }
        });

        // greg.customer.register_subnet(baseId, vlanId, routeKey, ips) → bool
        customerTable["register_subnet"] = (Func<int, int, string, DynValue, bool>)((baseId, vlanId, routeKey, ips) =>
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

        // greg.customer.unregister_subnet(baseId, routeKey) → bool
        customerTable["unregister_subnet"] = (Func<int, string, bool>)((baseId, routeKey) =>
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

        greg["customer"] = customerTable;
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
