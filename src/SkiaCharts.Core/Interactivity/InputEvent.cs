using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Base class for all input events.
/// </summary>
public abstract class InputEvent
{
    /// <summary>
    /// Gets or sets whether the event has been handled.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.Now;
}

/// <summary>
/// Represents a mouse event.
/// </summary>
public class MouseEvent : InputEvent
{
    /// <summary>
    /// Gets the mouse position in screen coordinates.
    /// </summary>
    public SKPoint Position { get; init; }

    /// <summary>
    /// Gets the mouse button that triggered the event.
    /// </summary>
    public MouseButton Button { get; init; }

    /// <summary>
    /// Gets the type of mouse event.
    /// </summary>
    public MouseEventType EventType { get; init; }

    /// <summary>
    /// Gets the modifier keys pressed during the event.
    /// </summary>
    public KeyModifiers Modifiers { get; init; }

    /// <summary>
    /// Gets the mouse wheel delta for scroll events.
    /// </summary>
    public float WheelDelta { get; init; }

    /// <summary>
    /// Gets the number of clicks (for click events).
    /// </summary>
    public int ClickCount { get; init; } = 1;
}

/// <summary>
/// Represents a touch event.
/// </summary>
public class TouchEvent : InputEvent
{
    /// <summary>
    /// Gets the list of touch points.
    /// </summary>
    public IReadOnlyList<TouchPoint> TouchPoints { get; init; } = Array.Empty<TouchPoint>();

    /// <summary>
    /// Gets the type of touch event.
    /// </summary>
    public TouchEventType EventType { get; init; }

    /// <summary>
    /// Gets the primary touch point (first touch).
    /// </summary>
    public TouchPoint? PrimaryTouchPoint => TouchPoints.FirstOrDefault();
}

/// <summary>
/// Represents a single touch point.
/// </summary>
public class TouchPoint
{
    /// <summary>
    /// Gets the unique identifier for this touch.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the touch position in screen coordinates.
    /// </summary>
    public SKPoint Position { get; init; }

    /// <summary>
    /// Gets the touch pressure (0.0 to 1.0).
    /// </summary>
    public float Pressure { get; init; } = 1.0f;

    /// <summary>
    /// Gets the touch radius (for contact area).
    /// </summary>
    public float Radius { get; init; }

    /// <summary>
    /// Gets the touch state.
    /// </summary>
    public TouchState State { get; init; }
}

/// <summary>
/// Represents a keyboard event.
/// </summary>
public class KeyboardEvent : InputEvent
{
    /// <summary>
    /// Gets the key that triggered the event.
    /// </summary>
    public Key Key { get; init; }

    /// <summary>
    /// Gets the type of keyboard event.
    /// </summary>
    public KeyboardEventType EventType { get; init; }

    /// <summary>
    /// Gets the modifier keys pressed during the event.
    /// </summary>
    public KeyModifiers Modifiers { get; init; }

    /// <summary>
    /// Gets the character representation of the key (if applicable).
    /// </summary>
    public char? Character { get; init; }
}

/// <summary>
/// Represents a gesture event.
/// </summary>
public class GestureEvent : InputEvent
{
    /// <summary>
    /// Gets the type of gesture.
    /// </summary>
    public GestureType GestureType { get; init; }

    /// <summary>
    /// Gets the center point of the gesture.
    /// </summary>
    public SKPoint Center { get; init; }

    /// <summary>
    /// Gets the scale factor for pinch gestures.
    /// </summary>
    public float Scale { get; init; } = 1.0f;

    /// <summary>
    /// Gets the rotation angle in radians for rotation gestures.
    /// </summary>
    public float Rotation { get; init; }

    /// <summary>
    /// Gets the translation delta for pan gestures.
    /// </summary>
    public SKPoint Delta { get; init; }

    /// <summary>
    /// Gets the velocity for swipe gestures.
    /// </summary>
    public SKPoint Velocity { get; init; }

    /// <summary>
    /// Gets the gesture state.
    /// </summary>
    public GestureState State { get; init; }
}

/// <summary>
/// Mouse button enumeration.
/// </summary>
public enum MouseButton
{
    /// <summary>No button.</summary>
    None = 0,
    /// <summary>Left mouse button.</summary>
    Left = 1,
    /// <summary>Middle mouse button.</summary>
    Middle = 2,
    /// <summary>Right mouse button.</summary>
    Right = 3,
    /// <summary>Mouse button 4 (back).</summary>
    XButton1 = 4,
    /// <summary>Mouse button 5 (forward).</summary>
    XButton2 = 5
}

