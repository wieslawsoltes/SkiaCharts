using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Base class for all technical indicators.
/// </summary>
public abstract class IndicatorBase : IIndicator
{
    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <summary>
    /// Gets or sets the indicator color.
    /// </summary>
    public SKColor Color { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the line width.
    /// </summary>
    public float LineWidth { get; set; } = 1.5f;

    /// <summary>
    /// Gets or sets whether the indicator is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the line dash pattern (null for solid line).
    /// </summary>
    public float[]? DashPattern { get; set; }

    /// <inheritdoc/>
    public abstract IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series);

    /// <summary>
    /// Helper method to convert data points to OHLC data points.
    /// </summary>
    protected OhlcDataPoint ToOhlc(IDataPoint point)
    {
        return point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y);
    }

    /// <summary>
    /// Helper method to get the close price from a data point.
    /// </summary>
    protected double GetClose(IDataPoint point)
    {
        if (point is OhlcDataPoint ohlc)
            return ohlc.Close;
        return point.Y;
    }

    /// <summary>
    /// Helper method to get the high price from a data point.
    /// </summary>
    protected double GetHigh(IDataPoint point)
    {
        if (point is OhlcDataPoint ohlc)
            return ohlc.High;
        return point.Y;
    }

    /// <summary>
    /// Helper method to get the low price from a data point.
    /// </summary>
    protected double GetLow(IDataPoint point)
    {
        if (point is OhlcDataPoint ohlc)
            return ohlc.Low;
        return point.Y;
    }

    /// <summary>
    /// Helper method to get the volume from a data point.
    /// </summary>
    protected double GetVolume(IDataPoint point)
    {
        if (point is OhlcDataPoint ohlc)
            return ohlc.Volume;
        return 0;
    }
}

/// <summary>
/// Base class for overlay indicators.
/// </summary>
public abstract class OverlayIndicatorBase : IndicatorBase, IOverlayIndicator
{
}

/// <summary>
/// Base class for panel indicators.
/// </summary>
public abstract class PanelIndicatorBase : IndicatorBase, IPanelIndicator
{
    /// <inheritdoc/>
    public double MinValue { get; protected set; }

    /// <inheritdoc/>
    public double MaxValue { get; protected set; }
}
