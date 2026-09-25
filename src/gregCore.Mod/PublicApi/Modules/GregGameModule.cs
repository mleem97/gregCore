using gregCore.Core.Abstractions;
using gregCore.GameApi;

namespace gregCore.PublicApi.Modules;

/// <summary>
/// Modder facade over all generated game-type modules (greg.Game).
/// </summary>
public sealed class GregGameModule
{
    private readonly GregApiContext _ctx;
    internal GregGameModule(GregApiContext ctx) => _ctx = ctx;

    public IReadOnlyList<GameApiModuleDescriptor> Modules => GregGameApiRegistry.Modules;

    public GameApiModuleDescriptor? FindModule(string gameTypeName) =>
        GregGameApiRegistry.TryGetModule(gameTypeName);

    public T? FindFirst<T>() where T : UnityEngine.Object => GregGameModuleHost.FindFirst<T>();

    public bool Hook(string gameTypeName, string methodName)
    {
        try
        {
            var module = GregGameApiRegistry.TryGetModule(gameTypeName);
            if (module == null) return false;
            var type = Type.GetType(module.GameTypeName);
            if (type == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(module.GameTypeName, throwOnError: false, ignoreCase: false);
                    if (type != null) break;
                }
            }
            if (type == null) return false;
            return GregGameModuleHost.HookMethod(
                GregGameModuleHost.Harmony, type, methodName,
                _ctx.EventBus as Core.Events.GregEventBus, _ctx.Logger);
        }
        catch { return false; }
    }

    public GameModLoaderSnapshotStub GameLoaderSnapshot()
    {
        try
        {
            var loader = global::Il2Cpp.ModLoader.instance;
            if (loader == null || loader.Pointer == System.IntPtr.Zero)
                return new GameModLoaderSnapshotStub { IsAvailable = false };
            return new GameModLoaderSnapshotStub
            {
                IsAvailable = true,
                LoadedPluginCount = loader.loadedPlugins?.Count ?? 0,
                NextModId = loader.nextModID
            };
        }
        catch { return new GameModLoaderSnapshotStub { IsAvailable = false }; }
    }

    public sealed record GameModLoaderSnapshotStub
    {
        public bool IsAvailable { get; init; }
        public int LoadedPluginCount { get; init; }
        public int NextModId { get; init; }
    }
}
