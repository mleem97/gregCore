/// <file-summary>
/// Schicht:      Core
/// Zweck:        Hilfsklasse zum schnellen Erstellen von EventPayloads.
/// Maintainer:   Reduziert Boilerplate beim Dispatchen von Events.
/// </file-summary>

namespace gregCore.Core.Events;

public static class EventPayloadBuilder
{
    public static EventPayload ForScene(int buildIndex, string sceneName) =>
        new EventPayload
        {
            HookName = HookName.Create("lifecycle", "SceneLoaded").Full,
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>
            {
                { "BuildIndex", buildIndex },
                { "SceneName", sceneName }
            },
            IsCancelable = false
        };

    public static EventPayload ForValueChange(string propertyName, object oldValue, object newValue) =>
        new EventPayload
        {
            HookName = string.Empty, // Wird vom Caller überschrieben
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>
            {
                { "Property", propertyName },
                { "OldValue", oldValue },
                { "NewValue", newValue }
            },
            IsCancelable = true
        };
}
