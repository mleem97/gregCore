using Xunit;
using gregSdk;
using gregSdk.Definitions;
using gregSdk.Registries;
using gregSdk.Validators;

namespace gregTests;

public class ContentTests
{
    [Fact]
    public void ServerValidation_ShouldFail_WhenUnitsInvalid()
    {
        var validator = new ServerValidator();
        var server = new ServerDefinition { Id = "test", RackUnits = 0 };
        
        var result = validator.Validate(server, out var error);
        
        Assert.False(result);
        Assert.Contains("RackUnits", error);
    }

    [Fact]
    public void Registry_ShouldEmitEvent_WhenRegistered()
    {
        string capturedId = null;
        gregEventDispatcher.On(gregNativeEventHooks.ContentRegistered, payload => {
            capturedId = gregPayload.Get<string>(payload, "Id");
        });

        var registry = new GregServerRegistry();
        registry.Register(new ServerDefinition { Id = "event-test" });

        Assert.Equal("event-test", capturedId);
    }

    [Fact]
    public void NetworkCompatibility_ShouldValidateCorrectly()
    {
        var switches = new GregSwitchRegistry();
        var sfps = new GregSfpRegistry();
        var cables = new GregCableRegistry();

        switches.Register(new SwitchDefinition { Id = "sw1", SupportedSfpProfiles = new[] { "10G" } });
        sfps.Register(new SfpDefinition { Id = "sfp1", SpeedGbps = 10, CompatibilityTags = new[] { "10G" } });
        cables.Register(new CableDefinition { Id = "c1", MaxSpeedGbps = 10 });

        GregNetworkCompatibilityService.Initialize(switches, sfps, cables);
        
        var canLink = GregNetworkCompatibilityService.CanLink("sw1", "sfp1", "c1", out var reason);
        
        Assert.True(canLink, reason);
    }

    [Fact]
    public void CustomerPolicy_ShouldEnforceEnterpriseGrade()
    {
        var customer = new CustomerDefinition { Id = "c1", Segment = "Enterprise" };
        var server = new ServerDefinition { Id = "s1", Tags = new[] { "ConsumerGrade" } };

        var canAssign = GregCustomerPolicyEngine.CanAssignToServer(customer, server, out var reason);

        Assert.False(canAssign);
        Assert.Contains("Enterprise-grade", reason);
    }
}

