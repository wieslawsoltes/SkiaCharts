namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the rendering mode for line charts.
/// </summary>
public enum LineMode
{
    /// <summary>
    /// Standard linear interpolation between points.
    /// </summary>
    Linear,

    /// <summary>
    /// Stepped line with horizontal then vertical segments.
    /// </summary>
    Stepped,

    /// <summary>
    /// Smooth curve using cubic Bezier interpolation.
    /// </summary>
    Smooth
}

/// <summary>
/// Defines marker shapes for line charts.
/// </summary>
public enum MarkerShape
{
    /// <summary>
    /// No marker.
    /// </summary>
    None,

    /// <summary>
    /// Circle marker.
    /// </summary>
    Circle,

    /// <summary>
    /// Square marker.
    /// </summary>
    Square,

    /// <summary>
    /// Diamond marker.
    /// </summary>
    Diamond,

    /// <summary>
    /// Triangle marker.
    /// </summary>
    Triangle,

    /// <summary>
    /// Inverted triangle marker.
    /// </summary>
    TriangleDown,

    /// <summary>
    /// Cross marker.
    /// </summary>
    Cross,

    /// <summary>
    /// Plus marker.
    /// </summary>
    Plus
}

/// <summary>
/// Configuration for a line series style.
/// </summary>
public class LineSeriesStyle
{
    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    public SkiaSharp.SKColor LineColor { get; set; } = SkiaSharp.SKColors.Blue;

    /// <summary>
    /// Gets or sets the line width in pixels.
    /// </summary>
    public float LineWidth { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the line rendering mode.
    /// </summary>
    public LineMode LineMode { get; set; } = LineMode.Linear;

    /// <summary>
    /// Gets or sets the marker shape.
    /// </summary>
    public MarkerShape MarkerShape { get; set; } = MarkerShape.Circle;

    /// <summary>
    /// Gets or sets the marker size in pixels.
    /// </summary>
    public float MarkerSize { get; set; } = 6f;

    /// <summary>
    /// Gets or sets the marker fill color (null uses line color).
    /// </summary>
    public SkiaSharp.SKColor? MarkerFillColor { get; set; }

    /// <summary>
    /// Gets or sets the marker stroke color (null for no stroke).
    /// </summary>
    public SkiaSharp.SKColor? MarkerStrokeColor { get; set; }

    /// <summary>
    /// Gets or sets the marker stroke width.
    /// </summary>
    public float MarkerStrokeWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets whether to fill the area under the line.
    /// </summary>
    public bool FillArea { get; set; } = false;

    /// <summary>
    /// Gets or sets the fill color (null uses line color with alpha).
    /// </summary>
    public SkiaSharp.SKColor? FillColor { get; set; }

    /// <summary>
    /// Gets or sets the fill alpha (0-255).
    /// </summary>
    public byte FillAlpha { get; set; } = 50;

    /// <summary>
    /// Gets or sets the dash pattern for the line (null for solid).
    /// Example: new[] { 10f, 5f } for 10px dash, 5px gap.
    /// </summary>
    public float[]? DashPattern { get; set; }

    /// <summary>
    /// Gets or sets the tension for smooth curves (0-1, default 0.5).
    /// Only applies when LineMode is Smooth.
    /// </summary>
    public float SmoothTension { get; set; } = 0.5f;
}
