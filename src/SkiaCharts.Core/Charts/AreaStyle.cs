using SkiaCharts.Core.Theming;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the stacking mode for area charts.
/// </summary>
public enum AreaStackMode
{
    /// <summary>
    /// No stacking - areas overlap.
    /// </summary>
    None,

    /// <summary>
    /// Areas stack on top of each other.
    /// </summary>
    Stacked
}

/// <summary>
/// Defines the rendering mode for area boundaries.
/// </summary>
public enum AreaMode
{
    /// <summary>
    /// Standard linear interpolation between points.
    /// </summary>
    Linear,

    /// <summary>
    /// Stepped area with horizontal then vertical segments.
    /// </summary>
    Stepped,

    /// <summary>
    /// Smooth curve using cubic Bezier interpolation.
    /// </summary>
    Smooth
}

/// <summary>
/// Defines gradient direction for area fills.
/// </summary>
public enum GradientDirection
{
    /// <summary>
    /// Vertical gradient (top to bottom).
    /// </summary>
    Vertical,

    /// <summary>
    /// Horizontal gradient (left to right).
    /// </summary>
    Horizontal,

    /// <summary>
    /// Radial gradient from center.
    /// </summary>
    Radial
}

/// <summary>
/// Configuration for an area series style.
/// </summary>
public class AreaSeriesStyle
{
    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public SkiaSharp.SKColor FillColor { get; set; } = SkiaSharp.SKColors.Blue;

    /// <summary>
    /// Gets or sets the fill alpha (0-255).
    /// </summary>
    public byte FillAlpha { get; set; } = 100;

    /// <summary>
    /// Gets or sets the line color for the boundary.
    /// </summary>
    public SkiaSharp.SKColor LineColor { get; set; } = SkiaSharp.SKColors.Blue;

    /// <summary>
    /// Gets or sets the line width in pixels.
    /// </summary>
    public float LineWidth { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the area rendering mode.
    /// </summary>
    public AreaMode AreaMode { get; set; } = AreaMode.Linear;

    /// <summary>
    /// Gets or sets whether to draw the boundary line.
    /// </summary>
    public bool ShowLine { get; set; } = true;

    /// <summary>
    /// Gets or sets the gradient colors for gradient fills (null for solid fill).
    /// If set, creates a gradient from first to last color.
    /// </summary>
    public SkiaSharp.SKColor[]? GradientColors { get; set; }

    /// <summary>
    /// Gets or sets the gradient direction.
    /// Only applies when GradientColors is set.
    /// </summary>
    public GradientDirection GradientDirection { get; set; } = GradientDirection.Vertical;

    /// <summary>
    /// Gets or sets the tension for smooth curves (0-1, default 0.5).
    /// Only applies when AreaMode is Smooth.
    /// </summary>
    public float SmoothTension { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the dash pattern for the boundary line (null for solid).
    /// Example: new[] { 10f, 5f } for 10px dash, 5px gap.
    /// </summary>
    public float[]? DashPattern { get; set; }

    /// <summary>
    /// Gets or sets the baseline value (default 0).
    /// Areas are filled from this value to the data points.
    /// </summary>
    public double Baseline { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the pattern fill type (null for no pattern override).
    /// </summary>
    public PatternType? FillPattern { get; set; }

    /// <summary>
    /// Gets or sets the pattern scale factor (default 1.0).
    /// </summary>
    public float PatternScale { get; set; } = 1.0f;
}

/// <summary>
/// Configuration for area chart layout.
/// </summary>
public class AreaChartConfiguration
{
    /// <summary>
    /// Gets or sets the stacking mode.
    /// </summary>
    public AreaStackMode StackMode { get; set; } = AreaStackMode.None;

    /// <summary>
    /// Gets or sets whether to handle negative values by splitting areas.
    /// When true, positive and negative areas are rendered separately.
    /// </summary>
    public bool SplitNegativeAreas { get; set; } = true;
}
