/// <file-summary>
/// Layer:       Tests
/// Purpose:     Mock logger for unit tests.
/// Maintainer:   For use in tests only.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Linq;
using gregCore.Core.Abstractions;

namespace gregCore.Tests.Mocks;

public class MockLogger : IGregLogger
{
    public enum LogLevel { Debug, Info, Warning, Error }
    public List<(LogLevel Level, string Message)> Logs { get; } = new();

    public void Debug(string message) => Logs.Add((LogLevel.Debug, message));
    public void Info(string message) => Logs.Add((LogLevel.Info, message));

    public void Success(string message) => Logs.Add((LogLevel.Info, $"[SUCCESS] {message}"));

    public void Warning(string message) => Logs.Add((LogLevel.Warning, message));
    public void Error(string message) => Logs.Add((LogLevel.Error, message));
    public void Error(string message, Exception? ex) => Logs.Add((LogLevel.Error, $"{message} {ex?.Message}"));

    public IGregLogger ForContext(string context) => this;

    public bool AssertLogged(LogLevel level, string partialMessage) =>
        Logs.Any(l => l.Level == level && l.Message.Contains(partialMessage));
}
