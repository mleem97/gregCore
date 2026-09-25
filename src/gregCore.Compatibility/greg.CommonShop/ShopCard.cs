using System;
using System.Reflection;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using greg.Logging;

namespace greg.CommonShop
{
    /// <summary>
    /// Builds purely custom shop cards: template clone without the vanilla
    /// <c>ShopItem</c> component, direct texts/icon/tint and a rewired buy
    /// button (cart stacking + <c>OnBuy</c>). Best-effort; returns false when
    /// the game cart API is unavailable so callers can use the legacy
    /// ShopItemSO path instead. Never throws.
    /// </summary>
    internal static partial class ShopCard
    {
        private static readonly GregModLogger _log = new GregModLogger("CommonShop");
        private static MethodInfo _cartMethod;
        private static bool _cartProbed;
        private static readonly object _cartLock = new object();

        internal static bool HasCartApi => ResolveCartMethod() != null;

        /// <summary>Builds the card; returns it, or null when the caller
        /// should use the legacy ShopItemSO path.</summary>
        internal static GameObject Create(ComputerShop shop, Transform parent, ShopItem template, CustomShopItem data)
        {
            try
            {
                var cart = ResolveCartMethod();
                if (cart == null) return null!;
                GameObject card = CloneCard(parent, template, data);
                if (card == null) return null!;
                NameCard(card, data);
                RemoveVanillaComponent(card);
                ApplyCardTexts(card, data);
                ApplyCardImages(card, template, data);
                RewireBuyButton(shop, card, data, cart);
                InvokeUIReady(card, data);
                try { card.SetActive(true); } catch { }
                return card;
            }
            catch (Exception ex)
            {
                _log.Warn($"ShopCard build failed '{data?.Name}': {ex.GetBaseException().Message}");
                return null!;
            }
        }

        /// <summary>Greys out a card and disables its button (locked preset).</summary>
        internal static void ApplyLocked(GameObject card)
        {
            try
            {
                if (card == null) return;
                DimImages(card);
                DimTexts(card);
                DisableButton(card);
            }
            catch { }
        }

        private static void DimImages(GameObject card)
        {
            try
            {
                foreach (var img in card.GetComponentsInChildren<Image>(true))
                {
                    if (img == null) continue;
                    try { img.color = new Color(img.color.r * 0.4f, img.color.g * 0.4f, img.color.b * 0.4f, img.color.a); } catch { }
                }
            }
            catch { }
        }

        private static void DimTexts(GameObject card)
        {
            try
            {
                foreach (var txt in card.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                {
                    if (txt == null) continue;
                    try { txt.color = new Color(0.5f, 0.5f, 0.5f, 1f); } catch { }
                }
            }
            catch { }
        }

        private static void DisableButton(GameObject card)
        {
            ButtonExtended btn = null!;
            try { btn = card.GetComponentInChildren<ButtonExtended>(true); } catch { }
            if (btn == null) return;
            try { btn.onClick.RemoveAllListeners(); } catch { }
            try { btn.interactable = false; } catch { }
        }
    }
}
