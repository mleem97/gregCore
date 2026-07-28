using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace gregCore.Core.Compatibility;

public enum CompatibilityLevel
{
    Unknown = 0,
    Declared = 1,
    SizeVerified = 2,
    HashVerified = 3,
    RuntimeVerified = 4,
    Incompatible = 100
}

public sealed class CompatibilityProfile
{
    [JsonProperty("schemaVersion")]
    public int SchemaVersion { get; set; }

    [JsonProperty("profileId")]
    public string ProfileId { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = "experimental";

    [JsonProperty("framework")]
    public FrameworkCompatibility Framework { get; set; } = new();

    [JsonProperty("game")]
    public GameCompatibility Game { get; set; } = new();

    [JsonProperty("unity")]
    public UnityCompatibility Unity { get; set; } = new();

    [JsonProperty("runtime")]
    public RuntimeCompatibility Runtime { get; set; } = new();

    [JsonProperty("referenceFiles")]
    public List<ReferenceCompatibility> ReferenceFiles { get; set; } = new();

    [JsonProperty("features")]
    public Dictionary<string, bool> Features { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonProperty("hookManifest")]
    public HookManifestCompatibility? HookManifest { get; set; }

    [JsonProperty("notes")]
    public List<string> Notes { get; set; } = new();

    public bool Supports(string capability) =>
        Features.TryGetValue(capability, out bool supported) && supported;
}

public sealed class FrameworkCompatibility
{
    [JsonProperty("versionLine")]
    public string VersionLine { get; set; } = string.Empty;

    [JsonProperty("minimumVersion")]
    public string MinimumVersion { get; set; } = string.Empty;

    [JsonProperty("maximumVersionExclusive")]
    public string? MaximumVersionExclusive { get; set; }
}

public sealed class GameCompatibility
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    [JsonProperty("buildId")]
    public string? BuildId { get; set; }
}

public sealed class UnityCompatibility
{
    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    [JsonProperty("exactVersionKnown")]
    public bool ExactVersionKnown { get; set; }

    [JsonProperty("backend")]
    public string Backend { get; set; } = "IL2CPP";

    [JsonProperty("metadataVersion")]
    public int? MetadataVersion { get; set; }
}

public sealed class RuntimeCompatibility
{
    [JsonProperty("loader")]
    public string Loader { get; set; } = string.Empty;

    [JsonProperty("loaderVersion")]
    public string LoaderVersion { get; set; } = string.Empty;

    [JsonProperty("interop")]
    public string Interop { get; set; } = string.Empty;

    [JsonProperty("interopVersion")]
    public string? InteropVersion { get; set; }

    [JsonProperty("architectures")]
    public List<string> Architectures { get; set; } = new();

    [JsonProperty("platforms")]
    public List<string> Platforms { get; set; } = new();
}

public sealed class ReferenceCompatibility
{
    [JsonProperty("path")]
    public string Path { get; set; } = string.Empty;

    [JsonProperty("required")]
    public bool Required { get; set; }

    [JsonProperty("size")]
    public long? Size { get; set; }

    [JsonProperty("sha256")]
    public string? Sha256 { get; set; }

    [JsonProperty("assemblyVersion")]
    public string? AssemblyVersion { get; set; }
}

public sealed class HookManifestCompatibility
{
    [JsonProperty("schemaVersion")]
    public int SchemaVersion { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; } = string.Empty;

    [JsonProperty("sha256")]
    public string? Sha256 { get; set; }
}

public sealed record CompatibilityIssue(
    string Code,
    string Message,
    bool IsFatal,
    string? Path = null);

public sealed class CompatibilityReport
{
    public string ProfileId { get; init; } = string.Empty;
    public CompatibilityLevel Level { get; init; }
    public bool SafeMode { get; init; }
    public bool CanLoadGameAdapters => !SafeMode && Level != CompatibilityLevel.Incompatible;
    public IReadOnlyList<CompatibilityIssue> Issues { get; init; } = Array.Empty<CompatibilityIssue>();

    public string ToDiagnosticText()
    {
        var lines = new List<string>
        {
            $"Profile: {ProfileId}",
            $"Level: {Level}",
            $"SafeMode: {SafeMode}"
        };
        lines.AddRange(Issues.Select(issue =>
            $"[{(issue.IsFatal ? "FATAL" : "WARN")}] {issue.Code}: {issue.Message}"));
        return string.Join(Environment.NewLine, lines);
    }
}

public static class CompatibilityProfileLoader
{
    public static CompatibilityProfile Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("A profile path is required.", nameof(path));

        string json = File.ReadAllText(path);
        var profile = JsonConvert.DeserializeObject<CompatibilityProfile>(json)
            ?? throw new InvalidDataException($"Compatibility profile is empty: {path}");

