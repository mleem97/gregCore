using System;
using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;

namespace greg.CommonShop
{
    /// <summary>
    /// Built-in shop categories of the base game. Use <see cref="VanillaCategoryExtensions.ToShopString"/>
    /// or pass directly to <see cref="CustomShopItem.SetCategory"/>.
    /// </summary>
    public enum VanillaCategory
    {
        SystemXServers,
        RISCServers,
        MainframeServers,
        GPUServers,
        Switches,
        PassiveComponents,
        Cables,
        SFPs,
        HLMods
    }

    public static class VanillaCategoryExtensions
    {
        /// <summary>Returns the exact category string the base game uses.</summary>
        public static string ToShopString(this VanillaCategory category) => category switch
        {
            VanillaCategory.SystemXServers => "System X Servers",
            VanillaCategory.RISCServers => "RISC Servers",
            VanillaCategory.MainframeServers => "Mainframe Servers",
            VanillaCategory.GPUServers => "GPU Servers",
            VanillaCategory.Switches => "Switches",
            VanillaCategory.PassiveComponents => "Passive components",
            VanillaCategory.Cables => "Cables",
            VanillaCategory.SFPs => "SFPs",
            VanillaCategory.HLMods => "HL Mods",
            _ => category.ToString()
        };
    }

    public class CustomShopItem
    {
        public string Name = string.Empty;
        public int Price;
        public Sprite? Icon;
        public Action? OnBuy;
        public PlayerManager.ObjectInHand TemplateType;
        public int TemplateID;
        public Color? BackgroundColor;
        public GameObject? CustomPrefab;
        public Action<GameObject>? OnUIReady;
        public int? ResultItemID;
        public Action<int>? OnCheckout;
        public string Category = "Mods";
        public string SubCategory = "";
        /// <summary>
        /// Optional purchase color passed to the game cart (custom-color
        /// presets). Null = default/plain item color.
        /// </summary>
        public Color? PurchaseColor;

        /// <summary>Assigns a vanilla base-game category to this item.</summary>
        public void SetCategory(VanillaCategory category) => Category = category.ToShopString();

        internal static ShopItem? FindTemplate(ComputerShop shop, PlayerManager.ObjectInHand type, int id)
        {
            if (shop.shopItems == null) return null;
            
            foreach (var si in shop.shopItems)
            {
                if (si?.shopItemSO != null && si.shopItemSO.itemType == type && si.shopItemSO.itemID == id)
                    return si;
            }
            
            return shop.shopItems.Length > 0 ? shop.shopItems[0] : null; 
        }
    }
}
