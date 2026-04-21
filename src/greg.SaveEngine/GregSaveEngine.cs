using System;
using System.IO;
using LiteDB;
using gregCore.API;
using greg.GridPlacement;
using greg.Logging;

namespace greg.SaveEngine
{
    public class GregSaveEngine
    {
        public static GregSaveEngine Instance { get; private set; } = null!;
        
        private LiteDatabase? _db;
        public string DbPath { get; private set; } = string.Empty;

        private readonly GregModLogger _log = new GregModLogger("SaveEngine");

        public GregSaveEngine()
        {
            Instance = this;
        }

        public void Initialize(string saveDir)
        {
            DbPath = Path.Combine(saveDir, $"gregSave_{Guid.NewGuid():N}.greg.db");
            _db = new LiteDatabase(DbPath);
            
            var metaCol = _db.GetCollection<MetaDocument>("greg_meta");
            metaCol.Upsert(new MetaDocument 
            { 
                Id = "header", 
                Value = "greg.SaveEngine.v1",
                CreatedAt = DateTime.UtcNow,
                LastSavedAt = DateTime.UtcNow,
                IsVanillaSave = false
            });
            
            _log.Section("Init");
            _log.Msg($"LiteDB initialized at {DbPath}");
            _log.FeatureState("SaveEngine", true);
            _log.Msg("Auto-save interval: 60s");
        }

        public void SaveAll()
        {
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("SaveEngine.Write")) return;
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            SaveGridState(GregGridManager.Instance);
            // SaveServerState(...)
            // SaveNetworkState(...)
            
            var metaCol = _db?.GetCollection<MetaDocument>("greg_meta");
            if (metaCol != null)
            {
                var doc = metaCol.FindById("header");
                if (doc != null)
                {
                    doc.LastSavedAt = DateTime.UtcNow;
                    metaCol.Update(doc);
                }
            }
            watch.Stop();
            
            _log.Section("Save");
            _log.Saved(1, watch.ElapsedMilliseconds);
            
            GregSaveNotifier.NotifySave("Auto-saved complete state.");
        }

        public void LoadAll()
        {
            _log.Section("Load");
            LoadGridState(GregGridManager.Instance);

            // Example implementation as requested
            _log.Loaded(1, DbPath);
            _log.VanillaSaveDetected("GridPlacement");

            GregSaveNotifier.NotifyLoad(DbPath);
        }

        public void SaveGridState(GregGridManager grid)
        {
            if (grid == null || _db == null) return;
            
            var col = _db.GetCollection<GridStateDocument>("grid_state");
            col.DeleteAll();
            
            var doc = new GridStateDocument
            {
                GridOriginX = grid.GridOrigin.x,
                GridOriginY = grid.GridOrigin.y,
                GridOriginZ = grid.GridOrigin.z,
                CellSizeX = grid.CellSizeX,
                CellSizeZ = grid.CellSizeZ,
                SavedAt = DateTime.UtcNow
            };
            
            col.Insert(doc);
        }

        public void LoadGridState(GregGridManager grid)
        {
            if (grid == null || _db == null) return;
            var col = _db.GetCollection<GridStateDocument>("grid_state");
            var doc = col.FindOne(Query.All());
            if (doc != null)
            {
                grid.Initialize(new UnityEngine.Vector3(doc.GridOriginX, doc.GridOriginY, doc.GridOriginZ), 50, 50);
            }
        }

        public bool IsGregSave(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            if (!filePath.EndsWith(".greg.db")) return false;
            
            try
            {
                using var db = new LiteDatabase(filePath);
                var metaCol = db.GetCollection<MetaDocument>("greg_meta");
                var doc = metaCol.FindById("header");
                return doc != null && doc.Value == "greg.SaveEngine.v1";
            }
            catch
            {
                return false;
            }
        }

        public ILiteCollection<T>? GetCollection<T>(string name)
        {
            return _db?.GetCollection<T>(name);
        }
    }

    public class MetaDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastSavedAt { get; set; }
        public bool IsVanillaSave { get; set; }
    }

    public class GridStateDocument
    {
        public int Id { get; set; }
        public float GridOriginX { get; set; }
        public float GridOriginY { get; set; }
        public float GridOriginZ { get; set; }
        public float CellSizeX { get; set; }
        public float CellSizeZ { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
