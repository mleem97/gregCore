using System;
using System.Reflection;

namespace gregCore.Bridge.CSharpScript;

/// <summary>
/// Holds runtime state for a single compiled C# mod.
/// </summary>
internal sealed class GregCSharpModContext
{
    public string Id { get; init; } = "";
    public string Directory { get; init; } = "";
    public Assembly Assembly { get; init; } = null!;
    public IGregCSharpMod Instance { get; init; } = null!;
    public bool Initialized { get; set; }

    public void SafeCall(Action action, string label)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Error($"[CSharpMod:{Id}] {label} failed: {ex.Message}");
        }
    }
}
