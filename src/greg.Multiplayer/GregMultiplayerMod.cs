using MelonLoader;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Multiplayer.GregMultiplayerMod), "greg.Multiplayer", "0.0.1", "mleem97")]
[assembly: MelonGame()]

namespace greg.Multiplayer
{
    /// <summary>
    /// MelonLoader 0.7.2 entry point for gregCore Multiplayer.
    /// Unity 6000.4.2f / IL2CPP / FishNet
    /// </summary>
    public class GregMultiplayerMod : MelonMod
    {
        public static GregMultiplayerMod Instance { get; private set; }
        internal static HarmonyLib.Harmony HarmonyInstance { get; private set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            LoggerInstance.Msg("[greg.Multiplayer] Initializing v0.0.1 ...");

            // Register IL2CPP types before anything else
            ClassInjector.RegisterTypeInIl2Cpp<GregRelayService>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerHud>();

            // Apply all Harmony patches
            HarmonyInstance = new HarmonyLib.Harmony("de.gregcore.multiplayer");
            HarmonyInstance.PatchAll();

            // Load config & init relay service
            var cfg = MultiplayerConfig.Load();
            GregRelayService.Initialize(cfg);

            LoggerInstance.Msg($"[greg.Multiplayer] Ready. Port={cfg.Port}, MaxPlayers={cfg.MaxPlayers}");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // Attach HUD when main game scene loads
            if (sceneName.Contains("DataCenter") || sceneName.Contains("Game"))
            {
                var go = new GameObject("GregMultiplayerHud");
                go.AddComponent<MultiplayerHud>();
                UnityEngine.Object.DontDestroyOnLoad(go);
            }
        }

        public override void OnApplicationQuit()
        {
            GregRelayService.Instance?.Shutdown();
        }
    }
}
