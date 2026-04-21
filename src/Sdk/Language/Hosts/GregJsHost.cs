using System;
using System.IO;
using Jint;
using MelonLoader;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregJsHost : IGregLanguageHost
{
    private Engine? _engine;

    public Language Language => Language.JavaScript;
    public string HostName => nameof(GregJsHost);
    public bool IsActive { get; private set; }

    public bool IsDependencyAvailable(out string detail)
    {
        detail = "JS-Runtime-Binding (Jint)";
        return typeof(Engine) != null;
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive)
        {
            return;
        }

        _engine = new Engine(cfg => cfg.LimitMemory(4_000_000));
        _engine.SetValue("greg_log", new Action<string>(msg => MelonLogger.Msg($"[gregCore][JS] {msg}")));

        foreach (var jsFile in Directory.EnumerateFiles(modsScriptsDir, "*.js", SearchOption.AllDirectories))
        {
            try
            {
                var code = File.ReadAllText(jsFile);
                _engine.Execute(code);
                MelonLogger.Msg($"[gregCore] JS script loaded: {Path.GetFileName(jsFile)}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] JS script error in {jsFile}: {ex}");
            }
        }

        foreach (var tsFile in Directory.EnumerateFiles(modsScriptsDir, "*.ts", SearchOption.AllDirectories))
        {
            MelonLogger.Warning($"[gregCore] TypeScript file found but no TS transpiler is configured: {Path.GetFileName(tsFile)}");
        }

        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
    }

    public void OnSceneLoaded(string sceneName)
    {
    }

    public void Shutdown()
    {
        _engine = null;
        IsActive = false;
    }
}
