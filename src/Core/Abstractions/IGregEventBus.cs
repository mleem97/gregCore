/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für das Event-Bus System.
/// Maintainer:   Zentraler Message-Broker für alle Mods und Hooks.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregEventBus
{
    void Subscribe(string hookName, Action<EventPayload> handler);
    void Unsubscribe(string hookName, Action<EventPayload> handler);
    bool Publish(string hookName, EventPayload payload);
}
