using SkiaCharts.Core.Axes;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the chart type for a series in a combo chart.
/// </summary>
public enum ComboSeriesType
{
    /// <summary>
    /// Line chart series.
    /// </summary>
    Line,

    /// <summary>
    /// Bar/Column chart series.
    /// </summary>
    Bar,

    /// <summary>
    /// Area chart series.
    /// </summary>
    Area,

    /// <summary>
    /// Scatter chart series.
    /// </summary>
    Scatter
}

/// <summary>
/// Defines which Y-axis a series should use in a combo chart.
/// </summary>
public enum YAxisSide
{
    /// <summary>
    /// Primary Y-axis (left side).
    /// </summary>
    Left,

    /// <summary>
    /// Secondary Y-axis (right side).
    /// </summary>
    Right
}

/// <summary>
/// Configuration for a series in a combo chart.
/// </summary>
public class ComboSeriesConfiguration
{
    /// <summary>
    /// Gets or sets the chart type for this series.
    /// </summary>
    public ComboSeriesType ChartType { get; set; } = ComboSeriesType.Line;

    /// <summary>
    /// Gets or sets which Y-axis this series should use.
    /// </summary>
    public YAxisSide YAxisSide { get; set; } = YAxisSide.Left;

    /// <summary>
    /// Gets or sets the line series style (when ChartType is Line).
    /// </summary>
    public LineSeriesStyle? LineStyle { get; set; }

    /// <summary>
    /// Gets or sets the bar series style (when ChartType is Bar).
    /// </summary>
    public BarSeriesStyle? BarStyle { get; set; }

    /// <summary>
    /// Gets or sets the area series style (when ChartType is Area).
    /// </summary>
    public AreaSeriesStyle? AreaStyle { get; set; }

    /// <summary>
    /// Gets or sets the scatter series style (when ChartType is Scatter).
    /// </summary>
    public ScatterSeriesStyle? ScatterStyle { get; set; }
}

/// <summary>
/// Configuration for combo chart layout.
/// </summary>
public class ComboChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to show the secondary Y-axis.
    /// When false, all series use the primary (left) Y-axis.
    /// </summary>
    public bool ShowSecondaryYAxis { get; set; } = false;

    /// <summary>
    /// Gets or sets the primary (left) Y-axis.
    /// </summary>
    public IAxis? PrimaryYAxis { get; set; }

    /// <summary>
    /// Gets or sets the secondary (right) Y-axis.
    /// Only used when ShowSecondaryYAxis is true.
    /// </summary>
    public IAxis? SecondaryYAxis { get; set; }

    /// <summary>
    /// Gets or sets whether to synchronize the zoom level between Y-axes.
    /// When true, zooming one axis will proportionally zoom the other.
    /// </summary>
    public bool SynchronizeYAxes { get; set; } = false;

    /// <summary>
    /// Gets or sets the bar orientation for bar series.
    /// </summary>
    public BarOrientation BarOrientation { get; set; } = BarOrientation.Vertical;

    /// <summary>
    /// Gets or sets whether bar series should be stacked.
    /// Only applies to bar series on the same Y-axis.
    /// </summary>
    public bool StackBars { get; set; } = false;
}
