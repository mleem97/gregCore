using System.Collections.Concurrent;

namespace greg.Sdk.Services;

public static class GregPowerService
{
    private static readonly ConcurrentDictionary<string, int> _powerConsumers = new();

    public static int GetTotalPowerDraw(string topologyZoneId)
    {
        // Currently ignoring topologyZoneId, calculating global mod power draw
        int total = 0;
        foreach (var val in _powerConsumers.Values)
        {
            total += val;
        }
        return total;
    }

    public static void RegisterConsumer(string deviceId, int watts)
    {
        _powerConsumers[deviceId] = watts;
    }

    public static void UnregisterConsumer(string deviceId)
    {
        _powerConsumers.TryRemove(deviceId, out _);
    }
}

