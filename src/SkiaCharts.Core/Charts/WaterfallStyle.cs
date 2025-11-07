using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the type of bar in a waterfall chart.
/// </summary>
public enum WaterfallBarType
{
    /// <summary>
    /// Automatic determination based on value (positive/negative).
    /// </summary>
    Automatic,

    /// <summary>
    /// Positive increase (green/up).
    /// </summary>
    Positive,

    /// <summary>
    /// Negative decrease (red/down).
    /// </summary>
    Negative,

    /// <summary>
    /// Total/subtotal bar (different color).
    /// </summary>
    Total
}

/// <summary>
/// Configuration for a waterfall bar.
/// </summary>
public class WaterfallBarConfiguration
{
    /// <summary>
    /// Gets or sets the bar type.
    /// </summary>
    public WaterfallBarType BarType { get; set; } = WaterfallBarType.Automatic;

    /// <summary>
    /// Gets or sets whether this bar is a total/subtotal.
    /// </summary>
    public bool IsTotal { get; set; } = false;

    /// <summary>
    /// Gets or sets the label for this bar.
    /// </summary>
    public string? Label { get; set; }
}

/// <summary>
/// Style configuration for waterfall series.
/// </summary>
public class WaterfallSeriesStyle
{
    /// <summary>
    /// Gets or sets the color for positive values.
    /// </summary>
    public SKColor PositiveColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the color for negative values.
    /// </summary>
    public SKColor NegativeColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets the color for total bars.
    /// </summary>
    public SKColor TotalColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the bar width as a ratio of available space (0-1).
    /// </summary>
    public float BarWidthRatio { get; set; } = 0.7f;

    /// <summary>
    /// Gets or sets whether to show connector lines between bars.
    /// </summary>
    public bool ShowConnectorLines { get; set; } = true;

    /// <summary>
    /// Gets or sets the connector line color.
    /// </summary>
    public SKColor ConnectorLineColor { get; set; } = new SKColor(150, 150, 150);

    /// <summary>
    /// Gets or sets the connector line width.
    /// </summary>
    public float ConnectorLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the connector line dash pattern (null for solid).
    /// </summary>
    public float[]? ConnectorDashPattern { get; set; } = new[] { 5f, 3f };

    /// <summary>
    /// Gets or sets the bar border color (null for no border).
    /// </summary>
    public SKColor? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the bar border width.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the corner radius for rounded bars.
    /// </summary>
    public float CornerRadius { get; set; } = 0f;

    /// <summary>
    /// Gets or sets whether to show value labels on bars.
    /// </summary>
    public bool ShowValueLabels { get; set; } = false;

    /// <summary>
    /// Gets or sets the value label font size.
    /// </summary>
    public float ValueLabelFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the value label color.
    /// </summary>
    public SKColor ValueLabelColor { get; set; } = SKColors.Black;
}

/// <summary>
/// Configuration for waterfall chart layout.
/// </summary>
public class WaterfallChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to start from zero or from the first value.
    /// </summary>
    public bool StartFromZero { get; set; } = true;

    /// <summary>
    /// Gets or sets the category labels for X-axis.
    /// </summary>
    public string[]? CategoryLabels { get; set; }

    /// <summary>
    /// Gets or sets whether to show category labels.
    /// </summary>
    public bool ShowCategoryLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets the category label font size.
    /// </summary>
    public float CategoryLabelFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the category label color.
    /// </summary>
    public SKColor CategoryLabelColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the category label rotation angle in degrees.
    /// </summary>
    public float CategoryLabelRotation { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the padding ratio (0-1) to add space around bars.
    /// </summary>
    public float PaddingRatio { get; set; } = 0.1f;
}
