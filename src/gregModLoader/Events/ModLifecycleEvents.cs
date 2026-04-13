using System;

namespace greg.Exporter
{
    public sealed class ModInitializedEvent : iModEvent
    {
        public ModInitializedEvent(DateTime occurredAtUtc, string version)
        {
            OccurredAtUtc = occurredAtUtc;
            Version = version;
        }

        public DateTime OccurredAtUtc { get; }
        public string Version { get; }
    }

    public sealed class ModTickEvent : iModEvent
    {
        public ModTickEvent(float deltaTime, int frame)
        {
            OccurredAtUtc = DateTime.UtcNow;
            DeltaTime = deltaTime;
            Frame = frame;
        }

        public DateTime OccurredAtUtc { get; }
        public float DeltaTime { get; }
        public int Frame { get; }
    }
}

