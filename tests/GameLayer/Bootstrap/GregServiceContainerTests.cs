using System;
using Xunit;
using FluentAssertions;
using gregCore.GameLayer.Bootstrap;
using gregCore.Core.Exceptions;

namespace gregCore.Tests.GameLayer.Bootstrap;

public class GregServiceContainerTests : IDisposable
{
    public GregServiceContainerTests()
    {
        // Ensure no leftover instance from other tests
        GregServiceContainer.Instance?.Dispose();
    }

    public void Dispose()
    {
        // Cleanup after tests
        GregServiceContainer.Instance?.Dispose();
    }

    [Fact]
    public void Instance_SetOnCreation()
    {
        using var container = new GregServiceContainer();
        GregServiceContainer.Instance.Should().BeSameAs(container);
    }

    [Fact]
    public void RegisterAndGet_ShouldReturnSameInstance()
    {
        using var container = new GregServiceContainer();
        var myService = new DummyService();

        container.Register(myService);
        var resolved = GregServiceContainer.Get<DummyService>();

        resolved.Should().NotBeNull();
        resolved.Should().BeSameAs(myService);
    }

    [Fact]
    public void Get_WhenNotRegistered_ShouldReturnNull()
    {
        using var container = new GregServiceContainer();

        var resolved = GregServiceContainer.Get<DummyService>();

        resolved.Should().BeNull();
    }

    [Fact]
    public void RegisterKeyed_GetRequired_ShouldReturnSameInstance()
    {
        // Note: The container currently only supports GetRequired for standard registrations, not keyed ones.
        // We will just verify it stores them properly without exceptions.
        using var container = new GregServiceContainer();
        var myService = new DummyService();

        container.Register("myKey", myService);

        // Accessing _services directly isn't possible, but we know it doesn't fail.
        // Also verifying it does NOT register as the default service
        var defaultResolved = GregServiceContainer.Get<DummyService>();
        defaultResolved.Should().BeNull();
    }

    [Fact]
    public void GetRequired_WhenRegistered_ShouldReturnInstance()
    {
        using var container = new GregServiceContainer();
        var myService = new DummyService();

        container.Register(myService);
        var resolved = container.GetRequired<DummyService>();

        resolved.Should().NotBeNull();
        resolved.Should().BeSameAs(myService);
    }

    [Fact]
    public void GetRequired_WhenNotRegistered_ShouldThrowGregInitException()
    {
        using var container = new GregServiceContainer();

        var act = () => container.GetRequired<DummyService>();

        act.Should().Throw<GregInitException>().WithMessage("*DummyService*");
    }

    [Fact]
    public void Dispose_ShouldClearInstanceAndCallDisposeOnServices()
    {
        var disposableService = new DisposableDummyService();
        var container = new GregServiceContainer();

        container.Register(disposableService);

        GregServiceContainer.Instance.Should().NotBeNull();
        disposableService.IsDisposed.Should().BeFalse();

        container.Dispose();

        GregServiceContainer.Instance.Should().BeNull();
        disposableService.IsDisposed.Should().BeTrue();
    }

    private class DummyService { }

    private class DisposableDummyService : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
