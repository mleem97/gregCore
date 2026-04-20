using System;
using gregCore.Sdk.Models;

namespace gregCore.Sdk;

/// <summary>
/// Das öffentliche Interface für alle Mod-Entwickler (SDK Layer).
/// Stellt eine stabile, versionierte API bereit.
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
}
