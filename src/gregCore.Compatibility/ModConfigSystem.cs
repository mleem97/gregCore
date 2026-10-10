using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine.UIElements;

namespace DataCenterModLoader;

public static partial class ModConfigSystem
{
    private enum ConfigEntryType
    {
        Bool,
        Int,
        Float
    }

    private class ConfigEntry
    {
        public ConfigEntryType Type;
        public string Key = null!;
        public string DisplayName = null!;
        public string Description = null!;

        // Bool
        public bool BoolValue;
        public bool BoolDefault;

        // Int
        public int IntValue;
        public int IntDefault;
        public int IntMin;
        public int IntMax;

        // Float
        public float FloatValue;
        public float FloatDefault;
        public float FloatMin;
        public float FloatMax;
    }

    private class ModConfig
    {
        public string ModId = null!;
        public string Author = "";
        public string Version = "";
        public Dictionary<string, ConfigEntry> Entries = new();
        public List<string> EntryOrder = new();
    }

    private static MelonLogger.Instance _logger = null!;
    private static string _configDir = null!;
    private static readonly Dictionary<string, ModConfig> _mods = new();
    private static readonly List<string> _modOrder = new();
    private static bool _initialized;

    private static bool _showPanel = false;
    private static UnityEngine.UIElements.VisualElement? _panelRoot;
    private static string? _selectedModId;
    private static float _scrollOffset;

    private static UnityEngine.EventSystems.EventSystem? _disabledEventSystem;
    private static int _reenableEventSystemCountdown;

    private static Il2Cpp.MainMenu? _mainMenuRef;

    public static Transform? SettingsButtonTransform { get; private set; }

    private static bool _pendingSettingsIntercept;
    private static float _settingsInterceptTimer;
    private static bool _deferredOpenGameSettings;

    private static bool _pendingPauseMenuInject;
    private static float _pauseMenuInjectTimer;
    private static GameObject? _pauseMenuModButton;


    // UI Toolkit implementation - see ModConfigSystem.UI.cs

    public static bool IsPanelVisible => _showPanel;

    public static void Initialize(MelonLogger.Instance logger)
    {
        try
        {
            _logger = logger;
            _configDir = Path.Combine(MelonEnvironment.UserDataDirectory, "ModConfigs");

            if (!Directory.Exists(_configDir))
            {
                Directory.CreateDirectory(_configDir);
            }

            _initialized = true;
            CrashLog.Log("ModConfig: initialized, config dir = " + _configDir);
            _logger.Msg("[ModConfig] Configuration system initialized.");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.Initialize", ex);
        }
    }

    public static void OnUpdate(float dt)
    {
        TickSettingsIntercept(dt);
        TickPauseMenuInject(dt);
        TickReenableEventSystem();
        OpenDeferredGameSettings();
        HandlePanelHotkey();
    }

    private static void TickSettingsIntercept(float dt)
    {
        if (!_pendingSettingsIntercept) return;
        _settingsInterceptTimer -= dt;
        if (_settingsInterceptTimer > 0f) return;
        _pendingSettingsIntercept = false;
        InterceptSettingsButton();
    }

    private static void TickPauseMenuInject(float dt)
    {
        if (!_pendingPauseMenuInject) return;
        _pauseMenuInjectTimer -= dt;
        if (_pauseMenuInjectTimer > 0f) return;
        _pendingPauseMenuInject = false;
        InjectPauseMenuButton();
    }

