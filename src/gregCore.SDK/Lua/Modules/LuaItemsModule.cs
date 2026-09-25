/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for registering custom shop/static items.
///               Specs arrive as Lua tables (fields as in the docs), meshes as
///               files in the mod folder. Errors -> false + log, no crash.
/// Maintainer:   greg.items.register_shop_item(), register_static_item()
/// </file-summary>

using System;
using System.IO;
using System.Text.Json;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaItemsModule
{
    public static void Register(Table greg, Script script, string modId, string modDir)
    {
        var itemsTable = new Table(script);
        RegisterRegisterShopItem(itemsTable, script, modId, modDir);
        RegisterRegisterStaticItem(itemsTable, script, modId, modDir);

        greg["items"] = itemsTable;
    }

    private static void RegisterRegisterShopItem(Table t, Script script, string modId, string modDir)
    {

        // greg.items.register_shop_item(subfolder, spec) → bool
        // spec: {name, price, xp, size_u, mass, scale, model, texture, icon, type, ...}
        t["register_shop_item"] = (Func<string, DynValue, bool>)((subfolder, spec) =>
        {
            try
            {
                string folderPath = ResolveFolder(modDir, subfolder);
                if (folderPath == null) return false;
                var dto = SpecToJson<gregCore.Core.Mods.GregModPack.ShopItem>(script, spec);
                if (dto == null) return false;
                return gregCore.Core.Mods.GregCustomItems.RegisterShopItem(folderPath, modId, dto);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] items.register_shop_item() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterRegisterStaticItem(Table t, Script script, string modId, string modDir)
    {

        // greg.items.register_static_item(subfolder, spec) → bool
        t["register_static_item"] = (Func<string, DynValue, bool>)((subfolder, spec) =>
        {
            try
            {
                string folderPath = ResolveFolder(modDir, subfolder);
                if (folderPath == null) return false;
                var dto = SpecToJson<gregCore.Core.Mods.GregModPack.StaticItem>(script, spec);
                if (dto == null) return false;
                return gregCore.Core.Mods.GregCustomItems.RegisterStaticItem(folderPath, modId, dto);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] items.register_static_item() failed: {ex.Message}");
                return false;
            }
        });
    }

    internal static string ResolveFolder(string modDir, string subfolder)
    {
        try
        {
            string root = string.IsNullOrEmpty(modDir) ? "." : modDir;
            string combined = string.IsNullOrEmpty(subfolder)
                ? root
                : Path.Combine(root, subfolder.Replace('/', Path.DirectorySeparatorChar));
            string full = Path.GetFullPath(combined);
            string rootFull = Path.GetFullPath(root);
            string sep = rootFull.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? rootFull : rootFull + Path.DirectorySeparatorChar;
            if (!full.Equals(rootFull, StringComparison.OrdinalIgnoreCase) &&
                !full.StartsWith(sep, StringComparison.OrdinalIgnoreCase))
                return null;
            return full;
        }
        catch { return null; }
    }

    private static readonly System.Collections.Generic.Dictionary<string, string> KeyAliases =
        new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "name", "ItemName" },
            { "price", "Price" },
            { "xp", "XpToUnlock" },
            { "size_u", "SizeInU" },
            { "mass", "Mass" },
            { "scale", "ModelScale" },
            { "model", "ModelFile" },
            { "texture", "TextureFile" },
            { "icon", "IconFile" },
            { "type", "ObjectType" },
        };

    internal static T SpecToJson<T>(Script script, DynValue spec) where T : class, new()
    {
        try
        {
            if (spec == null || spec.Type != DataType.Table) return null;
            object plain = LuaJsonModule.ToPlainObject(spec);
            if (plain is System.Collections.Generic.Dictionary<string, object> dict)
            {
                var mapped = new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in dict)
                {
                    string key = kv.Key;
                    string mapped2;
                    if (KeyAliases.TryGetValue(key, out mapped2)) key = mapped2;
                    mapped[key] = kv.Value;
                }
                plain = mapped;
            }
            string json = JsonSerializer.Serialize(plain);
            // Case-insensitive: Lua uses snake_case/lowercase, DTOs use PascalCase.
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch { return null; }
    }
}
