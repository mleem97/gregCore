using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using greg.Logging;

namespace greg.CommonShop
{
    public static class ShopAPI
    {
        private static List<CustomShopItem> _registeredItems = new();
        internal static List<CustomShopItem> RegisteredItems => _registeredItems;
        private static GregModLogger _log = new GregModLogger("CommonShop");

        private static bool _initialized;
        private static Dictionary<int, Dictionary<string, string>> _usedCustomIDs = new();
        internal static string RegistryFilePath =>
            Path.Combine(MelonEnvironment.UserDataDirectory, "gregCore", "CommonShop_CustomIDs.json");
        private static string LegacyRegistryFilePath =>
            Path.Combine(MelonEnvironment.UserDataDirectory, "CommonShop_CustomIDs.json");

        /// <summary>
        /// Initializes the shop library (idempotent). In gregCore the Harmony
        /// patches are already applied by MelonLoader, so this mainly loads the
        /// persistent custom-ID registry. External mods may call it exactly
        /// like the standalone CommonShop library.
        /// </summary>
        public static void Initialize(HarmonyLib.Harmony harmony)
        {
            if (_initialized) return;
            try { LoadIDRegistry(); } catch (Exception ex) { _log.Warn($"ID registry load failed: {ex.GetBaseException().Message}"); }
            _initialized = true;
        }

        internal static void EnsureInitialized()
        {
            if (_initialized) return;
            try { LoadIDRegistry(); } catch { }
            _initialized = true;
        }

        public static void RegisterItem(CustomShopItem item)
        {
            if (item == null) return;
            EnsureInitialized();

            if (_registeredItems.Any(i => i.Name == item.Name))
            {
                _log.Warn($"Conflict: An item named '{item.Name}' is already registered! Skipping.");
                return;
            }

            if (item.ResultItemID.HasValue)
            {
                if (_registeredItems.Any((i => i.TemplateType == item.TemplateType && i.ResultItemID == item.ResultItemID)))
                {
                    _log.Warn($"Conflict: ResultItemID {item.ResultItemID} for {item.TemplateType} is already claimed! Skipping.");
                    return;
                }
            }

            // Persistent custom-ID tracking for non-vanilla enum values.
            try { ClaimCustomId(item); } catch (Exception ex) { _log.Warn($"Custom ID claim failed '{item.Name}': {ex.GetBaseException().Message}"); }

            if (string.IsNullOrEmpty(item.Category))
            {
                item.Category = "Mods";
            }

            _registeredItems.Add(item);
            _log.Msg($"Registered: {item.Name} in {item.Category}");

            // Live injection when the shop is already open.
            try
            {
                var shop = MainGameManager.instance?.computerShop;
                if (shop != null && shop.gameObject != null && shop.gameObject.activeInHierarchy)
                {
                    _log.Msg("Shop is already active, forcing live injection...");
                    InjectAll(shop);
                }
            }
            catch { /* best-effort */ }
        }

        /// <summary>Attributes the calling mod for the persistent ID registry.</summary>
        private static string CallingModName()
        {
            try
            {
                var calling = Assembly.GetCallingAssembly();
                string modName = calling?.GetName()?.Name ?? "unknown";
                try
                {
                    foreach (var melon in MelonMod.RegisteredMelons)
                    {
                        if (melon?.MelonAssembly?.Assembly == calling)
                        {
                            modName = melon.Info?.Name ?? modName;
                            break;
                        }
                    }
                }
                catch { }
                return string.IsNullOrWhiteSpace(modName) ? "unknown" : modName;
            }
            catch { return "unknown"; }
        }

        private static void ClaimCustomId(CustomShopItem item)
        {
            bool defined;
            try { defined = Enum.IsDefined(typeof(PlayerManager.ObjectInHand), item.TemplateType); }
            catch { return; }
            if (defined) return;

            int customId = (int)item.TemplateType;
            string modName = CallingModName();

            if (_usedCustomIDs.TryGetValue(customId, out var existingEntry))
            {
                string existingMod = "Unknown Mod";
                string existingItem = "Unknown Item";
                try
                {
                    if (existingEntry != null)
                    {
                        if (existingEntry.TryGetValue("ModName", out var m)) existingMod = m;
                        if (existingEntry.TryGetValue("ItemName", out var n)) existingItem = n;
                    }
                }
                catch { }
                if (!string.Equals(existingMod, modName, StringComparison.Ordinal))
                {
                    _log.Error($"FATAL ID COLLISION! Mod '{modName}' tried to register custom ID [{customId}].");
                    _log.Error($"-> That ID is registered to '{existingMod}' for '{existingItem}'. Skipping item to prevent corruption.");
                    throw new InvalidOperationException($"Custom ID {customId} already claimed by '{existingMod}'.");
                }
                return;
            }

            _usedCustomIDs[customId] = new Dictionary<string, string>
            {
                { "ModName", modName },
                { "ItemName", item.Name }
            };
            SaveIDRegistry();
            _log.Msg($"Registered new custom ID [{customId}] to mod '{modName}' for '{item.Name}'.");
        }

