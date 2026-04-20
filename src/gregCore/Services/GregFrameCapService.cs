using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using greg.Diagnostic;

namespace gregCore.Services;

public sealed class GregFrameCapService
{
    public static GregFrameCapService Instance { get; private set; } = null!;

    private bool _appFocused = true;
    private bool _isAfk = false;
    private float _lastInput = 0f;

    public void Initialize()
    {
        Instance = this;
        ForceApply("init");
        MelonLogger.Msg("[FrameCap] Initialized and applied immediately.");
    }

    public void OnSceneLoaded(string sceneName)
    {
        var isMenu = sceneName.IndexOf("menu", StringComparison.OrdinalIgnoreCase) >= 0
                   || sceneName.IndexOf("main", StringComparison.OrdinalIgnoreCase) >= 0
                   || sceneName.IndexOf("load", StringComparison.OrdinalIgnoreCase) >= 0;

        GregPerfConfig.Instance.CurrentTarget = isMenu
            ? GregPerfConfig.Instance.MenuFps
            : GregPerfConfig.Instance.GameplayFps;

        ForceApply($"scene:{sceneName}");
    }

    public void OnFocusChanged(bool focused)
    {
        _appFocused = focused;
        GregPerfConfig.Instance.CurrentTarget = focused
            ? GregPerfConfig.Instance.GameplayFps
            : GregPerfConfig.Instance.BackgroundFps;
        ForceApply(focused ? "focused" : "background");
    }

    public void Tick()
    {
        var cfg = GregPerfConfig.Instance;
        if (!cfg.AfkEnabled) return;

        bool hasInput = false;
        try
        {
            hasInput = Keyboard.current?.anyKey?.isPressed == true
                    || Mouse.current?.delta?.ReadValue().sqrMagnitude > 0.1f
                    || Mouse.current?.leftButton?.isPressed == true;
        }
        catch { }

        if (hasInput)
        {
            _lastInput = Time.realtimeSinceStartup;
            if (_isAfk)
            {
                _isAfk = false;
                cfg.CurrentTarget = cfg.GameplayFps;
                ForceApply("afk-end");
            }
        }
        else
        {
            float idle = Time.realtimeSinceStartup - _lastInput;
            if (!_isAfk && idle > cfg.AfkSeconds)
            {
                _isAfk = true;
                cfg.CurrentTarget = cfg.AfkFps;
                ForceApply("afk-start");
            }
        }
    }

    void ForceApply(string reason)
    {
        int target = GregPerfConfig.Instance.CurrentTarget;
        Application.targetFrameRate = target;
        QualitySettings.vSyncCount = 0;
        MelonLogger.Msg($"[FrameCap] [{reason}] targetFPS={target} ✓");
    }
}