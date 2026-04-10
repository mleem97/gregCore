using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;

namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// TypeScript/JavaScript bridge placeholder with isolated lifecycle and file discovery.
/// Runtime execution can be attached to a JS engine without changing Core wiring.
/// </summary>
public sealed class TypeScriptJavaScriptLanguageBridge : IGregLanguageBridge
{
    private static readonly string[] Extensions = { ".js", ".ts", ".mjs", ".cjs" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _scriptsRoot;
    private string[] _loadedScripts = Array.Empty<string>();

    public TypeScriptJavaScriptLanguageBridge(MelonLogger.Instance logger, string scriptsRoot)
    {
        _logger = logger;
        _scriptsRoot = scriptsRoot;
    }

    public string LanguageName => "typescript/javascript";

    public IReadOnlyList<string> ScriptExtensions => Extensions;

    public void Initialize()
    {
        Directory.CreateDirectory(_scriptsRoot);
        _logger.Msg($"gregCore TS/JS bridge ready: {_scriptsRoot}");
    }

    public int LoadScripts()
    {
        string[] js = Directory.GetFiles(_scriptsRoot, "*.js", SearchOption.AllDirectories);
        string[] ts = Directory.GetFiles(_scriptsRoot, "*.ts", SearchOption.AllDirectories);
        string[] mjs = Directory.GetFiles(_scriptsRoot, "*.mjs", SearchOption.AllDirectories);
        string[] cjs = Directory.GetFiles(_scriptsRoot, "*.cjs", SearchOption.AllDirectories);

        _loadedScripts = new string[js.Length + ts.Length + mjs.Length + cjs.Length];
        int offset = 0;
        js.CopyTo(_loadedScripts, offset); offset += js.Length;
        ts.CopyTo(_loadedScripts, offset); offset += ts.Length;
        mjs.CopyTo(_loadedScripts, offset); offset += mjs.Length;
        cjs.CopyTo(_loadedScripts, offset);

        _logger.Msg($"gregCore TS/JS bridge discovered {_loadedScripts.Length} script(s).");
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
