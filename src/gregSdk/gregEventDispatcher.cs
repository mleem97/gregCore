using System;
using gregCore.Core.Events;

namespace greg.Sdk
{
    /// <summary>
    /// Legacy proxy for the event dispatcher to maintain compatibility with older mods.
    /// </summary>
    public static class gregEventDispatcher
    {
        public static void On(string hookName, Action<object> handler, string modId = "legacy")
        {
            GregEventDispatcher.On(hookName, handler, modId);
        }

        public static void Emit(string hookName, object data)
        {
            GregEventDispatcher.Emit(hookName, data);
        }
        
        public static void UnregisterAll(string modId)
        {
            GregEventDispatcher.UnregisterAll(modId);
        }
    }
}
