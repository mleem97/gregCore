using System;

namespace gregCore.UI;

public static partial class GregComputer
{
    // Captures and clears the currently open app under lock.
    private static (string Closing, Action? OnClosed) TakeClosing()
    {
        try
        {
            lock (_gate)
            {
                string closing = _currentAppId;
                _currentAppId = "";
                Action? onClosed = LookupCloseHandler(closing);
                return (closing, onClosed);
            }
        }
        catch { return ("", null); }
    }

    // Looks up the OnClosed handler for the closing app id.
    private static Action? LookupCloseHandler(string closing)
    {
        try
        {
            if (string.IsNullOrEmpty(closing)) return null;
            var app = FindAppById(closing);
            return app?.OnClosed;
        }
        catch { return null; }
    }

    // Finds an app by id without throwing.
    private static ComputerApp? FindAppById(string appId)
    {
        try
        {
            foreach (var a in _apps.Values)
            {
                try
                {
                    if (a != null && IsAppMatch(a, appId)) return a;
                }
                catch { }
            }
        }
        catch { }
        return null;
    }

    // Checks whether an app entry matches the given id.
    private static bool IsAppMatch(ComputerApp app, string appId)
    {
        try
        {
            return string.Equals(app.AppId, appId, StringComparison.Ordinal);
        }
        catch { return false; }
    }

    // Fires close callbacks and the AppClosed event for a closing app.
    private static void NotifyClosed(string closing, Action? onClosed)
    {
        try
        {
            if (string.IsNullOrEmpty(closing)) return;
            InvokeCloseHandler(onClosed);
            InvokeAppClosed(closing);
            EmitAppClosed(closing);
        }
        catch { }
    }

    // Invokes the app owner OnClosed handler defensively.
    private static void InvokeCloseHandler(Action? onClosed)
    {
        try { onClosed?.Invoke(); } catch { }
    }

    // Invokes the static AppClosed event defensively.
    private static void InvokeAppClosed(string closing)
    {
        try { AppClosed?.Invoke(closing); } catch { }
    }

    // Emits the HookAppClosed payload defensively.
    private static void EmitAppClosed(string closing)
    {
        try
        {
            SafeEmit(HookAppClosed, new Dictionary<string, object> { { "AppId", closing } });
        }
        catch { }
    }
}
