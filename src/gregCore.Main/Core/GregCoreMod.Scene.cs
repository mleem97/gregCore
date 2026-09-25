using System;
using System.Collections.Generic;
using MelonLoader;
using gregCore.UI;
using gregCore.Core.Events;
using gregCore.GameLayer.Bootstrap;
using gregCore.Infrastructure.Plugins;

namespace gregCore.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1118:Utility classes should not have public constructors", Justification = "Partial declaration holding only scene helpers; the class extends MelonMod and is instantiated by MelonLoader (instance members live in GregCoreMod.cs).")]
public sealed partial class GregCoreMod
{
    // Runs best-effort guards once the scene is stable.
    private static void NotifySceneGuards()
    {
        try
        {
            DisableIncompatibleIds();
            NotifyNetSession();
            LoadSaveSidecars();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Compatibility guard (second run: catches late-loading incompatible ID mods). /* ignored: defensive best-effort (CONVENTIONS.md) */
    private static void DisableIncompatibleIds()
    {
        try { gregCore.GameLayer.Patches.Hardware.IncompatibleModGuard.DisableIncompatibleIdMods(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // Network session: scene is stable, co-op lookups are safe from here on.
    private static void NotifyNetSession()
    {
        try { gregCore.Infrastructure.Networking.GregNetSession.NotifySceneLoaded(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // Loads the mods save sidecars for the current save game.
    private static void LoadSaveSidecars()
    {
        try { gregCore.Infrastructure.Persistence.GregSaveGuard.LoadSidecarsForCurrentSave(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // Updates Discord presence and hides vanilla UI outside the main menu.
    private static void UpdateScenePresence(string sceneName)
    {
        try
        {
            if (IsMainMenu(sceneName))
            {
                Infrastructure.Social.DiscordService.UpdatePresence("Planning Next Build", "Main Menu");
                return;
            }
            HideVanillaUi();
            Infrastructure.Social.DiscordService.UpdatePresence("Managing Infrastructure", $"Scene: {sceneName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Scene presence failed: {ex.Message}");
        }
    }

    // Checks whether the loaded scene is the main menu.
    private static bool IsMainMenu(string sceneName)
    {
        try
        {
            return string.Equals(sceneName, "MainMenu", StringComparison.Ordinal);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Hides vanilla UI when entering gameplay scenes.
    private static void HideVanillaUi()
    {
        try { GregUIOverrideManager.HideVanillaUI(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // Notifies the plugin registry about the scene change.
    private static void NotifyPluginSceneLoaded(string sceneName)
    {
        try
        {
            (GregServiceContainer.Get<IGregPluginRegistry>() as GregPluginRegistry)?.SceneLoaded(sceneName);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Publishes the sceneLoaded payload on both buses plus the legacy bridge.
    private static void PublishSceneLoaded(int buildIndex, string sceneName)
    {
        try
        {
            var scenePayload = BuildScenePayload(buildIndex, sceneName);
            DispatchScenePayload(scenePayload);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Builds the event payload for a scene change.
    private static gregCore.Core.Models.EventPayload BuildScenePayload(int buildIndex, string sceneName)
    {
        try
        {
            return new gregCore.Core.Models.EventPayload
            {
                HookName = "gregMod.lifecycle.sceneLoaded",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "buildIndex", buildIndex },
                    { "sceneName", sceneName }
                }
            };
        }
        catch /* ignored: defensive best-effort (CONVENTIONS.md) */
        {
            return new gregCore.Core.Models.EventPayload
            {
                HookName = "gregMod.lifecycle.sceneLoaded",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>()
            };
        }
    }

    // Dispatches the scene payload on current and legacy channels.
    private static void DispatchScenePayload(gregCore.Core.Models.EventPayload scenePayload)
    {
        try
        {
            HookBus?.Dispatch("gregMod.lifecycle.sceneLoaded", scenePayload);
            EventBus?.Publish("gregMod.lifecycle.sceneLoaded", scenePayload);
            DispatchLegacyScenePayload(scenePayload);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Dispatches the deprecated compatibility bridge payload.
    private static void DispatchLegacyScenePayload(gregCore.Core.Models.EventPayload scenePayload)
    {
        try
        {
            HookBus?.Dispatch("greg.lifecycle.SceneLoaded", scenePayload);
            EventBus?.Publish("greg.lifecycle.SceneLoaded", scenePayload);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }
}
