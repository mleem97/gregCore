/// <file-summary>
/// Schicht:      Tests
/// Zweck:        Vollabdeckung der reinen Demand-Entscheidungslogik
///               (DemandPlanner, DemandOptions, DemandScanResult).
/// Maintainer:   Il2Cpp-Zugriff ist ausgenommen (braucht Live-Spiel),
///               alles hier muss 100 % Zeilen-Coverage halten.
/// </file-summary>

using Xunit;
using FluentAssertions;
using gregCore.PublicApi.Modules;

namespace gregCore.Tests.PublicApi;

public class DemandPlannerTests
{
    [Theory]
    [InlineData(true, true, true, false, true, true, true, true)]
    [InlineData(true, true, true, true, false, false, false, false)]
    [InlineData(true, false, false, false, true, true, false, false)]
    [InlineData(false, true, false, false, false, false, false, false)]
    [InlineData(false, false, true, false, false, false, false, false)]
    [InlineData(false, false, false, false, false, false, false, false)]
    [InlineData(true, true, true, false, false, false, false, false)]
    public void ResolveActions_MapsOptionsCorrectly(
        bool route, bool feed, bool products, bool dryRun, bool canWrite,
        bool expectRoute, bool expectFeed, bool expectProducts)
    {
        var options = new DemandOptions
        {
            AutoRouteSubnets = route,
            AutoFeedPerformance = feed,
            AutoAddProducts = products,
            DryRun = dryRun,
            CanWriteWorld = canWrite,
        };

        var (doRoute, doFeed, doProducts) = DemandPlanner.ResolveActions(options);

        doRoute.Should().Be(expectRoute);
        doFeed.Should().Be(expectFeed);
        doProducts.Should().Be(expectProducts);
    }

    [Fact]
    public void ComputeFeed_ScalesWithMultiplier()
    {
        var req = new float[] { 10f, 5f, 7f, 3f };
        var cur = new float[] { 4f, 5f, 7.0005f, 9f };

        DemandPlanner.ComputeFeed(req, cur, 1f)
            .Should().BeEquivalentTo(
                new List<(int, float)> { (0, 6f) },
                opt => opt.WithStrictOrdering());

        DemandPlanner.ComputeFeed(req, cur, 2f)
            .Should().BeEquivalentTo(
                new List<(int, float)> { (0, 12f) },
                opt => opt.WithStrictOrdering());

        DemandPlanner.ComputeFeed(req, cur, 0.5f)[0].Amount.Should().BeApproximately(3f, 0.001f);

        // Multiplier 0 oder negativ: kein Feed (Nachfrage aus).
        DemandPlanner.ComputeFeed(req, cur, 0f).Should().BeEmpty();
        DemandPlanner.ComputeFeed(req, cur, -1f).Should().BeEmpty();
    }

    [Fact]
    public void ComputeFeed_EpsilonBoundary()
    {
        // Exakt an der Epsilon-Grenze: kein Feed.
        DemandPlanner.ComputeFeed(new float[] { 1f }, new float[] { 1f - 0.001f }, 1f)
            .Should().BeEmpty();
        // Knapp darueber: Feed.
        DemandPlanner.ComputeFeed(new float[] { 1f }, new float[] { 1f - 0.0011f }, 1f)
            .Should().HaveCount(1);
        // Kuerzeres Ist-Array begrenzt die Paare.
        DemandPlanner.ComputeFeed(new float[] { 5f, 5f }, new float[] { 1f }, 1f)
            .Should().HaveCount(1);
        // Leere Arrays: keine Paare, kein Fehler.
        DemandPlanner.ComputeFeed(Array.Empty<float>(), Array.Empty<float>(), 1f)
            .Should().BeEmpty();
    }

    [Fact]
    public void BuildRouteKey_HasStableFormat()
    {
        DemandPlanner.BuildRouteKey(12, 3).Should().Be("greg-demand-12-3");
    }

