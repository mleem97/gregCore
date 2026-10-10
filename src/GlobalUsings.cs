/// <file-summary>
/// Layer:       Core
/// Purpose:     Global usings and project rules
/// Maintainer:   Do not add external/Unity dependencies here.
/// </file-summary>

// SERIALIZER-REGEL:
// System.Text.Json  → Runtime (Persistence, MCP, Events)
// Newtonsoft.Json   → config files only
// DTOs must NOT have serializer-specific attributes!

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Text.Json;
global using System.Runtime.InteropServices;
global using gregCore.Core.Abstractions;
global using gregCore.Core.Models;
global using gregCore.Core.Events;
global using gregCore.Core.Exceptions;
