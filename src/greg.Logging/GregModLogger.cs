using System;
using System.Runtime.CompilerServices;

namespace greg.Logging
{
    public class GregModLogger
    {
        private readonly string _modTag;

        public GregModLogger(string modTag)
        {
            if (string.IsNullOrWhiteSpace(modTag))
                throw new ArgumentException("ModTag cannot be null or empty.");

            string safeTag = modTag.Length > 16 ? modTag.Substring(0, 16) : modTag;
            _modTag = $"[{safeTag}]".PadRight(16);
        }

        private string Format(string caller, string message)
        {
            return $"[gC-{caller}] {_modTag}{message}";
        }

        // We use GregLogger's internal MelonLogger wrapper to emit the fully formatted line
        // bypassing GregLogger's default format, or we can use an internal method.
        // For simplicity and to reuse the thread-safety of GregLogger, we'll just format
        // the string here and pass it as the "message" to GregLogger while passing "" as caller,
        // so GregLogger doesn't prepend its own `[gC-]` again.
        // Wait, GregLogger prepends `[gC-{caller}]`. If caller is empty, it outputs `[gC-]`.
        // To be exact to the spec, GregModLogger should call MelonLogger directly, OR we modify
        // GregLogger to accept pre-formatted messages.
        // I will use direct calls to MelonLoader.MelonLogger.Instance here, or just let GregLogger
        // prepend the `[gC-{caller}]` and we just prepend `_modTag`.
        // Let's look at GregLogger.Msg: it does `$"[gC-{caller}] {message}"`.
        // If we pass `_modTag + message` as the message, and `caller` as the caller, it outputs:
        // `[gC-{caller}] [{modTag}]      message` which is exactly the requested format!

        public void Msg(string message, [CallerMemberName] string caller = "")
        {
            GregLogger.Msg($"{_modTag}{message}", caller);
        }

        public void Warn(string message, [CallerMemberName] string caller = "")
        {
            GregLogger.Warn($"{_modTag}{message}", caller);
        }

        public void Error(string message, Exception? ex = null, [CallerMemberName] string caller = "")
        {
            GregLogger.Error($"{_modTag}{message}", ex, caller);
        }

        public void Debug(string message, [CallerMemberName] string caller = "")
        {
            GregLogger.Debug($"{_modTag}{message}", caller);
        }

        public void Section(string sectionTitle, [CallerMemberName] string caller = "")
        {
            Msg($"--- {sectionTitle} ---", caller);
        }

        public void PatchApplied(string target, [CallerMemberName] string caller = "")
        {
            Msg($"{"PATCH APPLIED".PadRight(16)}{target}", caller);
        }

        public void PatchFailed(string target, Exception? ex = null, [CallerMemberName] string caller = "")
        {
            Error($"{"PATCH FAILED".PadRight(16)}{target}", ex, caller);
        }

        public void HookSubscribed(string hookName, [CallerMemberName] string caller = "")
        {
            Msg($"{"HOOK SUBSCRIBED".PadRight(16)}{hookName}", caller);
        }

        public void HookFired(string hookName, [CallerMemberName] string caller = "")
        {
            Msg($"{"HOOK FIRED".PadRight(16)}{hookName}", caller);
        }

        public void Saved(int count, long ms, [CallerMemberName] string caller = "")
        {
            Msg($"{"SAVED".PadRight(16)}{count} objects in {ms}ms", caller);
        }

        public void Loaded(int count, string source, [CallerMemberName] string caller = "")
        {
            Msg($"{"LOADED".PadRight(16)}{count} objects from {source}", caller);
        }

        public void VanillaSaveDetected(string featureName, [CallerMemberName] string caller = "")
        {
            Msg($"VANILLA SAVE DETECTED -- {featureName} disabled", caller);
        }

        public void FeatureState(string featureKey, bool enabled, [CallerMemberName] string caller = "")
        {
            string state = enabled ? "ENABLED" : "DISABLED";
            Msg($"FEATURE {featureKey} = {state}", caller);
        }
    }
}
