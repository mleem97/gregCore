using System;

namespace gregAssetExporter
{
    public sealed class ToggleChangedEvent : iModEvent
    {
        public ToggleChangedEvent(DateTime occurredAtUtc, string toggleName, bool enabled)
        {
            OccurredAtUtc = occurredAtUtc;
            ToggleName = toggleName;
            Enabled = enabled;
        }

        public DateTime OccurredAtUtc { get; }
        public string ToggleName { get; }
        public bool Enabled { get; }
    }
}

