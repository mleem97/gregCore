using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime;

namespace gregCore.UI
{
    /// <summary>
    /// Builder for programmatic UGUI creation in gregCore.
    /// Avoids IMGUI stripping issues in Unity 6.
    /// </summary>
    public class GregUIBuilder
    {
        private GameObject _activePanel;

        public static GregUIBuilder Create(string title)
        {
            var builder = new GregUIBuilder();
            builder._activePanel = GregUIManager.CreateUIObject($"Panel_{title}");
            
            var rt = builder._activePanel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, 300);
            rt.anchoredPosition = Vector2.zero;

            var img = builder._activePanel.AddComponent<Image>();
            GregUITheme.ApplyBackground(img);

            // Add Header
            var header = GregUIManager.CreateUIObject("Header", builder._activePanel);
            var hRt = header.AddComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0, 1);
            hRt.anchorMax = new Vector2(1, 1);
            hRt.pivot = new Vector2(0.5f, 1);
            hRt.sizeDelta = new Vector2(0, GregUITheme.HeaderHeight);
            hRt.anchoredPosition = Vector2.zero;

            var hTxt = header.AddComponent<Text>();
            hTxt.text = title;
            GregUITheme.ApplyText(hTxt, true);
            hTxt.alignment = TextAnchor.MiddleCenter;

            // Add basic drag support
            builder._activePanel.AddComponent<GregUIDragHandler>();

            return builder;
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            var rt = _activePanel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, height);
            return this;
        }

        public GregUIBuilder AddLabel(string text, int fontSize = 14)
        {
            var labelObj = GregUIManager.CreateUIObject("Label", _activePanel);
            var txt = labelObj.AddComponent<Text>();
            txt.text = text;
            GregUITheme.ApplyText(txt);
            if (fontSize != 14) txt.fontSize = fontSize;
            txt.alignment = TextAnchor.UpperLeft;

            var rt = labelObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = new Vector2(-GregUITheme.Padding * 2, -GregUITheme.Padding * 2);

            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick)
        {
            var btnObj = GregUIManager.CreateUIObject("Button", _activePanel);
            var img = btnObj.AddComponent<Image>();
            
            var btn = btnObj.AddComponent<Button>();
            GregUITheme.ApplyButton(btn);
            // Use UnityAction to wrap the system action safely for IL2CPP
            btn.onClick.AddListener(new Action(onClick));

            var textObj = GregUIManager.CreateUIObject("Text", btnObj);
            var txt = textObj.AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);
            txt.color = Color.black; // Better contrast on buttons usually, or use Accent
            txt.alignment = TextAnchor.MiddleCenter;

            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            var toggleObj = GregUIManager.CreateUIObject("Toggle", _activePanel);
            var toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = currentValue;
            toggle.onValueChanged.AddListener(new Action<bool>(onChanged));

            var labelObj = GregUIManager.CreateUIObject("Label", toggleObj);
            var txt = labelObj.AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);

            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            var sliderObj = GregUIManager.CreateUIObject("Slider", _activePanel);
            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = currentValue;
            slider.onValueChanged.AddListener(new Action<float>(onChanged));

            var labelObj = GregUIManager.CreateUIObject("Label", sliderObj);
            var txt = labelObj.AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);

            return this;
        }

        public GameObject Build() 
        {
            var name = _activePanel.name.Replace("Panel_", "");
            GregUIManager.RegisterPanel(name, _activePanel);
            return _activePanel;
        }
    }

    /// <summary>
    /// Helper for dragging UI elements. 
    /// Must be registered in IL2CPP.
    /// </summary>
    public class GregUIDragHandler : MonoBehaviour
    {
        public GregUIDragHandler(IntPtr ptr) : base(ptr) { }

        [HideFromIl2Cpp]
        public void OnDrag(PointerEventData eventData)
        {
            transform.position += (Vector3)eventData.delta;
        }
    }
}
