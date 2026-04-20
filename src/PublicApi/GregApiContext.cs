/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        DI-Container-Ersatz für Mods.
/// Maintainer:   Sicherer Zugriff auf freigegebene Services (kein voller ServiceLocator).
/// </file-summary>

namespace gregCore.PublicApi;

public sealed class GregApiContext
{
    public required IGregLogger Logger { get; init; }
    public required IGregEventBus EventBus { get; init; }
    public required Core.Events.GregHookBus HookBus { get; init; }
    public required IGregConfigService Config { get; init; }
    public required IGregPersistenceService Persist { get; init; }
}
