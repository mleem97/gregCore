/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Shop bridge: read ScriptableObjects (ShopItemSO), runtime
///               items (ShopItem: unlock/buy), cart rows (ShopCartItem:
///               read, +/-), mod items (ModShopItem) as well as the native
///               FlexibleColorPicker (open, read/set color, cancel).
///               All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using gregCore.Core.Mods;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregShop
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class ItemDefinition
    {
        public string ItemName { get; set; } = "";
        public int Price { get; set; }
        public int XpToUnlock { get; set; }
        public string ItemType { get; set; } = "";
        public int ItemID { get; set; }
        public float Eol { get; set; }
        public bool IsCustomColor { get; set; }
        public bool IsUnlocked { get; set; }
    }

    public sealed class CartLine
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = "";
        public int Price { get; set; }
        public string ItemType { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public int TotalPrice { get; set; }
        public Color ItemColor = Color.white;
        public bool HasCustomColor;
    }

    // ── Read ShopItemSO ──────────────────────────────────────────────────────

    public static ItemDefinition ReadDefinition(global::Il2Cpp.ShopItemSO so, bool isUnlocked)
    {
        var dto = new ItemDefinition { IsUnlocked = isUnlocked };
        if (so == null) return dto;
        Try(() => dto.ItemName = so.itemName ?? "");
        Try(() => dto.Price = so.price);
        Try(() => dto.XpToUnlock = so.xpToUnlock);
        Try(() => dto.ItemType = so.itemType.ToString());
        Try(() => dto.ItemID = so.itemID);
        Try(() => dto.Eol = so.eol);
        Try(() => dto.IsCustomColor = so.isCustomColor);
        return dto;
    }

    // ── ShopItem (Runtime) ───────────────────────────────────────────────────

    public static ItemDefinition ReadItem(global::Il2Cpp.ShopItem si)
    {
        var dto = new ItemDefinition();
        if (si == null) return dto;
        global::Il2Cpp.ShopItemSO so = null;
        Try(() => so = si.shopItemSO);
        if (so != null)
        {
            bool unlocked = false;
            Try(() => unlocked = si.isUnlocked);
            dto = ReadDefinition(so, unlocked);
        }
        Try(() => dto.IsUnlocked = si.isUnlocked);
        return dto;
    }

    public static List<global::Il2Cpp.ShopItem> FindItems(Transform parent, bool includeInactive)
    {
        var result = new List<global::Il2Cpp.ShopItem>();
        if (parent == null) return result;
        Try(() =>
        {
            var go = parent.gameObject;
            if (go == null) return;
            var items = go.GetComponentsInChildren<global::Il2Cpp.ShopItem>(includeInactive);
            if (items == null) return;
            foreach (var si in items)
            {
                if (si == null) continue;
                try { result.Add(si); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static bool UnlockItem(global::Il2Cpp.ShopItem si)
    {
        if (si == null) return false;
        try
        {
            _ = si.gameObject; // liveness
            si.UnlockButton();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UnlockButton failed: {Base(ex)}");
            return false;
        }
    }

    public static bool BuyItem(global::Il2Cpp.ShopItem si)
    {
        if (si == null) return false;
        try
        {
            _ = si.gameObject; // liveness
            si.ButtonBuyItem();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonBuyItem failed: {Base(ex)}");
            return false;
        }
    }

    // ── ShopCartItem (cart rows) ─────────────────────────────────────────────

    public static CartLine ReadCartLine(global::Il2Cpp.ShopCartItem ci)
    {
        var dto = new CartLine();
        if (ci == null) return dto;
        Try(() => dto.ItemID = ci.ItemID);
        Try(() => dto.ItemName = ci.itemName ?? "");
        Try(() => dto.Price = ci.Price);
        Try(() => dto.ItemType = ci.ItemType.ToString());
        Try(() => dto.Quantity = ci.Quantity);
        Try(() => dto.TotalPrice = ci.TotalPrice);
        Try(() => dto.ItemColor = ci.itemColor);
        Try(() => dto.HasCustomColor = ci.hasCustomColor);
        return dto;
    }

    public static bool CartAddOne(global::Il2Cpp.ShopCartItem ci)
    {
        if (ci == null) return false;
        try
        {
            _ = ci.gameObject; // liveness
            ci.OnAddClicked();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Cart+ failed: {Base(ex)}");
            return false;
        }
    }

    public static bool CartRemoveOne(global::Il2Cpp.ShopCartItem ci)
    {
        if (ci == null) return false;
        try
        {
            _ = ci.gameObject; // liveness
            ci.OnRemoveClicked();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Cart− failed: {Base(ex)}");
            return false;
        }
    }

    // ── ModShopItem ──────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.ModShopItem> FindModItems(Transform parent, bool includeInactive)
    {
        var result = new List<global::Il2Cpp.ModShopItem>();
        if (parent == null) return result;
        Try(() =>
        {
            var go = parent.gameObject;
            if (go == null) return;
            var items = go.GetComponentsInChildren<global::Il2Cpp.ModShopItem>(includeInactive);
            if (items == null) return;
            foreach (var mi in items)
            {
                if (mi == null) continue;
                try { result.Add(mi); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static int GetModId(global::Il2Cpp.ModShopItem mi)
    {
        if (mi == null) return -1;
        try { return mi.modID; } catch { return -1; }
    }

    public static GregModPack.ShopItem ReadModConfig(global::Il2Cpp.ModShopItem mi)
    {
        if (mi == null) return new GregModPack.ShopItem();
        global::Il2Cpp.ShopItemConfig config = null;
        try
        {
            _ = mi.gameObject; // liveness
            config = mi.config;
        }
        catch { config = null; }
        return GregModPack.ReadShopItem(config);
    }

    public static bool BuyModItem(global::Il2Cpp.ModShopItem mi)
    {
        if (mi == null) return false;
        try
        {
            _ = mi.gameObject; // liveness
            mi.ButtonBuyItem();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ModShopItem-Buy failed: {Base(ex)}");
            return false;
        }
    }

    // ── FlexibleColorPicker (nativ) ──────────────────────────────────────────
    // Lets mods use the vanilla color flow directly (instead of handoff): open,
    // set/read color, cancel. The picker GameObject belongs to the shop.

    public static global::Il2Cpp.FlexibleColorPicker FindPicker(global::Il2Cpp.ComputerShop shop)
    {
        if (shop != null)
        {
            try
            {
                _ = shop.gameObject; // liveness
                var picker = shop.flexibleColorPicker;
                if (picker != null) return picker;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        global::Il2Cpp.FlexibleColorPicker found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.FlexibleColorPicker>();
            if (all == null) return;
            foreach (var p in all)
            {
                if (p == null) continue;
                try
                {
                    var go = p.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = p;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool IsPickerOpen(global::Il2Cpp.ComputerShop shop)
    {
        var picker = FindPicker(shop);
        if (picker == null) return false;
        try { return picker.gameObject.activeInHierarchy; } catch { return false; }
    }

    public static bool OpenPicker(global::Il2Cpp.ComputerShop shop)
    {
        if (shop == null) return false;
        try
        {
            _ = shop.gameObject; // liveness
            shop.OpenColorPicker();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"OpenColorPicker failed: {Base(ex)}");
            return false;
        }
    }

    public static bool CancelPicker(global::Il2Cpp.ComputerShop shop)
    {
        if (shop == null) return false;
        try
        {
            _ = shop.gameObject; // liveness
            shop.ButtonCancelColorPicker();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonCancelColorPicker failed: {Base(ex)}");
            return false;
        }
    }

    public static Color GetPickerColor(global::Il2Cpp.ComputerShop shop)
    {
        var picker = FindPicker(shop);
        if (picker == null) return Color.white;
        try { return picker.GetColor(); } catch { return Color.white; }
    }

    public static bool SetPickerColor(global::Il2Cpp.ComputerShop shop, Color color)
    {
        var picker = FindPicker(shop);
        if (picker == null) return false;
        try
        {
            _ = picker.gameObject; // liveness
            picker.SetColor(color);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetColor failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Shop: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Shop field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
