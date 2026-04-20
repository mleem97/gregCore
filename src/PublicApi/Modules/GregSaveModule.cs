namespace gregCore.PublicApi.Modules;

public sealed class GregSaveModule
{
    private readonly GregApiContext _ctx;
    internal GregSaveModule(GregApiContext ctx) => _ctx = ctx;

    public void TriggerSave() => global::Il2Cpp.SaveSystem.SaveGame();
    public void Set<T>(string key, T value) where T : notnull => _ctx.Persist.Set(key, value);
    public T Get<T>(string key, T defaultValue = default!) where T : notnull => _ctx.Persist.Get(key, defaultValue);
    public bool Has(string key) => _ctx.Persist.Has(key);
    public void Delete(string key) => _ctx.Persist.Delete(key);
}