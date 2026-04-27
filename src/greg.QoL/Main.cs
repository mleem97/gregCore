using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using greg.Logging;

namespace greg.QoL
{
    public class Main : MelonMod
    {
        private bool _hasFixedLayout = false;
        private GregModLogger _log = null!;
        private bool _shopGridFixEnabled = true;
        private bool _deleteHeldItemEnabled = true;
        private bool _trashCleanerEnabled = true;
        private float _cableSpoolLengthThreshold = 1.5f;
        private float _shopGridCheckTimer = ShopGridCheckInterval;
        private const float ShopGridCheckInterval = 1.0f;

        public override void OnInitializeMelon()
        {
            if (gregCore.Core.GregCoreMod.Instance == null)
            {
                LoggerInstance.Warning("[gC-QoL] gregCore not ready.");
                return;
            }

            _log = new GregModLogger("QoL");
            
            RegisterSettings();

            _log.FeatureState("QoL", true);
            _log.Msg("Initialization complete.");
        }

        private void RegisterSettings()
        {
            string modId = "qol";
            gregCore.API.GregAPI.RegisterMod(modId, "Quality of Life", "1.0.0");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "shop_grid_fix", "Shop Grid Fix", true, val => _shopGridFixEnabled = val, "QoL", "Fixes the mod shop layout by converting it to a grid.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "delete_held_item", "Delete Held Item (E at Dumpster)", true, val => _deleteHeldItemEnabled = val, "QoL", "Allows deleting modded items in your hand by pressing E while looking at the dumpster.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "trash_cleaner", "Enable Trash Cleaner (F9)", true, val => _trashCleanerEnabled = val, "QoL", "Allows cleaning empty boxes and short cable spools manually by pressing F9.");
            gregCore.API.GregAPI.Settings.RegisterSlider(modId, "trash_cleaner_spool_threshold", "Cable Spool Length Threshold", 0f, 10f, 1.5f, val => _cableSpoolLengthThreshold = val, "QoL", "Max length of cable spools to consider as trash.");
        }

        public override void OnUpdate()
        {
            if (_shopGridFixEnabled && !_hasFixedLayout)
            {
                _shopGridCheckTimer += Time.deltaTime;
                if (_shopGridCheckTimer >= ShopGridCheckInterval)
                {
                    _shopGridCheckTimer = 0f;
                    foreach (HorizontalLayoutGroup hGroup in UnityEngine.Object.FindObjectsOfType<HorizontalLayoutGroup>())
                    {
                        if (hGroup.gameObject.name == "HL Mods")
                        {
                            _log.Msg($"Found HL Mods with {hGroup.transform.childCount} children - applying grid fix...");
                            ConvertToGrid(hGroup);
                            break;
                        }
                    }
                }
            }

            if (_deleteHeldItemEnabled)
            {
                var kb = Keyboard.current;
                if (kb != null && kb.eKey.wasPressedThisFrame)
                {
                    TryDumpModdedItem();
                }
            }

            if (_trashCleanerEnabled)
            {
                var kb = Keyboard.current;
                if (kb != null && kb.f9Key.wasPressedThisFrame)
                {
                    int removedBoxes = RemoveEmptySfpBoxes();
                    int removedSpools = RemoveEmptyCableSpools();
                    gregCore.API.GregAPI.ShowNotification($"Trash Cleaned: {removedBoxes} boxes, {removedSpools} spools.");
                    _log.Msg($"Trash Cleaned: {removedBoxes} boxes, {removedSpools} spools.");
                }
            }
        }

        private int RemoveEmptySfpBoxes()
        {
            int count = 0;
            foreach (var box in UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.SFPBox>())
            {
                if (box == null || box.objectInHands || box.isOnTrolley || box.currentRackPosition != null) continue;
                
                bool isEmpty = true;
                if (box.usedPositions != null)
                {
                    foreach (var pos in box.usedPositions) if (pos != 0) { isEmpty = false; break; }
                }
                
                if (isEmpty)
                {
                    foreach (var module in box.GetComponentsInChildren<global::Il2Cpp.SFPModule>(true))
                    {
                        if (module != null && module.isInTheBox) { isEmpty = false; break; }
                    }
                }

                if (isEmpty)
                {
                    UnityEngine.Object.Destroy(box.gameObject);
                    count++;
                }
            }
            return count;
        }

        private int RemoveEmptyCableSpools()
        {
            int count = 0;
            foreach (var spool in UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.CableSpinner>())
            {
                if (spool == null || spool.objectInHands || spool.cableLenght > _cableSpoolLengthThreshold) continue;
                UnityEngine.Object.Destroy(spool.gameObject);
                count++;
            }
            return count;
        }

        private void TryDumpModdedItem()
        {
            GameObject gameObject = GameObject.Find("Player/vCam/holdingPos/holdingPosChangedFromObject");
            if (gameObject == null || gameObject.transform.childCount == 0)
                return;
                
            Transform child = gameObject.transform.GetChild(0);
            bool isUsable = false;
            bool isHardware = false;
            
            foreach (Component component in child.GetComponents<Component>())
            {
                if (component != null)
                {
                    try
                    {
                        string name = component.GetIl2CppType().Name;
                        if (name == "UsableObject") isUsable = true;
                        if (name == "Server" || name == "NetworkSwitch" || name == "PatchPanel" || name == "Rack") isHardware = true;
                    }
                    catch { }
                }
            }
            
            if (!isUsable && !isHardware) return;

            Camera main = Camera.main;
            if (main == null) return;
            
            RaycastHit hitInfo;
            if (!Physics.Raycast(main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0.0f)), out hitInfo, 3f))
                return;
                
            if (hitInfo.collider == null || hitInfo.collider.gameObject.name != "Dumpster_body")
                return;

            _log.Msg("Deleting modded item: " + child.name);
            UnityEngine.Object.Destroy(child.gameObject);
        }

        private void ConvertToGrid(HorizontalLayoutGroup hGroup)
        {
            GameObject gameObject = hGroup.gameObject;
            UnityEngine.Object.DestroyImmediate(hGroup);
            
            GridLayoutGroup gridLayoutGroup = gameObject.AddComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                _log.Error("Failed to add GridLayoutGroup");
                return;
            }

            gridLayoutGroup.cellSize = new Vector2(150f, 150f);
            gridLayoutGroup.spacing = new Vector2(10f, 10f);
            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 5;
            
            try
            {
                ContentSizeFitter contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
                if (contentSizeFitter != null)
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
            }
            catch
            {
                _log.Msg("ContentSizeFitter skipped");
            }
            
            _hasFixedLayout = true;
            _log.Msg($"Grid fix applied! {gameObject.transform.childCount} items arranged in rows of 5.");
        }
    }
}
