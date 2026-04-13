using UnityEngine;
using UnityEngine.UI;

namespace gregCoreSDK.Core.UI.Components;

public class GregSeparator : IGregUIComponent
{
    public GameObject Build(Transform parent)
    {
        var go = new GameObject("Separator");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 1);

        var img = go.AddComponent<Image>();
        img.color = GregUITheme.GhostBorder;

        return go;
    }
}