    [Fact]
    public void ComputeMissingProducts_OnlyReportsNewIds()
    {
        DemandPlanner.ComputeMissingProducts(new List<int> { 0, 1 }, new int[] { 1, 2, 2, 3 })
            .Should().BeEquivalentTo(new List<int> { 2, 3 }, opt => opt.WithStrictOrdering());
        DemandPlanner.ComputeMissingProducts(new List<int> { 0 }, null).Should().BeEmpty();
        DemandPlanner.ComputeMissingProducts(new List<int>(), new int[0])
            .Should().BeEmpty();
    }

    public static TheoryData<string?, int[]> AppIdCases => new TheoryData<string?, int[]>
    {
        { "1,2, 3", new int[] { 1, 2, 3 } },
        { "", new int[0] },
        { null, new int[0] },
        { "  ", new int[0] },
        { "5,,x,-1,5", new int[] { 5 } },
    };

    [Theory]
    [MemberData(nameof(AppIdCases))]
    public void ParseAppIds_ParsesRobustly(string? text, int[] expected)
    {
        DemandPlanner.ParseAppIds(text).Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
    }

    [Theory]
    [InlineData(null, true, true, true)]
    [InlineData(null, true, false, false)]
    [InlineData(null, false, true, false)]
    [InlineData(false, true, true, true)]
    [InlineData(true, true, true, false)]
    [InlineData(false, false, true, false)]
    [InlineData(true, false, true, false)]
    public void IsNewlySatisfied_OnlyFiresOnTransitionWhileActive(
        bool? wasMet, bool metAfter, bool active, bool expected)
    {
        DemandPlanner.IsNewlySatisfied(wasMet, metAfter, active).Should().Be(expected);
    }

    [Fact]
    public void DemandOptions_DefaultsAreSafe()
    {
        var options = new DemandOptions();
        options.AutoRouteSubnets.Should().BeFalse();
        options.AutoFeedPerformance.Should().BeFalse();
        options.AutoAddProducts.Should().BeFalse();
        options.DryRun.Should().BeTrue();
        options.CanWriteWorld.Should().BeTrue();
        options.DemandMultiplier.Should().Be(1f);
        options.ProductAppIds.Should().BeEmpty();
        options.ProductDifficulty.Should().Be(1);
        options.HighEndEnabled.Should().BeFalse();
        options.HighEndStep.Should().Be(0.1f);
        options.Quiet.Should().BeFalse();
    }

    [Fact]
    public void DemandScanResult_SummaryFormats()
    {
        var result = new DemandScanResult { DryRun = true };
        result.Summary.Should().Be("0 Kunden, 0 Apps | Route: 0 | Speed: 0.0 | Produkte: 0 | HighEnd: 0.0 (0) | Neu versorgt: 0 | DRY-RUN");

        var live = new DemandScanResult { DryRun = false };
        live.Summary.Should().NotContain("DRY-RUN");
    }

    [Fact]
    public void DemandScanResult_SummaryWithValues()
    {
        var result = new DemandScanResult
        {
            Customers = 2,
            Apps = 5,
            Routed = 3,
            FedTotal = 42.5f,
            ProductsAdded = 1,
            HighEndDrained = 10f,
            HighEndCustomers = 2,
            DryRun = false,
        };
        result.NewlySatisfied.Add(7);
        result.Summary.Should().Be("2 Kunden, 5 Apps | Route: 3 | Speed: 42.5 | Produkte: 1 | HighEnd: 10.0 (2) | Neu versorgt: 1");
    }

    // --- Demand Generation Tests ---

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    public void SamplePoissonInterval_InvalidRate_ReturnsMaxValue(float rate)
    {
        DemandPlanner.SamplePoissonInterval(rate).Should().Be(float.MaxValue);
    }

    [Fact]
    public void SamplePoissonInterval_PositiveRate_ReturnsPositiveValue()
    {
        var rng = new Random(42); // deterministisch
        float interval = DemandPlanner.SamplePoissonInterval(1f / 180f, rng);
        interval.Should().BeGreaterThan(0f);
        interval.Should().BeLessThan(1000f); // reasonable bound
    }

