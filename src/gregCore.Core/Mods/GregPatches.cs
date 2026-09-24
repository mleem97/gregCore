/// <file-summary>
/// Layer:      Core (Mods)
/// Purpose:    Defensive Harmony patching by method name — the pattern every
///             mod hand-rolls (lookup, null-check, warn-if-missing instead of
///             crashing on game updates). One call, never throws.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Harmony/runtime interop; needs running game.")]
public static class GregPatches
{
    private const BindingFlags AnyInstance =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Patches type.methodName with a prefix method. Returns false (with a
    /// warning) when the target is missing — e.g. after a game update —
    /// instead of throwing.
    /// </summary>
    public static bool TryPatchPrefix(HarmonyLib.Harmony harmony, Type targetType,
        string methodName, Type patchHolder, string prefixName, string logPrefix)
    {
        return TryPatch(harmony, targetType, methodName, patchHolder, prefixName, null, logPrefix);
    }

    /// <summary>Same as TryPatchPrefix but with a postfix method.</summary>
    public static bool TryPatchPostfix(HarmonyLib.Harmony harmony, Type targetType,
        string methodName, Type patchHolder, string postfixName, string logPrefix)
    {
        return TryPatch(harmony, targetType, methodName, patchHolder, null, postfixName, logPrefix);
    }

    private static bool TryPatch(HarmonyLib.Harmony harmony, Type targetType,
        string methodName, Type patchHolder, string prefixName, string postfixName, string logPrefix)
    {
        string tag = string.IsNullOrEmpty(logPrefix) ? "GregPatches" : logPrefix;
        try
        {
            if (harmony == null || targetType == null || patchHolder == null || string.IsNullOrEmpty(methodName))
            {
                try { MelonLogger.Warning($"[{tag}] TryPatch: bad arguments for '{methodName}'."); } catch { }
                return false;
            }

            var target = targetType.GetMethod(methodName, AnyInstance | BindingFlags.Static);
            if (target == null)
            {
                try { MelonLogger.Warning($"[{tag}] Could not find {targetType.Name}.{methodName} — skipped."); } catch { }
                return false;
            }

            HarmonyMethod prefix = null;
            HarmonyMethod postfix = null;
            try
            {
                if (!string.IsNullOrEmpty(prefixName))
                {
                    var pm = patchHolder.GetMethod(prefixName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (pm != null) prefix = new HarmonyMethod(pm);
                }
                if (!string.IsNullOrEmpty(postfixName))
                {
                    var pm = patchHolder.GetMethod(postfixName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (pm != null) postfix = new HarmonyMethod(pm);
                }
            }
            catch { }
            if (prefix == null && postfix == null)
            {
                try { MelonLogger.Warning($"[{tag}] No patch method found in {patchHolder.Name} — skipped."); } catch { }
                return false;
            }

            harmony.Patch(target, prefix, postfix);
            try { MelonLogger.Msg($"[{tag}] Patched {targetType.Name}.{methodName}."); } catch { }
            return true;
        }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[{tag}] Patch failed ({methodName}): {ex.GetBaseException().Message}"); } catch { }
            return false;
        }
    }
}