        Validate(profile, path);
        return profile;
    }

    public static void Validate(CompatibilityProfile profile, string? source = null)
    {
        if (profile.SchemaVersion != 2)
            throw new InvalidDataException($"Unsupported compatibility schema {profile.SchemaVersion} in {source ?? profile.ProfileId}.");
        if (string.IsNullOrWhiteSpace(profile.ProfileId))
            throw new InvalidDataException("Compatibility profileId is required.");
        if (!string.Equals(profile.Unity.Backend, "IL2CPP", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("gregCore compatibility profiles currently require the IL2CPP backend.");
        if (profile.ReferenceFiles.Any(reference => string.IsNullOrWhiteSpace(reference.Path)))
            throw new InvalidDataException("Every compatibility reference requires a path.");
    }
}

public static class CompatibilityVerifier
{
    public static CompatibilityReport Verify(
        CompatibilityProfile profile,
        Func<ReferenceCompatibility, string?> resolveReference,
        string? detectedUnityVersion = null,
        string? detectedArchitecture = null,
        string? detectedPlatform = null)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(resolveReference);

        var issues = new List<CompatibilityIssue>();
        bool allSizesVerified = true;
        bool allHashesVerified = true;
        bool anyHashDeclared = false;

        if (!string.IsNullOrWhiteSpace(detectedUnityVersion) &&
            !VersionMatches(profile.Unity, detectedUnityVersion))
        {
            issues.Add(new CompatibilityIssue(
                "UNITY_VERSION_MISMATCH",
                $"Expected Unity {profile.Unity.Version}, detected {detectedUnityVersion}.",
                IsFatal: true));
        }

        if (!string.IsNullOrWhiteSpace(detectedArchitecture) &&
            profile.Runtime.Architectures.Count > 0 &&
            !profile.Runtime.Architectures.Contains(detectedArchitecture, StringComparer.OrdinalIgnoreCase))
        {
            issues.Add(new CompatibilityIssue(
                "ARCHITECTURE_MISMATCH",
                $"Architecture {detectedArchitecture} is not declared by the profile.",
                IsFatal: true));
        }

        if (!string.IsNullOrWhiteSpace(detectedPlatform) &&
            profile.Runtime.Platforms.Count > 0 &&
            !profile.Runtime.Platforms.Contains(detectedPlatform, StringComparer.OrdinalIgnoreCase))
        {
            issues.Add(new CompatibilityIssue(
                "PLATFORM_MISMATCH",
                $"Platform {detectedPlatform} is not declared by the profile.",
                IsFatal: true));
        }

        foreach (ReferenceCompatibility reference in profile.ReferenceFiles)
        {
            string? resolvedPath = resolveReference(reference);
            if (string.IsNullOrWhiteSpace(resolvedPath) || !File.Exists(resolvedPath))
            {
                allSizesVerified = false;
                allHashesVerified = false;
                issues.Add(new CompatibilityIssue(
                    "REFERENCE_MISSING",
                    $"Reference file is missing: {reference.Path}",
                    reference.Required,
                    resolvedPath ?? reference.Path));
                continue;
            }

            var file = new FileInfo(resolvedPath);
            if (reference.Size.HasValue && file.Length != reference.Size.Value)
            {
                allSizesVerified = false;
                allHashesVerified = false;
                issues.Add(new CompatibilityIssue(
                    "REFERENCE_SIZE_MISMATCH",
                    $"{reference.Path}: expected {reference.Size.Value} bytes, found {file.Length} bytes.",
                    reference.Required,
                    resolvedPath));
            }

            if (!string.IsNullOrWhiteSpace(reference.Sha256))
            {
                anyHashDeclared = true;
                string actualHash = ComputeSha256(resolvedPath);
                if (!actualHash.Equals(reference.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    allHashesVerified = false;
                    issues.Add(new CompatibilityIssue(
                        "REFERENCE_HASH_MISMATCH",
                        $"SHA-256 mismatch for {reference.Path}.",
                        reference.Required,
                        resolvedPath));
                }
            }
            else
            {
                allHashesVerified = false;
            }
        }

        bool fatal = issues.Any(issue => issue.IsFatal);
        CompatibilityLevel level = fatal
            ? CompatibilityLevel.Incompatible
            : anyHashDeclared && allHashesVerified
                ? CompatibilityLevel.HashVerified
                : allSizesVerified
                    ? CompatibilityLevel.SizeVerified
                    : CompatibilityLevel.Declared;

        return new CompatibilityReport
        {
            ProfileId = profile.ProfileId,
            Level = level,
            SafeMode = fatal,
            Issues = issues
        };
    }

    private static bool VersionMatches(UnityCompatibility expected, string detected)
    {
        if (expected.ExactVersionKnown)
            return expected.Version.Equals(detected, StringComparison.OrdinalIgnoreCase);

        return detected.StartsWith(expected.Version, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeSha256(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using SHA256 algorithm = SHA256.Create();
        return Convert.ToHexString(algorithm.ComputeHash(stream)).ToLowerInvariant();
    }
}
