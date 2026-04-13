using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Sdk.UI;

/// <summary>
/// Skeleton for Task 1.3: Data table UI system.
/// </summary>
public class FixedTableUI : MonoBehaviour
{
    public FixedTableUI(IntPtr ptr) : base(ptr) { }

    private GameObject _container;
    private VerticalLayoutGroup _layout;

    void Awake()
    {
        // Setup base container
        _container = gameObject;
        _layout = _container.AddComponent<VerticalLayoutGroup>();
        _layout.childForceExpandHeight = false;
        _layout.spacing = 5;

        // Add dummy row
        AddRow("gregCore Framework", "v1.0.0", "Active");
    }

    public void AddRow(string col1, string col2, string col3)
    {
        // To be implemented: Instantiate row prefab and set text
        Debug.Log($"[gregUI] Table Row: {col1} | {col2} | {col3}");
    }
}

