using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using greg.Core;

namespace greg.Core.Scripting.Go;

/// <summary>
/// Go Language Bridge (v1.0.0.6)
/// Uses WebAssembly (WASM) to run Go-compiled mods within the .NET environment.
/// </summary>
public sealed class GoLanguageBridge : iGregLanguageBridge
{
    private readonly MelonLogger.Instance _logger;
    private readonly string _modDirectory;
    private readonly List<gregRuntimeUnit> _units = new();

    public string LanguageName => "Go (WASM)";
    public IReadOnlyList<string> ScriptExtensions => new List<string> { ".wasm" };

    public GoLanguageBridge(MelonLogger.Instance logger, string modDirectory)
    {
        _logger = logger;
        _modDirectory = modDirectory;

        if (!Directory.Exists(_modDirectory))
        {
            Directory.CreateDirectory(_modDirectory);
        }
    }

    public void Initialize()
    {
        _logger.Msg("[gregCore] Go Language Bridge Initialized (Experimental WASM Support)");
    }

    public int LoadScripts()
    {
        // Placeholder for WASM loading logic
        return 0;
    }

    public IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits() => _units;

    public bool SetUnitEnabled(string unitId, bool enabled) => false;

    public int ReloadEnabledUnits()
    {
        _logger.Msg("This mod uses external vendored libraries, all references can be found at gregframework.eu/vendored-libs");
        _logger.Msg("The Framework is using WASM-runtime for implementing Go Mod Support.");
        return 0;
    }

    public void OnSceneLoaded(string sceneName) { }
    public void OnUpdate(float deltaTime) { }
    public void OnGui() { }

    public void Shutdown()
    {
        _units.Clear();
    }
}
