using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregPlayerModule
{
    private readonly GregApiContext _ctx;
    internal GregPlayerModule(GregApiContext ctx) => _ctx = ctx;

    private global::Il2Cpp.Player? Player => global::Il2Cpp.PlayerManager.instance?.playerClass;

    public float GetReputation() => Player?.reputation ?? 0f;
    
    public Vector3 GetPosition() => Player?.transform.position ?? Vector3.zero;
    public Vector3 GetRotation() => Player?.transform.eulerAngles ?? Vector3.zero;
}
