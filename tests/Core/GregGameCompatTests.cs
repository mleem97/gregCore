/// <file-summary>
/// Layer:      Tests
/// Purpose:    Tests for the game-build compatibility latch (fail-safe
///             default) and for stable gregID derivation (deterministic
///             live/save correlation for the Hardware-ID system).
/// Maintainer: Each test sets the latch explicitly first: xunit may run
///             tests in any order and the latch is process-wide static.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using gregCore.Core.Diagnostics;
using gregCore.GameLayer.Patches.Hardware;

namespace gregCore.Tests.Core;

public class GregGameCompatTests
{
    [Fact]
    public void HwIdRewrites_WithoutVerdict_AreDisabled()
    {
        // Unknown build (no verdict yet, e.g. Doctor failed): fail safe.
        GregGameCompat.MarkEvaluated(false);

        GregGameCompat.HwIdRewritesAllowed.Should().BeFalse();
        GregGameCompat.IsSupportedBuild.Should().BeFalse();
    }

    [Fact]
    public void HwIdRewrites_WithSupportedVerdict_AreAllowed()
    {
        GregGameCompat.MarkEvaluated(true);

        try
        {
            GregGameCompat.IsSupportedBuild.Should().BeTrue();
            GregGameCompat.HwIdRewritesAllowed.Should().BeTrue();
        }
        finally
        {
            GregGameCompat.MarkEvaluated(false);
        }
    }

    [Fact]
    public void HwIdRewrites_WithUnsupportedVerdict_AreDisabled()
    {
        GregGameCompat.MarkEvaluated(false);

        GregGameCompat.HwIdRewritesAllowed.Should().BeFalse();
    }

    [Fact]
    public void NotifyHwIdGate_MirrorsAllowedFlag()
    {
        GregGameCompat.MarkEvaluated(true);
        try
        {
            GregGameCompat.NotifyHwIdGate().Should().BeTrue();
        }
        finally
        {
            GregGameCompat.MarkEvaluated(false);
        }

        GregGameCompat.NotifyHwIdGate().Should().BeFalse();
    }

    [Fact]
    public void StableGregId_SameInput_YieldsSameId()
    {
        string a = HardwareIdPersistencePatch.GenerateStableGregId("gregID:Switch:", "Switch16CU_-1573390");
        string b = HardwareIdPersistencePatch.GenerateStableGregId("gregID:Switch:", "Switch16CU_-1573390");

        a.Should().Be(b);
    }

    [Fact]
    public void StableGregId_MatchesSchema()
    {
        string id = HardwareIdPersistencePatch.GenerateStableGregId("gregID:PatchPanel:", "PatchPanel_fiber_-1448700");

        id.Should().StartWith("gregID:PatchPanel:");
        id.Substring("gregID:PatchPanel:".Length).Should().MatchRegex("^[0-9A-F]{12}$");
    }

    [Fact]
    public void StableGregId_DistinctInputs_YieldDistinctIds()
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < 500; i++)
            ids.Add(HardwareIdPersistencePatch.GenerateStableGregId("gregID:Server:", $"Server.Green2_-{1000000 + i}"));

        ids.Should().HaveCount(500);
    }

    [Fact]
    public void StableGregId_EmptyInput_FallsBackToRandomSchemaId()
    {
        string id = HardwareIdPersistencePatch.GenerateStableGregId("gregID:Switch:", "");

        id.Should().StartWith("gregID:Switch:");
        id.Substring("gregID:Switch:".Length).Should().MatchRegex("^[0-9A-F]{12}$");
    }

    [Fact]
    public void StableGregId_DiffersFromLegacyInput()
    {
        // The derived ID must never equal the legacy input (else the
        // HasPrefix skip-logic could misfire on a second pass).
        string legacy = "Switch16CU_-1573390";
        string id = HardwareIdPersistencePatch.GenerateStableGregId("gregID:Switch:", legacy);

        id.Should().NotBe(legacy);
    }
}
