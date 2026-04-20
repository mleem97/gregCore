using System;
using gregCore.Sdk.Models;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Infrastructure.Settings;
using gregCore.Infrastructure.Settings.Services;
using gregCore.Infrastructure.Plugins;
using gregCore.GameLayer.Bootstrap;

namespace gregCore.Sdk;

/// <summary>
/// Die zentrale Implementierung der öffentlichen SDK-API (SDK Layer).
/// Dient als stabiler Brückenkopf zwischen Mods und den internen Framework-Services.
/// </summary>
public sealed class GregAPI : IGregAPI
{
    private readonly IGregLogger _logger;
    private readonly GregHookBus _hookBus;
    private readonly GregModSettingsService _settingsService;
    private readonly GregKeybindRegistry _keybindRegistry;
    private readonly GregPluginRegistry _pluginRegistry;
    private readonly GregNotificationService _notificationService;
    private readonly Core.Services.GregValidationService _validationService;

    public string Version => "1.1.0";

    public GregAPI(
        IGregLogger logger,
        GregHookBus hookBus,
        GregModSettingsService settingsService,
        GregKeybindRegistry keybindRegistry,
        IGregPluginRegistry pluginRegistry,
        GregNotificationService notificationService,
        Core.Services.GregValidationService validationService)
    {
        _logger = logger.ForContext("SDK_API");
        _hookBus = hookBus;
        _settingsService = settingsService;
        _keybindRegistry = keybindRegistry;
        _pluginRegistry = (GregPluginRegistry)pluginRegistry;
        _notificationService = notificationService;
        _validationService = validationService;
    }

    // --- Hooks & Events ---
    public void On(string hookName, Action<GregPayload> handler)
    {
        if (!_validationService.ValidateHookName(hookName)) return;

        _hookBus.On(hookName, (payload) => {
            // Umwandlung in SDK-Payload für saubere Abstraktion
            var sdkPayload = new GregPayload(payload.HookName, payload.Trigger) {
                Data = payload.Data
            };
            handler(sdkPayload);
        });
    }

    public void Fire(string hookName, GregPayload payload)
    {
        var corePayload = new Core.Models.EventPayload {
            HookName = payload.HookName,
            Trigger = payload.Trigger,
            Data = payload.Data
        };
        _hookBus.Dispatch(hookName, corePayload);
    }

    // --- Mod Registration ---
    public void RegisterMod(string modId, string name, string version, object? apiObject = null)
    {
        if (!_validationService.ValidateModId(modId)) return;

        _pluginRegistry.RegisterMod(new ModMetadata {
            ModId = modId, Name = name, Version = version, ApiObject = apiObject
        });
    }

    // --- Settings & Input ---
    public void RegisterToggle(string modId, string settingId, string displayName, bool defaultValue, Action<bool>? onChanged = null, string category = "General", string description = "")
    {
        var entry = new Infrastructure.Settings.Models.SettingEntry<bool> {
            ModId = modId, SettingId = settingId, DisplayName = displayName, DefaultValue = defaultValue, OnValueChanged = onChanged, Category = category, Description = description
        };
        _settingsService.Register(entry);
    }

    public void RegisterSlider(string modId, string settingId, string displayName, float defaultValue, Action<float>? onChanged = null, string category = "General", string description = "")
    {
        var entry = new Infrastructure.Settings.Models.SettingEntry<float> {
            ModId = modId, SettingId = settingId, DisplayName = displayName, DefaultValue = defaultValue, OnValueChanged = onChanged, Category = category, Description = description
        };
        _settingsService.Register(entry);
    }

    public void RegisterKeybind(string modId, string actionId, string displayName, UnityEngine.KeyCode defaultKey, Action onPress, string category = "Controls", string description = "")
    {
        var entry = new Infrastructure.Settings.Models.KeybindEntry {
            ModId = modId, ActionId = actionId, DisplayName = displayName, DefaultKey = defaultKey, CurrentKey = defaultKey, Category = category, OnPress = onPress, Description = description
        };
        _keybindRegistry.Register(entry);
    }

    // --- Notifications ---
    public void ShowNotification(string title, string message, float duration = 5f)
    {
        _notificationService.Show(title, message, duration);
    }
}
