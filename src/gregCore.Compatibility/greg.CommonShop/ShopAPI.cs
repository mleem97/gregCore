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
    public static partial class ShopAPI
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
            if (HasNameConflict(item)) return;
            if (HasResultIdConflict(item)) return;

            // Persistent custom-ID tracking for non-vanilla enum values.
            try { ClaimCustomId(item); } catch (Exception ex) { _log.Warn($"Custom ID claim failed '{item.Name}': {ex.GetBaseException().Message}"); }

            EnsureCategory(item);
            _registeredItems.Add(item);
            _log.Msg($"Registered: {item.Name} in {item.Category}");
            TryLiveInject();
        }

        private static bool HasNameConflict(CustomShopItem item)
        {
            try
            {
                if (_registeredItems.Any(i => i.Name == item.Name))
                {
                    _log.Warn($"Conflict: An item named '{item.Name}' is already registered! Skipping.");
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static bool HasResultIdConflict(CustomShopItem item)
        {
            try
            {
                if (item.ResultItemID.HasValue &&
                    _registeredItems.Any((i => i.TemplateType == item.TemplateType && i.ResultItemID == item.ResultItemID)))
                {
                    _log.Warn($"Conflict: ResultItemID {item.ResultItemID} for {item.TemplateType} is already claimed! Skipping.");
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static void EnsureCategory(CustomShopItem item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Category))
                    item.Category = "Mods";
            }
            catch { }
        }

        private static void TryLiveInject()
        {
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
    }
}
