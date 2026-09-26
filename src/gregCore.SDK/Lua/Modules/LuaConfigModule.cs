/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for persistent mod settings (JSON file per mod).
/// Maintainer:   greg.config.get(), set(), delete(), has(), keys()
/// </file-summary>

using System;
using System.IO;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaConfigModule
{
    public static void Register(Table greg, Script script, string modId, string modDir)
    {
        var store = new LuaKvStore(modId, "config", Path.Combine(modDir ?? ".", "data"), "config.json");
        var configTable = new Table(script);
        RegisterGet(configTable, store, script, modId);
        RegisterSet(configTable, store, modId);
        RegisterKeys(configTable, store, script);

        greg["config"] = configTable;
    }

    private static void RegisterGet(Table t, LuaKvStore store, Script script, string modId)
    {

        // greg.config.get(key) → string or nil
        t["get"] = (Func<string, DynValue>)((key) =>
        {
            try
            {
                string value = store.Get(key);
                return value != null ? DynValue.FromObject(script, value) : DynValue.Nil;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] config.get('{key}') failed: {ex.Message}");
                return DynValue.Nil;
            }
        });

        // greg.config.get_or(key, default) → string
        t["get_or"] = (Func<string, string, string>)((key, fallback) =>
        {
            try
            {
                string value = store.Get(key);
                return value ?? fallback ?? "";
            }
            catch { return fallback ?? ""; }
        });
    }

    private static void RegisterSet(Table t, LuaKvStore store, string modId)
    {

        // greg.config.set(key, value)
        t["set"] = (Action<string, string>)((key, value) =>
        {
            try { store.Set(key, value); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] config.set('{key}') failed: {ex.Message}");
            }
        });

        // greg.config.delete(key) → bool
        t["delete"] = (Func<string, bool>)((key) =>
        {
            try { return store.Delete(key); }
            catch { return false; }
        });

        // greg.config.has(key) → bool
        t["has"] = (Func<string, bool>)((key) =>
        {
            try { return store.Has(key); }
            catch { return false; }
        });
    }

    private static void RegisterKeys(Table t, LuaKvStore store, Script script)
    {

        // greg.config.keys() → array of keys
        t["keys"] = (Func<Table>)(() =>
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
    }
}
