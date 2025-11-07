using SkiaSharp;

namespace SkiaCharts.Core.Legend;

/// <summary>
/// Manages data label display and positioning.
/// </summary>
public class DataLabelManager
{
    private readonly List<DataLabel> _labels;
    private readonly List<SKRect> _occupiedRegions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLabelManager"/> class.
    /// </summary>
    public DataLabelManager()
    {
        _labels = new List<DataLabel>();
        _occupiedRegions = new List<SKRect>();
        IsEnabled = true;
        FontSize = 10.0f;
        FontFamily = "Arial";
        TextColor = SKColors.Black;
        BackgroundColor = new SKColor(255, 255, 255, 200);
        BorderColor = new SKColor(200, 200, 200, 255);
        BorderWidth = 1.0f;
        Padding = 4.0f;
        CornerRadius = 3.0f;
        EnableCollisionDetection = true;
        CollisionPadding = 2.0f;
        MaxLabels = 100;
    }

    /// <summary>
    /// Gets or sets whether labels are enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

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
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius { get; set; }

    /// <summary>
    /// Gets or sets whether collision detection is enabled.
    /// </summary>
    public bool EnableCollisionDetection { get; set; }

    /// <summary>
    /// Gets or sets the collision padding.
    /// </summary>
    public float CollisionPadding { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of labels to display.
    /// </summary>
    public int MaxLabels { get; set; }

    /// <summary>
    /// Gets or sets the label formatter function.
    /// </summary>
    public Func<object, string>? Formatter { get; set; }

    /// <summary>
    /// Gets the data labels.
    /// </summary>
    public IReadOnlyList<DataLabel> Labels => _labels;

    /// <summary>
    /// Adds a data label.
    /// </summary>
    /// <param name="label">The label to add.</param>
    public void AddLabel(DataLabel label)
    {
        if (label != null && _labels.Count < MaxLabels)
        {
            _labels.Add(label);
        }
    }

    /// <summary>
    /// Clears all labels.
    /// </summary>
    public void Clear()
    {
        _labels.Clear();
        _occupiedRegions.Clear();
    }

    /// <summary>
    /// Calculates label layout with smart positioning.
    /// </summary>
    /// <param name="chartBounds">The chart bounds.</param>
    public void CalculateLayout(SKRect chartBounds)
    {
        if (!IsEnabled || _labels.Count == 0)
            return;

        _occupiedRegions.Clear();

        using var textPaint = new SKPaint
        {
            TextSize = FontSize,
            Typeface = SKTypeface.FromFamilyName(FontFamily)
        };

        foreach (var label in _labels)
        {
            // Format text
            var text = Formatter != null ? Formatter(label.Value) : label.Value.ToString() ?? string.Empty;
            label.FormattedText = text;

            // Measure text
            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            var labelWidth = textBounds.Width + Padding * 2;
            var labelHeight = textBounds.Height + Padding * 2;

            // Try to position the label
            var position = FindBestPosition(
                label.Position,
                labelWidth,
                labelHeight,
                chartBounds,
                label.Placement
            );

            if (position.HasValue)
            {
                label.Bounds = new SKRect(
                    position.Value.X,
                    position.Value.Y,
                    position.Value.X + labelWidth,
                    position.Value.Y + labelHeight
                );

                label.IsVisible = true;

                if (EnableCollisionDetection)
                {
                    var paddedBounds = label.Bounds;
                    paddedBounds.Inflate(CollisionPadding, CollisionPadding);
                    _occupiedRegions.Add(paddedBounds);
                }
            }
            else
            {
                label.IsVisible = false;
            }
        }
    }

    /// <summary>
    /// Renders all data labels.
    /// </summary>
    /// <param name="canvas">The canvas to render on.</param>
    public void Render(SKCanvas canvas)
    {
        if (!IsEnabled)
            return;

        using var textPaint = new SKPaint
        {
            Color = TextColor,
            TextSize = FontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(FontFamily)
        };

        using var bgPaint = new SKPaint
        {
            Color = BackgroundColor,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        using var borderPaint = new SKPaint
        {
            Color = BorderColor,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = BorderWidth,
            IsAntialias = true
        };

        foreach (var label in _labels)
        {
            if (!label.IsVisible)
                continue;

            // Draw background
            canvas.DrawRoundRect(label.Bounds, CornerRadius, CornerRadius, bgPaint);

            // Draw border
            if (BorderWidth > 0)
            {
                canvas.DrawRoundRect(label.Bounds, CornerRadius, CornerRadius, borderPaint);
            }

            // Draw text
            var textBounds = new SKRect();
            textPaint.MeasureText(label.FormattedText, ref textBounds);

            var textX = label.Bounds.Left + Padding;
            var textY = label.Bounds.Top + Padding - textBounds.Top;

            canvas.DrawText(label.FormattedText, textX, textY, textPaint);
        }
    }

    private SKPoint? FindBestPosition(
        SKPoint dataPosition,
        float width,
        float height,
        SKRect chartBounds,
        LabelPlacement preferredPlacement)
    {
        // Try preferred placement first
        var position = CalculatePosition(dataPosition, width, height, preferredPlacement);

        if (!EnableCollisionDetection)
        {
            return IsWithinBounds(position, width, height, chartBounds) ? position : null;
        }

        // Check if preferred position is valid
        if (IsValidPosition(position, width, height, chartBounds))
        {
            return position;
        }

        // Try alternative placements
        var placements = GetAlternativePlacements(preferredPlacement);

        foreach (var placement in placements)
        {
            position = CalculatePosition(dataPosition, width, height, placement);

            if (IsValidPosition(position, width, height, chartBounds))
            {
                return position;
            }
        }

        // No valid position found
        return null;
    }

    private SKPoint CalculatePosition(
        SKPoint dataPosition,
        float width,
        float height,
        LabelPlacement placement)
    {
        const float offset = 5.0f;

        return placement switch
        {
            LabelPlacement.Top => new SKPoint(dataPosition.X - width / 2, dataPosition.Y - height - offset),
            LabelPlacement.Bottom => new SKPoint(dataPosition.X - width / 2, dataPosition.Y + offset),
            LabelPlacement.Left => new SKPoint(dataPosition.X - width - offset, dataPosition.Y - height / 2),
            LabelPlacement.Right => new SKPoint(dataPosition.X + offset, dataPosition.Y - height / 2),
            LabelPlacement.TopLeft => new SKPoint(dataPosition.X - width - offset, dataPosition.Y - height - offset),
            LabelPlacement.TopRight => new SKPoint(dataPosition.X + offset, dataPosition.Y - height - offset),
            LabelPlacement.BottomLeft => new SKPoint(dataPosition.X - width - offset, dataPosition.Y + offset),
            LabelPlacement.BottomRight => new SKPoint(dataPosition.X + offset, dataPosition.Y + offset),
            _ => new SKPoint(dataPosition.X - width / 2, dataPosition.Y - height / 2)
        };
    }

    private bool IsValidPosition(SKPoint position, float width, float height, SKRect chartBounds)
    {
        var bounds = new SKRect(position.X, position.Y, position.X + width, position.Y + height);

        // Check chart bounds
        if (!IsWithinBounds(position, width, height, chartBounds))
            return false;

        // Check collisions
        var paddedBounds = bounds;
        paddedBounds.Inflate(CollisionPadding, CollisionPadding);

        return !_occupiedRegions.Any(r => r.IntersectsWith(paddedBounds));
    }

    private bool IsWithinBounds(SKPoint position, float width, float height, SKRect chartBounds)
    {
        var bounds = new SKRect(position.X, position.Y, position.X + width, position.Y + height);
        return chartBounds.Contains(bounds);
    }

    private List<LabelPlacement> GetAlternativePlacements(LabelPlacement preferred)
    {
        // Return placements in order of preference
        return preferred switch
        {
            LabelPlacement.Top => new List<LabelPlacement>
            {
                LabelPlacement.Bottom, LabelPlacement.Right, LabelPlacement.Left,
                LabelPlacement.TopRight, LabelPlacement.TopLeft,
                LabelPlacement.BottomRight, LabelPlacement.BottomLeft
            },
            LabelPlacement.Bottom => new List<LabelPlacement>
            {
                LabelPlacement.Top, LabelPlacement.Right, LabelPlacement.Left,
                LabelPlacement.BottomRight, LabelPlacement.BottomLeft,
                LabelPlacement.TopRight, LabelPlacement.TopLeft
            },
            LabelPlacement.Left => new List<LabelPlacement>
            {
                LabelPlacement.Right, LabelPlacement.Top, LabelPlacement.Bottom,
                LabelPlacement.TopLeft, LabelPlacement.BottomLeft,
                LabelPlacement.TopRight, LabelPlacement.BottomRight
            },
            LabelPlacement.Right => new List<LabelPlacement>
            {
                LabelPlacement.Left, LabelPlacement.Top, LabelPlacement.Bottom,
                LabelPlacement.TopRight, LabelPlacement.BottomRight,
                LabelPlacement.TopLeft, LabelPlacement.BottomLeft
            },
            _ => new List<LabelPlacement>
            {
                LabelPlacement.Top, LabelPlacement.Bottom, LabelPlacement.Left, LabelPlacement.Right,
                LabelPlacement.TopRight, LabelPlacement.TopLeft,
                LabelPlacement.BottomRight, LabelPlacement.BottomLeft
            }
        };
    }
}

/// <summary>
/// Represents a data label.
/// </summary>
public class DataLabel
{
    /// <summary>
    /// Gets or sets the data value.
    /// </summary>
    public required object Value { get; init; }

    /// <summary>
    /// Gets or sets the position to display the label.
    /// </summary>
    public required SKPoint Position { get; init; }

    /// <summary>
    /// Gets or sets the preferred label placement.
    /// </summary>
    public LabelPlacement Placement { get; init; } = LabelPlacement.Top;

    /// <summary>
    /// Gets or sets associated data (e.g., data point object).
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Gets the formatted text (calculated during layout).
    /// </summary>
    public string FormattedText { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the calculated bounds.
    /// </summary>
    public SKRect Bounds { get; internal set; }

    /// <summary>
    /// Gets whether the label is visible.
    /// </summary>
    public bool IsVisible { get; internal set; } = true;
}

/// <summary>
/// Label placement enumeration.
/// </summary>
public enum LabelPlacement
{
    /// <summary>Above the data point.</summary>
    Top,
    /// <summary>Below the data point.</summary>
    Bottom,
    /// <summary>Left of the data point.</summary>
    Left,
    /// <summary>Right of the data point.</summary>
    Right,
    /// <summary>Top-left of the data point.</summary>
    TopLeft,
    /// <summary>Top-right of the data point.</summary>
    TopRight,
    /// <summary>Bottom-left of the data point.</summary>
    BottomLeft,
    /// <summary>Bottom-right of the data point.</summary>
    BottomRight,
    /// <summary>Centered on the data point.</summary>
    Center
}
