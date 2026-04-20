namespace gregCore.PublicApi.Modules;

public sealed class GregTimeModule
{
    private readonly GregApiContext _ctx;
    internal GregTimeModule(GregApiContext ctx) => _ctx = ctx;

    public float GetTimeOfDay() => global::Il2Cpp.TimeController.instance?.currentTimeOfDay ?? 0f;
    public int GetDay() => global::Il2Cpp.TimeController.instance?.day ?? 1;
    public float GetSecondsInFullDay() => global::Il2Cpp.TimeController.instance?.secondsInFullDay ?? 1200f;
    public void SetSecondsInFullDay(float s) {
        if (global::Il2Cpp.TimeController.instance != null) global::Il2Cpp.TimeController.instance.secondsInFullDay = s;
    }
    public bool IsPaused() => global::UnityEngine.Time.timeScale == 0;
    public void SetPaused(bool paused) => global::UnityEngine.Time.timeScale = paused ? 0 : 1;
    public float GetTimeScale() => global::UnityEngine.Time.timeScale;
    public void SetTimeScale(float scale) => global::UnityEngine.Time.timeScale = scale;
}
