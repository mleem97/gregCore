using System;
using System.Collections.Concurrent;
using System.Text;

namespace greg.Sdk;

/// <summary>
/// Builds canonical greg hook strings: greg.&lt;DOMAIN&gt;.&lt;Action&gt;
/// </summary>
public static class gregHookName
{
    private const string Prefix = "greg";
    private static readonly ConcurrentDictionary<(GregDomain Domain, string Action), string> NoSubjectCache = new();

    public static string Create(GregDomain domain, string action)
        => Create(domain, action, null);

    public static string Create(GregDomain domain, string action, string subject)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required.", nameof(action));

        var normalizedAction = action.Trim();

        if (string.IsNullOrWhiteSpace(subject))
        {
            return NoSubjectCache.GetOrAdd((domain, normalizedAction), static key =>
            {
                var domainPart = DomainToSegment(key.Domain);
                var sb = new StringBuilder(Prefix.Length + 1 + domainPart.Length + 1 + key.Action.Length);
                sb.Append(Prefix).Append('.').Append(domainPart).Append('.').Append(key.Action);
                return sb.ToString();
            });
        }

        var domainPart = DomainToSegment(domain);
        var normalizedSubject = subject.Trim();
        var sb = new StringBuilder(Prefix.Length + 1 + domainPart.Length + 1 + normalizedAction.Length + 1 + normalizedSubject.Length);
        sb.Append(Prefix).Append('.').Append(domainPart).Append('.').Append(normalizedAction).Append('.').Append(normalizedSubject);

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



