/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für JSON (parse/stringify ohne Spiel-Abhaengigkeit).
/// Maintainer:   greg.json.parse(), stringify(); reine Konvertierung,
///               unit-testbar ohne Spiel.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Text.Json;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaJsonModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var jsonTable = new Table(script);

        // greg.json.parse(text) → table/string/number/boolean or nil
        jsonTable["parse"] = (Func<string, DynValue>)((text) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return DynValue.Nil;
                using (var doc = JsonDocument.Parse(text))
                {
                    return FromJsonElement(script, doc.RootElement);
                }
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] json.parse() failed: {ex.Message}");
                return DynValue.Nil;
            }
        });

        // greg.json.stringify(value) → string ("" on failure)
        jsonTable["stringify"] = (Func<DynValue, string>)((value) =>
        {
            try
            {
                object plain = ToPlainObject(value);
                return JsonSerializer.Serialize(plain);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] json.stringify() failed: {ex.Message}");
                return "";
            }
        });

        greg["json"] = jsonTable;
    }

    internal static DynValue FromJsonElement(Script script, JsonElement el)
    {
        try
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var t = new Table(script);
                        foreach (var prop in el.EnumerateObject())
                            t[prop.Name] = FromJsonElement(script, prop.Value);
                        return DynValue.FromObject(script, t);
                    }
                case JsonValueKind.Array:
                    {
                        var t = new Table(script);
                        int i = 1;
                        foreach (var item in el.EnumerateArray())
                            t[i++] = FromJsonElement(script, item);
                        return DynValue.FromObject(script, t);
                    }
                case JsonValueKind.String:
                    return DynValue.FromObject(script, el.GetString() ?? "");
                case JsonValueKind.Number:
                    {
                        double d;
                        if (el.TryGetDouble(out d)) return DynValue.FromObject(script, d);
                        return DynValue.FromObject(script, el.GetRawText());
                    }
                case JsonValueKind.True:
                    return DynValue.FromObject(script, true);
                case JsonValueKind.False:
                    return DynValue.FromObject(script, false);
                default:
                    return DynValue.Nil;
            }
        }
        catch { return DynValue.Nil; }
    }

    internal static object ToPlainObject(DynValue value)
    {
        try
        {
            if (value == null || value.IsNil()) return null;
            switch (value.Type)
            {
                case DataType.Boolean:
                    return value.Boolean;
                case DataType.Number:
                    return value.Number;
                case DataType.String:
                    return value.String;
                case DataType.Table:
                    {
                        var table = value.Table;
                        bool isArray = IsLuaArray(table);
                        if (isArray)
                        {
                            var list = new List<object>();
                            int i = 1;
                            while (true)
                            {
                                var item = table.Get(i);
                                if (item == null || item.IsNil()) break;
                                list.Add(ToPlainObject(item));
                                i++;
                            }
                            return list;
                        }
                        var dict = new Dictionary<string, object>();
                        foreach (var pair in table.Pairs)
                        {
                            string key = pair.Key.Type == DataType.String
                                ? pair.Key.String
                                : pair.Key.ToString();
                            dict[key] = ToPlainObject(pair.Value);
                        }
                        return dict;
                    }
                default:
                    return value.ToString();
            }
        }
        catch { return null; }
    }

    private static bool IsLuaArray(Table table)
    {
        try
        {
            if (table == null) return true;
            bool any = false;
            foreach (var pair in table.Pairs)
            {
                any = true;
                if (pair.Key.Type != DataType.Number) return false;
            }
            return true;
        }
        catch { return false; }
    }
}
