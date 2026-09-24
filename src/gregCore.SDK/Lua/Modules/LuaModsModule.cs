/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Mod-Registry und Abhängigkeiten.
/// Maintainer:   greg.mods.list(), is_loaded(), version(), declare(),
///               ensure(), check()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaModsModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var modsTable = new Table(script);

        // greg.mods.list() → array of {id, name, version}
        modsTable["list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var e in gregCore.Core.Mods.GregModRegistry.All())
                {
                    try
                    {
                        if (e == null) continue;
                        var t = new Table(script);
                        t["id"] = e.Id ?? "";
                        t["name"] = e.Name ?? "";
                        t["version"] = e.Version ?? "";
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] mods.list() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.mods.is_loaded(modIdOrName) → bool
        modsTable["is_loaded"] = (Func<string, bool>)((name) =>
        {
            try { return gregCore.Core.Mods.GregModDeps.IsMelonLoaded(name); }
            catch { return false; }
        });

        // greg.mods.version(modIdOrName) → string ("" when unknown)
        modsTable["version"] = (Func<string, string>)((name) =>
        {
            try { return gregCore.Core.Mods.GregModDeps.GetMelonVersion(name) ?? ""; }
            catch { return ""; }
        });

        // greg.mods.declare({{mod=, min_version=, required=}, ...}) → bool
        modsTable["declare"] = (Func<Table, bool>)((deps) =>
        {
            try
            {
                var list = new System.Collections.Generic.List<gregCore.Core.Mods.GregModDeps.Dependency>();
                if (deps != null)
                {
                    foreach (var pair in deps.Pairs)
                    {
                        try
                        {
                            if (pair.Value == null || pair.Value.Type != DataType.Table) continue;
                            var t = pair.Value.Table;
                            string depId = t.Get("mod").Type == DataType.String ? t.Get("mod").String : "";
                            if (string.IsNullOrWhiteSpace(depId)) continue;
                            string minVer = t.Get("min_version").Type == DataType.String
                                ? t.Get("min_version").String : "";
                            bool required = true;
                            if (t.Get("required").Type == DataType.Boolean)
                                required = t.Get("required").Boolean;
                            list.Add(new gregCore.Core.Mods.GregModDeps.Dependency
                            {
                                ModId = depId,
                                MinVersion = minVer ?? "",
                                Required = required,
                            });
                        }
                        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    }
                }
                gregCore.Core.Mods.GregModDeps.Declare(modId, list.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] mods.declare() failed: {ex.Message}");
                return false;
            }
        });

        // greg.mods.ensure({mod=, min_version=?, required=?}) → ok, detail
        modsTable["ensure"] = (Func<DynValue, DynValue>)((spec) =>
        {
            try
            {
                string name = "";
                string minVer = "";
                bool required = true;
                if (spec != null && spec.Type == DataType.Table)
                {
                    if (spec.Table.Get("mod").Type == DataType.String)
                        name = spec.Table.Get("mod").String;
                    if (spec.Table.Get("min_version").Type == DataType.String)
                        minVer = spec.Table.Get("min_version").String;
                    if (spec.Table.Get("required").Type == DataType.Boolean)
                        required = spec.Table.Get("required").Boolean;
                }
                var dep = new gregCore.Core.Mods.GregModDeps.Dependency
                {
                    ModId = name ?? "",
                    MinVersion = minVer ?? "",
                    Required = required,
                };
                string detail;
                bool ok = gregCore.Core.Mods.GregModDeps.EnsureLoaded(dep, out detail);
                return DynValue.NewTuple(
                    DynValue.NewBoolean(ok),
                    DynValue.NewString(detail ?? ""));
            }
            catch (Exception ex)
            {
                return DynValue.NewTuple(
                    DynValue.NewBoolean(false),
                    DynValue.NewString(ex.Message));
            }
        });

        // greg.mods.check() → array of {owner, mod, detail} problems (empty = ok)
        modsTable["check"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var p in gregCore.Core.Mods.GregModDeps.CheckOwner(modId))
                {
                    try
                    {
                        if (p == null) continue;
                        var t = new Table(script);
                        t["owner"] = p.OwnerModId ?? "";
                        t["mod"] = p.ModId ?? "";
                        t["detail"] = p.Detail ?? "";
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch { return new Table(script); }
        });

        greg["mods"] = modsTable;
    }
}
