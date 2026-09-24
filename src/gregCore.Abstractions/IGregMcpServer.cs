/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for the MCP endpoint (Model Context Protocol).
/// Maintainer:   Exposes HTTP-based debug/mod interfaces.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregMcpServer
{
    void Start(int port);
    void Stop();
}
