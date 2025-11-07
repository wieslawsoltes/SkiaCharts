using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the interpolation mode for heatmap rendering.
/// </summary>
public enum HeatmapInterpolation
{
    /// <summary>
    /// Nearest neighbor - each cell has a solid color, no smoothing.
    /// </summary>
    Nearest,

    /// <summary>
    /// Bilinear interpolation - smooth color gradients between cells.
    /// </summary>
    Bilinear,

    /// <summary>
    /// Bicubic interpolation - smoother gradients using cubic curves.
    /// (Note: This is approximated using bilinear for performance)
    /// </summary>
    Bicubic
}

/// <summary>
/// Configuration for heatmap series style.
/// </summary>
public class HeatmapSeriesStyle
{
    /// <summary>
    /// Gets or sets the color scale for value mapping.
    /// If null, uses a default gradient (blue → cyan → green → yellow → red).
    /// </summary>
    public SKColor[]? ColorScale { get; set; }

    /// <summary>
    /// Gets or sets the interpolation mode.
    /// </summary>
    public HeatmapInterpolation Interpolation { get; set; } = HeatmapInterpolation.Nearest;

    /// <summary>
    /// Gets or sets whether to show cell borders.
    /// </summary>
    public bool ShowCellBorders { get; set; } = false;

    /// <summary>
    /// Gets or sets the cell border color.
    /// </summary>
    public SKColor CellBorderColor { get; set; } = new SKColor(200, 200, 200);

    /// <summary>
    /// Gets or sets the cell border width in pixels.
    /// </summary>
    public float CellBorderWidth { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets whether to show cell values as text.
    /// </summary>
    public bool ShowCellValues { get; set; } = false;

    /// <summary>
    /// Gets or sets the cell value text color.
    /// </summary>
    public SKColor CellValueColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the cell value font size.
    /// </summary>
    public float CellValueFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the cell value format string.
    /// </summary>
    public string CellValueFormat { get; set; } = "F1";

    /// <summary>
    /// Gets or sets the minimum cell size to show values.
    /// Values are hidden for smaller cells to avoid clutter.
    /// </summary>
    public float MinCellSizeForValues { get; set; } = 20f;

    /// <summary>
    /// Gets or sets whether to show contour lines.
    /// </summary>
    public bool ShowContourLines { get; set; } = false;

    /// <summary>
    /// Gets or sets the contour line color.
    /// </summary>
    public SKColor ContourLineColor { get; set; } = new SKColor(100, 100, 100);

    /// <summary>
    /// Gets or sets the contour line width.
    /// </summary>
    public float ContourLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the number of contour levels.
    /// </summary>
    public int ContourLevels { get; set; } = 5;
}

/// <summary>
/// Configuration for heatmap chart layout.
/// </summary>
public class HeatmapChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to show the color legend/scale bar.
    /// </summary>
    public bool ShowColorLegend { get; set; } = true;

    /// <summary>
    /// Gets or sets the color legend position.
    /// </summary>
    public LegendPosition LegendPosition { get; set; } = LegendPosition.Right;

    /// <summary>
    /// Gets or sets the color legend width in pixels.
    /// </summary>
    public float LegendWidth { get; set; } = 60f;

    /// <summary>
    /// Gets or sets the color legend height (as ratio of chart height).
    /// </summary>
    public float LegendHeightRatio { get; set; } = 0.8f;

    /// <summary>
    /// Gets or sets the spacing between chart and legend in pixels.
    /// </summary>
    public float LegendSpacing { get; set; } = 10f;

    /// <summary>
    /// Gets or sets whether to show X-axis labels.
    /// </summary>
    public bool ShowXLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show Y-axis labels.
    /// </summary>
    public bool ShowYLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum value for color mapping (null = auto from data).
    /// </summary>
    public double? MinValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for color mapping (null = auto from data).
    /// </summary>
    public double? MaxValue { get; set; }
}

/// <summary>
/// Defines the position of the legend.
/// </summary>
public enum LegendPosition
{
    /// <summary>
    /// Legend on the right side.
    /// </summary>
    Right,

    /// <summary>
    /// Legend on the left side.
    /// </summary>
    Left,

    /// <summary>
    /// Legend on the bottom.
    /// </summary>
    Bottom,

    /// <summary>
    /// Legend on the top.
    /// </summary>
    Top,

    /// <summary>
    /// No legend.
    /// </summary>
    None
}
