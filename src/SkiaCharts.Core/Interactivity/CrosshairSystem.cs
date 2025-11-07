using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Manages crosshair display for charts.
/// </summary>
public class CrosshairManager
{
    private SKPoint? _position;
    private bool _isVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrosshairManager"/> class.
    /// </summary>
    public CrosshairManager()
    {
        IsEnabled = true;
        LineColor = new SKColor(128, 128, 128, 180);
        LineWidth = 1.0f;
        LineStyle = CrosshairLineStyle.Dashed;
        ShowLabels = true;
        LabelBackgroundColor = new SKColor(0, 0, 0, 200);
        LabelTextColor = SKColors.White;
        LabelFontSize = 10.0f;
    }

    /// <summary>
    /// Gets or sets whether the crosshair is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    public SKColor LineColor { get; set; }

    /// <summary>
    /// Gets or sets the line width.
    /// </summary>
    public float LineWidth { get; set; }

    /// <summary>
    /// Gets or sets the line style.
    /// </summary>
    public CrosshairLineStyle LineStyle { get; set; }

    /// <summary>
    /// Gets or sets whether to show coordinate labels.
    /// </summary>
    public bool ShowLabels { get; set; }

    /// <summary>
    /// Gets or sets the label background color.
    /// </summary>
    public SKColor LabelBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the label text color.
    /// </summary>
    public SKColor LabelTextColor { get; set; }

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; }

    /// <summary>
    /// Gets or sets whether to snap to data points.
    /// </summary>
    public bool SnapToData { get; set; }

    /// <summary>
    /// Gets or sets the snap distance threshold.
    /// </summary>
    public float SnapDistance { get; set; } = 20.0f;

    /// <summary>
    /// Gets the current crosshair position (null if not visible).
    /// </summary>
    public SKPoint? Position => _isVisible ? _position : null;

    /// <summary>
    /// Gets whether the crosshair is currently visible.
    /// </summary>
    public bool IsVisible => _isVisible && _position.HasValue;

    /// <summary>
    /// Event raised when the crosshair position changes.
    /// </summary>
    public event EventHandler<CrosshairEventArgs>? PositionChanged;

    /// <summary>
    /// Updates the crosshair position.
    /// </summary>
    /// <param name="position">The new position, or null to hide.</param>
    /// <param name="viewport">Optional viewport for coordinate conversion.</param>
    public void Update(SKPoint? position, Viewport? viewport = null)
    {
        if (!IsEnabled)
        {
            Hide();
            return;
        }

        var oldPosition = _position;
        _position = position;
        _isVisible = position.HasValue;

        if (oldPosition != _position)
        {
            SKPoint? dataPosition = null;
            if (viewport != null && _position.HasValue)
            {
                dataPosition = viewport.ScreenToData(_position.Value);
            }

            OnPositionChanged(dataPosition);
        }
    }

    /// <summary>
    /// Shows the crosshair at the specified position.
    /// </summary>
    /// <param name="position">The position to show at.</param>
    public void Show(SKPoint position)
    {
        _position = position;
        _isVisible = true;
        OnPositionChanged(null);
    }

    /// <summary>
    /// Hides the crosshair.
    /// </summary>
    public void Hide()
    {
        if (_isVisible)
        {
            _isVisible = false;
            _position = null;
            OnPositionChanged(null);
        }
    }

    /// <summary>
    /// Renders the crosshair.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    /// <param name="bounds">The chart bounds.</param>
    /// <param name="viewport">Optional viewport for data coordinates.</param>
    public void Render(SKCanvas canvas, SKRect bounds, Viewport? viewport = null)
    {
        if (!IsVisible || !_position.HasValue)
            return;

        var pos = _position.Value;

        using var paint = new SKPaint
        {
            Color = LineColor,
            StrokeWidth = LineWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        // Apply line style
        if (LineStyle == CrosshairLineStyle.Dashed)
        {
            paint.PathEffect = SKPathEffect.CreateDash(new[] { 5f, 5f }, 0);
        }
        else if (LineStyle == CrosshairLineStyle.Dotted)
        {
            paint.PathEffect = SKPathEffect.CreateDash(new[] { 2f, 3f }, 0);
        }

        // Draw vertical line
        canvas.DrawLine(pos.X, bounds.Top, pos.X, bounds.Bottom, paint);

        // Draw horizontal line
        canvas.DrawLine(bounds.Left, pos.Y, bounds.Right, pos.Y, paint);

        // Draw labels if enabled
        if (ShowLabels && viewport != null)
        {
            var dataPos = viewport.ScreenToData(pos);
            RenderLabels(canvas, bounds, pos, dataPos);
        }
    }

    private void RenderLabels(SKCanvas canvas, SKRect bounds, SKPoint screenPos, SKPoint dataPos)
    {
        using var textPaint = new SKPaint
        {
            Color = LabelTextColor,
            TextSize = LabelFontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };

        using var bgPaint = new SKPaint
        {
            Color = LabelBackgroundColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var padding = 4f;

        // X-axis label (bottom)
        var xLabel = dataPos.X.ToString("F2");
        var xTextBounds = new SKRect();
        textPaint.MeasureText(xLabel, ref xTextBounds);

        var xLabelRect = new SKRect(
            screenPos.X - xTextBounds.Width / 2 - padding,
            bounds.Bottom - xTextBounds.Height - padding * 2,
            screenPos.X + xTextBounds.Width / 2 + padding,
            bounds.Bottom
        );

        // Keep within bounds
        if (xLabelRect.Left < bounds.Left)
            xLabelRect.Offset(bounds.Left - xLabelRect.Left, 0);
        if (xLabelRect.Right > bounds.Right)
            xLabelRect.Offset(bounds.Right - xLabelRect.Right, 0);

        canvas.DrawRect(xLabelRect, bgPaint);
        canvas.DrawText(xLabel, xLabelRect.Left + padding, xLabelRect.Bottom - padding - xTextBounds.Bottom, textPaint);

        // Y-axis label (left)
        var yLabel = dataPos.Y.ToString("F2");
        var yTextBounds = new SKRect();
        textPaint.MeasureText(yLabel, ref yTextBounds);

        var yLabelRect = new SKRect(
            bounds.Left,
            screenPos.Y - yTextBounds.Height / 2 - padding,
            bounds.Left + yTextBounds.Width + padding * 2,
            screenPos.Y + yTextBounds.Height / 2 + padding
        );

        // Keep within bounds
        if (yLabelRect.Top < bounds.Top)
            yLabelRect.Offset(0, bounds.Top - yLabelRect.Top);
        if (yLabelRect.Bottom > bounds.Bottom)
            yLabelRect.Offset(0, bounds.Bottom - yLabelRect.Bottom);

        canvas.DrawRect(yLabelRect, bgPaint);
        canvas.DrawText(yLabel, yLabelRect.Left + padding, yLabelRect.Bottom - padding - yTextBounds.Bottom, textPaint);
    }

    private void OnPositionChanged(SKPoint? dataPosition)
    {
        PositionChanged?.Invoke(this, new CrosshairEventArgs
        {
            ScreenPosition = _position,
            DataPosition = dataPosition,
            IsVisible = _isVisible
        });
    }
}

/// <summary>
/// Crosshair line style enumeration.
/// </summary>
public enum CrosshairLineStyle
{
    /// <summary>Solid line.</summary>
    Solid,
    /// <summary>Dashed line.</summary>
    Dashed,
    /// <summary>Dotted line.</summary>
    Dotted
}

/// <summary>
/// Event arguments for crosshair events.
/// </summary>
public class CrosshairEventArgs : EventArgs
{
    /// <summary>
    /// Gets the screen position of the crosshair (null if hidden).
    /// </summary>
    public SKPoint? ScreenPosition { get; init; }

    /// <summary>
    /// Gets the data position of the crosshair (null if hidden or no viewport).
    /// </summary>
    public SKPoint? DataPosition { get; init; }

    /// <summary>
    /// Gets whether the crosshair is visible.
    /// </summary>
    public bool IsVisible { get; init; }
}

/// <summary>
/// Synchronized crosshair manager for multiple charts.
/// </summary>
public class SynchronizedCrosshairManager
{
    private readonly List<CrosshairManager> _crosshairs;
    private SKPoint? _sharedPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedCrosshairManager"/> class.
    /// </summary>
    public SynchronizedCrosshairManager()
    {
        _crosshairs = new List<CrosshairManager>();
    }

    /// <summary>
    /// Registers a crosshair to be synchronized.
    /// </summary>
    /// <param name="crosshair">The crosshair to register.</param>
    public void Register(CrosshairManager crosshair)
    {
        if (!_crosshairs.Contains(crosshair))
        {
            _crosshairs.Add(crosshair);
            crosshair.PositionChanged += OnCrosshairPositionChanged;
        }
    }

    /// <summary>
    /// Unregisters a crosshair.
    /// </summary>
    /// <param name="crosshair">The crosshair to unregister.</param>
    public void Unregister(CrosshairManager crosshair)
    {
        if (_crosshairs.Remove(crosshair))
        {
            crosshair.PositionChanged -= OnCrosshairPositionChanged;
        }
    }

    /// <summary>
    /// Updates all synchronized crosshairs.
    /// </summary>
    /// <param name="position">The new position, or null to hide.</param>
    public void UpdateAll(SKPoint? position)
    {
        _sharedPosition = position;

        foreach (var crosshair in _crosshairs)
        {
            // Temporarily detach to avoid circular updates
            crosshair.PositionChanged -= OnCrosshairPositionChanged;
            crosshair.Update(position);
            crosshair.PositionChanged += OnCrosshairPositionChanged;
        }
    }

    private void OnCrosshairPositionChanged(object? sender, CrosshairEventArgs e)
    {
        if (sender is CrosshairManager source)
        {
            // Update all other crosshairs to match
            foreach (var crosshair in _crosshairs)
            {
                if (crosshair != source)
                {
                    crosshair.PositionChanged -= OnCrosshairPositionChanged;
                    crosshair.Update(e.ScreenPosition);
                    crosshair.PositionChanged += OnCrosshairPositionChanged;
                }
            }
        }
    }
}
