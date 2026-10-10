using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using greg.Logging;

namespace greg.FasterSFP
{
    public class ModuleDef
    {
        public string Name;
        public int SpeedGbps;
        public float SpeedInternal => SpeedGbps / 5f;
        public int Price;
        public int ResultID;
        
        public ModuleDef(string name, int speed, int price, int id)
        {
            Name = name; SpeedGbps = speed; Price = price; ResultID = id;
        }
    }

    public class Main : MelonMod
    {
        private GregModLogger _log = null!;
        private bool _enabled = true;
        
        public static List<ModuleDef> Modules = new()
        {
            new ModuleDef("QSFP28 100Gbps", 100, 1000, 100),
            new ModuleDef("QSFP56 200Gbps", 200, 2500, 101),
            new ModuleDef("QSFP-DD 400Gbps", 400, 6000, 102),
            new ModuleDef("QSFP-DD 800Gbps", 800, 12000, 103),
            new ModuleDef("QSFP-DWDM 1.6Tbps", 1600, 25000, 104),
            new ModuleDef("QSFP-DWDM 3.2Tbps", 3200, 50000, 105),
            new ModuleDef("QSFP-DWDM 6.4Tbps", 6400, 100000, 106)
        };

        public override void OnInitializeMelon()
        {
            if (gregCore.Core.GregCoreMod.Instance == null) return;
            _log = new GregModLogger("FasterSFP");
            
            string modId = "faster_sfp";
            gregCore.API.GregAPI.RegisterMod(modId, "Faster SFP Modules", "1.0.0");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "enable_faster_sfp", "Enable Faster SFP Modules", true, val => _enabled = val, "Hardware", "Adds 100Gbps to 6.4Tbps SFP modules to the shop.");

            RegisterShopItems();
            
            _log.FeatureState("FasterSFP", true);
        }

