using System.Collections.Generic;
using System.Linq;
using gregSdk.Definitions;

namespace gregSdk;

public static class GregCustomerPolicyEngine
{
    public static bool EvaluateRequirement(CustomerDefinition customer, string ruleId, object payload, out string error)
    {
        error = string.Empty;
        
        // Placeholder for real rule evaluation
        // In a real implementation, this would look up ruleId in a Registry of Rules
        
        switch (ruleId)
        {
            case "MinimumIOPS":
                if (payload is int iops && iops < 1000) { error = "IOPS requirement not met."; return false; }
                break;
            case "RequiredSpeed":
                if (payload is int speed && speed < 10) { error = "Network speed requirement not met."; return false; }
                break;
        }

        return true;
    }

    public static bool CanAssignToServer(CustomerDefinition customer, ServerDefinition server, out string reason)
    {
        reason = string.Empty;
        
        // Check segment compatibility
        if (customer.Segment == "Enterprise" && !server.Tags.Contains("EnterpriseGrade"))
        {
            reason = "Customer requires Enterprise-grade hardware.";
            return false;
        }

        return true;
    }
}
