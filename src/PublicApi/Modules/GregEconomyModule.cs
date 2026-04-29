using gregCore.PublicApi.Types;

namespace gregCore.PublicApi.Modules;

public sealed class GregEconomyModule
{
    private readonly GregApiContext _ctx;
    internal GregEconomyModule(GregApiContext ctx) => _ctx = ctx;

    private global::Il2Cpp.Player? Player => global::Il2Cpp.PlayerManager.instance?.playerClass;

    public float GetBalance() => Player?.money ?? 0f;
    public float GetXP() => Player?.xp ?? 0f;
    public float GetReputation() => Player?.reputation ?? 0f;

    public bool AddMoney(float amount)
    {
        var player = Player;
        if (player == null) return false;
        player.UpdateCoin(amount, false);
        return true;
    }

    public bool SetBalance(float amount)
    {
        var player = Player;
        if (player == null) return false;
        player.UpdateCoin(amount - player.money, false);
        return true;
    }

    public bool AddXP(float amount)
    {
        var player = Player;
        if (player == null) return false;
        player.UpdateXP(amount);
        return true;
    }

    public bool AddReputation(float amount)
    {
        var player = Player;
        if (player == null) return false;
        player.UpdateReputation(amount);
        return true;
    }

    public event Action<float>? OnMoneyChanged
    {
        add => _ctx.EventBus.Subscribe("greg.economy.PlayerCoinUpdated", p => value?.Invoke((float)p.Data["amount"]));
        remove => _ctx.EventBus.Unsubscribe("greg.economy.PlayerCoinUpdated", p => value?.Invoke((float)p.Data["amount"]));
    }
}
