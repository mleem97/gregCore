using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using greg.Core;

namespace greg.Core.Scripting;

/// <summary>
/// gregCoreLoader SDK runtime host that orchestrates language bridges and keeps failures isolated.
/// </summary>
public sealed class gregLanguageBridgeHost
{
    private readonly MelonLogger.Instance _logger;
    private readonly List<iGregLanguageBridge> _bridges = new();

    public Lua.LuaLanguageBridge LuaBridge { get; }

    public gregLanguageBridgeHost(MelonLogger.Instance logger, string rustModsPath, greg.Core.gregFfiBridge gregFfiBridge)
    {
        _logger = logger;

        string scriptRoot = Path.Combine(MelonEnvironment.ModsDirectory, "ScriptMods");
        string luaRoot = Path.Combine(scriptRoot, "lua");
        string jsTsRoot = Path.Combine(scriptRoot, "js");
        string goRoot = Path.Combine(scriptRoot, "go");

        LuaBridge = TryCreateBridge(
            bridgeDisplayName: "Lua",
            factory: () => new Lua.LuaLanguageBridge(_logger, luaRoot)) as Lua.LuaLanguageBridge;

        if (LuaBridge != null)
        {
            _bridges.Add(LuaBridge);
        }

        TryAddBridge(
            bridgeDisplayName: "TS/JS",
            factory: () => new JS.TypeScriptJavaScriptLanguageBridge(_logger, jsTsRoot));

        TryAddBridge(
            bridgeDisplayName: "Rust/Native",
            factory: () => new Rust.RustLanguageBridgeAdapter(_logger, rustModsPath, gregFfiBridge));

        TryAddBridge(
            bridgeDisplayName: "Go (WASM)",
            factory: () => new Go.GoLanguageBridge(_logger, goRoot));
    }

    private void TryAddBridge(string bridgeDisplayName, Func<iGregLanguageBridge> factory)
    {
        iGregLanguageBridge bridge = TryCreateBridge(bridgeDisplayName, factory);
        if (bridge != null)
        {
            _bridges.Add(bridge);
        }
    }

    private iGregLanguageBridge TryCreateBridge(string bridgeDisplayName, Func<iGregLanguageBridge> factory)
    {
        try
        {
            return factory();
        }
        catch (Exception exception) when (
            exception is FileNotFoundException ||
            exception is FileLoadException ||
            exception is DllNotFoundException ||
            exception is BadImageFormatException)
        {
            _logger.Warning($"gregCore bridge '{bridgeDisplayName}' disabled due to missing/invalid dependency: {exception.Message}");
            CrashLog.LogException($"gregLanguageBridgeHost.Create.{bridgeDisplayName}", exception);
            return null;
        }
        catch (Exception exception)
        {
            _logger.Warning($"gregCore bridge '{bridgeDisplayName}' disabled due to initialization error: {exception.Message}");
            CrashLog.LogException($"gregLanguageBridgeHost.Create.{bridgeDisplayName}", exception);
            return null;
        }
    }

    public void Initialize()
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.Initialize();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.Initialize.{bridge.LanguageName}", exception);
            }
        }
    }

    public int LoadAll()
    {
        int total = 0;
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                total += bridge.LoadScripts();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.LoadAll.{bridge.LanguageName}", exception);
            }
        }

        _logger.Msg($"gregCore language host initialized bridges={_bridges.Count}, loadedUnits={total}.");
        return total;
    }

    public void OnSceneLoaded(string sceneName)
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.OnSceneLoaded(sceneName);
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.OnSceneLoaded.{bridge.LanguageName}", exception);
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.OnUpdate(deltaTime);
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.OnUpdate.{bridge.LanguageName}", exception);
            }
        }
    }

    public void OnGui()
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.OnGui();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.OnGui.{bridge.LanguageName}", exception);
            }
        }
    }

    public IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits()
    {
        var units = new List<gregRuntimeUnit>();
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                var bridgeUnits = bridge.GetRuntimeUnits();
                for (int unitIndex = 0; unitIndex < bridgeUnits.Count; unitIndex++)
                {
                    units.Add(bridgeUnits[unitIndex]);
                }
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.GetRuntimeUnits.{bridge.LanguageName}", exception);
            }
        }

        return units;
    }

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                if (bridge.SetUnitEnabled(unitId, enabled))
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.SetUnitEnabled.{bridge.LanguageName}", exception);
            }
        }

        return false;
    }

    public int ReloadHotloadableUnits()
    {
        int total = 0;
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                total += bridge.ReloadEnabledUnits();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.ReloadEnabledUnits.{bridge.LanguageName}", exception);
            }
        }

        return total;
    }

    public void Shutdown()
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            iGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.Shutdown();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"gregLanguageBridgeHost.Shutdown.{bridge.LanguageName}", exception);
            }
        }
    }
}




