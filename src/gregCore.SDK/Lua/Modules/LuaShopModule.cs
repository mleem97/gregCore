/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für den Shop (Katalog lesen, freischalten, kaufen,
///               Warenkorb). Index-basiert (1-based), Reihenfolge ist
///               best-effort (Shop baut Karten neu auf).
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

        // greg.shop.items() → array of {idx, name, price, xp, type, id, unlocked}
        shopTable["items"] = (Func<Table>)(() =>
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

        // greg.shop.unlock(idx) → bool
        shopTable["unlock"] = (Func<int, bool>)((idx) =>
        {
            try
            {
                var si = ItemAt(idx);
                return si != null && gregCore.Core.Networking.GregShop.UnlockItem(si);
            }
            catch { return false; }
        });

        // greg.shop.buy(idx) → bool (legt in den Warenkorb / kauft)
        shopTable["buy"] = (Func<int, bool>)((idx) =>
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

        // greg.shop.cart() → array of {ref, name, price, qty, total}
        shopTable["cart"] = (Func<Table>)(() =>
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
            catch { return new Table(script); }
        });

        // greg.shop.cart_add(ref) / cart_remove(ref) → bool
        shopTable["cart_add"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var rows = CartRows();
                if (r < 1 || r > rows.Count) return false;
                return gregCore.Core.Networking.GregShop.CartAddOne(rows[r - 1]);
            }
            catch { return false; }
        });
        shopTable["cart_remove"] = (Func<int, bool>)((r) =>
        {
            try
            {
                var rows = CartRows();
                if (r < 1 || r > rows.Count) return false;
                return gregCore.Core.Networking.GregShop.CartRemoveOne(rows[r - 1]);
            }
            catch { return false; }
        });

        greg["shop"] = shopTable;
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
        catch { return null; }
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
        catch { return null; }
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
