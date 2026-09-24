/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Vollabdeckung der RAM-Alert-Entscheidungslogik
///               (Metrik-Auswahl, Klassifizierung, Edge-Trigger).
/// Maintainer:   Alles hier muss 100 % Zeilen-Coverage halten.
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.Infrastructure.Performance;

namespace gregCore.Tests.Infrastructure;

public class RamAlertEvaluatorTests
{
    [Theory]
    [InlineData(5000, 1200, 1200)]
    [InlineData(5000, 0, 5000)]
    [InlineData(5000, -10, 5000)]
    public void SelectRamMetric_PrefersPrivateBytes(int workingSet, int privateMb, int expected)
    {
        RamAlertEvaluator.SelectRamMetricMb(workingSet, privateMb).Should().Be(expected);
    }

    [Theory]
    [InlineData(100, 2048, 4096, RamAlertLevel.None)]
    [InlineData(2047, 2048, 4096, RamAlertLevel.None)]
    [InlineData(2048, 2048, 4096, RamAlertLevel.Warning)]
    [InlineData(3000, 2048, 4096, RamAlertLevel.Warning)]
    [InlineData(4095, 2048, 4096, RamAlertLevel.Warning)]
    [InlineData(4096, 2048, 4096, RamAlertLevel.Critical)]
    [InlineData(9000, 2048, 4096, RamAlertLevel.Critical)]
    public void Classify_UsesThresholds(int ramMb, int warnMb, int critMb, RamAlertLevel expected)
    {
        RamAlertEvaluator.Classify(ramMb, warnMb, critMb).Should().Be(expected);
    }

    [Fact]
    public void AlertState_FiresOncePerEscalation()
    {
        var state = new RamAlertState();
        state.Evaluate(100, 2048, 4096).Should().BeNull();
        state.Evaluate(2500, 2048, 4096).Should().Be(RamAlertLevel.Warning);
        // Anhaltend: kein zweites Feuern.
        state.Evaluate(2500, 2048, 4096).Should().BeNull();
        state.Evaluate(2600, 2048, 4096).Should().BeNull();
        // Eskalation feuert.
        state.Evaluate(5000, 2048, 4096).Should().Be(RamAlertLevel.Critical);
        state.Evaluate(5000, 2048, 4096).Should().BeNull();
        state.Active.Should().Be(RamAlertLevel.Critical);
    }

    [Fact]
    public void AlertState_RearmsBelowHysteresis()
    {
        var state = new RamAlertState();
        state.Evaluate(2500, 2048, 4096).Should().Be(RamAlertLevel.Warning);
        // Knapp unter der Schwelle: noch nicht re-armed (Hysterese 256).
        state.Evaluate(2047, 2048, 4096).Should().BeNull();
        state.Active.Should().Be(RamAlertLevel.Warning);
        // Deutlich darunter: re-armed, naechster Anstieg feuert wieder.
        state.Evaluate(100, 2048, 4096).Should().BeNull();
        state.Active.Should().Be(RamAlertLevel.None);
        state.Evaluate(2500, 2048, 4096).Should().Be(RamAlertLevel.Warning);
    }

    [Fact]
    public void AlertState_DeescalationFiresOnceWhenRearmed()
    {
        var state = new RamAlertState();
        state.Evaluate(5000, 2048, 4096).Should().Be(RamAlertLevel.Critical);
        // Unter Critical-Hysterese (4096-256), aber ueber Warning: Deeskalation.
        state.Evaluate(3000, 2048, 4096).Should().Be(RamAlertLevel.Warning);
        state.Active.Should().Be(RamAlertLevel.Warning);
        state.Evaluate(3000, 2048, 4096).Should().BeNull();
    }
}
