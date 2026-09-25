/// <file-summary>
/// Layer:       Tests
/// Purpose:     Tests for the SaveGuard backup guarantee: path resolution
///               (Windows + Linux), write-gate truth table, sanitize and
///               sidecar-path helpers — pure managed logic, no game.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.Persistence;

namespace gregCore.Tests.Core;

public class GregSaveGuardTests
{
    private static bool ExistsNone(string _) => false;

    [Fact]
    public void ResolveBackupRoot_WindowsDocuments_UsesIt()
    {
        // Forward slashes act as separators on both platforms.
        bool ExistsWinDocs(string p) => string.Equals(p, "C:/Users/greg/Documents", StringComparison.Ordinal);
        string root = GregSaveGuard.ResolveBackupRoot(
            "C:/Users/greg/Documents", "C:/Users/greg", ExistsWinDocs);

        root.Should().Be(Path.Combine("C:/Users/greg/Documents", "DatacenterBackups"));
    }

    [Fact]
    public void ResolveBackupRoot_LinuxHomeItself_FallsBackToHome()
    {
        // .NET on Linux maps MyDocuments to $HOME (basename != Documents).
        string root = GregSaveGuard.ResolveBackupRoot("/home/greg", "/home/greg", ExistsNone);

        root.Should().Be(Path.Combine("/home/greg", "DatacenterBackups"));
    }

    [Fact]
    public void ResolveBackupRoot_LinuxDocumentsExists_UsesIt()
    {
        bool ExistsDocs(string p) => string.Equals(p, "/home/greg/Documents", StringComparison.Ordinal);
        string root = GregSaveGuard.ResolveBackupRoot("/home/greg", "/home/greg", ExistsDocs);

        root.Should().Be(Path.Combine("/home/greg/Documents", "DatacenterBackups"));
    }

    [Fact]
    public void ResolveBackupRoot_EmptyInputs_FallsBackToDot()
    {
        string root = GregSaveGuard.ResolveBackupRoot(null, null, ExistsNone);

        root.Should().Be(Path.Combine(".", "DatacenterBackups"));
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ResolveSaveWriteGate_OnlyWritesWhenBackedUp(bool enabled, bool ok, bool expected)
    {
        GregSaveGuard.ResolveSaveWriteGate(enabled, ok).Should().Be(expected);
    }

    [Theory]
    [InlineData("My Save", "My_Save")]
    [InlineData("slot.1", "slot_1")]
    [InlineData("", "save")]
    [InlineData("   ", "save")]
    public void Sanitize_CleansFileNames(string input, string expected)
    {
        GregSaveGuard.Sanitize(input).Should().Be(expected);
    }

    [Fact]
    public void SidecarPath_NestsNextToSave()
    {
        string dir = Path.Combine("saves");
        string path = GregSaveGuard.SidecarPath(dir, "Slot 1", "shift_helper");

        path.Should().Be(Path.Combine(dir, "greg_shift_helper.Slot_1.tsv"));
    }
}
