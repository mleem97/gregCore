using System;
using HarmonyLib;
using greg.Sdk.Services;
using Il2Cpp;

namespace greg.Harmony;

[HarmonyPatch(typeof(Interact), nameof(Interact.OnHoverOver))]
public static class GregHexViewerPatch
{
    static bool Prepare()
    {
        return IsExplicitlyEnabledByEnvironment();
    }

    [ThreadStatic]
    private static bool _isProcessing;

    private static bool IsExplicitlyEnabledByEnvironment()
    {
        var value = Environment.GetEnvironmentVariable("GREG_ENABLE_HOVER_PATCH");
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase);
    }

    static void Postfix(Interact __instance)
    {
        if (_isProcessing || __instance is null)
        {
            return;
        }

        try
        {
            _isProcessing = true;
            var metadata = GregMetadataService.GetMetadata("Il2Cpp.Interact");
            if (metadata is { Count: > 0 })
            {
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            _isProcessing = false;
        }
    }
}
