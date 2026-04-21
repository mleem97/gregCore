using System;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;
using gregCore.GameLayer.Bootstrap;

namespace gregCore.Core.Events
{
    public static class GregEventDispatcher
    {
        private static IGregEventBus? Bus => GregServiceContainer.Get<IGregEventBus>();

        public static void On(string hookName, Action<object> handler, string modId)
        {
            // The IGregEventBus interface seems to use Action<EventPayload> in some places
            // but we can wrap it for the static API.
            Bus?.Subscribe(hookName, (payload) => handler(payload));
        }

        public static void Emit(string hookName, object data)
        {
            Bus?.Publish(hookName, data is EventPayload p ? p : EventPayloadBuilder.ForGeneric(hookName, data));
        }

        public static void UnregisterAll(string modId)
        {
            // Implementation depends on GregEventBus capabilities
        }
    }
}
