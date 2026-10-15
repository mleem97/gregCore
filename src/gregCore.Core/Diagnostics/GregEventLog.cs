/// <file-summary>
/// Layer:       Core (Diagnostics)
/// Purpose:     Aggregated event log: EVERYTHING the framework sees, verbose
///               in file, clean on screen. Same key twice = counter++, never
///               a duplicate line ("Server A -> Switch B: 1204 packets x312").
/// Maintainer:  Pure aggregation + file sink. All entry points try-caught;
///               Emit never throws. Ring-capped (300 keys), daily log files.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gregCore.Core.Diagnostics
{
    /// <summary>
    /// Verbose-but-deduped event stream for menus, hooks, saves, jobs and API
    /// traffic. File sink keeps EVERYTHING; <see cref="Snapshot"/> renders one
    /// clean row per key for UI.
    /// </summary>
    public static class GregEventLog
    {
        public sealed class Row
        {
            public string Key { get; set; } = "";
            public string LastText { get; set; } = "";
            public string Kind { get; set; } = "info";
            public long Count { get; set; }
            public DateTime FirstUtc { get; set; }
            public DateTime LastUtc { get; set; }
        }

        private const int MaxKeys = 300;
        private static readonly object _gate = new();
        private static readonly Dictionary<string, Row> _rows =
            new Dictionary<string, Row>(StringComparer.Ordinal);
        private static string? _dirOverride;

        /// <summary>Override for tests. Null = UserData/gregCore/logs.</summary>
        public static string? LogDirectoryOverride
        {
            get { lock (_gate) return _dirOverride; }
            set { lock (_gate) _dirOverride = value; }
        }

        public static void Emit(string key, string text, string kind = "info")
        {
            try
            {
                if (string.IsNullOrEmpty(key)) key = "misc";
                if (text == null) text = "";
                if (string.IsNullOrEmpty(kind)) kind = "info";
                DateTime now = DateTime.UtcNow;
                Row row;
                lock (_gate)
                {
                    if (!_rows.TryGetValue(key, out row))
                    {
                        if (_rows.Count >= MaxKeys) EvictLocked();
                        row = new Row { Key = key, FirstUtc = now };
                        _rows[key] = row;
                    }
                    row.Count++;
                    row.LastUtc = now;
                    row.LastText = text;
                    row.Kind = kind;
                }
                AppendFileLocked(row);
            }
            catch { /* logging must never break the game */ }
        }

        public static List<Row> Snapshot()
        {
            try
            {
                lock (_gate)
                {
                    var list = new List<Row>(_rows.Values);
                    list.Sort((a, b) => b.LastUtc.CompareTo(a.LastUtc));
                    return list;
                }
            }
            catch { return new List<Row>(); }
        }

        public static string FormatRow(Row r)
        {
            try
            {
                string t = r.LastUtc.ToLocalTime().ToString("HH:mm:ss");
                string rate = "";
                double secs = (r.LastUtc - r.FirstUtc).TotalSeconds;
                if (r.Count > 3 && secs >= 1)
                {
                    double per = r.Count / Math.Max(1, secs);
                    rate = per >= 1 ? $" ({per:0.#}/s)" : "";
                }
                string n = r.Count > 1 ? $" x{r.Count}" : "";
                return $"[{t}] {r.LastText}{n}{rate}";
            }
            catch { return r.LastText ?? ""; }
        }

        public static void Clear()
        {
            try { lock (_gate) _rows.Clear(); } catch { }
        }

        private static void EvictLocked()
        {
            string? oldest = null;
            DateTime when = DateTime.MaxValue;
            foreach (var kv in _rows)
            {
                if (kv.Value.LastUtc < when) { when = kv.Value.LastUtc; oldest = kv.Key; }
            }
            if (oldest != null) _rows.Remove(oldest);
        }

        private static void AppendFileLocked(Row row)
        {
            try
            {
                string dir = LogDirectoryOverride ?? DefaultDir();
                if (string.IsNullOrEmpty(dir)) return;
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, "events-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
                PruneOldLocked(dir);
                string line = row.LastUtc.ToString("o") + " [" + row.Kind + "] "
                    + row.Key + " #" + row.Count + " " + row.LastText + Environment.NewLine;
                File.AppendAllText(file, line, Encoding.UTF8);
            }
            catch { /* file sink is best-effort */ }
        }

        private static string? DefaultDir()
        {
            try
            {
                return Path.Combine(
                    MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "gregCore", "logs");
            }
            catch { return null; }
        }

        private static void PruneOldLocked(string dir)
        {
            try
            {
                var files = Directory.GetFiles(dir, "events-*.log");
                if (files.Length <= 7) return;
                Array.Sort(files, StringComparer.Ordinal);
                for (int i = 0; i < files.Length - 7; i++)
                {
                    try { File.Delete(files[i]); } catch { }
                }
            }
            catch { }
        }
    }
}
