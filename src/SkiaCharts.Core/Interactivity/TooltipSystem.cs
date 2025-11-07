using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages tooltip display for chart elements.
/// </summary>
public class TooltipManager
{
    private TooltipInfo? _currentTooltip;
    private SKPoint _lastPosition;
    private DateTime _lastHoverTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="TooltipManager"/> class.
    /// </summary>
    public TooltipManager()
    {
        ShowDelay = TimeSpan.FromMilliseconds(500);
        HideDelay = TimeSpan.FromMilliseconds(200);
        Offset = new SKPoint(10, 10);
        IsEnabled = true;
    }

    /// <summary>
    /// Gets or sets the delay before showing a tooltip.
    /// </summary>
    public TimeSpan ShowDelay { get; set; }

    /// <summary>
    /// Gets or sets the delay before hiding a tooltip.
    /// </summary>
    public TimeSpan HideDelay { get; set; }

    /// <summary>
    /// Gets or sets the offset from the cursor position.
    /// </summary>
    public SKPoint Offset { get; set; }

    /// <summary>
    /// Gets or sets whether tooltips are enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the tooltip renderer.
    /// </summary>
    public ITooltipRenderer? Renderer { get; set; }

    /// <summary>
    /// Gets the current tooltip information.
    /// </summary>
    public TooltipInfo? CurrentTooltip => _currentTooltip;

    /// <summary>
    /// Gets whether a tooltip is currently visible.
    /// </summary>
    public bool IsVisible => _currentTooltip != null && ShouldShow();

    /// <summary>
    /// Event raised when the tooltip changes.
    /// </summary>
    public event EventHandler<TooltipEventArgs>? TooltipChanged;

    /// <summary>
    /// Updates the tooltip based on hover information.
    /// </summary>
    /// <param name="hitInfo">The hit test information, or null if no hit.</param>
    /// <param name="position">The current mouse/touch position.</param>
    public void Update(object? hitInfo, SKPoint position)
    {
        if (!IsEnabled)
        {
            Hide();
            return;
        }

        _lastPosition = position;

        if (hitInfo == null)
        {
            // No hit - hide tooltip after delay
            if (_currentTooltip != null)
            {
                var timeSinceHover = DateTime.Now - _lastHoverTime;
                if (timeSinceHover > HideDelay)
                {
                    Hide();
                }
            }
            return;
        }

        // Hit detected - show tooltip after delay
        if (_currentTooltip?.Data != hitInfo)
        {
            _lastHoverTime = DateTime.Now;
            _currentTooltip = CreateTooltipInfo(hitInfo, position);
            OnTooltipChanged();
        }
        else
        {
            // Update position for existing tooltip
            if (_currentTooltip != null)
            {
                _currentTooltip = _currentTooltip with { Position = position + Offset };
            }
        }
    }

    /// <summary>
    /// Shows a tooltip for specific data.
    /// </summary>
    /// <param name="data">The data to show.</param>
    /// <param name="position">The position to show at.</param>
    public void Show(object data, SKPoint position)
    {
        _lastHoverTime = DateTime.Now;
        _lastPosition = position;
        _currentTooltip = CreateTooltipInfo(data, position);
        OnTooltipChanged();
    }

    /// <summary>
    /// Hides the current tooltip.
    /// </summary>
    public void Hide()
    {
        if (_currentTooltip != null)
        {
            _currentTooltip = null;
            OnTooltipChanged();
        }
    }

    /// <summary>
    /// Renders the current tooltip.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="bounds">The chart bounds.</param>
    public void Render(SKCanvas canvas, SKRect bounds)
    {
        if (!IsVisible || _currentTooltip == null || Renderer == null)
            return;

        Renderer.Render(canvas, _currentTooltip, bounds);
    }

    private bool ShouldShow()
    {
        if (_currentTooltip == null)
            return false;

        var timeSinceHover = DateTime.Now - _lastHoverTime;
        return timeSinceHover >= ShowDelay;
    }

    private TooltipInfo CreateTooltipInfo(object data, SKPoint position)
    {
        return new TooltipInfo
        {
            Data = data,
            Position = position + Offset,
            Content = data.ToString() ?? string.Empty
        };
    }

