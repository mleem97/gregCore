/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for mod-save entries (SaveData.modItemData).
///               Unlike greg.save (file, global), this travels WITH the
///               save game (per save). Vectors as {x,y,z} tables.
/// Maintainer:   greg.modsave.list(), upsert(), remove()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaModSaveModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var saveTable = new Table(script);
        RegisterList(saveTable, script, modId);
        RegisterUpsert(saveTable, modId);
        RegisterRemove(saveTable);

        greg["modsave"] = saveTable;
    }

    private static void RegisterList(Table t, Script script, string modId)
    {

        // greg.modsave.list(folder) → array of entries
        t["list"] = (Func<string, Table>)((folder) =>
        {
            try
            {
                var result = new Table(script);
                var list = ModItemList();
                if (list == null) return result;
                int i = 1;
                foreach (var dto in gregCore.Core.Mods.GregModSave.ReadAll(list))
                {
                    try
                    {
                        if (dto == null) continue;
                        if (!string.IsNullOrEmpty(folder) &&
                            !string.Equals(dto.ModFolderName ?? "", folder, StringComparison.OrdinalIgnoreCase))
                            continue;
                        result[i++] = ItemToTable(script, dto);
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] modsave.list() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterUpsert(Table t, string modId)
    {

        // greg.modsave.upsert(folder, spec) → bool (spec: position{x,y,z},
        // rotation{x,y,z}, values[], ints[], ints2[])
        t["upsert"] = (Func<string, DynValue, bool>)((folder, spec) =>
        {
            try
            {
                var list = ModItemList();
                if (list == null || spec == null || spec.Type != DataType.Table) return false;
                var dto = new gregCore.Core.Mods.GregModSave.ItemSave();
                dto.ModFolderName = folder ?? "";
                var t = spec.Table;
                var pos = LuaServerModule.Vec(t, "position");
                if (pos.HasValue) dto.Position = pos.Value;
                var rot = LuaServerModule.Vec(t, "rotation");
                if (rot.HasValue)
                    dto.Rotation = new UnityEngine.Quaternion(rot.Value.x, rot.Value.y, rot.Value.z, 1f);
                dto.SaveValue = FloatArray(t.Get("values"));
                dto.SaveIntArray = IntArray(t.Get("ints"));
                dto.SaveIntArray2 = IntArray(t.Get("ints2"));
                return gregCore.Core.Mods.GregModSave.Upsert(list, dto) != null;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] modsave.upsert() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterRemove(Table t)
    {

        // greg.modsave.remove(folder) → bool (all entries of the folder)
        t["remove"] = (Func<string, bool>)((folder) =>
        {
            try
            {
                var list = ModItemList();
                if (list == null || string.IsNullOrEmpty(folder)) return false;
                return gregCore.Core.Mods.GregModSave.Remove(list, folder);
            }
            catch { return false; }
        });
    }

    internal static Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData> ModItemList()
    {
        try
        {
            var save = greg.Sdk.GregPublicAPI.GetSaveDataSafe();
            if (save == null) return null;
            var list = save.modItemData;
            if (list == null) return null;
            return list;
        }
        catch { return null; }
    }

    internal static Table ItemToTable(Script script, gregCore.Core.Mods.GregModSave.ItemSave dto)
    {
        try
        {
            var t = new Table(script);
            t["folder"] = dto.ModFolderName ?? "";
            var pos = new Table(script);
            pos["x"] = (double)dto.Position.x;
            pos["y"] = (double)dto.Position.y;
            pos["z"] = (double)dto.Position.z;
            t["position"] = pos;
            return t;
        }
        catch { return null; }
    }

    internal static float[] FloatArray(DynValue v)
    {
        try
        {
            if (v == null || v.Type != DataType.Table) return Array.Empty<float>();
            var list = new System.Collections.Generic.List<float>();
            foreach (var pair in v.Table.Pairs)
            {
                try
                {
                    if (pair.Value.Type == DataType.Number)
                        list.Add((float)pair.Value.Number);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            return list.ToArray();
        }
        catch { return Array.Empty<float>(); }
    }

    internal static int[] IntArray(DynValue v)
    {
        try
        {
            if (v == null || v.Type != DataType.Table) return Array.Empty<int>();
            var list = new System.Collections.Generic.List<int>();
            foreach (var pair in v.Table.Pairs)
            {
                try
                {
                    if (pair.Value.Type == DataType.Number)
                        list.Add((int)pair.Value.Number);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            return list.ToArray();
        }
        catch { return Array.Empty<int>(); }
    }
}
