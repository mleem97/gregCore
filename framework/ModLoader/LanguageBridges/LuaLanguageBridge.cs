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
    private readonly List<GregRuntimeUnit> _runtimeUnits = new();

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
        _runtimeUnits.Clear();
        string[] loadedScripts = Directory.GetFiles(_scriptsRoot, "*.lua", SearchOption.AllDirectories);

        for (int index = 0; index < loadedScripts.Length; index++)
        {
            string scriptPath = loadedScripts[index];
            string relativePath = Path.GetRelativePath(_scriptsRoot, scriptPath).Replace('\\', '/');
            string unitId = $"lua:{relativePath}";
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

        _logger.Msg($"gregCore Lua bridge discovered {_runtimeUnits.Count} script(s).");
        return _runtimeUnits.Count;
    }

    public IReadOnlyList<GregRuntimeUnit> GetRuntimeUnits()
    {
        return _runtimeUnits;
    }

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(unitId) || !unitId.StartsWith("lua:", StringComparison.OrdinalIgnoreCase))
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
