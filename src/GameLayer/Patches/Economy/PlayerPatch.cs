/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Extrahiert Daten aus dem IL2CPP Player-Objekt.
/// Maintainer:   EINZIGE Verantwortung: Daten extrahieren + dispatchen. Kein Business-Logic.
/// </file-summary>

using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Economy;

// [GREG_SYNC_INSERT_PATCHES]

internal static class PlayerPatch
{
    internal static void OnCoinUpdated(object __instance, float _coinChhangeAmount)
    {
        try
        {
            var payload = EventPayloadBuilder.ForValueChange("money", 0f, _coinChhangeAmount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerCoinUpdated"), payload);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnCoinUpdated), ex);
        }
    }
}
