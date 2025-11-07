namespace SkiaCharts.Core.Charts;

/// <summary>
/// Configuration for a scatter series style.
/// </summary>
public class ScatterSeriesStyle
{
    /// <summary>
    /// Gets or sets the marker shape.
    /// </summary>
    public MarkerShape MarkerShape { get; set; } = MarkerShape.Circle;

    /// <summary>
    /// Gets or sets the marker size in pixels.
    /// </summary>
    public float MarkerSize { get; set; } = 8f;

    /// <summary>
    /// Gets or sets the marker fill color.
    /// </summary>
    public SkiaSharp.SKColor FillColor { get; set; } = SkiaSharp.SKColors.Blue;

    /// <summary>
    /// Gets or sets the marker border color (null for no border).
    /// </summary>
    public SkiaSharp.SKColor? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the marker border width.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets whether to use variable marker sizes based on data.
    /// When true, uses the Z value from data points to determine size.
    /// </summary>
    public bool UseVariableSizes { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum marker size for variable sizing (in pixels).
    /// </summary>
    public float MinMarkerSize { get; set; } = 4f;

    /// <summary>
    /// Gets or sets the maximum marker size for variable sizing (in pixels).
    /// </summary>
    public float MaxMarkerSize { get; set; } = 20f;

    /// <summary>
    /// Gets or sets whether to use color mapping based on data values.
    /// When true, uses a color scale to map values to colors.
    /// </summary>
    public bool UseColorMapping { get; set; } = false;

    /// <summary>
    /// Gets or sets the color scale for color mapping (null for default blue-red scale).
    /// Minimum value maps to first color, maximum to last color.
    /// </summary>
    public SkiaSharp.SKColor[]? ColorScale { get; set; }
}

/// <summary>
/// Configuration for scatter chart layout.
/// </summary>
public class ScatterChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to show connecting lines between points.
    /// </summary>
    public bool ShowConnectingLines { get; set; } = false;

    /// <summary>
    /// Gets or sets the connecting line color.
    /// </summary>
    public SkiaSharp.SKColor ConnectingLineColor { get; set; } = SkiaSharp.SKColors.Gray;

    /// <summary>
    /// Gets or sets the connecting line width.
    /// </summary>
    public float ConnectingLineWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the connecting line alpha (0-255).
    /// </summary>
    public byte ConnectingLineAlpha { get; set; } = 128;
}

/// <summary>
/// Extended data point for scatter charts with optional size and color values.
/// </summary>
public class ScatterDataPoint : Data.IDataPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterDataPoint"/> class.
    /// </summary>
    public ScatterDataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterDataPoint"/> class with size.
    /// </summary>
    public ScatterDataPoint(double x, double y, double size)
    {
        X = x;
        Y = y;
        Size = size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterDataPoint"/> class with size and color value.
    /// </summary>
    public ScatterDataPoint(double x, double y, double size, double colorValue)
    {
        X = x;
        Y = y;
        Size = size;
        ColorValue = colorValue;
    }

    /// <inheritdoc/>
    public double X { get; }

    /// <inheritdoc/>
    public double Y { get; }

    /// <summary>
    /// Gets the size value for variable sizing.
    /// </summary>
    public double Size { get; }

    /// <summary>
    /// Gets the color value for color mapping.
    /// </summary>
    public double ColorValue { get; }
}
