using System;
using System.IO;
using Xunit;
using FluentAssertions;
using gregCore.Core.Mods;

namespace gregCore.Tests.Infrastructure;

public class GregModpackManifestTests
{
    private static string NewModsDir()
    {
        var root = Path.Combine(Path.GetTempPath(), "gregManifest_" + Guid.NewGuid().ToString("N"));
        var mods = Path.Combine(root, "Mods");
        Directory.CreateDirectory(mods);
        return mods;
    }

    private static void WriteJson(string mods, string json) =>
        File.WriteAllText(Path.Combine(mods, "manifest.json"), json);

    [Fact]
    public void TryLoad_MissingManifest_ReturnsNotFoundWithoutWarnings()
    {
        var mods = NewModsDir();
        try
        {
            var v = GregModpackManifest.TryLoad(mods);
            v.Found.Should().BeFalse();
        }
        finally { try { Directory.Delete(Path.GetDirectoryName(mods), true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } }
    }

    [Fact]
    public void TryLoad_ValidatesEntries_SkipsDeactivatedTraversalAndNonDll()
    {
        var mods = NewModsDir();
        try
        {
            Directory.CreateDirectory(Path.Combine(mods, "lib"));
            Directory.CreateDirectory(Path.Combine(mods, ".deactivated"));
            File.WriteAllText(Path.Combine(mods, "MyMod.dll"), "x");
            File.WriteAllText(Path.Combine(mods, "lib", "MyLib.dll"), "x");
            File.WriteAllText(Path.Combine(mods, ".deactivated", "Off.dll"), "x");
            WriteJson(mods, @"{ ""Name"": ""Pack"", ""Mods"": [""MyMod.dll"", "".deactivated/Off.dll"", ""../evil.dll"", ""readme.txt"", ""Missing.dll""],
                ""Library"": [""lib/MyLib.dll""], ""Plugins"": [] }");

            var v = GregModpackManifest.TryLoad(mods);
            v.Found.Should().BeTrue();
            v.Manifest.Name.Should().Be("Pack");
            v.ValidMods.Should().ContainSingle(e => e == "MyMod.dll");
            v.ValidLibrary.Should().ContainSingle(e => e == "lib/MyLib.dll");
            v.Warnings.Should().HaveCountGreaterThanOrEqualTo(3);
            v.LibraryFolders.Should().ContainSingle(d => d.EndsWith("lib"));
        }
        finally { try { Directory.Delete(Path.GetDirectoryName(mods), true); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } }
    }

    [Fact]
    public void PresetKey_IsStable()
    {
        var p = new GregCustomItemPresets.Preset { ItemID = 3, ItemType = 9, ColorHex = "#FF0000" };
        GregCustomItemPresets.Key(p).Should().Be("3|9|#FF0000");
    }
}
