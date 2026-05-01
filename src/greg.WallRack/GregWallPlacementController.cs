using System;
using UnityEngine;
using UnityEngine.InputSystem;
using greg.Logging;

namespace greg.WallRack
{
    public class GregWallPlacementController
    {
        public static GregWallPlacementController Instance { get; } = new();

        public bool wallBuildModeActive = false;
        public GregWallDevice? previewDevice;
        public GregWallGrid? targetGrid;
        public GregWallSlot? hoveredSlot;

        public bool showGridOverlay = true;
        public bool showSlotLabels = false;

        private readonly GregModLogger _log = new GregModLogger("WallRack");

        public void ActivateWallBuildMode()
        {
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack")) return;
            wallBuildModeActive = true;
            _log.Msg("Wall Build Mode activated.");
        }

        public void DeactivateWallBuildMode()
        {
            wallBuildModeActive = false;
            targetGrid = null;
            hoveredSlot = null;
            _log.Msg("Wall Build Mode deactivated.");
        }

        public void OnUpdate()
        {
            if (!wallBuildModeActive) return;

            // Simple raycast against walls in a real scenario
            // For now, pseudo-code behavior
            targetGrid = null;
            hoveredSlot = null;
            
            // Vector3 hitPos = ...
            // targetGrid = GregWallRegistry.Instance.GetGridAtWorldPos(hitPos, 2.0f);
            // if (targetGrid != null) {
            //     hoveredSlot = targetGrid.GetSlotAtWorldPos(hitPos);
            // }
        }

        public void TryMount(Vector3 worldPos)
        {
            if (previewDevice == null) return;
            var grid = GregWallRegistry.Instance.GetGridAtWorldPos(worldPos, 2.0f);
            if (grid == null) return;

            var slot = grid.GetSlotAtWorldPos(worldPos);
            if (slot == null || slot.isOccupied) return;

            if (grid.MountDevice(slot.coord, previewDevice))
            {
                GregWallUndoRedoService.Instance.PushAction(
                    new MountAction(grid.wallId, slot.coord, previewDevice)
                );
                _log.Msg($"Mounted device {previewDevice.deviceId} at {slot.coord}");
            }
        }

        public void TryUnmount(Vector3 worldPos)
        {
            var grid = GregWallRegistry.Instance.GetGridAtWorldPos(worldPos, 2.0f);
            if (grid == null) return;

            var slot = grid.GetSlotAtWorldPos(worldPos);
            if (slot == null || !slot.isOccupied) return;

            var dev = slot.mountedDevice;
            if (dev != null && grid.UnmountDevice(slot.coord))
            {
                GregWallUndoRedoService.Instance.PushAction(
                    new UnmountAction(grid.wallId, slot.coord, dev)
                );
                _log.Msg($"Unmounted device from {slot.coord}");
            }
        }

        public void TrySwap(Vector3 worldPos)
        {
            var grid = GregWallRegistry.Instance.GetGridAtWorldPos(worldPos, 2.0f);
            if (grid == null) return;

            var slot = grid.GetSlotAtWorldPos(worldPos);
            if (slot == null || !slot.isOccupied) return;

            var oldDev = slot.mountedDevice;
            var newDev = previewDevice; // From inventory

            if (oldDev != null && newDev != null && grid.SwapDevice(slot.coord, newDev))
            {
                GregWallUndoRedoService.Instance.PushAction(
                    new SwapAction(grid.wallId, slot.coord, oldDev, newDev)
                );
                _log.Msg($"Swapped device at {slot.coord}");
                
                gregCore.Core.Events.GregEventDispatcher.Emit(gregCore.GameLayer.Hooks.GregNativeEventHooks.WorldWallDeviceSwapped, slot.coord);
            }
        }

        public void OnInteract(Vector3 worldPos)
        {
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack")) return;

            var grid = GregWallRegistry.Instance.GetGridAtWorldPos(worldPos, 2.0f);
            if (grid == null) return;

            var slot = grid.GetSlotAtWorldPos(worldPos);
            if (slot == null || !slot.isOccupied) return;

            // Open context menu Widget
            var mousePos = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            var builder = gregCore.UI.GregUIBuilder.CreateWidget($"Rack_{slot.coord}", mousePos.x, Screen.height - mousePos.y)
                .SetSize(250, 150)
                .AddHeadline("Device Context")
                .AddLabel($"ID: {slot.mountedDevice?.deviceId}")
                .AddPrimaryButton("UNMOUNT", () => {
                    TryUnmount(worldPos);
                    gregCore.UI.GregUIManager.SetPanelActive($"Rack_{slot.coord}", false);
                })
                .AddSecondaryButton("CLOSE", () => {
                    gregCore.UI.GregUIManager.SetPanelActive($"Rack_{slot.coord}", false);
                });
            
            builder.Build();
        }
    }
}
