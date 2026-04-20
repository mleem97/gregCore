namespace greg.Diagnostic;

public sealed class TelemetryConfig
{
    public bool Enabled { get; set; } = true;
    public int ExportIntervalSeconds { get; set; } = 30;
    public bool ArchiveSnapshots { get; set; } = false;
    public bool LogToConsole { get; set; } = true;
    public bool TrackHookEvents { get; set; } = true;
}

public sealed class TelemetrySnapshot
{
    public System.DateTime Timestamp { get; set; }
    public float SessionSeconds { get; set; }
    public string GregCoreVersion { get; set; } = "";
    public string MelonLoaderVersion { get; set; } = "";

    public float FpsCurrent { get; set; }
    public float FpsAverage { get; set; }
    public float FpsMin { get; set; }
    public float FpsMax { get; set; }
    public int FrameSpikeCount { get; set; }
    public int TargetFps { get; set; }

    public float RamUsedMb { get; set; }
    public float UnityHeapMb { get; set; }
    public int SystemRamMb { get; set; }
    public int GpuMemoryMb { get; set; }

    public string GpuName { get; set; } = "";
    public string CpuName { get; set; } = "";
    public int CpuCores { get; set; }

    public string GameState { get; set; } = "";

    public int TotalEventsThisSession { get; set; }
    public System.Collections.Generic.Dictionary<string, int> EventCounts { get; set; } = new();

    public int ErrorCount { get; set; }
}