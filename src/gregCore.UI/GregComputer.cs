/// <file-summary>
/// Layer:       UI
/// Purpose:     Custom shortcuts + apps/pages for the in-game computer
///              (Il2Cpp.ComputerShop main screen). Mods register shortcuts
///              (buttons injected next to the vanilla screen buttons) and
///              apps (overlay pages with Back navigation). Registry logic is
///              pure and unit-tested; Unity injection is best-effort and
///              never throws. Events: greg.COMPUTER.ShortcutClicked,
///              greg.COMPUTER.AppOpened, greg.COMPUTER.AppClosed.
/// Maintainer:  Patch entry: GregComputerPatch (gregCore.Patches).
///              Lua surface: greg.computer (LuaComputerModule).
/// </file-summary>

using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.UI;

public sealed class ComputerShortcut
{
    public string ModId { get; init; } = "";
    public string Id { get; init; } = "";
    public string Label { get; init; } = "";
    public int Order { get; init; } = 100;
    public string AppId { get; init; } = "";
    public Action? OnClick { get; init; }
}

public sealed class ComputerApp
{
    public string ModId { get; init; } = "";
    public string AppId { get; init; } = "";
    public string Title { get; init; } = "";
    public Action<GregPanelBuilder>? Build { get; init; }
    public Action? OnClosed { get; init; }
    /// <summary>
    /// True: GregComputer shows a frame page (title + Back button + input
    /// lock) and calls Build to fill it. False: the owner subscribes
    /// AppOpened/AppClosed and owns all presentation (used by the Lua
    /// bridge, which opens a tablet with panel_add_* content).
    /// </summary>
    public bool FramePage { get; init; } = true;
}

public static partial class GregComputer
{
    public static string ButtonPrefix { get; } = "greg-computer-";
    public static string HookShortcutClicked { get; } = "greg.COMPUTER.ShortcutClicked";
    public static string HookAppOpened { get; } = "greg.COMPUTER.AppOpened";
    public static string HookAppClosed { get; } = "greg.COMPUTER.AppClosed";

    private static readonly Dictionary<string, ComputerShortcut> _shortcuts =
        new Dictionary<string, ComputerShortcut>(StringComparer.Ordinal);
    private static readonly Dictionary<string, ComputerApp> _apps =
        new Dictionary<string, ComputerApp>(StringComparer.Ordinal);
    private static readonly object _gate = new object();

    private static string _currentAppId = "";
    private static GregPanelBuilder? _appPage;

    public static event Action? Changed;
    public static event Action<string>? AppOpened;
    public static event Action<string>? AppClosed;

    public static string CurrentAppId
    {
        get { lock (_gate) { return _currentAppId; } }
    }

    // ── Registry (pure, unit-tested) ─────────────────────────────────────────

    private static string Key(string modId, string id) => modId + "\u0000" + id;

    public static bool RegisterShortcut(string modId, string id, string label,
        Action? onClick)
    {
        return RegisterShortcut(modId, id, label, onClick, null, 100);
    }

    public static bool RegisterShortcut(string modId, string id, string label,
        Action? onClick, string? appId)
    {
        return RegisterShortcut(modId, id, label, onClick, appId, 100);
    }

    public static bool RegisterShortcut(string modId, string id, string label,
        Action? onClick, int order)
    {
        return RegisterShortcut(modId, id, label, onClick, null, order);
    }

