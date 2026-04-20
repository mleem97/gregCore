/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für den GregEventBus.
/// Maintainer:   Stellt Thread-Safety und Funktionalität sicher.
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.Tests.Mocks;

namespace gregCore.Tests.Events;

public class GregEventBusTests
{
    [Fact]
    public void SubscribeAndPublish_ShouldInvokeHandler()
    {
        var bus = new GregEventBus(new MockLogger());
        var invoked = false;

        bus.Subscribe("test.hook", p => invoked = true);
        bus.Publish("test.hook", new EventPayload());

        invoked.Should().BeTrue();
    }

    [Fact]
    public void Unsubscribe_ShouldNotInvokeHandler()
    {
        var bus = new GregEventBus(new MockLogger());
        var invoked = false;
        Action<EventPayload> handler = p => invoked = true;

        bus.Subscribe("test.hook", handler);
        bus.Unsubscribe("test.hook", handler);
        bus.Publish("test.hook", new EventPayload());

        invoked.Should().BeFalse();
    }

    [Fact]
    public void CancelableEvent_ShouldReturnFalseWhenCancelled()
    {
        var bus = new GregEventBus(new MockLogger());

        bus.Subscribe("test.hook", p => p.IsCancelled = true);
        var result = bus.Publish("test.hook", new EventPayload { IsCancelable = true });

        result.Should().BeFalse();
    }
}
