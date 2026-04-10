using System;
using System.Text;

namespace gregFramework.Core;

/// <summary>
/// Builds canonical greg hook strings: greg.&lt;DOMAIN&gt;.&lt;Action&gt;
/// </summary>
public static class GregHookName
{
    private const string Prefix = "greg";

    public static string Create(GregDomain domain, string action)
        => Create(domain, action, null);

    public static string Create(GregDomain domain, string action, string subject)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required.", nameof(action));

        var domainPart = DomainToSegment(domain);
        var sb = new StringBuilder(Prefix.Length + 1 + domainPart.Length + 1 + action.Length + 32);
        sb.Append(Prefix).Append('.').Append(domainPart).Append('.').Append(action.Trim());

        if (!string.IsNullOrWhiteSpace(subject))
            sb.Append('.').Append(subject.Trim());

        return sb.ToString();
    }

    private static string DomainToSegment(GregDomain domain) => domain switch
    {
        GregDomain.Gameplay => "GAMEPLAY",
        GregDomain.Player => "PLAYER",
        GregDomain.Employee => "EMPLOYEE",
        GregDomain.Customer => "CUSTOMER",
        GregDomain.Server => "SERVER",
        GregDomain.Rack => "RACK",
        GregDomain.Network => "NETWORK",
        GregDomain.Power => "POWER",
        GregDomain.Ui => "UI",
        GregDomain.System => "SYSTEM",
        _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
    };
}
