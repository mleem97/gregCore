using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using greg.Logging;

namespace greg.CableRemoval
{
    public class Main : MelonMod
    {
        private GregModLogger _log = null!;
        private bool _enabled = true;
        
        private const float WorldPurgeHoldSeconds = 10f;
        private bool _showMassRemoveHint;
        private bool _showWorldPurgeHint;
        private bool _charging;
        private float _chargeElapsed;
        private float _chargeHoldSeconds = 0.85f;
        private NetworkSwitch? _chargeSwitch;
        private PatchPanel? _chargePanel;
        private float _worldPurgeElapsed;
        private GameObject? _hintPanel;
        private UnityEngine.UI.Text? _hintText;

        public override void OnInitializeMelon()
        {
            if (gregCore.Core.GregCoreMod.Instance == null)
            {
                LoggerInstance.Warning("[gC-CableRemoval] gregCore not ready.");
                return;
            }

            _log = new GregModLogger("CableRemoval");
            RegisterSettings();
            _log.FeatureState("CableRemoval", true);
            _log.Msg("Initialization complete.");
        }

        private void BuildUI()
        {
            if (_hintPanel != null) return;

            var builder = gregCore.UI.GregUIBuilder.Create("CableRemovalHint")
                .SetSize(600, 80);
            
            _hintPanel = builder.Build();
            _hintText = _hintPanel.GetComponentInChildren<UnityEngine.UI.Text>();

            var rt = _hintPanel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 50);

            _hintPanel.SetActive(false);
        }

        private void RegisterSettings()
        {
            string modId = "cable_removal";
            gregCore.API.GregAPI.RegisterMod(modId, "Mass Cable Removal", "1.0.0");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "enable_mass_removal", "Enable Mass Cable Removal", true, val => _enabled = val, "Maintenance", "Allows mass removing cables by holding a button (default: Left Alt + Click).");
        }

        public override void OnUpdate()
        {
            if (!_enabled) return;
            if (_hintPanel == null) BuildUI();

            _showMassRemoveHint = false;
            _showWorldPurgeHint = false;
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            // Simplified default binding: Left Alt + Left Click
            if (kb == null || mouse == null)
            {
                CancelCharge();
                CancelWorldPurge();
                UpdateHintUI();
                return;
            }

            bool isAimHeld = kb.leftAltKey.isPressed;
            bool isChargePressed = isAimHeld && mouse.leftButton.isPressed;
            bool chargePressedThisFrame = isAimHeld && mouse.leftButton.wasPressedThisFrame;

            if (!isAimHeld)
            {
                CancelCharge();
                CancelWorldPurge();
                UpdateHintUI();
                return;
            }

            if (TryGetLookedAtCableDevice(out var sw, out var panel))
            {
                CancelWorldPurge();
                UpdateDeviceChargeFlow(isChargePressed, chargePressedThisFrame, sw, panel);
                UpdateHintUI();
                return;
            }

            CancelCharge();
            if (!isChargePressed)
            {
                CancelWorldPurge();
                UpdateHintUI();
                return;
            }

            _worldPurgeElapsed += Time.deltaTime;
            _showWorldPurgeHint = true;

            if (_worldPurgeElapsed < WorldPurgeHoldSeconds)
            {
                UpdateHintUI();
                return;
            }

            TryDisconnectAllInWorld();
            CancelWorldPurge();
            UpdateHintUI();
        }

        private void UpdateHintUI()
        {
            if (_hintPanel == null || _hintText == null) return;

            if (_showWorldPurgeHint)
            {
                _hintText.text = $"L-ALT: WORLD purge — NOT looking at a device.\nKeep holding L-CLICK for {WorldPurgeHoldSeconds - _worldPurgeElapsed:0}s to remove ALL cables everywhere.";
                _hintPanel.SetActive(true);
            }
            else if (_showMassRemoveHint)
            {
                _hintText.text = _charging ? $"L-ALT: Removing ALL cables — keep holding L-CLICK." : $"L-ALT: Hold L-CLICK to remove ALL cables from this device.";
                _hintPanel.SetActive(true);
            }
            else
            {
                _hintPanel.SetActive(false);
            }
        }

        private void UpdateDeviceChargeFlow(bool isChargePressed, bool chargePressedThisFrame, NetworkSwitch? sw, PatchPanel? panel)
        {
            if (_charging && !IsSameChargeTarget(sw, panel))
                CancelCharge();

            _showMassRemoveHint = true;

            if (!_charging)
            {
                if (!chargePressedThisFrame) return;

                if (sw != null)
                {
                    _chargeSwitch = sw;
                    _chargePanel = null;
                    _chargeHoldSeconds = 0.85f;
                }
                else
                {
                    _chargePanel = panel;
                    _chargeSwitch = null;
                    _chargeHoldSeconds = 0.85f;
                }

                _chargeElapsed = 0f;
                _charging = true;
                return;
            }

            if (!isChargePressed)
            {
                CancelCharge();
                return;
            }

            _chargeElapsed += Time.deltaTime;
            if (_chargeElapsed < _chargeHoldSeconds) return;

            var s = _chargeSwitch;
            var p = _chargePanel;
            CancelCharge();

            if (s != null) TryDisconnectOnNetworkSwitch(s);
            else if (p != null) TryDisconnectOnPatchPanel(p);
        }

        private bool IsSameChargeTarget(NetworkSwitch? sw, PatchPanel? panel)
        {
            if (_chargeSwitch != null) return sw == _chargeSwitch;
            if (_chargePanel != null) return panel == _chargePanel;
            return false;
        }

        private void CancelCharge() { _charging = false; _chargeElapsed = 0f; _chargeSwitch = null; _chargePanel = null; }
        private void CancelWorldPurge() { _worldPurgeElapsed = 0f; }

        public override void OnGUI()
        {
            // IMGUI disabled
        }

        private bool TryGetLookedAtCableDevice(out NetworkSwitch? sw, out PatchPanel? panel)
        {
            sw = null; panel = null;
            var cam = Camera.main;
            if (cam == null) return false;

            var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, 14f))
            {
                sw = hit.collider.GetComponentInParent<NetworkSwitch>();
                if (sw != null) return true;
                panel = hit.collider.GetComponentInParent<PatchPanel>();
                return panel != null;
            }
            return false;
        }

        private void TryDisconnectOnNetworkSwitch(NetworkSwitch sw)
        {
            try { sw.DisconnectCables(); } catch { }
        }

        private void TryDisconnectOnPatchPanel(PatchPanel p)
        {
            try 
            { 
                if (p.cableLinkPorts == null) return;
                foreach (var port in p.cableLinkPorts)
                {
                    if (port != null)
                    {
                        port.SecondActionOnClick();
                    }
                }
            } 
            catch { }
        }

        private void TryDisconnectAllInWorld()
        {
            foreach (var sw in Resources.FindObjectsOfTypeAll<NetworkSwitch>()) 
                if (sw.gameObject.scene.isLoaded) TryDisconnectOnNetworkSwitch(sw);
            
            foreach (var p in Resources.FindObjectsOfTypeAll<PatchPanel>()) 
                if (p.gameObject.scene.isLoaded) TryDisconnectOnPatchPanel(p);
                
            _log.Msg("World purge finished.");
        }
    }
}