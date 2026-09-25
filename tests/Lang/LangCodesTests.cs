/// <file-summary>
/// Layer:       Tests
/// Purpose:     Tests for LangCodes normalization (pure logic, no game).
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.Lang;

namespace gregCore.Tests.Lang;

public class LangCodesTests
{
    [Theory]
    [InlineData("de", "de")]
    [InlineData("DE", "de")]
    [InlineData("de-DE", "de")]
    [InlineData("en-US", "en")]
    public void Normalize_Codes_PassThrough(string input, string expected)
    {
        LangCodes.Normalize(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("German", "de")]
    [InlineData("german", "de")]
    [InlineData("English", "en")]
    [InlineData("French", "fr")]
    public void Normalize_SystemNames_MapToCodes(string input, string expected)
    {
        LangCodes.Normalize(input).Should().Be(expected);
    }

    [Fact]
    public void Normalize_Auto_IsKept()
    {
        LangCodes.Normalize("auto").Should().Be(LangCodes.Auto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Klingon")]
    [InlineData("../../etc")]
    public void Normalize_Unknown_ReturnsFallback(string input)
    {
        LangCodes.Normalize(input).Should().Be(LangCodes.Fallback);
    }
}