    public static bool RegisterShortcut(string modId, string id, string label,
        Action? onClick, string? appId, int order)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modId) || string.IsNullOrWhiteSpace(id)) return false;
            if (string.IsNullOrWhiteSpace(label)) return false;
            lock (_gate)
            {
                _shortcuts[Key(modId, id)] = new ComputerShortcut
                {
                    ModId = modId, Id = id, Label = label,
                    Order = order, AppId = appId ?? "", OnClick = onClick,
                };
            }
            RaiseChanged();
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public static bool UnregisterShortcut(string modId, string id)
    {
        try
        {
            bool removed;
            lock (_gate) { removed = _shortcuts.Remove(Key(modId, id)); }
            if (removed) RaiseChanged();
            return removed;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public static bool RegisterApp(string modId, string appId, string title,
        Action<GregPanelBuilder>? build, Action? onClosed, bool framePage)
    {
        return RegisterAppImpl(modId, appId, title, build, onClosed, framePage);
    }

    public static bool RegisterApp(string modId, string appId, string title,
        Action<GregPanelBuilder>? build)
    {
        return RegisterAppImpl(modId, appId, title, build, null, true);
    }

    public static bool RegisterApp(string modId, string appId, string title,
        Action<GregPanelBuilder>? build, bool framePage)
    {
        return RegisterAppImpl(modId, appId, title, build, null, framePage);
    }

    private static bool RegisterAppImpl(string modId, string appId, string title,
        Action<GregPanelBuilder>? build, Action? onClosed, bool framePage)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modId) || string.IsNullOrWhiteSpace(appId)) return false;
            if (string.IsNullOrWhiteSpace(title)) return false;
            lock (_gate)
            {
                _apps[Key(modId, appId)] = new ComputerApp
                {
                    ModId = modId, AppId = appId, Title = title,
                    Build = build, OnClosed = onClosed, FramePage = framePage,
                };
            }
            RaiseChanged();
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public static bool UnregisterApp(string modId, string appId)
    {
        try
        {
            bool removed;
            lock (_gate) { removed = _apps.Remove(Key(modId, appId)); }
            if (removed)
            {
                if (string.Equals(CurrentAppId, appId, StringComparison.Ordinal)) CloseApp();
                RaiseChanged();
            }
            return removed;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public static int UnregisterAll(string modId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modId)) return 0;
            int n = RemoveOwned(modId);
            string openOwned = FindOrphanedApp();
            if (openOwned != null) CloseApp();
            if (n > 0) RaiseChanged();
            return n;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return 0; }
    }

    private static int RemoveOwned(string modId)
    {
        try
        {
            int n = RemoveOwnedShortcuts(modId);
            n += RemoveOwnedApps(modId);
            return n;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return 0; }
    }

    private static int RemoveOwnedShortcuts(string modId)
    {
        int n = 0;
        try
        {
            string[] keys;
            lock (_gate) { keys = _shortcuts.Keys.ToArray(); }
            foreach (var k in keys)
            {
                try
                {
                    lock (_gate)
                    {
                        if (_shortcuts.TryGetValue(k, out var s) && s != null && s.ModId == modId && _shortcuts.Remove(k)) n++;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return n;
    }

    private static int RemoveOwnedApps(string modId)
    {
        int n = 0;
        try
        {
            string[] keys;
            lock (_gate) { keys = _apps.Keys.ToArray(); }
            foreach (var k in keys)
            {
                try
                {
                    lock (_gate)
                    {
                        if (_apps.TryGetValue(k, out var a) && a != null && a.ModId == modId && _apps.Remove(k)) n++;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return n;
    }

    private static string FindOrphanedApp()
    {
        try
        {
            lock (_gate)
            {
                if (string.IsNullOrEmpty(_currentAppId)) return null;
                var cur = _apps.Values.FirstOrDefault(a => a != null && a.AppId == _currentAppId);
                if (cur == null) return _currentAppId;
                return null;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
    }

    public static IReadOnlyList<ComputerShortcut> Shortcuts()
    {
        try
        {
            lock (_gate)
            {
                return _shortcuts.Values
                    .Where(s => s != null)
                    .OrderBy(s => s.Order)
                    .ThenBy(s => s.Label, StringComparer.Ordinal)
                    .ToArray();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return Array.Empty<ComputerShortcut>(); }
    }

    public static IReadOnlyList<ComputerApp> Apps()
    {
        try
        {
            lock (_gate)
            {
                return _apps.Values
                    .Where(a => a != null)
                    .OrderBy(a => a.Title, StringComparer.Ordinal)
                    .ToArray();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return Array.Empty<ComputerApp>(); }
    }

    public static bool TryGetApp(string appId, out ComputerApp? app)
    {
        app = null;
        try
        {
            if (string.IsNullOrWhiteSpace(appId)) return false;
            lock (_gate)
            {
                app = _apps.Values.FirstOrDefault(a => a != null && a.AppId == appId);
            }
            return app != null;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  app = null; return false; }
    }

    public static bool TryOpenApp(string appId)
    {
        try
        {
            if (!TryGetApp(appId, out var app) || app == null) return false;
            lock (_gate) { _currentAppId = app.AppId; }
            try { AppOpened?.Invoke(app.AppId); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            SafeEmit(HookAppOpened, new Dictionary<string, object>
            {
                { "AppId", app.AppId }, { "ModId", app.ModId }, { "Title", app.Title },
            });
            if (app.FramePage) ShowAppPage(app);
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public static void CloseApp()
    {
        try
        {
            var state = TakeClosing();
            HideAppPage(state.Closing);
            NotifyClosed(state.Closing, state.OnClosed);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public static bool InvokeShortcut(string modId, string id)
    {
        try
        {
            ComputerShortcut? s;
            lock (_gate) { _shortcuts.TryGetValue(Key(modId, id), out s); }
            if (s == null) return false;
            SafeEmit(HookShortcutClicked, new Dictionary<string, object>
            {
                { "ModId", s.ModId }, { "Id", s.Id }, { "Label", s.Label }, { "AppId", s.AppId },
            });
            if (!string.IsNullOrEmpty(s.AppId) && TryGetApp(s.AppId, out _)) return TryOpenApp(s.AppId);
            try { s.OnClick?.Invoke(); } catch (Exception ex)
            {
                MelonLogger.Warning($"[gregCore][Computer] Shortcut '{id}' failed: {ex.GetBaseException().Message}");
            }
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static void RaiseChanged()
    {
        try { Changed?.Invoke(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void SafeEmit(string hook, Dictionary<string, object> data)
    {
        try
        {
            GregEventDispatcher.Emit(hook, new EventPayload
            {
                HookName = hook,
                OccurredAtUtc = DateTime.UtcNow,
                Data = data,
            });
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

}
