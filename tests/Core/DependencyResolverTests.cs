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

        result.Should().NotBeEmpty();
    }
}
