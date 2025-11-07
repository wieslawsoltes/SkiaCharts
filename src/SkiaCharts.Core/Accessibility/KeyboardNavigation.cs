namespace SkiaCharts.Core.Accessibility;

/// <summary>
/// Keyboard navigation support for charts.
/// </summary>
public class KeyboardNavigation
{
    private readonly List<INavigableElement> _elements = new();
    private int _focusedIndex = -1;

    /// <summary>
    /// Gets or sets whether keyboard navigation is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the currently focused element.
    /// </summary>
    public INavigableElement? FocusedElement =>
        _focusedIndex >= 0 && _focusedIndex < _elements.Count
            ? _elements[_focusedIndex]
            : null;

    /// <summary>
    /// Gets all navigable elements.
    /// </summary>
    public IReadOnlyList<INavigableElement> Elements => _elements;

    /// <summary>
    /// Registers a navigable element.
    /// </summary>
    public void RegisterElement(INavigableElement element)
    {
        if (!_elements.Contains(element))
        {
            _elements.Add(element);
        }
    }

    /// <summary>
    /// Unregisters a navigable element.
    /// </summary>
    public void UnregisterElement(INavigableElement element)
    {
        var index = _elements.IndexOf(element);
        if (index >= 0)
        {
            _elements.RemoveAt(index);

            if (_focusedIndex == index)
            {
                _focusedIndex = -1;
            }
            else if (_focusedIndex > index)
            {
                _focusedIndex--;
            }
        }
    }

    /// <summary>
    /// Clears all navigable elements.
    /// </summary>
    public void Clear()
    {
        _elements.Clear();
        _focusedIndex = -1;
    }

    /// <summary>
    /// Handles a keyboard event.
    /// </summary>
    /// <returns>True if the event was handled.</returns>
    public bool HandleKey(KeyboardKey key, KeyModifiers modifiers)
    {
        if (!IsEnabled || _elements.Count == 0)
            return false;

        switch (key)
        {
            case KeyboardKey.Tab:
                return modifiers.HasFlag(KeyModifiers.Shift) ? MoveToPrevious() : MoveToNext();

            case KeyboardKey.ArrowRight:
            case KeyboardKey.ArrowDown:
                return MoveToNext();

            case KeyboardKey.ArrowLeft:
            case KeyboardKey.ArrowUp:
                return MoveToPrevious();

            case KeyboardKey.Home:
                return MoveToFirst();

            case KeyboardKey.End:
                return MoveToLast();

            case KeyboardKey.Enter:
            case KeyboardKey.Space:
                return ActivateFocused();

            case KeyboardKey.Escape:
                return ClearFocus();

            default:
                // Let the focused element handle other keys
                return FocusedElement?.HandleKey(key, modifiers) ?? false;
        }
    }

