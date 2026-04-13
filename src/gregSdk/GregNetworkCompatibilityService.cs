using System.Linq;
using greg.Sdk.Registries;

namespace greg.Sdk;

public static class GregNetworkCompatibilityService
{
    private static GregSwitchRegistry _switchRegistry;
    private static GregSfpRegistry _sfpRegistry;
    private static GregCableRegistry _cableRegistry;

    public static void Initialize(GregSwitchRegistry switches, GregSfpRegistry sfps, GregCableRegistry cables)
    {
        _switchRegistry = switches;
        _sfpRegistry = sfps;
        _cableRegistry = cables;
    }

    public static bool CanLink(string switchId, string sfpId, string cableId, out string reason)
    {
        reason = string.Empty;

        if (!_switchRegistry.TryGet(switchId, out var switchDef)) { reason = "Switch not found."; return false; }
        if (!_sfpRegistry.TryGet(sfpId, out var sfpDef)) { reason = "SFP not found."; return false; }
        if (!_cableRegistry.TryGet(cableId, out var cableDef)) { reason = "Cable not found."; return false; }

        // 1. Check if Switch supports this SFP profile
        if (switchDef.SupportedSfpProfiles.Length > 0 && !sfpDef.CompatibilityTags.Any(tag => switchDef.SupportedSfpProfiles.Contains(tag)))
        {
            reason = $"Switch {switchId} does not support SFP profile tags: {string.Join(", ", sfpDef.CompatibilityTags)}";
            return false;
        }

        // 2. Check speed compatibility
        if (cableDef.MaxSpeedGbps < sfpDef.SpeedGbps)
        {
            reason = $"Cable speed ({cableDef.MaxSpeedGbps} Gbps) is lower than SFP speed ({sfpDef.SpeedGbps} Gbps).";
            return false;
        }

        return true;
    }
}

