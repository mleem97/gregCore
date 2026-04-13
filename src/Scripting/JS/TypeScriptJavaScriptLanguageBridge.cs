#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using Jint;
using Jint.Native;
using MelonLoader;
using greg.Core;
using greg.Core.Scripting.Lua;

namespace greg.Core.Scripting.JS;

/// <summary>
/// TypeScript/JavaScript bridge with Jint integration for runtime JS execution.
/// </summary>
public sealed class TypeScriptJavaScriptLanguageBridge : iGregLanguageBridge
{
    private static readonly string[] Extensions = { ".js", ".ts", ".mjs", ".cjs" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _scriptsRoot;
    private readonly List<gregRuntimeUnit> _runtimeUnits = new();
    
    private readonly List<Engine> _engines = new();
    private readonly List<Action<float>> _onUpdateHandlers = new();
    private readonly List<Action> _onGuiHandlers = new();

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
        _runtimeUnits.Clear();
        _engines.Clear();
        _onUpdateHandlers.Clear();
        _onGuiHandlers.Clear();

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

        for (int index = 0; index < loadedScripts.Length; index++)
        {
            string scriptPath = loadedScripts[index];
            string relativePath = Path.GetRelativePath(_scriptsRoot, scriptPath).Replace('\\', '/');
            string unitId = $"tsjs:{relativePath}";
            bool enabled = gregModActivationService.IsEnabled(unitId, true);

            var unit = new gregRuntimeUnit
            {
                Id = unitId,
                DisplayName = relativePath,
                Language = LanguageName,
                Enabled = enabled,
                SupportsHotReload = true,
                IsNativeModule = false
            };

            _runtimeUnits.Add(unit);

            if (enabled)
            {
                try
                {
                    var engine = new Engine(options =>
                    {
                        options.LimitMemory(4000000);
                        options.TimeoutInterval(TimeSpan.FromSeconds(5));
                    });

                    var eventsObj = new Dictionary<string, object>
                    {
                        ["onUpdate"] = new Action<JsValue>(handler =>
                        {
                            _onUpdateHandlers.Add(dt => engine.Invoke(handler, dt));
                        }),
                        ["onGui"] = new Action<JsValue>(handler =>
                        {
                            _onGuiHandlers.Add(() => engine.Invoke(handler));
                        }),
                        ["on"] = new Action<string, JsValue, string>((hook, handler, modId) =>
                        {
                            greg.Sdk.gregEventDispatcher.On(hook, payload => {
                                try {
                                    engine.Invoke(handler, JsValue.FromObject(engine, payload));
                                } catch(Exception e) {
                                    _logger.Error($"Event execution failed in JS: {e}");
                                }
                            }, string.IsNullOrEmpty(modId) ? unit.Id : modId);
                        }),
                        ["off"] = new Action<string>(hook => {
                            greg.Sdk.gregEventDispatcher.UnregisterAll(unit.Id);
                        })
                    };

                    var hudObj = new Dictionary<string, object>
                    {
                        ["beginPanel"] = new Action<string, float, float, float, float>(gregGameHooks.GuiBeginPanel),
                        ["label"] = new Action<string>(gregGameHooks.GuiLabel),
                        ["endPanel"] = new Action(gregGameHooks.GuiEndPanel)
                    };

                    var targetObj = new Dictionary<string, object>
                    {
                        ["raycastForward"] = new Func<float, object>(distance =>
                        {
                            var hit = gregGameHooks.RaycastForward(distance);
                            if (hit == null) return null;
                            var result = new Dictionary<string, object> {
                                ["name"] = hit.Value.Name,
                                ["distance"] = hit.Value.Distance,
                                ["point"] = new Dictionary<string, float> { ["x"] = hit.Value.Point.x, ["y"] = hit.Value.Point.y, ["z"] = hit.Value.Point.z }
                            };
                            if (hit.Value.Entity != null)
                            {
                                result["entityHandle"] = gregLuaObjectHandleRegistry.Register(hit.Value.Entity);
                            }
                            return result;
                        })
                    };

                    var payloadObj = new Dictionary<string, object>
                    {
                        ["get"] = new Func<object, string, object, object>(global::greg.Sdk.gregPayload.Get<object>)
                    };

                    var gregObj = new Dictionary<string, object>
                    {
                        ["log"] = new Action<string>(msg => _logger.Msg($"[tsjs:{unit.DisplayName}] {msg}")),
                        ["events"] = eventsObj,
                        ["hud"] = hudObj,
                        ["target"] = targetObj,
                        ["payload"] = payloadObj
                    };

                    engine.SetValue("greg", gregObj);

                    engine.Execute(File.ReadAllText(scriptPath));
                    _engines.Add(engine);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to load TS/JS script '{unit.DisplayName}': {ex}");
                }
            }
        }

        if (_runtimeUnits.Count > 0)
        {
            _logger.Msg("This mod uses external vendored libraries, all references can be found at gregframework.eu/vendored-libs");
            _logger.Msg("The Framework is using Jint by Sebastien Ros for implementing JS/TS Mod Support - https://github.com/sebastienros/jint");
        }

        _logger.Msg($"gregCore TS/JS bridge evaluated {_engines.Count} active script(s).");
        return _runtimeUnits.Count;
    }

    public IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits() => _runtimeUnits;

    public bool SetUnitEnabled(string unitId, bool enabled)
    {
        for (int i = 0; i < _runtimeUnits.Count; i++)
        {
            if (_runtimeUnits[i].Id.Equals(unitId, StringComparison.OrdinalIgnoreCase))
            {
                gregModActivationService.SetEnabled(unitId, enabled);
                return true;
            }
        }
        return false;
    }

    public int ReloadEnabledUnits() => LoadScripts();

    public void OnSceneLoaded(string sceneName) { }

    public void OnUpdate(float deltaTime)
    {
        foreach (var handler in _onUpdateHandlers)
        {
            try { handler(deltaTime); } catch { }
        }
    }

    public void OnGui()
    {
        foreach (var handler in _onGuiHandlers)
        {
            try { handler(); } catch { }
        }
    }

    public void Shutdown()
    {
        foreach (var unit in _runtimeUnits)
        {
            greg.Sdk.gregEventDispatcher.UnregisterAll(unit.Id);
        }
        _engines.Clear();
        _onUpdateHandlers.Clear();
        _onGuiHandlers.Clear();
        _runtimeUnits.Clear();
    }
}






