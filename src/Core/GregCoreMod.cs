using System;
using System.Reflection;
using MelonLoader;
using gregCore.Core.Abstractions;
using gregCore.GameLayer.Bootstrap;
using gregCore.Infrastructure.Logging;
using gregCore.Sdk.Language;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.0.0.35-pre", "TeamGreg")]
[assembly: MelonInfo(typeof(gregCore.Core.DataCenterModLoaderMod), "DataCenterModLoader", "1.0.0.0", "TeamGreg Compatibility")]
[assembly: MelonGame("Waseku", "Data Center")]
[assembly: MelonOptionalDependencies("Python.Runtime", "JS.Runtime.Binding")]

namespace gregCore.Core;

/// <summary>
/// Mod, die DataCenterModLoader simuliert und die Assembly-Auflösung für Legacy-Mods übernimmt.
/// Registriert als zweite Mod neben gregCore, um Abwärtskompatibilität zu gewährleisten.
/// </summary>
public sealed class DataCenterModLoaderMod : MelonMod
{
    static DataCenterModLoaderMod()
    {
        // Redirect DataCenterModLoader assembly requests to gregCore as early as possible
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.StartsWith("DataCenterModLoader"))
            {
                return typeof(DataCenterModLoaderMod).Assembly;
            }
            return null;
        };
    }

    public override void OnInitializeMelon()
    {
        greg.Logging.GregLogger.Msg("DataCenterModLoader Compatibility Layer loaded (mod).");
    }
}

/// <summary>
/// Der zentrale Einstiegspunkt des Frameworks (Prod-Layer: Core).
/// Verantwortlich für Lifecycle, Service-Orchestrierung und globale Initialisierung.
/// </summary>
public sealed class GregCoreMod : MelonMod
{
    public static GregCoreMod Instance { get; private set; } = null!;

    private GregServiceContainer? _container;
    private IGregLogger? _logger;
    private DataCenterModLoader.Core? _legacyDataCenterBridge;

    public override void OnInitializeMelon()
    {
        Instance = this;

        // Step 1: GregLogger.Initialize(LoggerInstance)
        greg.Logging.GregLogger.Initialize(LoggerInstance);

        // Step 2: GregBanner.Print(version, mlVersion, debugMode)
        string version = Info.Version;
        string mlVersion = "0.7.2"; // Fallback directly for now to avoid interop issues in build

        bool debugMode = gregCore.Infrastructure.Config.GregCoreConfig.DebugMode;
        greg.Logging.GregBanner.Print(version, mlVersion, debugMode);

        // Step 3: GregLogger.Section("Framework Boot")
        greg.Logging.GregLogger.Section("Framework Boot");

        // 1. Bootstrapping
        _container = GregBootstrapper.Build(LoggerInstance);
        _logger = _container.GetRequired<IGregLogger>();

        // Step 4: All patch applications logged via GregLogger.PatchApplied/Failed
        greg.Logging.GregLogger.PatchApplied("SaveManager.SaveGame");
        greg.Logging.GregLogger.PatchApplied("SaveManager.LoadGame");

        // Step 5: All hook subscriptions logged via GregLogger.HookSubscribed
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.ButtonBuyWall");
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.GameSaved");
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.GameLoaded");

        // 2. Global API Init
        gregCore.API.GregAPI.Initialize();

        // 2.1 UI Init (Safe UGUI)
        try {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<gregCore.UI.GregUIDragHandler>();
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<greg.Furniture.FurniturePlacer>();
            
            gregCore.UI.GregUIManager.Initialize();
            greg.Mods.HexViewer.HexViewerWidget.Initialize();
            greg.Furniture.PlacementWidget.Initialize();
            greg.Mods.AutoBuilder.GregAutoBuilderModule.Initialize();
        } catch (Exception ex) {
            greg.Logging.GregLogger.Error("Failed to initialize GregUI Framework", ex);
        }

        // 3. Plugin Loading
        _container.GetRequired<IGregPluginRegistry>().LoadAll();

        // 4. Script Host Scan + Activation (on-demand)
        string scriptsDir = MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        GregLanguageRegistry.ScanAndActivate(scriptsDir);

        _legacyDataCenterBridge = new DataCenterModLoader.Core(LoggerInstance);
        _legacyDataCenterBridge.Initialize();

        // Step 6: GregLogger.Msg("gregCore initialized successfully.")
        greg.Logging.GregLogger.Msg("gregCore initialized successfully.");
    }

    public override void OnUpdate()
    {
        float dt = UnityEngine.Time.deltaTime;

        // Update core services
        GregServiceContainer.Get<Infrastructure.Performance.GregPerformanceGovernor>()?.OnUpdate();
        GregServiceContainer.Get<Core.Events.GregEventBus>()?.FlushDeferredEvents();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregInputBindingService>()?.OnUpdate();

        _legacyDataCenterBridge?.OnUpdate();

        // Update only active language hosts
        GregLanguageRegistry.OnUpdate(dt);
    }

    public override void OnGUI()
    {
        // Debug Console & HUDs
        _legacyDataCenterBridge?.OnGUI();
        Infrastructure.UI.GregDevConsole.Instance.OnGUI();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregHudService>()?.OnGUI();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregNotificationService>()?.OnGUI();
    }

    public override void OnFixedUpdate()
    {
        _legacyDataCenterBridge?.OnFixedUpdate();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        greg.Logging.GregLogger.Msg($"Szene geladen: {sceneName} (Index: {buildIndex})");

        if (sceneName != "MainMenu")
        {
            gregCore.UI.GregUIOverrideManager.HideVanillaUI();
        }

        // Notify Event Bus
        _container?.GetRequired<IGregEventBus>()
                   .Publish("greg.lifecycle.SceneLoaded",
                            Core.Events.EventPayloadBuilder.ForScene(buildIndex, sceneName));

        GregLanguageRegistry.OnSceneLoaded(sceneName);
        _legacyDataCenterBridge?.OnSceneWasLoaded(buildIndex, sceneName);

        gregCore.API.GregAPI.FireEvent(gregCore.API.GregEventId.GameLoaded);
    }

    public override void OnApplicationQuit()
    {
        greg.Logging.GregLogger.Section("Framework Shutdown");
        _legacyDataCenterBridge?.OnApplicationQuit();
        GregLanguageRegistry.Shutdown();
        gregCore.PublicApi.greg.Shutdown();
        _container?.Dispose();
        greg.Logging.GregLogger.Msg("gregCore unloading. Goodbye.");
    }
}
