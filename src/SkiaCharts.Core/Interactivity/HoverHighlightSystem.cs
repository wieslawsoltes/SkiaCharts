using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages hover highlighting for chart elements.
/// </summary>
public class HoverHighlightManager
{
    private object? _hoveredItem;
    private SKPoint _hoverPosition;
    private DateTime _hoverStartTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="HoverHighlightManager"/> class.
    /// </summary>
    public HoverHighlightManager()
    {
        IsEnabled = true;
        HighlightDelay = TimeSpan.FromMilliseconds(100);
        HighlightColor = new SKColor(255, 255, 0, 100);
        HighlightStrokeColor = new SKColor(255, 200, 0, 200);
        HighlightStrokeWidth = 2.0f;
        ScaleFactor = 1.2f;
    }

    /// <summary>
    /// Gets or sets whether hover highlighting is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the delay before showing highlight.
    /// </summary>
    public TimeSpan HighlightDelay { get; set; }

    /// <summary>
    /// Gets or sets the highlight fill color.
    /// </summary>
    public SKColor HighlightColor { get; set; }

    /// <summary>
    /// Gets or sets the highlight stroke color.
    /// </summary>
    public SKColor HighlightStrokeColor { get; set; }

    /// <summary>
    /// Gets or sets the highlight stroke width.
    /// </summary>
    public float HighlightStrokeWidth { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for highlighted elements.
    /// </summary>
    public float ScaleFactor { get; set; }

    /// <summary>
    /// Gets or sets the highlight style.
    /// </summary>
    public HighlightStyle HighlightStyle { get; set; } = HighlightStyle.Glow;

    /// <summary>
    /// Gets the currently hovered item (null if none).
    /// </summary>
    public object? HoveredItem => ShouldHighlight() ? _hoveredItem : null;

    /// <summary>
    /// Gets whether an item is currently being highlighted.
    /// </summary>
    public bool IsHighlighting => HoveredItem != null;

    /// <summary>
    /// Event raised when the hovered item changes.
    /// </summary>
    public event EventHandler<HoverChangedEventArgs>? HoverChanged;

    /// <summary>
    /// Updates the hover state.
    /// </summary>
    /// <param name="item">The item being hovered (null if none).</param>
    /// <param name="position">The hover position.</param>
    public void Update(object? item, SKPoint position)
    {
        if (!IsEnabled)
        {
            Clear();
            return;
        }

        if (item != _hoveredItem)
        {
            var oldItem = _hoveredItem;
            _hoveredItem = item;
            _hoverPosition = position;
            _hoverStartTime = DateTime.Now;

            OnHoverChanged(oldItem, item);
        }
        else if (item != null)
        {
            _hoverPosition = position;
        }
    }

    /// <summary>
    /// Clears the current hover state.
    /// </summary>
    public void Clear()
    {
        if (_hoveredItem != null)
        {
            var oldItem = _hoveredItem;
            _hoveredItem = null;
            OnHoverChanged(oldItem, null);
        }
    }

    /// <summary>
    /// Checks if an item is currently hovered.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is hovered and should be highlighted.</returns>
    public bool IsHovered(object item)
    {
        return IsHighlighting && ReferenceEquals(_hoveredItem, item);
    }

    /// <summary>
    /// Renders a highlight effect for an item.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="bounds">The bounds of the item to highlight.</param>
    public void RenderHighlight(SKCanvas canvas, SKRect bounds)
    {
        if (!IsHighlighting)
            return;

        switch (HighlightStyle)
        {
            case HighlightStyle.Glow:
                RenderGlowHighlight(canvas, bounds);
                break;
            case HighlightStyle.Outline:
                RenderOutlineHighlight(canvas, bounds);
                break;
            case HighlightStyle.Fill:
                RenderFillHighlight(canvas, bounds);
                break;
            case HighlightStyle.Scale:
                // Scaling is handled by the caller adjusting bounds
                break;
        }
    }

    /// <summary>
    /// Renders a highlight effect for a circular item.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="center">The center of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    public void RenderCircleHighlight(SKCanvas canvas, SKPoint center, float radius)
    {
        if (!IsHighlighting)
            return;

        switch (HighlightStyle)
        {
            case HighlightStyle.Glow:
                RenderCircleGlow(canvas, center, radius);
                break;
            case HighlightStyle.Outline:
                RenderCircleOutline(canvas, center, radius);
                break;
            case HighlightStyle.Fill:
                RenderCircleFill(canvas, center, radius);
                break;
            case HighlightStyle.Scale:
                RenderCircleOutline(canvas, center, radius * ScaleFactor);
                break;
        }
    }

    /// <summary>
    /// Gets the scale factor to apply if highlighting with scale.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>The scale factor (1.0 if not highlighting this item).</returns>
    public float GetScaleFactor(object item)
    {
        if (HighlightStyle == HighlightStyle.Scale && IsHovered(item))
        {
            return ScaleFactor;
        }
        return 1.0f;
    }

    private bool ShouldHighlight()
    {
        if (_hoveredItem == null)
            return false;

        var hoverDuration = DateTime.Now - _hoverStartTime;
        return hoverDuration >= HighlightDelay;
    }

    private void RenderGlowHighlight(SKCanvas canvas, SKRect bounds)
    {
        using var paint = new SKPaint
        {
            Color = HighlightColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = HighlightStrokeWidth * 3,
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5.0f)
        };

        var expandedBounds = bounds;
        expandedBounds.Inflate(5, 5);
        canvas.DrawRect(expandedBounds, paint);
    }

