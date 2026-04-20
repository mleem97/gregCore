namespace gregCore.PublicApi.Modules;

public sealed class GregSaveModule
{
    private readonly GregApiContext _ctx;
    internal GregSaveModule(GregApiContext ctx) => _ctx = ctx;

    public string GetCurrentScene() => global::UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    public void TriggerSave() => global::Il2Cpp.SaveSystem.SaveGameData();
    public int GetDifficulty() => 1; // Default fallback
    
    public void Set<T>(string key, T value) where T : notnull => _ctx.Persist.Set(key, value);
    public T Get<T>(string key, T defaultValue = default!) where T : notnull => _ctx.Persist.Get(key, defaultValue);
}
