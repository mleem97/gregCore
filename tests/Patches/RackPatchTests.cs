/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für RackPatch Position-Registry-Logik.
/// Maintainer:   Testet MarkPositionUsed/Free, GetUsedCount, ClearRack und Thread-Safety.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using gregCore.GameLayer.Patches.Hardware;

namespace gregCore.Tests.Patches;

public class RackPatchTests
{
    [Fact]
    public void MarkPositionUsed_ShouldTrackPosition()
    {
        int rackId = 42;
        
        RackPatch.ClearRack(rackId); // Clean state
        RackPatch.MarkPositionUsed(rackId, 0);
        
        RackPatch.GetUsedCount(rackId).Should().Be(1);
        
        // Cleanup
        RackPatch.ClearRack(rackId);
    }

    [Fact]
    public void MarkPositionFree_ShouldRemovePosition()
    {
        int rackId = 43;
        
        RackPatch.ClearRack(rackId);
        RackPatch.MarkPositionUsed(rackId, 0);
        RackPatch.MarkPositionUsed(rackId, 1);
        RackPatch.MarkPositionFree(rackId, 0);
        
        RackPatch.GetUsedCount(rackId).Should().Be(1);
        
        RackPatch.ClearRack(rackId);
    }

    [Fact]
    public void ClearRack_ShouldRemoveAllPositions()
    {
        int rackId = 44;
        
        RackPatch.MarkPositionUsed(rackId, 0);
        RackPatch.MarkPositionUsed(rackId, 1);
        RackPatch.MarkPositionUsed(rackId, 2);
        
        RackPatch.ClearRack(rackId);
        
        RackPatch.GetUsedCount(rackId).Should().Be(0);
    }

    [Fact]
    public void MarkPositionUsed_DuplicatePosition_ShouldNotDoubleCcount()
    {
        int rackId = 45;
        
        RackPatch.ClearRack(rackId);
        RackPatch.MarkPositionUsed(rackId, 0);
        RackPatch.MarkPositionUsed(rackId, 0); // Duplicate
        
        RackPatch.GetUsedCount(rackId).Should().Be(1);
        
        RackPatch.ClearRack(rackId);
    }

    [Fact]
    public void MarkPositionFree_NonExistentRack_ShouldNotThrow()
    {
        int rackId = 99999;
        
        var action = () => RackPatch.MarkPositionFree(rackId, 0);
        action.Should().NotThrow();
    }

    [Fact]
    public async Task ConcurrentAccess_ShouldBeThreadSafe()
    {
        int rackId = 46;
        RackPatch.ClearRack(rackId);
        
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            int pos = i;
            tasks.Add(Task.Run(() => RackPatch.MarkPositionUsed(rackId, pos)));
        }
        
        await Task.WhenAll(tasks);
        
        RackPatch.GetUsedCount(rackId).Should().Be(100);
        
        RackPatch.ClearRack(rackId);
    }
}
