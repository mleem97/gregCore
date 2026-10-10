/// <file-summary>
/// Layer:       Tests
/// Purpose:     Tests for the GregComputer registry (shortcuts + apps) —
///               pure managed logic, runnable without the game.
/// </file-summary>

using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using gregCore.UI;

namespace gregCore.Tests.UI;

public class GregComputerTests : IDisposable
{
    public GregComputerTests()
    {
        GregComputer.UnregisterAll("test.a");
        GregComputer.UnregisterAll("test.b");
        GregComputer.CloseApp();
    }

    public void Dispose()
    {
        GregComputer.UnregisterAll("test.a");
        GregComputer.UnregisterAll("test.b");
        GregComputer.CloseApp();
    }

    [Fact]
    public void RegisterShortcut_Valid_ReturnsTrueAndLists()
    {
        GregComputer.RegisterShortcut("test.a", "s1", "Alpha", () => { }).Should().BeTrue();

        var all = GregComputer.Shortcuts();
        all.Should().ContainSingle(s => s.Id == "s1" && s.Label == "Alpha" && s.ModId == "test.a");
    }

    [Fact]
    public void RegisterShortcut_InvalidInput_ReturnsFalse()
    {
        GregComputer.RegisterShortcut("", "s", "L", () => { }).Should().BeFalse();
        GregComputer.RegisterShortcut("test.a", "", "L", () => { }).Should().BeFalse();
        GregComputer.RegisterShortcut("test.a", "s", "", () => { }).Should().BeFalse();
    }

    [Fact]
    public void RegisterShortcut_SameId_Replaces()
    {
        GregComputer.RegisterShortcut("test.a", "s1", "Old", () => { }).Should().BeTrue();
        GregComputer.RegisterShortcut("test.a", "s1", "New", () => { }).Should().BeTrue();

        var all = GregComputer.Shortcuts().Where(s => s.ModId == "test.a" && s.Id == "s1").ToArray();
        all.Should().ContainSingle().Which.Label.Should().Be("New");
    }

    [Fact]
    public void Shortcuts_AreOrderedByOrderThenLabel()
    {
        GregComputer.RegisterShortcut("test.a", "s1", "Bravo", () => { }, order: 200);
        GregComputer.RegisterShortcut("test.a", "s2", "Alpha", () => { }, order: 10);

        var mine = GregComputer.Shortcuts().Where(s => s.ModId == "test.a").Select(s => s.Id).ToArray();
        mine.Should().Equal("s2", "s1");
    }

    [Fact]
    public void InvokeShortcut_RunsOnClickAndReturnsTrue()
    {
        bool ran = false;
        GregComputer.RegisterShortcut("test.a", "s1", "Run", () => { ran = true; });

        GregComputer.InvokeShortcut("test.a", "s1").Should().BeTrue();
        ran.Should().BeTrue();
    }

    [Fact]
    public void InvokeShortcut_Unknown_ReturnsFalse()
    {
        GregComputer.InvokeShortcut("test.a", "nope").Should().BeFalse();
    }

    [Fact]
    public void InvokeShortcut_WithAppId_OpensApp()
    {
        GregComputer.RegisterApp("test.a", "app1", "App One", build: null, framePage: false);
        GregComputer.RegisterShortcut("test.a", "s1", "Open", null, appId: "app1");

        GregComputer.InvokeShortcut("test.a", "s1").Should().BeTrue();
        GregComputer.CurrentAppId.Should().Be("app1");
    }

    [Fact]
    public void UnregisterShortcut_RemovesOnlyOwn()
    {
        GregComputer.RegisterShortcut("test.a", "s1", "A", () => { });
        GregComputer.RegisterShortcut("test.b", "s1", "B", () => { });

        GregComputer.UnregisterShortcut("test.a", "s1").Should().BeTrue();
        GregComputer.UnregisterShortcut("test.a", "s1").Should().BeFalse();

        GregComputer.Shortcuts().Should().ContainSingle(s => s.ModId == "test.b");
    }

    [Fact]
    public void RegisterApp_InvalidInput_ReturnsFalse()
    {
        GregComputer.RegisterApp("", "app", "T", null).Should().BeFalse();
        GregComputer.RegisterApp("test.a", "", "T", null).Should().BeFalse();
        GregComputer.RegisterApp("test.a", "app", "", null).Should().BeFalse();
    }

    [Fact]
    public void TryOpenApp_Unknown_ReturnsFalse()
    {
        GregComputer.TryOpenApp("nope").Should().BeFalse();
        GregComputer.CurrentAppId.Should().BeEmpty();
    }

    [Fact]
    public void TryOpenApp_SetsCurrentAndFiresEvents()
    {
        string? opened = null;
        string? closed = null;
        Action<string> onOpened = id => { opened = id; };
        Action<string> onClosedEvt = id => { closed = id; };
        GregComputer.AppOpened += onOpened;
        GregComputer.AppClosed += onClosedEvt;
        try
        {
            bool closedCb = false;
            GregComputer.RegisterApp("test.a", "app1", "App One", build: null,
                onClosed: () => { closedCb = true; }, framePage: false);

            GregComputer.TryOpenApp("app1").Should().BeTrue();
            GregComputer.CurrentAppId.Should().Be("app1");
            opened.Should().Be("app1");

            GregComputer.CloseApp();
            GregComputer.CurrentAppId.Should().BeEmpty();
            closed.Should().Be("app1");
            closedCb.Should().BeTrue();
        }
        finally
        {
            GregComputer.AppOpened -= onOpened;
            GregComputer.AppClosed -= onClosedEvt;
        }
    }

    [Fact]
    public void UnregisterApp_WhileOpen_ClosesApp()
    {
        GregComputer.RegisterApp("test.a", "app1", "App One", build: null, framePage: false);
        GregComputer.TryOpenApp("app1").Should().BeTrue();

        GregComputer.UnregisterApp("test.a", "app1").Should().BeTrue();
        GregComputer.CurrentAppId.Should().BeEmpty();
    }

    [Fact]
    public void UnregisterAll_RemovesOnlyOwnScope()
    {
        GregComputer.RegisterShortcut("test.a", "s1", "A", () => { });
        GregComputer.RegisterApp("test.a", "app1", "App One", build: null, framePage: false);
        GregComputer.RegisterShortcut("test.b", "s1", "B", () => { });

        GregComputer.UnregisterAll("test.a").Should().Be(2);
        GregComputer.Shortcuts().Should().ContainSingle(s => s.ModId == "test.b");
        GregComputer.Apps().Should().BeEmpty();
    }

    [Fact]
    public void Snapshots_NeverNull()
    {
        GregComputer.Apps().Should().NotBeNull();
        GregComputer.Shortcuts().Should().NotBeNull();
    }
}
