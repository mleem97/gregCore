using System;
using System.Runtime.CompilerServices;
using MelonLoader;
using gregCore.Infrastructure.Config;

namespace greg.Logging
{
    public static class GregLogger
    {
        private static MelonLoader.MelonLogger.Instance? _logger;
        private static readonly object _lock = new object();

        public static void Initialize(MelonLoader.MelonLogger.Instance logger)
        {
            lock (_lock)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }
        }

        private static string Format(string caller, string message)
        {
            return $"[gC-{caller}] {message}";
        }

        public static void Msg(string message, [CallerMemberName] string caller = "")
        {
            if (_logger == null) return;
            lock (_lock)
            {
                _logger.Msg(Format(caller, message));
            }
        }

        public static void Warn(string message, [CallerMemberName] string caller = "")
        {
            if (_logger == null) return;
            lock (_lock)
            {
                _logger.Warning(Format(caller, message));
            }
        }

        public static void Error(string message, Exception? ex = null, [CallerMemberName] string caller = "")
        {
            if (_logger == null) return;
            lock (_lock)
            {
                string fullMsg = ex != null ? $"{message}\n{ex}" : message;
                _logger.Error(Format(caller, fullMsg));
            }
        }

        public static void Debug(string message, [CallerMemberName] string caller = "")
        {
            if (_logger == null) return;
            if (!GregCoreConfig.DebugMode) return;

            lock (_lock)
            {
                _logger.Msg(Format(caller, message));
            }
        }

        public static void Section(string sectionTitle, [CallerMemberName] string caller = "")
        {
            Msg($"--- {sectionTitle} ---", caller);
        }

        public static void PatchApplied(string patchTargetDescription, [CallerMemberName] string caller = "")
        {
            Msg($"{"PATCH APPLIED".PadRight(16)}{patchTargetDescription}", caller);
        }

        public static void PatchFailed(string patchTargetDescription, Exception? ex = null, [CallerMemberName] string caller = "")
        {
            Error($"{"PATCH FAILED".PadRight(16)}{patchTargetDescription}", ex, caller);
        }

        public static void HookSubscribed(string hookName, [CallerMemberName] string caller = "")
        {
            Msg($"{"HOOK SUBSCRIBED".PadRight(16)}{hookName}", caller);
        }

        public static void HookFired(string hookName, [CallerMemberName] string caller = "")
        {
            Msg($"{"HOOK FIRED".PadRight(16)}{hookName}", caller);
        }

        public static void Saved(int objectCount, long elapsedMs, [CallerMemberName] string caller = "")
        {
            Msg($"{"SAVED".PadRight(16)}{objectCount} objects in {elapsedMs}ms", caller);
        }

        public static void Loaded(int objectCount, string source, [CallerMemberName] string caller = "")
        {
            Msg($"{"LOADED".PadRight(16)}{objectCount} objects from {source}", caller);
        }

        public static void VanillaSaveDetected(string featureName, [CallerMemberName] string caller = "")
        {
            Msg($"VANILLA SAVE DETECTED -- {featureName} disabled", caller);
        }

        public static void FeatureState(string featureKey, bool enabled, [CallerMemberName] string caller = "")
        {
            string state = enabled ? "ENABLED" : "DISABLED";
            Msg($"FEATURE {featureKey} = {state}", caller);
        }
    }
}
