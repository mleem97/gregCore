using HarmonyLib;
using gregSdk.Services;
using Il2Cpp;

namespace gregHarmony;

[HarmonyPatch(typeof(Interact), nameof(Interact.OnHoverOver))]
public static class GregHexViewerPatch
{
    static void Postfix(Interact __instance)
    {
        var metadata = GregMetadataService.GetMetadata(__instance.name);
        if (metadata.Count > 0)
        {
            // Logic to display metadata on the HUD
            // This would normally call a UI Manager in gregCore
        }
    }
}
