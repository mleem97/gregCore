using HarmonyLib;
using Il2Cpp;
using System;
using gregCore.GameLayer.Hooks;

namespace greg.CommonShop
{
    [HarmonyPatch]
    public static class CommonShopPatch
    {
        [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.ButtonShopScreen))]
        [HarmonyPostfix]
        public static void OnShopScreenOpened(global::Il2Cpp.ComputerShop __instance)
        {
            try
            {
                ShopAPI.InjectAll(__instance);
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CommonShopPatch), ex);
            }
        }

        [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.ButtonCheckOut))]
        [HarmonyPrefix]
        public static void OnCheckout(global::Il2Cpp.ComputerShop __instance)
        {
            try
            {
                if (__instance.cartUIItems == null) return;

                foreach (var cartItem in __instance.cartUIItems)
                {
                    if (cartItem == null) continue;

                    foreach (var customItem in ShopAPI.RegisteredItems)
                    {
                        int targetID = customItem.ResultItemID ?? customItem.TemplateID;

                        if (cartItem.itemID == targetID && cartItem.itemName == customItem.Name)
                        {
                            customItem.OnCheckout?.Invoke(cartItem.Quantity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(CommonShopPatch), ex);
            }
        }
    }
}
