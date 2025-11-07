using SkiaSharp;

namespace SkiaCharts.Core.Legend;

/// <summary>
/// Manages chart legend display and interaction.
/// </summary>
public class LegendManager
{
    private readonly List<LegendItem> _items;
    private SKRect _bounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegendManager"/> class.
    /// </summary>
    public LegendManager()
    {
        _items = new List<LegendItem>();
        IsVisible = true;
        Position = LegendPosition.TopRight;
        Orientation = LegendOrientation.Vertical;
        BackgroundColor = new SKColor(255, 255, 255, 230);
        BorderColor = new SKColor(200, 200, 200, 255);
        BorderWidth = 1.0f;
        Padding = 10.0f;
        ItemSpacing = 5.0f;
        SymbolSize = 12.0f;
        SymbolTextSpacing = 8.0f;
        FontSize = 11.0f;
        FontFamily = "Arial";
        TextColor = SKColors.Black;
        CornerRadius = 4.0f;
    }

    /// <summary>
    /// Gets or sets whether the legend is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the legend position.
    /// </summary>
    public LegendPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the legend orientation.
    /// </summary>
    public LegendOrientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public SKColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public SKColor BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public float BorderWidth { get; set; }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public float Padding { get; set; }

    /// <summary>
    /// Gets or sets the spacing between items.
    /// </summary>
    public float ItemSpacing { get; set; }

    /// <summary>
    /// Gets or sets the symbol size.
    /// </summary>
    public float SymbolSize { get; set; }