/// <summary>
/// Mouse event type enumeration.
/// </summary>
public enum MouseEventType
{
    /// <summary>Mouse moved.</summary>
    Move,
    /// <summary>Mouse button pressed.</summary>
    Down,
    /// <summary>Mouse button released.</summary>
    Up,
    /// <summary>Mouse clicked.</summary>
    Click,
    /// <summary>Mouse double-clicked.</summary>
    DoubleClick,
    /// <summary>Mouse wheel scrolled.</summary>
    Wheel,
    /// <summary>Mouse entered the area.</summary>
    Enter,
    /// <summary>Mouse left the area.</summary>
    Leave
}

/// <summary>
/// Touch event type enumeration.
/// </summary>
public enum TouchEventType
{
    /// <summary>Touch started.</summary>
    Begin,
    /// <summary>Touch moved.</summary>
    Move,
    /// <summary>Touch ended.</summary>
    End,
    /// <summary>Touch cancelled.</summary>
    Cancel
}

/// <summary>
/// Touch state enumeration.
/// </summary>
public enum TouchState
{
    /// <summary>Touch just began.</summary>
    Began,
    /// <summary>Touch moved.</summary>
    Moved,
    /// <summary>Touch stationary.</summary>
    Stationary,
    /// <summary>Touch ended.</summary>
    Ended,
    /// <summary>Touch cancelled.</summary>
    Cancelled
}

/// <summary>
/// Keyboard event type enumeration.
/// </summary>
public enum KeyboardEventType
{
    /// <summary>Key pressed down.</summary>
    KeyDown,
    /// <summary>Key released.</summary>
    KeyUp,
    /// <summary>Character typed.</summary>
    CharInput
}

/// <summary>
/// Key enumeration (common keys).
/// </summary>
public enum Key
{
    /// <summary>Unknown key.</summary>
    Unknown = 0,
    /// <summary>Escape key.</summary>
    Escape,
    /// <summary>Enter/Return key.</summary>
    Enter,
    /// <summary>Tab key.</summary>
    Tab,
    /// <summary>Backspace key.</summary>
    Backspace,
    /// <summary>Delete key.</summary>
    Delete,
    /// <summary>Left arrow key.</summary>
    Left,
    /// <summary>Right arrow key.</summary>
    Right,
    /// <summary>Up arrow key.</summary>
    Up,
    /// <summary>Down arrow key.</summary>
    Down,
    /// <summary>Home key.</summary>
    Home,
    /// <summary>End key.</summary>
    End,
    /// <summary>Page Up key.</summary>
    PageUp,
    /// <summary>Page Down key.</summary>
    PageDown,
    /// <summary>Space key.</summary>
    Space,
    /// <summary>Plus key.</summary>
    Plus,
    /// <summary>Minus key.</summary>
    Minus,
    /// <summary>Zero key.</summary>
    D0,
    /// <summary>A key.</summary>
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    /// <summary>F1 key.</summary>
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12
}

/// <summary>
/// Key modifier flags.
/// </summary>
[Flags]
public enum KeyModifiers
{
    /// <summary>No modifiers.</summary>
    None = 0,
    /// <summary>Shift key.</summary>
    Shift = 1,
    /// <summary>Control key.</summary>
    Control = 2,
    /// <summary>Alt key.</summary>
    Alt = 4,
    /// <summary>Meta/Command key.</summary>
    Meta = 8
}

/// <summary>
/// Gesture type enumeration.
/// </summary>
public enum GestureType
{
    /// <summary>Tap gesture.</summary>
    Tap,
    /// <summary>Double tap gesture.</summary>
    DoubleTap,
    /// <summary>Long press gesture.</summary>
    LongPress,
    /// <summary>Pan gesture.</summary>
    Pan,
    /// <summary>Pinch gesture.</summary>
    Pinch,
    /// <summary>Rotation gesture.</summary>
    Rotation,
    /// <summary>Swipe gesture.</summary>
    Swipe
}

/// <summary>
/// Gesture state enumeration.
/// </summary>
public enum GestureState
{
    /// <summary>Gesture began.</summary>
    Began,
    /// <summary>Gesture updated.</summary>
    Changed,
    /// <summary>Gesture ended.</summary>
    Ended,
    /// <summary>Gesture cancelled.</summary>
    Cancelled
}
