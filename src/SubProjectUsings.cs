/// <file-summary>
/// Schicht:      Build (alle Teilprojekte)
/// Zweck:        Gemeinsame BCL-globale-Usings fuer alle gregCore.*-Teilprojekte.
///               Echte Source-Datei (kein ImplicitUsings): generierte
///               Usings-Dateien vergiften den Nullable-Kontext (CS0656
///               gegen Il2Cpp-Metadaten). Gleicher Mechanismus wie
///               src/GlobalUsings.cs im Hauptprojekt.
///               gregCore-Namespaces stehen pro Projekt in GlobalUsings.cs.
/// </file-summary>

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Text.Json;
global using System.Runtime.InteropServices;
