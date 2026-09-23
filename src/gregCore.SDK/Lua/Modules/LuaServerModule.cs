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
                            result[i++] = info;
                        }
                        catch { }
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
                        catch { }
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
                        catch { }
                    }
                }
                return repaired;
            }
            catch { return 0; }
        });

        greg["server"] = serverTable;
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
        catch { }
        return null;
    }
}
