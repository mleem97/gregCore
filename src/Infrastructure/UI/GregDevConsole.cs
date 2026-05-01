using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using gregCore.UI;

namespace gregCore.Infrastructure.UI
{
    public class GregDevConsole : MonoBehaviour
    {
        public GregDevConsole(IntPtr ptr) : base(ptr) { }

        public static GregDevConsole Instance { get; private set; } = null!;

        private readonly List<string> _logs = new();
        private string _input = "";
        private bool _isVisible = false;
        private Rect _rect = new Rect(50, 50, 600, 400);

        public static void Initialize()
        {
            var go = new GameObject("greg_DevConsole_Host");
            UnityEngine.Object.DontDestroyOnLoad(go);
            
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<GregDevConsole>();
            Instance = go.AddComponent(Il2CppInterop.Runtime.Il2CppType.Of<GregDevConsole>()).Cast<GregDevConsole>();
        }

        public bool IsOpen => _isVisible;

        public void AddLog(string msg, string type = "INFO")
        {
            string typeStr = string.IsNullOrEmpty(type) ? "INFO" : type;
            MelonLoader.MelonLogger.Msg($"[{typeStr}] {msg}");
            _logs.Add($"[{typeStr}] {msg}");
            if (_logs.Count > 50) _logs.RemoveAt(0);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.backquoteKey.wasPressedThisFrame || keyboard.f12Key.wasPressedThisFrame)
            {
                _isVisible = !_isVisible;
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            GregImGui.EnsureInitialized();
            GregImGui.DrawBox(_rect, "Developer Console");

            float x = _rect.x + 15;
            float y = _rect.y + 40;
            float w = _rect.width - 30;

            // Log display
            var logRect = new Rect(x, y, w, _rect.height - 100);
            GUI.Box(logRect, "", GUI.skin.box);
            
            float logY = logRect.y + 5;
            for (int i = Math.Max(0, _logs.Count - 15); i < _logs.Count; i++)
            {
                GUI.Label(new Rect(x + 5, logY, w - 10, 20), _logs[i], GregImGui.stLabel);
                logY += 18;
            }

            // Input
            var inputRect = new Rect(x, _rect.y + _rect.height - 40, w, 25);
            _input = GUI.TextField(inputRect, _input);
            
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                if (!string.IsNullOrWhiteSpace(_input))
                {
                    AddLog(_input, "COMMAND");
                    _input = "";
                }
            }
        }
    }
}
