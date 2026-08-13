/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Basis-Klasse für alle gregCore-Mods.
/// Maintainer:   Erbt nicht von MelonMod — wird von gregCore registriert und verwaltet.
/// </file-summary>

namespace gregCore.PublicApi;

public abstract class GregMod
{
    protected IGregLogger Logger { get; private set; } = null!;
    protected IGregEventBus EventBus { get; private set; } = null!;
    protected GregApiContext Api { get; private set; } = null!;
    private readonly List<IDisposable> _subscriptions = new();
    protected CancellationToken CancellationToken => Api.CancellationToken;
    protected IGregMainThreadDispatcher MainThread => Api.MainThread;
    protected GregResourceRegistry Resources => Api.Resources;

    public virtual void OnLoad() { }
    public virtual void OnReady() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnSceneLoaded(string sceneName) { }
    public virtual void OnUnload() { }
    public virtual void OnShutdown() => OnUnload();

    internal void Initialize(GregApiContext context)
    {
        Api = new GregApiContext
        {
            Logger = context.Logger,
            EventBus = context.EventBus,
            HookBus = context.HookBus,
            Config = context.Config,
            Persist = context.Persist,
            CancellationToken = context.CancellationToken,
            Events = context.Events,
            MainThread = context.MainThread,
            Resources = new GregResourceRegistry(),
            LifetimeSource = context.LifetimeSource
        };
        Logger = context.Logger.ForContext(GetType().Name);
        EventBus = context.EventBus;
    }

    protected IDisposable On(string hookName, Action<EventPayload> handler)
    {
        var subscription = Api.Events.On(hookName, handler);
        _subscriptions.Add(subscription);
        return subscription;
    }

    internal void DisposeSubscriptions()
    {
        foreach (var subscription in _subscriptions.ToArray()) subscription.Dispose();
        _subscriptions.Clear();
    }

    internal void DisposeResources() => Api.Resources?.Dispose();
}
