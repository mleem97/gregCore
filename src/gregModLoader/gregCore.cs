using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using HarmonyLib;
using greg.Core.Plugins;
using greg.Exporter;
using greg.Core.Scripting;
using greg.Core.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(greg.Core.gregCoreLoader), "gregCore", greg.Core.gregReleaseVersion.Current, "MLeeM97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Core;

// file-based crash logger, never throws
public static class CrashLog
{
    private static string _frameworkDirectory;
    private static string _logPath;
    private static string _errorLogPath;
    private static readonly object _lock = new();

    public static void Init(string gameRoot)
    {
        try
        {
            _frameworkDirectory = Path.Combine(gameRoot, "gregCore");
            Directory.CreateDirectory(_frameworkDirectory);

            _logPath = Path.Combine(_frameworkDirectory, "gregcore-debug.log");
            _errorLogPath = Path.Combine(_frameworkDirectory, "gregcore-errors.log");

            var header =
                $"===== gregCore Debug Log ====={Environment.NewLine}" +
                $"Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}{Environment.NewLine}" +
                $"========================================={Environment.NewLine}";

            File.WriteAllText(_logPath, header);
            File.AppendAllText(_errorLogPath, header);
        }
        catch { }
    }

    public static void Log(string msg)
    {
        try
        {
            if (_logPath == null) return;
            lock (_lock)
            {
                File.AppendAllText(_logPath,
                    $"[{DateTime.Now:HH:mm:ss.fff}] {msg}{Environment.NewLine}");
            }
        }
        catch { }
    }

    public static void LogException(string context, Exception ex)
    {
        try
        {
            if (_logPath == null) return;
            lock (_lock)
            {
                string exceptionBlock =
                    $"[{DateTime.Now:HH:mm:ss.fff}] EXCEPTION in {context}:{Environment.NewLine}" +
                    $"  Type: {ex.GetType().FullName}{Environment.NewLine}" +
                    $"  Message: {ex.Message}{Environment.NewLine}" +
                    $"  StackTrace:{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}" +
                    (ex.InnerException != null
                        ? $"  InnerException: {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}{Environment.NewLine}" +
                          $"  InnerStackTrace:{Environment.NewLine}{ex.InnerException.StackTrace}{Environment.NewLine}"
                        : "") +
                    Environment.NewLine;

                File.AppendAllText(_logPath, exceptionBlock);

                if (!string.IsNullOrEmpty(_errorLogPath))
                    File.AppendAllText(_errorLogPath, exceptionBlock);
            }
        }
        catch { }
    }

    public static void LogError(string category, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(_errorLogPath))
                return;

            lock (_lock)
            {
                string safeCategory = string.IsNullOrWhiteSpace(category) ? "general" : category.Trim();
                File.AppendAllText(_errorLogPath,
                    $"[{DateTime.Now:HH:mm:ss.fff}] ERROR [{safeCategory}] {message}{Environment.NewLine}");
            }
        }
        catch { }
    }

    public static void LogModError(string modId, string message, Exception ex = null)
    {
        string safeModId = string.IsNullOrWhiteSpace(modId) ? "unknown-mod" : modId.Trim();
        LogError($"mod:{safeModId}", message ?? "(no message)");

        if (ex != null)
            LogException($"mod:{safeModId}", ex);
    }
}

public class gregCoreLoader : MelonMod
{
    public static gregCoreLoader Instance { get; private set; }
    public static gregMultiplayerBridge Multiplayer { get; private set; }

    private gregFfiBridge _ffiBridge;
    private gregLanguageBridgeHost _languageBridgeHost;
    private gregLangserverCompatRuntime _langserverCompatRuntime;
    private string _modsPath;
    private bool _globalExceptionHooksInstalled;
    private float _nextHarmonyGuardAt;
    private float _nextHotReloadAt;
    private bool _diagnosticDisablePatches;
    private bool _diagnosticDisableBridges;

#if DEBUG
    private readonly Il2CppEventCatalogService _il2CppEventCatalog = new Il2CppEventCatalogService();
    private readonly Il2CppGameplayIndexService _il2CppGameplayIndex = new Il2CppGameplayIndexService();
    private readonly RuntimeHookService _runtimeHookService = new RuntimeHookService();
    private readonly GameSignalSnapshotService _gameSignalSnapshot = new GameSignalSnapshotService();
#endif

