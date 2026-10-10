/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for cable management.
/// Maintainer:   greg.cable.count(), get_next_id()
///               Delegates to CablePositionsPatch for thread-safe ID generation.
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
        RegisterCount(cableTable);
        RegisterGetAll(cableTable, script, modId);
        RegisterLinks(cableTable, script, modId);
        RegisterSetSpeed(cableTable, modId);
        RegisterSecondAction(cableTable);

        greg["cable"] = cableTable;
    }

    private static void RegisterCount(Table t)
    {

        // greg.cable.count() → number
        t["count"] = (Func<int>)(() =>
        {
            try
            {
                var cables = UnityEngine.Object.FindObjectsOfType<Il2Cpp.CablePositions>();
                return cables?.Count ?? 0;
            }
            catch { return 0; }
        });

        // greg.cable.get_next_id() → number
        t["get_next_id"] = (Func<int>)(() =>
        {
            try { return CablePositionsPatch.PeekNextId(); }
            catch { return -1; }
        });
    }

    private static void RegisterGetAll(Table t, Script script, string modId)
    {

        // greg.cable.get_all() → table of cable info
        t["get_all"] = (Func<Table>)(() =>
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
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] cable.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterLinks(Table t, Script script, string modId)
    {

        // greg.cable.links() → array of port links {_ref, type, speed, sfp, parent ids}.
        // _ref is enumeration order (best-effort: re-resolve per call, abort on count shift).
        t["links"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var links = gregCore.Core.Networking.GregCables.FindAll();
                for (int i = 0; i < links.Count; i++)
                {
                    try
                    {
                        var dto = gregCore.Core.Networking.GregCables.Read(links[i]);
                        if (dto == null) continue;
                        var t = new Table(script);
                        t["_ref"] = i + 1;
                        t["type"] = dto.Type ?? "";
                        t["speed"] = (double)dto.ConnectionSpeed;
                        t["sfp_in"] = dto.SfpTypeInserted;
                        t["sfp_supported"] = dto.SfpTypeSupported;
                        t["is_fibre"] = dto.IsFibrePort;
                        t["cabled"] = dto.CableIDsOnLink != 0;
                        t["parent_server"] = dto.ParentServerID ?? "";
                        t["parent_switch"] = dto.ParentSwitchID ?? "";
                        t["parent_patch"] = dto.ParentPatchPanelID ?? "";
                        result[i + 1] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] cable.links() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterSetSpeed(Table t, string modId)
    {

        // greg.cable.set_speed(ref, speed) → bool
        t["set_speed"] = (Func<int, double, bool>)((r, speed) =>
        {
            try
            {
                var link = LinkAt(r);
                return link != null &&
                    gregCore.Core.Networking.GregCables.SetConnectionSpeed(link, (float)speed);
            }
            catch { return false; }
        });

        // greg.cable.remove_sfp(ref) → bool
        t["remove_sfp"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var link = LinkAt(r);
                return link != null &&
                    gregCore.Core.Networking.GregCables.RemoveSFP(link);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] cable.remove_sfp() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterSecondAction(Table t)
    {

        // greg.cable.second_action(ref) → bool
        t["second_action"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var link = LinkAt(r);
                return link != null &&
                    gregCore.Core.Networking.GregCables.SecondAction(link);
            }
            catch { return false; }
        });

        // greg.cable.label_action(ref) → bool
        t["label_action"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var link = LinkAt(r);
                return link != null &&
                    gregCore.Core.Networking.GregCables.LabelAction(link);
            }
            catch { return false; }
        });
    }

    // Resolves a _ref from links() against a fresh enumeration.
    // Null when out of range (scene changed between calls).
    internal static Il2Cpp.CableLink LinkAt(int r)
    {
        try
        {
            if (r < 1) return null;
            var links = gregCore.Core.Networking.GregCables.FindAll();
            if (links == null || r > links.Count) return null;
            var link = links[r - 1];
            if (link == null) return null;
            try
            {
                var go = link.gameObject;
                if (go == null) return null;
            }
            catch { return null; }
            return link;
        }
        catch { return null; }
    }
}
