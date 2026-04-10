using System;
using Il2Cpp;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DataCenterModLoader;

public static class ModSettingsMenuBridge
{
    private enum CoreMenuSection
    {
        Dashboard,
        Ui,
        Gameplay,
        Multiplayer,
        Mods,
    }

    private static bool _showCoreHub;
    private static CoreMenuSection _activeSection = CoreMenuSection.Dashboard;
    private static Rect _hubRect = new Rect(0, 0, 960, 760);

    private static bool _stylesInitialized;
    private static GUIStyle _windowStyle;
    private static GUIStyle _titleStyle;
    private static GUIStyle _subtitleStyle;
    private static GUIStyle _buttonStyle;
    private static GUIStyle _labelStyle;
    private static GUIStyle _tabStyle;

    private static GameObject _settingsRoot;
    private static MainMenu _mainMenu;
    private static bool _replaceMainMenuWithWeb;
    private static readonly Key[] _keyOptions =
    {
        Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6,
        Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12
    };

    private static Key _hostKey = Key.F9;
    private static Key _panelKey = Key.F10;
    private static Key _disconnectKey = Key.F11;
    private static Vector2 _runtimeModsScroll;
    private static Vector2 _contentScroll;

    public static void OnSceneLoaded(string sceneName)
    {
        _showCoreHub = false;
        _settingsRoot = null;
        _mainMenu = null;
        _activeSection = CoreMenuSection.Dashboard;
    }

    public static void OnSettingsOpened(MainMenu mainMenu)
    {
        if (mainMenu == null)
            return;

        _mainMenu = mainMenu;
        _settingsRoot = mainMenu.settings;
        _showCoreHub = true;
        _activeSection = CoreMenuSection.Gameplay;

        var bridge = Core.Multiplayer;
        if (bridge != null)
        {
            _hostKey = bridge.HostKey;
            _panelKey = bridge.PanelKey;
            _disconnectKey = bridge.DisconnectKey;
        }

        CenterRect();
    }

    public static void OpenMainMenuSection(GameObject mainMenuRoot, string sectionKey)
    {
        if (mainMenuRoot == null)
            return;

        _mainMenu = mainMenuRoot.GetComponent<MainMenu>();
        if (_mainMenu == null)
            _mainMenu = mainMenuRoot.GetComponentInChildren<MainMenu>(true);

        if (_mainMenu != null)
            _settingsRoot = _mainMenu.settings;

        _showCoreHub = true;
        _activeSection = ParseSection(sectionKey);
        CenterRect();
    }

    public static void DrawGUI()
    {
        if (!_showCoreHub)
            return;

        EnsureStyles();
        CenterRect();

        GUI.Box(_hubRect, "gregCore Main Menu Hub", _windowStyle);
        DrawHubWindow();
    }

    private static void DrawHubWindow()
    {
        float x = _hubRect.x + 18f;
        float y = _hubRect.y + 42f;
        float w = _hubRect.width - 36f;

        GUI.Label(new Rect(x, y, w, 28f), "Modernes Core UI (MainMenu Replace)", _titleStyle);
        y += 28f;
        GUI.Label(new Rect(x, y, w, 22f), "Vollständiger Hub für Settings, Multiplayer und Mods", _subtitleStyle);
        y += 30f;

        DrawTabs(x, y, w);
        y += 46f;

        float panelHeight = _hubRect.height - 170f;
        GUI.Box(new Rect(x, y, w, panelHeight), string.Empty, _windowStyle);
        _contentScroll = GUI.BeginScrollView(
            new Rect(x + 6f, y + 6f, w - 12f, panelHeight - 12f),
            _contentScroll,
            new Rect(0, 0, w - 32f, 980f));

        float contentX = 8f;
        float contentY = 8f;
        float contentW = w - 48f;

        switch (_activeSection)
        {
            case CoreMenuSection.Dashboard:
                DrawDashboard(contentX, ref contentY, contentW);
                break;
            case CoreMenuSection.Ui:
                DrawUiSection(contentX, ref contentY, contentW);
                break;
            case CoreMenuSection.Gameplay:
                DrawGameplaySection(contentX, ref contentY, contentW);
                break;
            case CoreMenuSection.Multiplayer:
                DrawMultiplayerSection(contentX, ref contentY, contentW);
                break;
            case CoreMenuSection.Mods:
                DrawModsSection(contentX, ref contentY, contentW);
                break;
        }

        GUI.EndScrollView();

        float footerY = _hubRect.y + _hubRect.height - 54f;
        if (GUI.Button(new Rect(x, footerY, 240f, 36f), "Game Settings öffnen", _buttonStyle))
            _mainMenu?.Settings();

        if (GUI.Button(new Rect(_hubRect.x + _hubRect.width - 160f, footerY, 140f, 36f), "Schließen", _buttonStyle))
            _showCoreHub = false;
    }