    private static void TickReenableEventSystem()
    {
        // Wait N frames so the closing click doesn't pass through to the game
        if (_reenableEventSystemCountdown <= 0) return;
        _reenableEventSystemCountdown--;
        if (_reenableEventSystemCountdown > 0) return;
        try
        {
            if (_disabledEventSystem != null)
            {
                _disabledEventSystem.enabled = true;
                _disabledEventSystem = null;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void OpenDeferredGameSettings()
    {
        // Deferred from IMGUI - must run outside OnGUI
        if (!_deferredOpenGameSettings) return;
        _deferredOpenGameSettings = false;

        // Game settings need EventSystem immediately - cancel any deferred countdown
        _reenableEventSystemCountdown = 0;
        try
        {
            if (_disabledEventSystem != null)
            {
                _disabledEventSystem.enabled = true;
                _disabledEventSystem = null;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        try
        {
            if (_mainMenuRef != null)
            {
                CrashLog.Log("ModConfig: opening game settings (direct call, no Harmony).");
                _mainMenuRef.Settings();
            }
            else
            {
                CrashLog.Log("ModConfig: _mainMenuRef was null - cannot open game settings.");
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem deferred game settings open", ex);
        }
    }

    private static void HandlePanelHotkey()
    {
        // F8 hotkey to toggle panel, ESC to close (uses new Input System - legacy Input throws in this game)
        try
        {
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.f8Key.wasPressedThisFrame)
                {
                    if (_showPanel)
                        HidePanel();
                    else
                        ShowPanel();
                }
                else if (kb.escapeKey.wasPressedThisFrame)
                {
                    if (_showPanel)
                        HidePanel();
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.OnUpdate (hotkey)", ex);
        }
    }

    // Removed - IMGUI DrawGUI replaced by UI Toolkit in ModConfigSystem.UI.cs

    public static void OnSceneLoaded(string sceneName)
    {
        try
        {
            CrashLog.Log($"ModConfig: scene '{sceneName}' loaded.");

            if (sceneName == "MainMenu")
            {
                _pendingSettingsIntercept = true;
                _settingsInterceptTimer = 0.6f;
                _pauseMenuModButton = null;
                CrashLog.Log("ModConfig: will intercept Settings button in 0.6s.");
            }
            else
            {
                // Game scene - inject "Mod Settings" button into the pause menu
                _pendingPauseMenuInject = true;
                _pauseMenuInjectTimer = 1.0f;
                _pauseMenuModButton = null;
                CrashLog.Log("ModConfig: will inject pause-menu Mod Settings button in 1.0s.");
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.OnSceneLoaded", ex);
        }
    }

    public static void Shutdown()
    {
        try
        {
            foreach (var kvp in _mods)
            {
                SaveModConfig(kvp.Value);
            }
            CrashLog.Log("ModConfig: shutdown, all configs saved.");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.Shutdown", ex);
        }
    }

    // Removed - see ModConfigSystem.UI.cs for UI Toolkit version

    // Removed - see ModConfigSystem.UI.cs for UI Toolkit version

    public static void ShowSettingsChoice()
    {
        CrashLog.Log("ModConfig: settings choice redirected to ModConfig panel.");
        ShowPanel();
    }

    // Replaces the Harmony prefix approach which had static-field visibility issues in Il2Cpp Harmony.
    private static void InterceptSettingsButton()
    {
        try
        {
            var settingsBtn = FindSettingsButton();
            if (settingsBtn == null)
            {
                CrashLog.Log("ModConfig: could not find Settings button (no ButtonExtended with persistent 'Settings' listener).");
                return;
            }

            SettingsButtonTransform = settingsBtn;
            CaptureMainMenu();
            RewireSettingsButton(settingsBtn);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.InterceptSettingsButton", ex);
        }
    }

    private static Transform? FindSettingsButton()
    {
        var allButtons = Resources.FindObjectsOfTypeAll<ButtonExtended>();
        if (allButtons == null) return null;
        foreach (var btn in allButtons)
        {
            try
            {
                var onClick = btn.onClick;
                if (onClick == null) continue;
                int count = onClick.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    if (onClick.GetPersistentMethodName(i) == "Settings")
                        return btn.transform;
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return null;
    }

    private static void CaptureMainMenu()
    {
        var menus = Resources.FindObjectsOfTypeAll<Il2Cpp.MainMenu>();
        if (menus != null && menus.Count > 0)
        {
            _mainMenuRef = menus[0];
            CrashLog.Log("ModConfig: captured MainMenu reference.");
        }
        else
        {
            CrashLog.Log("ModConfig: WARNING - no MainMenu instance found.");
        }
    }

    private static void RewireSettingsButton(Transform settingsBtn)
    {
        var btnExt = settingsBtn.GetComponent<ButtonExtended>();
        if (btnExt != null)
        {
            btnExt.onClick = new ButtonExtended.ButtonClickedEvent();
            btnExt.onClick.AddListener((System.Action)(() => ShowSettingsChoice()));
            CrashLog.Log("ModConfig: Settings button onClick replaced (ButtonExtended).");
            return;
        }

        // Fallback to standard Unity Button
        var btn = settingsBtn.GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            btn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            btn.onClick.AddListener((System.Action)(() => ShowSettingsChoice()));
            CrashLog.Log("ModConfig: Settings button onClick replaced (Button fallback).");
        }
        else
        {
            CrashLog.Log("ModConfig: no ButtonExtended or Button found on Settings button.");
        }
    }

    // Uses component hierarchy - LocalisedText hasn't set button labels yet when UI is inactive.
    private static void InjectPauseMenuButton()
    {
        try
        {
            if (_pauseMenuModButton != null) return;

            var pmUI = FindPauseMenuUI();
            if (pmUI == null) return;

            var actionButtons = CollectActionButtons(pmUI);
            if (actionButtons == null) return;

            var (templateBtn, buttonPanel) = FindButtonPanel(actionButtons);
            if (templateBtn == null || buttonPanel == null)
            {
                CrashLog.Log("ModConfig: button panel is null - cannot inject.");
                return;
            }

            var clone = ClonePauseMenuButton(templateBtn, buttonPanel);
            WirePauseMenuButton(clone);

            _pauseMenuModButton = clone;
            CrashLog.Log("ModConfig: Mod Settings button injected into pause menu.");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.InjectPauseMenuButton", ex);
        }
    }

    private static Transform? FindPauseMenuUI()
    {
        var pauseMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
        if (pauseMenus == null || pauseMenus.Count == 0)
        {
            CrashLog.Log("ModConfig: no PauseMenu instance found - cannot inject button.");
            return null;
        }

        var pauseMenu = pauseMenus[0];
        var pmUI = pauseMenu.pauseMenuUI;
        if (pmUI == null)
        {
            CrashLog.Log("ModConfig: PauseMenu.pauseMenuUI is null.");
            return null;
        }

        CrashLog.Log($"ModConfig: found PauseMenu, pauseMenuUI = '{pmUI.name}'.");
        return pmUI.transform;
    }

    private static System.Collections.Generic.List<ButtonExtended> CollectActionButtons(Transform pmUI)
    {
        // The pause menu has two kinds of ButtonExtended:
        //   - Settings tab buttons (System, Audio, etc.) which ALSO have PauseMenu_TabButton
        //   - Action buttons (Resume, Save Game, etc.) which do NOT have PauseMenu_TabButton
        // We only want the action buttons.
        var allButtons = pmUI.GetComponentsInChildren<ButtonExtended>(true);
        if (allButtons == null || allButtons.Count == 0)
        {
            CrashLog.Log("ModConfig: no ButtonExtended found in pauseMenuUI.");
            return null;
        }

        // Filter out tab buttons - keep only pure action buttons
        var actionButtons = new System.Collections.Generic.List<ButtonExtended>();
        foreach (var be in allButtons)
        {
            if (be.GetComponent<PauseMenu_TabButton>() == null)
                actionButtons.Add(be);
        }

        CrashLog.Log($"ModConfig: found {allButtons.Count} ButtonExtended(s), {actionButtons.Count} are action buttons (non-tab).");

        if (actionButtons.Count == 0)
        {
            CrashLog.Log("ModConfig: no action buttons found in pause menu.");
            return null;
        }
        return actionButtons;
    }

    private static (Transform? template, Transform? panel) FindButtonPanel(
        System.Collections.Generic.List<ButtonExtended> actionButtons)
    {
        Transform? templateBtn = null;
        Transform? buttonPanel = null;

        foreach (var be in actionButtons)
        {
            var parent = be.transform.parent;
            if (parent == null) continue;

            var panelActionBtns = new System.Collections.Generic.List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                var childBtn = child.GetComponent<ButtonExtended>();
                if (childBtn != null && child.GetComponent<PauseMenu_TabButton>() == null)
                    panelActionBtns.Add(child);
            }

            if (panelActionBtns.Count >= 4)
            {
                templateBtn = panelActionBtns.Count > 1 ? panelActionBtns[1] : panelActionBtns[0];
                buttonPanel = parent;
                CrashLog.Log($"ModConfig: using action button panel '{parent.name}' ({panelActionBtns.Count} action buttons), template='{templateBtn.name}'.");
                break;
            }
        }

        if (templateBtn == null || buttonPanel == null)
        {
            templateBtn = actionButtons[0].transform;
            buttonPanel = templateBtn.parent;
            CrashLog.Log($"ModConfig: fallback - using first action button in '{buttonPanel?.name}'.");
        }
        return (templateBtn, buttonPanel);
    }

    private static UnityEngine.GameObject ClonePauseMenuButton(Transform templateBtn, Transform buttonPanel)
    {
        var clone = UnityEngine.Object.Instantiate(templateBtn.gameObject, buttonPanel);
        clone.name = "ModSettingsButton";

        // Place before "Quit to desktop"
        int childCount = buttonPanel.childCount;
        if (childCount >= 3)
            clone.transform.SetSiblingIndex(childCount - 3);

        // Destroy LocalisedText so our label sticks
        var locTexts = clone.GetComponentsInChildren<LocalisedText>(true);
        if (locTexts != null)
        {
            foreach (var lt in locTexts)
                UnityEngine.Object.Destroy(lt);
        }

        var cloneTexts = clone.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true);
        if (cloneTexts != null)
        {
            foreach (var t in cloneTexts)
            {
                t.text = "Mod Settings";
                try { t.SetText("Mod Settings"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { t.ForceMeshUpdate(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        return clone;
    }

    private static void WirePauseMenuButton(UnityEngine.GameObject clone)
    {
        var btnExt = clone.GetComponent<ButtonExtended>();
        if (btnExt != null)
        {
            btnExt.onClick = new ButtonExtended.ButtonClickedEvent();
            btnExt.onClick.AddListener((System.Action)(() => ShowPanel()));
            CrashLog.Log("ModConfig: pause menu Mod Settings wired (ButtonExtended).");
            return;
        }

        var btn = clone.GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            btn.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            btn.onClick.AddListener((System.Action)(() => ShowPanel()));
            CrashLog.Log("ModConfig: pause menu Mod Settings wired (Button fallback).");
        }
        else
        {
            CrashLog.Log("ModConfig: no ButtonExtended or Button on cloned pause menu button.");
        }
    }

    // Removed - IMGUI methods replaced by UI Toolkit in ModConfigSystem.UI.cs

}
