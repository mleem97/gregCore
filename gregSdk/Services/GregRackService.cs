using System.Collections.Generic;

namespace gregSdk.Services;

public static class GregRackService
{
    private static readonly Dictionary<string, HashSet<int>> _rackOccupancy = new Dictionary<string, HashSet<int>>();

    public static bool CanFitInRack(string rackId, int requiredUnits, int startingSlot)
    {
        if (!_rackOccupancy.TryGetValue(rackId, out var occupied)) return true;

        for (int i = 0; i < requiredUnits; i++)
        {
            if (occupied.Contains(startingSlot + i)) return false;
        }
        return true;
    }

    public static void MountServer(string rackId, string serverId, int startingSlot, int units = 1)
    {
        if (!_rackOccupancy.ContainsKey(rackId)) _rackOccupancy[rackId] = new HashSet<int>();
        
        for (int i = 0; i < units; i++)
        {
            _rackOccupancy[rackId].Add(startingSlot + i);
        }
    }

    public static void UnmountServer(string rackId, int startingSlot, int units = 1)
    {
        if (_rackOccupancy.TryGetValue(rackId, out var occupied))
        {
            for (int i = 0; i < units; i++)
            {
                occupied.Remove(startingSlot + i);
            }
        }
    }

    public static void CloneRackTransaction(string sourceRackId, string targetRackId) { }
}
