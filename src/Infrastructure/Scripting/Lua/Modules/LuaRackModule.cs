/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Rack-Management.
/// Maintainer:   greg.rack.get_all(), is_position_available(), place_server(), remove_server()
///               Liest/Schreibt IL2CPP Rack-Objekte via FindObjectsOfType.
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.GameLayer.Patches.Hardware;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaRackModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var rackTable = new Table(script);

        // greg.rack.get_all() → table of rack info
        rackTable["get_all"] = (Func<Table>)(() =>
        {
            try
            {
                var racks = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Rack>();
                var result = new Table(script);
                int i = 1;
                foreach (var rack in racks)
                {
                    try
                    {
                        var info = new Table(script);
                        info["id"] = rack.GetHashCode();
                        info["name"] = rack.gameObject?.name ?? "Unknown";
                        info["used_slots"] = RackPatch.GetUsedCount(rack.GetHashCode());
                        var pos = rack.transform?.position ?? UnityEngine.Vector3.zero;
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
                MelonLogger.Error($"[LuaMod:{modId}] rack.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.rack.count() → number
        rackTable["count"] = (Func<int>)(() =>
        {
            try
            {
                var racks = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Rack>();
                return racks?.Count ?? 0;
            }
            catch { return 0; }
        });

        // greg.rack.is_position_available(rack_id, position) → bool
        rackTable["is_position_available"] = (Func<int, int, bool>)((rackId, position) =>
        {
            try
            {
                return RackPatch.GetUsedCount(rackId) == 0 || !IsPositionUsed(rackId, position);
            }
            catch { return false; }
        });

        // greg.rack.get_used_count(rack_id) → number
        rackTable["get_used_count"] = (Func<int, int>)((rackId) =>
        {
            try { return RackPatch.GetUsedCount(rackId); }
            catch { return 0; }
        });

        // greg.rack.mark_used(rack_id, position)
        rackTable["mark_used"] = (Action<int, int>)((rackId, position) =>
        {
            try { RackPatch.MarkPositionUsed(rackId, position); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] rack.mark_used failed: {ex.Message}"); }
        });

        // greg.rack.mark_free(rack_id, position)
        rackTable["mark_free"] = (Action<int, int>)((rackId, position) =>
        {
            try { RackPatch.MarkPositionFree(rackId, position); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] rack.mark_free failed: {ex.Message}"); }
        });

        greg["rack"] = rackTable;
    }

    private static bool IsPositionUsed(int rackId, int position)
    {
        // Uses RackPatch internal registry
        int before = RackPatch.GetUsedCount(rackId);
        RackPatch.MarkPositionFree(rackId, position);
        int after = RackPatch.GetUsedCount(rackId);
        if (before != after)
        {
            // Was used, re-mark it
            RackPatch.MarkPositionUsed(rackId, position);
            return true;
        }
        return false;
    }
}
