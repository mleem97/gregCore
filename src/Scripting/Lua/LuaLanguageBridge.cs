using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MoonSharp.Interpreter;
using greg.Core;

namespace gregCoreSDK.Core.Scripting.Lua;

/// <summary>
/// Lua bridge backed by MoonSharp. Each discovered .lua file gets its own <see cref="Script"/>
/// with an isolated <c>greg</c> global table. Lifecycle hooks (<c>on_update</c>, <c>on_scene</c>)
/// are called per-frame by the bridge host when the unit is enabled.
/// </summary>
public sealed class LuaLanguageBridge : iGregLanguageBridge
{
    private static readonly string[] Extensions = { ".lua" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _scriptsRoot;
    private readonly List<gregRuntimeUnit> _runtimeUnits = new();
    private readonly List<LuaScriptState> _scripts = new();
    private readonly List<iGregLuaModule> _modules = new();

    public LuaLanguageBridge(MelonLogger.Instance logger, string scriptsRoot)
    {
        _logger = logger;
        _scriptsRoot = scriptsRoot;

        _modules.Add(new gregHooksLuaModule());
        _modules.Add(new gregUnityLuaModule());
        _modules.Add(new gregIoLuaModule());
        _modules.Add(new gregInputLuaModule());
    }

    public string LanguageName => "lua";

    public IReadOnlyList<string> ScriptExtensions => Extensions;

    /// <summary>Register a C#-backed module that injects API into every new Lua <see cref="Script"/>.</summary>
    public void RegisterModule(iGregLuaModule module)
    {
        _modules.Add(module);
    }

    public void Initialize()
    {
        Directory.CreateDirectory(_scriptsRoot);
        _logger.Msg($"gregCore Lua bridge ready (MoonSharp): {_scriptsRoot}");
    }

    public int LoadScripts()
    {
        ShutdownScripts();
        _runtimeUnits.Clear();

        string[] files;
        try
        {
            files = Directory.GetFiles(_scriptsRoot, "*.lua", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("LuaLanguageBridge.LoadScripts.Discovery", ex);
            return 0;
        }

        for (int i = 0; i < files.Length; i++)
        {
            string scriptPath = files[i];
            string relativePath = Path.GetRelativePath(_scriptsRoot, scriptPath).Replace('\\', '/');
            string unitId = $"lua:{relativePath}";
            bool enabled = gregModActivationService.IsEnabled(unitId, true);

            _runtimeUnits.Add(new gregRuntimeUnit
            {
                Id = unitId,
                DisplayName = relativePath,
                Language = LanguageName,
                Enabled = enabled,
                SupportsHotReload = true,
                IsNativeModule = false
            });

            if (!enabled)
            {
                _scripts.Add(null);
                continue;
            }

            try
            {
                var state = CompileAndRun(scriptPath, unitId);
                _scripts.Add(state);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Lua script failed to load: {relativePath} — {ex.Message}");
                CrashLog.LogException($"LuaLanguageBridge.Load[{unitId}]", ex);
                _scripts.Add(null);
            }
        }

        if (_runtimeUnits.Count > 0)
        {
            _logger.Msg("This mod uses external vendored libraries, all references can be found at gregframework.eu/vendored-libs");
            _logger.Msg("The Framework is using MoonSharp by Marco Cecchi for implementing Lua Mod Support - https://github.com/moonsharp-devs/moonsharp");
        }

        _logger.Msg($"gregCore Lua bridge loaded {_runtimeUnits.Count} script(s).");
        return _runtimeUnits.Count;
    }

    public IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits() => _runtimeUnits;

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(unitId) || !unitId.StartsWith("lua:", StringComparison.OrdinalIgnoreCase))
            return false;

        for (int i = 0; i < _runtimeUnits.Count; i++)
        {
            var unit = _runtimeUnits[i];
            if (!string.Equals(unit.Id, unitId, StringComparison.OrdinalIgnoreCase))
                continue;

            gregModActivationService.SetEnabled(unitId, enabled);
            _runtimeUnits[i] = new gregRuntimeUnit
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

    public int ReloadEnabledUnits() => LoadScripts();

    public void OnSceneLoaded(string sceneName)
    {
        for (int i = 0; i < _scripts.Count; i++)
        {
            if (i >= _runtimeUnits.Count || !_runtimeUnits[i].Enabled)
                continue;

            var state = _scripts[i];
            if (state?.OnScene == null)
                continue;

            try
            {
                state.Vm.Call(state.OnScene, sceneName);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Lua on_scene error [{_runtimeUnits[i].Id}]: {ex.Message}");
                CrashLog.LogException($"LuaLanguageBridge.OnScene[{_runtimeUnits[i].Id}]", ex);
            }
        }
    }

    public void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < _scripts.Count; i++)
        {
            if (i >= _runtimeUnits.Count || !_runtimeUnits[i].Enabled)
                continue;

            var state = _scripts[i];
            if (state == null) continue;

            // 1. Classic global on_update(dt)
            if (state.OnUpdate != null)
            {
                try { state.Vm.Call(state.OnUpdate, deltaTime); }
                catch (Exception ex) { _logger.Warning($"Lua on_update error [{_runtimeUnits[i].Id}]: {ex.Message}"); }
            }

            // 2. New greg.events.on_update handlers
            try
            {
                var internalUpdate = state.Vm.Globals.Get("greg").Table.Get("_internal_update");
                if (internalUpdate.Type == DataType.Function)
                    state.Vm.Call(internalUpdate, (double)deltaTime);
            }
            catch { }
        }
    }

