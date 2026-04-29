using System;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using gregCore.UI;
using gregCore.Infrastructure.UI;
using gregCore.Core.Events;
using gregCore.Core.Persistence;
using gregCore.Sdk;
using gregCore.Sdk.Language;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.1.0", "TeamGreg")]
[assembly: MelonColor(255, 0, 191, 165)] // Teal
[assembly: MelonPriority(-1000)] // Load first!

namespace gregCore.Core
{
    /// <summary>
    /// Central MelonMod entry point for gregCore.
    /// Provides the modding framework backbone, IL2CPP type registration,
    /// UI Toolkit initialization, and gregExt discovery.
    /// </summary>
    public sealed class GregCoreMod : MelonMod
    {
        public static GregCoreMod Instance { get; private set; }
        public static IGregAPI? PublicAPI { get; private set; }
        private static bool _lateInitCompleted;

        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonLogger.Msg("--- Framework Boot v1.1.0 ---");
            
            // Register persistent IL2CPP components
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<GregHardwareID>();
                MelonLogger.Msg("[gregCore] IL2CPP types registered.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] IL2CPP type registration failed: {ex.Message}");
            }
            
            // Initialize UI Toolkit root
            try
            {
                GregUIManager.Initialize();
                GregDevConsole.Initialize();
                MelonLogger.Msg("[gregCore] UI Toolkit root initialized.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UI initialization failed: {ex.Message}");
            }
        }

        public override void OnUpdate()
        {
            // Deferred late initialization (Load-Order Safety)
            if (!_lateInitCompleted)
            {
                _lateInitCompleted = true;
                try
                {
                    DiscoverGregExtHosts();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] gregExt discovery failed: {ex.Message}");
                }

                try
                {
                    var modsDir = System.IO.Path.Combine(global::MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "Mods", "Scripts");
                    GregLanguageRegistry.ScanAndActivate(modsDir);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] Language host activation failed: {ex.Message}");
                }

                MelonLogger.Msg("[gregCore] Framework initialization complete.");
            }

            try
            {
                GregLanguageRegistry.OnUpdate(Time.deltaTime);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Update callback failed: {ex.Message}");
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                if (sceneName != "MainMenu")
                {
                    GregUIOverrideManager.HideVanillaUI();
                }
                GregLanguageRegistry.OnSceneLoaded(sceneName);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Scene load callback failed: {ex.Message}");
            }
        }

        public override void OnApplicationQuit()
        {
            try
            {
                GregLanguageRegistry.Shutdown();
                GregUIManager.Shutdown();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Shutdown failed: {ex.Message}");
            }
            base.OnApplicationQuit();
        }

        /// <summary>
        /// Discovers IGregLanguageHost implementations in assemblies named gregExt.*
        /// or marked with [GregExtension] and registers them dynamically.
        /// </summary>
        private static void DiscoverGregExtHosts()
        {
            var extAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name?.StartsWith("gregExt.") == true);

            foreach (var asm in extAssemblies)
            {
                try
                {
                    var hostTypes = asm.GetTypes()
                        .Where(t => typeof(IGregLanguageHost).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var hostType in hostTypes)
                    {
                        var instance = (IGregLanguageHost?)Activator.CreateInstance(hostType);
                        if (instance != null)
                        {
                            GregLanguageRegistry.RegisterHost(instance.HostId, instance);
                            MelonLogger.Msg($"[gregCore] gregExt host registered: {instance.HostId} ({instance.HostName})");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    MelonLogger.Warning($"[gregCore] Could not load types from {asm.GetName().Name}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] gregExt discovery error in {asm.GetName().Name}: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Assembly resolution shim to redirect legacy mod loader references to gregCore.
    /// </summary>
    public sealed class DataCenterModLoaderMod : MelonMod
    {
        static DataCenterModLoaderMod()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.StartsWith("DataCenterModLoader") || args.Name.StartsWith("gregCore"))
                {
                    return typeof(DataCenterModLoaderMod).Assembly;
                }
                return null;
            };
        }
    }
}