    private void OnTooltipChanged()
    {
        TooltipChanged?.Invoke(this, new TooltipEventArgs { Tooltip = _currentTooltip });
    }
}

/// <summary>
/// Contains information about a tooltip.
/// </summary>
public record TooltipInfo
{
    /// <summary>
    /// Gets the data object associated with the tooltip.
    /// </summary>
    public required object Data { get; init; }

    /// <summary>
    /// Gets the position where the tooltip should be displayed.
    /// </summary>
    public required SKPoint Position { get; init; }

    /// <summary>
    /// Gets the content to display in the tooltip.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets additional metadata for the tooltip.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Event arguments for tooltip events.
/// </summary>
public class TooltipEventArgs : EventArgs
{
    /// <summary>
    /// Gets the tooltip information (null if hidden).
    /// </summary>
    public TooltipInfo? Tooltip { get; init; }
}

/// <summary>
/// Interface for tooltip renderers.
/// </summary>
public interface ITooltipRenderer
{
    /// <summary>
    /// Renders a tooltip.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="tooltip">The tooltip information.</param>
    /// <param name="bounds">The chart bounds.</param>
    void Render(SKCanvas canvas, TooltipInfo tooltip, SKRect bounds);
}

/// <summary>
/// Default tooltip renderer with simple box and text.
/// </summary>
public class DefaultTooltipRenderer : ITooltipRenderer
{
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public SKColor BackgroundColor { get; set; } = new SKColor(0, 0, 0, 220);

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public SKColor TextColor { get; set; } = SKColors.White;

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public SKColor BorderColor { get; set; } = new SKColor(255, 255, 255, 100);

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public float BorderWidth { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius { get; set; } = 4.0f;

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public float Padding { get; set; } = 8.0f;

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public float FontSize { get; set; } = 12.0f;

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string FontFamily { get; set; } = "Arial";

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, TooltipInfo tooltip, SKRect bounds)
    {
        using var textPaint = new SKPaint
        {
            Color = TextColor,
            TextSize = FontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(FontFamily)
        };

        // Measure text
        var textBounds = new SKRect();
        textPaint.MeasureText(tooltip.Content, ref textBounds);

        // Calculate tooltip box
        var boxWidth = textBounds.Width + Padding * 2;
        var boxHeight = textBounds.Height + Padding * 2;

        var boxX = tooltip.Position.X;
        var boxY = tooltip.Position.Y;

        // Keep tooltip within bounds
        if (boxX + boxWidth > bounds.Right)
            boxX = bounds.Right - boxWidth;
        if (boxY + boxHeight > bounds.Bottom)
            boxY = bounds.Bottom - boxHeight;
        if (boxX < bounds.Left)
            boxX = bounds.Left;
        if (boxY < bounds.Top)
            boxY = bounds.Top;

        var tooltipRect = new SKRect(boxX, boxY, boxX + boxWidth, boxY + boxHeight);

        // Draw background
        using var bgPaint = new SKPaint
        {
            Color = BackgroundColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawRoundRect(tooltipRect, CornerRadius, CornerRadius, bgPaint);

        // Draw border
        if (BorderWidth > 0)
        {
            using var borderPaint = new SKPaint
            {
                Color = BorderColor,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = BorderWidth
            };

            canvas.DrawRoundRect(tooltipRect, CornerRadius, CornerRadius, borderPaint);
        }

        // Draw text
        var textX = boxX + Padding;
        var textY = boxY + Padding - textBounds.Top;

        canvas.DrawText(tooltip.Content, textX, textY, textPaint);
    }
}

/// <summary>
/// Custom tooltip renderer using a template function.
/// </summary>
public class CustomTooltipRenderer : ITooltipRenderer
{
    /// <summary>
    /// Gets or sets the custom render function.
    /// </summary>
    public Action<SKCanvas, TooltipInfo, SKRect>? RenderFunction { get; set; }

    /// <inheritdoc/>
    public void Render(SKCanvas canvas, TooltipInfo tooltip, SKRect bounds)
    {
        RenderFunction?.Invoke(canvas, tooltip, bounds);
    }
}
