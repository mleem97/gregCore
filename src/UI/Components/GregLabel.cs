using UnityEngine;

namespace gregCoreSDK.Core.UI.Components;

public class GregLabel : IGregUIComponent
{
    public string Text { get; set; } = "Label";
    public Color? Color { get; set; }
    public float FontSize { get; set; } = 12;

    public GameObject Build(Transform parent)
    {
        var go = new GameObject($"Label_{Text}");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, FontSize + 4);

        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = Text;
        text.fontSize = FontSize;
        text.color = Color ?? GregUITheme.OnSurface;
        text.alignment = TextAlignmentOptions.Left;

        return go;
    }
}
