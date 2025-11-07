using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages selection of chart elements.
/// </summary>
public class SelectionManager
{
    private readonly HashSet<object> _selectedItems;
    private SelectionRange? _selectedRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionManager"/> class.
    /// </summary>
    public SelectionManager()
    {
        _selectedItems = new HashSet<object>();
        SelectionMode = SelectionMode.Single;
        IsEnabled = true;
    }

    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    public SelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets whether selection is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether to clear selection on background click.
    /// </summary>
    public bool ClearOnBackgroundClick { get; set; } = true;

    /// <summary>
    /// Gets the currently selected items.
    /// </summary>
    public IReadOnlyCollection<object> SelectedItems => _selectedItems;

    /// <summary>
    /// Gets the currently selected range (if any).
    /// </summary>
    public SelectionRange? SelectedRange => _selectedRange;

    /// <summary>
    /// Gets whether any items are selected.
    /// </summary>
    public bool HasSelection => _selectedItems.Count > 0 || _selectedRange != null;

    /// <summary>
    /// Event raised when the selection changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Selects an item.
    /// </summary>
    /// <param name="item">The item to select.</param>
    /// <param name="addToSelection">Whether to add to existing selection (multi-select mode).</param>
    /// <returns>True if the selection changed.</returns>
    public bool Select(object item, bool addToSelection = false)
    {
        if (!IsEnabled || item == null)
            return false;

        var changed = false;

        if (SelectionMode == SelectionMode.Single || !addToSelection)
        {
            if (_selectedItems.Count != 1 || !_selectedItems.Contains(item))
            {
                _selectedItems.Clear();
                _selectedItems.Add(item);
                _selectedRange = null;
                changed = true;
            }
        }
        else if (SelectionMode == SelectionMode.Multiple)
        {
            if (_selectedItems.Add(item))
            {
                _selectedRange = null;
                changed = true;
            }
        }

        if (changed)
        {
            OnSelectionChanged();
        }

        return changed;
    }

    /// <summary>
    /// Selects multiple items.
    /// </summary>
    /// <param name="items">The items to select.</param>
    /// <param name="addToSelection">Whether to add to existing selection.</param>
    /// <returns>True if the selection changed.</returns>
    public bool SelectMultiple(IEnumerable<object> items, bool addToSelection = false)
    {
        if (!IsEnabled || SelectionMode == SelectionMode.None)
            return false;

        if (!addToSelection)
        {
            _selectedItems.Clear();
        }

        var changed = false;
        foreach (var item in items)
        {
            if (item != null && _selectedItems.Add(item))
            {
                changed = true;
            }
        }

        if (changed)
        {
            _selectedRange = null;
            OnSelectionChanged();
        }

        return changed;
    }

    /// <summary>
    /// Deselects an item.
    /// </summary>
    /// <param name="item">The item to deselect.</param>
    /// <returns>True if the selection changed.</returns>
    public bool Deselect(object item)
    {
        if (!IsEnabled || item == null)
            return false;

        if (_selectedItems.Remove(item))
        {
            OnSelectionChanged();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Toggles the selection state of an item.
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    /// <returns>True if the item is now selected.</returns>
    public bool Toggle(object item)
    {
        if (!IsEnabled || item == null)
            return false;

        if (_selectedItems.Contains(item))
        {
            Deselect(item);
            return false;
        }
        else
        {
            Select(item, SelectionMode == SelectionMode.Multiple);
            return true;
        }
    }

    /// <summary>
    /// Selects a range.
    /// </summary>
    /// <param name="range">The range to select.</param>
    /// <returns>True if the selection changed.</returns>
    public bool SelectRange(SelectionRange range)
    {
        if (!IsEnabled || range == null)
            return false;

        var changed = _selectedRange != range;

        if (changed)
        {
            _selectedRange = range;
            _selectedItems.Clear();
            OnSelectionChanged();
        }

        return changed;
    }

    /// <summary>
    /// Clears all selection.
    /// </summary>
    /// <returns>True if the selection changed.</returns>
    public bool Clear()
    {
        if (_selectedItems.Count == 0 && _selectedRange == null)
            return false;

        _selectedItems.Clear();
        _selectedRange = null;
        OnSelectionChanged();
        return true;
    }

    /// <summary>
    /// Checks if an item is selected.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is selected.</returns>
    public bool IsSelected(object item)
    {
        return _selectedItems.Contains(item);
    }

    /// <summary>
    /// Checks if a point is within the selected range.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is within the selected range.</returns>
    public bool IsInSelectedRange(SKPoint point)
    {
        if (_selectedRange == null)
            return false;

        return _selectedRange.Contains(point);
    }

    private void OnSelectionChanged()
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
        {
            SelectedItems = _selectedItems.ToList(),
            SelectedRange = _selectedRange
        });
    }
}

