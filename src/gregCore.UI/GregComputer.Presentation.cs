using System;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace gregCore.UI;

public static partial class GregComputer
{
    internal static string MenuIdFor(string appId) => "computer.app." + appId;

    public static void Sync(global::Il2Cpp.ComputerShop? shop)
    {
        try
        {
            if (shop == null || shop.Pointer == IntPtr.Zero) return;
            RemoveStaleButtons(shop);
            InjectShortcuts(shop);
            ShowCurrentIfNeeded();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ShowCurrentIfNeeded()
    {
        try
        {
            var current = CurrentAppId;
            if (string.IsNullOrEmpty(current)) return;
            if (_appPage != null) return;
            if (TryGetApp(current, out var app) && app != null)
                ShowAppPage(app);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public static void OnComputerClosed()
    {
        try { CloseApp(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void RemoveStaleButtons(global::Il2Cpp.ComputerShop shop)
    {
        try
        {
            var buttons = shop.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            if (buttons == null) return;
            var wanted = CollectWantedNames();
            RemoveUnwanted(buttons, wanted);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static HashSet<string> CollectWantedNames()
    {
        var wanted = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            foreach (var s in Shortcuts())
            {
                try { wanted.Add(ButtonName(s)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return wanted;
    }

    private static void RemoveUnwanted(System.Collections.Generic.IEnumerable<UnityEngine.UI.Button> buttons, HashSet<string> wanted)
    {
        try
        {
            foreach (var b in buttons)
            {
                try { TryRemoveOne(b, wanted); }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void TryRemoveOne(UnityEngine.UI.Button b, HashSet<string> wanted)
    {
        try
        {
            if (b == null || b.gameObject == null) return;
            var n = b.gameObject.name ?? "";
            if (n.StartsWith(ButtonPrefix, StringComparison.Ordinal) && !wanted.Contains(n))
                UnityEngine.Object.Destroy(b.gameObject);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void InjectShortcuts(global::Il2Cpp.ComputerShop shop)
    {
        try
        {
            var all = Shortcuts();
            if (all.Count == 0) return;
            var template = FindTemplate(shop);
            if (template == null) return;
            var container = template.transform != null ? template.transform.parent : null;
            if (container == null) return;
            InjectAll(template, container, all);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static UnityEngine.UI.Button FindTemplate(global::Il2Cpp.ComputerShop shop)
    {
        try
        {
            var buttons = shop.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            if (buttons == null || buttons.Count == 0) return null;
            foreach (var b in buttons)
            {
                try
                {
                    if (IsTemplateCandidate(b)) return b;
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
            return null;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
    }

    private static bool IsTemplateCandidate(UnityEngine.UI.Button b)
    {
        try
        {
            if (b == null || b.gameObject == null) return false;
            var n = b.gameObject.name ?? "";
            if (n.StartsWith(ButtonPrefix, StringComparison.Ordinal)) return false;
            if (string.IsNullOrEmpty(ReadLabel(b))) return false;
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static void InjectAll(UnityEngine.UI.Button template, UnityEngine.Transform container, System.Collections.Generic.IReadOnlyList<ComputerShortcut> all)
    {
        try
        {
            foreach (var s in all)
            {
                try { InjectOne(template, container, s); }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void InjectOne(UnityEngine.UI.Button template, UnityEngine.Transform container, ComputerShortcut s)
    {
        try
        {
            var name = ButtonName(s);
            if (container.Find(name) != null) return;
            var clone = UnityEngine.Object.Instantiate(template.gameObject, container, false);
            if (clone == null) return;
            clone.name = name;
            try { clone.transform.localPosition = Vector3.zero; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            try { clone.transform.localScale = Vector3.one; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            SetLabel(clone, s.Label);
            WireClone(clone, s);
            try { clone.SetActive(true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void WireClone(GameObject clone, ComputerShortcut s)
    {
        try
        {
            var btn = clone.GetComponent<UnityEngine.UI.Button>();
            if (btn == null)
            {
                try { UnityEngine.Object.Destroy(clone); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                return;
            }
            try { btn.onClick.RemoveAllListeners(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            var captured = s;
            try
            {
                btn.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(
                    new Action(() => InvokeShortcut(captured.ModId, captured.Id))));
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return ButtonPrefix + "x"; }
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return "";
    }

    private static void SetLabel(GameObject root, string label)
    {
        try
        {
            var text = root.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (text != null) { try { text.text = label; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } return; }
            var tmp = root.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) { try { tmp.text = label; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ShowAppPage(ComputerApp app)
    {
        try
        {
            HideAppPage("");
            var builder = GregPanelBuilder.Create(app.Title).SetSize(560, 640).Build();
            TryBuildApp(app, builder);
            builder.AddSpacer(8).AddSeparator().AddButton("← Back to computer", () => CloseApp());
            builder.Show();
            _appPage = builder;
            TryRegisterAppMenu(app);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void TryBuildApp(ComputerApp app, GregPanelBuilder builder)
    {
        try { app.Build?.Invoke(builder); }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Computer] App '{app.AppId}' build failed: {ex.GetBaseException().Message}");
        }
    }

    private static void TryRegisterAppMenu(ComputerApp app)
    {
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void HideAppPage(string closingAppId)
    {
        try
        {
            var page = _appPage;
            _appPage = null;
            if (page != null)
            {
                try { page.Hide(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                try { page.Destroy(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
            if (!string.IsNullOrEmpty(closingAppId))
            {
                try { GregMenuRegistry.SetOpen(MenuIdFor(closingAppId), false); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }
}
