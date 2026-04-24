using System;
using UnityEngine;
using UnityEngine.UI;
using gregCore.UI;

namespace gregCore.Infrastructure.UI
{
    public class GregDevConsole : MonoBehaviour
    {
        public GregDevConsole(IntPtr ptr) : base(ptr) { }

        public static GregDevConsole Instance { get; private set; } = null!;

        private GameObject _uiPanel = null!;
        private InputField _inputField = null!;
        private Text _logDisplay = null!;
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F12))
            {
                _isVisible = !_isVisible;
                if (_isVisible && _uiPanel == null) BuildUI();
                GregUIManager.SetPanelActive("DevConsole", _isVisible);
            }
        }

        private void BuildUI()
        {
            var builder = GregUIBuilder.CreateWidget("DevConsole", 50, Screen.height - 450)
                .SetSize(600, 400)
                .AddHeadline("Developer Console")
                .AddLabel("gregCore v1.0.0.38-pre (Unity 6 / IL2CPP)");
            
            _uiPanel = builder.Build();
            
            // Note: Full console implementation with input field/log would go here
            // For now, it's a functional "Tablet" widget as a proof of concept.
        }
    }
}
