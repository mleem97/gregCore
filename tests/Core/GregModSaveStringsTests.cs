/// <file-summary>
/// Layer:       Tests
/// Purpose:     Tests for GregModSaveStrings (pure codec logic, without the game
///               runnable). The Il2Cpp glue (UpsertText/TryReadText) needs
///               the running game and is excluded from coverage.
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.Core.Mods;

namespace gregCore.Tests.Core;

public class GregModSaveStringsTests
{
    [Fact]
    public void Encode_NullOrEmpty_ReturnsEmpty()
    {
        GregModSaveStrings.Encode(null).Should().BeEmpty();
        GregModSaveStrings.Encode("").Should().BeEmpty();
    }

    [Fact]
    public void EncodeDecode_RoundTrips_Text()
    {
        const string text = "10.0.0.5\nShop #42\nÄpfel & Zebra-Ünïcödé!";
        var decoded = GregModSaveStrings.Decode(GregModSaveStrings.Encode(text));
        decoded.Should().Be(text);
    }

    [Fact]
    public void Encode_RespectsMaxChars()
    {
        var units = GregModSaveStrings.Encode("abcdef", maxChars: 3);
        units.Should().BeEquivalentTo(new[] { (int)'a', (int)'b', (int)'c' });
        GregModSaveStrings.Decode(units).Should().Be("abc");
    }

    [Fact]
    public void Decode_Sanitizes_OutOfRangeUnits()
    {
        var decoded = GregModSaveStrings.Decode(new[] { 65, -1, 0x110000, 66 });
        decoded.Should().Be("A??B");
    }

    [Fact]
    public void Decode_NullOrEmpty_ReturnsEmpty()
    {
        GregModSaveStrings.Decode(null).Should().Be("");
        GregModSaveStrings.Decode(new int[0]).Should().Be("");
    }

    [Fact]
    public void CombineSplitTitleBody_RoundTrips()
    {
        var combined = GregModSaveStrings.CombineTitleBody("NOTES", "line1\nline2");
        GregModSaveStrings.SplitTitleBody(combined, out var title, out var body);
        title.Should().Be("NOTES");
        body.Should().Be("line1\nline2");
    }

    [Fact]
    public void SplitTitleBody_NoNewline_YieldsTitleOnly()
    {
        GregModSaveStrings.SplitTitleBody("just-a-title", out var title, out var body);
        title.Should().Be("just-a-title");
        body.Should().Be("");
    }

    [Fact]
    public void SplitTitleBody_Null_YieldsEmpty()
    {
        GregModSaveStrings.SplitTitleBody(null, out var title, out var body);
        title.Should().Be("");
        body.Should().Be("");
    }
}
