/// <file-summary>
/// Layer:       PublicApi
/// Purpose:      Attribute marking hook handlers in mods.
/// Maintainer:   Used by the EventBus for auto-registration.
/// </file-summary>

namespace gregCore.PublicApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GregHookAttribute : Attribute
{
    public string HookName { get; }
    public GregHookAttribute(string hookName) => HookName = hookName;
}
