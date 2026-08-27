/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Server-Management.
/// Maintainer:   greg.server.get_all(), power_on/off(), repair(), count()
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

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
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.servers == null) return result;

                int i = 1;
                foreach (var kvp in nm.servers)
                {
                    var s = kvp.Value;
                    if (s == null) continue;
                    try
                    {
                        var info = new Table(script);
                        info["id"] = s.ServerID ?? s.GetHashCode().ToString();
                        info["hash"] = s.GetHashCode();
                        info["is_on"] = s.isOn;
                        info["is_broken"] = s.isBroken;
                        info["server_type"] = (int)s.serverType;
                        info["size_u"] = s.sizeInU;
                        var pos = s.transform?.position ?? UnityEngine.Vector3.zero;
                        info["x"] = (double)pos.x;
                        info["y"] = (double)pos.y;
                        info["z"] = (double)pos.z;
                        result[i++] = info;
                    }
                    catch { }
                }
                return result;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaMod:{modId}] server.get_all() failed: {ex.Message}");
                return new Table(script);
            }
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
                return 0; // Avoid expensive fallback
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
                if (nm == null || nm.servers == null) return false;

                foreach (var kvp in nm.servers)
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
                    catch { }
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
                if (nm == null || nm.servers == null) return 0;

                var serversToRepair = new System.Collections.Generic.List<Il2Cpp.Server>();
                foreach (var kvp in nm.servers)
                {
                    if (kvp.Value != null && kvp.Value.isBroken) serversToRepair.Add(kvp.Value);
                }

                if (serversToRepair.Count == 0) return 0;

                foreach (var s in serversToRepair)
                {
                    try
                    {
                        if (s.isBroken)
                        {
                            s.RepairDevice();
                            repaired++;
                        }
                    }
                    catch { }
                }
                return repaired;
            }
            catch { return 0; }
        });

        greg["server"] = serverTable;
    }
}
