using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBuyShopItem))]
internal static class Patch_ComputerShop_ButtonBuyShopItem
{
    internal static void Postfix(int __0, int __1, int __2)
    {
        try { EventDispatcher.FireShopItemAdded(__0, __1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonBuyShopItem: {ex.Message}"); }
    }
}
