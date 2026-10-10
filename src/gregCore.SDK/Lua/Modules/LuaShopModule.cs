/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for the shop (reading catalog, unlocking, buying,
///               cart). Index-based (1-based), order is
///               best-effort (shop rebuilds cards from scratch).
/// Maintainer:   greg.shop.items(), unlock(), buy(), cart(), cart_add(),
///               cart_remove()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaShopModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var shopTable = new Table(script);
        RegisterItems(shopTable, script, modId);
        RegisterUnlock(shopTable, modId);
        RegisterCart(shopTable, script);
        RegisterCartAdd(shopTable);
        RegisterModItems(shopTable, script, modId);
        RegisterBuyModItem(shopTable);
        RegisterPickerOpen(shopTable);
        RegisterPickerColor(shopTable, script);

        greg["shop"] = shopTable;
    }

    private static void RegisterItems(Table t, Script script, string modId)
    {

        // greg.shop.items() → array of {idx, name, price, xp, type, id, unlocked}
        t["items"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var shop = GetShop();
                if (shop?.shopItems == null) return result;
                int i = 1;
                foreach (var si in shop.shopItems)
                {
                    try
                    {
                        if (si == null) continue;
                        var dto = gregCore.Core.Networking.GregShop.ReadItem(si);
                        if (dto == null) continue;
                        var t = new Table(script);
                        t["idx"] = i;
                        t["name"] = dto.ItemName ?? "";
                        t["price"] = dto.Price;
                        t["xp"] = dto.XpToUnlock;
                        t["type"] = dto.ItemType ?? "";
                        t["id"] = dto.ItemID;
                        t["unlocked"] = dto.IsUnlocked;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] shop.items() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterUnlock(Table t, string modId)
    {

        // greg.shop.unlock(idx) → bool
        t["unlock"] = (Func<int, bool>)((idx) =>
        {
            try
            {
                var si = ItemAt(idx);
                return si != null && gregCore.Core.Networking.GregShop.UnlockItem(si);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });

        // greg.shop.buy(idx) → bool (adds to cart / buys)
        t["buy"] = (Func<int, bool>)((idx) =>
        {
            try
            {
                var si = ItemAt(idx);
                return si != null && gregCore.Core.Networking.GregShop.BuyItem(si);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] shop.buy() failed: {ex.Message}");
                return false;
            }
        });
    }

    private static void RegisterCart(Table t, Script script)
    {

        // greg.shop.cart() → array of {ref, name, price, qty, total}
        t["cart"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var rows = CartRows();
                for (int i = 0; i < rows.Count; i++)
                {
                    try
                    {
                        var dto = gregCore.Core.Networking.GregShop.ReadCartLine(rows[i]);
                        if (dto == null) continue;
                        var t = new Table(script);
                        t["ref"] = i + 1;
                        t["name"] = dto.ItemName ?? "";
                        t["price"] = dto.Price;
                        t["qty"] = dto.Quantity;
                        t["total"] = dto.TotalPrice;
                        result[i + 1] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return new Table(script); }
        });
    }

    private static void RegisterCartAdd(Table t)
    {

        // greg.shop.cart_add(ref) / cart_remove(ref) → bool
        t["cart_add"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var rows = CartRows();
                if (r < 1 || r > rows.Count) return false;
                return gregCore.Core.Networking.GregShop.CartAddOne(rows[r - 1]);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
        t["cart_remove"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var rows = CartRows();
                if (r < 1 || r > rows.Count) return false;
                return gregCore.Core.Networking.GregShop.CartRemoveOne(rows[r - 1]);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterModItems(Table t, Script script, string modId)
    {

        // greg.shop.mod_items() → array of {ref, name, price, mod_id}
        t["mod_items"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                var found = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.ModShopItem>();
                if (found == null) return result;
                int i = 1;
                foreach (var mi in found)
                {
                    try
                    {
                        if (mi == null) continue;
                        var dto = gregCore.Core.Networking.GregShop.ReadModConfig(mi);
                        var t = new Table(script);
                        t["ref"] = i;
                        t["name"] = dto != null ? dto.ItemName ?? "" : "";
                        t["price"] = dto != null ? dto.Price : 0;
                        int modId = 0;
                        try { modId = gregCore.Core.Networking.GregShop.GetModId(mi); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                        t["mod_id"] = modId;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] shop.mod_items() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterBuyModItem(Table t)
    {

        // greg.shop.buy_mod_item(ref) → bool
        t["buy_mod_item"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var found = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.ModShopItem>();
                if (found == null || r < 1 || r > found.Count) return false;
                var mi = found[r - 1];
                return mi != null && gregCore.Core.Networking.GregShop.BuyModItem(mi);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });

        // greg.shop.picker_is_open() / picker_open() / picker_cancel() → bool
        t["picker_is_open"] = (Func<bool>)(() =>
        {
            try
            {
                var shop = GetShop();
                return shop != null && gregCore.Core.Networking.GregShop.IsPickerOpen(shop);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPickerOpen(Table t)
    {
        t["picker_open"] = (Func<bool>)(() =>
        {
            try
            {
                var shop = GetShop();
                return shop != null && gregCore.Core.Networking.GregShop.OpenPicker(shop);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
        t["picker_cancel"] = (Func<bool>)(() =>
        {
            try
            {
                var shop = GetShop();
                return shop != null && gregCore.Core.Networking.GregShop.CancelPicker(shop);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    private static void RegisterPickerColor(Table t, Script script)
    {

        // greg.shop.picker_color() → {r,g,b,a} or nil; picker_set_color(r,g,b,a) → bool
        t["picker_color"] = (Func<DynValue>)(() =>
        {
            try
            {
                var shop = GetShop();
                if (shop == null) return DynValue.Nil;
                var c = gregCore.Core.Networking.GregShop.GetPickerColor(shop);
                var t = new Table(script);
                t["r"] = (double)c.r;
                t["g"] = (double)c.g;
                t["b"] = (double)c.b;
                t["a"] = (double)c.a;
                return DynValue.FromObject(script, t);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return DynValue.Nil; }
        });
        t["picker_set_color"] = (Func<double, double, double, double, bool>)((r, g, b, a) =>
        {
            try
            {
                var shop = GetShop();
                if (shop == null) return false;
                return gregCore.Core.Networking.GregShop.SetPickerColor(shop,
                    new UnityEngine.Color((float)r, (float)g, (float)b, (float)a));
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        });
    }

    internal static global::Il2Cpp.ComputerShop GetShop()
    {
        try
        {
            var mgm = global::Il2Cpp.MainGameManager.instance;
            if (mgm == null || mgm.Pointer == IntPtr.Zero) return null;
            var shop = mgm.computerShop;
            if (shop == null || shop.Pointer == IntPtr.Zero) return null;
            return shop;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
    }

    internal static global::Il2Cpp.ShopItem ItemAt(int idx)
    {
        try
        {
            var shop = GetShop();
            if (shop?.shopItems == null || idx < 1) return null;
            int i = 1;
            foreach (var si in shop.shopItems)
            {
                if (i == idx) return si;
                i++;
            }
            return null;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
    }

    internal static System.Collections.Generic.List<global::Il2Cpp.ShopCartItem> CartRows()
    {
        var rows = new System.Collections.Generic.List<global::Il2Cpp.ShopCartItem>();
        try
        {
            var found = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.ShopCartItem>();
            if (found == null) return rows;
            foreach (var ci in found)
            {
                try { if (ci != null) rows.Add(ci); }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return rows;
    }
}
