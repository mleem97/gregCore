using System;

namespace greg.Sdk.Services;

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