        internal static void InjectAll(ComputerShop shop)
        {
            try
            {
                EnsureInitialized();
                var items = _registeredItems;
                try
                {
                    var presets = gregCore.Core.Mods.GregCustomItemPresets.GetShopItems(shop);
                    if (presets != null && presets.Count > 0)
                        items = items.Concat(presets.Where(p => !items.Any(i =>
                            i.Name == p.Name && i.TemplateID == p.TemplateID &&
                            i.TemplateType == p.TemplateType &&
                            Nullable.Equals(i.BackgroundColor, p.BackgroundColor)))).ToList();
                }
                catch { }

                if (items.Count == 0) return;

                var injectedGrids = new List<Transform>();
                var usedTemplates = new HashSet<ShopItem>();
                var categoryGroups = items.GroupBy(item => item.Category);

                foreach (var catGroup in categoryGroups)
                {
                    string mainCategory = catGroup.Key;
                    var subGroups = catGroup.GroupBy(item => item.SubCategory ?? "");

                    foreach (var subGroup in subGroups)
                    {
                        string subCategory = subGroup.Key;

                        Transform container = ShopUI.EnsureCategoryContainer(shop, mainCategory, subCategory);
                        if (container == null) continue;

                        for (int i = container.childCount - 1; i >= 0; i--)
                        {
                            try
                            {
                                if (container.GetChild(i).name.StartsWith("ModCard_"))
                                    Object.DestroyImmediate(container.GetChild(i).gameObject);
                            }
                            catch { }
                        }

                        foreach (var data in subGroup)
                        {
                            if (HasExternalModConflict(shop, data))
                            {
                                _log.Error($"External Conflict: Another mod is using '{data.Name}'. Skipping injection.");
                                continue;
                            }

                            ShopItem template = FindTemplateWithFallback(shop, data);
                            if (template != null)
                            {
                                var card = ShopCard.Create(shop, container, template, data);
                                if (card != null)
                                {
                                    usedTemplates.Add(template);
                                    try
                                    {
                                        if (gregCore.Core.Mods.GregCustomItemPresets.IsLocked(data.Name))
                                            ShopCard.ApplyLocked(card);
                                    }
                                    catch { }
                                }
                                else CreateShopCardLegacy(shop, container, template, data);
                            }
                        }

                        injectedGrids.Add(container);
                    }
                }

                var sr = shop.shopItemParent.GetComponentInParent<ScrollRect>();
                if (sr?.content != null)
                {
                    try { LayoutRebuilder.ForceRebuildLayoutImmediate(sr.content); } catch { }
                }

                // Il2Cpp shares the native ButtonExtended.onClick event between a
                // clone and its template: restoring the template button keeps the
                // vanilla item buying via ButtonBuyItem.
                foreach (var template in usedTemplates)
                    RestoreTemplateButton(template);

                foreach (var grid in injectedGrids)
                    ShopUI.FixGridHeight(grid);

                ShopUI.UpdateLayoutHeight(shop);
            }
            catch (Exception ex)
            {
                _log.Warn($"InjectAll failed: {ex.GetBaseException().Message}");
            }
        }

        private static ShopItem FindTemplateWithFallback(ComputerShop shop, CustomShopItem data)
        {
            ShopItem exact = null;
            try
            {
                if (shop.shopItems != null)
                {
                    foreach (var vanillaItem in shop.shopItems)
                    {
                        if (vanillaItem != null && vanillaItem.shopItemSO != null &&
                            vanillaItem.shopItemSO.itemType == data.TemplateType &&
                            vanillaItem.shopItemSO.itemID == data.TemplateID)
                        {
                            exact = vanillaItem;
                            break;
                        }
                    }
                }
            }
            catch { }
            if (exact != null) return exact;
            try
            {
                if (shop.shopItems != null && shop.shopItems.Length > 0)
                {
                    _log.Warn($"Could not find template {data.TemplateType} ID {data.TemplateID} for {data.Name}. Using fallback.");
                    return shop.shopItems[0];
                }
            }
            catch { }
            return null;
        }

