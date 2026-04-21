using System;
using System.Collections.Generic;
using LiteDB;
using greg.Logging;

namespace greg.WallRack.Integration
{
    public static class WallSaveIntegration
    {
        private static readonly GregModLogger _log = new GregModLogger("WallRack");

        public static void SaveWallRackState(GregWallRegistry registry, LiteDatabase db)
        {
            var col = db.GetCollection<WallRackStateDoc>("wallrack_state");
            col.DeleteAll();

            var wallsList = new List<WallDoc>();

            foreach (var grid in registry.GetAllGrids())
            {
                var mounted = new List<MountedDoc>();
                foreach (var slot in grid.slots.Values)
                {
                    if (slot.isOccupied && slot.mountedDevice != null)
                    {
                        var dev = slot.mountedDevice;
                        mounted.Add(new MountedDoc {
                            deviceId = dev.deviceId,
                            slotCoordX = dev.mountedAt.x,
                            slotCoordY = dev.mountedAt.y,
                            deviceType = dev.deviceType.ToString(),
                            customerId = dev.customerId,
                            deviceLabel = dev.deviceLabel,
                            isCustomerOwned = dev.isCustomerOwned,
                            mountedAt = DateTime.UtcNow
                        });
                    }
                }

                wallsList.Add(new WallDoc {
                    wallId = grid.wallId,
                    worldPosX = grid.wallOrigin.x,
                    worldPosY = grid.wallOrigin.y,
                    worldPosZ = grid.wallOrigin.z,
                    wallNormalX = grid.wallNormal.x,
                    wallNormalY = grid.wallNormal.y,
                    wallNormalZ = grid.wallNormal.z,
                    columns = grid.columns,
                    rows = grid.rows,
                    mountedDevices = mounted
                });
            }

            col.Insert(new WallRackStateDoc {
                sessionId = Guid.NewGuid().ToString(),
                savedAt = DateTime.UtcNow,
                walls = wallsList
            });

            _log.Saved(wallsList.Count, 0);
        }

        public static void LoadWallRackState(GregWallRegistry registry, LiteDatabase db)
        {
            var col = db.GetCollection<WallRackStateDoc>("wallrack_state");
            var doc = col.FindOne(Query.All());
            if (doc == null) return;

            foreach (var w in doc.walls)
            {
                var grid = new GregWallGrid();
                grid.Initialize(w.wallId, 
                    new UnityEngine.Vector3(w.worldPosX, w.worldPosY, w.worldPosZ),
                    new UnityEngine.Vector3(w.wallNormalX, w.wallNormalY, w.wallNormalZ),
                    UnityEngine.Vector3.up, w.columns, w.rows);

                foreach (var m in w.mountedDevices)
                {
                    var dev = new GregWallDevice {
                        deviceId = m.deviceId,
                        deviceLabel = m.deviceLabel,
                        customerId = m.customerId,
                        isCustomerOwned = m.isCustomerOwned,
                        deviceType = Enum.TryParse<GregWallSlotType>(m.deviceType, out var t) ? t : GregWallSlotType.Generic
                    };
                    grid.MountDevice(new UnityEngine.Vector2Int(m.slotCoordX, m.slotCoordY), dev);
                }

                registry.RegisterWall(w.wallId, grid);
            }

            _log.Loaded(doc.walls.Count, "wallrack_state");
        }
    }

    public class WallRackStateDoc
    {
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        public string sessionId { get; set; } = string.Empty;
        public DateTime savedAt { get; set; }
        public List<WallDoc> walls { get; set; } = new();
    }

    public class WallDoc
    {
        public string wallId { get; set; } = string.Empty;
        public float worldPosX { get; set; }
        public float worldPosY { get; set; }
        public float worldPosZ { get; set; }
        public float wallNormalX { get; set; }
        public float wallNormalY { get; set; }
        public float wallNormalZ { get; set; }
        public int columns { get; set; }
        public int rows { get; set; }
        public List<MountedDoc> mountedDevices { get; set; } = new();
    }

    public class MountedDoc
    {
        public string deviceId { get; set; } = string.Empty;
        public int slotCoordX { get; set; }
        public int slotCoordY { get; set; }
        public string deviceType { get; set; } = string.Empty;
        public string? customerId { get; set; }
        public string deviceLabel { get; set; } = string.Empty;
        public bool isCustomerOwned { get; set; }
        public DateTime mountedAt { get; set; }
    }
}
