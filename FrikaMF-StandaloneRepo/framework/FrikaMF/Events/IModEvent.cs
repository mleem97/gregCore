using System;

namespace AssetExporter
{
    public interface IModEvent
    {
        DateTime OccurredAtUtc { get; }
    }
}
