/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Tests für GregAttackEscalation.ComputeTierStats (reine
///               Mathe, ohne Spiel lauffähig).
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.Core.Networking;

namespace gregCore.Tests.Core;

public class GregAttackEscalationTests
{
    [Fact]
    public void Tier0_KeepsBaseValues()
    {
        var cfg = new GregAttackEscalation.EscalationConfig();

        var stats = GregAttackEscalation.ComputeTierStats(120f, 2f, 5f, 0, cfg);

        stats.AttackInterval.Should().BeApproximately(120f, 0.01f);
        stats.SpawnRate.Should().BeApproximately(2f, 0.01f);
        stats.MoveSpeed.Should().BeApproximately(5f, 0.01f);
    }

    [Fact]
    public void HigherTier_ShorterInterval_HigherRateAndSpeed()
    {
        var cfg = new GregAttackEscalation.EscalationConfig
        {
            IntervalFactor = 0.9f,
            RateGrowthPerTier = 0.15f,
            SpeedGrowthPerTier = 0.10f,
            MinIntervalSeconds = 20f,
        };

        var stats = GregAttackEscalation.ComputeTierStats(120f, 2f, 5f, 3, cfg);

        stats.AttackInterval.Should().BeLessThan(120f);
        stats.SpawnRate.Should().BeGreaterThan(2f);
        stats.MoveSpeed.Should().BeGreaterThan(5f);
    }

    [Fact]
    public void MinInterval_Clamps()
    {
        var cfg = new GregAttackEscalation.EscalationConfig
        {
            IntervalFactor = 0.5f,
            MinIntervalSeconds = 20f,
        };

        var stats = GregAttackEscalation.ComputeTierStats(120f, 2f, 5f, 99, cfg);

        stats.AttackInterval.Should().BeApproximately(20f, 0.01f);
    }

    [Fact]
    public void NegativeTier_TreatedAsZero()
    {
        var cfg = new GregAttackEscalation.EscalationConfig();

        var stats = GregAttackEscalation.ComputeTierStats(120f, 2f, 5f, -5, cfg);

        stats.AttackInterval.Should().BeApproximately(120f, 0.01f);
    }
}
