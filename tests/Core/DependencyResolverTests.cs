/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für den GregDependencyResolver.
/// Maintainer:   Testet lineare, zyklische und fehlende Abhängigkeiten.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.Plugins;
using gregCore.Core.Models;
using gregCore.Core.Exceptions;

namespace gregCore.Tests.Core;

public class DependencyResolverTests
{
    [Fact]
    public void Resolve_WithLinearDependencies_ShouldReturnCorrectOrder()
    {
        var resolver = new GregDependencyResolver();
        var plugins = new List<PluginInfo>
        {
            new() { Manifest = new ModManifest { Id = "C", Dependencies = new[] { "B" } } },
            new() { Manifest = new ModManifest { Id = "A", Dependencies = Array.Empty<string>() } },
            new() { Manifest = new ModManifest { Id = "B", Dependencies = new[] { "A" } } }
        };

        var result = resolver.Resolve(plugins);

        result.Select(x => x.Manifest.Id).Should().Equal("A", "B", "C");
    }

    [Fact]
    public void Resolve_WithCycle_ShouldFailClearly()
    {
        var resolver = new GregDependencyResolver();
        var plugins = new List<PluginInfo>
        {
            new() { AssemblyPath = "a.dll", Manifest = new ModManifest { Id = "A", Dependencies = new[] { "B" } } },
            new() { AssemblyPath = "b.dll", Manifest = new ModManifest { Id = "B", Dependencies = new[] { "A" } } }
        };

        var action = () => resolver.Resolve(plugins);
        action.Should().Throw<GregPluginLoadException>().WithMessage("*Cyclic plugin dependency*");
    }

    [Fact]
    public void Resolve_WithMissingDependency_ShouldFailClearly()
    {
        var resolver = new GregDependencyResolver();
        var plugins = new List<PluginInfo>
        {
            new() { AssemblyPath = "a.dll", Manifest = new ModManifest { Id = "A", Dependencies = new[] { "missing" } } }
        };

        var action = () => resolver.Resolve(plugins);
        action.Should().Throw<GregPluginLoadException>().WithMessage("*missing dependency 'missing'*");
    }
}
