using System;

namespace greg.WallRack
{
    [Flags]
    public enum GregWallSlotType
    {
        None    = 0,
        Rack    = 1 << 0,
        Switch  = 1 << 1,
        Router  = 1 << 2,
        Patch   = 1 << 3,
        Generic = 1 << 4,
        Any     = ~0
    }

    public class GregWallSlot
    {
        public UnityEngine.Vector2Int coord;
        public bool isOccupied;
        public GregWallDevice? mountedDevice;
        public GregWallSlotType allowedTypes = GregWallSlotType.Any;
        public bool isBlocked = false;

        public GregWallSlot(UnityEngine.Vector2Int c)
        {
            coord = c;
        }
    }
}
