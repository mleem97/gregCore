using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppInterop.Runtime.Attributes;

namespace gregCore.UI
{
    public class GregUIBuilder
    {
        private GameObject _activePanel;
        private GameObject _contentContainer;

        public static GregUIBuilder Create(string title) => CreateTablet(title);

        public static GregUIBuilder CreateTablet(string title)
        {
            var builder = CreateBase(title, true);
            var rt = builder._activePanel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.15f, 0.15f);
            rt.anchorMax = new Vector2(0.85f, 0.85f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            
            builder._activePanel.GetComponent<Image>().color = GregUITheme.PrimaryAccent;
            return builder;
        }

        public static GregUIBuilder CreateWidget(string title, float x = 50, float y = 50)
        {
            var builder = CreateBase(title, false);
            var rt = builder._activePanel.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(300, 200);
            rt.anchoredPosition = new Vector2(x, -y);

            builder._activePanel.GetComponent<Image>().color = GregUITheme.NeutralBorder;
            return builder;
        }
private static GregUIBuilder CreateBase(string title, bool isTablet)
{
    var builder = new GregUIBuilder();
    builder._activePanel = GregUIManager.CreateUIObject($"Panel_{title}");
    builder._activePanel.SetActive(false); // Zuerst verstecken!

    // Outer Border
    var border = builder._activePanel.AddComponent<Image>();
            border.sprite = GregUITheme.RoundedSprite;
            border.type = Image.Type.Sliced;

            var bgObj = GregUIManager.CreateUIObject("Background", builder._activePanel);
            var bgRt = bgObj.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            float bWidth = isTablet ? GregUITheme.BorderWidthTablet : GregUITheme.BorderWidthWidget;
            bgRt.sizeDelta = new Vector2(-bWidth * 2, -bWidth * 2);
            
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = GregUITheme.PanelBackground;
            bgImg.sprite = GregUITheme.RoundedSprite;
            bgImg.type = Image.Type.Sliced;

            var header = GregUIManager.CreateUIObject("Header", builder._activePanel);
            var hRt = header.AddComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0, 1);
            hRt.anchorMax = new Vector2(1, 1);
            hRt.pivot = new Vector2(0.5f, 1);
            hRt.sizeDelta = new Vector2(0, GregUITheme.HeaderHeight);
            hRt.anchoredPosition = Vector2.zero;

            var hTxt = header.AddComponent<Text>();
            hTxt.text = title.ToUpper();
            GregUITheme.ApplyText(hTxt, true);
            hTxt.alignment = TextAnchor.MiddleCenter;

            builder._contentContainer = GregUIManager.CreateUIObject("Content", builder._activePanel);
            var cRt = builder._contentContainer.AddComponent<RectTransform>();
            cRt.anchorMin = Vector2.zero;
            cRt.anchorMax = Vector2.one;
            cRt.offsetMin = new Vector2(GregUITheme.Padding, GregUITheme.Padding);
            cRt.offsetMax = new Vector2(-GregUITheme.Padding, -GregUITheme.HeaderHeight - GregUITheme.Padding);

            var layout = builder._contentContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = GregUITheme.Spacing;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            builder._activePanel.AddComponent<GregUIDragHandler>();
            return builder;
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            var rt = _activePanel.GetComponent<RectTransform>();
            if (rt != null) rt.sizeDelta = new Vector2(width, height);
            return this;
        }

        public GregUIBuilder AddHeadline(string text)
        {
            var obj = GregUIManager.CreateUIObject("Headline", _contentContainer);
            var txt = obj.AddComponent<Text>();
            txt.text = text.ToUpper();
            GregUITheme.ApplyText(txt, true);
            return this;
        }

        public GregUIBuilder AddLabel(string text)
        {
            var obj = GregUIManager.CreateUIObject("Label", _contentContainer);
            var txt = obj.AddComponent<Text>();
            txt.text = text;
            GregUITheme.ApplyText(txt);
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick) => AddPrimaryButton(label, onClick);

        public GregUIBuilder AddPrimaryButton(string label, Action onClick) => AddButtonImpl(label, onClick, true);
        public GregUIBuilder AddSecondaryButton(string label, Action onClick) => AddButtonImpl(label, onClick, false);

