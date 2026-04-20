namespace gregCore.PublicApi.Modules;

public sealed class GregPlayerModule
{
    private readonly GregApiContext _ctx;
    internal GregPlayerModule(GregApiContext ctx) => _ctx = ctx;

    private global::Il2Cpp.Player? Player => global::Il2Cpp.PlayerManager.instance?.playerClass;

    public float GetReputation() => Player?.reputation ?? 0f;
    public string GetPlayerName() => "Player"; // No playerName field in Player.cs dump
}