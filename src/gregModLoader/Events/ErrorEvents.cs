using System;

namespace gregCoreSDK.Exporter
{
    public sealed class ModErrorEvent : iModEvent
    {
        public ModErrorEvent(DateTime occurredAtUtc, string context, string message)
        {
            OccurredAtUtc = occurredAtUtc;
            Context = context;
            Message = message;
        }

        public DateTime OccurredAtUtc { get; }
        public string Context { get; }
        public string Message { get; }
    }
}

