/// <file-summary>
/// Layer:      UI
/// Purpose:     Behavior options for registered mod menus.
///              Mods register their menu once with these flags in
///              GregMenuRegistry; input lock, cursor, drag and slide are
///              then controlled centrally (gregCore.UI kit).
/// </file-summary>

namespace gregCore.UI;

public sealed class GregMenuOptions
{
    // Block camera look while open.
    public bool LockCamera { get; set; } = true;
    // Block player movement (WASD etc.).
    public bool LockMovement { get; set; } = true;
    // Block world interaction via look ray.
    public bool LockInteract { get; set; } = true;
    // Show + unlock mouse cursor.
    public bool ShowCursor { get; set; } = true;
    // Panel movable via drag handle.
    public bool Draggable { get; set; } = false;
    // Panel slides in from the right (sidebar). Otherwise visible immediately.
    public bool SlideFromRight { get; set; } = false;
    // Panel width in px.
    public float PanelWidth { get; set; } = 420f;

    public GregMenuOptions Clone() => (GregMenuOptions)MemberwiseClone();
}
