using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using gregCore.UI;
using MelonLoader;

namespace gregCore.Infrastructure.UI
{
    public class GregDevConsole : MonoBehaviour
    {
        public GregDevConsole(IntPtr ptr) : base(ptr) { }

        public static GregDevConsole Instance { get; private set; } = null!;

        /// <summary>Framework menu ID of the console (for input lock/cursor).</summary>
        public static string MenuId { get; } = "greg.console";

        private readonly List<string> _logs = new();
        private readonly Queue<(string line, LogType type)> _pendingLines = new();
        private readonly object _pendingSync = new object();
        private bool _feedSubscribed;
        private bool _visible;
        private GregPanelBuilder? _panel;
        private ScrollView? _logScrollView;
        private TextField? _inputField;
        private VisualElement? _logContainer;

        public static void Initialize()
        {
            if (Instance != null) return;

            try
            {
                var go = new GameObject("greg_DevConsole_Host");
                UnityEngine.Object.DontDestroyOnLoad(go);

                Instance = go.AddComponent(Il2CppInterop.Runtime.Il2CppType.Of<GregDevConsole>()).Cast<GregDevConsole>();
                Instance.SubscribeLogFeed();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] Initialize failed: {ex.Message}");
            }
        }

        public bool IsOpen => _visible;

        public void AddLog(string msg)
        {
            AddLog(msg, "INFO");
        }

        public void AddLog(string msg, string type)
        {
            string typeStr = string.IsNullOrEmpty(type) ? "INFO" : type;
            MelonLoader.MelonLogger.Msg($"[{typeStr}] {msg}");
            AddRaw($"[{typeStr}] {msg}");
        }

        private void AddRaw(string line)
        {
            _logs.Add(line);
            if (_logs.Count > 200) _logs.RemoveAt(0);
            RefreshLogDisplay();
        }

        // Unity log feed: captures errors/warnings from the game + all mods.
        // May run off the main thread -> only enqueue, display in Update.
        // AddRaw without MelonLogger echo -> no feedback loop. Own echoes
        // (timestamp prefix) are detected as duplicates and skipped.
        private void SubscribeLogFeed()
        {
            if (_feedSubscribed) return;
            _feedSubscribed = true;
            try
            {
                // Note: the interop assembly does not project the event as a
                // C# event — only add_/remove_. Create the IL2CPP delegate via
                // DelegateSupport from the managed method.
                var cb = Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<Application.LogCallback>(
                    (Action<string, string, LogType>)OnUnityLog);
                Application.add_logMessageReceived(cb);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[GregDevConsole] Log-Feed failed: {ex.Message}");
            }
        }

