using System;
using System.Collections.Generic;

namespace greg.Multiplayer
{
    /// <summary>
    /// Delta payload for a single rack state update.
    /// Only changed slots are transmitted to minimise bandwidth.
    /// </summary>
    [Serializable]
    public class RackSyncPayload
    {
        public int RackId { get; set; }
        public string Action { get; set; }   // "PLACE", "REMOVE", "FULL"
        public long Timestamp { get; set; }  // UTC ticks
        public Dictionary<int, SlotData> ChangedSlots { get; set; } = new();
    }

    [Serializable]
    public class SlotData
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool IsOccupied { get; set; }
        public float Iops { get; set; }
        public float PowerWatts { get; set; }
    }
}
