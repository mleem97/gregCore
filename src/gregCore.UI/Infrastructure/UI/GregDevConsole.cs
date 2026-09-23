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

        /// <summary>Framework-Menue-ID der Console (fuer Input-Lock/Cursor).</summary>
        public const string MenuId = "greg.console";

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

        public void AddLog(string msg, string type = "INFO")
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

        // Unity-Log-Feed: fängt Fehler/Warnungen aus Spiel + allen Mods ein.
        // Läuft ggf. abseits des Main-Threads -> nur einreihen, Anzeige im Update.
        // AddRaw ohne MelonLogger-Echo -> kein Feedback-Loop. Eigene Echos
        // (Timestamp-Prefix) werden als Duplikat erkannt und übersprungen.
        private void SubscribeLogFeed()
        {
            if (_feedSubscribed) return;
            _feedSubscribed = true;
            try
            {
                // Hinweis: Die Interop-Assembly projiziert das Event nicht als
                // C#-Event — nur add_/remove_. IL2CPP-Delegate per
                // DelegateSupport aus der verwalteten Methode erzeugen.
                var cb = Il2CppInterop.Runtime.DelegateSupport.ConvertDelegate<Application.LogCallback>(
                    (Action<string, string, LogType>)OnUnityLog);
                Application.add_logMessageReceived(cb);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[GregDevConsole] Log-Feed fehlgeschlagen: {ex.Message}");
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
            catch { }
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
                    // Eigenes MelonLogger-Echo (mit Timestamp) wiedererkennen.
                    string stripped = System.Text.RegularExpressions.Regex.Replace(
                        next.line, @"^\[\d{2}:\d{2}:\d{2}[.\d]*\]\s*", "");
                    lock (_logs)
                    {
                        if (_logs.Count > 0 && _logs[_logs.Count - 1] == stripped) continue;
                    }
                    AddRaw(next.line);
                }
            }
            catch { }
        }

        private void Update()
        {
            try
            {
                FlushLogFeed();

                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                // Nur Backquote: F12 gehört dem Export (sonst Doppelauslösung).
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
                try { GregInputLock.Refresh(); } catch { }
            }
            catch { }
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
                try { GregInputLock.Refresh(); } catch { }
            }
            catch { }
        }

        private void BuildUI()
        {
            try
            {
                _panel = GregPanelBuilder.Create("Developer Console")
                    .SetSize(700, 450)
                    .SetPosition(50, 50)
                    .Build(GregUILayerType.Overlay);

                var root = _panel.Root;
                if (root == null) return;

                var defaultScroll = root.Q<ScrollView>("PanelContent");
                if (defaultScroll != null)
                {
                    defaultScroll.RemoveFromHierarchy();
                }

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
                root.Add(_logScrollView);

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

                root.Add(inputRow);

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

                root.Add(buttonRow);

                RefreshLogDisplay();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] BuildUI failed: {ex.Message}");
            }
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
                if (_logScrollView != null)
                {
                    _logScrollView.scrollOffset = new Vector2(0, float.MaxValue);
                }
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
                var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts.Length > 0 ? parts[0].ToLowerInvariant() : "";
                switch (cmd)
                {
                    case "help":
                        AddRaw("[INFO] Commands: help, clear, mods, menus, keys, version");
                        break;
                    case "clear":
                        _logs.Clear();
                        RefreshLogDisplay();
                        break;
                    case "mods":
                        try
                        {
                            var mods = Core.Mods.GregModRegistry.All();
                            if (mods == null || mods.Count == 0) { AddRaw("[INFO] Keine Mods registriert."); break; }
                            foreach (var m in mods)
                            {
                                if (m == null) continue;
                                AddRaw($"[INFO] {m.Name} v{m.Version}");
                            }
                        }
                        catch (Exception ex) { AddRaw($"[ERROR] mods: {ex.Message}"); }
                        break;
                    case "menus":
                        try
                        {
                            var menus = GregMenuRegistry.Snapshot();
                            if (menus == null || menus.Count == 0) { AddRaw("[INFO] Keine Menues registriert."); break; }
                            foreach (var m in menus)
                            {
                                if (m == null) continue;
                                AddRaw($"[INFO] {m.MenuId} [{(m.Open ? "Offen" : "Zu")}]");
                            }
                        }
                        catch (Exception ex) { AddRaw($"[ERROR] menus: {ex.Message}"); }
                        break;
                    case "keys":
                        try
                        {
                            var entries = GregHudRegistry.All();
                            if (entries == null || entries.Count == 0) { AddRaw("[INFO] Keine Hotkeys registriert."); break; }
                            foreach (var e in entries)
                            {
                                if (e == null) continue;
                                AddRaw($"[INFO] {e.Key} -> {e.Label}");
                            }
                        }
                        catch (Exception ex) { AddRaw($"[ERROR] keys: {ex.Message}"); }
                        break;
                    case "version":
                        try
                        {
                            var v = typeof(GregDevConsole).Assembly.GetName().Version;
                            AddRaw($"[INFO] gregCore {v}");
                        }
                        catch (Exception ex) { AddRaw($"[ERROR] version: {ex.Message}"); }
                        break;
                    default:
                        AddRaw($"[ERROR] Unknown command '{cmd}'. Type 'help'.");
                        break;
                }
            }
            catch (Exception ex)
            {
                AddRaw($"[ERROR] Command failed: {ex.Message}");
            }
        }
    }
}
