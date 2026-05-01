using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private static bool _showPanel;
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
        if (_pendingSettingsIntercept)
        {
            _settingsInterceptTimer -= dt;
            if (_settingsInterceptTimer <= 0f)
            {
                _pendingSettingsIntercept = false;
                InterceptSettingsButton();
            }
        }

        if (_pendingPauseMenuInject)
        {
            _pauseMenuInjectTimer -= dt;
            if (_pauseMenuInjectTimer <= 0f)
            {
                _pendingPauseMenuInject = false;
                InjectPauseMenuButton();
            }
        }

        // Wait N frames so the closing click doesn't pass through to the game
        if (_reenableEventSystemCountdown > 0)
        {
            _reenableEventSystemCountdown--;
            if (_reenableEventSystemCountdown <= 0)
            {
                try
                {
                    if (_disabledEventSystem != null)
                    {
                        _disabledEventSystem.enabled = true;
                        _disabledEventSystem = null;
                    }
                }
                catch { }
            }
        }

        // Deferred from IMGUI - must run outside OnGUI
        if (_deferredOpenGameSettings)
        {
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
            catch { }

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

            var allButtons = Resources.FindObjectsOfTypeAll<ButtonExtended>();
            Transform? settingsBtn = null;

            if (allButtons != null)
            {
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
                            {
                                settingsBtn = btn.transform;
                                break;
                            }
                        }
                        if (settingsBtn != null) break;
                    }
                    catch { }
                }
            }

            if (settingsBtn == null)
            {
                CrashLog.Log("ModConfig: could not find Settings button (no ButtonExtended with persistent 'Settings' listener).");
                return;
            }

            SettingsButtonTransform = settingsBtn;

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

            var btnExt = settingsBtn.GetComponent<ButtonExtended>();
            if (btnExt != null)
            {
                btnExt.onClick = new ButtonExtended.ButtonClickedEvent();
                btnExt.onClick.AddListener((System.Action)(() => ShowSettingsChoice()));
                CrashLog.Log("ModConfig: Settings button onClick replaced (ButtonExtended).");
            }
            else
            {
                // Fallback to standard Unity Button
                var btn = settingsBtn.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick = new Button.ButtonClickedEvent();
                    btn.onClick.AddListener((System.Action)(() => ShowSettingsChoice()));
                    CrashLog.Log("ModConfig: Settings button onClick replaced (Button fallback).");
                }
                else
                {
                    CrashLog.Log("ModConfig: no ButtonExtended or Button found on Settings button.");
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.InterceptSettingsButton", ex);
        }
    }

    // Uses component hierarchy - LocalisedText hasn't set button labels yet when UI is inactive.
    private static void InjectPauseMenuButton()
    {
        try
        {
            if (_pauseMenuModButton != null) return;

            var pauseMenus = Resources.FindObjectsOfTypeAll<PauseMenu>();
            if (pauseMenus == null || pauseMenus.Count == 0)
            {
                CrashLog.Log("ModConfig: no PauseMenu instance found - cannot inject button.");
                return;
            }

            var pauseMenu = pauseMenus[0];
            var pmUI = pauseMenu.pauseMenuUI;
            if (pmUI == null)
            {
                CrashLog.Log("ModConfig: PauseMenu.pauseMenuUI is null.");
                return;
            }

            CrashLog.Log($"ModConfig: found PauseMenu, pauseMenuUI = '{pmUI.name}'.");

            // The pause menu has two kinds of ButtonExtended:
            //   - Settings tab buttons (System, Audio, etc.) which ALSO have PauseMenu_TabButton
            //   - Action buttons (Resume, Save Game, etc.) which do NOT have PauseMenu_TabButton
            // We only want the action buttons.
            var allButtons = pmUI.GetComponentsInChildren<ButtonExtended>(true);
            if (allButtons == null || allButtons.Count == 0)
            {
                CrashLog.Log("ModConfig: no ButtonExtended found in pauseMenuUI.");
                return;
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
                return;
            }

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

            if (buttonPanel == null)
            {
                CrashLog.Log("ModConfig: button panel is null - cannot inject.");
                return;
            }

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
                    try { t.SetText("Mod Settings"); } catch { }
                    try { t.ForceMeshUpdate(); } catch { }
                }
            }

            var btnExt = clone.GetComponent<ButtonExtended>();
            if (btnExt != null)
            {
                btnExt.onClick = new ButtonExtended.ButtonClickedEvent();
                btnExt.onClick.AddListener((System.Action)(() => ShowPanel()));
                CrashLog.Log("ModConfig: pause menu Mod Settings wired (ButtonExtended).");
            }
            else
            {
                var btn = clone.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick = new Button.ButtonClickedEvent();
                    btn.onClick.AddListener((System.Action)(() => ShowPanel()));
                    CrashLog.Log("ModConfig: pause menu Mod Settings wired (Button fallback).");
                }
                else
                {
                    CrashLog.Log("ModConfig: no ButtonExtended or Button on cloned pause menu button.");
                }
            }

            _pauseMenuModButton = clone;
            CrashLog.Log("ModConfig: Mod Settings button injected into pause menu.");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.InjectPauseMenuButton", ex);
        }
    }

    // Removed - IMGUI methods replaced by UI Toolkit in ModConfigSystem.UI.cs

    public static uint RegisterBool(string modId, string key, string displayName, bool defaultValue, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterBool '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Bool,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                BoolDefault = defaultValue,
                BoolValue = defaultValue
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Bool)
            {
                entry.BoolValue = persisted.BoolValue;
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterBool '{modId}/{key}' = {entry.BoolValue} (default={defaultValue})");
            _logger.Msg($"[ModConfig] Registered bool '{modId}/{key}': {entry.BoolValue}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterBool", ex);
            return 0;
        }
    }

    public static uint RegisterInt(string modId, string key, string displayName, int defaultValue, int min, int max, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterInt '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Int,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                IntDefault = defaultValue,
                IntValue = defaultValue,
                IntMin = min,
                IntMax = max
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Int)
            {
                entry.IntValue = Math.Clamp(persisted.IntValue, min, max);
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterInt '{modId}/{key}' = {entry.IntValue} (default={defaultValue}, range={min}-{max})");
            _logger.Msg($"[ModConfig] Registered int '{modId}/{key}': {entry.IntValue}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterInt", ex);
            return 0;
        }
    }

    public static uint RegisterFloat(string modId, string key, string displayName, float defaultValue, float min, float max, string description)
    {
        try
        {
            var mod = GetOrCreateMod(modId);

            if (mod.Entries.ContainsKey(key))
            {
                CrashLog.Log($"ModConfig: RegisterFloat '{modId}/{key}' already exists, returning 0.");
                return 0;
            }

            var entry = new ConfigEntry
            {
                Type = ConfigEntryType.Float,
                Key = key,
                DisplayName = displayName ?? key,
                Description = description ?? "",
                FloatDefault = defaultValue,
                FloatValue = defaultValue,
                FloatMin = min,
                FloatMax = max
            };

            var persisted = LoadPersistedValue(modId, key);
            if (persisted != null && persisted.Type == ConfigEntryType.Float)
            {
                entry.FloatValue = Math.Clamp(persisted.FloatValue, min, max);
            }

            mod.Entries[key] = entry;
            mod.EntryOrder.Add(key);

            CrashLog.Log($"ModConfig: RegisterFloat '{modId}/{key}' = {entry.FloatValue:F1} (default={defaultValue:F1}, range={min:F1}-{max:F1})");
            _logger.Msg($"[ModConfig] Registered float '{modId}/{key}': {entry.FloatValue:F1}");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.RegisterFloat", ex);
            return 0;
        }
    }

    public static uint GetBool(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Bool)
                    return entry.BoolValue ? 1u : 0u;
            }
            return 0xFFFFFFFF;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetBool", ex);
            return 0xFFFFFFFF;
        }
    }

    public static int GetInt(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                    return entry.IntValue;
            }
            return 0;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetInt", ex);
            return 0;
        }
    }

    public static float GetFloat(string modId, string key)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                    return entry.FloatValue;
            }
            return 0f;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.GetFloat", ex);
            return 0f;
        }
    }

    public static void SetModInfo(string modId, string author, string version)
    {
        try
        {
            if (string.IsNullOrEmpty(modId)) return;
            var mod = GetOrCreateMod(modId);
            if (!string.IsNullOrEmpty(author)) mod.Author = author;
            if (!string.IsNullOrEmpty(version)) mod.Version = version;
            CrashLog.Log($"ModConfig: set mod info for '{modId}': author='{author}', version='{version}'");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetModInfo", ex);
        }
    }

    public static bool RegisterBoolOption(string modId, string key, string displayName, bool defaultValue, string description = "")
        => RegisterBool(modId, key, displayName, defaultValue, description) == 1;

    public static bool RegisterIntOption(string modId, string key, string displayName, int defaultValue, int min, int max, string description = "")
        => RegisterInt(modId, key, displayName, defaultValue, min, max, description) == 1;

    public static bool RegisterFloatOption(string modId, string key, string displayName, float defaultValue, float min, float max, string description = "")
        => RegisterFloat(modId, key, displayName, defaultValue, min, max, description) == 1;

    public static bool GetBoolValue(string modId, string key, bool defaultValue = false)
    {
        uint raw = GetBool(modId, key);
        if (raw == 0xFFFFFFFF) return defaultValue;
        return raw == 1;
    }

    public static int GetIntValue(string modId, string key, int defaultValue = 0)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                    return entry.IntValue;
            }
            return defaultValue;
        }
        catch { return defaultValue; }
    }

    public static float GetFloatValue(string modId, string key, float defaultValue = 0f)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                    return entry.FloatValue;
            }
            return defaultValue;
        }
        catch { return defaultValue; }
    }

    public static bool SetBoolValue(string modId, string key, bool value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Bool)
                {
                    entry.BoolValue = value;
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetBoolValue", ex);
            return false;
        }
    }

    public static bool SetIntValue(string modId, string key, int value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Int)
                {
                    entry.IntValue = Math.Clamp(value, entry.IntMin, entry.IntMax);
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetIntValue", ex);
            return false;
        }
    }

    public static bool SetFloatValue(string modId, string key, float value)
    {
        try
        {
            if (_mods.TryGetValue(modId, out var mod) && mod.Entries.TryGetValue(key, out var entry))
            {
                if (entry.Type == ConfigEntryType.Float)
                {
                    entry.FloatValue = Math.Clamp(value, entry.FloatMin, entry.FloatMax);
                    SaveModConfig(mod);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.SetFloatValue", ex);
            return false;
        }
    }

    public static bool HasOption(string modId, string key)
    {
        return _mods.TryGetValue(modId, out var mod) && mod.Entries.ContainsKey(key);
    }

    public static void OpenPanel() => ShowPanel();
    public static void ClosePanel() => HidePanel();

    private static ModConfig GetOrCreateMod(string modId)
    {
        if (!_mods.TryGetValue(modId, out var mod))
        {
            mod = new ModConfig { ModId = modId };
            _mods[modId] = mod;
            _modOrder.Add(modId);

            if (_selectedModId == null)
                _selectedModId = modId;

            CrashLog.Log($"ModConfig: created mod config for '{modId}'.");
        }
        return mod;
    }

    private static string GetConfigPath(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
            throw new ArgumentException("modId cannot be null or empty", nameof(modId));

        if (modId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            modId.Contains(Path.DirectorySeparatorChar) ||
            modId.Contains(Path.AltDirectorySeparatorChar) ||
            modId.Contains(".."))
        {
            CrashLog.Log($"[Security] Attempted path traversal detected with modId: {modId}");
            throw new ArgumentException("Invalid characters in modId", nameof(modId));
        }

        return Path.Combine(_configDir, modId + ".json");
    }

    private static void SaveModConfig(ModConfig mod)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"entries\": {");

            bool first = true;
            foreach (var key in mod.EntryOrder)
            {
                if (!mod.Entries.TryGetValue(key, out var entry))
                    continue;

                if (!first) sb.AppendLine(",");
                first = false;

                string escapedKey = EscapeJsonString(key);

                switch (entry.Type)
                {
                    case ConfigEntryType.Bool:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"bool\", \"value\": {(entry.BoolValue ? "true" : "false")} }}");
                        break;
                    case ConfigEntryType.Int:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"int\", \"value\": {entry.IntValue} }}");
                        break;
                    case ConfigEntryType.Float:
                        sb.Append($"    \"{escapedKey}\": {{ \"type\": \"float\", \"value\": {entry.FloatValue.ToString(System.Globalization.CultureInfo.InvariantCulture)} }}");
                        break;
                }
            }

            if (!first) sb.AppendLine();
            sb.AppendLine("  }");
            sb.AppendLine("}");

            string path = GetConfigPath(mod.ModId);
            string? dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            CrashLog.Log($"ModConfig: saved config for '{mod.ModId}' to {path}");
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"ModConfigSystem.SaveModConfig({mod.ModId})", ex);
        }
    }

    private static ConfigEntry? LoadPersistedValue(string modId, string key)
    {
        try
        {
            string path = GetConfigPath(modId);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path, Encoding.UTF8);
            var entries = ParseConfigJson(json);

            if (entries != null && entries.TryGetValue(key, out var entry))
                return entry;
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"ModConfigSystem.LoadPersistedValue({modId}/{key})", ex);
        }
        return null;
    }

    private static Dictionary<string, ConfigEntry> ParseConfigJson(string json)
    {
        var result = new Dictionary<string, ConfigEntry>();

        try
        {
            // Find the "entries" object
            int entriesIdx = json.IndexOf("\"entries\"", StringComparison.Ordinal);
            if (entriesIdx < 0) return result;

            // Find the opening brace of entries object
            int braceStart = json.IndexOf('{', entriesIdx + 9);
            if (braceStart < 0) return result;

            // Find the matching closing brace
            int braceEnd = FindMatchingBrace(json, braceStart);
            if (braceEnd < 0) return result;

            string entriesBlock = json.Substring(braceStart + 1, braceEnd - braceStart - 1);

            // Parse each entry: "key": { "type": "...", "value": ... }
            int pos = 0;
            while (pos < entriesBlock.Length)
            {
                // Find the next key
                int keyStart = entriesBlock.IndexOf('"', pos);
                if (keyStart < 0) break;

                int keyEnd = entriesBlock.IndexOf('"', keyStart + 1);
                if (keyEnd < 0) break;

                string entryKey = UnescapeJsonString(entriesBlock.Substring(keyStart + 1, keyEnd - keyStart - 1));

                // Find the entry object opening brace
                int entryBraceStart = entriesBlock.IndexOf('{', keyEnd);
                if (entryBraceStart < 0) break;

                int entryBraceEnd = FindMatchingBrace(entriesBlock, entryBraceStart);
                if (entryBraceEnd < 0) break;

                string entryBody = entriesBlock.Substring(entryBraceStart + 1, entryBraceEnd - entryBraceStart - 1);

                // Parse type
                string? type = ExtractJsonStringValue(entryBody, "type");
                if (type == null)
                {
                    pos = entryBraceEnd + 1;
                    continue;
                }

                // Parse value
                string? valueStr = ExtractJsonRawValue(entryBody, "value");
                if (valueStr == null)
                {
                    pos = entryBraceEnd + 1;
                    continue;
                }

                var entry = new ConfigEntry { Key = entryKey };

                switch (type)
                {
                    case "bool":
                        entry.Type = ConfigEntryType.Bool;
                        entry.BoolValue = valueStr.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "int":
                        entry.Type = ConfigEntryType.Int;
                        if (int.TryParse(valueStr.Trim(), System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out int iv))
                            entry.IntValue = iv;
                        break;
                    case "float":
                        entry.Type = ConfigEntryType.Float;
                        if (float.TryParse(valueStr.Trim(), System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out float fv))
                            entry.FloatValue = fv;
                        break;
                    default:
                        pos = entryBraceEnd + 1;
                        continue;
                }

                result[entryKey] = entry;
                pos = entryBraceEnd + 1;
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ModConfigSystem.ParseConfigJson", ex);
        }

        return result;
    }

    private static int FindMatchingBrace(string text, int openPos)
    {
        int depth = 0;
        for (int i = openPos; i < text.Length; i++)
        {
            if (text[i] == '{') depth++;
            else if (text[i] == '}') depth--;

            if (depth == 0) return i;
        }
        return -1;
    }

    private static string? ExtractJsonStringValue(string body, string key)
    {
        string pattern = "\"" + key + "\"";
        int idx = body.IndexOf(pattern, StringComparison.Ordinal);
        if (idx < 0) return null;

        int colonIdx = body.IndexOf(':', idx + pattern.Length);
        if (colonIdx < 0) return null;

        // Find the opening quote of the value
        int quoteStart = body.IndexOf('"', colonIdx + 1);
        if (quoteStart < 0) return null;

        int quoteEnd = body.IndexOf('"', quoteStart + 1);
        if (quoteEnd < 0) return null;

        return body.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
    }

    private static string? ExtractJsonRawValue(string body, string key)
    {
        string pattern = "\"" + key + "\"";
        int idx = body.IndexOf(pattern, StringComparison.Ordinal);
        if (idx < 0) return null;

        int colonIdx = body.IndexOf(':', idx + pattern.Length);
        if (colonIdx < 0) return null;

        // Skip whitespace after colon
        int start = colonIdx + 1;
        while (start < body.Length && (body[start] == ' ' || body[start] == '\t'))
            start++;

        if (start >= body.Length) return null;

        // If it's a quoted string, extract it
        if (body[start] == '"')
        {
            int quoteEnd = body.IndexOf('"', start + 1);
            if (quoteEnd < 0) return null;
            return body.Substring(start + 1, quoteEnd - start - 1);
        }

        // Otherwise read until comma, brace, bracket, or end
        int end = start;
        while (end < body.Length && body[end] != ',' && body[end] != '}' && body[end] != ']'
               && body[end] != '\r' && body[end] != '\n')
            end++;

        return body.Substring(start, end - start).Trim();
    }

    private static string EscapeJsonString(string? s)
    {
        if (s == null) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"")
                .Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    private static string UnescapeJsonString(string? s)
    {
        if (s == null) return "";
        return s.Replace("\\\"", "\"").Replace("\\\\", "\\")
                .Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
    }


}