    [Fact]
    public void SamplePoissonInterval_DistributionSpread()
    {
        // Bei rate = 1/180 sollte der Mittelwert ~180 sein
        var rng = new Random(42);
        float sum = 0f;
        int samples = 1000;
        for (int i = 0; i < samples; i++)
            sum += DemandPlanner.SamplePoissonInterval(1f / 180f, rng);
        float mean = sum / samples;
        mean.Should().BeInRange(140f, 260f); // ~180 +/- 40%
    }

    [Fact]
    public void SelectDemandTargets_ZeroApps_ReturnsEmpty()
    {
        DemandPlanner.SelectDemandTargets(0, 3).Should().BeEmpty();
    }

    [Fact]
    public void SelectDemandTargets_AllApps()
    {
        var targets = DemandPlanner.SelectDemandTargets(5, 0);
        targets.Should().HaveCount(5);
        targets.Should().OnlyHaveUniqueItems();
        targets.Should().BeEquivalentTo(new[] { 0, 1, 2, 3, 4 });
    }

    [Fact]
    public void SelectDemandTargets_LimitedApps()
    {
        var rng = new Random(42);
        var targets = DemandPlanner.SelectDemandTargets(10, 3, rng);
        targets.Should().HaveCount(3);
        targets.Should().OnlyHaveUniqueItems();
        targets.Should().AllSatisfy(t => t.Should().BeInRange(0, 9));
    }

    [Fact]
    public void SelectDemandTargets_MaxExceedsTotal()
    {
        var targets = DemandPlanner.SelectDemandTargets(2, 10);
        targets.Should().HaveCount(2);
    }

    [Fact]
    public void ScaleSpeedRequirement_Normal()
    {
        DemandPlanner.ScaleSpeedRequirement(100f, 1.5f).Should().BeApproximately(150f, 0.001f);
        DemandPlanner.ScaleSpeedRequirement(100f, 1.0f).Should().BeApproximately(100f, 0.001f);
    }

    [Fact]
    public void ScaleSpeedRequirement_ZeroFactor_ReturnsZero()
    {
        DemandPlanner.ScaleSpeedRequirement(100f, 0f).Should().Be(0f);
        DemandPlanner.ScaleSpeedRequirement(100f, -1f).Should().Be(0f);
    }

    [Theory]
    [InlineData(30, 1.0f, 30)]
    [InlineData(30, 1.5f, 45)]
    [InlineData(0, 1.0f, 30)]  // Fallback
    [InlineData(30, 0f, 30)]   // Minimum 5
    [InlineData(30, 0.1f, 5)]  // Minimum 5
    public void ComputeDemandTimeout_ScalesCorrectly(int baseTimeout, float scale, int expected)
    {
        DemandPlanner.ComputeDemandTimeout(baseTimeout, scale).Should().Be(expected);
    }

    // --- DemandShift Tests ---

    [Fact]
    public void PickShiftKind_OnlySurge_WhenOthersDisabled()
    {
        var rng = new Random(7);
        for (int i = 0; i < 20; i++)
            DemandPlanner.PickShiftKind(rng, false, false, false)
                .Should().Be(DemandPlanner.DemandShiftKind.Surge);
    }

    [Fact]
    public void PickShiftKind_NoService_WithoutPool()
    {
        var rng = new Random(7);
        for (int i = 0; i < 50; i++)
            DemandPlanner.PickShiftKind(rng, true, true, false)
                .Should().NotBe(DemandPlanner.DemandShiftKind.NewService);
    }

    [Fact]
    public void PickShiftKind_AllKindsReachable()
    {
        var seen = new HashSet<DemandPlanner.DemandShiftKind>();
        var rng = new Random(12345);
        for (int i = 0; i < 500; i++)
            seen.Add(DemandPlanner.PickShiftKind(rng, true, true, true));
        seen.Should().Contain(DemandPlanner.DemandShiftKind.Surge);
        seen.Should().Contain(DemandPlanner.DemandShiftKind.Dip);
        seen.Should().Contain(DemandPlanner.DemandShiftKind.NewService);
    }

