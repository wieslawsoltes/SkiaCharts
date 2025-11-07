namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages keyboard shortcuts and their actions.
/// </summary>
public class KeyboardShortcutManager
{
    private readonly Dictionary<KeyboardShortcut, Action> _shortcuts;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardShortcutManager"/> class.
    /// </summary>
    public KeyboardShortcutManager()
    {
        _shortcuts = new Dictionary<KeyboardShortcut, Action>();
    }

    /// <summary>
    /// Registers a keyboard shortcut.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="action">The action to execute.</param>
    public void Register(Key key, KeyModifiers modifiers, Action action)
    {
        var shortcut = new KeyboardShortcut(key, modifiers);
        _shortcuts[shortcut] = action;
    }

    /// <summary>
    /// Unregisters a keyboard shortcut.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    public void Unregister(Key key, KeyModifiers modifiers)
    {
        var shortcut = new KeyboardShortcut(key, modifiers);
        _shortcuts.Remove(shortcut);
    }

    /// <summary>
    /// Processes a keyboard event and executes matching shortcuts.
    /// </summary>
    /// <param name="keyboardEvent">The keyboard event.</param>
    /// <returns>True if a shortcut was executed.</returns>
    public bool ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        if (keyboardEvent.EventType != KeyboardEventType.KeyDown)
            return false;

        var shortcut = new KeyboardShortcut(keyboardEvent.Key, keyboardEvent.Modifiers);

        if (_shortcuts.TryGetValue(shortcut, out var action))
        {
            action.Invoke();
            keyboardEvent.Handled = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears all registered shortcuts.
    /// </summary>
    public void Clear()
    {
        _shortcuts.Clear();
    }

    /// <summary>
    /// Gets all registered shortcuts.
    /// </summary>
    public IReadOnlyDictionary<KeyboardShortcut, Action> Shortcuts => _shortcuts;
}

/// <summary>
/// Represents a keyboard shortcut (key + modifiers).
/// </summary>
public readonly struct KeyboardShortcut : IEquatable<KeyboardShortcut>
{
    /// <summary>
    /// Gets the key.
    /// </summary>
    public Key Key { get; }

    /// <summary>
    /// Gets the modifier keys.
    /// </summary>
    public KeyModifiers Modifiers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardShortcut"/> struct.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    public KeyboardShortcut(Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        Key = key;
        Modifiers = modifiers;
    }

    /// <inheritdoc/>
    public bool Equals(KeyboardShortcut other)
    {
        return Key == other.Key && Modifiers == other.Modifiers;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is KeyboardShortcut other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Modifiers);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var parts = new List<string>();

        if (Modifiers.HasFlag(KeyModifiers.Control))
            parts.Add("Ctrl");
        if (Modifiers.HasFlag(KeyModifiers.Shift))
            parts.Add("Shift");
        if (Modifiers.HasFlag(KeyModifiers.Alt))
            parts.Add("Alt");
        if (Modifiers.HasFlag(KeyModifiers.Meta))
            parts.Add("Meta");

        parts.Add(Key.ToString());

        return string.Join("+", parts);
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(KeyboardShortcut left, KeyboardShortcut right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(KeyboardShortcut left, KeyboardShortcut right)
    {
        return !left.Equals(right);
    }
}

/// <summary>
/// Provides common keyboard shortcuts for charts.
/// </summary>
public static class CommonShortcuts
{
    /// <summary>Zoom in (Ctrl/Cmd + Plus).</summary>
    public static KeyboardShortcut ZoomIn => new(Key.Plus, KeyModifiers.Control);

    /// <summary>Zoom out (Ctrl/Cmd + Minus).</summary>
    public static KeyboardShortcut ZoomOut => new(Key.Minus, KeyModifiers.Control);

    /// <summary>Reset zoom (Ctrl/Cmd + 0).</summary>
    public static KeyboardShortcut ResetZoom => new(Key.D0, KeyModifiers.Control);

    /// <summary>Pan left (Left arrow).</summary>
    public static KeyboardShortcut PanLeft => new(Key.Left);

    /// <summary>Pan right (Right arrow).</summary>
    public static KeyboardShortcut PanRight => new(Key.Right);

    /// <summary>Pan up (Up arrow).</summary>
    public static KeyboardShortcut PanUp => new(Key.Up);

    /// <summary>Pan down (Down arrow).</summary>
    public static KeyboardShortcut PanDown => new(Key.Down);

    /// <summary>Home (go to beginning).</summary>
    public static KeyboardShortcut Home => new(Key.Home);

    /// <summary>End (go to end).</summary>
    public static KeyboardShortcut End => new(Key.End);

    /// <summary>Page up.</summary>
    public static KeyboardShortcut PageUp => new(Key.PageUp);

    /// <summary>Page down.</summary>
    public static KeyboardShortcut PageDown => new(Key.PageDown);

    /// <summary>Select all (Ctrl/Cmd + A).</summary>
    public static KeyboardShortcut SelectAll => new(Key.A, KeyModifiers.Control);

    /// <summary>Escape (cancel operation).</summary>
    public static KeyboardShortcut Cancel => new(Key.Escape);

    /// <summary>Enter (confirm operation).</summary>
    public static KeyboardShortcut Confirm => new(Key.Enter);
}
