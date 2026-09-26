/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     MCP server implementation.
/// Maintainer:  Provides HTTP API. Uses System.Text.Json.
/// </file-summary>

namespace gregCore.Infrastructure.Networking;

public sealed class GregMcpServer : IGregMcpServer
{
    public void Start(int port) { /* No-op stub: the MCP server transport is not implemented yet. The interface exists so callers can be wired before the transport lands. */ }
    public void Stop() { /* No-op stub: see Start. */ }
}
