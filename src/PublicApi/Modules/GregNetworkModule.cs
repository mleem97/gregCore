namespace gregCore.PublicApi.Modules;

public sealed class GregNetworkModule
{
    private readonly GregApiContext _ctx;
    internal GregNetworkModule(GregApiContext ctx) => _ctx = ctx;

    public bool ConnectDevice(string sourceId, string targetId)
    {
        var map = global::Il2Cpp.NetworkMap.instance;
        if (map == null) return false;
        map.Connect(sourceId, targetId);
        return true;
    }

    public bool DisconnectDevice(string sourceId, string targetId)
    {
        var map = global::Il2Cpp.NetworkMap.instance;
        if (map == null) return false;
        map.Disconnect(sourceId, targetId);
        return true;
    }

    public int GetFreeVlanId() => global::Il2Cpp.MainGameManager.instance?.GetFreeVlanId() ?? -1;
    
    public bool IsIpDuplicate(string ip) 
        => global::Il2Cpp.NetworkMap.instance?.IsIpAddressDuplicate(ip, null!) ?? false;

    public event Action<string, string>? OnDeviceConnected
    {
        add => _ctx.EventBus.Subscribe("greg.networking.Connect", p => value?.Invoke((string)p.Data["source"], (string)p.Data["target"]));
        remove => _ctx.EventBus.Unsubscribe("greg.networking.Connect", p => value?.Invoke((string)p.Data["source"], (string)p.Data["target"]));
    }
}