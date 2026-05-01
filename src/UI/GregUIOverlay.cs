using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// TextMeshProUGUI text overlay for UI Toolkit panels.
    /// Renders visible text using the game's TMP font assets (SDF quality).
    /// Canvas sits at sortingOrder+1 above the UI Toolkit panel.
    /// </summary>
    public class GregUIOverlay
    {
        private GameObject _canvasGo;
        private Canvas _canvas;
        private TMP_FontAsset? _tmpFont;
        private bool _isVisible;

        public bool IsVisible => _isVisible;

        public GregUIOverlay(string name, int sortingOrder)
        {
            _canvasGo = new GameObject($"Overlay_{name}");
            _canvasGo.layer = 5; // UI Layer
            UnityEngine.Object.DontDestroyOnLoad(_canvasGo);

            _canvas = _canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = sortingOrder + 1; // always one above the UI Toolkit panel

            var scaler = _canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            _canvasGo.AddComponent<GraphicRaycaster>();

            _tmpFont = GregFontLoader.GetTMPFontAsset();
            if (_tmpFont == null)
                MelonLogger.Warning("[UIOverlay] No TMP_FontAsset found — text will use TMP default");
            else
                MelonLogger.Msg($"[UIOverlay] Using TMP font: {_tmpFont.name}");

            _canvasGo.SetActive(false);
        }

        /// <summary>
        /// Add a text label at UI Toolkit coordinates (origin top-left, y grows downward).
        /// </summary>
        public TextMeshProUGUI AddLabel(string text, float x, float y, float width, float height,
            int fontSize = 14, Color? color = null, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject($"Label_{text.GetHashCode()}");
            go.layer = 5; // UI Layer
            go.transform.SetParent(_canvasGo.transform, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);

            var txt = go.AddComponent<TextMeshProUGUI>();
            if (_tmpFont != null)
            {
                txt.font = _tmpFont;
                var mat = GregFontLoader.GetTMPMaterial();
                if (mat != null)
                {
                    txt.fontSharedMaterial = mat;
                }
            }

            txt.fontSize = fontSize;
            txt.fontStyle = style == FontStyle.Bold ? FontStyles.Bold : FontStyles.Normal;
            txt.color = color ?? Color.white;
            txt.text = text;
            txt.alignment = TextAlignmentOptions.MidlineLeft;
            txt.overflowMode = TextOverflowModes.Truncate;
            txt.enableWordWrapping = true;
            
            txt.enabled = true;
            try { txt.ForceMeshUpdate(); } catch { }

            return txt;
        }

        public void Show()
        {
            _isVisible = true;
            _canvasGo.SetActive(true);
        }

        public void Hide()
        {
            _isVisible = false;
            _canvasGo.SetActive(false);
        }

        public void Toggle()
        {
            if (_isVisible) Hide();
            else Show();
        }

        public void Destroy()
        {
            if (_canvasGo != null)
                UnityEngine.Object.Destroy(_canvasGo);
        }
    }
}
