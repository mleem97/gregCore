using System;

namespace AssetExporter
{
    public sealed class ModInitializedEvent : IModEvent
    {
        public ModInitializedEvent(DateTime occurredAtUtc, string version)
        {
            OccurredAtUtc = occurredAtUtc;
            Version = version;
        }

        public DateTime OccurredAtUtc { get; }
        public string Version { get; }
    }

    public sealed class ModTickEvent : IModEvent
    {
        public ModTickEvent(DateTime occurredAtUtc)
        {
            OccurredAtUtc = occurredAtUtc;
        }

        public DateTime OccurredAtUtc { get; }
    }
}
