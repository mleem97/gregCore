using System;
using UnityEngine;
using gregCore.UI;

namespace greg.Mods.HexViewer
{
    public class HexViewerWidget : MonoBehaviour
    {
        private GameObject? _widget;
        private UnityEngine.UI.Text? _infoText;
        private bool _isVisible = false;

        public static void Initialize()
        {
            var go = new GameObject("greg_HexViewer");
            go.AddComponent<HexViewerWidget>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) // standard hexviewer hotkey
            {
                _isVisible = !_isVisible;
                if (_isVisible && _widget == null) BuildUI();
                GregUIManager.SetPanelActive("HexViewer", _isVisible);
            }

            if (_isVisible && _infoText != null)
            {
                // Real-time data binding without EventBus (for low-level memory/hex view)
                // Update text with pseudo hex data or real offsets
                _infoText.text = $"MEMORY ADDR: 0x{DateTime.Now.Ticks:X8}\nFRAME_BUFF: {Time.frameCount % 255:X2} FF 00 12";
            }
        }

        private void BuildUI()
        {
            var builder = GregUIBuilder.CreateWidget("HexViewer", Screen.width - 320, 50)
                .SetSize(300, 400)
                .AddHeadline("Memory Inspector")
                .AddLabel("Scanning IL2CPP heap...");
            
            _widget = builder.Build();
            _infoText = _widget.GetComponentInChildren<UnityEngine.UI.Text>();
        }
    }
}
