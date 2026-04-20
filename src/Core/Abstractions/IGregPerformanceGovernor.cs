/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für den Performance Governor.
/// Maintainer:   Wird vom EventBus für Throttling genutzt.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregPerformanceGovernor
{
    bool CanDispatchEvent();
    void OnUpdate();
}
