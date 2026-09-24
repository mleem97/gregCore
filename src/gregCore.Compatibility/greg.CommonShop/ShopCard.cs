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
    internal static class ShopCard
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
                if (cart == null) return null;

                GameObject card;
                try
                {
                    card = data.CustomPrefab != null
                        ? Object.Instantiate(data.CustomPrefab, parent)
                        : Object.Instantiate(template.gameObject, parent);
                }
                catch { return null; }

                try { card.name = $"ModCard_{data.Name.Replace(" ", "_")}"; } catch { }

                try
                {
                    var si = card.GetComponent<ShopItem>();
                    if (si != null) Object.DestroyImmediate(si);
                }
                catch { }

                try
                {
                    foreach (var txt in card.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                    {
                        if (txt == null) continue;
                        string n = "";
                        try { n = txt.name.ToLower(); } catch { continue; }
                        try
                        {
                            if (n == "textprice") txt.text = $"{data.Price} $";
                            else if (n == "text") txt.text = data.Name;
                        }
                        catch { }
                    }
                }
                catch { }

                try
                {
                    foreach (var img in card.GetComponentsInChildren<Image>(true))
                    {
                        if (img == null) continue;
                        string n = "";
                        try { n = img.name.ToLower(); } catch { continue; }
                        try
                        {
                            if (n == "bcg" && data.BackgroundColor.HasValue)
                                img.color = data.BackgroundColor.Value;
                            else if (n == "image" && template.shopItemSO != null)
                            {
                                img.sprite = data.Icon ?? template.shopItemSO.sprite;
                                img.color = Color.white;
                            }
                        }
                        catch { }
                    }
                }
                catch { }

                ButtonExtended btnExt = null;
                try { btnExt = card.GetComponentInChildren<ButtonExtended>(true); } catch { }
                if (btnExt != null)
                {
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

                if (data.OnUIReady != null)
                {
                    try { data.OnUIReady.Invoke(card); }
                    catch (Exception ex) { _log.Warn($"OnUIReady failed '{data.Name}': {ex.GetBaseException().Message}"); }
                }

                try { card.SetActive(true); } catch { }
                return card;
            }
            catch (Exception ex)
            {
                _log.Warn($"ShopCard build failed '{data?.Name}': {ex.GetBaseException().Message}");
                return null;
            }
        }

        /// <summary>Greys out a card and disables its button (locked preset).</summary>
        internal static void ApplyLocked(GameObject card)
        {
            try
            {
                if (card == null) return;
                foreach (var img in card.GetComponentsInChildren<Image>(true))
                {
                    if (img == null) continue;
                    try { img.color = new Color(img.color.r * 0.4f, img.color.g * 0.4f, img.color.b * 0.4f, img.color.a); } catch { }
                }
                foreach (var txt in card.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                {
                    if (txt == null) continue;
                    try { txt.color = new Color(0.5f, 0.5f, 0.5f, 1f); } catch { }
                }
                ButtonExtended btn = null;
                try { btn = card.GetComponentInChildren<ButtonExtended>(true); } catch { }
                if (btn != null)
                {
                    try { btn.onClick.RemoveAllListeners(); } catch { }
                    try { btn.interactable = false; } catch { }
                }
            }
            catch { }
        }

        internal static void AddCustomItemToCart(ComputerShop shop, CustomShopItem data, MethodInfo cart)
        {
            try
            {
                int targetID = data.ResultItemID ?? data.TemplateID;

                try
                {
                    if (shop.cartUIItems != null)
                    {
                        foreach (var cartItem in shop.cartUIItems)
                        {
                            if (cartItem == null) continue;
                            int cid = -1; int cprice = -1; string cname = null;
                            try { cid = cartItem.itemID; } catch { continue; }
                            try { cprice = cartItem.price; } catch { }
                            try { cname = cartItem.itemName; } catch { }
                            if (cid == targetID && cprice == data.Price && cname == data.Name)
                            {
                                try
                                {
                                    cartItem.OnAddClicked();
                                    shop.UpdateCartTotal();
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    _log.Warn($"Cart stack failed '{data.Name}': {ex.GetBaseException().Message}");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch { }

                try
                {
                    var ps = cart.GetParameters();
                    object colorArg;
                    if (ps.Length >= 5)
                    {
                        if (data.PurchaseColor.HasValue)
                        {
                            try { colorArg = Activator.CreateInstance(ps[4].ParameterType, new object[] { data.PurchaseColor.Value }); }
                            catch { colorArg = Activator.CreateInstance(ps[4].ParameterType); }
                        }
                        else colorArg = Activator.CreateInstance(ps[4].ParameterType);
                    }
                    else colorArg = null;
                    cart.Invoke(shop, new object[] { targetID, data.Price, data.TemplateType, data.Name, colorArg });
                    shop.UpdateCartTotal();
                }
                catch (Exception ex)
                {
                    _log.Warn($"Cart add failed '{data.Name}': {ex.GetBaseException().Message}");
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"AddCustomItemToCart failed '{data?.Name}': {ex.GetBaseException().Message}");
            }
        }

        private static MethodInfo ResolveCartMethod()
        {
            try
            {
                if (_cartProbed) return _cartMethod;
                lock (_cartLock)
                {
                    if (_cartProbed) return _cartMethod;
                    _cartProbed = true;
                    foreach (var name in new[] { "SpawnNewCartItem", "AddNewCartItem" })
                    {
                        try
                        {
                            foreach (var m in typeof(ComputerShop).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                            {
                                if (m.Name != name) continue;
                                var ps = m.GetParameters();
                                if (ps.Length != 5) continue;
                                if (ps[0].ParameterType != typeof(int)) continue;
                                if (ps[1].ParameterType != typeof(int)) continue;
                                if (ps[3].ParameterType != typeof(string)) continue;
                                _cartMethod = m;
                                break;
                            }
                            if (_cartMethod != null) break;
                        }
                        catch { }
                    }
                    return _cartMethod;
                }
            }
            catch { return null; }
        }
    }
}
