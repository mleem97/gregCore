using System;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace greg.Sdk.Services;

/// <summary>
/// Service for handling Unity UI Toolkit (UXML/USS) interfaces.
/// Allows loading visual trees from AssetBundles and attaching them to the game viewport.
/// </summary>
public static class GregUxmlService
{
    private static GameObject _uiRoot;
    private static PanelSettings _defaultPanelSettings;
    
    // UI name (e.g. "MainMenu") -> UI override action
    private static System.Collections.Generic.Dictionary<string, Action> _overrides = new();

    public static void RegisterOverride(string uiName, Action showAction)
    {
        _overrides[uiName] = showAction;
        MelonLogger.Msg($"[GregUxmlService] Registered UXML override for '{uiName}'.");
    }

    public static bool HasOverride(string uiName)
    {
        return _overrides.ContainsKey(uiName);
    }

    public static void ShowOverride(string uiName)
    {
        if (_overrides.TryGetValue(uiName, out var action))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Creates a new UIDocument from a UXML template.
    /// </summary>
    /// <param name="documentName">Name for the GameObject container.</param>
    /// <param name="visualTree">The UXML template (VisualTreeAsset).</param>
    /// <param name="styleSheet">Optional USS stylesheet.</param>
    /// <returns>The created UIDocument component.</returns>
    public static UIDocument CreateInterface(string documentName, VisualTreeAsset visualTree, StyleSheet styleSheet = null)
    {
        EnsureRoot();

        var go = new GameObject($"GregUxml_{documentName}");
        go.transform.SetParent(_uiRoot.transform);

        var doc = go.AddComponent<UIDocument>();
        doc.panelSettings = GetDefaultSettings();
        doc.visualTreeAsset = visualTree;

        if (styleSheet != null)
        {
            doc.rootVisualElement.styleSheets.Add(styleSheet);
        }

        return doc;
    }

    private static void EnsureRoot()
    {
        if (_uiRoot != null) return;

        _uiRoot = new GameObject("GregUxml_Root");
        UnityEngine.Object.DontDestroyOnLoad(_uiRoot);
    }

    private static PanelSettings GetDefaultSettings()
    {
        if (_defaultPanelSettings != null) return _defaultPanelSettings;

        // Try to find existing panel settings or create a default one
        _defaultPanelSettings = Resources.FindObjectsOfTypeAll<PanelSettings>().Length > 0 
            ? Resources.FindObjectsOfTypeAll<PanelSettings>()[0] 
            : ScriptableObject.CreateInstance<PanelSettings>();
            
        return _defaultPanelSettings;
    }
}
