using System;

namespace gregSdk.Services;

public static class GregDiagnostics
{
    public static void LogContentError(string category, string id, string message)
    {
        Console.WriteLine($"[GregCore][ContentError][{category}] {id}: {message}");
    }

    public static void LogContentWarning(string category, string id, string message)
    {
        Console.WriteLine($"[GregCore][ContentWarning][{category}] {id}: {message}");
    }
}
