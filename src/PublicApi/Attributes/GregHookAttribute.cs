/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Attribut zur Markierung von Hook-Handlern in Mods.
/// Maintainer:   Wird vom EventBus zur Auto-Registrierung genutzt.
/// </file-summary>

namespace gregCore.PublicApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class GregHookAttribute : Attribute
{
    public string HookName { get; }
    public GregHookAttribute(string hookName) => HookName = hookName;
}
