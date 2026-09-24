/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Tutorial bridge: show, stop,
///               skip vanilla tutorials/videos. All best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregTutorials
{
    public static global::Il2Cpp.Tutorials GetInstance()
    {
        try { return global::Il2Cpp.Tutorials.instance; }
        catch { return null; }
    }

    public static bool ShowTutorial(int index)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.ShowTutorial(index);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShowTutorial failed: {Base(ex)}");
            return false;
        }
    }

    public static bool StopTutorial()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.StopTutorial();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"StopTutorial failed: {Base(ex)}");
            return false;
        }
    }

    public static bool SkipTutorials()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.SkipTutorials();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SkipTutorials failed: {Base(ex)}");
            return false;
        }
    }

    public static bool PlayVideo(int tutorialIndex, bool inPauseMenu)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.PlayVideo(tutorialIndex, inPauseMenu);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"PlayVideo failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ShowTutorialInPauseMenu(int index)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.ButtonShowTutorialInPauseMenu(index);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonShowTutorialInPauseMenu failed: {Base(ex)}");
            return false;
        }
    }

    public static bool StopVideoInPauseMenu()
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.StopVideoInPauseMenu();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"StopVideoInPauseMenu failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Tutorials: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
