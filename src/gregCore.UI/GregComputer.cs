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

public static class GregComputer
{
    public const string ButtonPrefix = "greg-computer-";
    public const string HookShortcutClicked = "greg.COMPUTER.ShortcutClicked";
    public const string HookAppOpened = "greg.COMPUTER.AppOpened";
    public const string HookAppClosed = "greg.COMPUTER.AppClosed";

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
        Action? onClick, string? appId = null, int order = 100)
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
        catch { return false; }
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
        catch { return false; }
    }

    public static bool RegisterApp(string modId, string appId, string title,
        Action<GregPanelBuilder>? build, Action? onClosed = null, bool framePage = true)
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
        catch { return false; }
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
        catch { return false; }
    }

    public static int UnregisterAll(string modId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modId)) return 0;
            int n = 0;
            string? openOwned = null;
            lock (_gate)
            {
                foreach (var k in _shortcuts.Keys.ToArray())
                    if (_shortcuts[k] != null && _shortcuts[k].ModId == modId && _shortcuts.Remove(k)) n++;
                foreach (var k in _apps.Keys.ToArray())
                    if (_apps[k] != null && _apps[k].ModId == modId && _apps.Remove(k)) n++;
                if (!string.IsNullOrEmpty(_currentAppId))
                {
                    var cur = _apps.Values.FirstOrDefault(a => a != null && a.AppId == _currentAppId);
                    if (cur == null) openOwned = _currentAppId; // owner gone with its app entry
                }
            }
            if (openOwned != null) CloseApp();
            if (n > 0) RaiseChanged();
            return n;
        }
        catch { return 0; }
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
        catch { return Array.Empty<ComputerShortcut>(); }
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
        catch { return Array.Empty<ComputerApp>(); }
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
        catch { app = null; return false; }
    }

    public static bool TryOpenApp(string appId)
    {
        try
        {
            if (!TryGetApp(appId, out var app) || app == null) return false;
            lock (_gate) { _currentAppId = app.AppId; }
            try { AppOpened?.Invoke(app.AppId); } catch { }
            SafeEmit(HookAppOpened, new Dictionary<string, object>
            {
                { "AppId", app.AppId }, { "ModId", app.ModId }, { "Title", app.Title },
            });
            if (app.FramePage) ShowAppPage(app);
            return true;
        }
        catch { return false; }
    }

    public static void CloseApp()
    {
        string closing = "";
        Action? onClosed = null;
        try
        {
            lock (_gate)
            {
                closing = _currentAppId;
                _currentAppId = "";
                if (!string.IsNullOrEmpty(closing))
                {
                    var app = _apps.Values.FirstOrDefault(a => a != null && a.AppId == closing);
                    onClosed = app?.OnClosed;
                }
            }
            HideAppPage(closing);
            if (!string.IsNullOrEmpty(closing))
            {
                try { onClosed?.Invoke(); } catch { }
                try { AppClosed?.Invoke(closing); } catch { }
                SafeEmit(HookAppClosed, new Dictionary<string, object> { { "AppId", closing } });
            }
        }
        catch { }
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
        catch { return false; }
    }

    private static void RaiseChanged()
    {
        try { Changed?.Invoke(); } catch { }
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
        catch { }
    }

    // ── Presentation (Unity; best-effort, never throws) ──────────────────────

    internal static string MenuIdFor(string appId) => "computer.app." + appId;

    public static void Sync(global::Il2Cpp.ComputerShop? shop)
    {
        try
        {
            if (shop == null || shop.Pointer == IntPtr.Zero) return;
            RemoveStaleButtons(shop);
            InjectShortcuts(shop);
            var current = CurrentAppId;
            if (!string.IsNullOrEmpty(current) && _appPage == null && TryGetApp(current, out var app) && app != null)
                ShowAppPage(app);
        }
        catch { }
    }

    public static void OnComputerClosed()
    {
        try { CloseApp(); } catch { }
    }

    private static void RemoveStaleButtons(global::Il2Cpp.ComputerShop shop)
    {
        try
        {
            var buttons = shop.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            if (buttons == null) return;
            var wanted = new HashSet<string>(StringComparer.Ordinal);
            foreach (var s in Shortcuts())
            {
                try { wanted.Add(ButtonName(s)); } catch { }
            }
            foreach (var b in buttons)
            {
                try
                {
                    if (b == null || b.gameObject == null) continue;
                    var n = b.gameObject.name ?? "";
                    if (n.StartsWith(ButtonPrefix, StringComparison.Ordinal) && !wanted.Contains(n))
                        UnityEngine.Object.Destroy(b.gameObject);
                }
                catch { }
            }
        }
        catch { }
    }

    private static void InjectShortcuts(global::Il2Cpp.ComputerShop shop)
    {
        try
        {
            var all = Shortcuts();
            if (all.Count == 0) return;
            var buttons = shop.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            if (buttons == null || buttons.Count == 0) return;

            UnityEngine.UI.Button? template = null;
            foreach (var b in buttons)
            {
                try
                {
                    if (b == null || b.gameObject == null) continue;
                    var n = b.gameObject.name ?? "";
                    if (n.StartsWith(ButtonPrefix, StringComparison.Ordinal)) continue;
                    if (string.IsNullOrEmpty(ReadLabel(b))) continue;
                    template = b;
                    break;
                }
                catch { }
            }
            if (template == null) return;
            var container = template.transform != null ? template.transform.parent : null;
            if (container == null) return;

            foreach (var s in all)
            {
                try
                {
                    var name = ButtonName(s);
                    if (container.Find(name) != null) continue;
                    var clone = UnityEngine.Object.Instantiate(template.gameObject, container, false);
                    if (clone == null) continue;
                    clone.name = name;
                    try { clone.transform.localPosition = Vector3.zero; } catch { }
                    try { clone.transform.localScale = Vector3.one; } catch { }
                    SetLabel(clone, s.Label);
                    var btn = clone.GetComponent<UnityEngine.UI.Button>();
                    if (btn == null)
                    {
                        try { UnityEngine.Object.Destroy(clone); } catch { }
                        continue;
                    }
                    try { btn.onClick.RemoveAllListeners(); } catch { }
                    var captured = s;
                    try
                    {
                        btn.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(
                            new Action(() => InvokeShortcut(captured.ModId, captured.Id))));
                    }
                    catch { }
                    try { clone.SetActive(true); } catch { }
                }
                catch { }
            }
        }
        catch { }
    }

    private static string ButtonName(ComputerShortcut s)
    {
        try
        {
            var safe = new string((s.ModId + "-" + s.Id)
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
            if (string.IsNullOrEmpty(safe)) safe = "x";
            return ButtonPrefix + safe;
        }
        catch { return ButtonPrefix + "x"; }
    }

    private static string ReadLabel(UnityEngine.UI.Button b)
    {
        try
        {
            var text = b.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (text != null && !string.IsNullOrEmpty(text.text)) return text.text;
            var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null && !string.IsNullOrEmpty(tmp.text)) return tmp.text;
        }
        catch { }
        return "";
    }

    private static void SetLabel(GameObject root, string label)
    {
        try
        {
            var text = root.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (text != null) { try { text.text = label; } catch { } return; }
            var tmp = root.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) { try { tmp.text = label; } catch { } }
        }
        catch { }
    }

    private static void ShowAppPage(ComputerApp app)
    {
        try
        {
            HideAppPage("");
            var builder = GregPanelBuilder.Create(app.Title).SetSize(560, 640).Build();
            try { app.Build?.Invoke(builder); }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[gregCore][Computer] App '{app.AppId}' build failed: {ex.GetBaseException().Message}");
            }
            builder.AddSpacer(8).AddSeparator().AddButton("\u2190 Back to computer", () => CloseApp());
            builder.Show();
            _appPage = builder;
            try
            {
                GregMenuRegistry.RegisterMenu(MenuIdFor(app.AppId), new GregMenuOptions
                {
                    LockCamera = false,
                    LockMovement = true,
                    LockInteract = true,
                    ShowCursor = true,
                    PanelWidth = 560f,
                });
                GregMenuRegistry.SetOpen(MenuIdFor(app.AppId), true);
            }
            catch { }
        }
        catch { }
    }

    private static void HideAppPage(string closingAppId)
    {
        try
        {
            var page = _appPage;
            _appPage = null;
            if (page != null)
            {
                try { page.Hide(); } catch { }
                try { page.Destroy(); } catch { }
            }
            if (!string.IsNullOrEmpty(closingAppId))
            {
                try { GregMenuRegistry.SetOpen(MenuIdFor(closingAppId), false); } catch { }
            }
        }
        catch { }
    }
}
