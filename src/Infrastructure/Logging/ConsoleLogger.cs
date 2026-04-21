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

    public void Info(string message) => greg.Logging.GregLogger.Msg(message, _context);
    public void Warning(string message) => greg.Logging.GregLogger.Warn(message, _context);
    public void Error(string message, Exception? ex = null) 
    {
        greg.Logging.GregLogger.Error(message, ex, _context);
    }
    public void Debug(string message) => greg.Logging.GregLogger.Debug(message, _context);
    public void Success(string message) => greg.Logging.GregLogger.Msg(message, _context);
    
    public void Bridge(string bridgeName, string message) => greg.Logging.GregLogger.Msg(message, bridgeName);

    public IGregLogger ForContext(string context)
    {
        return new ConsoleLogger(_melonLogger, string.IsNullOrEmpty(_context) ? context : $"{_context}::{context}");
    }
}