    /// <summary>
    /// Gets or sets the spacing between symbol and text.
    /// </summary>
    public float SymbolTextSpacing { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public float FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public SKColor TextColor { get; set; }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius { get; set; }

    /// <summary>
    /// Gets or sets whether legend items are interactive.
    /// </summary>
    public bool IsInteractive { get; set; } = true;

    /// <summary>
    /// Gets or sets the legend renderer.
    /// </summary>
    public ILegendRenderer? Renderer { get; set; }

    /// <summary>
    /// Gets the legend items.
    /// </summary>
    public IReadOnlyList<LegendItem> Items => _items;

    /// <summary>
    /// Gets the calculated legend bounds.
    /// </summary>
    public SKRect Bounds => _bounds;

    /// <summary>
    /// Event raised when a legend item is clicked.
    /// </summary>
    public event EventHandler<LegendItemEventArgs>? ItemClicked;

    /// <summary>
    /// Event raised when a legend item's visibility changes.
    /// </summary>
    public event EventHandler<LegendItemEventArgs>? ItemVisibilityChanged;

    /// <summary>
    /// Adds a legend item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(LegendItem item)
    {
        if (item != null)
        {
            _items.Add(item);
        }
    }

    /// <summary>
    /// Removes a legend item.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public bool RemoveItem(LegendItem item)
    {
        return _items.Remove(item);
    }

    /// <summary>
    /// Clears all legend items.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }

    /// <summary>
    /// Handles a click at the specified position.
    /// </summary>
    /// <param name="position">The click position.</param>
    /// <returns>True if a legend item was clicked.</returns>
    public bool HandleClick(SKPoint position)
    {
        if (!IsInteractive || !IsVisible)
            return false;

        var item = HitTest(position);
        if (item != null)
        {
            OnItemClicked(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs hit testing to find the item at the given position.
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <returns>The item at the position, or null if none.</returns>
    public LegendItem? HitTest(SKPoint position)
    {
        if (!_bounds.Contains(position))
            return null;

        foreach (var item in _items)
        {
            if (item.Bounds.Contains(position))
                return item;
        }

        return null;
    }

    /// <summary>
    /// Toggles the visibility of a legend item.
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    public void ToggleItem(LegendItem item)
    {
        if (item == null)
            return;

        item.IsVisible = !item.IsVisible;
        OnItemVisibilityChanged(item);
    }

    /// <summary>
    /// Calculates the legend layout.
    /// </summary>
    /// <param name="chartBounds">The chart bounds.</param>
    public void CalculateLayout(SKRect chartBounds)
    {
        if (!IsVisible || _items.Count == 0)
        {
            _bounds = SKRect.Empty;
            return;
        }

        using var textPaint = new SKPaint
        {
            TextSize = FontSize,
            Typeface = SKTypeface.FromFamilyName(FontFamily)
        };

        // Calculate item sizes
        float maxItemWidth = 0;
        float totalHeight = Padding;

        foreach (var item in _items)
        {
            var textBounds = new SKRect();
            textPaint.MeasureText(item.Text, ref textBounds);

            var itemWidth = SymbolSize + SymbolTextSpacing + textBounds.Width + Padding * 2;
            var itemHeight = Math.Max(SymbolSize, textBounds.Height);

            item.Width = itemWidth;
            item.Height = itemHeight;

            if (Orientation == LegendOrientation.Vertical)
            {
                maxItemWidth = Math.Max(maxItemWidth, itemWidth);
                totalHeight += itemHeight + ItemSpacing;
            }
        }

        totalHeight += Padding;

        // Calculate legend size
        float legendWidth, legendHeight;

        if (Orientation == LegendOrientation.Vertical)
        {
            legendWidth = maxItemWidth;
            legendHeight = totalHeight;
        }
        else // Horizontal
        {
            legendWidth = _items.Sum(i => i.Width) + ItemSpacing * (_items.Count - 1) + Padding * 2;
            legendHeight = _items.Max(i => i.Height) + Padding * 2;
        }

        // Position the legend
        var legendX = Position switch
        {
            LegendPosition.TopLeft or LegendPosition.MiddleLeft or LegendPosition.BottomLeft =>
                chartBounds.Left + Padding,
            LegendPosition.TopCenter or LegendPosition.MiddleCenter or LegendPosition.BottomCenter =>
                chartBounds.MidX - legendWidth / 2,
            _ => chartBounds.Right - legendWidth - Padding
        };

        var legendY = Position switch
        {
            LegendPosition.TopLeft or LegendPosition.TopCenter or LegendPosition.TopRight =>
                chartBounds.Top + Padding,
            LegendPosition.MiddleLeft or LegendPosition.MiddleCenter or LegendPosition.MiddleRight =>
                chartBounds.MidY - legendHeight / 2,
            _ => chartBounds.Bottom - legendHeight - Padding
        };

        _bounds = new SKRect(legendX, legendY, legendX + legendWidth, legendY + legendHeight);

        // Calculate item positions
        float x = legendX + Padding;
        float y = legendY + Padding;

        foreach (var item in _items)
        {
            item.Bounds = new SKRect(
                x,
                y,
                x + item.Width - Padding * 2,
                y + item.Height
            );

            if (Orientation == LegendOrientation.Vertical)
            {
                y += item.Height + ItemSpacing;
            }
            else
            {
                x += item.Width + ItemSpacing;
            }
        }
    }

    /// <summary>
    /// Renders the legend.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="chartBounds">The chart bounds.</param>
    public void Render(SKCanvas canvas, SKRect chartBounds)
    {
        if (!IsVisible || _items.Count == 0)
            return;

        if (Renderer != null)
        {
            Renderer.Render(canvas, this, chartBounds);
        }
        else
        {
            RenderDefault(canvas);
        }
    }

    private void RenderDefault(SKCanvas canvas)
    {
        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = BackgroundColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        canvas.DrawRoundRect(_bounds, CornerRadius, CornerRadius, bgPaint);

        // Draw border
        if (BorderWidth > 0)
        {
            using var borderPaint = new SKPaint
            {
                Color = BorderColor,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = BorderWidth,
                IsAntialias = true
            };

            canvas.DrawRoundRect(_bounds, CornerRadius, CornerRadius, borderPaint);
        }

        // Draw items
        using var textPaint = new SKPaint
        {
            Color = TextColor,
            TextSize = FontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(FontFamily)
        };

        foreach (var item in _items)
        {
            var alpha = item.IsVisible ? (byte)255 : (byte)128;

            // Draw symbol
            var symbolX = item.Bounds.Left;
            var symbolY = item.Bounds.MidY - SymbolSize / 2;
            var symbolRect = new SKRect(symbolX, symbolY, symbolX + SymbolSize, symbolY + SymbolSize);

            var symbolColor = item.Color;
            symbolColor = new SKColor(symbolColor.Red, symbolColor.Green, symbolColor.Blue, alpha);

            using var symbolPaint = new SKPaint
            {
                Color = symbolColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            switch (item.SymbolType)
            {
                case LegendSymbolType.Rectangle:
                    canvas.DrawRect(symbolRect, symbolPaint);
                    break;
                case LegendSymbolType.Circle:
                    canvas.DrawCircle(symbolRect.MidX, symbolRect.MidY, SymbolSize / 2, symbolPaint);
                    break;
                case LegendSymbolType.Line:
                    symbolPaint.Style = SKPaintStyle.Stroke;
                    symbolPaint.StrokeWidth = 2;
                    canvas.DrawLine(symbolRect.Left, symbolRect.MidY, symbolRect.Right, symbolRect.MidY, symbolPaint);
                    break;
            }

            // Draw text
            var textX = symbolX + SymbolSize + SymbolTextSpacing;
            var textY = item.Bounds.MidY;

            var textColor = TextColor;
            textColor = new SKColor(textColor.Red, textColor.Green, textColor.Blue, alpha);
            textPaint.Color = textColor;

            var textBounds = new SKRect();
            textPaint.MeasureText(item.Text, ref textBounds);

            canvas.DrawText(item.Text, textX, textY - textBounds.MidY, textPaint);
        }
    }

    private void OnItemClicked(LegendItem item)
    {
        if (IsInteractive)
        {
            ToggleItem(item);
        }

        ItemClicked?.Invoke(this, new LegendItemEventArgs { Item = item });
    }

    private void OnItemVisibilityChanged(LegendItem item)
    {
        ItemVisibilityChanged?.Invoke(this, new LegendItemEventArgs { Item = item });
    }
}

/// <summary>
/// Represents a legend item.
/// </summary>
public class LegendItem
{
    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    public required SKColor Color { get; init; }

    /// <summary>
    /// Gets or sets the symbol type.
    /// </summary>
    public LegendSymbolType SymbolType { get; init; } = LegendSymbolType.Rectangle;

    /// <summary>
    /// Gets or sets whether the item is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets associated data (e.g., series object).
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Gets the calculated bounds.
    /// </summary>
    public SKRect Bounds { get; internal set; }

    /// <summary>
    /// Gets the calculated width.
    /// </summary>
    public float Width { get; internal set; }

    /// <summary>
    /// Gets the calculated height.
    /// </summary>
    public float Height { get; internal set; }
}

/// <summary>
/// Legend position enumeration (9 positions).
/// </summary>
public enum LegendPosition
{
    /// <summary>Top-left corner.</summary>
    TopLeft,
    /// <summary>Top-center.</summary>
    TopCenter,
    /// <summary>Top-right corner.</summary>
    TopRight,
    /// <summary>Middle-left.</summary>
    MiddleLeft,
    /// <summary>Middle-center.</summary>
    MiddleCenter,
    /// <summary>Middle-right.</summary>
    MiddleRight,
    /// <summary>Bottom-left corner.</summary>
    BottomLeft,
    /// <summary>Bottom-center.</summary>
    BottomCenter,
    /// <summary>Bottom-right corner.</summary>
    BottomRight
}

/// <summary>
/// Legend orientation enumeration.
/// </summary>
public enum LegendOrientation
{
    /// <summary>Vertical layout.</summary>
    Vertical,
    /// <summary>Horizontal layout.</summary>
    Horizontal
}

/// <summary>
/// Legend symbol type enumeration.
/// </summary>
public enum LegendSymbolType
{
    /// <summary>Rectangle symbol.</summary>
    Rectangle,
    /// <summary>Circle symbol.</summary>
    Circle,
    /// <summary>Line symbol.</summary>
    Line
}

/// <summary>
/// Interface for legend renderers.
/// </summary>
public interface ILegendRenderer
{
    /// <summary>
    /// Renders a legend.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="legend">The legend manager.</param>
    /// <param name="chartBounds">The chart bounds.</param>
    void Render(SKCanvas canvas, LegendManager legend, SKRect chartBounds);
}

/// <summary>
/// Custom legend renderer using a template function.
/// </summary>
public class CustomLegendRenderer : ILegendRenderer
{
    /// <summary>
    /// Gets or sets the custom render function.
    /// </summary>
    public Action<SKCanvas, LegendManager, SKRect>? RenderFunction { get; set; }

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, LegendManager legend, SKRect chartBounds)
    {
        RenderFunction?.Invoke(canvas, legend, chartBounds);
    }
}

/// <summary>
/// Event arguments for legend item events.
/// </summary>
public class LegendItemEventArgs : EventArgs
{
    /// <summary>
    /// Gets the legend item.
    /// </summary>
    public required LegendItem Item { get; init; }
}
