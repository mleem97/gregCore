namespace gregCore.Core.Models;

public sealed record RackBuildConfig
{
    public int RackId { get; init; }
    public RackSlotConfig[] Servers { get; init; } = Array.Empty<RackSlotConfig>();
    public string? SwitchId { get; init; }
    public string? PduId { get; init; }
    public bool AutoPowerOn { get; init; } = true;
}

public sealed record RackSlotConfig
{
    public string ServerId { get; init; } = string.Empty;
    public int Slot { get; init; }
    public string? IpAddress { get; init; }
}