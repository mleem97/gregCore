using System;
using MelonLoader;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.UI;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Haupt-Logger für gregCore. Formatiert Logs einheitlich für Terminal und In-Game Console.
/// </summary>
public sealed class ConsoleLogger : IGregLogger
{
    private readonly MelonLogger.Instance _melonLogger;
    private readonly string _context;

    public ConsoleLogger(MelonLogger.Instance melonLogger, string context = "")
    {
        _melonLogger = melonLogger ?? throw new ArgumentNullException(nameof(melonLogger));
        _context = context;
    }

    public void Info(string message) => GregLogger.Info(_context, message);
    public void Warning(string message) => GregLogger.Warning(_context, message);
    public void Error(string message, Exception? ex = null) 
    {
        string fullMessage = ex != null ? $"{message}\n{ex}" : message;
        GregLogger.Error(_context, fullMessage);
    }
    public void Debug(string message) => GregLogger.Debug(_context, message);
    public void Success(string message) => GregLogger.Success(_context, message);
    
    public void Bridge(string bridgeName, string message) => GregLogger.BridgeInfo(bridgeName, message);

    public IGregLogger ForContext(string context)
    {
        return new ConsoleLogger(_melonLogger, string.IsNullOrEmpty(_context) ? context : $"{_context}::{context}");
    }
}
