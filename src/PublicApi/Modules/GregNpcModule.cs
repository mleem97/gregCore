namespace gregCore.PublicApi.Modules;

public sealed class GregNpcModule
{
    private readonly GregApiContext _ctx;
    internal GregNpcModule(GregApiContext ctx) => _ctx = ctx;
    public bool HireEmployee(string techId) => true;
}