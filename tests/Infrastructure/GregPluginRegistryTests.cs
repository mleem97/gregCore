using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using FluentAssertions;
using NSubstitute;
using gregCore.Infrastructure.Plugins;
using gregCore.Core.Abstractions;

namespace gregCore.Tests.Infrastructure;

public class GregPluginRegistryTests
{
    private readonly IAssemblyScanner _scanner = Substitute.For<IAssemblyScanner>();
    private readonly IGregLogger _logger = Substitute.For<IGregLogger>();
    private readonly IGregEventBus _eventBus = Substitute.For<IGregEventBus>();
    private readonly GregPluginRegistry _registry;

    public GregPluginRegistryTests()
    {
        _logger.ForContext(Arg.Any<string>()).Returns(_logger);
        _registry = new GregPluginRegistry(_scanner, _logger, _eventBus);
    }

    [Fact]
    public void RegisterMod_WhenPersistentIdIsEmpty_ShouldGeneratePersistentIdUsingSha256()
    {
        // Arrange
        var modId = "test-mod";
        var metadata = new ModMetadata
        {
            ModId = modId,
            Name = "Test Mod",
            Version = "1.0.0"
        };

        // Expected GUID calculation
        var fullHash = SHA256.HashData(Encoding.UTF8.GetBytes(modId));
        var guidBytes = new byte[16];
        Array.Copy(fullHash, guidBytes, 16);
        var expectedGuid = new Guid(guidBytes).ToString();

        // Act
        _registry.RegisterMod(metadata);

        // Assert
        metadata.PersistentId.Should().Be(expectedGuid);
    }
}
