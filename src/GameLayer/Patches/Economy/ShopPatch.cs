using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Economy;

internal static class ShopPatch
{
    internal static void OnCheckOut(object __instance)
    {
        try
        {
            var payload = EventPayloadBuilder.ForGeneric("greg.SYSTEM.ButtonCheckOut", new Dictionary<string, object>
            {
                { "Timestamp", DateTime.UtcNow }
            });
            HookIntegration.Emit(HookName.Create("system", "ButtonCheckOut").ToString(), payload);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnCheckOut), ex);
        }
    }
}

