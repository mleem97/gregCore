/// <file-summary>
/// Layer:      Tests
/// Purpose:    Tests for keybind auto-resolve: collisions and game-reserved
///             keys move to a free fallback key instead of biting each other.
///             Pure managed logic, runnable without the game.
/// </file-summary>

using System.Linq;
using UnityEngine;
using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.Settings;
using gregCore.Infrastructure.Settings.Models;

namespace gregCore.Tests.UI;

public class GregKeybindAutoResolveTests
{
    private static GregKeybindRegistry FreshRegistry(out gregCore.Tests.Mocks.MockLogger log)
    {
        log = new gregCore.Tests.Mocks.MockLogger();
        return new GregKeybindRegistry(log);
    }

    private static KeybindEntry Entry(string mod, string action, KeyCode key)
    {
        return new KeybindEntry
        {
            ModId = mod,
            ActionId = action,
            DisplayName = action,
            Description = action,
            Category = "Controls",
            DefaultKey = key,
            CurrentKey = key,
        };
    }

    [Fact]
    public void Register_FreeKey_StaysUntouched()
    {
        var reg = FreshRegistry(out _);

        reg.Register(Entry("mod.a", "toggle", KeyCode.F7));

        var got = reg.Get("mod.a", "toggle");
        got.CurrentKey.Should().Be(KeyCode.F7);
        got.AutoResolved.Should().BeFalse();
        got.HasConflict.Should().BeFalse();
    }

    [Fact]
    public void Register_CollidingKey_MovesToFirstFreePoolKey()
    {
        var reg = FreshRegistry(out var log);
        reg.Register(Entry("mod.a", "toggle", KeyCode.F7));

        reg.Register(Entry("mod.b", "toggle", KeyCode.F7));

        var got = reg.Get("mod.b", "toggle");
        got.CurrentKey.Should().Be(KeyCode.F12);
        got.AutoResolved.Should().BeTrue();
        got.HasConflict.Should().BeFalse();
        log.AssertLogged(gregCore.Tests.Mocks.MockLogger.LogLevel.Warning, "auto-resolved").Should().BeTrue();
        // First entry keeps its key.
        reg.Get("mod.a", "toggle").CurrentKey.Should().Be(KeyCode.F7);
    }

    [Fact]
    public void Register_GameReservedKey_MovesAway()
    {
        var reg = FreshRegistry(out _);
        reg.Register(Entry("mod.a", "toggle", KeyCode.Escape));

        var got = reg.Get("mod.a", "toggle");
        got.CurrentKey.Should().NotBe(KeyCode.Escape);
        got.AutoResolved.Should().BeTrue();
    }

    [Fact]
    public void Register_PersistedKeyKept_WhenFree()
    {
        var reg = FreshRegistry(out _);
        // Simulates a load: CurrentKey came from gregCore_Keybinds.json.
        reg.Register(Entry("mod.a", "toggle", KeyCode.F8));

        reg.Get("mod.a", "toggle").CurrentKey.Should().Be(KeyCode.F8);
    }

    [Fact]
    public void Register_PersistedKeyColliding_ReResolves()
    {
        var reg = FreshRegistry(out _);
        reg.Register(Entry("mod.a", "toggle", KeyCode.F8));
        reg.Register(Entry("mod.b", "toggle", KeyCode.F8));

        reg.Get("mod.b", "toggle").CurrentKey.Should().NotBe(KeyCode.F8);
        reg.Get("mod.b", "toggle").AutoResolved.Should().BeTrue();
    }

    [Fact]
    public void Register_PoolExhausted_KeepsKeyAndFlagsConflict()
    {
        var reg = FreshRegistry(out _);
        int i = 0;
        foreach (var key in GregKeybindRegistry.FallbackPool)
            reg.Register(Entry("mod.fill", "a" + (i++), key));

        reg.Register(Entry("mod.late", "toggle", KeyCode.F12));

        var got = reg.Get("mod.late", "toggle");
        got.CurrentKey.Should().Be(KeyCode.F12);
        got.HasConflict.Should().BeTrue();
    }

    [Fact]
    public void FindFreeKey_SkipsReservedAndTaken()
    {
        var reg = FreshRegistry(out _);
        reg.Register(Entry("mod.a", "toggle", KeyCode.F12));

        var free = reg.FindFreeKey();

        free.Should().NotBeNull();
        free.Value.Should().NotBe(KeyCode.F12);
        GregKeybindRegistry.GameReservedKeys.Should().NotContain(free.Value);
    }

    [Fact]
    public void Unregister_FreesKeyForNextClaim()
    {
        var reg = FreshRegistry(out _);
        reg.Register(Entry("mod.a", "toggle", KeyCode.F7));
        reg.Register(Entry("mod.b", "toggle", KeyCode.F7));
        reg.Get("mod.b", "toggle").CurrentKey.Should().NotBe(KeyCode.F7);

        reg.Unregister("mod.a", "toggle");
        reg.Register(Entry("mod.c", "toggle", KeyCode.F7));

        // F7 is free again: no need to move.
        reg.Get("mod.c", "toggle").CurrentKey.Should().Be(KeyCode.F7);
    }
}
