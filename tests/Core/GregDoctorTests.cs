using gregCore.Core.Diagnostics;
using Xunit;
using FluentAssertions;

namespace gregCore.Tests.Core;

public sealed class GregDoctorTests
{
    [Fact]
    public void CreateWithoutGameFiles_ShouldReportUnsupportedBuild()
    {
        var report = GregDoctor.Create(Path.Combine(Path.GetTempPath(), "gregcore-no-game"), "missing-manifest.json", "test.log");
        report.Status.Should().Be("UNSUPPORTED_GAME_BUILD");
        report.ErrorCode.Should().Be("UNSUPPORTED_GAME_BUILD");
        report.Recommendations.Should().NotBeEmpty();
    }
}
