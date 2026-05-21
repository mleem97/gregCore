using System;
using gregCore.Core.Models;

namespace gregCore.Compatibility.FishNet;

/// <summary>
/// Placeholder bridge for sending FishNet ServerRpcs.
/// This acts as a centralized stub until the actual FishNet networking is fully implemented.
/// </summary>
public static class FishNetBridge
{
    public static void SendServerRpc(string rpcName, params object[] args)
    {
        // TODO: Implement actual FishNet ObserversRpc/ServerRpc call via reflection or direct integration
    }
}