    private static void DrawTabs(float x, float y, float width)
    {
        float tabWidth = (width - 24f) / 5f;
        DrawTabButton(new Rect(x + tabWidth * 0, y, tabWidth - 4f, 36f), CoreMenuSection.Dashboard, "Home");
        DrawTabButton(new Rect(x + tabWidth * 1, y, tabWidth - 4f, 36f), CoreMenuSection.Gameplay, "Settings");
        DrawTabButton(new Rect(x + tabWidth * 2, y, tabWidth - 4f, 36f), CoreMenuSection.Multiplayer, "Multiplayer");
        DrawTabButton(new Rect(x + tabWidth * 3, y, tabWidth - 4f, 36f), CoreMenuSection.Mods, "Mods");
        DrawTabButton(new Rect(x + tabWidth * 4, y, tabWidth - 4f, 36f), CoreMenuSection.Ui, "UI");
    }

    private static void DrawTabButton(Rect rect, CoreMenuSection section, string label)
    {
        bool active = _activeSection == section;
        string renderedLabel = active ? $"● {label}" : label;
        if (GUI.Button(rect, renderedLabel, _tabStyle))
            _activeSection = section;
    }

    private static void DrawDashboard(float x, ref float y, float w)
    {
        GUI.Label(new Rect(x, y, w, 24f), "Core Hub Übersicht", _titleStyle);
        y += 30f;
        GUI.Label(new Rect(x, y, w, 90f), "Dieses MainMenu-Replace vereint alle relevanten Funktionen in einem performanten Hub: Gameplay-Settings, Multiplayer-Controls, Mod-Aktivierung und UI-Konfiguration.", _labelStyle);
        y += 104f;
        GUI.Label(new Rect(x, y, w, 72f), "Tipp: Über die Tabs oben erreichst du direkte Bereiche, inklusive separater Menüpunkte für Multiplayer und Mods.", _labelStyle);
    }

    private static void DrawGameplaySection(float x, ref float y, float w)
    {
        GUI.Label(new Rect(x, y, w, 24f), "Gameplay & allgemeine Einstellungen", _titleStyle);
        y += 30f;
        GUI.Label(new Rect(x, y, w, 72f), "Die nativen Game Settings bleiben voll erhalten und werden bei Bedarf über den Footer-Button geöffnet.", _labelStyle);
        y += 80f;

        if (GUI.Button(new Rect(x, y, 300f, 34f), "Native Game Settings öffnen", _buttonStyle))
            _mainMenu?.Settings();
    }

    private static void DrawUiSection(float x, ref float y, float w)
    {
        GUI.Label(new Rect(x, y, w, 24f), "UI Framework", _titleStyle);
        y += 30f;

        bool webBridgeEnabled = UiExtensionBridge.GetWebBridgeEnabled();
        webBridgeEnabled = GUI.Toggle(new Rect(x, y, w, 24f), webBridgeEnabled, "DC2WEB Bridge aktiv", _labelStyle);
        UiExtensionBridge.SetWebBridgeEnabled(webBridgeEnabled);
        y += 28f;

        UiModernizer.Enabled = GUI.Toggle(new Rect(x, y, w, 24f), UiModernizer.Enabled, "Unity UI Modernizer aktiv", _labelStyle);
        y += 28f;

        _replaceMainMenuWithWeb = GUI.Toggle(new Rect(x, y, w, 24f), _replaceMainMenuWithWeb, "MainMenu komplett per Web-Overlay ersetzen", _labelStyle);
        y += 34f;

        if (GUI.Button(new Rect(x, y, 300f, 34f), "UI Replace auf MainMenu anwenden", _buttonStyle))
        {
            UiExtensionBridge.SetWebProfileReplaceMode("MainMenu", _replaceMainMenuWithWeb);
            if (_mainMenu != null)
            {
                UiExtensionBridge.ResetWebAppliedState(_mainMenu.gameObject);
                UiExtensionBridge.TryApplyOrReplace(_mainMenu.gameObject, "MainMenu");
            }
        }
    }

