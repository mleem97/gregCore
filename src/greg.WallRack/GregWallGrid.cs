using System;
using System.Collections.Generic;
using UnityEngine;

namespace greg.WallRack
{
    public class GregWallGrid
    {
        public string wallId = string.Empty;
        public Vector3 wallOrigin;
        public Vector3 wallNormal;
        public Vector3 wallUp;
        public float slotWidth = 0.5f;
        public float slotHeight = 0.5f;
        public int columns;
        public int rows;

        public Dictionary<Vector2Int, GregWallSlot> slots = new();

        public void Initialize(string wId, Vector3 origin, Vector3 normal, Vector3 up, int cols, int r)
        {
            wallId = wId;
            wallOrigin = origin;
            wallNormal = normal.normalized;
            wallUp = up.normalized;
            columns = cols;
            rows = r;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    slots[new Vector2Int(x, y)] = new GregWallSlot(new Vector2Int(x, y));
                }
            }
        }

        public GregWallSlot? GetSlot(Vector2Int coord)
        {
            if (slots.TryGetValue(coord, out var slot))
                return slot;
            return null;
        }

        public GregWallSlot? GetSlotAtWorldPos(Vector3 worldPos)
        {
            Vector2Int coord = WorldPosToSlot(worldPos);
            return GetSlot(coord);
        }

        public bool IsSlotOccupied(Vector2Int coord)
        {
            var slot = GetSlot(coord);
            return slot != null && slot.isOccupied;
        }

        public bool MountDevice(Vector2Int coord, GregWallDevice device)
        {
            var slot = GetSlot(coord);
            if (slot == null || slot.isOccupied || slot.isBlocked) return false;

            if ((slot.allowedTypes & device.deviceType) == 0) return false;

            Vector3 wPos = SlotToWorldPos(coord);
            Quaternion rot = Quaternion.LookRotation(wallNormal, wallUp);
            
            device.MountTo(slot, wPos, rot);
            return true;
        }

        public bool UnmountDevice(Vector2Int coord)
        {
            var slot = GetSlot(coord);
            if (slot == null || !slot.isOccupied || slot.mountedDevice == null) return false;

            slot.mountedDevice.Unmount();
            slot.isOccupied = false;
            slot.mountedDevice = null;
            return true;
        }

        public bool SwapDevice(Vector2Int coord, GregWallDevice newDevice)
        {
            if (!UnmountDevice(coord)) return false;
            return MountDevice(coord, newDevice);
        }

        public Vector3 SlotToWorldPos(Vector2Int coord)
        {
            Vector3 right = Vector3.Cross(wallUp, wallNormal).normalized;
            Vector3 localOffset = right * (coord.x * slotWidth) + wallUp * (coord.y * slotHeight);
            return wallOrigin + localOffset;
        }

        public Vector2Int WorldPosToSlot(Vector3 worldPos)
        {
            Vector3 right = Vector3.Cross(wallUp, wallNormal).normalized;
            Vector3 localPos = worldPos - wallOrigin;

            float x = Vector3.Dot(localPos, right);
            float y = Vector3.Dot(localPos, wallUp);

            int col = Mathf.RoundToInt(x / slotWidth);
            int row = Mathf.RoundToInt(y / slotHeight);

            return new Vector2Int(col, row);
        }

        public void DrawDebugGrid()
        {
            if (!gregCore.Infrastructure.Config.GregCoreConfig.DebugMode) return;
            
            // Draw Grid Lines via GL or Debug.DrawLine
            Vector3 right = Vector3.Cross(wallUp, wallNormal).normalized;
            for (int x = 0; x <= columns; x++)
            {
                Vector3 start = wallOrigin + right * (x * slotWidth);
                Vector3 end = start + wallUp * (rows * slotHeight);
                Debug.DrawLine(start, end, Color.cyan);
            }
            for (int y = 0; y <= rows; y++)
            {
                Vector3 start = wallOrigin + wallUp * (y * slotHeight);
                Vector3 end = start + right * (columns * slotWidth);
                Debug.DrawLine(start, end, Color.cyan);
            }
        }
    }
}
