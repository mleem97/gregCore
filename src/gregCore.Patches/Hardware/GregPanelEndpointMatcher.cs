using System;

namespace gregCore.GameLayer.Patches.Hardware;

/// <summary>
/// Matches save cable endpoints against a legacy patch-panel ID and remaps
/// them to the healed gregID.
///
/// Panel endpoints are either the bare panel ID or the panel ID plus a
/// suffix (port/designation part). A naive StartsWith + Replace corrupts
/// FOREIGN endpoints whenever one panel ID is a prefix of another — with
/// vanilla numeric suffixes (Panel1 vs Panel12 vs Panel100, ...) such pairs
/// are the norm, not the exception. The corrupted endpoint then points at a
/// non-existent device: after load the switch cannot resolve it (raw-name
/// fallback in the port list, missing service, merged routes — see
/// Mantis #19) and later healing passes skip it because it no longer
/// matches anything. Strictly one-directional damage, permanent until the
/// save is repaired by hand.
///
/// Matching rule: exact equality, or prefix where the ID boundary is marked
/// by a separator on at least one side (the ID itself ends with a
/// non-alphanumeric, or the next endpoint char is non-alphanumeric).
/// Remapping rewrites the leading ID only, never later occurrences.
/// </summary>
internal static class GregPanelEndpointMatcher
{
    internal static bool Matches(string endpoint, string oldId)
    {
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(oldId))
            return false;
        if (string.Equals(endpoint, oldId, StringComparison.Ordinal))
            return true;
        if (!endpoint.StartsWith(oldId, StringComparison.Ordinal))
            return false;
        if (!char.IsLetterOrDigit(oldId[oldId.Length - 1]))
            return true;
        return !char.IsLetterOrDigit(endpoint[oldId.Length]);
    }

    internal static string Remap(string endpoint, string oldId, string newGuid)
    {
        // Prefix-only rewrite: endpoint is guaranteed (by Matches) to start
        // with oldId, so a Substring splice cannot touch later occurrences
        // the way string.Replace would.
        return newGuid + endpoint.Substring(oldId.Length);
    }
}
