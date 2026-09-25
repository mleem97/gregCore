using System;
using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace greg.CommonShop
{
    public static partial class ShopAPI
    {
        internal static void InjectAll(ComputerShop shop)
        {
            try
            {
                EnsureInitialized();
                var items = MergePresetItems(shop);
                if (items.Count == 0) return;
                var ctx = new InjectionContext();
                InjectCategoryGroups(shop, items, ctx);
                FinalizeInjection(shop, ctx);
            }
            catch (Exception ex)
            {
                _log.Warn($"InjectAll failed: {ex.GetBaseException().Message}");
            }
        }

        private sealed class InjectionContext
        {
            internal readonly List<Transform> Grids = new();
            internal readonly HashSet<ShopItem> UsedTemplates = new();
        }

        private static List<CustomShopItem> MergePresetItems(ComputerShop shop)
        {
            var items = _registeredItems;
            try
            {
                var presets = gregCore.Core.Mods.GregCustomItemPresets.GetShopItems(shop);
                if (presets != null && presets.Count > 0)
                    items = items.Concat(presets.Where(p => !items.Any(i =>
                        i.Name == p.Name && i.TemplateID == p.TemplateID &&
                        i.TemplateType == p.TemplateType &&
                        Nullable.Equals(i.BackgroundColor, p.BackgroundColor)))).ToList();
            }
            catch { }
            return items;
        }

        private static void InjectCategoryGroups(ComputerShop shop, List<CustomShopItem> items, InjectionContext ctx)
        {
            try
            {
                var categoryGroups = items.GroupBy(item => item.Category);
                foreach (var catGroup in categoryGroups)
                    InjectCategoryGroup(shop, catGroup, ctx);
            }
            catch { }
        }

        private static void InjectCategoryGroup(ComputerShop shop, IGrouping<string, CustomShopItem> catGroup, InjectionContext ctx)
        {
            try
            {
                string mainCategory = catGroup.Key;
                var subGroups = catGroup.GroupBy(item => item.SubCategory ?? "");
                foreach (var subGroup in subGroups)
                    InjectSubGroup(shop, mainCategory, subGroup.Key, subGroup, ctx);
            }
            catch { }
        }

        private static void InjectSubGroup(ComputerShop shop, string mainCategory, string subCategory, IEnumerable<CustomShopItem> items, InjectionContext ctx)
        {
            try
            {
                Transform container = ShopUI.EnsureCategoryContainer(shop, mainCategory, subCategory);
                if (container == null) return;
                ClearModCards(container);
                foreach (var data in items)
                    InjectSingleItem(shop, container, data, ctx);
                ctx.Grids.Add(container);
            }
            catch { }
        }

        private static void ClearModCards(Transform container)
        {
            try
            {
                for (int i = container.childCount - 1; i >= 0; i--)
                {
                    try
                    {
                        if (container.GetChild(i).name.StartsWith("ModCard_"))
                            Object.DestroyImmediate(container.GetChild(i).gameObject);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static void InjectSingleItem(ComputerShop shop, Transform container, CustomShopItem data, InjectionContext ctx)
        {
            try
            {
                if (HasExternalModConflict(shop, data))
                {
                    _log.Error($"External Conflict: Another mod is using '{data.Name}'. Skipping injection.");
                    return;
                }
                ShopItem template = FindTemplateWithFallback(shop, data);
                if (template == null) return;
                var card = ShopCard.Create(shop, container, template, data);
                if (card != null)
                {
                    ctx.UsedTemplates.Add(template);
                    TryApplyLocked(data, card);
                }
                else CreateShopCardLegacy(shop, container, template, data);
            }
            catch { }
        }

        private static void TryApplyLocked(CustomShopItem data, GameObject card)
        {
            try
            {
                if (gregCore.Core.Mods.GregCustomItemPresets.IsLocked(data.Name))
                    ShopCard.ApplyLocked(card);
            }
            catch { }
        }

        private static void FinalizeInjection(ComputerShop shop, InjectionContext ctx)
        {
            try { RebuildShopLayout(shop); } catch { }
            try
            {
                foreach (var template in ctx.UsedTemplates)
                    RestoreTemplateButton(template);
            }
            catch { }
            try
            {
                foreach (var grid in ctx.Grids)
                    ShopUI.FixGridHeight(grid);
            }
            catch { }
            try { ShopUI.UpdateLayoutHeight(shop); } catch { }
        }

        private static void RebuildShopLayout(ComputerShop shop)
        {
            try
            {
                var sr = shop.shopItemParent.GetComponentInParent<ScrollRect>();
                if (sr?.content != null)
                {
                    try { LayoutRebuilder.ForceRebuildLayoutImmediate(sr.content); } catch { }
                }
            }
            catch { }
        }

        private static ShopItem FindTemplateWithFallback(ComputerShop shop, CustomShopItem data)
        {
            var exact = FindExactTemplate(shop, data);
            if (exact != null) return exact;
            return GetFallbackTemplate(shop, data);
        }

        private static ShopItem FindExactTemplate(ComputerShop shop, CustomShopItem data)
        {
            try
            {
                if (shop.shopItems == null) return null!;
                foreach (var vanillaItem in shop.shopItems)
                {
                    if (vanillaItem != null && vanillaItem.shopItemSO != null &&
                        vanillaItem.shopItemSO.itemType == data.TemplateType &&
                        vanillaItem.shopItemSO.itemID == data.TemplateID)
                        return vanillaItem;
                }
            }
            catch { }
            return null!;
        }

        private static ShopItem GetFallbackTemplate(ComputerShop shop, CustomShopItem data)
        {
            try
            {
                if (shop.shopItems != null && shop.shopItems.Length > 0)
                {
                    _log.Warn($"Could not find template {data.TemplateType} ID {data.TemplateID} for {data.Name}. Using fallback.");
                    return shop.shopItems[0];
                }
            }
            catch { }
            return null!;
        }

        private static void RestoreTemplateButton(ShopItem template)
        {
            try
            {
                if (template == null) return;
                var btn = template.buttonExtended;
                if (btn == null) return;
                btn.onClick.RemoveAllListeners();
                ShopItem cap = template;
                btn.onClick.AddListener((Action)(() =>
                {
                    try { cap.ButtonBuyItem(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }));
            }
            catch { /* best-effort: vanilla card keeps previous wiring */ }
        }

        private static bool HasExternalModConflict(ComputerShop shop, CustomShopItem data)
        {
            try
            {
                int targetID = data.ResultItemID ?? data.TemplateID;
                var allUIItems = shop.shopItemParent.GetComponentsInChildren<ShopItem>(true);
                foreach (var uiItem in allUIItems)
                {
                    if (uiItem?.shopItemSO == null) continue;
                    if (uiItem.shopItemSO.itemName == data.Name) return true;
                    if (IsSameResultItem(uiItem, data, targetID)) return true;
                }
            }
            catch { }
            return false;
        }

        private static bool IsSameResultItem(ShopItem uiItem, CustomShopItem data, int targetID)
        {
            try
            {
                return data.ResultItemID.HasValue &&
                    uiItem.shopItemSO.itemType == data.TemplateType &&
                    uiItem.shopItemSO.itemID == targetID;
            }
            catch { return false; }
        }
    }
}
