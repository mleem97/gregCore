/// <file-summary>
/// Layer:       Core
/// Purpose:     Helper class for quickly creating EventPayloads.
/// Maintainer:  Reduces boilerplate when dispatching events.
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
            HookName = string.Empty, // Overridden by the caller
            OccurredAtUtc = DateTime.UtcNow,
            Data = new Dictionary<string, object>
            {
                { "Property", propertyName },
                { "OldValue", oldValue },
                { "NewValue", newValue }
            },
            IsCancelable = true
        };

    public static EventPayload ForGeneric(string hookName, object data) =>
        new EventPayload
        {
            HookName = hookName,
            OccurredAtUtc = DateTime.UtcNow,
            Data = data as Dictionary<string, object> ?? new Dictionary<string, object> { { "Data", data } },
            IsCancelable = false
        };
}