    private static void DrawMultiplayerSection(float x, ref float y, float w)
    {
        GUI.Label(new Rect(x, y, w, 24f), "Multiplayer", _titleStyle);
        y += 30f;

        if (GUI.Button(new Rect(x, y, w, 32f), $"Host Key: {_hostKey}", _buttonStyle))
            _hostKey = NextKey(_hostKey);
        y += 38f;

        if (GUI.Button(new Rect(x, y, w, 32f), $"Panel Key: {_panelKey}", _buttonStyle))
            _panelKey = NextKey(_panelKey);
        y += 38f;

        if (GUI.Button(new Rect(x, y, w, 32f), $"Disconnect Key: {_disconnectKey}", _buttonStyle))
            _disconnectKey = NextKey(_disconnectKey);
        y += 38f;

        if (GUI.Button(new Rect(x, y, 320f, 34f), "MP Keybinds speichern", _buttonStyle))
        {
            var bridge = Core.Multiplayer;
            bridge?.SetKeybinds(_hostKey, _panelKey, _disconnectKey);
        }
    }

    private static void DrawModsSection(float x, ref float y, float w)
    {
        GUI.Label(new Rect(x, y, w, 24f), "Mods", _titleStyle);
        y += 30f;

        var core = Core.Instance;
        var units = core?.GetRuntimeUnits();
        if (units == null || units.Count == 0)
        {
            GUI.Label(new Rect(x, y, w, 24f), "Keine Runtime-Units gefunden.", _labelStyle);
            return;
        }

        float scrollHeight = 280f;
        float contentHeight = 10f + (units.Count * 30f);
        _runtimeModsScroll = GUI.BeginScrollView(
            new Rect(x, y, w, scrollHeight),
            _runtimeModsScroll,
            new Rect(0, 0, w - 22f, contentHeight));

        float rowY = 6f;
        for (int index = 0; index < units.Count; index++)
        {
            var unit = units[index];
            string hotloadSuffix = unit.SupportsHotReload ? string.Empty : " [DLL: Neustart nötig]";
            string label = $"{unit.DisplayName} [{unit.Language}]{hotloadSuffix}";
            bool updated = GUI.Toggle(new Rect(8f, rowY, w - 40f, 24f), unit.Enabled, label, _labelStyle);
            if (updated != unit.Enabled)
                core?.SetRuntimeUnitEnabled(unit.Id, updated);

            rowY += 30f;
        }

        GUI.EndScrollView();
        y += scrollHeight + 10f;

        if (GUI.Button(new Rect(x, y, 340f, 34f), "Enabled Runtime-Units neu laden", _buttonStyle))
            core?.ReloadRuntimeUnits();
    }

    private static void EnsureStyles()
    {
        if (_stylesInitialized)
            return;

        _windowStyle = GUI.skin.window;
        _titleStyle = GUI.skin.label;
        _subtitleStyle = GUI.skin.label;
        _buttonStyle = GUI.skin.button;
        _labelStyle = GUI.skin.label;
        _tabStyle = GUI.skin.button;

        _subtitleStyle.fontSize = Mathf.Max(16, _subtitleStyle.fontSize);
        _titleStyle.fontSize = Mathf.Max(20, _titleStyle.fontSize);

        _stylesInitialized = true;
    }

    private static void CenterRect()
    {
        _hubRect.x = (Screen.width - _hubRect.width) * 0.5f;
        _hubRect.y = (Screen.height - _hubRect.height) * 0.5f;
    }

    private static CoreMenuSection ParseSection(string sectionKey)
    {
        switch ((sectionKey ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "ui":
                return CoreMenuSection.Ui;
            case "settings":
            case "gameplay":
                return CoreMenuSection.Gameplay;
            case "multiplayer":
            case "mp":
                return CoreMenuSection.Multiplayer;
            case "mods":
            case "mod":
                return CoreMenuSection.Mods;
            default:
                return CoreMenuSection.Dashboard;
        }
    }

    private static Key NextKey(Key current)
    {
        int index = Array.IndexOf(_keyOptions, current);
        if (index < 0)
            return _keyOptions[0];

        return _keyOptions[(index + 1) % _keyOptions.Length];
    }
}