    /// <summary>
    /// Moves focus to the next element.
    /// </summary>
    public bool MoveToNext()
    {
        if (_elements.Count == 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = (_focusedIndex + 1) % _elements.Count;

        UpdateFocus(oldIndex, _focusedIndex);
        return true;
    }

    /// <summary>
    /// Moves focus to the previous element.
    /// </summary>
    public bool MoveToPrevious()
    {
        if (_elements.Count == 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = _focusedIndex <= 0 ? _elements.Count - 1 : _focusedIndex - 1;

        UpdateFocus(oldIndex, _focusedIndex);
        return true;
    }

    /// <summary>
    /// Moves focus to the first element.
    /// </summary>
    public bool MoveToFirst()
    {
        if (_elements.Count == 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = 0;

        UpdateFocus(oldIndex, _focusedIndex);
        return true;
    }

    /// <summary>
    /// Moves focus to the last element.
    /// </summary>
    public bool MoveToLast()
    {
        if (_elements.Count == 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = _elements.Count - 1;

        UpdateFocus(oldIndex, _focusedIndex);
        return true;
    }

    /// <summary>
    /// Moves focus to a specific element.
    /// </summary>
    public bool MoveTo(INavigableElement element)
    {
        var index = _elements.IndexOf(element);
        if (index < 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = index;

        UpdateFocus(oldIndex, _focusedIndex);
        return true;
    }

    /// <summary>
    /// Activates the currently focused element.
    /// </summary>
    public bool ActivateFocused()
    {
        if (FocusedElement == null)
            return false;

        FocusedElement.Activate();
        return true;
    }

    /// <summary>
    /// Clears the current focus.
    /// </summary>
    public bool ClearFocus()
    {
        if (_focusedIndex < 0)
            return false;

        var oldIndex = _focusedIndex;
        _focusedIndex = -1;

        UpdateFocus(oldIndex, -1);
        return true;
    }

    private void UpdateFocus(int oldIndex, int newIndex)
    {
        if (oldIndex >= 0 && oldIndex < _elements.Count)
        {
            _elements[oldIndex].IsFocused = false;
            _elements[oldIndex].OnFocusLost();
        }

        if (newIndex >= 0 && newIndex < _elements.Count)
        {
            _elements[newIndex].IsFocused = true;
            _elements[newIndex].OnFocusGained();
        }
    }
}

/// <summary>
/// Interface for elements that support keyboard navigation.
/// </summary>
public interface INavigableElement
{
    /// <summary>
    /// Gets or sets whether this element is currently focused.
    /// </summary>
    bool IsFocused { get; set; }

    /// <summary>
    /// Gets whether this element can receive focus.
    /// </summary>
    bool CanFocus { get; }

    /// <summary>
    /// Gets the accessible name for screen readers.
    /// </summary>
    string AccessibleName { get; }

    /// <summary>
    /// Gets the accessible description.
    /// </summary>
    string? AccessibleDescription { get; }

    /// <summary>
    /// Called when this element gains focus.
    /// </summary>
    void OnFocusGained();

    /// <summary>
    /// Called when this element loses focus.
    /// </summary>
    void OnFocusLost();

    /// <summary>
    /// Activates this element (Enter/Space key).
    /// </summary>
    void Activate();

    /// <summary>
    /// Handles a keyboard key press.
    /// </summary>
    bool HandleKey(KeyboardKey key, KeyModifiers modifiers);
}

/// <summary>
/// Keyboard keys.
/// </summary>
public enum KeyboardKey
{
    None,
    Tab,
    Enter,
    Space,
    Escape,
    ArrowUp,
    ArrowDown,
    ArrowLeft,
    ArrowRight,
    Home,
    End,
    PageUp,
    PageDown,
    Plus,
    Minus,
    Delete,
    Backspace
}

/// <summary>
/// Keyboard modifiers.
/// </summary>
[Flags]
public enum KeyModifiers
{
    None = 0,
    Shift = 1,
    Control = 2,
    Alt = 4,
    Meta = 8
}

/// <summary>
/// Base implementation of a navigable element.
/// </summary>
public abstract class NavigableElementBase : INavigableElement
{
    /// <summary>
    /// Gets or sets whether this element is focused.
    /// </summary>
    public bool IsFocused { get; set; }

    /// <summary>
    /// Gets whether this element can receive focus.
    /// </summary>
    public virtual bool CanFocus => true;

    /// <summary>
    /// Gets the accessible name.
    /// </summary>
    public abstract string AccessibleName { get; }

    /// <summary>
    /// Gets the accessible description.
    /// </summary>
    public virtual string? AccessibleDescription => null;

    /// <summary>
    /// Called when gaining focus.
    /// </summary>
    public virtual void OnFocusGained()
    {
        // Default: do nothing
    }

    /// <summary>
    /// Called when losing focus.
    /// </summary>
    public virtual void OnFocusLost()
    {
        // Default: do nothing
    }

    /// <summary>
    /// Activates the element.
    /// </summary>
    public virtual void Activate()
    {
        // Default: do nothing
    }

    /// <summary>
    /// Handles a keyboard key.
    /// </summary>
    public virtual bool HandleKey(KeyboardKey key, KeyModifiers modifiers)
    {
        return false;
    }
}

/// <summary>
/// Keyboard shortcuts manager.
/// </summary>
public class KeyboardShortcuts
{
    private readonly Dictionary<KeyBinding, Action> _shortcuts = new();

    /// <summary>
    /// Registers a keyboard shortcut.
    /// </summary>
    public void Register(KeyboardKey key, KeyModifiers modifiers, Action action)
    {
        var binding = new KeyBinding(key, modifiers);
        _shortcuts[binding] = action;
    }

    /// <summary>
    /// Unregisters a keyboard shortcut.
    /// </summary>
    public void Unregister(KeyboardKey key, KeyModifiers modifiers)
    {
        var binding = new KeyBinding(key, modifiers);
        _shortcuts.Remove(binding);
    }

    /// <summary>
    /// Handles a keyboard event.
    /// </summary>
    public bool Handle(KeyboardKey key, KeyModifiers modifiers)
    {
        var binding = new KeyBinding(key, modifiers);

        if (_shortcuts.TryGetValue(binding, out var action))
        {
            action();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears all shortcuts.
    /// </summary>
    public void Clear()
    {
        _shortcuts.Clear();
    }

    /// <summary>
    /// Gets the count of registered shortcuts.
    /// </summary>
    public int Count => _shortcuts.Count;

    internal record KeyBinding(KeyboardKey Key, KeyModifiers Modifiers);
}

/// <summary>
/// Standard keyboard shortcuts for charts.
/// </summary>
public static class StandardShortcuts
{
    /// <summary>
    /// Zoom in (Ctrl/Cmd + Plus).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) ZoomIn =>
        (KeyboardKey.Plus, KeyModifiers.Control);

    /// <summary>
    /// Zoom out (Ctrl/Cmd + Minus).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) ZoomOut =>
        (KeyboardKey.Minus, KeyModifiers.Control);

    /// <summary>
    /// Reset zoom (Ctrl/Cmd + 0).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) ResetZoom =>
        (KeyboardKey.None, KeyModifiers.Control);

    /// <summary>
    /// Pan left (Arrow Left).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) PanLeft =>
        (KeyboardKey.ArrowLeft, KeyModifiers.None);

    /// <summary>
    /// Pan right (Arrow Right).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) PanRight =>
        (KeyboardKey.ArrowRight, KeyModifiers.None);

    /// <summary>
    /// Pan up (Arrow Up).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) PanUp =>
        (KeyboardKey.ArrowUp, KeyModifiers.None);

    /// <summary>
    /// Pan down (Arrow Down).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) PanDown =>
        (KeyboardKey.ArrowDown, KeyModifiers.None);

    /// <summary>
    /// Toggle legend (L).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) ToggleLegend =>
        (KeyboardKey.None, KeyModifiers.None); // Would need 'L' key

    /// <summary>
    /// Toggle tooltip (T).
    /// </summary>
    public static (KeyboardKey Key, KeyModifiers Modifiers) ToggleTooltip =>
        (KeyboardKey.None, KeyModifiers.None); // Would need 'T' key
}
