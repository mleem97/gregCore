using System.Collections.Generic;

namespace greg.Sdk.Events;

// ═══ Server Events ═══
public class ServerOnlineEvent   { public string ServerId; public int CustomerId; public string Ip; }
public class ServerOfflineEvent  { public string ServerId; }

// ═══ IP Events ═══
public class IpChangedEvent      { public string ServerId; public string OldIp; public string NewIp; }
public class IpConflictEvent     { public string Ip; public List<string> AffectedServerIds; }

// ═══ VLAN Events ═══
public class VlanChangedEvent    { public string SwitchId; public int PortIndex; public int VlanId; public string Action; }

// ═══ Scan Events ═══
public class ScanCompletedEvent  { public int ServerCount; public int SwitchCount; }

// ═══ Lifecycle Events ═══
public class EolWarningEvent     { public string EntityId; public string EntityType; public int DaysRemaining; }

// ═══ SLA Events ═══
public class SlaBreachEvent      { public int CustomerId; public int AppId; public float Required; public float Actual; }

// ═══ Alert Events ═══
public class AlertTriggeredEvent { public string AlertId; public string Severity; public string Message; public string EntityId; }

// ═══ Topology Events ═══
public class TopologyChangedEvent { public List<string> AffectedNodes; }

// ═══ Reset Events ═══
public class ResetEvent          { public string SwitchId; public string ResetType; }

