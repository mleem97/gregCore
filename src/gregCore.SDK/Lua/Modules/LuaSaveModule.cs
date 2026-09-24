/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für persistenten Mod-State (JSON-Datei pro Mod,
///               unabhaengig vom Spiel-Save; wird beim Start geladen).
/// Maintainer:   greg.save.get(), set(), delete(), has(), keys(), save_now()
/// </file-summary>

using System;
using System.IO;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaSaveModule
{
    public static void Register(Table greg, Script script, string modId, string modDir)
    {
        var store = new LuaKvStore(modId, "save", Path.Combine(modDir ?? ".", "data"), "save.json");
        var saveTable = new Table(script);

        // greg.save.get(key) → string or nil
        saveTable["get"] = (Func<string, DynValue>)((key) =>
        {
            try
            {
                string value = store.Get(key);
                return value != null ? DynValue.FromObject(script, value) : DynValue.Nil;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] save.get('{key}') failed: {ex.Message}");
                return DynValue.Nil;
            }
        });

        // greg.save.get_or(key, default) → string
        saveTable["get_or"] = (Func<string, string, string>)((key, fallback) =>
        {
            try
            {
                string value = store.Get(key);
                return value ?? fallback ?? "";
            }
            catch { return fallback ?? ""; }
        });

        // greg.save.set(key, value) — writes through immediately
        saveTable["set"] = (Action<string, string>)((key, value) =>
        {
            try { store.Set(key, value); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] save.set('{key}') failed: {ex.Message}");
            }
        });

        // greg.save.delete(key) → bool
        saveTable["delete"] = (Func<string, bool>)((key) =>
        {
            try { return store.Delete(key); }
            catch { return false; }
        });

        // greg.save.has(key) → bool
        saveTable["has"] = (Func<string, bool>)((key) =>
        {
            try { return store.Has(key); }
            catch { return false; }
        });

        // greg.save.keys() → array of keys
        saveTable["keys"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var k in store.Keys()) result[i++] = k;
                return result;
            }
            catch { return new Table(script); }
        });

        // greg.save.save_now() → bool (force flush to disk)
        saveTable["save_now"] = (Func<bool>)(() =>
        {
            try { return store.SaveNow(); }
            catch { return false; }
        });

        greg["save"] = saveTable;
    }
}