    public override void OnInitializeMelon()
    {
        try
        {
            Instance = this;

            ApplyDiagnosticSwitches();

            CrashLog.Init(MelonEnvironment.GameRootDirectory);
            CrashLog.Log("step: CrashLog initialized");
            gregModActivationService.Initialize(LoggerInstance);
            CrashLog.Log("step: gregModActivationService initialized");

            ModSaveCompatibilityService.Initialize();

            _modsPath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "RustMods");

            LoggerInstance.Msg("╔══════════════════════════════════════════╗");
            LoggerInstance.Msg("║   gregCore Runtime                       ║");
            LoggerInstance.Msg("║   Native + Script Bridges Active         ║");
            LoggerInstance.Msg("╚══════════════════════════════════════════╝");

            if (!Directory.Exists(_modsPath))
            {
                Directory.CreateDirectory(_modsPath);
                LoggerInstance.Msg($"Created Mods/RustMods directory: {_modsPath}");
            }

            CrashLog.Log("step: creating gregFfiBridge");
            _ffiBridge = new gregFfiBridge(LoggerInstance, _modsPath);

            CrashLog.Log("step: creating greg language bridge host");
            if (_diagnosticDisableBridges)
            {
                LoggerInstance.Warning("[Diagnostic] Language bridges disabled via --greg-disable-bridges.");
                CrashLog.Log("diagnostic: language bridges disabled");
            }
            else
            {
                _languageBridgeHost = new gregLanguageBridgeHost(LoggerInstance, _modsPath, _ffiBridge);
                _languageBridgeHost.Initialize();
                _languageBridgeHost.LoadAll();
            }

            CrashLog.Log("step: initializing greg.Langserver.Compat runtime");
            _langserverCompatRuntime = gregLangserverCompatRuntime.Initialize(LoggerInstance);

            InstallGlobalExceptionHooks();

            CrashLog.Log("step: initializing EventDispatcher");
            EventDispatcher.Initialize(_ffiBridge, LoggerInstance);

            CrashLog.Log("step: applying Harmony patches");
            if (_diagnosticDisablePatches)
            {
                LoggerInstance.Warning("[Diagnostic] Harmony patches disabled via --greg-disable-patches.");
                CrashLog.Log("diagnostic: harmony patches disabled");
            }
            else
            {
                try
                {
                    int applied = ApplyHarmonyPatchesWithDiagnostics();
                    LoggerInstance.Msg($"Harmony patches applied (classes): {applied}");
                    CrashLog.Log($"step: Harmony patches applied successfully ({applied} classes)");
                    _langserverCompatRuntime?.DetectHarmonyConflicts(HarmonyInstance?.Id ?? string.Empty);
                }
                catch (Exception ex)
                {
                    LoggerInstance.Error($"Failed to apply Harmony patches: {ex.Message}");
                    LoggerInstance.Msg("Continuing without full event support.");
                    CrashLog.LogException("Harmony patching", ex);
                }
            }

            CrashLog.Log("step: loading all mods");
            _ffiBridge.LoadAllMods();

            // gregApi is now fully integrated into gregCore.
            // All game API surfaces (Player, Network, Time, Localisation, UI, World)
            // are accessible via the consolidated gregApi class.
            CrashLog.Log("step: gregApi integrated");

            CrashLog.Log("step: notifying registered greg plugins");
            gregRegistry.NotifyFrameworkReady();

            LoggerInstance.Msg("Modloader initialization complete.");
            LoggerInstance.Msg("Hotkeys: Ctrl+Shift+R reload enabled mods (gregMain Menu only)");
            LoggerInstance.Msg("API: Access game systems via gregApi (Player, Network, Time, Localisation, UI, World)");
            ModFramework.Events.Publish(new ModInitializedEvent(DateTime.UtcNow, greg.Core.gregReleaseVersion.Current));

#if DEBUG
            try
            {
                string diagnosticsDir = Path.Combine(MelonEnvironment.GameRootDirectory, "gregCore", "Diagnostics");
                string snapshotPath = _gameSignalSnapshot.ExportAll(
                    diagnosticsDir,
                    _il2CppEventCatalog,
                    _il2CppGameplayIndex,
                    _runtimeHookService);
                LoggerInstance.Msg($"gregCore: IL2CPP diagnostics snapshot written to {snapshotPath}");
            }
            catch (Exception ex)
            {
                LoggerInstance.Warning($"gregCore: IL2CPP diagnostics snapshot failed: {ex.Message}");
                CrashLog.LogException("Il2CppDiagnostics", ex);
            }
#endif
            CrashLog.Log("step: OnInitializeMelon complete");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnInitializeMelon", ex);
            CrashLog.LogError("gregCoreLoader", "Initialization failed. See exception details above.");
            throw;
        }
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        try
        {
            _ffiBridge?.OnSceneLoaded(sceneName);
            _languageBridgeHost?.OnSceneLoaded(sceneName);
            gregUiExtensionBridge.OnSceneLoaded(sceneName);

            // Initialize extra technician hiring (safe to call multiple times)
            TechnicianHiring.Initialize();

            // Re-register salaries for previously hired custom employees
            CustomEmployeeManager.ReregisterSalariesIfNeeded();

            // Export hires snapshot for framework clients/mod tooling
            HireRosterService.ExportAvailableHiresSnapshot();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnSceneWasLoaded", ex);
        }
    }

    public override void OnUpdate()
    {
        try
        {
            _ffiBridge?.OnUpdate(Time.deltaTime);
            _languageBridgeHost?.OnUpdate(Time.deltaTime);
            HandleMainMenuHotReload();
            HandleModConfigHotkey();

            if (Time.time >= _nextHarmonyGuardAt)
            {
                _nextHarmonyGuardAt = Time.time + 5f;
                if (!_diagnosticDisablePatches)
                {
                    bool priorityStable = _langserverCompatRuntime?.EnsureFrameworkPatchPresence(HarmonyInstance?.Id ?? string.Empty) ?? true;
                    if (!priorityStable)
                    {
                        CrashLog.Log("Harmony guard detected missing framework owners. Reapplying framework patches.");
                        ApplyHarmonyPatchesWithDiagnostics();
                        _langserverCompatRuntime?.DetectHarmonyConflicts(HarmonyInstance?.Id ?? string.Empty);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnUpdate", ex);
        }
    }

    private void HandleModConfigHotkey()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.f6Key.wasPressedThisFrame)
        {
            gregModConfigManager.Toggle(true);
        }
    }

    private void HandleMainMenuHotReload()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        bool ctrlPressed = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
        bool shiftPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        if (!ctrlPressed || !shiftPressed || !keyboard.rKey.wasPressedThisFrame)
            return;

        if (Time.unscaledTime < _nextHotReloadAt)
            return;

        _nextHotReloadAt = Time.unscaledTime + 0.75f;

        var activeScene = SceneManager.GetActiveScene();
        bool isMainMenu = string.Equals(activeScene.name, "MainMenu", StringComparison.OrdinalIgnoreCase);
        if (!isMainMenu)
        {
            LoggerInstance.Warning("Hotload is gregMain Menu only. Return to MainMenu and press Ctrl+Shift+R.");
            return;
        }

        try
        {
            int loaded = _languageBridgeHost?.ReloadHotloadableUnits() ?? 0;
            LoggerInstance.Msg($"Hotload complete. Reloaded runtime units: {loaded}");
        }
        catch (Exception ex)
        {
            LoggerInstance.Error($"Hotload failed: {ex.Message}");
            CrashLog.LogException("HandleMainMenuHotReload", ex);
        }
    }

    private int ApplyHarmonyPatchesWithDiagnostics()
    {
        int appliedClasses = 0;
        var assembly = typeof(gregCoreLoader).Assembly;
        var types = assembly.GetTypes();

        for (int index = 0; index < types.Length; index++)
        {
            var type = types[index];
            if (!Attribute.IsDefined(type, typeof(HarmonyPatch), inherit: true))
                continue;

            try
            {
                HarmonyInstance.CreateClassProcessor(type).Patch();
                appliedClasses++;
            }
            catch (Exception ex)
            {
                string typeName = type.FullName ?? type.Name;
                LoggerInstance.Error($"Harmony patch failed for '{typeName}': {ex.Message}");
                CrashLog.LogException($"Harmony patching '{typeName}'", ex);
            }
        }

        return appliedClasses;
    }

    public override void OnFixedUpdate()
    {
        try
        {
            _ffiBridge?.OnFixedUpdate(Time.fixedDeltaTime);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnFixedUpdate", ex);
        }
    }

    public override void OnGUI()
    {
        try
        {
            _languageBridgeHost?.OnGui();
            gregUiExtensionBridge.DrawGui();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnGUI", ex);
        }
    }

    public override void OnApplicationQuit()
    {
        try
        {
            LoggerInstance.Msg("Shutting down modloader...");
            CrashLog.Log("step: OnApplicationQuit starting");
            // UiExtensionBootstrap.UnregisterBuiltInHandlers();
            UninstallGlobalExceptionHooks();
            _ffiBridge?.Shutdown();
            _languageBridgeHost?.Shutdown();
            _ffiBridge?.Dispose();
            CrashLog.Log("step: OnApplicationQuit complete");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnApplicationQuit", ex);
        }
    }

    private void InstallGlobalExceptionHooks()
    {
        if (_globalExceptionHooksInstalled)
            return;

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        _globalExceptionHooksInstalled = true;
        CrashLog.Log("step: global exception hooks installed");
    }

    private void UninstallGlobalExceptionHooks()
    {
        if (!_globalExceptionHooksInstalled)
            return;

        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        _globalExceptionHooksInstalled = false;
        CrashLog.Log("step: global exception hooks removed");
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception ex)
        {
            CrashLog.LogException("AppDomain.CurrentDomain.UnhandledException", ex);
            CrashLog.LogError("runtime", "Unhandled exception captured from AppDomain.");
            return;
        }

        CrashLog.LogError("runtime", "Unhandled non-Exception object captured from AppDomain.");
    }

    private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
    {
        CrashLog.LogException("TaskScheduler.UnobservedTaskException", args.Exception);
        CrashLog.LogError("runtime", "Unobserved task exception captured.");
        args.SetObserved();
    }

    public static void RegisterMultiplayerBridge(gregMultiplayerBridge bridge)
    {
        Multiplayer = bridge;
    }

    public static void UnregisterMultiplayerBridge(gregMultiplayerBridge bridge)
    {
        if (ReferenceEquals(Multiplayer, bridge))
            Multiplayer = null;
    }

    public System.Collections.Generic.IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits()
    {
        return _languageBridgeHost?.GetRuntimeUnits() ?? Array.Empty<gregRuntimeUnit>();
    }

    public bool SetRuntimeUnitEnabled(string unitId, bool enabled)
    {
        return _languageBridgeHost?.SetUnitEnabled(unitId, enabled) ?? false;
    }

    public int ReloadRuntimeUnits()
    {
        return _languageBridgeHost?.ReloadHotloadableUnits() ?? 0;
    }

    private void ApplyDiagnosticSwitches()
    {
        try
        {
            string[] args = Environment.GetCommandLineArgs();

            _diagnosticDisablePatches = HasArg(args, "--greg-disable-patches") || IsEnvEnabled("GREG_DISABLE_PATCHES");
            _diagnosticDisableBridges = HasArg(args, "--greg-disable-bridges") || IsEnvEnabled("GREG_DISABLE_BRIDGES");

            if (_diagnosticDisablePatches || _diagnosticDisableBridges)
            {
                LoggerInstance.Warning($"[Diagnostic] Active switches: disablePatches={_diagnosticDisablePatches}, disableBridges={_diagnosticDisableBridges}");
                CrashLog.Log($"diagnostic switches: disablePatches={_diagnosticDisablePatches}, disableBridges={_diagnosticDisableBridges}");
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ApplyDiagnosticSwitches", ex);
        }
    }

    private static bool HasArg(string[] args, string flag)
    {
        if (args == null || args.Length == 0)
            return false;

        for (int index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], flag, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static bool IsEnvEnabled(string key)
    {
        string value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value == "1"
               || value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
               || value.Equals("on", StringComparison.OrdinalIgnoreCase);
    }
}





