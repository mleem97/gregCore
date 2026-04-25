using System;

namespace greg.Multiplayer
{
    [Serializable]
    public class CableSyncPayload
    {
        public int CableId { get; set; }
        public string Action { get; set; }   // "CONNECT", "DISCONNECT"
        public int SourcePortId { get; set; }
        public int TargetPortId { get; set; }
        public string CableType { get; set; } // "SFP", "ETH", "FIBER"
        public long Timestamp { get; set; }
    }
}