        private void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            try
            {
                if (string.IsNullOrEmpty(condition)) return;
                string line = type == LogType.Error || type == LogType.Exception
                    ? $"[ERROR] {condition}"
                    : type == LogType.Warning ? $"[WARN] {condition}" : $"[INFO] {condition}";
                lock (_pendingSync)
                {
                    if (_pendingLines.Count > 500) _pendingLines.Clear();
                    _pendingLines.Enqueue((line, type));
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        private void FlushLogFeed()
        {
            try
            {
                while (true)
                {
                    (string line, LogType _) next;
                    lock (_pendingSync)
                    {
                        if (_pendingLines.Count == 0) return;
                        next = _pendingLines.Dequeue();
                    }
                    // Recognize own MelonLogger echo (with timestamp).
                    string stripped = System.Text.RegularExpressions.Regex.Replace(
                        next.line, @"^\[\d{2}:\d{2}:\d{2}[.\d]*\]\s*", "");
                    lock (_logs)
                    {
                        if (_logs.Count > 0 && _logs[_logs.Count - 1] == stripped) continue;
                    }
                    AddRaw(next.line);
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        private void Update()
        {
            try
            {
                FlushLogFeed();

                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                // Backquote only: F12 belongs to export (otherwise double trigger).
                if (keyboard.backquoteKey.wasPressedThisFrame)
                {
                    Toggle();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] Update error: {ex.Message}");
            }
        }

        public void Toggle()
        {
            if (_panel == null) BuildUI();

            if (_visible) Hide();
            else Show();
        }

        private void Show()
        {
            try
            {
                GregMenuRegistry.RegisterMenu(MenuId, new GregMenuOptions
                {
                    LockCamera = true,
                    LockMovement = true,
                    LockInteract = true,
                    ShowCursor = true,
                });
                GregMenuRegistry.SetOpen(MenuId, true);
                try { GregInputLock.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            _panel?.Show();
            _visible = true;
            _inputField?.Focus();
        }

        private void Hide()
        {
            _panel?.Hide();
            _visible = false;
            try
            {
                GregMenuRegistry.SetOpen(MenuId, false);
                try { GregInputLock.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        private void BuildUI()
        {
            try
            {
                var root = CreatePanelRoot();
                if (root == null) return;
                root.Add(CreateLogView());
                root.Add(CreateInputRow());
                root.Add(CreateButtonRow());
                RefreshLogDisplay();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] BuildUI failed: {ex.Message}");
            }
        }

        private VisualElement CreatePanelRoot()
        {
            try
            {
                _panel = GregPanelBuilder.Create("Developer Console")
                    .SetSize(700, 450)
                    .SetPosition(50, 50)
                    .Build(GregUILayerType.Overlay);
                var root = _panel.Root;
                if (root == null) return null;
                var defaultScroll = root.Q<ScrollView>("PanelContent");
                if (defaultScroll != null)
                {
                    defaultScroll.RemoveFromHierarchy();
                }
                return root;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] Panel failed: {ex.Message}");
                return null;
            }
        }

        private ScrollView CreateLogView()
        {
            _logScrollView = new ScrollView();
            _logScrollView.name = "ConsoleLogScroll";
            _logScrollView.style.flexGrow = 1;
            _logScrollView.style.backgroundColor = new Color(0.05f, 0.05f, 0.06f, 0.8f);
            _logScrollView.style.borderTopLeftRadius = 4;
            _logScrollView.style.borderTopRightRadius = 4;
            _logScrollView.style.borderBottomLeftRadius = 4;
            _logScrollView.style.borderBottomRightRadius = 4;
            _logScrollView.style.marginBottom = 8;
            _logScrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
            _logScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _logContainer = _logScrollView.contentContainer;
            return _logScrollView;
        }

        private VisualElement CreateInputRow()
        {
            var inputRow = new VisualElement();
            inputRow.style.flexDirection = FlexDirection.Row;
            inputRow.style.height = 30;
            inputRow.style.alignItems = Align.Center;
            var prompt = new Label(">");
            prompt.style.color = GregUITheme.PrimaryAccent;
            prompt.style.width = 20;
            prompt.style.unityFontStyleAndWeight = FontStyle.Bold;
            inputRow.Add(prompt);
            _inputField = new TextField();
            _inputField.style.flexGrow = 1;
            _inputField.style.height = 30;
            _inputField.RegisterCallback<KeyDownEvent>(new Action<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    SubmitCommand();
                    evt.StopPropagation();
                }
            }));
            inputRow.Add(_inputField);
            var submitBtn = new Button();
            submitBtn.text = "Send";
            submitBtn.style.width = 60;
            submitBtn.style.height = 30;
            submitBtn.style.marginLeft = 8;
            submitBtn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => SubmitCommand()));
            inputRow.Add(submitBtn);
            return inputRow;
        }

        private VisualElement CreateButtonRow()
        {
            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.marginTop = 8;
            var clearBtn = new Button();
            clearBtn.text = "Clear";
            clearBtn.style.width = 80;
            clearBtn.style.height = 28;
            clearBtn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ =>
            {
                _logs.Clear();
                RefreshLogDisplay();
            }));
            buttonRow.Add(clearBtn);
            return buttonRow;
        }

        private void RefreshLogDisplay()
        {
            if (_logContainer == null) return;
            _logContainer.Clear();

            int start = Math.Max(0, _logs.Count - 50);
            for (int i = start; i < _logs.Count; i++)
            {
                var logLabel = new Label(_logs[i]);
                logLabel.style.color = GetLogColor(_logs[i]);
                logLabel.style.fontSize = 12;
                logLabel.style.whiteSpace = WhiteSpace.Normal;
                logLabel.style.marginBottom = 2;
                _logContainer.Add(logLabel);
            }

            _logScrollView?.schedule.Execute(new Action<TimerState>(_ =>
            {
                try
                {
                    var scroll = _logScrollView;
                    if (scroll == null) return;
                    var content = scroll.contentContainer;
                    if (content != null && content.childCount > 0)
                        scroll.ScrollTo(content[content.childCount - 1]);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }));
        }

        private Color GetLogColor(string log)
        {
            if (log.Contains("[ERROR]")) return new Color(1f, 0.4f, 0.4f);
            if (log.Contains("[WARN]")) return new Color(1f, 0.8f, 0.3f);
            if (log.Contains("[COMMAND]")) return GregUITheme.PrimaryAccent;
            return new Color(0.85f, 0.85f, 0.87f);
        }

        private void SubmitCommand()
        {
            if (_inputField == null) return;
            var text = _inputField.value?.Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                AddLog(text, "COMMAND");
                ExecuteCommand(text);
                _inputField.value = "";
            }
        }

        private void ExecuteCommand(string text)
        {
            try
            {
                string cmd = ParseCommand(text);
                if (TryRunBuiltin(cmd)) return;
                if (TryRunInfo(cmd)) return;
                AddRaw($"[ERROR] Unknown command '{cmd}'. Type 'help'.");
            }
            catch (Exception ex)
            {
                AddRaw($"[ERROR] Command failed: {ex.Message}");
            }
        }

        private static string ParseCommand(string text)
        {
            try
            {
                var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length > 0 ? parts[0].ToLowerInvariant() : "";
            }
            catch { return ""; }
        }

        private bool TryRunBuiltin(string cmd)
        {
            try
            {
                if (cmd == "help")
                {
                    AddRaw("[INFO] Commands: help, clear, mods, menus, keys, version");
                    return true;
                }
                if (cmd == "clear")
                {
                    _logs.Clear();
                    RefreshLogDisplay();
                    return true;
                }
                return false;
            }
            catch (Exception ex) { AddRaw($"[ERROR] builtin: {ex.Message}"); return true; }
        }

        private bool TryRunInfo(string cmd)
        {
            try
            {
                if (cmd == "mods") { ShowMods(); return true; }
                if (cmd == "menus") { ShowMenus(); return true; }
                if (cmd == "keys") { ShowKeys(); return true; }
                if (cmd == "version") { ShowVersion(); return true; }
                return false;
            }
            catch (Exception ex) { AddRaw($"[ERROR] info: {ex.Message}"); return true; }
        }

        private void ShowMods()
        {
            try
            {
                var mods = Core.Mods.GregModRegistry.All();
                if (mods == null || mods.Count == 0) { AddRaw("[INFO] No mods registered."); return; }
                foreach (var m in mods)
                {
                    if (m == null) continue;
                    AddRaw($"[INFO] {m.Name} v{m.Version}");
                }
            }
            catch (Exception ex) { AddRaw($"[ERROR] mods: {ex.Message}"); }
        }

        private void ShowMenus()
        {
            try
            {
                var menus = GregMenuRegistry.Snapshot();
                if (menus == null || menus.Count == 0) { AddRaw("[INFO] No menus registered."); return; }
                foreach (var m in menus)
                {
                    if (m == null) continue;
                    AddRaw($"[INFO] {m.MenuId} [{(m.Open ? "Offen" : "Zu")}]");
                }
            }
            catch (Exception ex) { AddRaw($"[ERROR] menus: {ex.Message}"); }
        }

        private void ShowKeys()
        {
            try
            {
                var entries = GregHudRegistry.All();
                if (entries == null || entries.Count == 0) { AddRaw("[INFO] No hotkeys registered."); return; }
                foreach (var e in entries)
                {
                    if (e == null) continue;
                    AddRaw($"[INFO] {e.Key} -> {e.Label}");
                }
            }
            catch (Exception ex) { AddRaw($"[ERROR] keys: {ex.Message}"); }
        }

        private void ShowVersion()
        {
            try
            {
                var v = typeof(GregDevConsole).Assembly.GetName().Version;
                AddRaw($"[INFO] gregCore {v}");
            }
            catch (Exception ex) { AddRaw($"[ERROR] version: {ex.Message}"); }
        }
    }
}
