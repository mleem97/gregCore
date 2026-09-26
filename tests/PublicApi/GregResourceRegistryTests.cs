using gregCore.PublicApi;
using Xunit;
using FluentAssertions;

namespace gregCore.Tests.PublicApi;

public sealed class GregResourceRegistryTests
{
    [Fact]
    public void Dispose_ShouldReleaseResourcesInReverseOrder()
    {
        var calls = new List<int>();
        using (var registry = new GregResourceRegistry())
        {
            registry.Track(new Disposable(() => calls.Add(1)));
            registry.Track(new Disposable(() => calls.Add(2)));
        }
        calls.Should().Equal(2, 1);
    }

    private sealed class Disposable : IDisposable
    {
        private readonly Action _action;
        public Disposable(Action action) => _action = action;
        public void Dispose() => _action();
    }
}
