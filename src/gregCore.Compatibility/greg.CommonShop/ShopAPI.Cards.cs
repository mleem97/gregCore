using System;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace greg.CommonShop
{
    public static partial class ShopAPI
    {
        /// <summary>
        /// Legacy card path (vanilla ShopItemSO flow): used when the custom
        /// cart API is unavailable on this game build. Keeps vanilla buy,
        /// checkout and stacking intact; adds visuals plus notify-only OnBuy.
        /// </summary>
        private static void CreateShopCardLegacy(Transform container, ShopItem template, CustomShopItem data)
        {
            try
            {
                var clone = CloneTemplateCard(container, template, data);
                if (clone == null) return;
                var si = clone.GetComponent<ShopItem>();
                if (si == null) return;
                FillLegacyShopItem(si, data);
                try { si.Start(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                ApplyCardVisuals(clone, template, data);
                try { data.OnUIReady?.Invoke(clone); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
            catch (Exception ex)
            {
                _log.Warn($"Legacy card failed '{data?.Name}': {ex.GetBaseException().Message}");
            }
        }

        private static GameObject CloneTemplateCard(Transform container, ShopItem template, CustomShopItem data)
        {
            try
            {
                var clone = Object.Instantiate(template.gameObject, container);
                clone.name = "ModCard_" + data.Name;
                try { clone.SetActive(true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                return clone;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null!; }
        }

        private static void FillLegacyShopItem(ShopItem si, CustomShopItem data)
        {
            try
            {
                var newSo = ScriptableObject.CreateInstance<ShopItemSO>();
                newSo.itemName = data.Name;
                newSo.price = data.Price;
                newSo.itemType = data.TemplateType;
                newSo.itemID = data.ResultItemID ?? data.TemplateID;
                if (data.Icon != null) newSo.sprite = data.Icon;
                si.shopItemSO = newSo;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        internal static void ApplyCardVisuals(GameObject card, ShopItem template, CustomShopItem data)
        {
            try { ApplyCardTexts(card, data); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            try { ApplyCardImages(card, template, data); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static void ApplyCardTexts(GameObject card, CustomShopItem data)
        {
            try
            {
                foreach (var txt in card.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                {
                    if (txt == null) continue;
                    if (!TryGetLowerName(txt.name, out string n)) continue;
                    try
                    {
                        if (n == "textprice") txt.text = $"{data.Price} $";
                        else if (n == "text") txt.text = data.Name;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static void ApplyCardImages(GameObject card, ShopItem template, CustomShopItem data)
        {
            try
            {
                foreach (var img in card.GetComponentsInChildren<Image>(true))
                {
                    if (img == null) continue;
                    if (!TryGetLowerName(img.name, out string n)) continue;
                    try { ApplySingleImage(img, template, data, n); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
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
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static bool TryGetLowerName(string name, out string lower)
        {
            lower = "";
            try { lower = name.ToLower(); return true; }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        }
    }
}
