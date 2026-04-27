/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Cable/Kabel-Management.
/// Maintainer:   greg.cable.count(), get_next_id()
///               Delegiert an CablePositionsPatch für thread-safe ID-Generierung.
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.GameLayer.Patches.Networking;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaCableModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var cableTable = new Table(script);

        // greg.cable.count() → number
        cableTable["count"] = (Func<int>)(() =>
        {
            try
            {
                var cables = UnityEngine.Object.FindObjectsOfType<Il2Cpp.CablePositions>();
                return cables?.Count ?? 0;
            }
            catch { return 0; }
        });

        // greg.cable.get_next_id() → number
        cableTable["get_next_id"] = (Func<int>)(() =>
        {
            try { return CablePositionsPatch.PeekNextId(); }
            catch { return -1; }
        });

        // greg.cable.get_all() → table of cable info
        cableTable["get_all"] = (Func<Table>)(() =>
        {
            try
            {
                var cables = UnityEngine.Object.FindObjectsOfType<Il2Cpp.CablePositions>();
                var result = new Table(script);
                int i = 1;
                foreach (var c in cables)
                {
                    try
                    {
                        var info = new Table(script);
                        info["id"] = c.GetHashCode();
                        info["name"] = c.gameObject?.name ?? "Cable";
                        var pos = c.transform?.position ?? UnityEngine.Vector3.zero;
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
                MelonLogger.Error($"[LuaMod:{modId}] cable.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });

        greg["cable"] = cableTable;
    }
}
