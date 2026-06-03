using System;
using System.IO;
using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.Scripting;

namespace gregCore.Tests.Infrastructure;

public class GregSandboxHelperTests : IDisposable
{
    private string _testDir;

    public GregSandboxHelperTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "gregCore_sandbox_tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Fact]
    public void IsPathInsideDirectory_ValidSubPath_ReturnsTrue()
    {
        string baseDir = Path.Combine(_testDir, "mods", "modA", "data");
        string fullPath = Path.Combine(baseDir, "file.txt");

        GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir).Should().BeTrue();
    }

    [Fact]
    public void IsPathInsideDirectory_SameDirectory_ReturnsTrue()
    {
        string baseDir = Path.Combine(_testDir, "mods", "modA", "data");

        GregSandboxHelper.IsPathInsideDirectory(baseDir, baseDir).Should().BeTrue();
    }

    [Fact]
    public void IsPathInsideDirectory_PrefixMatchBypass_ReturnsFalse()
    {
        string baseDir = Path.Combine(_testDir, "mods", "modA", "data");
        string maliciousDir = Path.Combine(_testDir, "mods", "modA", "data-secret");
        string fullPath = Path.Combine(maliciousDir, "file.txt");

        GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir).Should().BeFalse();
    }

    [Fact]
    public void IsPathInsideDirectory_PathTraversalUp_ReturnsFalse()
    {
        string baseDir = Path.Combine(_testDir, "mods", "modA", "data");
        string fullPath = Path.GetFullPath(Path.Combine(baseDir, "..", "secret.txt"));

        GregSandboxHelper.IsPathInsideDirectory(fullPath, baseDir).Should().BeFalse();
    }

    [Fact]
    public void IsPathInsideDirectory_NullOrEmpty_ReturnsFalse()
    {
        GregSandboxHelper.IsPathInsideDirectory(null, "some/path").Should().BeFalse();
        GregSandboxHelper.IsPathInsideDirectory("some/path", null).Should().BeFalse();
        GregSandboxHelper.IsPathInsideDirectory("", "some/path").Should().BeFalse();
    }
}
