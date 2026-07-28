using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using FluentAssertions;
using gregCore.Core.Compatibility;
using Xunit;

namespace gregCore.Tests;

public sealed class CompatibilityProfilesTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        "gregcore-compat-tests-" + Guid.NewGuid().ToString("N"));

    public CompatibilityProfilesTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Verify_returns_size_verified_for_matching_required_reference()
    {
        string referencePath = WriteReference("Assembly-CSharp.dll", new byte[] { 1, 2, 3, 4 });
        CompatibilityProfile profile = CreateProfile(
            new ReferenceCompatibility
            {
                Path = "Assembly-CSharp.dll",
                Required = true,
                Size = 4,
                Sha256 = null
            });

        CompatibilityReport report = CompatibilityVerifier.Verify(
            profile,
            reference => reference.Path == "Assembly-CSharp.dll" ? referencePath : null,
            detectedUnityVersion: "6000.5.3f1",
            detectedArchitecture: "x64",
            detectedPlatform: "windows");

        report.Level.Should().Be(CompatibilityLevel.SizeVerified);
        report.SafeMode.Should().BeFalse();
        report.CanLoadGameAdapters.Should().BeTrue();
        report.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Verify_enables_safe_mode_for_required_hash_mismatch()
    {
        string referencePath = WriteReference("Assembly-CSharp.dll", new byte[] { 1, 2, 3, 4 });
        CompatibilityProfile profile = CreateProfile(
            new ReferenceCompatibility
            {
                Path = "Assembly-CSharp.dll",
                Required = true,
                Size = 4,
                Sha256 = new string('0', 64)
            });

        CompatibilityReport report = CompatibilityVerifier.Verify(
            profile,
            _ => referencePath,
            detectedUnityVersion: "6000.5.3f1",
            detectedArchitecture: "x64",
            detectedPlatform: "windows");

        report.Level.Should().Be(CompatibilityLevel.Incompatible);
        report.SafeMode.Should().BeTrue();
        report.CanLoadGameAdapters.Should().BeFalse();
        report.Issues.Should().ContainSingle(issue =>
            issue.Code == "REFERENCE_HASH_MISMATCH" && issue.IsFatal);
    }

    [Fact]
    public void Verify_rejects_unexpected_exact_unity_version()
    {
        string referencePath = WriteReference("Assembly-CSharp.dll", new byte[] { 7 });
        CompatibilityProfile profile = CreateProfile(
            new ReferenceCompatibility
            {
                Path = "Assembly-CSharp.dll",
                Required = true,
                Size = 1
            });
        profile.Unity.Version = "6000.5.3f1";
        profile.Unity.ExactVersionKnown = true;

        CompatibilityReport report = CompatibilityVerifier.Verify(
            profile,
            _ => referencePath,
            detectedUnityVersion: "6000.5.4f1",
            detectedArchitecture: "x64",
            detectedPlatform: "windows");

        report.SafeMode.Should().BeTrue();
        report.Issues.Should().ContainSingle(issue =>
            issue.Code == "UNITY_VERSION_MISMATCH" && issue.IsFatal);
    }

    [Fact]
    public void Verify_returns_hash_verified_when_all_declared_hashes_match()
    {
        byte[] bytes = { 9, 8, 7, 6 };
        string referencePath = WriteReference("Assembly-CSharp.dll", bytes);
        CompatibilityProfile profile = CreateProfile(
            new ReferenceCompatibility
            {
                Path = "Assembly-CSharp.dll",
                Required = true,
                Size = bytes.Length,
                Sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()
            });

        CompatibilityReport report = CompatibilityVerifier.Verify(
            profile,
            _ => referencePath,
            detectedUnityVersion: "6000.5.3f1",
            detectedArchitecture: "x64",
            detectedPlatform: "linux");

        report.Level.Should().Be(CompatibilityLevel.HashVerified);
        report.SafeMode.Should().BeFalse();
    }

    private string WriteReference(string name, byte[] content)
    {
        string path = Path.Combine(_directory, name);
        File.WriteAllBytes(path, content);
        return path;
    }

    private static CompatibilityProfile CreateProfile(ReferenceCompatibility reference)
    {
        return new CompatibilityProfile
        {
            SchemaVersion = 2,
            ProfileId = "test-profile",
            Status = "supported",
            Framework = new FrameworkCompatibility
            {
                VersionLine = "1.2.x",
                MinimumVersion = "1.2.1",
                MaximumVersionExclusive = "2.0.0"
            },
            Game = new GameCompatibility
            {
                Id = "data-center",
                Version = "test"
            },
            Unity = new UnityCompatibility
            {
                Version = "6000.5",
                ExactVersionKnown = false,
                Backend = "IL2CPP"
            },
            Runtime = new RuntimeCompatibility
            {
                Loader = "MelonLoader",
                LoaderVersion = "0.7.x",
                Interop = "Il2CppInterop",
                Architectures = new List<string> { "x64" },
                Platforms = new List<string> { "windows", "linux" }
            },
            ReferenceFiles = new List<ReferenceCompatibility> { reference },
            Features = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
            {
                ["classInjection"] = true
            }
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }
}
