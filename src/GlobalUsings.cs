/// <file-summary>
/// Schicht:      Core
/// Zweck:        Globale Usings und Projekt-Regeln
/// Maintainer:   Keine externen/Unity Abhängigkeiten hier einfügen.
/// </file-summary>

// SERIALIZER-REGEL:
// System.Text.Json  → Runtime (Persistence, MCP, Events)
// Newtonsoft.Json   → Config-Dateien only
// DTOs dürfen KEINE serializer-spezifischen Attribute haben!

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
