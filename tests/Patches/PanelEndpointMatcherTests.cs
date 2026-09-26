/// <file-summary>
/// Layer:       Tests
/// Purpose:     Regression tests for panel endpoint healing (Mantis #19).
/// Maintainer:   Guards against prefix over-matching: healing panel "Panel1"
///               must never rewrite endpoints of the unrelated panel "Panel12".
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.GameLayer.Patches.Hardware;

namespace gregCore.Tests.Patches;

public class PanelEndpointMatcherTests
{
    [Theory]
    [InlineData("Panel1", "Panel1", true)]          // exact ID
    [InlineData("Panel1:3", "Panel1", true)]        // composite, ':' separator
    [InlineData("Panel1-3", "Panel1", true)]        // composite, '-' separator
    [InlineData("Panel1/3", "Panel1", true)]        // composite, '/' separator
    [InlineData("Panel1 3", "Panel1", true)]        // composite, space separator
    [InlineData("Panel12", "Panel1", false)]        // unrelated device, pure prefix
    [InlineData("Panel12-3", "Panel1", false)]      // unrelated composite
    [InlineData("Panel1", "Panel12", false)]        // shorter endpoint
    [InlineData("OtherPanel1", "Panel1", false)]    // substring, not prefix
    [InlineData("", "Panel1", false)]
    [InlineData("Panel1", "", false)]
    [InlineData("PP-", "PP-", true)]                // ID ending in separator
    [InlineData("PP-3", "PP-", true)]               // separator-terminated prefix
    public void Matches_DistinguishesOwnFromForeignEndpoints(
        string endpoint, string oldId, bool expected)
    {
        GregPanelEndpointMatcher.Matches(endpoint, oldId).Should().Be(expected);
    }

    [Fact]
    public void Remap_RewritesOnlyTheLeadingId()
    {
        // Even if the old ID reappears later in the string (pathological but
        // possible with numeric suffixes), only the leading occurrence moves.
        GregPanelEndpointMatcher.Remap("Panel1-x-Panel1", "Panel1", "gregID:PatchPanel:ABC")
            .Should().Be("gregID:PatchPanel:ABC-x-Panel1");
    }

    [Fact]
    public void Remap_PreservesCompositeSuffix()
    {
        GregPanelEndpointMatcher.Remap("Panel1:3", "Panel1", "gregID:PatchPanel:ABC")
            .Should().Be("gregID:PatchPanel:ABC:3");
    }

    [Fact]
    public void Remap_ExactIdMapsCleanly()
    {
        GregPanelEndpointMatcher.Remap("Panel1", "Panel1", "gregID:PatchPanel:ABC")
            .Should().Be("gregID:PatchPanel:ABC");
    }
}
