using System;
using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;

using NetworkSwitch = Il2Cpp.NetworkSwitch;

namespace greg.Sdk.Services
{
    public class SwitchInfo
    {
        public string SwitchId;
        public string Label;
        public string RackId;
        public string SlotId;
        public bool IsOn;
        public bool IsBroken;
        public int EolTime;
        public int RackPositionUID;
        public NetworkSwitch Instance;
    }

    public static class GregSwitchDiscoveryService
    {
        public static List<SwitchInfo> ScanAll()
        {
            var result = new List<SwitchInfo>();
            var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>();
            foreach (var sw in switches)
            {
                result.Add(new SwitchInfo
                {
                    SwitchId = sw.switchId,
                    Label = sw.label,
                    IsOn = sw.isOn,
                    IsBroken = sw.isBroken,
                    EolTime = sw.eolTime,
                    Instance = sw
                });
            }
            return result;
        }

        public static SwitchInfo GetById(string switchId)
        {
            var all = ScanAll();
            foreach (var s in all)
            {
                if (s.SwitchId == switchId) return s;
            }
            return null;
        }

        public static List<SwitchInfo> GetByRack(string rackId)
        {
            return new List<SwitchInfo>(); // Placeholder
        }
    }
}
