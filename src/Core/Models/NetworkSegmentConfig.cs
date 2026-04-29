namespace gregCore.Core.Models;

public sealed record NetworkSegmentConfig
{
    public string[] Devices { get; init; } = Array.Empty<string>();
    public string? Switch { get; init; }
    public int VlanId { get; init; }
    public string? SubnetBase { get; init; }
    public string? Gateway { get; init; }
    public string? DnsServer { get; init; }
}
