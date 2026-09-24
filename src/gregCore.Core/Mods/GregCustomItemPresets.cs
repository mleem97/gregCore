/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Custom-color presets: captures vanilla color purchases
///              (ButtonChosenColor), persists them as JSON and re-registers
///              them as shop items (Mods category) with color, price and
///              unlock gating. Opt-in service, no impact when unused.
/// Maintainer:  Storage under UserData/gregCore (directory policy);
///              legacy UserData/CustomItemPresets.json is migrated once.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Filesystem persistence + live Il2Cpp shop interop; needs running game.")]
public static class GregCustomItemPresets
{
    public sealed class Preset
    {
        public string ColorHex { get; set; } = "#FFFFFF";
        public int ItemID { get; set; }
        public int ItemType { get; set; }
        public int Price { get; set; }
        public string DisplayName { get; set; } = "";
    }

    private sealed class PresetList
    {
        public List<Preset> Presets { get; set; } = new();
    }

    private static readonly object _lock = new();
    private static PresetList _presets = new();
    private static bool _loaded;
    private static readonly Dictionary<string, bool> _lockedByName = new(StringComparer.Ordinal);

    internal static string StorePath =>
        Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "gregCore", "CustomItemPresets.json");
    private static string LegacyStorePath =>
        Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "CustomItemPresets.json");

    internal static string Key(Preset p) => $"{p.ItemID}|{p.ItemType}|{p.ColorHex}";

    public static IReadOnlyList<Preset> All
    {
        get { lock (_lock) { EnsureLoaded(); return _presets.Presets.ToArray(); } }
    }

    /// <summary>Capture a color purchase. Returns true when a new preset was stored.</summary>
    public static bool TryCapture(int itemId, int itemType, int price, string displayName, Color color)
    {
        string hex = "#" + ColorUtility.ToHtmlStringRGB(color);
        lock (_lock)
        {
            EnsureLoaded();
            if (_presets.Presets.Any(p => p.ItemID == itemId && p.ItemType == itemType && string.Equals(p.ColorHex, hex, StringComparison.OrdinalIgnoreCase)))
                return false;
            _presets.Presets.Add(new Preset
            {
                ColorHex = hex,
                ItemID = itemId,
                ItemType = itemType,
                Price = price,
                DisplayName = displayName ?? ""
            });
            Save();
            try { MelonLogger.Msg($"[gregCore][Presets] Saved: '{displayName}' color={hex}"); } catch { }
            return true;
        }
    }

    /// <summary>Builds shop items for all presets; refreshes unlock state when shop given.</summary>
    internal static List<greg.CommonShop.CustomShopItem> GetShopItems(global::Il2Cpp.ComputerShop shop)
    {
        lock (_lock)
        {
            EnsureLoaded();
            _lockedByName.Clear();
            var items = new List<greg.CommonShop.CustomShopItem>();
            foreach (var p in _presets.Presets)
            {
                Color? tint = null;
                try { if (ColorUtility.TryParseHtmlString(p.ColorHex, out var c)) tint = c; } catch { }
                Sprite icon = null;
                try { icon = FindCustomColorIcon(p); } catch { }
                bool locked = false;
                try { locked = IsLocked(shop, p); } catch { }
                var item = new greg.CommonShop.CustomShopItem
                {
                    Name = p.DisplayName,
                    Price = p.Price,
                    TemplateType = (global::Il2Cpp.PlayerManager.ObjectInHand)p.ItemType,
                    TemplateID = p.ItemID,
                    BackgroundColor = tint,
                    PurchaseColor = tint,
                    Icon = icon,
                    Category = "Mods",
                    SubCategory = ""
                };
                items.Add(item);
                _lockedByName[item.Name] = locked;
            }
            return items;
        }
    }

    internal static bool IsLocked(string itemName)
    {
        lock (_lock) { return _lockedByName.TryGetValue(itemName, out var l) && l; }
    }

    private static bool IsLocked(global::Il2Cpp.ComputerShop shop, Preset p)
    {
        try
        {
            if (shop == null || shop.shopItems == null) return false;
            foreach (var si in shop.shopItems)
            {
                if (si?.shopItemSO != null &&
                    si.shopItemSO.itemID == p.ItemID &&
                    (int)si.shopItemSO.itemType == p.ItemType &&
                    si.shopItemSO.isCustomColor)
                    return !si.isUnlocked;
            }
        }
        catch { }
        return false;
    }

    private static Sprite FindCustomColorIcon(Preset p)
    {
        try
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.ShopItemSO>();
            if (all == null) return null;
            foreach (var so in all)
            {
                if (so != null && so.itemID == p.ItemID && (int)so.itemType == p.ItemType && so.isCustomColor)
                    return so.sprite;
            }
        }
        catch { }
        return null;
    }

    private static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        try
        {
            string path = StorePath;
            if (!File.Exists(path) && File.Exists(LegacyStorePath))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    File.Copy(LegacyStorePath, path, overwrite: false);
                }
                catch { }
            }
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            var list = JsonSerializer.Deserialize<PresetList>(json);
            if (list?.Presets == null) return;
            _presets = list;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var deduped = new List<Preset>();
            foreach (var p in _presets.Presets)
            {
                if (p == null) continue;
                if (seen.Add(Key(p))) deduped.Add(p);
            }
            if (deduped.Count != _presets.Presets.Count)
            {
                _presets.Presets = deduped;
                Save();
            }
        }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Presets] Load failed: {ex.GetBaseException().Message}"); } catch { }
        }
    }

    private static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StorePath)!);
            File.WriteAllText(StorePath, JsonSerializer.Serialize(_presets, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Presets] Save failed: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
