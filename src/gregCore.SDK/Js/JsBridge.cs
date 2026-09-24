using System;
using System.IO;
using System.Collections.Generic;
using Jint;
using gregCore.Core.Abstractions;
using gregCore.API;

namespace gregCore.Infrastructure.Scripting.Js;

public sealed class JsBridge : IGregLanguageBridge
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;
    private readonly Engine _engine;

    public JsBridge(IGregLogger logger, IGregEventBus eventBus)
    {
        _logger = logger.ForContext("JsBridge");
        _eventBus = eventBus;
        _engine = new Engine(options => {
            options.AllowClr();
        });
    }

    public void Initialize()
    {
        _logger.Info("JS Bridge initializing...");
        
        // Register API
        var greg = new Dictionary<string, object>();
        RegisterApi(greg);
        _engine.SetValue("greg", greg);

        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        // Policy: runtime compatibility (no mod) -> EXCLUSIVELY ./UserLibs.
        string jsDir = Path.Combine(gameRoot, "UserLibs", "Js");
        if (!Directory.Exists(jsDir)) Directory.CreateDirectory(jsDir);
        string legacyDir = Path.Combine(gameRoot, "Plugins", "Js");
        try
        {
            if (Directory.Exists(legacyDir) && Directory.GetFiles(legacyDir, "*.js").Length > 0)
                MelonLoader.MelonLogger.Warning("[gregCore][Dirs] Deprecated: JS files under ./Plugins/Js are ignored - move them to ./UserLibs/Js.");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        foreach (var file in global::gregCore.Infrastructure.IO.GregFileSystem.EnumerateFilesByExtension(jsDir, ".js"))
        {
            ExecuteScript(File.ReadAllText(file));
        }
    }

    private void RegisterApi(Dictionary<string, object> greg)
    {
        greg["logInfo"] = (Action<string>)GregAPI.LogInfo;
        greg["logWarning"] = (Action<string>)GregAPI.LogWarning;
        greg["logError"] = (Action<string>)GregAPI.LogError;

        greg["on"] = (Action<string, Jint.Native.JsValue>)((hookName, callback) => {
            GregAPI.Hooks.On(hookName, payload => {
                callback.Call(Jint.Native.JsValue.FromObject(_engine, payload));
            });
        });

        greg["fire"] = (Action<string, IDictionary<string, object>>)((hookName, data) => {
            var payload = new gregCore.Sdk.Models.GregPayload(hookName, "JsMod");
            foreach (var kvp in data) payload.Data[kvp.Key] = kvp.Value;
            GregAPI.Hooks.Fire(hookName, payload);
        });
    }

    public void ExecuteScript(string scriptContent)
    {
        try
        {
            _engine.Execute(scriptContent);
            _logger.Debug("JS script executed.");
        }
        catch (Exception ex)
        {
            _logger.Error($"[JsBridge] JS error: {ex.Message}", ex);
        }
    }
}
