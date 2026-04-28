/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Erweiterte Tests für den GregEventBus.
/// Maintainer:   Testet Deferred Events, Dispose-Safety und Multi-Handler-Szenarien.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.Tests.Mocks;

namespace gregCore.Tests.Events;

public class GregEventBusExtendedTests
{
    [Fact]
    public void Publish_WithNoSubscribers_ShouldReturnTrue()
    {
        using var bus = new GregEventBus(new MockLogger());
        
        var result = bus.Publish("unknown.hook", new EventPayload
        {
            HookName = "unknown.hook",
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>()
        });
        
        result.Should().BeTrue();
    }

    [Fact]
    public void MultipleHandlers_ShouldAllBeInvoked()
    {
        using var bus = new GregEventBus(new MockLogger());
        int count = 0;
        
        bus.Subscribe("test.multi", _ => Interlocked.Increment(ref count));
        bus.Subscribe("test.multi", _ => Interlocked.Increment(ref count));
        bus.Subscribe("test.multi", _ => Interlocked.Increment(ref count));
        
        bus.Publish("test.multi", new EventPayload
        {
            HookName = "test.multi",
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>()
        });
        
        count.Should().Be(3);
    }

    [Fact]
    public void HandlerThrows_ShouldNotAffectOtherHandlers()
    {
        using var bus = new GregEventBus(new MockLogger());
        bool secondHandlerCalled = false;
        
        bus.Subscribe("test.error", _ => throw new InvalidOperationException("boom"));
        bus.Subscribe("test.error", _ => secondHandlerCalled = true);
        
        bus.Publish("test.error", new EventPayload
        {
            HookName = "test.error",
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>()
        });
        
        secondHandlerCalled.Should().BeTrue();
    }

    [Fact]
    public void Dispose_ShouldPreventFurtherPublish()
    {
        var bus = new GregEventBus(new MockLogger());
        bool called = false;
        
        bus.Subscribe("test.dispose", _ => called = true);
        bus.Dispose();
        
        bus.Publish("test.dispose", new EventPayload());
        
        called.Should().BeFalse();
    }

    [Fact]
    public void Dispose_ShouldPreventFurtherSubscribe()
    {
        var bus = new GregEventBus(new MockLogger());
        bus.Dispose();
        
        // Should not throw
        var action = () => bus.Subscribe("test.disposed", _ => { });
        action.Should().NotThrow();
    }

    [Fact]
    public void CancelableEvent_FirstHandlerCancels_ShouldStopChain()
    {
        using var bus = new GregEventBus(new MockLogger());
        bool secondCalled = false;
        
        bus.Subscribe("test.cancel", p => p.IsCancelled = true);
        bus.Subscribe("test.cancel", _ => secondCalled = true);
        
        var result = bus.Publish("test.cancel", new EventPayload
        {
            HookName = "test.cancel",
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>(),
            IsCancelable = true
        });
        
        result.Should().BeFalse();
        secondCalled.Should().BeFalse();
    }

    [Fact]
    public void NonCancelableEvent_SettingCancelled_ShouldNotStopChain()
    {
        using var bus = new GregEventBus(new MockLogger());
        bool secondCalled = false;
        
        bus.Subscribe("test.noncancel", p => p.IsCancelled = true);
        bus.Subscribe("test.noncancel", _ => secondCalled = true);
        
        var result = bus.Publish("test.noncancel", new EventPayload
        {
            HookName = "test.noncancel",
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>(),
            IsCancelable = false // Non-cancelable
        });
        
        result.Should().BeTrue();
        secondCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ConcurrentPublish_ShouldBeThreadSafe()
    {
        using var bus = new GregEventBus(new MockLogger());
        int counter = 0;
        
        bus.Subscribe("test.concurrent", _ => Interlocked.Increment(ref counter));
        
        var tasks = Enumerable.Range(0, 100).Select(_ =>
            Task.Run(() => bus.Publish("test.concurrent", new EventPayload
            {
                HookName = "test.concurrent",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>()
            })));
        
        await Task.WhenAll(tasks);
        
        counter.Should().Be(100);
    }
}
