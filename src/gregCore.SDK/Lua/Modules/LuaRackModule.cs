/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for rack management.
/// Maintainer:   greg.rack.get_all(), is_position_available(), place_server(), remove_server()
///               Reads/writes IL2CPP rack objects via FindObjectsOfType.
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
        RegisterGetAll(rackTable, script, modId);
        RegisterCount(rackTable);
        RegisterGetUsedCount(rackTable);
        RegisterPositionUsage(rackTable, script, modId);
        RegisterUnmount(rackTable, modId);
        RegisterPositionAllowed(rackTable, modId);
        RegisterMarkUsed(rackTable, modId);

        greg["rack"] = rackTable;
    }

    private static void RegisterGetAll(Table t, Script script, string modId)
    {

        // greg.rack.get_all() → table of rack info
        t["get_all"] = (Func<Table>)(() =>
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
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] rack.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterCount(Table t)
    {

        // greg.rack.count() → number
        t["count"] = (Func<int>)(() =>
        {
            try
            {
                var nm = Il2Cpp.NetworkMap.instance;
                if (nm != null)
                {
                    var arr = nm.GetNumberOfDevices();
                    if (arr != null && arr.Length > 2) return arr[2];
                }
                var racks = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Rack>();
                return racks?.Count ?? 0;
            }
            catch { return 0; }
        });

        // greg.rack.is_position_available(rack_id, position) → bool
        t["is_position_available"] = (Func<int, int, bool>)((rackId, position) =>
        {
            try
            {
                return RackPatch.GetUsedCount(rackId) == 0 || !IsPositionUsed(rackId, position);
            }
            catch { return false; }
        });
    }

    private static void RegisterGetUsedCount(Table t)
    {

        // greg.rack.get_used_count(rack_id) → number
        t["get_used_count"] = (Func<int, int>)((rackId) =>
        {
            try { return RackPatch.GetUsedCount(rackId); }
            catch { return 0; }
        });

        // greg.rack.position_count(rack_id) → number (-1 unknown rack)
        t["position_count"] = (Func<int, int>)((rackId) =>
        {
            try
            {
                var rack = FindRackByHash(rackId);
                if (rack == null) return -1;
                return gregCore.Core.Networking.GregRacks.GetPositionCount(rack);
            }
            catch { return -1; }
        });
    }

    private static void RegisterPositionUsage(Table t, Script script, string modId)
    {

        // greg.rack.position_usage(rack_id) → array of bool
        t["position_usage"] = (Func<int, Table>)((rackId) =>
        {
            try
            {
                var result = new Table(script);
                var rack = FindRackByHash(rackId);
                if (rack == null) return result;
                var usage = gregCore.Core.Networking.GregRacks.GetPositionUsage(rack);
                if (usage == null) return result;
                for (int i = 0; i < usage.Length; i++) result[i + 1] = usage[i];
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] rack.position_usage() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterUnmount(Table t, string modId)
    {

        // greg.rack.unmount(rack_id) → bool
        t["unmount"] = (Func<int, bool>)((rackId) =>
        {
            try
            {
                var rack = FindRackByHash(rackId);
                return rack != null && gregCore.Core.Networking.GregRacks.UnmountRack(rack);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] rack.unmount() failed: {ex.Message}");
                return false;
            }
        });

        // greg.rack.position_set_used(uid, used) → bool (RackPosition by UID)
        t["position_set_used"] = (Func<int, bool, bool>)((uid, used) =>
        {
            try
            {
                var pos = gregCore.Core.Networking.GregRacks.GetPositionByUID(uid);
                return pos != null && gregCore.Core.Networking.GregRacks.SetPositionUsed(pos, used);
            }
            catch { return false; }
        });
    }

    private static void RegisterPositionAllowed(Table t, string modId)
    {

        // greg.rack.position_allowed(uid) → bool
        t["position_allowed"] = (Func<int, bool>)((uid) =>
        {
            try
            {
                var pos = gregCore.Core.Networking.GregRacks.GetPositionByUID(uid);
                return pos != null && gregCore.Core.Networking.GregRacks.IsAllowedItem(pos, true);
            }
            catch { return false; }
        });

        // greg.rack.position_begin_insert(uid) → bool
        t["position_begin_insert"] = (Func<int, bool>)((uid) =>
        {
            try
            {
                var pos = gregCore.Core.Networking.GregRacks.GetPositionByUID(uid);
                return pos != null && gregCore.Core.Networking.GregRacks.BeginInsertItem(pos);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] rack.position_begin_insert() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterMarkUsed(Table t, string modId)
    {

        // greg.rack.mark_used(rack_id, position)
        t["mark_used"] = (Action<int, int>)((rackId, position) =>
        {
            try { RackPatch.MarkPositionUsed(rackId, position); }
            catch (Exception ex) { LuaLog.Error($"[LuaMod:{modId}] rack.mark_used failed: {ex.Message}"); }
        });

        // greg.rack.mark_free(rack_id, position)
        t["mark_free"] = (Action<int, int>)((rackId, position) =>
        {
            try { RackPatch.MarkPositionFree(rackId, position); }
            catch (Exception ex) { LuaLog.Error($"[LuaMod:{modId}] rack.mark_free failed: {ex.Message}"); }
        });
    }

    internal static Il2Cpp.Rack FindRackByHash(int rackId)
    {
        try
        {
            var racks = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Rack>();
            if (racks == null) return null;
            foreach (var rack in racks)
            {
                try { if (rack != null && rack.GetHashCode() == rackId) return rack; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            return null;
        }
        catch { return null; }
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
