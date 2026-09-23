/// <file-summary>
/// Schicht:      Core
/// Zweck:        Internals (z.B. DemandScanResult-Zustand) fuer die
///               Unit-Tests sichtbar machen. Keine Laufzeitwirkung.
/// </file-summary>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("gregCore.Tests")]
