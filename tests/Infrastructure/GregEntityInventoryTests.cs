/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Abdeckung der spielunabhaengigen Inventar-Helfer
///               (UID-Format, Keys, TSV-Roundtrip). Kein Spiel noetig.
/// Maintainer:   Alles hier muss 100 % Zeilen-Coverage halten.
/// </file-summary>

using System;
using FluentAssertions;
using Xunit;
using gregCore.Infrastructure.Persistence;

namespace gregCore.Tests.Infrastructure;

public class GregEntityInventoryTests
{
    [Theory]
    [InlineData("gregUID:Cable:42", true)]
    [InlineData("greguid:server:abc", true)]
    [InlineData("gregID:Server:D4AB126B7F70", true)]
    [InlineData("gregID:Switch:001122334455", true)]
    [InlineData("gregID:PatchPanel:001122334455", true)]
    [InlineData("Server.Yellow1", false)]
    [InlineData("", false)]
    [InlineData("gregID:Router:001122334455", false)]
    public void IsGregUid_ClassifiesPrefixes(string uid, bool expected)
    {
        GregEntityInventory.IsGregUid(uid).Should().Be(expected);
    }

    [Theory]
    [InlineData("gregID:Server:ABC", "gregID:Server:", true)]
    [InlineData("gregid:server:abc", "gregID:Server:", true)]
    [InlineData("gregUID:Server:ABC", "gregID:Server:", false)]
    [InlineData("", "gregID:Server:", false)]
    public void IsGregDeviceUid_MatchesPrefixCaseInsensitive(string uid, string prefix, bool expected)
    {
        GregEntityInventory.IsGregDeviceUid(uid, prefix).Should().Be(expected);
    }

    [Fact]
    public void NewUid_HasKindAndTwelveHexDigits()
    {
        string uid = GregEntityInventory.NewUid(GregEntityInventory.InventoryKind.Router);
        uid.Should().StartWith("gregUID:Router:");
        uid.Substring("gregUID:Router:".Length).Should().MatchRegex("^[0-9A-F]{12}$");
        GregEntityInventory.NewUid(GregEntityInventory.InventoryKind.Router).Should().NotBe(uid);
    }

    [Fact]
    public void NewHex_IsDeterministicTwelveHex()
    {
        string a = GregEntityInventory.NewHex("Server.Yellow1");
        string b = GregEntityInventory.NewHex("Server.Yellow1");
        a.Should().Be(b);
        a.Should().MatchRegex("^[0-9A-F]{12}$");
        GregEntityInventory.NewHex("Server.Blue1").Should().NotBe(a);
    }

    [Fact]
    public void KindKey_CombinesKindAndTrimmedKey()
    {
        GregEntityInventory.KindKey(GregEntityInventory.InventoryKind.Cable, "  cable#7 ")
            .Should().Be("Cable\ncable#7");
    }

    [Fact]
    public void SanitizeField_RemovesTsvBreakers()
    {
        GregEntityInventory.SanitizeField("a\tb\rc\nd").Should().Be("a b c d");
        GregEntityInventory.SanitizeField(null!).Should().Be("");
    }

    [Theory]
    [InlineData("ID gregID:Server:D4AB126B7F70 online", "Server.Yellow1", "ID Server.Yellow1 online")]
    [InlineData("plain vanilla text", "Server.Yellow1", "plain vanilla text")]
    [InlineData("", "Server.Yellow1", "")]
    [InlineData("gregID:Switch:ABC_def-1", "Switch", "Switch")]
    public void ScrubGregIds_ReplacesOnlyTokens(string text, string replacement, string expected)
    {
        GregEntityInventory.ScrubGregIds(text, replacement).Should().Be(expected);
    }

    [Theory]
    [InlineData("Server.Yellow1(Clone)", "Server.Yellow1")]
    [InlineData("Server.Yellow1 (1)", "Server.Yellow1")]
    [InlineData("Server.Yellow1", "Server.Yellow1")]
    [InlineData("gregID:Server:D4AB126B7F70", "")]
    [InlineData("", "")]
    public void CleanDisplayName_StripsCloneAndDuplicateSuffix(string name, string expected)
    {
        GregEntityInventory.CleanDisplayName(name).Should().Be(expected);
    }

    [Fact]
    public void ParseUidMap_RoundtripSkipsCommentsAndJunk()
    {
        string tsv = "# gregUID inventory\n"
            + "\n"
            + "Router\trouter#0\tgregUID:Router:AABBCCDDEEFF\tasn:1/routes:2\n"
            + "Firewall\tfirewall#0\tnot-a-uid\tcluster:x\n"
            + "broken-line\n"
            + "SfpModule\tsfp#3\tgregUID:SfpModule:001122334455\t\n";
        var (uids, hints) = GregEntityInventory.ParseUidMap(tsv);
        uids.Should().HaveCount(2);
        uids["Router\nrouter#0"].Should().Be("gregUID:Router:AABBCCDDEEFF");
        hints["Router\nrouter#0"].Should().Be("asn:1/routes:2");
        uids["SfpModule\nsfp#3"].Should().Be("gregUID:SfpModule:001122334455");
        uids.Should().NotContainKey("Firewall\nfirewall#0");
    }

    [Fact]
    public void ParseMap_SerializeMap_PersistEntries()
    {
        string tsv = "Cable\tcable#9911\tgregUID:Cable:9911\t\n";
        GregEntityInventory.ParseMap(tsv);
        string uid;
        GregEntityInventory.TryGetUid(GregEntityInventory.InventoryKind.Cable, "cable#9911", out uid)
            .Should().BeFalse("nur Sidecar-Map, kein Rebuild");
        string serialized = GregEntityInventory.SerializeMap();
        serialized.Should().Contain("cable#9911").And.Contain("gregUID:Cable:9911");
        GregEntityInventory.IsGregUid("gregUID:Cable:9911").Should().BeTrue();
    }
}
