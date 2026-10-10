/// <file-summary>
/// Layer:       PublicApi
/// Purpose:     DI container replacement for mods.
/// Maintainer:  Safe access to shared services (not a full service locator).
/// </file-summary>

namespace gregCore.PublicApi;

public sealed class GregApiContext
{
    public IGregLogger Logger { get; init; } = null!;
    public IGregEventBus EventBus { get; init; } = null!;
    public Core.Events.GregHookBus HookBus { get; init; } = null!;
    public IGregConfigService Config { get; init; } = null!;
    public IGregPersistenceService Persist { get; init; } = null!;
    public CancellationToken CancellationToken { get; init; }
    public GregEventBusPublic Events { get; init; } = null!;
    public IGregMainThreadDispatcher MainThread { get; init; } = null!;
    public GregResourceRegistry Resources { get; init; } = null!;
    public CancellationTokenSource? LifetimeSource { get; init; }
}
