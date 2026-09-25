using System;
using System.IO;
using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.IO;

namespace gregCore.Tests.Infrastructure;

public class GregDeactivatedGuardTests
{
    [Theory]
    [InlineData("Mods/.deactivated/foo.dll", true)]
    [InlineData("Mods/.DEACTIVATED/foo.dll", true)]
    [InlineData("Mods/gregNative/.deactivated/foo.dll", true)]
    [InlineData("Mods/foo.dll", false)]
    [InlineData("Mods/gregNative/foo.dll", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void IsDeactivatedPath_DetectsSegment(string? path, bool expected)
    {
        GregDeactivatedGuard.IsDeactivatedPath(path).Should().Be(expected);
    }

    [Fact]
    public void FileSystem_ExcludesDeactivated_ByDefault()
    {
        var root = Path.Combine(Path.GetTempPath(), "gregDeactivated_" + Guid.NewGuid().ToString("N"));
        var active = Path.Combine(root, "Mods");
        var off = Path.Combine(active, ".deactivated");
        Directory.CreateDirectory(active);
        Directory.CreateDirectory(off);
        File.WriteAllText(Path.Combine(active, "a.dll"), "x");
        File.WriteAllText(Path.Combine(off, "b.dll"), "x");
        try
        {
            var top = GregFileSystem.EnumerateFilesByExtension(active, ".dll");
            top.Should().ContainSingle(f => f.EndsWith("a.dll"));

            var all = GregFileSystem.EnumerateFilesByExtension(active, ".dll", SearchOption.AllDirectories);
            all.Should().ContainSingle(f => f.EndsWith("a.dll"));

            var allOptIn = GregFileSystem.EnumerateFilesByExtension(active, ".dll", SearchOption.AllDirectories, includeDeactivated: true);
            allOptIn.Should().HaveCount(2);
        }
        finally
        {
            try { Directory.Delete(root, true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }
    }

    [Fact]
    public void ActivationService_RoundTrips_File()
    {
        var root = Path.Combine(Path.GetTempPath(), "gregActivate_" + Guid.NewGuid().ToString("N"));
        var active = Path.Combine(root, "Mods");
        var off = Path.Combine(active, ".deactivated");
        Directory.CreateDirectory(off);
        var deactivatedFile = Path.Combine(off, "m.dll");
        File.WriteAllText(deactivatedFile, "x");
        try
        {
            // Only gregCore activates: move out, never load in place.
            var target = GregModActivationService.Activate(deactivatedFile);
            File.Exists(target).Should().BeTrue();
            GregDeactivatedGuard.IsDeactivatedPath(target).Should().BeFalse();

            var back = GregModActivationService.Deactivate(target);
            File.Exists(back).Should().BeTrue();
            GregDeactivatedGuard.IsDeactivatedPath(back).Should().BeTrue();
        }
        finally
        {
            try { Directory.Delete(root, true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }
    }
}
