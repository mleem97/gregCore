/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for patch panels (mirror of LuaSwitchModule).
/// Maintainer:   greg.patch.get_all(), get_list(), count(), find_by_id(),
///               has_cable()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaPatchModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var patchTable = new Table(script);
        RegisterGetAll(patchTable, script, modId);
        RegisterGetList(patchTable, script);
        RegisterFindById(patchTable, script);
        RegisterValidPosition(patchTable, script);
        RegisterInsertIntoRack(patchTable, modId);

        greg["patch"] = patchTable;
    }

    private static void RegisterGetAll(Table t, Script script, string modId)
    {

        // greg.patch.get_all() → table of patch panel info
        t["get_all"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var pp in FindAllPanels())
                {
                    try
                    {
                        var info = PanelToTable(script, pp);
                        if (info != null) result[i++] = info;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] patch.get_all() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterGetList(Table t, Script script)
    {

        // greg.patch.get_list() → array of patch panel IDs
        t["get_list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var pp in FindAllPanels())
                {
                    try
                    {
                        string id = null;
                        try { id = pp.patchPanelId; } catch { continue; }
                        if (!string.IsNullOrEmpty(id)) result[i++] = id;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch { return new Table(script); }
        });

        // greg.patch.count() → number
        t["count"] = (Func<int>)(() =>
        {
            try { return FindAllPanels().Count; }
            catch { return 0; }
        });
    }

    private static void RegisterFindById(Table t, Script script)
    {

        // greg.patch.find_by_id(id) → info table or nil
        t["find_by_id"] = (Func<string, DynValue>)((id) =>
        {
            try
            {
                var pp = FindById(id);
                var info = pp != null ? PanelToTable(script, pp) : null;
                return info != null ? DynValue.FromObject(script, info) : DynValue.Nil;
            }
            catch { return DynValue.Nil; }
        });

        // greg.patch.has_cable(id) → bool
        t["has_cable"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var pp = FindById(id);
                return pp != null && gregCore.Core.Networking.GregPatchPanels.IsAnyCableConnected(pp);
            }
            catch { return false; }
        });
    }

    private static void RegisterValidPosition(Table t, Script script)
    {

        // greg.patch.valid_position(id) → bool
        t["valid_position"] = (Func<string, bool>)((id) =>
        {
            try
            {
                var pp = FindById(id);
                return pp != null && gregCore.Core.Networking.GregPatchPanels.ValidateRackPosition(pp);
            }
            catch { return false; }
        });

        // greg.patch.capture(id) → snapshot table or nil
        t["capture"] = (Func<string, DynValue>)((id) =>
        {
            try
            {
                var pp = FindById(id);
                if (pp == null) return DynValue.Nil;
                var dto = gregCore.Core.Networking.GregPatchPanels.Capture(pp);
                if (dto == null) return DynValue.Nil;
                var t = new Table(script);
                t["id"] = dto.PatchPanelID ?? "";
                t["rack_uid"] = dto.RackPositionUID;
                t["type"] = dto.PatchPanelType;
                return DynValue.FromObject(script, t);
            }
            catch { return DynValue.Nil; }
        });
    }

    private static void RegisterInsertIntoRack(Table t, string modId)
    {

        // greg.patch.insert_into_rack(id, spec) → bool (spec: rack_uid/type)
        t["insert_into_rack"] = (Func<string, DynValue, bool>)((id, spec) =>
        {
            try
            {
                var pp = FindById(id);
                if (pp == null || spec == null || spec.Type != DataType.Table) return false;
                var dto = new gregCore.Core.Networking.GregPatchPanels.PatchPanelSave();
                int? i = LuaServerModule.Int(spec.Table, "rack_uid");
                if (i.HasValue) dto.RackPositionUID = i.Value;
                i = LuaServerModule.Int(spec.Table, "type");
                if (i.HasValue) dto.PatchPanelType = i.Value;
                string s = LuaServerModule.Str(spec.Table, "id");
                if (s != null) dto.PatchPanelID = s;
                return gregCore.Core.Networking.GregPatchPanels.InsertIntoRack(pp, dto);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] patch.insert_into_rack() failed: {ex.Message}");
                return false;
            }
        });
    }

    internal static System.Collections.Generic.List<Il2Cpp.PatchPanel> FindAllPanels()
    {
        var list = new System.Collections.Generic.List<Il2Cpp.PatchPanel>();
        try
        {
            var found = UnityEngine.Object.FindObjectsOfType<Il2Cpp.PatchPanel>();
            if (found == null) return list;
            foreach (var pp in found)
            {
                try { if (pp != null) list.Add(pp); }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return list;
    }

    internal static Il2Cpp.PatchPanel FindById(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var pp in FindAllPanels())
            {
                string pid = null;
                try { pid = pp.patchPanelId; } catch { continue; }
                if (string.Equals(pid, id, StringComparison.OrdinalIgnoreCase)) return pp;
            }
            return null;
        }
        catch { return null; }
    }

    internal static Table PanelToTable(Script script, Il2Cpp.PatchPanel pp)
    {
        try
        {
            if (pp == null) return null;
            var info = new Table(script);
            string id = "";
            try { id = pp.patchPanelId ?? ""; } catch { }
            info["id"] = string.IsNullOrEmpty(id) ? pp.GetHashCode().ToString() : id;
            info["hash"] = pp.GetHashCode();
            try
            {
                var pos = pp.transform != null ? pp.transform.position : UnityEngine.Vector3.zero;
                info["x"] = (double)pos.x;
                info["y"] = (double)pos.y;
                info["z"] = (double)pos.z;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return info;
        }
        catch { return null; }
    }
}
