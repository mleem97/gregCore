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

        gregUiExtensionBridge.RegisterWebReplacement((root, screenKey) => DC2WebBridge.TryApplyOrReplace(root, screenKey));
        gregUiExtensionBridge.RegisterWebConfiguration(
            getEnabled: () => DC2WebBridge.Enabled,
            setEnabled: enabled => DC2WebBridge.Enabled = enabled,
            setProfileReplaceMode: (profileKey, replace) => DC2WebBridge.SetProfileReplaceMode(profileKey, replace),
            resetAppliedState: root => DC2WebBridge.ResetAppliedState(root));

        gregUiExtensionBridge.RegisterUiHandlers(
            tryModernize: (root, sourceTag) => gregUiModernizer.TryModernize(root, sourceTag),
            onSceneLoaded: sceneName => ModSettingsMenuBridge.OnSceneLoaded(sceneName),
            drawGui: ModSettingsMenuBridge.DrawGUI,
            onSettingsOpened: mainMenu => ModSettingsMenuBridge.OnSettingsOpened(mainMenu));

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