    /// <summary>Call from Unity OnGUI — dispatches <c>on_gui()</c> to enabled Lua scripts.</summary>
    public void OnGui()
    {
        for (int i = 0; i < _scripts.Count; i++)
        {
            if (i >= _runtimeUnits.Count || !_runtimeUnits[i].Enabled)
                continue;

            var state = _scripts[i];
            if (state == null) continue;

            // 1. Classic global on_gui()
            if (state.OnGui != null)
            {
                try { state.Vm.Call(state.OnGui); }
                catch (Exception ex) { _logger.Warning($"Lua on_gui error [{_runtimeUnits[i].Id}]: {ex.Message}"); }
            }

            // 2. New greg.events.on_gui handlers
            try
            {
                var internalGui = state.Vm.Globals.Get("greg").Table.Get("_internal_gui");
                if (internalGui.Type == DataType.Function)
                    state.Vm.Call(internalGui);
            }
            catch { }
        }
    }

    public void Shutdown()
    {
        ShutdownScripts();
        _runtimeUnits.Clear();
    }

    private LuaScriptState CompileAndRun(string filePath, string unitId)
    {
        var vm = new Script(CoreModules.Preset_SoftSandbox);

        var greg = new Table(vm);
        vm.Globals["greg"] = greg;

        // greg.log
        greg["log"] = (Action<string>)(msg => _logger.Msg($"[lua:{unitId}] {msg}"));
        greg["warn"] = (Action<string>)(msg => _logger.Warning($"[lua:{unitId}] {msg}"));
        greg["error"] = (Action<string>)(msg =>
        {
            _logger.Error($"[lua:{unitId}] {msg}");
            CrashLog.LogError($"lua:{unitId}", msg);
        });

        // greg.paths
        var paths = new Table(vm);
        paths["scripts"] = _scriptsRoot;
        paths["userdata"] = MelonLoader.Utils.MelonEnvironment.UserDataDirectory;
        paths["mods"] = MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        paths["game"] = MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        greg["paths"] = paths;

        // Let registered modules inject their API surfaces
        for (int m = 0; m < _modules.Count; m++)
        {
            try
            {
                _modules[m].Register(vm, greg);
            }
            catch (Exception ex)
            {
                CrashLog.LogException($"LuaLanguageBridge.Module[{_modules[m].GetType().Name}]", ex);
            }
        }

        string code = File.ReadAllText(filePath);
        vm.DoString(code, codeFriendlyName: unitId);

        var onUpdate = vm.Globals.Get("on_update");
        var onScene = vm.Globals.Get("on_scene");
        var onGui = vm.Globals.Get("on_gui");

        return new LuaScriptState
        {
            Vm = vm,
            OnUpdate = onUpdate.Type == DataType.Function ? onUpdate : null,
            OnScene = onScene.Type == DataType.Function ? onScene : null,
            OnGui = onGui.Type == DataType.Function ? onGui : null
        };
    }

    private void ShutdownScripts()
    {
        _scripts.Clear();
    }

    private sealed class LuaScriptState
    {
        public Script Vm;
        public DynValue OnUpdate;
        public DynValue OnScene;
        public DynValue OnGui;
    }
}


