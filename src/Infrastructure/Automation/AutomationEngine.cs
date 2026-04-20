using System.Collections;
using gregCore.PublicApi;

namespace gregCore.Infrastructure.Automation;

internal sealed class AutomationEngine
{
    public DeliveryZoneEngine Delivery { get; }
    public CableLayingEngine Cabling { get; }
    public RackBuildEngine RackBuild { get; }
    public NetworkConfigEngine Network { get; }
    public RepairEngine Repair { get; }

    internal AutomationEngine(GregApiContext ctx)
    {
        Delivery = new DeliveryZoneEngine(ctx.Logger, ctx.EventBus);
        Cabling = new CableLayingEngine(ctx.Logger, ctx.EventBus);
        RackBuild = new RackBuildEngine(ctx.Logger, Cabling);
        Network = new NetworkConfigEngine(ctx.Logger);
        Repair = new RepairEngine(ctx.Logger);
    }
}