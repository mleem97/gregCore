using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.Core.Persistence;

namespace greg.Sdk.Services
{
    public static class GregResetSwitchScanItem
    {
        public static IEnumerable<GameObject> GetAll() => HardwareIDManager.GetAllHardware().Values;
    }

    public class ServerInfo
    {
        public string id = "";
        public string name = "";
    }

    public static class GregTaskQueueService
    {
        public static void Enqueue(object task) { }
    }

    public static class GregResetSwitchService
    {
        public static void ResetAll() { }
    }

    public static class GregServerDiscoveryService
    {
        public static List<ServerInfo> GetAllServers() => new();
    }
}

namespace greg.Core
{
    public static class CustomEmployeeManager
    {
        public static int Register(string id, string name, string desc, float salary, float rep, bool req) => 1;
    }

    public static class GregModRegistry
    {
        public static void Register(object mod) { }
    }
}

namespace greg.Core.UI.Components
{
    public class GregPanel : MonoBehaviour
    {
        public GregPanel(IntPtr ptr) : base(ptr) { }
        public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    }
}
