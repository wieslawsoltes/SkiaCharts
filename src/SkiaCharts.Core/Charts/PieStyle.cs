namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the label position for pie/donut chart slices.
/// </summary>
public enum PieLabelPosition
{
    /// <summary>
    /// No labels displayed.
    /// </summary>
    None,

    /// <summary>
    /// Labels positioned inside the slice.
    /// </summary>
    Inside,

    /// <summary>
    /// Labels positioned outside with leader lines.
    /// </summary>
    Outside
}

/// <summary>
/// Defines the label content for pie/donut chart slices.
/// </summary>
public enum PieLabelContent
{
    /// <summary>
    /// Show percentage only.
    /// </summary>
    Percentage,

    /// <summary>
    /// Show value only.
    /// </summary>
    Value,

    /// <summary>
    /// Show both percentage and value.
    /// </summary>
    Both,

    /// <summary>
    /// Show label name only.
    /// </summary>
    Name,

    /// <summary>
    /// Show name and percentage.
    /// </summary>
    NameAndPercentage,

    /// <summary>
    /// Show name and value.
    /// </summary>
    NameAndValue
}

/// <summary>
/// Configuration for a pie/donut slice style.
/// </summary>
public class PieSliceStyle
{
    /// <summary>
    /// Gets or sets the fill color for the slice.
    /// </summary>
    public SkiaSharp.SKColor FillColor { get; set; } = SkiaSharp.SKColors.Blue;

    /// <summary>
    /// Gets or sets the border color (null for no border).
    /// </summary>
    public SkiaSharp.SKColor? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the border width in pixels.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the gradient colors for gradient fills (null for solid fill).
    /// Creates a radial gradient from center to edge.
    /// </summary>
    public SkiaSharp.SKColor[]? GradientColors { get; set; }

    /// <summary>
    /// Gets or sets the explode distance in pixels (0 for no explosion).
    /// </summary>
    public float ExplodeDistance { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the label for this slice (optional).
    /// </summary>
    public string? Label { get; set; }
}

/// <summary>
/// Configuration for pie/donut chart layout and appearance.
/// </summary>
public class PieChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to render as a donut chart.
    /// </summary>
    public bool IsDonut { get; set; } = false;

    /// <summary>
    /// Gets or sets the inner radius ratio for donut charts (0-1).
    /// 0 = pie chart, 0.5 = half donut, 0.9 = thin ring.
    /// Only applies when IsDonut is true.
    /// </summary>
    public double InnerRadiusRatio { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the start angle in degrees (0 = right, 90 = top, 180 = left, 270 = bottom).
    /// </summary>
    public float StartAngle { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the label position.
    /// </summary>
    public PieLabelPosition LabelPosition { get; set; } = PieLabelPosition.Outside;

    /// <summary>
    /// Gets or sets the label content type.
    /// </summary>
    public PieLabelContent LabelContent { get; set; } = PieLabelContent.Percentage;

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; } = 12f;

    /// <summary>
    /// Gets or sets the label color.
    /// </summary>
    public SkiaSharp.SKColor LabelColor { get; set; } = SkiaSharp.SKColors.Black;

    /// <summary>
    /// Gets or sets the value format string for labels (e.g., "0.00", "N2").
    /// </summary>
    public string ValueFormat { get; set; } = "0.##";

    /// <summary>
    /// Gets or sets the percentage format string for labels (e.g., "0.0%", "P1").
    /// </summary>
    public string PercentageFormat { get; set; } = "0.0%";

    /// <summary>
    /// Gets or sets the leader line color for outside labels.
    /// </summary>
    public SkiaSharp.SKColor LeaderLineColor { get; set; } = SkiaSharp.SKColors.Gray;

    /// <summary>
    /// Gets or sets the leader line width in pixels.
    /// </summary>
    public float LeaderLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the leader line length in pixels.
    /// </summary>
    public float LeaderLineLength { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the minimum slice angle in degrees to show a label.
    /// Slices smaller than this will not have labels to avoid overcrowding.
    /// </summary>
    public float MinimumLabelAngle { get; set; } = 5f;

    /// <summary>
    /// Gets or sets the radius ratio (0-1) relative to the available space.
    /// Default is 0.9, leaving 10% padding around the chart.
    /// </summary>
    public double RadiusRatio { get; set; } = 0.9;
}

/// <summary>
/// Data point for pie/donut charts with a single value and optional label.
/// </summary>
public class PieDataPoint : Data.IDataPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PieDataPoint"/> class.
    /// </summary>
    /// <param name="value">The value for this slice.</param>
    /// <param name="label">Optional label for this slice.</param>
    public PieDataPoint(double value, string? label = null)
    {
        // For pie charts, X is the index and Y is the value
        X = 0; // Will be set by the chart
        Y = value;
        Label = label;
    }

    /// <inheritdoc/>
    public double X { get; internal set; }

    /// <inheritdoc/>
    public double Y { get; }

    /// <summary>
    /// Gets the label for this slice.
    /// </summary>
    public string? Label { get; }
}
