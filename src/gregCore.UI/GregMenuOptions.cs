/// <file-summary>
/// Schicht:      UI
/// Zweck:        Verhaltens-Optionen fuer registrierte Mod-Menues.
///               Moder registrieren ihr Menue einmal mit diesen Flags in der
///               GregMenuRegistry; Input-Lock, Cursor, Drag und Slide werden
///               dann zentral gesteuert (gregCore.UI Baukasten).
/// </file-summary>

namespace gregCore.UI;

public sealed class GregMenuOptions
{
    // Kamera-Look blockieren solange offen.
    public bool LockCamera { get; set; } = true;
    // Spieler-Bewegung (WASD etc.) blockieren.
    public bool LockMovement { get; set; } = true;
    // Welt-Interaktion per Blick-Ray blockieren.
    public bool LockInteract { get; set; } = true;
    // Mauszeiger zeigen + entriegeln.
    public bool ShowCursor { get; set; } = true;
    // Panel per Drag-Handle verschiebbar.
    public bool Draggable { get; set; } = false;
    // Panel slidet von rechts rein (Sidebar). Sonst sofort sichtbar.
    public bool SlideFromRight { get; set; } = false;
    // Panel-Breite in px.
    public float PanelWidth { get; set; } = 420f;

    public GregMenuOptions Clone() => (GregMenuOptions)MemberwiseClone();
}
