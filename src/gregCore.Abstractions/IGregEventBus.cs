/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for the event bus system.
/// Maintainer:   Central message broker for all mods and hooks.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregEventBus
{
    void Subscribe(string hookName, Action<EventPayload> handler);
    void Unsubscribe(string hookName, Action<EventPayload> handler);
    bool Publish(string hookName, EventPayload payload);
}
