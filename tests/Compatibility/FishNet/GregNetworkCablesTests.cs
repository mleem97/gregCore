using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.Core.Abstractions;
using gregCore.Compatibility.FishNet;
using gregCore.Tests.Mocks;

namespace gregCore.Tests.Compatibility.FishNet;

public class GregNetworkCablesTests : IDisposable
{
    private readonly IGregLogger _logger;
    private readonly GregEventBus _eventBus;
    private readonly GregNetworkCables _sut;

    public GregNetworkCablesTests()
    {
        _logger = new MockLogger();
        _eventBus = new GregEventBus(_logger);
        _sut = new GregNetworkCables(_eventBus, _logger);
    }

    [Fact]
    public void RegisterCable_WithValidCableData_ShouldAddCableToSyncList()
    {
        // Arrange
        var cableData = new SyncedCableData
        {
            CableId = 42,
            StartDeviceId = "ServerA",
            EndDeviceId = "SwitchB",
            StartPort = 1,
            EndPort = 2,
            BezierPoints = new float[] { 0f, 0f, 0f, 1f, 1f, 1f }
        };

        // Act
        _sut.RegisterCable(cableData);

        // Assert
        _sut.CableCount.Should().Be(1);
        var cables = _sut.GetAllCables();
        var registeredCable = cables.FirstOrDefault();
        registeredCable.Should().NotBeNull();
        registeredCable!.CableId.Should().Be(42);
        registeredCable.StartDeviceId.Should().Be("ServerA");
        registeredCable.EndDeviceId.Should().Be("SwitchB");
    }

    [Fact]
    public void RegisterCable_WithValidCableData_ShouldPublishEvent()
    {
        // Arrange
        var cableData = new SyncedCableData
        {
            CableId = 42,
            StartDeviceId = "ServerA",
            EndDeviceId = "SwitchB",
            StartPort = 1,
            EndPort = 2,
            BezierPoints = new float[] { 0f, 0f, 0f, 1f, 1f, 1f }
        };

        bool eventPublished = false;
        EventPayload? publishedPayload = null;

        _eventBus.Subscribe("greg.NET.CableRegistered", payload =>
        {
            eventPublished = true;
            publishedPayload = payload;
        });

        // Act
        _sut.RegisterCable(cableData);

        // Assert
        eventPublished.Should().BeTrue();
        publishedPayload.Should().NotBeNull();
        publishedPayload!.Data.Should().ContainKey("CableId").WhoseValue.Should().Be(42);
        publishedPayload.Data.Should().ContainKey("StartDevice").WhoseValue.Should().Be("ServerA");
        publishedPayload.Data.Should().ContainKey("EndDevice").WhoseValue.Should().Be("SwitchB");
        publishedPayload.Data.Should().ContainKey("BezierPointCount").WhoseValue.Should().Be(2);
    }

    [Fact]
    public void RegisterCable_WithNullData_ShouldNotAddCable()
    {
        // Act
        _sut.RegisterCable(null!);

        // Assert
        _sut.CableCount.Should().Be(0);
    }

    [Fact]
    public void RegisterCable_WhenDisposed_ShouldNotAddCable()
    {
        // Arrange
        var cableData = new SyncedCableData { CableId = 42 };
        _sut.Dispose();

        // Act
        _sut.RegisterCable(cableData);

        // Assert
        _sut.CableCount.Should().Be(0);
    }

    [Fact]
    public void RegisterCable_WithDuplicateId_ShouldUpdateExistingCable()
    {
        // Arrange
        var firstCable = new SyncedCableData { CableId = 1, StartDeviceId = "A" };
        var updatedCable = new SyncedCableData { CableId = 1, StartDeviceId = "B" };

        _sut.RegisterCable(firstCable);

        // Act
        _sut.RegisterCable(updatedCable);

        // Assert
        _sut.CableCount.Should().Be(1);
        var cables = _sut.GetAllCables();
        cables.First().StartDeviceId.Should().Be("B");
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _eventBus?.Dispose();
    }
}