        private static void RestoreTemplateButton(ShopItem template)
        {
            try
            {
                if (template == null) return;
                var btn = template.buttonExtended;
                if (btn == null) return;
                btn.onClick.RemoveAllListeners();
                ShopItem cap = template;
                btn.onClick.AddListener((Action)(() =>
                {
                    try { cap.ButtonBuyItem(); } catch { }
                }));
            }
            catch { /* best-effort: vanilla card keeps previous wiring */ }
        }

        private static bool HasExternalModConflict(ComputerShop shop, CustomShopItem data)
        {
            try
            {
                int targetID = data.ResultItemID ?? data.TemplateID;
                var allUIItems = shop.shopItemParent.GetComponentsInChildren<ShopItem>(true);

                foreach (var uiItem in allUIItems)
                {
                    if (uiItem?.shopItemSO != null)
                    {
                        if (uiItem.shopItemSO.itemName == data.Name) return true;

                        if (data.ResultItemID.HasValue &&
                            uiItem.shopItemSO.itemType == data.TemplateType &&
                            uiItem.shopItemSO.itemID == targetID)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Legacy card path (vanilla ShopItemSO flow): used when the custom
        /// cart API is unavailable on this game build. Keeps vanilla buy,
        /// checkout and stacking intact; adds visuals plus notify-only OnBuy.
        /// </summary>
        private static void CreateShopCardLegacy(ComputerShop shop, Transform container, ShopItem template, CustomShopItem data)
        {
            try
            {
                var clone = Object.Instantiate(template.gameObject, container);
                clone.name = "ModCard_" + data.Name;
                try { clone.SetActive(true); } catch { }

                var si = clone.GetComponent<ShopItem>();
                if (si != null)
                {
                    var newSo = ScriptableObject.CreateInstance<ShopItemSO>();
                    newSo.itemName = data.Name;
                    newSo.price = data.Price;
                    newSo.itemType = data.TemplateType;
                    newSo.itemID = data.ResultItemID ?? data.TemplateID;
                    if (data.Icon != null) newSo.sprite = data.Icon;

                    si.shopItemSO = newSo;
                    try { si.Start(); } catch { }

                    // Visuals only here: the vanilla buy flow stays untouched.
                    // (No OnBuy wiring — the clone shares the native onClick
                    // event with its template, so AddListener would leak onto
                    // the vanilla card. OnBuy fires on the ShopCard path.)
                    ApplyCardVisuals(clone, template, data);

                    // Invoke callback
                    try { data.OnUIReady?.Invoke(clone); } catch { }
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"Legacy card failed '{data?.Name}': {ex.GetBaseException().Message}");
            }
        }

        internal static void ApplyCardVisuals(GameObject card, ShopItem template, CustomShopItem data)
        {
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
        }

        private static void LoadIDRegistry()
        {
            _usedCustomIDs.Clear();
            string path = RegistryFilePath;
            try
            {
                if (!File.Exists(path))
                {
                    // Migrate legacy location once.
                    if (File.Exists(LegacyRegistryFilePath))
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                            File.Copy(LegacyRegistryFilePath, path, overwrite: false);
                        }
                        catch { }
                    }
                    else return;
                }
                if (!File.Exists(path)) return;

                string jsonString = File.ReadAllText(path);
                try
                {
                    var loaded = JsonSerializer.Deserialize<Dictionary<int, Dictionary<string, string>>>(jsonString);
                    if (loaded != null) _usedCustomIDs = loaded;
                    _log.Msg($"Loaded {_usedCustomIDs.Count} claimed custom IDs from registry.");
                }
                catch
                {
                    try
                    {
                        var legacy = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonString);
                        if (legacy != null)
                        {
                            foreach (var kvp in legacy)
                                _usedCustomIDs[kvp.Key] = new Dictionary<string, string>
                                {
                                    { "ModName", kvp.Value },
                                    { "ItemName", "Unknown (Legacy Format)" }
                                };
                            SaveIDRegistry();
                            _log.Msg("Upgraded legacy ID registry format.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to load ID registry: {ex.GetBaseException().Message}");
                        _usedCustomIDs = new Dictionary<int, Dictionary<string, string>>();
                    }
                }
            }
            catch { }
        }

        private static void SaveIDRegistry()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RegistryFilePath)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(RegistryFilePath, JsonSerializer.Serialize(_usedCustomIDs, options));
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to save ID registry: {ex.GetBaseException().Message}");
            }
        }
    }
}
