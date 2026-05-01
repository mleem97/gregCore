using gregCore.API;
using gregCore.Bridge.CSharpScript;
using gregCore.Core.Models;

public class SampleMod : IGregCSharpMod
{
    public string ModId => "sample_csharp_mod";
    public string ModName => "Sample C# Mod";
    public string Version => "1.0.0";

    public void OnInit()
    {
        GregAPI.LogInfo("[SampleMod] Initialized!");

        GregAPI.EventBus?.Subscribe("OnCoinsChanged", (EventPayload payload) =>
        {
            double money = GregAPI.GetPlayerMoney();
            GregAPI.LogInfo($"[SampleMod] Coins changed! Current money: {money}");
        });
    }

    public void OnUpdate(float dt)
    {
        // Intentionally left empty to avoid log spam.
        // Use this for per-frame logic (e.g., input handling, state checks).
    }

    public void OnSceneLoaded(string sceneName)
    {
        GregAPI.LogInfo($"[SampleMod] Scene loaded: {sceneName}");
    }

    public void OnShutdown()
    {
        GregAPI.LogInfo("[SampleMod] Shutting down...");
    }
}
