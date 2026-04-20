using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace greg.Diagnostic;

public sealed class GregFrameLimiterService
{
    public static GregFrameLimiterService Instance { get; private set; } = null!;

    public enum GameState { Unknown, Menu, Gameplay, Loading }
    public string CurrentStateName => _currentState.ToString();

    private GameState _currentState = GameState.Unknown;
    private bool _isFocused = true;
    private bool _isMinimized = false;
    private bool _isAfk = false;
    private float _lastInputTime = 0f;
    private int _frame = 0;

    private FrameLimiterConfig? _cfg;

    public void Initialize(FrameLimiterConfig cfg)
    {
        Instance = this;
        _cfg = cfg;

        if (cfg == null || !cfg.Enabled)
        {
            MelonLogger.Msg("[FrameLimiter] Disabled via config.");
            return;
        }

        SetState(GameState.Menu);
        MelonLogger.Msg("[FrameLimiter] Initialized. Menu limit active.");
    }

    public void SetState(GameState state)
    {
        if (_currentState == state) return;
        _currentState = state;
        ApplyCurrentLimit();
    }

    public void OnFocusChanged(bool focused)
    {
        _isFocused = focused;
        ApplyCurrentLimit();
    }

    public void OnMinimizeChanged(bool minimized)
    {
        _isMinimized = minimized;
        ApplyCurrentLimit();
    }

    public void Tick()
    {
        if (_cfg == null || !_cfg.Enabled) return;

        bool anyInput = false;
        try
        {
            if (Keyboard.current != null)
            {
                anyInput = Keyboard.current.anyKey.isPressed;
            }
            if (!anyInput && Mouse.current != null)
            {
                anyInput = Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;
            }
        }
        catch { }

        if (anyInput)
        {
            _lastInputTime = Time.realtimeSinceStartup;
            if (_isAfk)
            {
                _isAfk = false;
                ApplyCurrentLimit();
                MelonLogger.Msg("[FrameLimiter] AFK ended — restoring limit.");
            }
        }
        else if (!_isAfk
              && _cfg?.Afk?.Enabled == true
              && _currentState == GameState.Gameplay
              && (Time.realtimeSinceStartup - _lastInputTime) > _cfg.Afk.AfkAfterSeconds)
        {
            _isAfk = true;
            ApplyCurrentLimit();
            MelonLogger.Msg("[FrameLimiter] AFK detected — reducing FPS.");
        }
    }

    private void ApplyCurrentLimit()
    {
        if (_cfg == null || !_cfg.Enabled) return;

        int targetFps;
        int vSync;
        string reason;

        if (_isMinimized)
        {
            targetFps = _cfg.Minimized.TargetFps;
            vSync = _cfg.Minimized.VSync;
            reason = "minimized";
        }
        else if (!_isFocused)
        {
            targetFps = _cfg.Background.TargetFps;
            vSync = _cfg.Background.VSync;
            reason = "background";
        }
        else if (_isAfk)
        {
            targetFps = _cfg.Afk.TargetFps;
            vSync = _cfg.Afk.VSync;
            reason = "afk";
        }
        else
        {
            (targetFps, vSync, reason) = _currentState switch
            {
                GameState.Menu => (_cfg.Menu.TargetFps, _cfg.Menu.VSync, "menu"),
                GameState.Loading => (_cfg.Menu.TargetFps, _cfg.Menu.VSync, "loading"),
                GameState.Gameplay => (_cfg.Gameplay.TargetFps, _cfg.Gameplay.VSync, "gameplay"),
                _ => (_cfg.Menu.TargetFps, _cfg.Menu.VSync, "unknown"),
            };
        }

        try
        {
            if (Application.targetFrameRate == targetFps
             && QualitySettings.vSyncCount == vSync)
                return;

            Application.targetFrameRate = targetFps;
            QualitySettings.vSyncCount = vSync;

            MelonLogger.Msg($"[FrameLimiter] {reason} → targetFPS={targetFps} vSync={vSync}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[FrameLimiter] ApplyCurrentLimit failed: {ex.Message}");
        }
    }
}