using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.API;

namespace greg.GridPlacement
{
    public class GregGridManager
    {
        public static GregGridManager Instance { get; private set; }

        private readonly Dictionary<Vector2Int, GregGridCell> _cells = new();
        public float CellSizeX { get; private set; } = 2.0f;
        public float CellSizeZ { get; private set; } = 2.0f;
        public Vector3 GridOrigin { get; private set; } = Vector3.zero;
        
        public bool ShowGridLines { get; set; } = false;
        public bool ShowSubGrid { get; set; } = false;

        public GregGridManager()
        {
            Instance = this;
        }

        public void Initialize(Vector3 origin, int width, int depth)
        {
            GridOrigin = origin;
            _cells.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var coord = new Vector2Int(x, z);
                    _cells[coord] = new GregGridCell(coord);
                }
            }
        }

        public GregGridCell? GetCell(Vector2Int coord)
        {
            return _cells.TryGetValue(coord, out var cell) ? cell : null;
        }

        public GregGridCell? GetCellAtWorldPos(Vector3 worldPos)
        {
            var localPos = worldPos - GridOrigin;
            int x = Mathf.FloorToInt(localPos.x / CellSizeX);
            int z = Mathf.FloorToInt(localPos.z / CellSizeZ);
            return GetCell(new Vector2Int(x, z));
        }

        public bool IsCellOccupied(Vector2Int coord)
        {
            var cell = GetCell(coord);
            return cell != null && cell.IsOccupied;
        }

        public bool PlaceRack(Vector2Int coord, GregPlaceableRack rack)
        {
            var cell = GetCell(coord);
            if (cell == null || cell.IsOccupied || cell.IsBlocked) return false;

            cell.Occupant = rack;
            cell.IsOccupied = true;
            rack.GridCoord = coord;

            // Firing the hook
            var payload = new gregCore.Sdk.Models.GregPayload("greg.WORLD.RackPlaced", "gregCore");
            payload.Data["rackId"] = rack.RackId;
            payload.Data["gridCoord"] = $"{coord.x},{coord.y}";
            payload.Data["worldPos"] = $"{rack.UnityGameObject?.transform.position.x},{rack.UnityGameObject?.transform.position.y},{rack.UnityGameObject?.transform.position.z}";
            payload.Data["modId"] = "greg.GridPlacement";
            GregAPI.Hooks.Fire("greg.WORLD.RackPlaced", payload);

            return true;
        }

        public bool RemoveRack(Vector2Int coord)
        {
            var cell = GetCell(coord);
            if (cell == null || !cell.IsOccupied) return false;

            var rack = cell.Occupant;
            cell.Occupant = null;
            cell.IsOccupied = false;

            if (rack != null)
            {
                var payload = new gregCore.Sdk.Models.GregPayload("greg.WORLD.RackRemoved", "gregCore");
                payload.Data["rackId"] = rack.RackId;
                payload.Data["gridCoord"] = $"{coord.x},{coord.y}";
                payload.Data["modId"] = "greg.GridPlacement";
                GregAPI.Hooks.Fire("greg.WORLD.RackRemoved", payload);
            }

            return true;
        }

        public Vector3 SnapToGrid(Vector3 worldPos)
        {
            var localPos = worldPos - GridOrigin;
            int x = Mathf.FloorToInt(localPos.x / CellSizeX);
            int z = Mathf.FloorToInt(localPos.z / CellSizeZ);
            return GridOrigin + new Vector3(x * CellSizeX + (CellSizeX / 2), 0, z * CellSizeZ + (CellSizeZ / 2));
        }

        public void ClearAll()
        {
            foreach (var cell in _cells.Values)
            {
                if (cell.IsOccupied)
                {
                    cell.Occupant?.Remove();
                    cell.IsOccupied = false;
                    cell.Occupant = null;
                }
            }
        }
        
        public IReadOnlyDictionary<Vector2Int, GregGridCell> GetCells() => _cells;
    }
}
