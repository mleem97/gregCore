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
                int i = 1;

                Action<Il2Cpp.Server> processServer = (s) =>
                {
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
                };

                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    foreach (var kvp in nm.servers)
                    {
                        processServer(kvp.Value);
                    }
                }
                else
                {
                    var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                    if (servers != null)
                    {
                        foreach (var s in servers)
                        {
                            processServer(s);
                        }
                    }
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
                if (nm != null)
                {
                    var counts = nm.GetNumberOfDevices();
                    if (counts != null && counts.Length > 0)
                    {
                        return counts[0];
                    }
                }
                var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                return servers?.Length ?? 0;
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
                bool repaired = false;
                Action<Il2Cpp.Server> processServer = (s) =>
                {
                    if (repaired) return;
                    try
                    {
                        if (s.GetHashCode() == hash && s.isBroken)
                        {
                            s.RepairDevice();
                            repaired = true;
                        }
                    }
                    catch { }
                };

                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    foreach (var kvp in nm.servers)
                    {
                        processServer(kvp.Value);
                        if (repaired) return true;
                    }
                }
                else
                {
                    var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                    if (servers != null)
                    {
                        foreach (var s in servers)
                        {
                            processServer(s);
                            if (repaired) return true;
                        }
                    }
                }
                return repaired;
            }
            catch { return false; }
        });

        // greg.server.repair_all() → number of repaired
        serverTable["repair_all"] = (Func<int>)(() =>
        {
            try
            {
                int repaired = 0;
                Action<Il2Cpp.Server> processServer = (s) =>
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
                };

                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null && nm.servers != null)
                {
                    foreach (var kvp in nm.servers)
                    {
                        processServer(kvp.Value);
                    }
                }
                else
                {
                    var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
                    if (servers != null)
                    {
                        foreach (var s in servers)
                        {
                            processServer(s);
                        }
                    }
                }
                return repaired;
            }
            catch { return 0; }
        });

        greg["server"] = serverTable;
    }
}
