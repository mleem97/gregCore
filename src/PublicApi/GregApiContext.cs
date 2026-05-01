/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        DI-Container-Ersatz für Mods.
/// Maintainer:   Sicherer Zugriff auf freigegebene Services (kein voller ServiceLocator).
/// </file-summary>

namespace gregCore.PublicApi;

public sealed class GregApiContext
{
    public IGregLogger Logger { get; init; } = null!;
    public IGregEventBus EventBus { get; init; } = null!;
    public Core.Events.GregHookBus HookBus { get; init; } = null!;
    public IGregConfigService Config { get; init; } = null!;
    public IGregPersistenceService Persist { get; init; } = null!;
}
