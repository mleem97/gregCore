using HarmonyLib;
using greg.Sdk.Services;
using Il2Cpp;

namespace greg.Harmony;

[HarmonyPatch(typeof(Server), nameof(Server.OnLoadingStarted))]
public static class GregServerModelPatch
{
    static void Postfix(Server __instance)
    {
        var modelService = new GregModelOverrideService(); // Simplified for now
        if (modelService.TryGetOverride(__instance.name, out var manifest))
        {
            GregDiagnostics.LogContentWarning("ModelOverride", __instance.name, $"Applying override: {manifest.ReplacementPath}");
            // In a real scenario, we would trigger the Unity AssetBundle loader here
        }
    }
}
