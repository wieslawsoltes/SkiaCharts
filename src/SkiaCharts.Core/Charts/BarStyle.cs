namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the stacking mode for bar/column charts.
/// </summary>
public enum BarStackMode
{
    /// <summary>
    /// No stacking - bars are side-by-side (grouped/clustered).
    /// </summary>
    None,

    /// <summary>
    /// Absolute stacking - bars stack on top of each other with actual values.
    /// </summary>
    Absolute,

    /// <summary>
    /// Percentage stacking - bars stack to 100%, showing relative proportions.
    /// </summary>
    Percentage
}

/// <summary>
/// Defines the orientation for bar charts.
/// </summary>
public enum BarOrientation
{
    /// <summary>
    /// Vertical bars (column chart).
    /// </summary>
    Vertical,

    /// <summary>
    /// Horizontal bars (bar chart).
    /// </summary>
    Horizontal
}

/// <summary>
/// Configuration for a bar series style.
/// </summary>
public class BarSeriesStyle
{
    /// <summary>
    /// Gets or sets the fill color.
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
    /// Gets or sets the corner radius for rounded corners (0 for sharp corners).
    /// </summary>
    public float CornerRadius { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the bar width ratio (0-1, relative to available space).
    /// Default is 0.8, leaving 20% spacing between bars.
    /// </summary>
    public double BarWidthRatio { get; set; } = 0.8;

    /// <summary>
    /// Gets or sets the gradient colors for gradient fills (null for solid fill).
    /// If set, creates a linear gradient from first to last color.
    /// </summary>
    public SkiaSharp.SKColor[]? GradientColors { get; set; }

    /// <summary>
    /// Gets or sets the gradient angle in degrees (0 = left-to-right, 90 = bottom-to-top).
    /// Only applies when GradientColors is set.
    /// </summary>
    public float GradientAngle { get; set; } = 90f;

    /// <summary>
    /// Gets or sets the minimum bar size in pixels (prevents invisible bars for small values).
    /// </summary>
    public float MinimumBarSize { get; set; } = 1f;
}

/// <summary>
/// Configuration for bar/column chart layout.
/// </summary>
public class BarChartConfiguration
{
    /// <summary>
    /// Gets or sets the stacking mode.
    /// </summary>
    public BarStackMode StackMode { get; set; } = BarStackMode.None;

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    public BarOrientation Orientation { get; set; } = BarOrientation.Vertical;

    /// <summary>
    /// Gets or sets the group spacing ratio (0-1, spacing between groups of bars).
    /// Only applies when StackMode is None (grouped bars).
    /// </summary>
    public double GroupSpacing { get; set; } = 0.2;

    /// <summary>
    /// Gets or sets whether to show value labels on bars.
    /// </summary>
    public bool ShowValueLabels { get; set; } = false;

    /// <summary>
    /// Gets or sets the value label format string (e.g., "0.00", "N2", "P0").
    /// </summary>
    public string ValueLabelFormat { get; set; } = "0.##";

    /// <summary>
    /// Gets or sets the value label color.
    /// </summary>
    public SkiaSharp.SKColor ValueLabelColor { get; set; } = SkiaSharp.SKColors.Black;

    /// <summary>
    /// Gets or sets the value label font size.
    /// </summary>
    public float ValueLabelFontSize { get; set; } = 10f;
}
