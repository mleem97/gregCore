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
    private readonly List<GregRuntimeUnit> _runtimeUnits = new();

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

        string[] loadedScripts = new string[js.Length + ts.Length + mjs.Length + cjs.Length];
        int offset = 0;
        js.CopyTo(loadedScripts, offset); offset += js.Length;
        ts.CopyTo(loadedScripts, offset); offset += ts.Length;
        mjs.CopyTo(loadedScripts, offset); offset += mjs.Length;
        cjs.CopyTo(loadedScripts, offset);

        _runtimeUnits.Clear();
        for (int index = 0; index < loadedScripts.Length; index++)
        {
            string scriptPath = loadedScripts[index];
            string relativePath = Path.GetRelativePath(_scriptsRoot, scriptPath).Replace('\\', '/');
            string unitId = $"tsjs:{relativePath}";
            bool enabled = ModActivationService.IsEnabled(unitId, true);

            _runtimeUnits.Add(new GregRuntimeUnit
            {
                Id = unitId,
                DisplayName = relativePath,
                Language = LanguageName,
                Enabled = enabled,
                SupportsHotReload = true,
                IsNativeModule = false
            });
        }

        _logger.Msg($"gregCore TS/JS bridge discovered {_runtimeUnits.Count} script(s).");
        return _runtimeUnits.Count;
    }

    public IReadOnlyList<GregRuntimeUnit> GetRuntimeUnits()
    {
        return _runtimeUnits;
    }

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(unitId) || !unitId.StartsWith("tsjs:", StringComparison.OrdinalIgnoreCase))
            return false;

        for (int index = 0; index < _runtimeUnits.Count; index++)
        {
            var unit = _runtimeUnits[index];
            if (!string.Equals(unit.Id, unitId, StringComparison.OrdinalIgnoreCase))
                continue;

            ModActivationService.SetEnabled(unitId, enabled);
            _runtimeUnits[index] = new GregRuntimeUnit
            {
                Id = unit.Id,
                DisplayName = unit.DisplayName,
                Language = unit.Language,
                Enabled = enabled,
                SupportsHotReload = unit.SupportsHotReload,
                IsNativeModule = unit.IsNativeModule
            };
            return true;
        }

        return false;
    }

    public int ReloadEnabledUnits()
    {
        return LoadScripts();
    }

    public void OnSceneLoaded(string sceneName)
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void Shutdown()
    {
        _runtimeUnits.Clear();
    }
}
