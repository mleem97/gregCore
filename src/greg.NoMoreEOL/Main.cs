using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Il2Cpp;
using Il2CppSystem.Linq;
using greg.Logging;

namespace greg.NoMoreEOL
{
    public class Main : MelonMod
    {
        private GregModLogger _log = null!;

        // Options
        private bool _autoRepairBrokenSwitches = true;
        private bool _autoRepairBrokenServers = true;
        private bool _disableSwitchesEOL = true;
        private bool _disableServersEOL = true;
        internal static bool WarningsVisible = true;

        // Internal State
        internal static readonly HashSet<PositionIndicator> Indicators = new();
        private bool _readyToRun;
        private NetworkMap? _networkMap;
        private MainGameManager? _gameManager;
        private float _maintenanceTimer;

        private Dictionary<int, int> _switchTypeDefaultEOL = new Dictionary<int, int>();
        private Dictionary<int, int> _serverTypeDefaultEOL = new Dictionary<int, int>();
        private const int DefaultEOL = 14401;

        public override void OnInitializeMelon()
        {
            if (gregCore.Core.GregCoreMod.Instance == null)
            {
                LoggerInstance.Warning("[gC-NoMoreEOL] gregCore not ready.");
                return;
            }

            _log = new GregModLogger("NoMoreEOL");
            _log.Section("Init");
            _log.Msg("Starting initialization.");

            RegisterSettings();

            _log.FeatureState("NoMoreEOL", true);
            _log.Msg("Initialization complete.");
        }

        private void RegisterSettings()
        {
            string modId = "nomore_eol";

            gregCore.API.GregAPI.RegisterMod(modId, "NoMoreEOL", "1.0.0");

            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "auto_repair_switches", "Auto Repair Switches", true, val => _autoRepairBrokenSwitches = val, "Maintenance", "Automatically repairs broken network switches.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "auto_repair_servers", "Auto Repair Servers", true, val => _autoRepairBrokenServers = val, "Maintenance", "Automatically repairs broken servers.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "disable_switches_eol", "Disable Switches EOL", true, val => _disableSwitchesEOL = val, "Maintenance", "Prevents switches from reaching End-Of-Life.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "disable_servers_eol", "Disable Servers EOL", true, val => _disableServersEOL = val, "Maintenance", "Prevents servers from reaching End-Of-Life.");
            gregCore.API.GregAPI.Settings.RegisterToggle(modId, "show_warning_signs", "Show Warning Signs", true, val => 
            {
                WarningsVisible = val;
                UpdateWarningSignsVisibility();
            }, "Maintenance", "Show or hide EOL and error warning triangles above devices.");
        }

        private void UpdateWarningSignsVisibility()
        {
            Indicators.RemoveWhere(indicator => indicator == null || indicator.Pointer == IntPtr.Zero);

            foreach (var indicator in Indicators)
            {
                if (indicator.gameObject != null)
                {
                    indicator.gameObject.SetActive(WarningsVisible);
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Main Menu is 0, Base Scene is 1
            if (buildIndex == 0)
            {
                _readyToRun = false;
                _gameManager = null;
                _networkMap = null;
                Indicators.Clear();
            }
        }

        public override void OnUpdate()
        {
            if (_readyToRun && _networkMap != null)
            {
                _maintenanceTimer += Time.deltaTime;
                if (_maintenanceTimer >= 1.0f)
                {
                    _maintenanceTimer = 0f;
                    RepairSwitches();
                    RepairServers();
                    HandleSwitchesEOL();
                    HandleServersEOL();
                }
            }
            else
            {
                if (SceneManager.GetActiveScene().buildIndex != 1) return;

                try
                {
                    var map = NetworkMap.instance;
                    var gm = MainGameManager.instance;
                    
                    if (map == null || gm == null) return;

                    _networkMap = map;
                    _gameManager = gm;
                    _readyToRun = true;
                    _log.Msg("Attached to active game instance.");
                }
                catch (Exception ex)
                {
                    _log.Error($"OnUpdate Attach Error: {ex.Message}");
                }
            }
        }

        private void RepairSwitches()
        {
            if (!_autoRepairBrokenSwitches || _networkMap == null) return;

            // Optimization: Skip expensive GetAllBrokenSwitches() and defensive array allocation
            // if there are no broken switches to repair. Reduces GC pressure in OnUpdate.
            if (_networkMap.brokenSwitches == null || _networkMap.brokenSwitches.Count == 0) return;

            var broken = _networkMap.GetAllBrokenSwitches();
            if (broken != null)
            {
                foreach (var sw in broken.ToArray<NetworkSwitch>())
                {
                    if (sw != null) sw.RepairDevice();
                }
            }
        }

        private void RepairServers()
        {
            if (!_autoRepairBrokenServers || _networkMap == null) return;

            // Optimization: Skip expensive GetAllBrokenServers() and defensive array allocation
            // if there are no broken servers to repair. Reduces GC pressure in OnUpdate.
            if (_networkMap.brokenServers == null || _networkMap.brokenServers.Count == 0) return;

            var broken = _networkMap.GetAllBrokenServers();
            if (broken != null)
            {
                foreach (var srv in broken.ToArray<Server>())
                {
                    if (srv != null) srv.RepairDevice();
                }
            }
        }

        private void HandleSwitchesEOL()
        {
            if (!_disableSwitchesEOL || _networkMap == null) return;

            foreach (var kvp in _networkMap.switches)
            {
                var sw = kvp.Value;
                if (sw != null)
                {
                    sw.eolTime = GetEOLDeviceDefaultEOL(true, sw.switchType);
                }
            }
        }

        private void HandleServersEOL()
        {
            if (!_disableServersEOL || _networkMap == null) return;

            foreach (var kvp in _networkMap.servers)
            {
                var srv = kvp.Value;
                if (srv != null)
                {
                    srv.eolTime = GetEOLDeviceDefaultEOL(false, srv.serverType);
                }
            }
        }

        private int GetEOLDeviceDefaultEOL(bool isSwitch, int type)
        {
            var dict = isSwitch ? _switchTypeDefaultEOL : _serverTypeDefaultEOL;

            if (dict.TryGetValue(type, out int defaultEol))
                return defaultEol;

            if (!_readyToRun || _gameManager == null)
                return DefaultEOL;

            GameObject prefab = isSwitch ? _gameManager.GetSwitchPrefab(type) : _gameManager.GetServerPrefab(type);
            if (prefab == null) return DefaultEOL;

            int deviceDefaultEol = DefaultEOL;
            if (isSwitch)
            {
                var comp = prefab.GetComponent<NetworkSwitch>();
                if (comp != null) deviceDefaultEol = comp.eolTime;
            }
            else
            {
                var comp = prefab.GetComponent<Server>();
                if (comp != null) deviceDefaultEol = comp.eolTime;
            }

            if (deviceDefaultEol < int.MaxValue)
                deviceDefaultEol++;

            dict[type] = deviceDefaultEol;
            return deviceDefaultEol;
        }
    }
}
