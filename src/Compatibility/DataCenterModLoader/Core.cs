using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

public static class CrashLog
{
    private static string? _logPath;
    private static readonly object _lock = new();

    public static void Init(string gameRoot)
    {
        try
        {
            _logPath = Path.Combine(gameRoot, "dc_modloader_debug.log");
            var header =
                $"===== RustBridge Debug Log ====={Environment.NewLine}" +
                $"Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}{Environment.NewLine}" +
                $"========================================={Environment.NewLine}";
            File.WriteAllText(_logPath, header);
        }
        catch
        {
        }
    }

    public static void Log(string msg)
    {
        try
        {
            if (_logPath == null)
            {
                return;
            }

            lock (_lock)
            {
                File.AppendAllText(_logPath,
                    $"[{DateTime.Now:HH:mm:ss.fff}] {msg}{Environment.NewLine}");
            }
        }
        catch
        {
        }
    }

    public static void LogException(string context, Exception ex)
    {
        try
        {
            if (_logPath == null)
            {
                return;
            }

            lock (_lock)
            {
                File.AppendAllText(_logPath,
                    $"[{DateTime.Now:HH:mm:ss.fff}] EXCEPTION in {context}:{Environment.NewLine}" +
                    $"  Type: {ex.GetType().FullName}{Environment.NewLine}" +
                    $"  Message: {ex.Message}{Environment.NewLine}" +
                    $"  StackTrace:{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}" +
                    (ex.InnerException != null
                        ? $"  InnerException: {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}{Environment.NewLine}" +
                          $"  InnerStackTrace:{Environment.NewLine}{ex.InnerException.StackTrace}{Environment.NewLine}"
                        : "") +
                    Environment.NewLine);
            }
        }
        catch
        {
        }
    }
}

public class Core
{
    public static Core? Instance { get; private set; }

    public MelonLogger.Instance LoggerInstance { get; }

    private FFIBridge? _ffiBridge;
    private MultiplayerBridge? _mpBridge;
    private string _modsPath = string.Empty;
    private HarmonyLib.Harmony? _harmony;

    private float _queueDrainTimer;
    private const float QueueDrainInterval = 2f;

    public Core(MelonLogger.Instance loggerInstance)
    {
        LoggerInstance = loggerInstance;
        Instance = this;
    }

    public void Initialize()
    {
        try
        {
            CrashLog.Init(MelonEnvironment.GameRootDirectory);
            CrashLog.Log("step: CrashLog initialized");

            _modsPath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "native");

            LoggerInstance.Msg("╔══════════════════════════════════════════╗");
            LoggerInstance.Msg("║   Rust Bridge (Integrated)               ║");
            LoggerInstance.Msg("║   Rust FFI Bridge Active                 ║");
            LoggerInstance.Msg("╚══════════════════════════════════════════╝");

            if (!Directory.Exists(_modsPath))
            {
                Directory.CreateDirectory(_modsPath);
                LoggerInstance.Msg($"Created Mods/native directory: {_modsPath}");
            }

            CrashLog.Log("step: creating FFIBridge");
            _ffiBridge = new FFIBridge(LoggerInstance, _modsPath);

            CrashLog.Log("step: initializing EventDispatcher");
            EventDispatcher.Initialize(_ffiBridge, LoggerInstance);

            CrashLog.Log("step: applying Harmony patches");
            ApplyDataCenterHarmonyPatches();

            CrashLog.Log("step: initializing ModConfigSystem");
            ModConfigSystem.Initialize(LoggerInstance);

            CrashLog.Log("step: loading all mods");
            _ffiBridge.LoadAllMods();

            var mpDllPath = Path.Combine(_modsPath, "dc_multiplayer.dll");
            if (File.Exists(mpDllPath))
            {
                _mpBridge = new MultiplayerBridge(LoggerInstance);
            }

            LoggerInstance.Msg("Integrated Rust bridge initialization complete.");
            CrashLog.Log("step: Initialize complete");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("Initialize", ex);
            throw;
        }
    }

    public void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        try
        {
            _ffiBridge?.OnSceneLoaded(sceneName);
            _mpBridge?.OnSceneLoaded(sceneName);
            ModConfigSystem.OnSceneLoaded(sceneName);
            CustomEmployeeManager.ResetInjectionState();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnSceneWasLoaded", ex);
        }
    }

    public void OnUpdate()
    {
        try
        {
            _ffiBridge?.OnUpdate(Time.deltaTime);
            _mpBridge?.OnUpdate(Time.deltaTime);
            ModConfigSystem.OnUpdate(Time.deltaTime);
            CustomEmployeeManager.ReregisterSalariesIfNeeded();
            EntityManager.Update();
            CarryStateMonitor.Update();

            _queueDrainTimer += Time.deltaTime;
            if (_queueDrainTimer >= QueueDrainInterval)
            {
                _queueDrainTimer = 0f;
                try
                {
                    var technicianManager = Il2Cpp.TechnicianManager.instance;
                    if (technicianManager != null)
                    {
                        GameHooks.ForceProcessPendingQueue(technicianManager);
                    }
                }
                catch
                {
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnUpdate", ex);
        }
    }

    public void OnFixedUpdate()
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

    public void OnGUI()
    {
        // IMGUI Drawing disabled globally for Unity 6 stability.
    }

    public void OnApplicationQuit()
    {
        try
        {
            LoggerInstance.Msg("Shutting down integrated Rust bridge...");
            CrashLog.Log("step: OnApplicationQuit starting");
            EntityManager.DestroyAll();
            _mpBridge?.Shutdown();
            ModConfigSystem.Shutdown();
            _ffiBridge?.Shutdown();
            _ffiBridge?.Dispose();
            _harmony?.UnpatchSelf();
            CrashLog.Log("step: OnApplicationQuit complete");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("OnApplicationQuit", ex);
        }
    }

    private void ApplyDataCenterHarmonyPatches()
    {
        try
        {
            _harmony = new HarmonyLib.Harmony("gregcore.compat.datacenter");
            var patchTypes = typeof(Core).Assembly
                .GetTypes()
                .Where(type => type.Namespace == "DataCenterModLoader")
                .Where(type => type.GetCustomAttributes(typeof(HarmonyPatch), inherit: false).Length > 0)
                .ToArray();

            foreach (var patchType in patchTypes)
            {
                _harmony.CreateClassProcessor(patchType).Patch();
            }

            LoggerInstance.Msg($"DataCenter Harmony patches applied: {patchTypes.Length}");
            CrashLog.Log("step: Harmony patches applied successfully");
        }
        catch (Exception ex)
        {
            LoggerInstance.Error($"Failed to apply DataCenter Harmony patches: {ex.Message}");
            LoggerInstance.Msg("Continuing without full DataCenter patch support.");
            CrashLog.LogException("Harmony patching", ex);
        }
    }
}
