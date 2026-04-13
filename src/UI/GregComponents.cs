using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Sdk.UI;

/// <summary>
/// Skeleton for Task 1.3: Standardized Button component.
/// </summary>
public class GregButton : MonoBehaviour
{
    public GregButton(IntPtr ptr) : base(ptr) { }

    public delegate void OnClickDelegate();
    public OnClickDelegate OnClick;

    public void Setup(string label)
    {
        var text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = label;

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(new Action(() => OnClick?.Invoke()));
        }
    }
}

/// <summary>
/// Skeleton for Task 1.3: Standardized Toggle component.
/// </summary>
public class GregToggle : MonoBehaviour
{
    public GregToggle(IntPtr ptr) : base(ptr) { }

    public delegate void OnToggleDelegate(bool value);
    public OnToggleDelegate OnValueChanged;

    public void Setup(bool initialState)
    {
        var toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = initialState;
            toggle.onValueChanged.AddListener(new Action<bool>((val) => OnValueChanged?.Invoke(val)));
        }
    }
}
