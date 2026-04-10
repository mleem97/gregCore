using System.Collections.Generic;
using MelonLoader;

namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// Adapter that represents native Rust support inside the shared language host.
/// Actual loading/execution remains delegated to <see cref="FFIBridge"/>.
/// </summary>
public sealed class RustLanguageBridgeAdapter : IGregLanguageBridge
{
    private static readonly string[] Extensions = { ".dll", ".greg", ".gregr", ".gregl", ".gregp" };

    private readonly MelonLogger.Instance _logger;
    private readonly string _modsPath;
    private readonly FFIBridge _ffiBridge;

    public RustLanguageBridgeAdapter(MelonLogger.Instance logger, string modsPath, FFIBridge ffiBridge)
    {
        _logger = logger;
        _modsPath = modsPath;
        _ffiBridge = ffiBridge;
    }

    public string LanguageName => "rust/native";

    public IReadOnlyList<string> ScriptExtensions => Extensions;

    public void Initialize()
    {
        _logger.Msg($"gregCore Rust/native bridge delegated to FFIBridge: {_modsPath}");
    }

    public int LoadScripts()
    {
        return 0;
    }

    public void OnSceneLoaded(string sceneName)
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }

    public void Shutdown()
    {
    }
}
