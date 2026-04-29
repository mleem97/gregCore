using System;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.UI;

namespace gregCore.Infrastructure.UI
{
    public class GregDevConsole : MonoBehaviour
    {
        public GregDevConsole(IntPtr ptr) : base(ptr) { }

        public static GregDevConsole Instance { get; private set; } = null!;

        private VisualElement? _root;
        private TextField? _inputField;
        private ScrollView? _logDisplay;
        private bool _isVisible = false;

        public static void Initialize()
        {
            var go = new GameObject("greg_DevConsole_Host");
            UnityEngine.Object.DontDestroyOnLoad(go);
            
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<GregDevConsole>();
            Instance = go.AddComponent(Il2CppInterop.Runtime.Il2CppType.Of<GregDevConsole>()).Cast<GregDevConsole>();
        }

        public bool IsOpen => _isVisible;

        public void AddLog(string msg, object type = null)
        {
            string typeStr = type?.ToString() ?? "INFO";
            MelonLoader.MelonLogger.Msg($"[{typeStr}] {msg}");
            
            if (_logDisplay != null)
            {
                var label = new Label($"[{typeStr}] {msg}")
                {
                    style =
                    {
                        fontSize = 12,
                        color = typeStr == "ERROR" ? new Color(1f, 0.32f, 0.32f) : 
                               typeStr == "WARN" ? new Color(1f, 0.76f, 0.03f) : 
                               new Color(0.88f, 0.88f, 0.88f),
                        whiteSpace = WhiteSpace.Normal,
                        marginBottom = 2
                    }
                };
                _logDisplay.Add(label);
                _logDisplay.ScrollTo(label);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F12))
            {
                _isVisible = !_isVisible;
                if (_isVisible && _root == null) BuildUI();
                if (_root != null)
                    _root.style.display = _isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (_isVisible && _inputField != null && Input.GetKeyDown(KeyCode.Return))
            {
                string command = _inputField.value;
                if (!string.IsNullOrWhiteSpace(command))
                {
                    AddLog(command, "COMMAND");
                    _inputField.value = "";
                }
            }
        }

        private void BuildUI()
        {
            _root = new VisualElement
            {
                name = "DevConsole",
                style =
                {
                    position = Position.Absolute,
                    top = 50,
                    left = 50,
                    width = 600,
                    height = 400,
                    backgroundColor = new Color(0.07f, 0.07f, 0.07f, 0.96f),
                    borderTopColor = new Color(0f, 0.75f, 0.65f),
                    borderBottomColor = new Color(0f, 0.75f, 0.65f),
                    borderLeftColor = new Color(0f, 0.75f, 0.65f),
                    borderRightColor = new Color(0f, 0.75f, 0.65f),
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderTopLeftRadius = 8,
                    borderTopRightRadius = 8,
                    borderBottomLeftRadius = 8,
                    borderBottomRightRadius = 8,
                    flexDirection = FlexDirection.Column,
                    paddingTop = 10,
                    paddingBottom = 10,
                    paddingLeft = 10,
                    paddingRight = 10
                }
            };

            // Header
            var header = new Label("Developer Console")
            {
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = new Color(0f, 0.75f, 0.65f),
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginBottom = 8,
                    borderBottomColor = new Color(0.2f, 0.2f, 0.2f),
                    borderBottomWidth = 1,
                    paddingBottom = 6
                }
            };
            _root.Add(header);

            // Log display
            _logDisplay = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    marginTop = 4,
                    marginBottom = 8,
                    backgroundColor = new Color(0.05f, 0.05f, 0.05f, 0.8f),
                    borderTopColor = new Color(0.15f, 0.15f, 0.15f),
                    borderBottomColor = new Color(0.15f, 0.15f, 0.15f),
                    borderLeftColor = new Color(0.15f, 0.15f, 0.15f),
                    borderRightColor = new Color(0.15f, 0.15f, 0.15f),
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    borderLeftWidth = 1,
                    borderRightWidth = 1,
                    borderTopLeftRadius = 4,
                    borderTopRightRadius = 4,
                    borderBottomLeftRadius = 4,
                    borderBottomRightRadius = 4,
                    paddingTop = 6,
                    paddingBottom = 6,
                    paddingLeft = 6,
                    paddingRight = 6
                }
            };
            _root.Add(_logDisplay);

            // Input field
            _inputField = new TextField
            {
                style =
                {
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                    color = Color.white,
                    height = 24
                }
            };
            _root.Add(_inputField);

            GregUIManager.RegisterPanel("DevConsole", _root);
        }
    }
}
