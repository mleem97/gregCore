using System.Collections.Generic;

namespace gregSdk.Services;

public static class GregIPAllocationService
{
    private static readonly HashSet<string> _allocatedIps = new HashSet<string>();
    private static int _nextIpSuffix = 100;

    public static string AllocateIP(string deviceMac, string subnetPrefix = "192.168.1")
    {
        string ip = $"{subnetPrefix}.{_nextIpSuffix++}";
        while (_allocatedIps.Contains(ip) && _nextIpSuffix < 255)
        {
            ip = $"{subnetPrefix}.{_nextIpSuffix++}";
        }
        
        _allocatedIps.Add(ip);
        return ip;
    }

    public static void ReleaseIP(string ipAddress)
    {
        _allocatedIps.Remove(ipAddress);
    }
}
