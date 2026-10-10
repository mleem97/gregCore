using System;
using System.Collections.Generic;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.Infrastructure.Settings;
using gregCore.Infrastructure.Settings.Models;
using gregCore.Infrastructure.Plugins;
using gregCore.Infrastructure.Settings.Services;
using gregCore.Core.Services;
using gregCore.Sdk.Models;
using UnityEngine;

namespace gregCore.Sdk;

public class GregAPI : IGregAPI
{
    private readonly IGregLogger _logger;
    private readonly GregHookBus _hookBus;
    private readonly GregModSettingsService _settings;
    private readonly GregKeybindRegistry _keybinds;
    private readonly GregPluginRegistry _plugins;
    private readonly GregNotificationService _notifications;
    private readonly GregValidationService _validation;

    public string Version => "1.2.3";

    public GregAPI(
        IGregLogger logger,
        GregHookBus hookBus,
        GregModSettingsService settings,
        GregKeybindRegistry keybinds,
        GregPluginRegistry plugins,
        GregNotificationService notifications,
        GregValidationService validation)
    {
        _logger = logger;
        _hookBus = hookBus;
        _settings = settings;
        _keybinds = keybinds;
        _plugins = plugins;
        _notifications = notifications;
        _validation = validation;
    }

    public void On(string hookName, Action<GregPayload> handler)
    {
        if (string.IsNullOrEmpty(hookName) || handler == null) return;
        _hookBus.On(hookName, p =>
        {
            var data = new Dictionary<string, object>();
            if (p.Data != null)
            {
                foreach (var kv in p.Data)
                    data[kv.Key] = kv.Value;
            }
            handler(new GregPayload
            {
                HookName = p.HookName,
                Data = data
            });
        });
    }

    public void Fire(string hookName, GregPayload payload)
    {
        if (string.IsNullOrEmpty(hookName)) return;
        _hookBus.Dispatch(hookName, new EventPayload
        {
            HookName = payload.HookName,
            Data = payload.Data
        });
    }

    public void RegisterMod(string modId, string name, string version)
    {
        RegisterMod(modId, name, version, null);
    }

    public void RegisterMod(string modId, string name, string version, object? apiObject)
    {
        _plugins.RegisterMod(new ModMetadata
        {
            ModId = modId,
            Name = name,
            Version = version
        });
    }

    public void RegisterToggle(string modId, string settingId, string displayName, bool defaultValue)
    {
        RegisterToggle(modId, settingId, displayName, defaultValue, null, "General", "");
    }

    public void RegisterToggle(string modId, string settingId, string displayName, bool defaultValue, Action<bool>? onChanged, string category, string description)
    {
        _settings.Register(new SettingEntry<bool>
        {
            ModId = modId,
            SettingId = settingId,
            DisplayName = displayName,
            DefaultValue = defaultValue,
            Value = defaultValue,
            Category = category,
            Description = description,
            OnValueChanged = onChanged
        });
    }

    public void RegisterSlider(string modId, string settingId, string displayName, float defaultValue)
    {
        RegisterSlider(modId, settingId, displayName, defaultValue, null, "General", "");
    }

    public void RegisterSlider(string modId, string settingId, string displayName, float defaultValue, Action<float>? onChanged, string category, string description)
    {
        _settings.Register(new SettingEntry<float>
        {
            ModId = modId,
            SettingId = settingId,
            DisplayName = displayName,
            DefaultValue = defaultValue,
            Value = defaultValue,
            Category = category,
            Description = description,
            OnValueChanged = onChanged
        });
    }

    public void RegisterKeybind(string modId, string actionId, string displayName, KeyCode defaultKey, Action onPress)
    {
        RegisterKeybind(modId, actionId, displayName, defaultKey, onPress, "Controls", "");
    }

    public void RegisterKeybind(string modId, string actionId, string displayName, KeyCode defaultKey, Action onPress, string category, string description)
    {
        _keybinds.Register(new KeybindEntry
        {
            ModId = modId,
            ActionId = actionId,
            DisplayName = displayName,
            DefaultKey = defaultKey,
            OnPress = onPress,
            Category = category,
            Description = description
        });
    }

    public void ShowNotification(string title, string message)
    {
        ShowNotification(title, message, 5f);
    }

    public void ShowNotification(string title, string message, float duration)
    {
        _notifications.Show(title, message, duration);
    }

    public void Log(string message)
    {
        try { _logger.Info(message); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void Warn(string message)
    {
        try { _logger.Warning(message); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void Error(string message)
    {
        try { _logger.Error(message); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void Toast(string message)
    {
        Toast(message, 3f);
    }

    public void Toast(string message, float duration)
    {
        try { gregCore.UI.GregNotificationManager.Show(message, duration); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void ToastRich(string top, string title, string sub)
    {
        ToastRich(top, title, sub, 5f);
    }

    public void ToastRich(string top, string title, string sub, float duration)
    {
        try { gregCore.UI.GregNotificationManager.ShowRich(top, title, sub, null, null, duration); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void BindMenuToggle(string menuId, Action toggle, Func<bool> isOpen)
    {
        try { gregCore.UI.GregMenuBinding.BindToggle(menuId, toggle, isOpen); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void ReportMenu(string menuId, bool open)
    {
        try { gregCore.UI.GregMenuBinding.Report(menuId, open); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public void RegisterShopPrefab(int itemId, int baseItemId, Func<UnityEngine.GameObject> resolver)
    {
        try { gregCore.Core.Networking.GregShopItems.RegisterPrefab(itemId, baseItemId, resolver); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public bool TryResolveShopPrefab(int itemId, out UnityEngine.GameObject prefab)
    {
        prefab = null;
        try { return gregCore.Core.Networking.GregShopItems.TryResolvePrefab(itemId, out prefab); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    public void RegisterSaveSidecar(string modId, Func<string> save, Action<string> load)
    {
        try { gregCore.Infrastructure.Persistence.GregSaveGuard.RegisterSidecar(modId, save, load); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public bool TryReloadScriptsNow()
    {
        try { return gregCore.Bridge.CSharpScript.GregCSharpScriptBridge.TryReloadNow(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }
}
