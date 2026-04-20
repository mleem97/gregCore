namespace gregCore.PublicApi.Modules;

public sealed class GregUIModule
{
    private readonly GregApiContext _ctx;
    internal GregUIModule(GregApiContext ctx) => _ctx = ctx;

    public void ShowToast(string message, float durationSeconds = 3f)
        => _ctx.EventBus.Publish("greg.ui.ShowToast", new EventPayload { 
            OccurredAtUtc = DateTime.UtcNow, 
            Data = new Dictionary<string, object> { ["message"] = message, ["duration"] = durationSeconds } 
        });

    public void ShowError(string message) => ShowToast($"⚠ {message}", 5f);
}