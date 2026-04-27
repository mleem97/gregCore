using System;
using MelonLoader;
using UnityEngine;
using gregCore.UI;
using gregCore.Infrastructure.UI;
using gregCore.Core.Events;
using gregCore.Core.Persistence;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.1.0", "TeamGreg")]
[assembly: MelonColor(255, 0, 191, 165)] // Teal
[assembly: MelonPriority(-1000)] // Load first!

namespace gregCore.Core
{
    public sealed class GregCoreMod : MelonMod
    {
        public static GregCoreMod Instance { get; private set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonLogger.Msg("--- Framework Boot v1.1.0 ---");
            
            // Register persistent components
            ClassInjector.RegisterTypeInIl2Cpp<GregHardwareID>();
            ClassInjector.RegisterTypeInIl2Cpp<GregUIDragHandler>();
            
            // Initialize Core Systems
            GregUIManager.Initialize();
            GregDevConsole.Initialize();
            
            MelonLogger.Msg("gregCore initialized successfully.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenu")
            {
                GregUIOverrideManager.HideVanillaUI();
            }
        }

        public override void OnGUI()
        {
            Sdk.Language.GregLanguageRegistry.OnGUI();
        }
    }

    public sealed class DataCenterModLoaderMod : MelonMod
    {
        static DataCenterModLoaderMod()
        {
            // Redirect ALL gregCore and DataCenterModLoader requests to this assembly
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
