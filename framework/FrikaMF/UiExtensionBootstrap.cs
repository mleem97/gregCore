using MelonLoader;

namespace DataCenterModLoader;

/// <summary>
/// Registers built-in UI extension handlers (web replacement, modernizer, mod settings bridge)
/// inside the framework assembly so gameplay does not depend on optional FFM.Plugin.* shims.
/// </summary>
public static class UiExtensionBootstrap
{
    private static bool _registered;

    /// <summary>
    /// Wires <see cref="DC2WebBridge"/>, <see cref="UiModernizer"/>, and <see cref="ModSettingsMenuBridge"/>
    /// into <see cref="UiExtensionBridge"/>. Safe to call once per process; subsequent calls are ignored.
    /// </summary>
    public static void RegisterBuiltInHandlers()
    {
        if (_registered)
            return;

        _registered = true;

        UiExtensionBridge.RegisterWebReplacement((root, screenKey) => DC2WebBridge.TryApplyOrReplace(root, screenKey));
        UiExtensionBridge.RegisterWebConfiguration(
            getEnabled: () => DC2WebBridge.Enabled,
            setEnabled: enabled => DC2WebBridge.Enabled = enabled,
            setProfileReplaceMode: (profileKey, replace) => DC2WebBridge.SetProfileReplaceMode(profileKey, replace),
            resetAppliedState: root => DC2WebBridge.ResetAppliedState(root));

        UiExtensionBridge.RegisterUiHandlers(
            tryModernize: (root, sourceTag) => UiModernizer.TryModernize(root, sourceTag),
            onSceneLoaded: sceneName => ModSettingsMenuBridge.OnSceneLoaded(sceneName),
            drawGui: ModSettingsMenuBridge.DrawGUI,
            onSettingsOpened: mainMenu => ModSettingsMenuBridge.OnSettingsOpened(mainMenu));

        MelonLogger.Msg("FrikaMF: built-in UI extension handlers registered (Web UI + settings/modernizer UI; not multiplayer).");
    }

    /// <summary>
    /// Clears UI extension registrations. Called from <see cref="Core"/> on shutdown.
    /// </summary>
    public static void UnregisterBuiltInHandlers()
    {
        if (!_registered)
            return;

        UiExtensionBridge.Unregister();
        _registered = false;
    }
}
