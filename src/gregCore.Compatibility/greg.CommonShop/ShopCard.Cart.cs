using System;
using System.Reflection;
using Il2Cpp;

namespace greg.CommonShop
{
    internal static partial class ShopCard
    {
        internal static void AddCustomItemToCart(ComputerShop shop, CustomShopItem data, MethodInfo cart)
        {
            try
            {
                if (TryStackInCart(shop, data)) return;
                TryInvokeCartMethod(shop, data, cart);
            }
            catch (Exception ex)
            {
                _log.Warn($"AddCustomItemToCart failed '{data?.Name}': {ex.GetBaseException().Message}");
            }
        }

        private static bool TryStackInCart(ComputerShop shop, CustomShopItem data)
        {
            try
            {
                if (shop.cartUIItems == null) return false;
                int targetID = data.ResultItemID ?? data.TemplateID;
                foreach (var cartItem in shop.cartUIItems)
                {
                    if (cartItem == null) continue;
                    if (!IsSameCartEntry(cartItem, data, targetID)) continue;
                    try
                    {
                        cartItem.OnAddClicked();
                        shop.UpdateCartTotal();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _log.Warn($"Cart stack failed '{data.Name}': {ex.GetBaseException().Message}");
                        return true;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            return false;
        }

        private static bool IsSameCartEntry(ShopCartItem cartItem, CustomShopItem data, int targetID)
        {
            try
            {
                int cid = -1; int cprice = -1; string cname = null!;
                try { cid = cartItem.itemID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
                try { cprice = cartItem.price; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                try { cname = cartItem.itemName; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                return cid == targetID && cprice == data.Price && cname == data.Name;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        }

        private static void TryInvokeCartMethod(ComputerShop shop, CustomShopItem data, MethodInfo cart)
        {
            try
            {
                int targetID = data.ResultItemID ?? data.TemplateID;
                object colorArg = BuildColorArg(cart, data);
                cart.Invoke(shop, new object[] { targetID, data.Price, data.TemplateType, data.Name, colorArg });
                shop.UpdateCartTotal();
            }
            catch (Exception ex)
            {
                _log.Warn($"Cart add failed '{data.Name}': {ex.GetBaseException().Message}");
            }
        }

        private static object BuildColorArg(MethodInfo cart, CustomShopItem data)
        {
            try
            {
                var ps = cart.GetParameters();
                if (ps.Length < 5) return null!;
                if (!data.PurchaseColor.HasValue)
                    return Activator.CreateInstance(ps[4].ParameterType);
                try { return Activator.CreateInstance(ps[4].ParameterType, new object[] { data.PurchaseColor.Value }); }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return Activator.CreateInstance(ps[4].ParameterType); }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null!; }
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
                    _cartMethod = FindCartMethod();
                    return _cartMethod;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null!; }
        }

        private static MethodInfo FindCartMethod()
        {
            try
            {
                foreach (var name in new[] { "SpawnNewCartItem", "AddNewCartItem" })
                {
                    var found = FindNamedCartMethod(name);
                    if (found != null) return found;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            return null!;
        }

        private static MethodInfo FindNamedCartMethod(string name)
        {
            try
            {
                foreach (var m in typeof(ComputerShop).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (m.Name != name) continue;
                    if (MatchesCartSignature(m)) return m;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            return null!;
        }

        private static bool MatchesCartSignature(MethodInfo m)
        {
            try
            {
                var ps = m.GetParameters();
                if (ps.Length != 5) return false;
                if (ps[0].ParameterType != typeof(int)) return false;
                if (ps[1].ParameterType != typeof(int)) return false;
                if (ps[3].ParameterType != typeof(string)) return false;
                return true;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        }
    }
}
