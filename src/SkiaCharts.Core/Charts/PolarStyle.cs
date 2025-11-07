using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Configuration for a polar series style.
/// </summary>
public class PolarSeriesStyle
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
    /// Gets or sets whether to show markers at data points.
    /// </summary>
    public bool ShowMarkers { get; set; } = false;

    /// <summary>
    /// Gets or sets the marker size in pixels.
    /// </summary>
    public float MarkerSize { get; set; } = 6f;

    /// <summary>
    /// Gets or sets the marker color (null uses line color).
    /// </summary>
    public SKColor? MarkerColor { get; set; }

    /// <summary>
    /// Gets or sets the dash pattern for the line (null for solid).
    /// </summary>
    public float[]? DashPattern { get; set; }
}

/// <summary>
/// Configuration for polar chart layout.
/// </summary>
public class PolarChartConfiguration
{
    /// <summary>
    /// Gets or sets the number of angle grid lines (radial spokes).
    /// </summary>
    public int AngleGridLines { get; set; } = 12; // 30-degree increments

    /// <summary>
    /// Gets or sets the number of radius grid circles.
    /// </summary>
    public int RadiusGridCircles { get; set; } = 5;

    /// <summary>
    /// Gets or sets the grid line color.
    /// </summary>
    public SKColor GridLineColor { get; set; } = new SKColor(200, 200, 200);

    /// <summary>
    /// Gets or sets the grid line width.
    /// </summary>
    public float GridLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets whether to show angle labels (degrees).
    /// </summary>
    public bool ShowAngleLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets the angle label font size.
    /// </summary>
    public float AngleLabelFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the angle label color.
    /// </summary>
    public SKColor AngleLabelColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets whether angles are in degrees (true) or radians (false).
    /// Data X values are interpreted accordingly.
    /// </summary>
    public bool AngleInDegrees { get; set; } = true;

    /// <summary>
    /// Gets or sets the starting angle offset in degrees (0 = right/east, 90 = up/north).
    /// </summary>
    public float StartAngle { get; set; } = 90f; // North/up

    /// <summary>
    /// Gets or sets whether angles increase clockwise (true) or counterclockwise (false).
    /// </summary>
    public bool Clockwise { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum radius value (null = auto from data).
    /// </summary>
    public double? MaxRadius { get; set; }

    /// <summary>
    /// Gets or sets the minimum radius value.
    /// </summary>
    public double MinRadius { get; set; } = 0;

    /// <summary>
    /// Gets or sets the padding ratio (0-1) to add space around the chart.
    /// </summary>
    public float PaddingRatio { get; set; } = 0.1f;
}
