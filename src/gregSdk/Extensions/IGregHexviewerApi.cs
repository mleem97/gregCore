using System;
using Il2Cpp;

namespace greg.Sdk.Extensions
{
    public interface IGregHexviewerApi
    {
        void InspectObject(Il2CppSystem.Object obj, string label);
        void InspectSaveSegment(string sectionName, string entityId);
        void InspectBytes(byte[] data, string label);
        void InspectParsed<T>(T obj, string label);
        bool IsOpen { get; }
        void Toggle();
    }
}

