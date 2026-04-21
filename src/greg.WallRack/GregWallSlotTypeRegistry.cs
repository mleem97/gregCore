using System;
using System.Collections.Generic;

namespace greg.WallRack
{
    public static class GregWallSlotTypeRegistry
    {
        private static readonly Dictionary<string, int> _customTypes = new();

        public static void RegisterType(string typeId, int flagValue)
        {
            if (string.IsNullOrEmpty(typeId)) return;
            _customTypes[typeId] = flagValue;
        }

        public static int GetFlag(string typeId)
        {
            if (_customTypes.TryGetValue(typeId, out int flag))
            {
                return flag;
            }
            return 0;
        }
    }
}