        private GregUIBuilder AddButtonImpl(string label, Action onClick, bool isPrimary)
        {
            var btnObj = GregUIManager.CreateUIObject("Button", _contentContainer);
            var img = btnObj.AddComponent<Image>();
            var btn = btnObj.AddComponent<Button>();
            btnObj.AddComponent<LayoutElement>().minHeight = 40f;

            var txt = GregUIManager.CreateUIObject("Text", btnObj).AddComponent<Text>();
            txt.text = label.ToUpper();
            GregUITheme.ApplyText(txt);
            txt.color = Color.black;
            txt.alignment = TextAnchor.MiddleCenter;

            if (isPrimary) GregUITheme.ApplyPrimaryButton(btn, img);
            else GregUITheme.ApplySecondaryButton(btn, img);

            btn.onClick.AddListener(new Action(onClick));
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            var obj = GregUIManager.CreateUIObject("Toggle", _contentContainer);
            obj.AddComponent<LayoutElement>().minHeight = 34f;
            var toggle = obj.AddComponent<Toggle>();
            toggle.isOn = currentValue;
            toggle.onValueChanged.AddListener(new Action<bool>(onChanged));
            var txt = GregUIManager.CreateUIObject("Label", obj).AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            var obj = GregUIManager.CreateUIObject("SliderGroup", _contentContainer);
            obj.AddComponent<LayoutElement>().minHeight = 40f;
            var slider = obj.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = currentValue;
            slider.onValueChanged.AddListener(new Action<float>(onChanged));
            return this;
        }

        public GregUIBuilder AddInputField(string label, string defaultValue, Action<string> onChanged)
        {
            var obj = GregUIManager.CreateUIObject("InputGroup", _contentContainer);
            obj.AddComponent<LayoutElement>().minHeight = 40f;
            var inputObj = GregUIManager.CreateUIObject("InputField", obj);
            var input = inputObj.AddComponent<InputField>();
            input.text = defaultValue;
            input.onValueChanged.AddListener(new Action<string>(onChanged));
            var txt = inputObj.AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);
            return this;
        }

        public GregUIBuilder AddDropdown<T>(string label, T[] options, int selectedIndex, Action<T> onSelected)
        {
            var obj = GregUIManager.CreateUIObject("DropdownGroup", _contentContainer);
            obj.AddComponent<LayoutElement>().minHeight = 40f;
            var dp = obj.AddComponent<Dropdown>();
            dp.value = selectedIndex;
            var txt = GregUIManager.CreateUIObject("Label", obj).AddComponent<Text>();
            txt.text = label;
            GregUITheme.ApplyText(txt);
            return this;
        }

        public GregUIBuilder AddSection(string title)
        {
            return AddHeadline(title);
        }

        public GregUIBuilder AddSpacer(float height = 20f)
        {
            var obj = GregUIManager.CreateUIObject("Spacer", _contentContainer);
            obj.AddComponent<LayoutElement>().minHeight = height;
            return this;
        }

        public GregUIBuilder AddSearchableList<T>(IEnumerable<T> items, Func<T, string> labelSelector, Action<T> onSelected)
        {
            // Implementation of a scrollable list (UGUI ScrollRect based)
            var scrollObj = GregUIManager.CreateUIObject("ScrollList", _contentContainer);
            scrollObj.AddComponent<LayoutElement>().preferredHeight = 200f;
            var scrollRect = scrollObj.AddComponent<ScrollRect>();
            
            var viewport = GregUIManager.CreateUIObject("Viewport", scrollObj);
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            viewport.AddComponent<Mask>();
            
            var content = GregUIManager.CreateUIObject("Content", viewport);
            var cRt = content.AddComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot = new Vector2(0.5f, 1);
            
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = cRt;

            foreach (var item in items)
            {
                var label = labelSelector(item);
                var btnObj = GregUIManager.CreateUIObject("Item", content);
                var btn = btnObj.AddComponent<Button>();
                var txt = GregUIManager.CreateUIObject("Text", btnObj).AddComponent<Text>();
                txt.text = label;
                GregUITheme.ApplyText(txt);
                btn.onClick.AddListener(new Action(() => onSelected(item)));
            }

            return this;
        }

        public GameObject Build() 
        {
            var name = _activePanel.name.Replace("Panel_", "");
            GregUIManager.RegisterPanel(name, _activePanel);
            return _activePanel;
        }

        public GregUIBuilder Show() => SetVisible(true);
        public GregUIBuilder Hide() => SetVisible(false);
        public GregUIBuilder Toggle() => SetVisible(!_activePanel.activeSelf);

        private GregUIBuilder SetVisible(bool visible)
        {
            if (_activePanel != null) _activePanel.SetActive(visible);
            return this;
        }

        public bool IsVisible => _activePanel?.activeSelf ?? false;
    }

    public class GregUIDragHandler : MonoBehaviour
    {
        public GregUIDragHandler(IntPtr ptr) : base(ptr) { }
        [HideFromIl2Cpp] public void OnDrag(PointerEventData eventData) => transform.position += (Vector3)eventData.delta;
    }
}
