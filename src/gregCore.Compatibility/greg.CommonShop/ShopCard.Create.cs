using System;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace greg.CommonShop
{
    internal static partial class ShopCard
    {
        private static GameObject CloneCard(Transform parent, ShopItem template, CustomShopItem data)
        {
            try
            {
                return data.CustomPrefab != null
                    ? Object.Instantiate(data.CustomPrefab, parent)
                    : Object.Instantiate(template.gameObject, parent);
            }
            catch { return null!; }
        }

        private static void NameCard(GameObject card, CustomShopItem data)
        {
            try { card.name = $"ModCard_{data.Name.Replace(" ", "_")}"; } catch { }
        }

        private static void RemoveVanillaComponent(GameObject card)
        {
            try
            {
                var si = card.GetComponent<ShopItem>();
                if (si != null) Object.DestroyImmediate(si);
            }
            catch { }
        }

        private static void ApplyCardTexts(GameObject card, CustomShopItem data)
        {
            try
            {
                foreach (var txt in card.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                {
                    if (txt == null) continue;
                    if (!TryLowerName(txt.name, out string n)) continue;
                    try { ApplySingleText(txt, data, n); } catch { }
                }
            }
            catch { }
        }

        private static void ApplySingleText(Il2CppTMPro.TextMeshProUGUI txt, CustomShopItem data, string lowerName)
        {
            try
            {
                if (lowerName == "textprice") txt.text = $"{data.Price} $";
                else if (lowerName == "text") txt.text = data.Name;
            }
            catch { }
        }

        private static void ApplyCardImages(GameObject card, ShopItem template, CustomShopItem data)
        {
            try
            {
                foreach (var img in card.GetComponentsInChildren<Image>(true))
                {
                    if (img == null) continue;
                    if (!TryLowerName(img.name, out string n)) continue;
                    try { ApplySingleImage(img, template, data, n); } catch { }
                }
            }
            catch { }
        }

        private static void ApplySingleImage(Image img, ShopItem template, CustomShopItem data, string lowerName)
        {
            try
            {
                if (lowerName == "bcg" && data.BackgroundColor.HasValue)
                    img.color = data.BackgroundColor.Value;
                else if (lowerName == "image" && template.shopItemSO != null)
                {
                    img.sprite = data.Icon ?? template.shopItemSO.sprite;
                    img.color = Color.white;
                }
            }
            catch { }
        }

        private static bool TryLowerName(string name, out string lower)
        {
            lower = "";
            try { lower = name.ToLower(); return true; }
            catch { return false; }
        }

        private static void RewireBuyButton(ComputerShop shop, GameObject card, CustomShopItem data, System.Reflection.MethodInfo cart)
        {
            ButtonExtended btnExt = null!;
            try { btnExt = card.GetComponentInChildren<ButtonExtended>(true); } catch { }
            if (btnExt == null) return;
            try
            {
                btnExt.onClick.RemoveAllListeners();
                btnExt.interactable = true;
                Action buy = () =>
                {
                    try { AddCustomItemToCart(shop, data, cart); } catch { }
                    try { data.OnBuy?.Invoke(); } catch (Exception ex) { _log.Warn($"OnBuy failed '{data.Name}': {ex.GetBaseException().Message}"); }
                };
                btnExt.onClick.AddListener((Action)buy);
            }
            catch (Exception ex)
            {
                _log.Warn($"Button rewire failed '{data.Name}': {ex.GetBaseException().Message}");
            }
        }

        private static void InvokeUIReady(GameObject card, CustomShopItem data)
        {
            if (data.OnUIReady == null) return;
            try { data.OnUIReady.Invoke(card); }
            catch (Exception ex) { _log.Warn($"OnUIReady failed '{data.Name}': {ex.GetBaseException().Message}"); }
        }
    }
}
