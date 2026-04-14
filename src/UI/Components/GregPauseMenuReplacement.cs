using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Core.UI.Components;

public class GregPauseMenuReplacement : MonoBehaviour
{
    public static GregPauseMenuReplacement Instance { get; private set; }

    private GameObject _root;
    private GregPanel _mainPanel;
    private bool _isVisible = false;

    private Action _onResumeClicked;
    private Action _onSettingsClicked;
    private Action _onSaveClicked;
    private Action _onLoadClicked;
    private Action _onModsClicked;
    private Action _onQuitToMenuClicked;
    private Action _onQuitToDesktopClicked;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUI();
        gameObject.SetActive(false);
    }

    public void Configure(Action onResume, Action onSettings, Action onSave, Action onLoad, Action onMods, Action onQuitToMenu, Action onQuitToDesktop)
    {
        _onResumeClicked = onResume;
        _onSettingsClicked = onSettings;
        _onSaveClicked = onSave;
        _onLoadClicked = onLoad;
        _onModsClicked = onMods;
        _onQuitToMenuClicked = onQuitToMenu;
        _onQuitToDesktopClicked = onQuitToDesktop;
    }

    private void InitializeUI()
    {
        _mainPanel = GregUIBuilder.Panel("pause_menu_replacement")
            .Title("PAUSE")
            .Position(GregUIAnchor.Center)
            .Size(400, 550)
            .Closable(false)
            .Draggable(false)
            .AddButton("RESUME", () => _onResumeClicked?.Invoke(), GregButtonStyle.Primary)
            .AddSeparator()
            .AddButton("SETTINGS", () => _onSettingsClicked?.Invoke(), GregButtonStyle.Secondary)
            .AddButton("SAVE GAME", () => _onSaveClicked?.Invoke(), GregButtonStyle.Secondary)
            .AddButton("LOAD GAME", () => _onLoadClicked?.Invoke(), GregButtonStyle.Secondary)
            .AddButton("MODS", () => _onModsClicked?.Invoke(), GregButtonStyle.Secondary)
            .AddSeparator()
            .AddButton("MAIN MENU", () => _onQuitToMenuClicked?.Invoke(), GregButtonStyle.Danger)
            .AddButton("EXIT TO DESKTOP", () => _onQuitToDesktopClicked?.Invoke(), GregButtonStyle.Danger)
            .Build();

        if (_mainPanel.PanelRoot != null)
        {
            _mainPanel.PanelRoot.transform.SetParent(this.transform, false);
        }
    }

    public void Show()
    {
        _isVisible = true;
        gameObject.SetActive(true);
        _mainPanel.Show();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        _isVisible = false;
        _mainPanel.Hide();
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (_isVisible) Hide();
        else Show();
    }
}
