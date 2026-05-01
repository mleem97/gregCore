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

        private readonly List<string> _logs = new();
        private bool _isVisible = false;
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
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregDevConsole] Initialize failed: {ex.Message}");
            }
        }

        public bool IsOpen => _isVisible;

        public void AddLog(string msg, string type = "INFO")
        {
            string typeStr = string.IsNullOrEmpty(type) ? "INFO" : type;
            MelonLoader.MelonLogger.Msg($"[{typeStr}] {msg}");
            _logs.Add($"[{typeStr}] {msg}");
            if (_logs.Count > 200) _logs.RemoveAt(0);

            RefreshLogDisplay();
        }

        private void Update()
        {
            try
            {
                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                if (keyboard.backquoteKey.wasPressedThisFrame || keyboard.f12Key.wasPressedThisFrame)
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

            if (_isVisible) Hide();
            else Show();
        }

        private void Show()
        {
            _panel?.Show();
            _isVisible = true;
            _inputField?.Focus();
        }

        private void Hide()
        {
            _panel?.Hide();
            _isVisible = false;
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
                _inputField.value = "";
            }
        }
    }
}
