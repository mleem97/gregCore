namespace gregCore.PublicApi.Modules;

public sealed class GregServerModule
{
    private readonly GregApiContext _ctx;
    internal GregServerModule(GregApiContext ctx) => _ctx = ctx;

    public bool Spawn(string serverId, int rackId) => true; // API Logic
}