    private void RenderOutlineHighlight(SKCanvas canvas, SKRect bounds)
    {
        using var paint = new SKPaint
        {
            Color = HighlightStrokeColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = HighlightStrokeWidth,
            IsAntialias = true
        };

        var expandedBounds = bounds;
        expandedBounds.Inflate(2, 2);
        canvas.DrawRect(expandedBounds, paint);
    }

    private void RenderFillHighlight(SKCanvas canvas, SKRect bounds)
    {
        using var paint = new SKPaint
        {
            Color = HighlightColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        canvas.DrawRect(bounds, paint);
    }

    private void RenderCircleGlow(SKCanvas canvas, SKPoint center, float radius)
    {
        using var paint = new SKPaint
        {
            Color = HighlightColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = HighlightStrokeWidth * 2,
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 4.0f)
        };

        canvas.DrawCircle(center, radius + 3, paint);
    }

    private void RenderCircleOutline(SKCanvas canvas, SKPoint center, float radius)
    {
        using var paint = new SKPaint
        {
            Color = HighlightStrokeColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = HighlightStrokeWidth,
            IsAntialias = true
        };

        canvas.DrawCircle(center, radius + 2, paint);
    }

    private void RenderCircleFill(SKCanvas canvas, SKPoint center, float radius)
    {
        using var paint = new SKPaint
        {
            Color = HighlightColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        canvas.DrawCircle(center, radius, paint);
    }

    private void OnHoverChanged(object? oldItem, object? newItem)
    {
        HoverChanged?.Invoke(this, new HoverChangedEventArgs
        {
            OldItem = oldItem,
            NewItem = newItem,
            Position = _hoverPosition
        });
    }
}

/// <summary>
/// Highlight style enumeration.
/// </summary>
public enum HighlightStyle
{
    /// <summary>Glow effect around the item.</summary>
    Glow,
    /// <summary>Outline around the item.</summary>
    Outline,
    /// <summary>Fill the item with highlight color.</summary>
    Fill,
    /// <summary>Scale the item up.</summary>
    Scale
}

/// <summary>
/// Event arguments for hover changes.
/// </summary>
public class HoverChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previously hovered item (null if none).
    /// </summary>
    public object? OldItem { get; init; }

    /// <summary>
    /// Gets the newly hovered item (null if none).
    /// </summary>
    public object? NewItem { get; init; }

    /// <summary>
    /// Gets the hover position.
    /// </summary>
    public SKPoint Position { get; init; }
}
