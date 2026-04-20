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

    public virtual void OnLoad() { }
    public virtual void OnReady() { }
    public virtual void OnUnload() { }

    internal void Initialize(GregApiContext context)
    {
        Api = context;
        Logger = context.Logger.ForContext(GetType().Name);
        EventBus = context.EventBus;
    }
}
