using System;
using System.Collections.Generic;
using UnityEngine;

namespace greg.WallRack
{
    public abstract record GregWallAction(string WallId);

    public record MountAction(string WallId, Vector2Int Coord, GregWallDevice Device) : GregWallAction(WallId);
    public record UnmountAction(string WallId, Vector2Int Coord, GregWallDevice Device) : GregWallAction(WallId);
    public record SwapAction(string WallId, Vector2Int Coord, GregWallDevice OldDevice, GregWallDevice NewDevice) : GregWallAction(WallId);
    public record BuyWallAction(string WallId, Vector3 WallPos, Vector3 WallNormal) : GregWallAction(WallId);

    public class GregWallUndoRedoService
    {
        public static GregWallUndoRedoService Instance { get; } = new();

        private readonly Stack<GregWallAction> _undoStack = new(50);
        private readonly Stack<GregWallAction> _redoStack = new();

        public int UndoCount => _undoStack.Count;

        public void PushAction(GregWallAction action)
        {
            if (_undoStack.Count >= 50)
            {
                // Simple stack management
            }
            _undoStack.Push(action);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) return;
            var action = _undoStack.Pop();
            _redoStack.Push(action);

            var registry = GregWallRegistry.Instance;
            var grid = registry.GetGrid(action.WallId);

            switch (action)
            {
                case MountAction m:
                    grid?.UnmountDevice(m.Coord);
                    break;
                case UnmountAction u:
                    grid?.MountDevice(u.Coord, u.Device);
                    break;
                case SwapAction s:
                    grid?.SwapDevice(s.Coord, s.OldDevice);
                    break;
                case BuyWallAction b:
                    registry.UnregisterWall(b.WallId);
                    break;
            }
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) return;
            var action = _redoStack.Pop();
            _undoStack.Push(action);

            var registry = GregWallRegistry.Instance;
            var grid = registry.GetGrid(action.WallId);
            
            switch (action)
            {
                case MountAction m:
                    grid?.MountDevice(m.Coord, m.Device);
                    break;
                case UnmountAction u:
                    grid?.UnmountDevice(u.Coord);
                    break;
                case SwapAction s:
                    grid?.SwapDevice(s.Coord, s.NewDevice);
                    break;
                case BuyWallAction b:
                    var newGrid = new GregWallGrid();
                    newGrid.Initialize(b.WallId, b.WallPos, b.WallNormal, Vector3.up, 4, 3);
                    registry.RegisterWall(b.WallId, newGrid);
                    break;
            }
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
