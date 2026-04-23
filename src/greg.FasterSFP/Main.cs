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
        [HarmonyPatch(typeof(global::Il2Cpp.MainGameManager), nameof(global::Il2Cpp.MainGameManager.Awake))]
        [HarmonyPostfix]
        public static void SetupRegistry(global::Il2Cpp.MainGameManager __instance)
        {
            // Expand sfpPrefabs to hold our new modules
            if (__instance.sfpPrefabs == null) return;
            
            int maxId = 106;
            if (__instance.sfpPrefabs.Length <= maxId)
            {
                var newArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject>(maxId + 1);
                for (int i = 0; i < __instance.sfpPrefabs.Length; i++) newArr[i] = __instance.sfpPrefabs[i];
                
                // Clone vanilla SFP for each new type
                var basePrefab = __instance.sfpPrefabs[0]; 
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

        [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.GetPrefabForItem))]
        [HarmonyPrefix]
        public static bool GetPrefabForItemPatch(int itemID, PlayerManager.ObjectInHand itemType, ref GameObject __result)
        {
            var mgm = MainGameManager.instance;
            if (mgm == null || mgm.sfpPrefabs == null) return true;

            if (itemType == PlayerManager.ObjectInHand.SFPModule && itemID >= 100 && itemID <= 106)
            {
                if (itemID < mgm.sfpPrefabs.Length && mgm.sfpPrefabs[itemID] != null)
                {
                    __result = mgm.sfpPrefabs[itemID];
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(global::Il2Cpp.CableLink), nameof(global::Il2Cpp.CableLink.InsertSFP))]
        [HarmonyPrefix]
        public static void InsertSFPPatch(float speed, SFPModule module)
        {
            var usableObj = module?.GetComponent<UsableObject>();
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
    }
}
