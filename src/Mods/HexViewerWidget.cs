using System;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.UI;

namespace greg.Mods.HexViewer
{
    /// <summary>
    /// HexViewer widget using UI Toolkit (primary) with no UGUI dependencies.
    /// </summary>
    public class HexViewerWidget : MonoBehaviour
    {
        private VisualElement? _root;
        private Label? _infoLabel;
        private bool _isVisible = false;

        public static void Initialize()
        {
            var go = new GameObject("greg_HexViewer");
            UnityEngine.Object.DontDestroyOnLoad(go);

            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<HexViewerWidget>();
            go.AddComponent(Il2CppInterop.Runtime.Il2CppType.Of<HexViewerWidget>());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _isVisible = !_isVisible;
                if (_isVisible && _root == null) BuildUI();
                GregUIManager.SetPanelActive("HexViewer", _isVisible);
            }

            if (_isVisible && _infoLabel != null)
            {
                _infoLabel.text = $"MEMORY ADDR: 0x{DateTime.Now.Ticks:X8}\nFRAME_BUFF: {Time.frameCount % 255:X2} FF 00 12";
            }
        }

        private void BuildUI()
        {
            var builder = GregUIBuilder.CreateWidget("HexViewer", Screen.width - 320, 50)
                .SetSize(300, 400)
                .AddHeadline("Memory Inspector")
                .AddLabel("Scanning IL2CPP heap...");

            _root = builder.Build();

            // Find the last label added (our info label) by querying the content
            var content = _root?.Q<VisualElement>("Content");
            if (content != null)
            {
                // Replace the placeholder label with our tracked label
                var placeholder = content.Q<Label>();
                if (placeholder != null)
                {
                    _infoLabel = placeholder;
                }
            }
        }
    }
}
