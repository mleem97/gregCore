using HarmonyLib;
using UnityEngine;

namespace greg.Harmony;

/// <summary>
/// Patches the Unity Application version to ensure it is correctly reported to MelonLoader and other mods,
/// as it is not properly configured in the game's Unity build.
/// </summary>
[HarmonyPatch(typeof(Application), "version", MethodType.Getter)]
public static class GregGameVersionPatch
{
    private const string GameVersion = "1.0.45.5";

    public static bool Prefix(ref string __result)
    {
        __result = GameVersion;
        return false; // Skip the original getter
    }
}
