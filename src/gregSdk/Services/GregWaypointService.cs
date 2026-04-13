using System.Collections.Generic;

namespace gregCoreSDK.Sdk.Services;

/// <summary>
/// Bridge for the Waypoint Initialization System (cable routing and topology).
/// </summary>
public static class GregWaypointService
{
    public static void RegisterWaypoint(string waypointId, float x, float y, float z) { }
    public static List<string> FindShortestPath(string startWaypointId, string endWaypointId)
    {
        return new List<string>();
    }
    public static void RecalculateAllWaypoints() { }
}
