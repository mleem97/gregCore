using System;

namespace AssetExporter
{
    public sealed class ExportStartedEvent : IModEvent
    {
        public ExportStartedEvent(DateTime occurredAtUtc, string exportPath)
        {
            OccurredAtUtc = occurredAtUtc;
            ExportPath = exportPath;
        }

        public DateTime OccurredAtUtc { get; }
        public string ExportPath { get; }
    }

    public sealed class ExportCompletedEvent : IModEvent
    {
        public ExportCompletedEvent(DateTime occurredAtUtc, string exportPath, int objectsScanned)
        {
            OccurredAtUtc = occurredAtUtc;
            ExportPath = exportPath;
            ObjectsScanned = objectsScanned;
        }

        public DateTime OccurredAtUtc { get; }
        public string ExportPath { get; }
        public int ObjectsScanned { get; }
    }
}
