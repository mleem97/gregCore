using gregCore.GameApi;
using Xunit;
using FluentAssertions;

namespace gregCore.Tests.Core;

public sealed class GameApiRegistryTests
{
    [Fact]
    public void Registry_ShouldContainAllGeneratedModules()
    {
        GregGameApiRegistry.Modules.Should().NotBeEmpty();
        GregGameApiRegistry.Modules.Should().HaveCount(GregGameApiRegistry.ModuleCount);
    }

    [Fact]
    public void Registry_GameTypeNames_ShouldBeUniqueAndSorted()
    {
        var names = GregGameApiRegistry.Modules.Select(m => m.GameTypeName).ToArray();
        names.Should().OnlyHaveUniqueItems();
        names.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Should().BeEquivalentTo(names, o => o.WithStrictOrdering());
    }

    [Fact]
    public void Registry_GenerationManifest_ShouldBePresent()
    {
        GregGameApiRegistry.SourceAssembly.Should().Be("Assembly-CSharp.dll");
        GregGameApiRegistry.SourceSha256.Should().MatchRegex("^[0-9a-f]{64}$");
        GregGameApiRegistry.GeneratedUtc.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void TryGetModule_KnownType_ShouldResolve()
    {
        var module = GregGameApiRegistry.TryGetModule("Il2Cpp.MainGameManager");
        module.Should().NotBeNull();
        module!.Kind.Should().Be("Component");
        module.IsSingleton.Should().BeTrue();
    }

    [Fact]
    public void TryGetModule_UnknownType_ShouldReturnNull()
    {
        GregGameApiRegistry.TryGetModule("Does.Not.Exist").Should().BeNull();
        GregGameApiRegistry.TryGetModule("").Should().BeNull();
    }
}
