namespace gregCore.PublicApi.Modules;

public sealed class GregTimeModule
{
    private readonly GregApiContext _ctx;
    internal GregTimeModule(GregApiContext ctx) => _ctx = ctx;

    public event Action? OnDayEnd
    {
        add => _ctx.EventBus.Subscribe("greg.lifecycle.OnEndOfTheDay", _ => value?.Invoke());
        remove => _ctx.EventBus.Unsubscribe("greg.lifecycle.OnEndOfTheDay", _ => value?.Invoke());
    }

    public event Action<int, string>? OnSceneLoaded
    {
        add => _ctx.EventBus.Subscribe("greg.lifecycle.SceneLoaded", p => value?.Invoke((int)p.Data["buildIndex"], (string)p.Data["sceneName"]));
        remove => _ctx.EventBus.Unsubscribe("greg.lifecycle.SceneLoaded", p => value?.Invoke((int)p.Data["buildIndex"], (string)p.Data["sceneName"]));
    }

    public event Action? OnGameSaved
    {
        add => _ctx.EventBus.Subscribe("greg.persistence.SaveGame", _ => value?.Invoke());
        remove => _ctx.EventBus.Unsubscribe("greg.persistence.SaveGame", _ => value?.Invoke());
    }
}