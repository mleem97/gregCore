/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for server management.
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
        RegisterGetAll(serverTable, script, modId);
        RegisterGetList(serverTable, script);
        RegisterCount(serverTable);
        RegisterRepair(serverTable);
        RegisterRepairAll(serverTable);
        RegisterFindById(serverTable, script);
        RegisterPowerOn(serverTable);
        RegisterSetIp(serverTable);
        RegisterSetApp(serverTable);
        RegisterHasCable(serverTable);
        RegisterCapture(serverTable, script);
        RegisterInsertIntoRack(serverTable, modId);

        greg["server"] = serverTable;
    }

    private static void RegisterGetAll(Table t, Script script, string modId)
    {

        // greg.server.get_all() → table of server info
        t["get_all"] = (Func<Table>)(() =>
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
    }

    private static void RegisterGetList(Table t, Script script)
    {

        // greg.server.get_list() → array of server IDs (alias-friendly)
        t["get_list"] = (Func<Table>)(() =>
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
    }

    private static void RegisterCount(Table t)
    {

        // greg.server.count() → number
        t["count"] = (Func<int>)(() =>
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
        t["broken_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetBrokenServerCount(); }
            catch { return 0; }
        });
    }

    private static void RegisterRepair(Table t)
    {

        // greg.server.repair(server_hash) → bool
        t["repair"] = (Func<int, bool>)((hash) =>
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
    }

    private static void RegisterRepairAll(Table t)
    {

        // greg.server.repair_all() → number of repaired
        t["repair_all"] = (Func<int>)(() =>
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
    }

    private static void RegisterFindById(Table t, Script script)
    {

        // greg.server.find_by_id(id) → info table or nil
        t["find_by_id"] = (Func<string, DynValue>)((id) =>
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
        t["find_by_ip"] = (Func<string, DynValue>)((ip) =>
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
    }

    private static void RegisterPowerOn(Table t)
    {

        // greg.server.power_on(id) / power_off(id) → bool
        t["power_on"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetPower(s, true);
            }
            catch { return false; }
        });
        t["power_off"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetPower(s, false);
            }
            catch { return false; }
        });
    }

    private static void RegisterSetIp(Table t)
    {

        // greg.server.set_ip(id, ip) → bool
        t["set_ip"] = (Func<string, string, bool>)((id, ip) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.SetIP(s, ip);
            }
            catch { return false; }
        });

        // greg.server.set_customer(id, customerId) → bool
        t["set_customer"] = (Func<string, int, bool>)((id, customerId) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.UpdateCustomer(s, customerId);
            }
            catch { return false; }
        });
    }

    private static void RegisterSetApp(Table t)
    {

        // greg.server.set_app(id, appId) → bool
        t["set_app"] = (Func<string, int, bool>)((id, appId) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.UpdateAppID(s, appId);
            }
            catch { return false; }
        });

        // greg.server.clear_warning(id) → bool
        t["clear_warning"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.ClearWarning(s, true);
            }
            catch { return false; }
        });
    }

    private static void RegisterHasCable(Table t)
    {

        // greg.server.has_cable(id) → bool
        t["has_cable"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.IsAnyCableConnected(s);
            }
            catch { return false; }
        });

        // greg.server.valid_position(id) → bool
        t["valid_position"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                return s != null && gregCore.Core.Networking.GregServers.ValidateRackPosition(s);
            }
            catch { return false; }
        });
    }

    private static void RegisterCapture(Table t, Script script)
    {

        // greg.server.capture(id) → full snapshot table or nil
        t["capture"] = (Func<string, DynValue>)((id) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                if (s == null) return DynValue.Nil;
                var dto = gregCore.Core.Networking.GregServers.Capture(s);
                if (dto == null) return DynValue.Nil;
                var t = new Table(script);
                t["id"] = dto.ServerID ?? "";
                t["customer"] = dto.CustomerID;
                t["ip"] = dto.Ip ?? "";
                t["type"] = dto.ServerType;
                t["rack_uid"] = dto.RackPositionUID;
                t["prefab"] = dto.PrefabID;
                t["is_on"] = dto.IsOn;
                t["is_broken"] = dto.IsBroken;
                t["label"] = dto.Label ?? "";
                return DynValue.FromObject(script, t);
            }
            catch { return DynValue.Nil; }
        });
    }

    private static void RegisterInsertIntoRack(Table t, string modId)
    {

        // greg.server.insert_into_rack(id, spec) → bool (spec: rack_uid/prefab/is_on/...).
        t["insert_into_rack"] = (Func<string, DynValue, bool>)((id, spec) =>
        {
            try
            {
                var s = gregCore.Core.Networking.GregServers.FindById(id);
                if (s == null || spec == null || spec.Type != DataType.Table) return false;
                var dto = new gregCore.Core.Networking.GregServers.ServerSave();
                FillServerSaveFromTable(spec.Table, dto);
                return gregCore.Core.Networking.GregServers.InsertIntoRack(s, dto);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] server.insert_into_rack() failed: {ex.Message}");
                return false;
            }
        });
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
            // INetworkEndpoint lost members on the game side (drift):
            // isBroken/sizeInU only on the concrete server and/or
            // sizeInU only on config types (ShopItemConfig ...).
            // Probe via TryCast + reflection, omit missing ones.
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

    // Maps a Lua spec table onto a ServerSave DTO (vectors read as {x,y,z}).
    internal static void FillServerSaveFromTable(Table spec,
        gregCore.Core.Networking.GregServers.ServerSave dto)
    {
        try
        {
            if (spec == null || dto == null) return;
            string s = Str(spec, "id");
            if (s != null) dto.ServerID = s;
            int? i = Int(spec, "customer");
            if (i.HasValue) dto.CustomerID = i.Value;
            s = Str(spec, "ip");
            if (s != null) dto.Ip = s;
            i = Int(spec, "type");
            if (i.HasValue) dto.ServerType = i.Value;
            var pos = Vec(spec, "position");
            if (pos.HasValue) dto.Position = pos.Value;
            i = Int(spec, "rack_uid");
            if (i.HasValue) dto.RackPositionUID = i.Value;
            i = Int(spec, "prefab");
            if (i.HasValue) dto.PrefabID = i.Value;
            bool? b = Bool(spec, "is_on");
            if (b.HasValue) dto.IsOn = b.Value;
            b = Bool(spec, "is_broken");
            if (b.HasValue) dto.IsBroken = b.Value;
            s = Str(spec, "label");
            if (s != null) dto.Label = s;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    internal static string Str(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            return v.Type == DataType.String ? v.String : null;
        }
        catch { return null; }
    }

    internal static int? Int(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            if (v.Type == DataType.Number) return (int)v.Number;
            return null;
        }
        catch { return null; }
    }

    internal static bool? Bool(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            if (v.Type == DataType.Boolean) return v.Boolean;
            return null;
        }
        catch { return null; }
    }

    internal static UnityEngine.Vector3? Vec(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            if (v == null || v.Type != DataType.Table) return null;
            float x = (float)(v.Table.Get("x").Type == DataType.Number ? v.Table.Get("x").Number : 0.0);
            float y = (float)(v.Table.Get("y").Type == DataType.Number ? v.Table.Get("y").Number : 0.0);
            float z = (float)(v.Table.Get("z").Type == DataType.Number ? v.Table.Get("z").Number : 0.0);
            return new UnityEngine.Vector3(x, y, z);
        }
        catch { return null; }
    }
    // Probes a member (property or field) at runtime. Null when the
    // game no longer has it (API drift) - callers then omit the key.
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
