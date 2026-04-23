using System;
using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;

namespace greg.CommonShop
{
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
