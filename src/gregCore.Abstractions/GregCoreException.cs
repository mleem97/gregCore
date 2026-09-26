/// <file-summary>
/// Layer:       Core
/// Purpose:      Base exception for all framework-owned exceptions.
/// Maintainer:   All custom exceptions should inherit from it.
/// </file-summary>

namespace gregCore.Core.Exceptions;

public abstract class GregCoreException : Exception
{
    protected GregCoreException(string message) : base(message) { }
    protected GregCoreException(string message, Exception inner) : base(message, inner) { }
}
