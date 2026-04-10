using System;
using System.Collections.Generic;
using System.IO;

namespace AssetExporter
{
    public sealed class GameSignalSnapshotService
    {
        public string ExportAll(string diagnosticsPath, Il2CppEventCatalogService eventCatalogService, Il2CppGameplayIndexService gameplayIndexService, RuntimeHookService runtimeHookService)
        {
            Directory.CreateDirectory(diagnosticsPath);

            string eventCatalogPath = eventCatalogService.ExportCatalog(diagnosticsPath);
            string gameplayIndexPath = gameplayIndexService.ExportGameplayIndex(diagnosticsPath);
            HookScanResult hookScan = runtimeHookService.ScanCandidates(100000);

            string snapshotPath = Path.Combine(diagnosticsPath, "game-signals-full.txt");
            var lines = new List<string>
            {
                "# Game Signals Full Snapshot",
                $"timestamp_utc={DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
                $"event_catalog={eventCatalogPath}",
                $"gameplay_index={gameplayIndexPath}",
                $"hook_candidates={hookScan.Candidates.Count}",
                "",
                "## Event Catalog (embedded)",
            };

            TryAppendFile(lines, eventCatalogPath);

            lines.Add("");
            lines.Add("## Gameplay Index (embedded)");
            TryAppendFile(lines, gameplayIndexPath);

            lines.Add("");
            lines.Add("## Hook Candidates");

            foreach (string candidate in hookScan.Candidates)
                lines.Add($"hook_candidate | method={candidate}");

            File.WriteAllLines(snapshotPath, lines);
            return snapshotPath;
        }

        private static void TryAppendFile(List<string> lines, string filePath)
        {
            try
            {
                foreach (string line in File.ReadAllLines(filePath))
                    lines.Add(line);
            }
            catch (Exception ex)
            {
                lines.Add($"embed_failed | file={filePath} | error={ex.Message}");
            }
        }
    }
}
