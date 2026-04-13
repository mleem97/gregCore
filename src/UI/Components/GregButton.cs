using System;
using UnityEngine;
using UnityEngine.UI;

namespace gregCoreSDK.Core.UI.Components;

public enum GregButtonStyle { Primary, Secondary, Tertiary, Danger }

public class GregButton : IGregUIComponent
{
    public string Label { get; set; } = "Button";
    public Action OnClick { get; set; }
    public GregButtonStyle Style { get; set; } = GregButtonStyle.Primary;

    public GameObject Build(Transform parent)
    {
        var go = new GameObject($"Button_{Label}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 32);

        var img = go.AddComponent<Image>();
        img.color = Style switch
        {
            GregButtonStyle.Primary   => GregUITheme.Primary,
            GregButtonStyle.Secondary => GregUITheme.Secondary,
            GregButtonStyle.Tertiary  => GregUITheme.Tertiary,
            GregButtonStyle.Danger    => GregUITheme.Error,
            _ => GregUITheme.Primary
        };

        var btn = go.AddComponent<Button>();
        if (OnClick != null)
        {
            btn.onClick.AddListener(OnClick);
        }

        var textGO = new GameObject("Label");
        textGO.transform.SetParent(go.transform, false);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = Label;
        text.fontSize = 12;
        text.color = GregUITheme.OnPrimary;
        text.alignment = TextAlignmentOptions.Center;
        
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return go;
    }
}