/// <summary>
/// Selection mode enumeration.
/// </summary>
public enum SelectionMode
{
    /// <summary>No selection allowed.</summary>
    None,
    /// <summary>Single item selection.</summary>
    Single,
    /// <summary>Multiple item selection.</summary>
    Multiple
}

/// <summary>
/// Represents a selection range.
/// </summary>
public abstract record SelectionRange
{
    /// <summary>
    /// Checks if a point is contained within the range.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is within the range.</returns>
    public abstract bool Contains(SKPoint point);

    /// <summary>
    /// Renders the selection range.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    public abstract void Render(SKCanvas canvas);
}

/// <summary>
/// A rectangular selection range.
/// </summary>
public record RectangleSelectionRange : SelectionRange
{
    /// <summary>
    /// Gets the selection rectangle.
    /// </summary>
    public required SKRect Rectangle { get; init; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; init; } = new SKColor(0, 120, 215, 50);

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public SKColor BorderColor { get; init; } = new SKColor(0, 120, 215, 150);

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public float BorderWidth { get; init; } = 1.0f;

    /// <inheritdoc/>
    public override bool Contains(SKPoint point)
    {
        return Rectangle.Contains(point);
    }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas)
    {
        // Draw fill
        using var fillPaint = new SKPaint
        {
            Color = FillColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRect(Rectangle, fillPaint);

        // Draw border
        using var borderPaint = new SKPaint
        {
            Color = BorderColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = BorderWidth,
            IsAntialias = true
        };
        canvas.DrawRect(Rectangle, borderPaint);
    }
}

/// <summary>
/// A time-based range selection.
/// </summary>
public record TimeRangeSelection : SelectionRange
{
    /// <summary>
    /// Gets the start time.
    /// </summary>
    public required DateTime StartTime { get; init; }

    /// <summary>
    /// Gets the end time.
    /// </summary>
    public required DateTime EndTime { get; init; }

    /// <summary>
    /// Gets or sets the screen bounds for rendering.
    /// </summary>
    public SKRect Bounds { get; init; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; init; } = new SKColor(0, 120, 215, 50);

    /// <inheritdoc/>
    public override bool Contains(SKPoint point)
    {
        // Time-based containment would require additional context
        // This is a simplified implementation
        return Bounds.Contains(point);
    }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas)
    {
        using var paint = new SKPaint
        {
            Color = FillColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRect(Bounds, paint);
    }
}

/// <summary>
/// A data value range selection.
/// </summary>
public record DataRangeSelection : SelectionRange
{
    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    public required float MinValue { get; init; }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    public required float MaxValue { get; init; }

    /// <summary>
    /// Gets or sets the screen bounds for rendering.
    /// </summary>
    public SKRect Bounds { get; init; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SKColor FillColor { get; init; } = new SKColor(0, 120, 215, 50);

    /// <inheritdoc/>
    public override bool Contains(SKPoint point)
    {
        return Bounds.Contains(point);
    }

    /// <inheritdoc/>
    public override void Render(SKCanvas canvas)
    {
        using var paint = new SKPaint
        {
            Color = FillColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawRect(Bounds, paint);
    }
}

/// <summary>
/// Event arguments for selection changes.
/// </summary>
public class SelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the currently selected items.
    /// </summary>
    public required List<object> SelectedItems { get; init; }

    /// <summary>
    /// Gets the currently selected range (if any).
    /// </summary>
    public SelectionRange? SelectedRange { get; init; }
}
