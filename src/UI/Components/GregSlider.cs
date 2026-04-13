using System;
using UnityEngine;
using UnityEngine.UI;

namespace gregCoreSDK.Core.UI.Components;

public class GregSlider : IGregUIComponent
{
    public string Label { get; set; } = "Slider";
    public float Min { get; set; } = 0;
    public float Max { get; set; } = 100;
    public float Value { get; set; }
    public Action<float> OnChange { get; set; }

    public GameObject Build(Transform parent)
    {
        var go = new GameObject($"Slider_{Label}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);

        var layout = go.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;
        layout.spacing = 4;

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var text = labelGO.AddComponent<TextMeshProUGUI>();
        text.text = Label;
        text.fontSize = 10;
        text.color = GregUITheme.OnSurface;

        var sliderRoot = new GameObject("SliderRoot");
        sliderRoot.transform.SetParent(go.transform, false);
        var sliderRect = sliderRoot.AddComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(0, 20);

        var slider = sliderRoot.AddComponent<Slider>();
        
        var background = new GameObject("Background");
        background.transform.SetParent(sliderRoot.transform, false);
        var bgImg = background.AddComponent<Image>();
        bgImg.color = GregUITheme.SurfaceContainer;
        var bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.sizeDelta = Vector2.zero;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderRoot.transform, false);
        var faRect = fillArea.AddComponent<RectTransform>();
        faRect.anchorMin = new Vector2(0, 0.25f);
        faRect.anchorMax = new Vector2(1, 0.75f);
        faRect.sizeDelta = new Vector2(-20, 0);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = GregUITheme.Primary;
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.fillRect.sizeDelta = Vector2.zero;

        var handleArea = new GameObject("Handle Area");
        handleArea.transform.SetParent(sliderRoot.transform, false);
        var haRect = handleArea.AddComponent<RectTransform>();
        haRect.anchorMin = new Vector2(0, 0);
        haRect.anchorMax = new Vector2(1, 1);
        haRect.sizeDelta = new Vector2(-20, 0);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = GregUITheme.Secondary;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.handleRect.sizeDelta = new Vector2(20, 0);

        slider.minValue = Min;
        slider.maxValue = Max;
        slider.value = Value;
        slider.onValueChanged.AddListener(OnChange);

        return go;
    }
}
