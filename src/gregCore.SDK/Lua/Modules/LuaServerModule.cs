/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Server-Management.
/// Maintainer:   greg.server.get_all(), power_on/off(), repair(), count()
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;
using Il2CppInterop.Runtime.InteropTypes;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaServerModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var serverTable = new Table(script);

        // greg.server.get_all() → table of server info
        serverTable["get_all"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    foreach (var kvp in nm.servers)
                    {
                        var s = kvp.Value;
                        if (s == null) continue;
                        try
                        {
                            var info = ServerToTable(script, s);
                            if (info != null) result[i++] = info;
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] server.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.server.get_list() → array of server IDs (alias-friendly)
        serverTable["get_list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    foreach (var kvp in nm.servers)
                    {
                        try
                        {
                            var s = kvp.Value;
                            if (s == null) continue;
                            string id = s.ServerID;
                            if (!string.IsNullOrEmpty(id)) result[i++] = id;
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                return result;
            }
            catch { return new Table(script); }
        });

        // greg.server.count() → number
        serverTable["count"] = (Func<int>)(() =>
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    return nm.servers.Count;
                }
                var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                return servers?.Count ?? 0;
            }
            catch { return 0; }
        });

        // greg.server.broken_count() → number
        serverTable["broken_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetBrokenServerCount(); }
            catch { return 0; }
        });

        // greg.server.repair(server_hash) → bool
        serverTable["repair"] = (Func<int, bool>)((hash) =>
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.brokenServers != null)
                {
                    foreach (var kvp in nm.brokenServers)
                    {
                        var s = kvp.Value;
                        if (s == null) continue;
                        try
                        {
                            if (s.GetHashCode() == hash && s.isBroken)
                            {
                                s.RepairDevice();
                                return true;
                            }
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                return false;
            }
            catch { return false; }
        });

        // greg.server.repair_all() → number of repaired
        serverTable["repair_all"] = (Func<int>)(() =>
        {
            try
            {
                int repaired = 0;
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.brokenServers != null)
                {
                    var brokenServers = new System.Collections.Generic.List<Il2Cpp.Server>();
                    foreach (var kvp in nm.brokenServers)
                    {
                        if (kvp.Value != null) brokenServers.Add(kvp.Value);
                    }
                    foreach (var s in brokenServers)
                    {
                        try
                        {
                            if (s.isBroken)
                            {
                                s.RepairDevice();
                                repaired++;
                            }
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                return repaired;
            }
            catch { return 0; }
        });

        // greg.server.find_by_id(id) → info table or nil
        serverTable["find_by_id"] = (Func<string, DynValue>)((id) =>
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return DynValue.Nil;
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                var info = s != null ? ServerToTable(script, s) : null;
                return info != null ? DynValue.FromObject(script, info) : DynValue.Nil;
            }
            catch { return DynValue.Nil; }
        });

        // greg.server.find_by_ip(ip) → info table or nil
        serverTable["find_by_ip"] = (Func<string, DynValue>)((ip) =>
        {
            try
            {
                if (string.IsNullOrEmpty(ip)) return DynValue.Nil;
                var s = gregCore.Core.Networking.GregServers.FindByIp(ip);
                var info = s != null ? ServerToTable(script, s) : null;
                return info != null ? DynValue.FromObject(script, info) : DynValue.Nil;
            }
            catch { return DynValue.Nil; }
        });

        // greg.server.power_on(id) / power_off(id) → bool
        serverTable["power_on"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetPower(s, true);
            }
            catch { return false; }
        });
        serverTable["power_off"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetPower(s, false);
            }
            catch { return false; }
        });

        // greg.server.set_ip(id, ip) → bool
        serverTable["set_ip"] = (Func<string, string, bool>)((id, ip) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetIP(s, ip);
            }
            catch { return false; }
        });

        // greg.server.set_customer(id, customerId) → bool
        serverTable["set_customer"] = (Func<string, int, bool>)((id, customerId) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.UpdateCustomer(s, customerId);
            }
            catch { return false; }
        });

        greg["server"] = serverTable;
    }

    internal static Table ServerToTable(Script script, Il2Cpp.INetworkEndpoint s)
    {
        try
        {
            if (s == null) return null;
            var info = new Table(script);
            info["id"] = s.ServerID ?? s.GetHashCode().ToString();
            info["hash"] = s.GetHashCode();
            info["is_on"] = s.isOn;
            // INetworkEndpoint verlor game-seitig Members (Drift):
            // isBroken/sizeInU nur noch auf konkretem Server bzw.
            // sizeInU nur noch auf Config-Typen (ShopItemConfig ...).
            // Per TryCast + Reflection proben, fehlendes auslassen.
            var concrete = s.TryCast<Il2Cpp.Server>();
            info["is_broken"] = concrete != null && concrete.isBroken;
            object? sizeU = TryReadMember(concrete ?? (object)s, "sizeInU");
            if (sizeU != null) info["size_u"] = Convert.ToInt32(sizeU);
            var pos = concrete != null && concrete.transform != null
                ? concrete.transform.position
                : UnityEngine.Vector3.zero;
            info["x"] = (double)pos.x;
            info["y"] = (double)pos.y;
            info["z"] = (double)pos.z;
            return info;
        }
        catch { return null; }
    }

    // Overload for concrete servers (e.g. GregServers.FindById results):
    // no probing needed, all members exist natively.
    internal static Table ServerToTable(Script script, Il2Cpp.Server s)
    {
        try
        {
            if (s == null) return null;
            var info = new Table(script);
            info["id"] = s.ServerID ?? s.GetHashCode().ToString();
            info["hash"] = s.GetHashCode();
            info["is_on"] = s.isOn;
            info["is_broken"] = s.isBroken;
            var pos = s.transform != null ? s.transform.position : UnityEngine.Vector3.zero;
            info["x"] = (double)pos.x;
            info["y"] = (double)pos.y;
            info["z"] = (double)pos.z;
            return info;
        }
        catch { return null; }
    }

    // Probed einen Member (Property oder Feld) zur Laufzeit. Null wenn das
    // Spiel ihn nicht mehr hat (API-Drift) - Aufrufer lassen den Key dann weg.
    private static object? TryReadMember(object target, string name)
    {
        try
        {
            var t = target.GetType();
            var prop = t.GetProperty(name);
            if (prop != null && prop.CanRead) return prop.GetValue(target);
            var field = t.GetField(name);
            if (field != null) return field.GetValue(target);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return null;
    }
}
