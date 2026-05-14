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
                // Optimization: Use NetworkMap singleton instead of expensive FindObjectsOfType
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.servers == null) return new Table(script);

                var result = new Table(script);
                int i = 1;
                foreach (var kvp in nm.servers)
                {
                    var s = kvp.Value;
                    if (s == null || s.Pointer == IntPtr.Zero) continue;
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
                // Optimization: Fast O(1) lookup via GregAPI
                return (int)API.GregAPI.GetServerCount();
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
                // Optimization: Iterate only broken servers instead of all objects
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenServers == null) return false;

                foreach (var kvp in nm.brokenServers)
                {
                    try
                    {
                        var s = kvp.Value;
                        if (s != null && s.Pointer != IntPtr.Zero && s.GetHashCode() == hash && s.isBroken)
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
                // Optimization: Use brokenServers singleton, copy keys to avoid modification during iteration
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm == null || nm.brokenServers == null) return 0;

                var keys = new System.Collections.Generic.List<string>();
                foreach (var kvp in nm.brokenServers) keys.Add(kvp.Key);

                foreach (var key in keys)
                {
                    try
                    {
                        var s = nm.brokenServers[key];
                        if (s != null && s.Pointer != IntPtr.Zero && s.isBroken)
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
