using System;
using System.Collections.Generic;
using MelonLoader;

namespace gregModLoader.LanguageBridges;

/// <summary>
/// Adapter that represents native Rust/C mod support inside the shared language host.
/// Delegates all lifecycle calls to <see cref="gregFfiBridge"/>.
/// </summary>
public sealed class RustLanguageBridgeAdapter : iGregLanguageBridge
{
    private static readonly string[] Extensions = { ".dll", ".greg", ".gregr", ".gregl", ".gregp" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _modsPath;
    private readonly gregFfiBridge _ffiBridge;

    public RustLanguageBridgeAdapter(MelonLogger.Instance logger, string modsPath, gregFfiBridge gregFfiBridge)
    {
        _logger = logger;
        _modsPath = modsPath;
        _ffiBridge = gregFfiBridge;
    }

    public string LanguageName => "rust/native";

    public IReadOnlyList<string> ScriptExtensions => Extensions;

    public void Initialize()
    {
        _logger.Msg($"[gregCore] Rust/native bridge initialized, mods path: {_modsPath}");
        CrashLog.Log("RustLanguageBridgeAdapter: Initialize");
    }

    public int LoadScripts()
    {
        try
        {
            _ffiBridge.LoadAllMods();
            var units = _ffiBridge.GetLoadedRuntimeUnits();
            _logger.Msg($"[gregCore] Rust/native bridge loaded {units.Count} mod(s).");
            return units.Count;
        }
        catch (Exception ex)
        {
            _logger.Error($"[gregCore] Rust/native bridge LoadAllMods failed: {ex.Message}");
            CrashLog.LogException("RustLanguageBridgeAdapter.LoadScripts", ex);
            return 0;
        }
    }

    public IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits()
    {
        return _ffiBridge.GetLoadedRuntimeUnits();
    }

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(unitId) || !unitId.StartsWith("native:", StringComparison.OrdinalIgnoreCase))
            return false;

        _ffiBridge.SetModEnabled(unitId, enabled);
        return true;
    }

    public int ReloadEnabledUnits()
    {
        return _ffiBridge.ReloadAllMods();
    }

    public void OnSceneLoaded(string sceneName)
    {
        try
        {
            _ffiBridge.OnSceneLoaded(sceneName);
        }
        catch (Exception ex)
        {
            _logger.Error($"[gregCore] Rust/native bridge OnSceneLoaded failed: {ex.Message}");
            CrashLog.LogException("RustLanguageBridgeAdapter.OnSceneLoaded", ex);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        try
        {
            _ffiBridge.OnUpdate(deltaTime);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RustLanguageBridgeAdapter.OnUpdate", ex);
        }
    }

    public void OnGui()
    {
        try
        {
            _ffiBridge.OnGui();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RustLanguageBridgeAdapter.OnGui", ex);
        }
    }

    public void Shutdown()
    {
        try
        {
            _logger.Msg("[gregCore] Shutting down Rust/native bridge...");
            _ffiBridge.Shutdown();
        }
        catch (Exception ex)
        {
            _logger.Error($"[gregCore] Rust/native bridge Shutdown failed: {ex.Message}");
            CrashLog.LogException("RustLanguageBridgeAdapter.Shutdown", ex);
        }
    }
}


