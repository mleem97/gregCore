/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Netzwerkgeräte aus dem Save (read-only).
///               Schreiben braucht C# (keine stabilen Live-Handles aus Lua).
/// Maintainer:   greg.net.routers(), firewalls(), sfps(), lacps(), cables()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaNetModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var netTable = new Table(script);

        // greg.net.routers() → array of {asn, next_route_id, routes, owned}
        netTable["routers"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var data = NetworkData();
                if (data?.routers == null) return result;
                int i = 1;
                foreach (var r in data.routers)
                {
                    try
                    {
                        if (r == null) continue;
                        var t = new Table(script);
                        int routes = 0, owned = 0;
                        try { routes = r.routes != null ? r.routes.Count : 0; } catch { }
                        try { owned = r.ownedSubnets != null ? r.ownedSubnets.Count : 0; } catch { }
                        int asn = 0, nextId = 0;
                        try { asn = r.asn; } catch { }
                        try { nextId = r.nextRouteId; } catch { }
                        t["asn"] = asn;
                        t["next_route_id"] = nextId;
                        t["routes"] = routes;
                        t["owned"] = owned;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] net.routers() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.net.firewalls() → array of {cluster_ip, rules}
        netTable["firewalls"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var data = NetworkData();
                if (data?.firewalls == null) return result;
                int i = 1;
                foreach (var f in data.firewalls)
                {
                    try
                    {
                        if (f == null) continue;
                        var t = new Table(script);
                        string cluster = "";
                        int rules = 0;
                        try { cluster = f.clusterIP ?? ""; } catch { }
                        try { rules = f.filterRules != null ? f.filterRules.Count : 0; } catch { }
                        t["cluster_ip"] = cluster;
                        t["rules"] = rules;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] net.firewalls() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.net.sfps() → array of {prefab, x, y, z, inserted}
        netTable["sfps"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var data = NetworkData();
                if (data?.sfpModules == null) return result;
                int i = 1;
                foreach (var s in data.sfpModules)
                {
                    try
                    {
                        if (s == null) continue;
                        var t = new Table(script);
                        int prefab = -1;
                        bool inserted = false;
                        try { prefab = s.prefabID; } catch { }
                        try { inserted = s.isInserted; } catch { }
                        t["prefab"] = prefab;
                        t["inserted"] = inserted;
                        try
                        {
                            var pos = s.position;
                            t["x"] = (double)pos.x;
                            t["y"] = (double)pos.y;
                            t["z"] = (double)pos.z;
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] net.sfps() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.net.lacps() → array of group ids
        netTable["lacps"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var data = NetworkData();
                if (data?.lacpGroups == null) return result;
                int i = 1;
                foreach (var g in data.lacpGroups)
                {
                    try
                    {
                        if (g == null) continue;
                        int gid = -1;
                        try { gid = g.groupId; } catch { }
                        result[i++] = gid;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch { return new Table(script); }
        });

        // greg.net.cables() → array of {id, maxspeed}
        netTable["cables"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var data = NetworkData();
                if (data?.cables == null) return result;
                int i = 1;
                foreach (var c in data.cables)
                {
                    try
                    {
                        if (c == null) continue;
                        var t = new Table(script);
                        int id = -1;
                        float speed = 0f;
                        try { id = c.cableID; } catch { }
                        try { speed = c.maxSpeed; } catch { }
                        t["id"] = id;
                        t["maxspeed"] = (double)speed;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] net.cables() failed: {ex.Message}");
                return new Table(script);
            }
        });

        greg["net"] = netTable;
    }

    internal static global::Il2Cpp.NetworkSaveData NetworkData()
    {
        try
        {
            var save = greg.Sdk.GregPublicAPI.GetSaveDataSafe();
            if (save == null) return null;
            var data = save.networkData;
            if (data == null || data.Pointer == IntPtr.Zero) return null;
            return data;
        }
        catch { return null; }
    }
}
