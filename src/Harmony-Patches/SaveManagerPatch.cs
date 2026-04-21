using System;
using HarmonyLib;
using greg.SaveEngine;

namespace greg.HarmonyPatches
{
    // WARNING: Due to MISSING.md context, the actual class signature might vary slightly.
    // [HarmonyPatch(typeof(Il2Cpp.SaveManager), nameof(Il2Cpp.SaveManager.SaveGame))]
    public static class SaveManagerPatch
    {
        // [HarmonyPostfix]
        public static void SaveGamePostfix()
        {
            if (frameworkSdk.GregFeatureGuard.IsEnabled("SaveEngine.Write"))
            {
                GregSaveEngine.Instance?.SaveAll();
            }
        }
    }

    // [HarmonyPatch(typeof(Il2Cpp.SaveManager), nameof(Il2Cpp.SaveManager.LoadGame))]
    public static class LoadManagerPatch
    {
        // [HarmonyPostfix]
        public static void LoadGamePostfix(string filePath)
        {
            bool isVanilla = GregVanillaDetector.CheckIfVanillaSave(filePath);
            frameworkSdk.GregFeatureGuard.SetVanillaSaveStatus(isVanilla);

            if (isVanilla)
            {
                GregSaveNotifier.NotifyVanillaDetect();
            }
            else
            {
                GregSaveEngine.Instance?.LoadAll();
            }
        }
    }
}
