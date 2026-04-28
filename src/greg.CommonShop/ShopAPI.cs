using System;
using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using UnityEngine;
using Object = UnityEngine.Object;
using greg.Logging;

namespace greg.CommonShop
{
    public static class ShopAPI
    {
        private static List<CustomShopItem> _registeredItems = new();
        internal static List<CustomShopItem> RegisteredItems => _registeredItems;
        private static GregModLogger _log = new GregModLogger("CommonShop");

        public static void RegisterItem(CustomShopItem item)
        {
            if (_registeredItems.Any(i => i.Name == item.Name))
            {
                _log.Warn($"Conflict: An item named '{item.Name}' is already registered! Skipping.");
                return;
            }

            if (item.ResultItemID.HasValue)
            {
                if (_registeredItems.Any((i => i.TemplateType == item.TemplateType && i.ResultItemID == item.ResultItemID)))
                {
                    _log.Warn($"Conflict: ResultItemID {item.ResultItemID} for {item.TemplateType} is already claimed! Skipping.");
                    return;
                }
            }
            
            _registeredItems.Add(item);
            _log.Msg($"Registered: {item.Name} in {item.Category}");
        }
        
        internal static void InjectAll(ComputerShop shop)
        {
            if (_registeredItems.Count == 0) return;

            var injectedGrids = new List<Transform>();
            var categoryGroups = _registeredItems.GroupBy(item => item.Category);

            foreach (var catGroup in categoryGroups)
            {
                string mainCategory = catGroup.Key;
                var subGroups = catGroup.GroupBy(item => item.SubCategory ?? "");

                foreach (var subGroup in subGroups)
                {
                    string subCategory = subGroup.Key;

                    Transform container = EnsureCategoryContainer(shop, mainCategory, subCategory);
                    if (container == null) continue;

                    for (int i = container.childCount - 1; i >= 0; i--)
                    {
                        if (container.GetChild(i).name.StartsWith("ModCard_"))
                            Object.DestroyImmediate(container.GetChild(i).gameObject);
                    }

                    foreach (var data in subGroup)
                    {
                        if (HasExternalModConflict(shop, data))
                        {
                            _log.Error($"External Conflict: Another mod is using '{data.Name}'. Skipping injection.");
                            continue;     
                        }
                        
                        ShopItem? template = CustomShopItem.FindTemplate(shop, data.TemplateType, data.TemplateID);
                        if (template != null)
                        {
                            CreateShopCard(shop, container, template, data);
                        }
                    }

                    injectedGrids.Add(container);
                }
            }

            var sr = shop.shopItemParent.GetComponentInParent<ScrollRect>();
            if (sr?.content != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(sr.content);
        }
        
        private static bool HasExternalModConflict(ComputerShop shop, CustomShopItem data)
        {
            int targetID = data.ResultItemID ?? data.TemplateID;
            var allUIItems = shop.shopItemParent.GetComponentsInChildren<ShopItem>(true);
            
            foreach (var uiItem in allUIItems)
            {
                if (uiItem?.shopItemSO != null)
                {
                    if (uiItem.shopItemSO.itemName == data.Name) return true;

                    if (data.ResultItemID.HasValue && 
                        uiItem.shopItemSO.itemType == data.TemplateType && 
                        uiItem.shopItemSO.itemID == targetID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Transform EnsureCategoryContainer(ComputerShop shop, string category, string subCategory)
        {
            // Try to find the vanilla parent
            var parent = shop.shopItemParent.transform;
            if (parent == null) return null!;
            return parent; // Simplification: append directly to main shop parent for now
        }

        private static void CreateShopCard(ComputerShop shop, Transform container, ShopItem template, CustomShopItem data)
        {
            var clone = Object.Instantiate(template.gameObject, container);
            clone.name = "ModCard_" + data.Name;
            clone.SetActive(true);

            var si = clone.GetComponent<ShopItem>();
            if (si != null)
            {
                var newSo = ScriptableObject.CreateInstance<ShopItemSO>();
                newSo.itemName = data.Name;
                newSo.price = data.Price;
                newSo.itemType = data.TemplateType;
                newSo.itemID = data.ResultItemID ?? data.TemplateID;
                if (data.Icon != null) newSo.sprite = data.Icon;
                
                si.shopItemSO = newSo;
                si.Start(); // Force update UI texts based on new SO
                
                // Invoke callback
                data.OnUIReady?.Invoke(clone);
            }
        }
    }
}
