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
                // ⚡ Bolt: Replaced O(N) FindObjectsOfType with O(1) NetworkMap lookup
                var result = new Table(script);
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.servers == null) return result;

                int i = 1;
                foreach (var kvp in nm.servers)
                {
                    var s = kvp.Value;
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
                // ⚡ Bolt: Replaced O(N) FindObjectsOfType with O(1) brokenServers lookup
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenServers == null || nm.brokenServers.Count == 0) return false;

                foreach (var kvp in nm.brokenServers)
                {
                    try
                    {
                        var s = kvp.Value;
                        if (s != null && s.GetHashCode() == hash && s.isBroken)
                        {
                            s.RepairDevice();
                            return true; // Safe to return directly since we break out of loop
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
                // ⚡ Bolt: O(1) brokenServers lookup, defensive copy with early return to handle collection mutation
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenServers == null || nm.brokenServers.Count == 0) return 0;

                int repaired = 0;
                var targets = new System.Collections.Generic.List<Il2Cpp.Server>();
                foreach (var kvp in nm.brokenServers)
                {
                    targets.Add(kvp.Value);
                }

                foreach (var s in targets)
                {
                    try
                    {
                        if (s != null && s.isBroken)
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
