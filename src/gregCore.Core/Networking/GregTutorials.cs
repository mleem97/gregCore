/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Tutorial-Brücke: Vanilla-Tutorials/Videos zeigen, stoppen,
///               überspringen. Alles best-effort.
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
            Warn($"ShowTutorial fehlgeschlagen: {Base(ex)}");
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
            Warn($"StopTutorial fehlgeschlagen: {Base(ex)}");
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
            Warn($"SkipTutorials fehlgeschlagen: {Base(ex)}");
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
            Warn($"PlayVideo fehlgeschlagen: {Base(ex)}");
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
            Warn($"ButtonShowTutorialInPauseMenu fehlgeschlagen: {Base(ex)}");
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
            Warn($"StopVideoInPauseMenu fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Tutorials: {message}"); } catch { }
    }
}
