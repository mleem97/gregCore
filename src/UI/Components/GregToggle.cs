using System;
using UnityEngine;
using UnityEngine.UI;

namespace gregCoreSDK.Core.UI.Components;

public class GregToggle : IGregUIComponent
{
    public string Label { get; set; } = "Toggle";
    public bool Value { get; set; }
    public Action<bool> OnChange { get; set; }

    public GameObject Build(Transform parent)
    {
        var go = new GameObject($"Toggle_{Label}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 24);

        var layout = go.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childForceExpandWidth = false;
        layout.spacing = 8;

        var toggleGO = new GameObject("ToggleRoot");
        toggleGO.transform.SetParent(go.transform, false);
        var toggleRect = toggleGO.AddComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(24, 24);

        var bg = toggleGO.AddComponent<Image>();
        bg.color = GregUITheme.SurfaceContainer;

        var toggle = toggleGO.AddComponent<Toggle>();
        
        var checkmarkGO = new GameObject("Checkmark");
        checkmarkGO.transform.SetParent(toggleGO.transform, false);
        var checkmark = checkmarkGO.AddComponent<Image>();
        checkmark.color = GregUITheme.Primary;
        toggle.graphic = checkmark;
        
        toggle.isOn = Value;
        toggle.onValueChanged.AddListener(OnChange);

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var text = labelGO.AddComponent<TextMeshProUGUI>();
        text.text = Label;
        text.fontSize = 12;
        text.color = GregUITheme.OnSurface;

        return go;
    }
}
