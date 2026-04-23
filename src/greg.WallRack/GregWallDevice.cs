using System;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes;

namespace greg.WallRack
{
    public class GregWallDevice
    {
        public string deviceId = string.Empty;
        public string persistentId = string.Empty;
        public GregWallSlotType deviceType;
        public Vector2Int mountedAt;
        public string wallId = string.Empty;
        public GameObject? unityGameObject;
        public Il2CppObjectBase? vanillaRef;
        public string? customerId;
        public string deviceLabel = string.Empty;
        public bool isCustomerOwned;

        public void MountTo(GregWallSlot slot, Vector3 worldPos, Quaternion rotation)
        {
            if (slot == null || slot.isOccupied) return;
            
            mountedAt = slot.coord;
            slot.isOccupied = true;
            slot.mountedDevice = this;

            if (unityGameObject != null)
            {
                unityGameObject.transform.position = worldPos;
                unityGameObject.transform.rotation = rotation;
                unityGameObject.SetActive(true);
            }
        }

        public void Unmount()
        {
            if (unityGameObject != null)
            {
                unityGameObject.SetActive(false);
            }
        }

        public void Swap(GregWallDevice newDevice)
        {
            // Internal logic for swapping
            Unmount();
            // The slot logic will remount the newDevice
        }

        public void Highlight(bool active, Color color)
        {
            if (unityGameObject == null) return;
            // Outline effect or material swap
        }

        public void SetLabel(string label)
        {
            deviceLabel = label;
        }
    }
}
