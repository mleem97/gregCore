using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;

namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// Lua bridge placeholder with isolated lifecycle and file discovery.
/// Runtime execution can be attached to a Lua VM without changing Core wiring.
/// </summary>
public sealed class LuaLanguageBridge : IGregLanguageBridge
{
    private static readonly string[] Extensions = { ".lua" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _scriptsRoot;
    private string[] _loadedScripts = Array.Empty<string>();

    public LuaLanguageBridge(MelonLogger.Instance logger, string scriptsRoot)
    {
        _logger = logger;
        _scriptsRoot = scriptsRoot;
    }

    public string LanguageName => "lua";

    public IReadOnlyList<string> ScriptExtensions => Extensions;

    public void Initialize()
    {
        Directory.CreateDirectory(_scriptsRoot);
        _logger.Msg($"gregCore Lua bridge ready: {_scriptsRoot}");
    }

    public int LoadScripts()
    {
        _loadedScripts = Directory.GetFiles(_scriptsRoot, "*.lua", SearchOption.AllDirectories);
        _logger.Msg($"gregCore Lua bridge discovered {_loadedScripts.Length} script(s).");
        return _loadedScripts.Length;
    }

    public void OnSceneLoaded(string sceneName)
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void Shutdown()
    {
        _loadedScripts = Array.Empty<string>();
    }
}
