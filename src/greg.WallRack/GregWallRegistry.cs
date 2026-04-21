using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace greg.WallRack
{
    public class GregWallRegistry
    {
        public static GregWallRegistry Instance { get; } = new();

        private readonly Dictionary<string, GregWallGrid> _walls = new();

        public void RegisterWall(string wallId, GregWallGrid grid)
        {
            if (string.IsNullOrEmpty(wallId)) return;
            _walls[wallId] = grid;
        }

        public void UnregisterWall(string wallId)
        {
            if (string.IsNullOrEmpty(wallId)) return;
            _walls.Remove(wallId);
        }

        public GregWallGrid? GetGrid(string wallId)
        {
            if (string.IsNullOrEmpty(wallId)) return null;
            _walls.TryGetValue(wallId, out var grid);
            return grid;
        }

        public GregWallGrid? GetGridAtWorldPos(Vector3 worldPos, float tolerance)
        {
            foreach (var grid in _walls.Values)
            {
                if (Vector3.Distance(grid.wallOrigin, worldPos) < tolerance)
                {
                    return grid;
                }
            }
            return null;
        }

        public IEnumerable<GregWallGrid> GetAllGrids() => _walls.Values;

        public GregWallSlot? FindNearestFreeSlot(Vector3 worldPos, GregWallSlotType type)
        {
            GregWallSlot? nearestSlot = null;
            float nearestDist = float.MaxValue;

            foreach (var grid in _walls.Values)
            {
                foreach (var slot in grid.slots.Values)
                {
                    if (slot.isOccupied || slot.isBlocked) continue;
                    if ((slot.allowedTypes & type) == 0) continue;

                    Vector3 pos = grid.SlotToWorldPos(slot.coord);
                    float d = Vector3.Distance(pos, worldPos);
                    if (d < nearestDist)
                    {
                        nearestDist = d;
                        nearestSlot = slot;
                    }
                }
            }
            return nearestSlot;
        }
    }
}
