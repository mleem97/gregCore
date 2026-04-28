/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für CablePositionsPatch ID-Generierung.
/// Maintainer:   Testet Uniqueness, Thread-Safety und SetBaseId.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using gregCore.GameLayer.Patches.Networking;

namespace gregCore.Tests.Patches;

public class CablePatchTests
{
    [Fact]
    public void SetBaseId_ShouldUpdateCounter()
    {
        int baseBefore = CablePositionsPatch.PeekNextId();
        int highBase = baseBefore + 1000;
        
        CablePositionsPatch.SetBaseId(highBase);
        
        CablePositionsPatch.PeekNextId().Should().Be(highBase + 1);
    }

    [Fact]
    public void SetBaseId_LowerValue_ShouldNotDecrease()
    {
        int current = CablePositionsPatch.PeekNextId();
        
        CablePositionsPatch.SetBaseId(1); // Much lower
        
        CablePositionsPatch.PeekNextId().Should().BeGreaterThanOrEqualTo(current);
    }

    [Fact]
    public async Task ConcurrentSetBaseId_ShouldRemainConsistent()
    {
        int startBase = CablePositionsPatch.PeekNextId();
        
        var tasks = new List<Task>();
        for (int i = 0; i < 50; i++)
        {
            int baseVal = startBase + i * 100;
            tasks.Add(Task.Run(() => CablePositionsPatch.SetBaseId(baseVal)));
        }
        
        await Task.WhenAll(tasks);
        
        // Should be at least the highest value set
        CablePositionsPatch.PeekNextId().Should().BeGreaterThanOrEqualTo(startBase + 49 * 100 + 1);
    }
}
