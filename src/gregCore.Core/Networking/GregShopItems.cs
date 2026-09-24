/// <file-summary>
/// Layer:      Core (Networking, next to GregShop)
/// Purpose:    Reusable primitives for custom shop items — the boilerplate
///             every shop mod (MoreSpools, MoreModules, MoreServers,
///             Backplanes, RealisticModules) hand-rolls: prefab-ID remap
///             registry, vanilla template lookup, "HL Mods" section lookup,
///             guid dedup, button creation. See docs/modding/shop-items.md.
///             Best-effort; never throws.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregShopItems
{
    private sealed class Resolver
    {
        public int BaseItemId;
        public Func<GameObject> Resolve;
    }

    private static readonly Dictionary<int, Resolver> _resolvers = new();

    // ── Prefab remap registry (for GetPrefabForItem prefixes) ───────────────

    public static void RegisterPrefab(int itemId, int baseItemId, Func<GameObject> resolver)
    {
        if (resolver == null) return;
        try { lock (_resolvers) { _resolvers[itemId] = new Resolver { BaseItemId = baseItemId, Resolve = resolver }; } }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore][Shop] RegisterPrefab failed ({itemId}): {ex.Message}"); }
    }

    public static void UnregisterPrefab(int itemId)
    {
        try { lock (_resolvers) { _resolvers.Remove(itemId); } } catch { }
    }

    public static bool TryGetBaseId(int itemId, out int baseItemId)
    {
        baseItemId = 0;
        try
        {
            lock (_resolvers)
            {
                if (_resolvers.TryGetValue(itemId, out var r) && r != null)
                {
                    baseItemId = r.BaseItemId;
                    return true;
                }
            }
        }
        catch { }
        return false;
    }

    public static bool TryResolvePrefab(int itemId, out GameObject prefab)
    {
        prefab = null;
        try
        {
            Resolver r = null;
            lock (_resolvers)
            {
                _resolvers.TryGetValue(itemId, out r);
            }
            if (r?.Resolve == null) return false;
            try { prefab = r.Resolve(); } catch { prefab = null; }
            return prefab != null;
        }
        catch { return false; }
    }

    // ── Shop structure lookup ────────────────────────────────────────────────

    public static global::Il2Cpp.ShopItem FindTemplate(global::Il2Cpp.ComputerShop shop,
        global::Il2Cpp.PlayerManager.ObjectInHand itemType)
    {
        try
        {
            if (shop == null) return null;
            var items = shop.shopItems;
            if (items == null) return null;
            foreach (var si in items)
            {
                if (si == null || si.shopItemSO == null) continue;
                int t = 0;
                try { t = (int)si.shopItemSO.itemType; } catch { continue; }
                if (t == (int)itemType) return si;
            }
        }
        catch { }
        return null;
    }

    public static GameObject FindSection(global::Il2Cpp.ComputerShop shop, string sectionName)
    {
        try
        {
            if (shop == null) return null;
            var parent = shop.shopItemParent;
            if (parent == null) return null;
            if (!string.IsNullOrEmpty(sectionName))
            {
                try
                {
                    var section = parent.transform.Find(sectionName);
                    if (section != null) return section.gameObject;
                }
                catch { }
            }
            return parent;
        }
        catch { return null; }
    }

    // ── Button dedup + creation ──────────────────────────────────────────────

    public static bool ButtonExists(GameObject parent, string guid)
    {
        try
        {
            if (parent == null || string.IsNullOrEmpty(guid)) return false;
            var tr = parent.transform;
            if (tr == null) return false;
            for (int i = 0; i < tr.childCount; i++)
            {
                GameObject child = null;
                try { child = tr.GetChild(i)?.gameObject; } catch { continue; }
                if (child == null) continue;
                global::Il2Cpp.ShopItem si = null;
                try { si = child.GetComponent<global::Il2Cpp.ShopItem>(); } catch { continue; }
                if (si == null) continue;
                string g = null;
                try { g = si.guid; } catch { continue; }
                if (string.Equals(g, guid, StringComparison.Ordinal)) return true;
            }
        }
        catch { }
        return false;
    }

    public static global::Il2Cpp.ShopItem AddButton(global::Il2Cpp.ShopItem template, GameObject parent,
        int itemId, string label, int price, int xpToUnlock, string guid, bool isCustomColor, Sprite sprite)
    {
        try
        {
            if (template == null || parent == null || template.shopItemSO == null) return null;
            if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(guid)) return null;
            if (ButtonExists(parent, guid)) return null;

            var src = template.shopItemSO;
            var so = ScriptableObject.CreateInstance<global::Il2Cpp.ShopItemSO>();
            if (so == null) return null;
            Try(() => so.itemName = label);
            Try(() => so.price = price);
            Try(() => so.xpToUnlock = xpToUnlock);
            Try(() => so.itemType = src.itemType);
            Try(() => so.itemID = itemId);
            Try(() => so.eol = src.eol);
            Try(() => so.sprite = sprite != null ? sprite : src.sprite);
            Try(() => so.isCustomColor = isCustomColor);

            GameObject cloned = null;
            try { cloned = UnityEngine.Object.Instantiate(template.gameObject, parent.transform, false); }
            catch { return null; }
            if (cloned == null) return null;
            Try(() => cloned.name = "ShopItem_" + label.Replace(" ", "_").Replace("(", "").Replace(")", ""));
            Try(() => cloned.transform.localPosition = Vector3.zero);
            Try(() => cloned.transform.localScale = Vector3.one);

            global::Il2Cpp.ShopItem item = null;
            try { item = cloned.GetComponent<global::Il2Cpp.ShopItem>(); } catch { }
            if (item == null)
            {
                try { UnityEngine.Object.Destroy(cloned); } catch { }
                return null;
            }
            Try(() => item.shopItemSO = so);
            Try(() => item.guid = guid);
            Try(() => cloned.SetActive(true));
            return item;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Shop] AddButton failed ('{label}'): {ex.Message}");
            return null;
        }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Shop] {ex.GetType().Name}: {ex.GetBaseException().Message}");
        }
    }
}
