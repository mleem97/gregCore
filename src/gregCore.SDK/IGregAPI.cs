using System;
using gregCore.Sdk.Models;

namespace gregCore.Sdk;

/// <summary>
/// The public interface for all mod developers (SDK layer).
/// Provides a stable, versioned API.
/// </summary>
public interface IGregAPI
{
    string Version { get; }

    // --- Hooks & Events ---
    void On(string hookName, Action<GregPayload> handler);
    void Fire(string hookName, GregPayload payload);

    // --- Mod Registration ---
    void RegisterMod(string modId, string name, string version, object? apiObject = null);

    // --- Settings & Input ---
    void RegisterToggle(string modId, string settingId, string displayName, bool defaultValue, Action<bool>? onChanged = null, string category = "General", string description = "");
    void RegisterSlider(string modId, string settingId, string displayName, float defaultValue, Action<float>? onChanged = null, string category = "General", string description = "");
    void RegisterKeybind(string modId, string actionId, string displayName, UnityEngine.KeyCode defaultKey, Action onPress, string category = "Controls", string description = "");

    // --- Notifications ---
    void ShowNotification(string title, string message, float duration = 5f);

    // --- Logging (short) ---
    void Log(string message);
    void Warn(string message);
    void Error(string message);

    // --- Toasts & F1 menus (UI) ---
    void Toast(string message, float duration = 3f);
    void ToastRich(string top, string title, string sub, float duration = 5f);
    void BindMenuToggle(string menuId, Action toggle, Func<bool> isOpen);
    void ReportMenu(string menuId, bool open);

    // --- Custom shop items (prefab remap + buttons) ---
    void RegisterShopPrefab(int itemId, int baseItemId, Func<UnityEngine.GameObject> resolver);
    bool TryResolveShopPrefab(int itemId, out UnityEngine.GameObject prefab);

    // --- Mod save sidecars (persisted next to the save) ---
    void RegisterSaveSidecar(string modId, Func<string> save, Action<string> load);

    // --- C# script HotLoad (main menu only; true = applied now) ---
    bool TryReloadScriptsNow();
}