    [Fact]
    public void PickShiftKind_NullRng_Throws()
    {
        FluentActions.Invoking(() => DemandPlanner.PickShiftKind(null, true, true, true))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ComputeDipFeed_ScalesWithAmount()
    {
        var req = new float[] { 10f, 5f };
        DemandPlanner.ComputeDipFeed(req, 0.3f)
            .Should().BeEquivalentTo(
                new List<(int, float)> { (0, 3f), (1, 1.5f) },
                opt => opt.WithStrictOrdering());
        DemandPlanner.ComputeDipFeed(req, 0f).Should().BeEmpty();
        DemandPlanner.ComputeDipFeed(req, -1f).Should().BeEmpty();
        DemandPlanner.ComputeDipFeed(null, 0.3f).Should().BeEmpty();
    }

    [Fact]
    public void ComputeNewServiceIds_OnlyReportsFreshIds()
    {
        DemandPlanner.ComputeNewServiceIds(new List<int> { 0, 1 }, new int[] { 1, 2, 2, 3 })
            .Should().BeEquivalentTo(new List<int> { 2, 3 }, opt => opt.WithStrictOrdering());
        DemandPlanner.ComputeNewServiceIds(new List<int> { 0 }, null).Should().BeEmpty();
        DemandPlanner.ComputeNewServiceIds(new List<int> { 0, 1, 2 }, new int[] { 0, 1 })
            .Should().BeEmpty();
    }

    // --- HighEnd Tests ---

    [Theory]
    [InlineData(0, 0.1f, 1f)]
    [InlineData(1, 0.1f, 1.1f)]
    [InlineData(2, 0.1f, 1.2f)]
    [InlineData(33, 0.1f, 4.3f)]
    [InlineData(5, 0.2f, 2f)]
    [InlineData(-3, 0.1f, 1f)]
    [InlineData(5, 0f, 1f)]
    [InlineData(5, -0.5f, 1f)]
    public void HighEndMultiplier_MapsIdToFactor(int customerId, float step, float expected)
    {
        DemandPlanner.HighEndMultiplier(customerId, step).Should().BeApproximately(expected, 0.0001f);
    }

    [Fact]
    public void ComputeHighEndDrain_ScalesWithMultiplier()
    {
        var req = new float[] { 10f, 5f };
        var cur = new float[] { 10f, 5f };
        // mult 1.2 -> Marge 20 % der Anforderung (Float-Toleranz beachten).
        var drain = DemandPlanner.ComputeHighEndDrain(req, cur, 1.2f);
        drain.Should().HaveCount(2);
        drain[0].AppId.Should().Be(0);
        drain[0].Amount.Should().BeApproximately(2f, 0.001f);
        drain[1].AppId.Should().Be(1);
        drain[1].Amount.Should().BeApproximately(1f, 0.001f);
        // mult <= 1 -> kein Drain
        DemandPlanner.ComputeHighEndDrain(req, cur, 1f).Should().BeEmpty();
        DemandPlanner.ComputeHighEndDrain(req, cur, 0.5f).Should().BeEmpty();
    }

    [Fact]
    public void ComputeHighEndDrain_NeverBelowZero()
    {
        // Wenig vorhanden -> nur Rest abziehen, nie negativ.
        DemandPlanner.ComputeHighEndDrain(new float[] { 10f }, new float[] { 1f }, 2f)
            .Should().BeEquivalentTo(new List<(int, float)> { (0, 1f) });
        // Nichts vorhanden -> nichts.
        DemandPlanner.ComputeHighEndDrain(new float[] { 10f }, new float[] { 0f }, 2f)
            .Should().BeEmpty();
        DemandPlanner.ComputeHighEndDrain(null, new float[] { 1f }, 2f).Should().BeEmpty();
        DemandPlanner.ComputeHighEndDrain(new float[] { 1f }, null, 2f).Should().BeEmpty();
    }
}
