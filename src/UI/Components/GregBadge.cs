using UnityEngine;
using UnityEngine.UI;

namespace gregCoreSDK.Core.UI.Components;

public class GregBadge : IGregUIComponent
{
    public string Text { get; set; } = "Badge";
    public Color? Color { get; set; }

    public GameObject Build(Transform parent)
    {
        var go = new GameObject($"Badge_{Text}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(60, 20);

        var img = go.AddComponent<Image>();
        img.color = Color ?? GregUITheme.PrimaryContainer;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = Text;
        text.fontSize = 10;
        text.color = GregUITheme.OnPrimary;
        text.alignment = TextAlignmentOptions.Center;

        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return go;
    }
}
