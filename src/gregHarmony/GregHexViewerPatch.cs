using System;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using greg.Sdk;
using greg.Core;
using Il2Cpp;

namespace gregCoreSDK.Harmony;

[HarmonyPatch(typeof(Interact), nameof(Interact.OnHoverOver))]
public static class GregHexViewerPatch
{
    private const string HexViewerTargetUpdatedHook = "greg.HEXVIEWER.TargetUpdated";
    private const float MaxHoverDistance = 12f;

    static bool Prepare()
    {
        return IsExplicitlyEnabledByEnvironment();
    }

    [ThreadStatic]
    private static bool _isProcessing;

    private static string _lastHoverTargetKey = string.Empty;
    private static float _lastHoverEmitAt;

    private static bool IsExplicitlyEnabledByEnvironment()
    {
        var disableValue = Environment.GetEnvironmentVariable("GREG_DISABLE_HOVER_PATCH");
        if (!string.IsNullOrWhiteSpace(disableValue) && IsTruthy(disableValue))
        {
            return false;
        }

        var legacyEnableValue = Environment.GetEnvironmentVariable("GREG_ENABLE_HOVER_PATCH");
        if (string.IsNullOrWhiteSpace(legacyEnableValue))
        {
            return false;
        }

        return IsTruthy(legacyEnableValue);
    }

    private static bool IsTruthy(string value)
    {
        return value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase);
    }

    static void Postfix()
    {
        if (_isProcessing)
        {
            return;
        }

        try
        {
            _isProcessing = true;

            var focusHit = gregGameHooks.RaycastForward(MaxHoverDistance);
            var entity = focusHit?.Entity;
            var source = "Physics.RaycastForward";

            if (entity == null)
            {
                return;
            }

            var entityType = "GameObject";
            var entityName = !string.IsNullOrWhiteSpace(focusHit?.Name) ? focusHit?.Name : entity.name;
            var entityId = entity.GetInstanceID();
            var distance = focusHit?.Distance ?? 0f;
            var targetKey = $"{entityType}:{entityId}";
            var now = Time.unscaledTime;

            if (string.Equals(_lastHoverTargetKey, targetKey, StringComparison.Ordinal)
                && now - _lastHoverEmitAt < 0.1f)
            {
                return;
            }

            _lastHoverTargetKey = targetKey;
            _lastHoverEmitAt = now;

            var payload = new Dictionary<string, object>
            {
                ["EntityType"] = entityType,
                ["EntityName"] = entityName,
                ["EntityId"] = entityId,
                ["HexId"] = $"0x{entityId:X8}",
                ["ServerType"] = string.IsNullOrWhiteSpace(entityName) ? entityType : entityName,
                ["Color"] = "N/A",
                ["CustomerId"] = "N/A",
                ["OwnerName"] = "N/A",
                ["DistanceMeters"] = distance,
                ["Source"] = source
            };

            gregEventDispatcher.Emit(gregNativeEventHooks.WorldInteractionHovered, payload);
            gregEventDispatcher.Emit(HexViewerTargetUpdatedHook, payload);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("GregHexViewerPatch.Postfix", ex);
        }
        finally
        {
            _isProcessing = false;
        }
    }
}
