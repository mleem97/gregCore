using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MelonLoader;
using UnityEngine;

namespace greg.Diagnostic;

public sealed class GregTelemetryService
{
    public static GregTelemetryService Instance { get; private set; } = null!;

    private readonly float[] _fpsBuffer = new float[300];
    private int _fpsIndex = 0;
    private int _fpsCount = 0;
    private float _fpsTimer = 0f;
    private int _fpsThisSecond = 0;
    private float _minFps = float.MaxValue;
    private float _maxFps = float.MinValue;
    private int _spikeCount = 0;

    private readonly DateTime _sessionStart = DateTime.UtcNow;
    private int _errorCount = 0;

    private TelemetryConfig? _cfg;
    private string _exportPath = "";
    private float _exportTimer = 0f;

    public void Initialize(TelemetryConfig cfg, string gameDataPath)
    {
        Instance = this;
        _cfg = cfg;

        if (cfg == null || !cfg.Enabled)
        {
            MelonLogger.Msg("[Telemetry] Disabled via config.");
            return;
        }

        _exportPath = Path.Combine(gameDataPath, "gregCore_telemetry");
        try
        {
            Directory.CreateDirectory(_exportPath);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Telemetry] Could not create export path: {ex.Message}");
            return;
        }

        MelonLogger.Msg($"[Telemetry] Initialized. Export every {cfg.ExportIntervalSeconds}s → {_exportPath}");
    }

    public void Tick()
    {
        if (_cfg == null || !_cfg.Enabled) return;

        float dt = Time.unscaledDeltaTime;
        float fps = dt > 0 ? 1f / dt : 0f;

        _fpsBuffer[_fpsIndex % _fpsBuffer.Length] = fps;
        _fpsIndex++;
        _fpsCount = Math.Min(_fpsCount + 1, _fpsBuffer.Length);

        if (fps < _minFps) _minFps = fps;
        if (fps > _maxFps) _maxFps = fps;
        if (dt > 0.033f) _spikeCount++;

        _fpsTimer += dt;
        _fpsThisSecond++;
        if (_fpsTimer >= 1f)
        {
            _fpsTimer = 0f;
            _fpsThisSecond = 0;
        }

        _exportTimer += dt;
        if (_exportTimer >= _cfg.ExportIntervalSeconds)
        {
            _exportTimer = 0f;
            Export();
        }
    }

    public void IncrementErrorCount() => _errorCount++;

    TelemetrySnapshot BuildSnapshot()
    {
        float avgFps = 0f;
        for (int i = 0; i < _fpsCount; i++) avgFps += _fpsBuffer[i];
        if (_fpsCount > 0) avgFps /= _fpsCount;

        return new TelemetrySnapshot
        {
            Timestamp = DateTime.UtcNow,
            SessionSeconds = (float)(DateTime.UtcNow - _sessionStart).TotalSeconds,
            GregCoreVersion = greg.Core.gregReleaseVersion.Current,
            MelonLoaderVersion = MelonLoader.Properties.BuildInfo.Version,

            FpsCurrent = _fpsBuffer[(_fpsIndex - 1 + _fpsBuffer.Length) % _fpsBuffer.Length],
            FpsAverage = MathF.Round(avgFps, 1),
            FpsMin = _minFps < float.MaxValue ? _minFps : 0f,
            FpsMax = _maxFps > float.MinValue ? _maxFps : 0f,
            FrameSpikeCount = _spikeCount,
            TargetFps = Application.targetFrameRate,

            RamUsedMb = MathF.Round(GC.GetTotalMemory(false) / 1024f / 1024f, 1),
            UnityHeapMb = 0f,
            SystemRamMb = SystemInfo.systemMemorySize,
            GpuMemoryMb = SystemInfo.graphicsMemorySize,

            GpuName = SystemInfo.graphicsDeviceName,
            CpuName = SystemInfo.processorType,
            CpuCores = SystemInfo.processorCount,

            GameState = GregFrameLimiterService.Instance?.CurrentStateName ?? "unknown",

            ErrorCount = _errorCount,
        };
    }

    void Export()
    {
        if (string.IsNullOrEmpty(_exportPath)) return;

        try
        {
            var snapshot = BuildSnapshot();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() },
            };

            string json = JsonSerializer.Serialize(snapshot, options);
            string latest = Path.Combine(_exportPath, "latest.json");

            File.WriteAllText(latest, json);

            if (_cfg?.ArchiveSnapshots == true)
            {
                string timestamped = Path.Combine(_exportPath, $"telemetry_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
                File.WriteAllText(timestamped, json);
            }

            if (_cfg?.LogToConsole == true)
            {
                MelonLogger.Msg($"[Telemetry] FPS={snapshot.FpsCurrent:F0} avg={snapshot.FpsAverage} RAM={snapshot.RamUsedMb}MB GPU={snapshot.GpuMemoryMb}MB");
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Telemetry] Export failed: {ex.Message}");
        }
    }
}