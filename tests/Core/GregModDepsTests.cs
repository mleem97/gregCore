/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für GregModDeps (Versionen, Manifest-Diff) — rein
///               verwaltete Logik, ohne Spiel lauffähig.
/// </file-summary>

using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using gregCore.Core.Mods;

namespace gregCore.Tests.Core;

public class GregModDepsTests
{
    [Theory]
    [InlineData("1.0.0", "1.0.0", true)]
    [InlineData("1.0.1", "1.0.0", true)]
    [InlineData("1.0.0", "1.0.1", false)]
    [InlineData("2.0", "1.9.9", true)]
    [InlineData("1.0", "1.0.0", true)]
    [InlineData("v1.2.3", "1.2.3", true)]
    [InlineData("1.2.3-beta", "1.2.3", true)]
    [InlineData("", "1.0.0", false)]
    [InlineData("1.0.0", "", true)]
    public void IsVersionAtLeast_ComparesNumerically(string have, string need, bool expected)
    {
        GregModDeps.IsVersionAtLeast(have, need).Should().Be(expected);
    }

    [Fact]
    public void DiffManifests_Identical_IsCompatible()
    {
        var local = new List<GregModDeps.ModEntry>
        {
            new GregModDeps.ModEntry { Id = "gregCore", Version = "1.2.3" },
        };
        var remote = new List<GregModDeps.ModEntry>
        {
            new GregModDeps.ModEntry { Id = "gregcore", Version = "1.2.3" },
        };

        var diff = GregModDeps.DiffManifests(local, remote);

        diff.Compatible.Should().BeTrue();
        GregModDeps.FormatDiff(diff).Should().Contain("kompatibel");
    }

    [Fact]
    public void DiffManifests_FindsMissingAndMismatches()
    {
        var local = new List<GregModDeps.ModEntry>
        {
            new GregModDeps.ModEntry { Id = "gregCore", Version = "1.2.3" },
            new GregModDeps.ModEntry { Id = "gregMod.IPAM", Version = "0.7.6" },
        };
        var remote = new List<GregModDeps.ModEntry>
        {
            new GregModDeps.ModEntry { Id = "gregCore", Version = "1.2.4" },
            new GregModDeps.ModEntry { Id = "gregMod.MusicPlayer", Version = "1.0.0" },
        };

        var diff = GregModDeps.DiffManifests(local, remote);

        diff.Compatible.Should().BeFalse();
        diff.MissingOnRemote.Should().ContainSingle(e => e.Id == "gregMod.IPAM");
        diff.MissingLocally.Should().ContainSingle(e => e.Id == "gregMod.MusicPlayer");
        diff.VersionMismatches.Should().ContainSingle(s => s.Contains("gregCore"));
    }

    [Fact]
    public void DiffManifests_NullSafe()
    {
        var diff = GregModDeps.DiffManifests(null, null);

        diff.Compatible.Should().BeTrue();
    }
}
