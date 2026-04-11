using System;

namespace gregSdk;

/// <summary>
/// Mandatory payload fields for normalized Unity signal conversion.
/// </summary>
public struct GregSignalPayload
{
    public string SourceAsm { get; set; }
    public string SourceType { get; set; }
    public string SourceMethod { get; set; }
    public string EntityId { get; set; }
    public string EventKind { get; set; }
    public DateTime TimestampUtc { get; set; }
    
    // Optional domain fields
    public string CustomerId { get; set; }
    public string ServerId { get; set; }
    public string RackId { get; set; }
    public string SwitchId { get; set; }
    public string CableId { get; set; }
    public float PlayerValue { get; set; }
    public string Reason { get; set; }
    public string State { get; set; }
    public float Delta { get; set; }
    public string RawArgsJson { get; set; }
}
