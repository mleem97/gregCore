using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Il2Cpp;

namespace greg.CommonShop
{
    public static partial class ShopAPI
    {
        private static void ClaimCustomId(CustomShopItem item)
        {
            if (IsVanillaTemplateType(item)) return;
            int customId = (int)item.TemplateType;
            string modName = CallingModName();
            if (TryValidateExistingClaim(customId, modName, item)) return;
            RegisterNewClaim(customId, modName, item);
        }

        private static bool IsVanillaTemplateType(CustomShopItem item)
        {
            try { return Enum.IsDefined(typeof(PlayerManager.ObjectInHand), item.TemplateType); }
            catch { return true; }
        }

        private static bool TryValidateExistingClaim(int customId, string modName, CustomShopItem item)
        {
            try
            {
                if (!_usedCustomIDs.TryGetValue(customId, out var existingEntry))
                    return false;
                string existingMod = GetClaimField(existingEntry, "ModName", "Unknown Mod");
                string existingItem = GetClaimField(existingEntry, "ItemName", "Unknown Item");
                if (!string.Equals(existingMod, modName, StringComparison.Ordinal))
                {
                    _log.Error($"FATAL ID COLLISION! Mod '{modName}' tried to register custom ID [{customId}].");
                    _log.Error($"-> That ID is registered to '{existingMod}' for '{existingItem}'. Skipping item to prevent corruption.");
                    throw new InvalidOperationException($"Custom ID {customId} already claimed by '{existingMod}'.");
                }
                return true;
            }
            catch (InvalidOperationException) { throw; }
            catch { return false; }
        }

        private static string GetClaimField(Dictionary<string, string> entry, string key, string fallback)
        {
            try
            {
                if (entry != null && entry.TryGetValue(key, out var v)) return v;
            }
            catch { }
            return fallback;
        }

        private static void RegisterNewClaim(int customId, string modName, CustomShopItem item)
        {
            try
            {
                _usedCustomIDs[customId] = new Dictionary<string, string>
                {
                    { "ModName", modName },
                    { "ItemName", item.Name }
                };
                SaveIDRegistry();
                _log.Msg($"Registered new custom ID [{customId}] to mod '{modName}' for '{item.Name}'.");
            }
            catch { }
        }

        private static void LoadIDRegistry()
        {
            _usedCustomIDs.Clear();
            string path = RegistryFilePath;
            try
            {
                if (!File.Exists(path) && !TryMigrateLegacyRegistry(path))
                    return;
                if (!File.Exists(path)) return;
                string jsonString = File.ReadAllText(path);
                if (TryLoadCurrentFormat(jsonString)) return;
                TryLoadLegacyFormat(jsonString);
            }
            catch { }
        }

        private static bool TryMigrateLegacyRegistry(string path)
        {
            try
            {
                if (!File.Exists(LegacyRegistryFilePath)) return false;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    File.Copy(LegacyRegistryFilePath, path, overwrite: false);
                }
                catch { }
                return true;
            }
            catch { return false; }
        }

        private static bool TryLoadCurrentFormat(string jsonString)
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<Dictionary<int, Dictionary<string, string>>>(jsonString);
                if (loaded != null) _usedCustomIDs = loaded;
                _log.Msg($"Loaded {_usedCustomIDs.Count} claimed custom IDs from registry.");
                return true;
            }
            catch { return false; }
        }

        private static void TryLoadLegacyFormat(string jsonString)
        {
            try
            {
                var legacy = JsonSerializer.Deserialize<Dictionary<int, string>>(jsonString);
                if (legacy == null) return;
                foreach (var kvp in legacy)
                    _usedCustomIDs[kvp.Key] = BuildLegacyEntry(kvp.Value);
                SaveIDRegistry();
                _log.Msg("Upgraded legacy ID registry format.");
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to load ID registry: {ex.GetBaseException().Message}");
                _usedCustomIDs = new Dictionary<int, Dictionary<string, string>>();
            }
        }

        private static Dictionary<string, string> BuildLegacyEntry(string modName)
        {
            return new Dictionary<string, string>
            {
                { "ModName", modName },
                { "ItemName", "Unknown (Legacy Format)" }
            };
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
