using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;

namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// Core SDK runtime host that orchestrates language bridges and keeps failures isolated.
/// </summary>
public sealed class GregLanguageBridgeHost
{
    private readonly MelonLogger.Instance _logger;
    private readonly List<IGregLanguageBridge> _bridges = new();

    public GregLanguageBridgeHost(MelonLogger.Instance logger, string rustModsPath, FFIBridge ffiBridge)
    {
        _logger = logger;

        string scriptRoot = Path.Combine(MelonEnvironment.ModsDirectory, "ScriptMods");
        string luaRoot = Path.Combine(scriptRoot, "lua");
        string jsTsRoot = Path.Combine(scriptRoot, "js");

        _bridges.Add(new LuaLanguageBridge(_logger, luaRoot));
        _bridges.Add(new TypeScriptJavaScriptLanguageBridge(_logger, jsTsRoot));
        _bridges.Add(new RustLanguageBridgeAdapter(_logger, rustModsPath, ffiBridge));
    }

    public void Initialize()
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            IGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.Initialize();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"GregLanguageBridgeHost.Initialize.{bridge.LanguageName}", exception);
            }
        }
    }

    public int LoadAll()
    {
        int total = 0;
        for (int index = 0; index < _bridges.Count; index++)
        {
            IGregLanguageBridge bridge = _bridges[index];
            try
            {
                total += bridge.LoadScripts();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"GregLanguageBridgeHost.LoadAll.{bridge.LanguageName}", exception);
            }
        }

        _logger.Msg($"gregCore language host initialized bridges={_bridges.Count}, loadedUnits={total}.");
        return total;
    }

    public void OnSceneLoaded(string sceneName)
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            IGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.OnSceneLoaded(sceneName);
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"GregLanguageBridgeHost.OnSceneLoaded.{bridge.LanguageName}", exception);
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            IGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.OnUpdate(deltaTime);
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"GregLanguageBridgeHost.OnUpdate.{bridge.LanguageName}", exception);
            }
        }
    }

    public void Shutdown()
    {
        for (int index = 0; index < _bridges.Count; index++)
        {
            IGregLanguageBridge bridge = _bridges[index];
            try
            {
                bridge.Shutdown();
            }
            catch (Exception exception)
            {
                CrashLog.LogException($"GregLanguageBridgeHost.Shutdown.{bridge.LanguageName}", exception);
            }
        }
    }
}
