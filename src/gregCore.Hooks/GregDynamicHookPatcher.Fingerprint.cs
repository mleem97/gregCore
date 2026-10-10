using System;
using System.IO;

namespace gregCore.GameLayer.Hooks
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1118:Utility classes should not have public constructors", Justification = "Partial declaration holding only static helpers; instance members live in GregDynamicHookPatcher.cs.")]
    public sealed partial class GregDynamicHookPatcher
    {
        // Resolves the game root from the manifest directory (two levels up).
        private static string ResolveGameRoot(string manifestDirectory)
        {
            try
            {
                string? parent = GetGrandParent(manifestDirectory);
                return string.IsNullOrEmpty(parent) ? manifestDirectory : parent;
            }
            catch { return manifestDirectory; }
        }

        // Returns the grand-parent directory path or null.
        private static string? GetGrandParent(string manifestDirectory)
        {
            try
            {
                var parent = Directory.GetParent(manifestDirectory);
                return parent?.Parent?.FullName;
            }
            catch { return null; }
        }

        // Captures the game fingerprint without throwing.
        private static Core.Diagnostics.GameFingerprint CaptureGameFingerprint(string gameRoot)
        {
            try
            {
                return Core.Diagnostics.GameFingerprint.Capture(gameRoot);
            }
            catch
            {
                return new Core.Diagnostics.GameFingerprint();
            }
        }

        // Checks whether the manifest carries a usable fingerprint.
        private static bool IsKnownFingerprint(GregHooksManifest manifest)
        {
            try
            {
                return HasFingerprintValue(manifest.AssemblyFingerprint);
            }
            catch { return false; }
        }

        // Validates a raw fingerprint string.
        private static bool HasFingerprintValue(string? fingerprint)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fingerprint)) return false;
                return !IsUnknownMarker(fingerprint);
            }
            catch { return false; }
        }

        // Checks for the UNKNOWN placeholder marker.
        private static bool IsUnknownMarker(string fingerprint)
        {
            try
            {
                return string.Equals(fingerprint, "UNKNOWN", StringComparison.Ordinal);
            }
            catch { return false; }
        }

        // Compares the manifest fingerprint against the captured one.
        private static bool IsFingerprintMatch(GregHooksManifest manifest, Core.Diagnostics.GameFingerprint fingerprint, bool known)
        {
            try
            {
                if (!known) return false;
                return MatchesCombined(manifest.AssemblyFingerprint, fingerprint.CombinedSha256);
            }
            catch { return false; }
        }

        // Compares two fingerprint hashes case-insensitively.
        private static bool MatchesCombined(string? expected, string? actual)
        {
            try
            {
                return string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        // Maps match results to the report status string.
        private static string FingerprintStatus(bool matches, bool known)
        {
            try
            {
                if (matches) return "match";
                return known ? "mismatch" : "unknown";
            }
            catch { return "unknown"; }
        }
    }
}
