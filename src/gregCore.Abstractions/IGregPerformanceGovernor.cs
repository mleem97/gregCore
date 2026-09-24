/// <file-summary>
/// Layer:       Core
/// Purpose:     Interface for the performance governor.
/// Maintainer:  Used by the EventBus for throttling.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregPerformanceGovernor
{
    bool CanDispatchEvent();
    void OnUpdate();
}