        private void RegisterShopItems()
        {
            foreach (var mod in Modules)
            {
                var item = new greg.CommonShop.CustomShopItem
                {
                    Name = mod.Name,
                    Price = mod.Price,
                    TemplateType = PlayerManager.ObjectInHand.SFPModule,
                    TemplateID = 0, // Vanilla QSFP+ is usually index 0
                    ResultItemID = mod.ResultID,
                    Category = "Hardware",
                    SubCategory = "SFP Modules",
                    OnUIReady = (go) => { }, // Visuals could be set here
                    OnCheckout = (qty) => 
                    {
                        // Logic to give the player the custom SFP module
                        // The actual prefab injection happens via Harmony patches 
                        // so the shop naturally dispenses them if the shop ID matches.
                    }
                };
                greg.CommonShop.ShopAPI.RegisterItem(item);
            }
        }
    }

    [HarmonyPatch]
    public static class SFPPatch
    {
        // gregMod.MoreModules / gregMod.RealisticModules own the same speed
        // tiers (100G-6.4T) with their own prefab IDs. Relabeling their
        // modules to 100-106 at insert time made the save restore them from
        // this module's templates instead of theirs, so stand down entirely
        // for insert relabeling and shop routing when either is loaded.
        private static bool? _siblingOwnsModules;
        internal static bool SiblingOwnsModules
        {
            get
            {
                if (_siblingOwnsModules.HasValue) return _siblingOwnsModules.Value;
                bool found = false;
                try
                {
                    foreach (var m in MelonLoader.MelonMod.RegisteredMelons)
                    {
                        string n = m?.Info?.Name;
                        if (n == "gregMod.MoreModules" || n == "gregMod.RealisticModules") { found = true; break; }
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) - melon list unreadable, assume no sibling */ }
                _siblingOwnsModules = found;
                if (found) MelonLogger.Msg("[FasterSFP] gregMod.MoreModules/RealisticModules detected — insert relabeling and shop routing disabled.");
                return found;
            }
        }

        // Template source: the vanilla QSFP+ module (name SFP_QSFP, normally
        // index 3). Index 0 is SFP_RJ45 — cloning that produced copper (sfpType
        // 0) templates that load can't seat in a fiber QSFP port.
        private static GameObject FindQsfpBase(global::Il2Cpp.MainGameManager mgm)
        {
            var arr = mgm.sfpPrefabs;
            for (int i = 0; i < arr.Length && i < 100; i++)
            {
                var go = arr[i];
                if (go == null) continue;
                if (go.name == "SFP_QSFP") return go;
            }
            for (int i = 0; i < arr.Length && i < 100; i++)
            {
                var go = arr[i];
                var s = go != null ? go.GetComponent<SFPModule>() : null;
                if (s != null && s.sfpType == 3) return go;
            }
            return arr.Length > 0 ? arr[0] : null;
        }
        [HarmonyPatch(typeof(global::Il2Cpp.MainGameManager), nameof(global::Il2Cpp.MainGameManager.Awake))]
        [HarmonyPostfix]
        public static void SetupRegistry(global::Il2Cpp.MainGameManager __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == System.IntPtr.Zero) return;
                if (__instance.sfpPrefabs == null) return;
                
                int maxId = 106;
                if (__instance.sfpPrefabs.Length <= maxId)
                {
                    var newArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject>(maxId + 1);
                    for (int i = 0; i < __instance.sfpPrefabs.Length; i++) newArr[i] = __instance.sfpPrefabs[i];
                    
                    var basePrefab = FindQsfpBase(__instance);
                    if (basePrefab != null)
                    {
                        foreach (var mod in Main.Modules)
                        {
                            var clone = UnityEngine.Object.Instantiate(basePrefab);
                            clone.name = "CustomSFP_" + mod.Name;
                            clone.SetActive(false);
                            UnityEngine.Object.DontDestroyOnLoad(clone);
                            
                            var comp = clone.GetComponent<SFPModule>();
                            if (comp != null) comp.speed = mod.SpeedInternal;
                            
                            var usable = clone.GetComponent<UsableObject>();
                            if (usable != null) usable.prefabID = mod.ResultID;

                            newArr[mod.ResultID] = clone;
                        }
                    }
                    
                    __instance.sfpPrefabs = newArr;
                    greg.Logging.GregLogger.Msg("FasterSFP modules injected into MainGameManager.", "FasterSFP");
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[FasterSFP] SetupRegistry failed: {ex.Message}");
            }
        }

        [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.GetPrefabForItem))]
        [HarmonyPrefix]
        public static bool GetPrefabForItemPatch(int itemID, PlayerManager.ObjectInHand itemType, ref GameObject __result)
        {
            try
            {
                if (SiblingOwnsModules) return true;
                var mgm = MainGameManager.instance;
                if (mgm == null || mgm.Pointer == System.IntPtr.Zero || mgm.sfpPrefabs == null) return true;

                if (itemType == PlayerManager.ObjectInHand.SFPModule && itemID >= 100 && itemID <= 106)
                {
                    if (itemID < mgm.sfpPrefabs.Length && mgm.sfpPrefabs[itemID] != null)
                    {
                        __result = mgm.sfpPrefabs[itemID];
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[FasterSFP] GetPrefabForItemPatch failed: {ex.Message}");
            }
            return true;
        }

        [HarmonyPatch(typeof(global::Il2Cpp.CableLink), nameof(global::Il2Cpp.CableLink.InsertSFP))]
        [HarmonyPrefix]
        public static void InsertSFPPatch(float speed, int type, SFPModule module)
        {
            try
            {
                if (SiblingOwnsModules) return;
                if (module == null || module.Pointer == System.IntPtr.Zero) return;

                var usableObj = module.GetComponent<UsableObject>();
                if (usableObj == null) return;
                
                foreach (var def in Main.Modules)
                {
                    if (Mathf.Approximately(speed, def.SpeedInternal))
                    {
                        usableObj.prefabID = def.ResultID;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"[FasterSFP] InsertSFPPatch failed: {ex.Message}");
            }
        }
    }
}
