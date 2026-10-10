using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.GameLayer.Hooks;

namespace greg.CommonShop
{
    /// <summary>
    /// Custom-color presets + cart correctness for custom cards.
    /// Port of the DataCenter-CustomItemSaver behavior into gregCore:
    /// capture color purchases, re-inject preset cards, keep plain buys
    /// working next to custom cart entries, refresh on unlock.
    /// All best-effort — a failing hook never breaks the shop.
    /// </summary>
    [HarmonyPatch]
    public static class CustomItemPresetPatch
    {
        [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonChosenColor))]
        [HarmonyPrefix]
        public static void CaptureColorPrefix(ComputerShop __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (__instance.flexibleColorPicker == null || !__instance.isPendingColorPurchase) return;

                Color chosen;
                try { chosen = __instance.flexibleColorPicker.color; } catch { return; }
                int itemId, itemType, price;
                string displayName;
                try
                {
                    itemId = __instance.pendingItemID;
                    itemType = Convert.ToInt32(__instance.pendingItemType);
                    price = __instance.pendingPrice;
                    displayName = __instance.pendingDisplayName;
                }
                catch { return; }

                gregCore.Core.Mods.GregCustomItemPresets.TryCapture(itemId, itemType, price, displayName, chosen);
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CustomItemPresetPatch), ex);
            }
        }

        [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonChosenColor))]
        [HarmonyPostfix]
        public static void ReinjectPostfix(ComputerShop __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                ShopAPI.InjectAll(__instance);
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CustomItemPresetPatch), ex);
            }
        }

        // When the user buys the plain version of an item that already has a
        // custom-coloured entry in the cart, the game would increment the
        // custom entry instead. Temporarily mask it so a plain entry is added.
        [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBuyShopItem))]
        [HarmonyPrefix]
        public static void MaskCustomCartPrefix(ComputerShop __instance, int itemID,
            int price, PlayerManager.ObjectInHand itemType, string displayName, bool isCustomColor)
        {
            _maskedItem = null;
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (isCustomColor) return;
                var cartItems = __instance.cartUIItems;
                if (cartItems == null) return;
                foreach (var ci in cartItems)
                {
                    if (ci != null && ci.itemID == itemID && ci.itemType == itemType && ci.hasCustomColor)
                    {
                        _maskedItem = ci;
                        _originalItemID = ci.itemID;
                        ci.itemID = -1;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CustomItemPresetPatch), ex);
            }
        }

        private static ShopCartItem _maskedItem;
        private static int _originalItemID;

        [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBuyShopItem))]
        [HarmonyPostfix]
        public static void UnmaskCustomCartPostfix()
        {
            try
            {
                if (_maskedItem != null)
                {
                    _maskedItem.itemID = _originalItemID;
                    _maskedItem = null;
                }
            }
            catch { _maskedItem = null; }
        }

        // UpdateVisualState fires after isUnlocked is set — refresh presets so
        // newly unlocked base items become buyable immediately.
        [HarmonyPatch(typeof(ShopItem), nameof(ShopItem.UpdateVisualState))]
        [HarmonyPostfix]
        public static void RefreshOnUnlockPostfix(ShopItem __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                if (!__instance.isUnlocked) return;
                ComputerShop shop = null;
                try { shop = UnityEngine.Object.FindObjectOfType<ComputerShop>(); } catch { return; }
                if (shop == null || shop.shopItemParent == null) return;
                try { if (!shop.shopItemParent.activeInHierarchy) return; } catch { return; }
                ShopAPI.InjectAll(shop);
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CustomItemPresetPatch), ex);
            }
        }
    }
}
