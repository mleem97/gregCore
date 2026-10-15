/// <file-summary>
/// Schicht:      Tests
/// Zweck:        GregEventLog ohne Spiel: Aggregation (keine Duplikate),
///               Counter, Snapshot-Reihenfolge, Format, Ring-Cap, File-Sink.
/// Maintainer:   Reine Logik + Temp-Verzeichnis via LogDirectoryOverride.
/// </file-summary>

using System.IO;
using Xunit;
using gregCore.Core.Diagnostics;

namespace gregCore.Tests.Core;

public class GregEventLogTests
{
    [Fact]
    public void Emit_AggregatesSameKey()
    {
        GregEventLog.Clear();
        GregEventLog.LogDirectoryOverride = Path.Combine(Path.GetTempPath(), "gregtest-" + System.Guid.NewGuid().ToString("N"));
        GregEventLog.Emit("net:tx", "Server A sendet 10 Daten an Switch B");
        GregEventLog.Emit("net:tx", "Server A sendet 20 Daten an Switch B");
        GregEventLog.Emit("net:tx", "Server A sendet 30 Daten an Switch B");
        var rows = GregEventLog.Snapshot();
        Assert.Single(rows);
        Assert.Equal(3, rows[0].Count);
        Assert.Contains("30 Daten", rows[0].LastText);
        Assert.Contains("x3", GregEventLog.FormatRow(rows[0]));
    }

    [Fact]
    public void Emit_SeparateKeysSeparateRows()
    {
        GregEventLog.Clear();
        GregEventLog.Emit("a", "eins");
        GregEventLog.Emit("b", "zwei");
        Assert.Equal(2, GregEventLog.Snapshot().Count);
    }

    [Fact]
    public void Emit_NeverThrows()
    {
        GregEventLog.Clear();
        var ex = Record.Exception(() =>
        {
            GregEventLog.Emit(null, null, null);
            GregEventLog.Emit("", "");
        });
        Assert.Null(ex);
    }

    [Fact]
    public void FileSink_WritesVerboseLog()
    {
        GregEventLog.Clear();
        string dir = Path.Combine(Path.GetTempPath(), "gregtest-" + System.Guid.NewGuid().ToString("N"));
        GregEventLog.LogDirectoryOverride = dir;
        GregEventLog.Emit("k1", "eins");
        GregEventLog.Emit("k1", "zwei");
        GregEventLog.Emit("k2", "drei");
        string file = Path.Combine(dir, "events-" + System.DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
        Assert.True(File.Exists(file));
        string[] lines = File.ReadAllLines(file);
        Assert.Equal(3, lines.Length);
        Assert.Contains("#2", lines[1]);
        GregEventLog.LogDirectoryOverride = null;
    }
}
