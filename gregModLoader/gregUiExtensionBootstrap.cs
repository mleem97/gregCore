using MelonLoader;

namespace gregModLoader;

/// <summary>
/// Registers built-in UI extension handlers (web replacement, modernizer, mod settings bridge)
/// inside the framework assembly so gameplay does not depend on optional greg.Plugin.* shims.
/// </summary>
public static class UiExtensionBootstrap
{
    private static bool _registered;

    /// <summary>
    /// Wires <see cref="DC2WebBridge"/>, <see cref="gregUiModernizer"/>, and <see cref="ModSettingsMenuBridge"/>
    /// into <see cref="gregUiExtensionBridge"/>. Safe to call once per process; subsequent calls are ignored.
    /// </summary>
    public static void RegisterBuiltInHandlers()
    {
        if (_registered)
            return;

        _registered = true;

        // Web replacement is disabled by default to prioritize native UI modernization.
        // It can still be registered by external plugins if needed.
        gregUiExtensionBridge.RegisterWebReplacement(null);
        
        gregUiExtensionBridge.RegisterWebConfiguration(
            getEnabled: () => false,
            setEnabled: enabled => { },
            setProfileReplaceMode: (profileKey, replace) => { },
            resetAppliedState: root => { });

        gregUiExtensionBridge.RegisterUiHandlers(
            tryModernize: (root, sourceTag) => gregUiModernizer.TryModernize(root, sourceTag),
            onSceneLoaded: sceneName => gregModSettingsMenuBridge.OnSceneLoaded(sceneName),
            drawGui: gregModSettingsMenuBridge.DrawGUI,
            onSettingsOpened: mainMenu => gregModSettingsMenuBridge.OnSettingsOpened(mainMenu));

        MelonLogger.Msg("gregCore: built-in UI extension handlers registered (full gregMain menu replace + settings/modernizer UI).");
    }

    /// <summary>
    /// Clears UI extension registrations. Called from <see cref="gregCoreLoader"/> on shutdown.
    /// </summary>
    public static void UnregisterBuiltInHandlers()
    {
        if (!_registered)
            return;

        gregUiExtensionBridge.Unregister();
        _registered = false;
    }
}



