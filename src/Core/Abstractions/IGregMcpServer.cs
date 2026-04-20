/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für den MCP-Endpunkt (Model Context Protocol).
/// Maintainer:   Bereitstellung von HTTP-basierten Debug/Mod-Schnittstellen.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregMcpServer
{
    void Start(int port);
    void Stop();
}
