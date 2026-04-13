using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace greg.Core.UI;

public enum UIMode
{
    MainMenu,
    Playing,
    Paused,
    Settings,
    ModConfig,
    ComputerShop,
    AssetManagement,
    BalanceSheet,
    Hire,
    Tutorial,
    Loading
}

public static class UIRouter
{
    private static UIMode _currentMode = UIMode.MainMenu;
    private static UIMode _previousMode = UIMode.MainMenu;
    private static bool _isInitialized = false;

    private static readonly Dictionary<UIMode, UIPageConfig> _pageConfigs = new()
    {
        [UIMode.MainMenu] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_OverAll", "CountersCanvas" },
            HideOnEnter = new[] { "PauseMenuCanvas", "Canvas_ComputerShop", "Canvas_Main", "Canvas_SetIP", "Canvas_ChooseCustomer" },
            ShowOnEnter = Array.Empty<string>(),
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.Playing] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_Main", "Canvas_OverAll", "CountersCanvas" },
            HideOnEnter = new[] { "PauseMenuCanvas", "Canvas_ComputerShop", "Canvas_SetIP", "Canvas_ChooseCustomer", "Canvas_SwitchSeting" },
            ShowOnEnter = Array.Empty<string>(),
            LockCursor = true,
            ShowCursor = false
        },
        [UIMode.Paused] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_Main", "Canvas_OverAll", "CountersCanvas", "PauseMenuCanvas" },
            HideOnEnter = new[] { "Canvas_ComputerShop", "Canvas_SetIP", "Canvas_ChooseCustomer" },
            ShowOnEnter = new[] { "PauseMenuCanvas" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.Settings] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_Main", "Canvas_OverAll", "CountersCanvas", "PauseMenuCanvas" },
            HideOnEnter = new[] { "Canvas_ComputerShop", "Canvas_SetIP", "Canvas_ChooseCustomer" },
            ShowOnEnter = new[] { "PauseMenuCanvas" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.ModConfig] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_Main", "Canvas_OverAll", "CountersCanvas" },
            HideOnEnter = Array.Empty<string>(),
            ShowOnEnter = Array.Empty<string>(),
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.ComputerShop] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_ComputerShop", "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas", "Canvas_SetIP", "Canvas_ChooseCustomer" },
            ShowOnEnter = new[] { "Canvas_ComputerShop" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.AssetManagement] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_ComputerShop", "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas" },
            ShowOnEnter = new[] { "Canvas_ComputerShop" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.BalanceSheet] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_ComputerShop", "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas" },
            ShowOnEnter = new[] { "Canvas_ComputerShop" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.Hire] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_ComputerShop", "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas" },
            ShowOnEnter = new[] { "Canvas_ComputerShop" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.Tutorial] = new UIPageConfig
        {
            CanvasNames = new[] { "Tutorials", "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas", "Canvas_ComputerShop" },
            ShowOnEnter = new[] { "Tutorials" },
            LockCursor = false,
            ShowCursor = true
        },
        [UIMode.Loading] = new UIPageConfig
        {
            CanvasNames = new[] { "Canvas_OverAll" },
            HideOnEnter = new[] { "Canvas_Main", "PauseMenuCanvas", "Canvas_ComputerShop", "Canvas_SetIP", "Canvas_ChooseCustomer", "CountersCanvas" },
            ShowOnEnter = new[] { "Canvas_OverAll" },
            LockCursor = false,
            ShowCursor = false
        }
    };

    public static UIMode CurrentMode => _currentMode;
    public static event Action<UIMode, UIMode> OnModeChanged;

    public static void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        MelonLogger.Msg("[UIRouter] Initialized.");
    }

    public static void SetMode(UIMode newMode)
    {
        if (_currentMode == newMode) return;
        if (!_pageConfigs.TryGetValue(newMode, out var config))
        {
            MelonLogger.Warning($"[UIRouter] No config for mode: {newMode}");
            return;
        }

        var oldMode = _currentMode;
        _previousMode = oldMode;
        _currentMode = newMode;

        ApplyConfig(config);
        OnModeChanged?.Invoke(oldMode, newMode);

        MelonLogger.Msg($"[UIRouter] Mode changed: {oldMode} -> {newMode}");
    }

    public static void GoBack()
    {
        if (_previousMode != _currentMode)
        {
            SetMode(_previousMode);
        }
    }

    public static void TogglePause()
    {
        if (_currentMode == UIMode.Paused)
        {
            SetMode(UIMode.Playing);
        }
        else if (_currentMode == UIMode.Playing)
        {
            SetMode(UIMode.Paused);
        }
    }

    private static void ApplyConfig(UIPageConfig config)
    {
        if (config.LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = config.ShowCursor;
        }

        var allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();

        foreach (var canvas in allCanvases)
        {
            if (canvas == null || canvas.gameObject == null) continue;

            string canvasName = canvas.gameObject.name;

            if (ContainsAny(canvasName, config.HideOnEnter))
            {
                canvas.gameObject.SetActive(false);
            }
            else if (ContainsAny(canvasName, config.ShowOnEnter))
            {
                canvas.gameObject.SetActive(true);
            }
        }
    }

    private static bool ContainsAny(string name, string[] patterns)
    {
        foreach (var pattern in patterns)
        {
            if (name.Contains(pattern))
                return true;
        }
        return false;
    }

    public static void ShowCanvas(string canvasName)
    {
        var canvas = GameObject.Find(canvasName);
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }

    public static void HideCanvas(string canvasName)
    {
        var canvas = GameObject.Find(canvasName);
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    public static void ForceHideAll()
    {
        var allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (var canvas in allCanvases)
        {
            if (canvas != null && canvas.gameObject != null)
            {
                if (!canvas.gameObject.name.Contains("GregMainMenu"))
                {
                    canvas.gameObject.SetActive(false);
                }
            }
        }
    }
}

public class UIPageConfig
{
    public string[] CanvasNames { get; set; } = Array.Empty<string>();
    public string[] HideOnEnter { get; set; } = Array.Empty<string>();
    public string[] ShowOnEnter { get; set; } = Array.Empty<string>();
    public bool LockCursor { get; set; } = true;
    public bool ShowCursor { get; set; } = false;
}
