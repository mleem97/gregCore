/// <file-summary>
/// Layer:       Tests
/// Purpose:     Tests for GregLangStore fallback chain (temp dirs on disk,
///              no game). Chain: &lt;lang&gt;.json -> en.json -> caller default.
/// </file-summary>

using System;
using System.IO;
using Xunit;
using FluentAssertions;
using gregCore.Lang;

namespace gregCore.Tests.Lang;

public class GregLangStoreTests : IDisposable
{
    private readonly string _root;
    private readonly GregLangStore _store;

    public GregLangStoreTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "gregLangTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "gregMod.Demo"));
        File.WriteAllText(
            Path.Combine(_root, "gregMod.Demo", "en.json"),
            "{ \"panel.title\": \"Demo\", \"value.money\": \"Money: {0}\" }");
        File.WriteAllText(
            Path.Combine(_root, "gregMod.Demo", "de.json"),
            "{ \"panel.title\": \"DEMO\" }");
        File.WriteAllText(
            Path.Combine(_root, "gregMod.Demo", "xx.json"),
            "{ broken json,,");
        _store = new GregLangStore(_root);
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, true); } catch { /* best-effort */ }
    }

    [Fact]
    public void Translate_CurrentLanguage_HitsTable()
    {
        _store.Translate("gregMod.Demo", "panel.title", "D", "de").Should().Be("DEMO");
    }

    [Fact]
    public void Translate_MissingKey_FallsBackToEnglishThenDefault()
    {
        _store.Translate("gregMod.Demo", "value.money", "D", "de", 42).Should().Be("Money: 42");
        _store.Translate("gregMod.Demo", "nope", "DEF", "de").Should().Be("DEF");
    }

    [Fact]
    public void Translate_BrokenOrMissingFile_DegradesGracefully()
    {
        _store.Translate("gregMod.Demo", "panel.title", "D", "xx").Should().Be("Demo");
        _store.Translate("gregMod.Demo", "panel.title", "D", "fr").Should().Be("Demo");
        _store.Translate("gregMod.Absent", "k", "DEF", "de").Should().Be("DEF");
    }

    [Fact]
    public void Translate_UnsafeModId_ReturnsDefault()
    {
        _store.Translate("../../etc", "k", "SAFE", "de").Should().Be("SAFE");
        _store.Translate("gregMod.Demo", "", "EMPTY", "de").Should().Be("EMPTY");
    }

    [Fact]
    public void AvailableLanguages_ListsTranslationFiles()
    {
        _store.AvailableLanguages("gregMod.Demo").Should().BeEquivalentTo("de", "en", "xx");
        _store.AvailableLanguages("gregMod.Absent").Should().BeEmpty();
    }
}
