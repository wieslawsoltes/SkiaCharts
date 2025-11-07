using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Configuration for a radar series style.
/// </summary>
public class RadarSeriesStyle
{
    /// <summary>
    /// Gets or sets the line color.
    /// </summary>
    public SKColor LineColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the line width in pixels.
    /// </summary>
    public float LineWidth { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the fill color for the area.
    /// </summary>
    public SKColor FillColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the fill alpha (0-255).
    /// </summary>
    public byte FillAlpha { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to fill the area.
    /// </summary>
    public bool FillArea { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show markers at data points.
    /// </summary>
    public bool ShowMarkers { get; set; } = true;

    /// <summary>
    /// Gets or sets the marker size in pixels.
    /// </summary>
    public float MarkerSize { get; set; } = 6f;

    /// <summary>
    /// Gets or sets the marker fill color (null uses line color).
    /// </summary>
    public SKColor? MarkerFillColor { get; set; }

    /// <summary>
    /// Gets or sets the dash pattern for the line (null for solid).
    /// </summary>
    public float[]? DashPattern { get; set; }
}

/// <summary>
/// Configuration for radar chart layout.
/// </summary>
public class RadarChartConfiguration
{
    /// <summary>
    /// Gets or sets the axis labels for each spoke.
    /// </summary>
    public string[]? AxisLabels { get; set; }

    /// <summary>
    /// Gets or sets the number of circular grid levels.
    /// </summary>
    public int GridLevels { get; set; } = 5;

    /// <summary>
    /// Gets or sets the grid line color.
    /// </summary>
    public SKColor GridLineColor { get; set; } = new SKColor(200, 200, 200);

    /// <summary>
    /// Gets or sets the grid line width.
    /// </summary>
    public float GridLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets whether to show spoke lines (radial lines).
    /// </summary>
    public bool ShowSpokeLines { get; set; } = true;

    /// <summary>
    /// Gets or sets the spoke line color.
    /// </summary>
    public SKColor SpokeLineColor { get; set; } = new SKColor(200, 200, 200);

    /// <summary>
    /// Gets or sets the spoke line width.
    /// </summary>
    public float SpokeLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets whether to show axis labels.
    /// </summary>
    public bool ShowAxisLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets the axis label font size.
    /// </summary>
    public float AxisLabelFontSize { get; set; } = 12f;

    /// <summary>
    /// Gets or sets the axis label color.
    /// </summary>
    public SKColor AxisLabelColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the label offset from the chart edge.
    /// </summary>
    public float LabelOffset { get; set; } = 15f;

    /// <summary>
    /// Gets or sets the starting angle in degrees (0 = top, clockwise).
    /// </summary>
    public float StartAngle { get; set; } = -90f; // Top

    /// <summary>
    /// Gets or sets the minimum value for the radar chart.
    /// </summary>
    public double MinValue { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum value for the radar chart (null = auto from data).
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// Gets or sets the padding ratio (0-1) to add space around the chart.
    /// </summary>
    public float PaddingRatio { get; set; } = 0.1f;
}
