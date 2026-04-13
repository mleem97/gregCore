#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
                        ["updateJadeBox"] = new Action<string, string, object>((title, subHeader, entries) => {
                            var list = new List<greg.Sdk.Services.GregMetadataEntry>();
                            // Handle JS object/array to C# list
                            if (entries is IEnumerable<object> e) {
                                foreach(var item in e) {
                                    if (item is IDictionary<string, object> d) {
                                        string label = d.ContainsKey("label") ? d["label"]?.ToString() : "";
                                        string val = d.ContainsKey("value") ? d["value"]?.ToString() : "";
                                        ColorUtility.TryParseHtmlString(d.ContainsKey("color") ? d["color"]?.ToString() : "#FFFFFF", out var c);
                                        list.Add(new greg.Sdk.Services.GregMetadataEntry(label, val, c));
                                    }
                                }
                            }
                            greg.Sdk.Services.GregHudService.UpdateJadeBox(title, subHeader, list);
                        }),
                        ["hideJadeBox"] = new Action(greg.Sdk.Services.GregHudService.HideJadeBox)
                    };

                    var targetObj = new Dictionary<string, object>
                    {
                        ["getTargetInfo"] = new Func<float, object>(distance =>
                        {
                            var info = greg.Sdk.Services.GregTargetingService.GetTargetInfo(distance);
                            return new Dictionary<string, object> {
                                ["type"] = info.TargetType.ToString(),
                                ["name"] = info.Name,
                                ["distance"] = info.Distance,
                                ["hitPoint"] = new Dictionary<string, float> { ["x"] = info.HitPoint.x, ["y"] = info.HitPoint.y, ["z"] = info.HitPoint.z }
                            };
                        })
                    };

                    var metadataObj = new Dictionary<string, object>
                    {
                        ["getMetadata"] = new Func<float, object>(distance =>
                        {
                            var info = greg.Sdk.Services.GregTargetingService.GetTargetInfo(distance);
                            var entries = greg.Sdk.Services.GregComponentMetadataService.GetMetadata(info);
                            var jsEntries = new List<object>();
                            foreach(var entry in entries) {
                                jsEntries.Add(new Dictionary<string, object> {
                                    ["label"] = entry.Label,
                                    ["value"] = entry.Value,
                                    ["color"] = $"#{ColorUtility.ToHtmlStringRGB(entry.ValueColor)}"
                                });
                            }
                            return new Dictionary<string, object> {
                                ["title"] = info.TargetType == greg.Sdk.Services.GregTargetType.None ? "SCANNING..." : info.TargetType.ToString().ToUpper(),
                                ["subHeader"] = info.TargetType == greg.Sdk.Services.GregTargetType.None ? "" : "TELEMETRY",
                                ["entries"] = jsEntries
                            };
                        })
                    };

                    var gregObj = new Dictionary<string, object>
                    {
                        ["log"] = new Action<string>(msg => _logger.Msg($"[tsjs:{unit.DisplayName}] {msg}")),
                        ["events"] = eventsObj,
                        ["hud"] = hudObj,
                        ["target"] = targetObj,
                        ["metadata"] = metadataObj
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







