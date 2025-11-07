using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages context menu display and interaction.
/// </summary>
public class ContextMenuManager
{
    private ContextMenuInfo? _currentMenu;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenuManager"/> class.
    /// </summary>
    public ContextMenuManager()
    {
        IsEnabled = true;
    }

    /// <summary>
    /// Gets or sets whether context menus are enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets the current context menu (null if not visible).
    /// </summary>
    public ContextMenuInfo? CurrentMenu => _currentMenu;

    /// <summary>
    /// Gets whether a context menu is currently visible.
    /// </summary>
    public bool IsVisible => _currentMenu != null;

    /// <summary>
    /// Event raised when a context menu is requested.
    /// </summary>
    public event EventHandler<ContextMenuRequestedEventArgs>? MenuRequested;

    /// <summary>
    /// Event raised when a context menu item is selected.
    /// </summary>
    public event EventHandler<ContextMenuItemEventArgs>? ItemSelected;

    /// <summary>
    /// Shows a context menu.
    /// </summary>
    /// <param name="position">The position to show the menu at.</param>
    /// <param name="target">The target object (data point, series, etc.).</param>
    /// <param name="items">The menu items.</param>
    public void Show(SKPoint position, object? target, List<ContextMenuItem> items)
    {
        if (!IsEnabled || items.Count == 0)
            return;

        _currentMenu = new ContextMenuInfo
        {
            Position = position,
            Target = target,
            Items = items
        };

        OnMenuRequested();
    }

    /// <summary>
    /// Hides the current context menu.
    /// </summary>
    public void Hide()
    {
        _currentMenu = null;
    }

    /// <summary>
    /// Handles a click at the specified position.
    /// </summary>
    /// <param name="position">The click position.</param>
    /// <returns>True if a menu item was clicked.</returns>
    public bool HandleClick(SKPoint position)
    {
        if (_currentMenu == null)
            return false;

        // This is a simplified implementation
        // In a real scenario, you'd calculate item bounds and detect clicks
        // For now, we'll just hide the menu on any click
        Hide();
        return true;
    }

    /// <summary>
    /// Triggers a context menu request for a specific target.
    /// </summary>
    /// <param name="position">The position where the menu was requested.</param>
    /// <param name="target">The target object.</param>
    public void RequestMenu(SKPoint position, object? target)
    {
        if (!IsEnabled)
            return;

        var args = new ContextMenuRequestedEventArgs
        {
            Position = position,
            Target = target
        };

        OnMenuRequested(args);

        // If items were provided, show the menu
        if (args.Items.Count > 0)
        {
            Show(position, target, args.Items);
        }
    }

    /// <summary>
    /// Selects a menu item.
    /// </summary>
    /// <param name="item">The item to select.</param>
    public void SelectItem(ContextMenuItem item)
    {
        if (item == null || _currentMenu == null)
            return;

        ItemSelected?.Invoke(this, new ContextMenuItemEventArgs
        {
            Item = item,
            Target = _currentMenu.Target
        });

        // Execute item action
        item.Action?.Invoke();

        // Hide menu after selection
        Hide();
    }

    private void OnMenuRequested(ContextMenuRequestedEventArgs? args = null)
    {
        args ??= new ContextMenuRequestedEventArgs
        {
            Position = _currentMenu?.Position ?? SKPoint.Empty,
            Target = _currentMenu?.Target
        };

        MenuRequested?.Invoke(this, args);
    }
}

/// <summary>
/// Contains information about a context menu.
/// </summary>
public class ContextMenuInfo
{
    /// <summary>
    /// Gets or sets the position where the menu should appear.
    /// </summary>
    public required SKPoint Position { get; init; }

    /// <summary>
    /// Gets or sets the target object (data point, series, etc.).
    /// </summary>
    public object? Target { get; init; }

    /// <summary>
    /// Gets or sets the menu items.
    /// </summary>
    public required List<ContextMenuItem> Items { get; init; }
}

/// <summary>
/// Represents a context menu item.
/// </summary>
public class ContextMenuItem
{
    /// <summary>
    /// Gets or sets the item text.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Gets or sets the item icon (optional).
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Gets or sets whether the item is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// Gets or sets whether this is a separator.
    /// </summary>
    public bool IsSeparator { get; init; }

    /// <summary>
    /// Gets or sets the action to execute when selected.
    /// </summary>
    public Action? Action { get; init; }

    /// <summary>
    /// Gets or sets sub-items (for hierarchical menus).
    /// </summary>
    public List<ContextMenuItem>? SubItems { get; init; }

    /// <summary>
    /// Creates a separator menu item.
    /// </summary>
    public static ContextMenuItem Separator() => new ContextMenuItem
    {
        Text = string.Empty,
        IsSeparator = true
    };
}

/// <summary>
/// Event arguments for context menu requests.
/// </summary>
public class ContextMenuRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the position where the menu was requested.
    /// </summary>
    public required SKPoint Position { get; init; }

    /// <summary>
    /// Gets or sets the target object.
    /// </summary>
    public object? Target { get; init; }

    /// <summary>
    /// Gets the collection of menu items to display.
    /// Can be populated by event handlers.
    /// </summary>
    public List<ContextMenuItem> Items { get; } = new();
}

/// <summary>
/// Event arguments for context menu item selection.
/// </summary>
public class ContextMenuItemEventArgs : EventArgs
{
    /// <summary>
    /// Gets the selected menu item.
    /// </summary>
    public required ContextMenuItem Item { get; init; }

    /// <summary>
    /// Gets the target object.
    /// </summary>
    public object? Target { get; init; }
}
