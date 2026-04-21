using System;
using UnityEngine;
using HarmonyLib;
using gregCore.Core.Events;
using gregCore.Sdk.Models;
using greg.Logging;

namespace greg.WallRack.Integration
{
    public static class WallBuyFlowIntegration
    {
        private static readonly GregModLogger _log = new GregModLogger("WallRack");

        public static void Initialize()
        {
            // Subscribe to BuyWall Hook
            gregCore.Core.Events.GregEventDispatcher.On(
                gregCore.GameLayer.Hooks.GregNativeEventHooks.SystemButtonBuyWall,
                OnBuyWallTriggered,
                "greg.WallRack"
            );

            _log.HookSubscribed(gregCore.GameLayer.Hooks.GregNativeEventHooks.SystemButtonBuyWall);
        }

        private static void OnBuyWallTriggered(object payload)
        {
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack")) return;

            string? wallId = GregPayload.Get<string>(payload, "wallId", null);
            string? wallPosStr = GregPayload.Get<string>(payload, "wallPos", null);
            string? wallNormStr = GregPayload.Get<string>(payload, "wallNormal", null);

            if (string.IsNullOrEmpty(wallId))
            {
                _log.Warn("wallId is null in payload, fallback required.");
                // Fallback logic
                wallId = $"wall_{Guid.NewGuid():N}";
            }

            Vector3 wallPos = ParseVector3(wallPosStr);
            Vector3 wallNormal = ParseVector3(wallNormStr);

            GregWallGrid grid = new GregWallGrid();
            grid.Initialize(wallId, wallPos, wallNormal, Vector3.up, 4, 3);
            GregWallRegistry.Instance.RegisterWall(wallId, grid);

            _log.Msg($"Wall registered: {wallId} at {wallPos} -- 4x3 slots");
            
            // Push Undo Action
            GregWallUndoRedoService.Instance.PushAction(
                new BuyWallAction(wallId, wallPos, wallNormal)
            );

            // Notify
            gregCore.Core.Events.GregEventDispatcher.Emit(gregCore.GameLayer.Hooks.GregNativeEventHooks.WorldWallRegistered, payload);
        }

        private static Vector3 ParseVector3(string? str)
        {
            if (string.IsNullOrEmpty(str)) return Vector3.zero;
            var parts = str.Split(',');
            if (parts.Length == 3 && 
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z))
            {
                return new Vector3(x, y, z);
            }
            return Vector3.zero;
        }
    }

    [HarmonyPatch(typeof(Il2Cpp.MainGameManager), "ButtonBuyWall")]
    internal static class ButtonBuyWallPatch
    {
        private static readonly GregModLogger _log = new GregModLogger("WallRack");

        [HarmonyPostfix]
        private static void Postfix()
        {
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack")) return;
            
            _log.Debug("ButtonBuyWall Postfix executed.");
            // If the hook payload was missing data, we can extract from instances here
        }
    }
}
