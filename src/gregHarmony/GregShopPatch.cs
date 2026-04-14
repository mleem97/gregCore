using System;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Harmony;

[HarmonyPatch(typeof(ComputerShop), "OnEnable")]
public static class GregShopPatch
{
    static void Postfix(ComputerShop __instance)
    {
        MelonLoader.MelonLogger.Msg("[gregCore] ComputerShop detected. Switching to modernized UI...");
        
        // Disable original UI siblings/children safely
        foreach (Transform child in __instance.transform)
        {
            child.gameObject.SetActive(false);
        }

        UIRouter.SetMode(UIMode.ComputerShop);
        InitializeReplacement();
    }

    private static void InitializeReplacement()
    {
        if (GregShopReplacement.Instance != null) return;
        
        var go = new GameObject("GregShop_Root");
        var comp = go.AddComponent<GregShopReplacement>();
        comp.Configure(
            onClose: OnCloseClicked,
            onCheckout: OnCheckoutClicked
        );
    }

    private static void OnCloseClicked()
    {
        GregShopReplacement.Instance?.Hide();
        UIRouter.SetMode(UIMode.Playing);
        
        // Find the Shop original instance and close it if necessary
        // In most games, disabling the UI or calling a Close method works.
        var shop = UnityEngine.Object.FindObjectOfType<ComputerShop>();
        if (shop != null)
        {
            // Trigger original close logic if it exists, or just ensure mode is back
            GameUIButtons.ClickButtonByName("CloseShop");
        }
    }

    private static void OnCheckoutClicked()
    {
        MelonLoader.MelonLogger.Msg("[Shop] Checkout clicked");
        GameUIButtons.ClickButtonByName("ButtonCheckOut");
    }
}
