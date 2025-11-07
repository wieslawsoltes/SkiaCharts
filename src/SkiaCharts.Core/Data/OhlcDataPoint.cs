namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents an OHLC (Open, High, Low, Close) data point for financial charts.
/// </summary>
public readonly struct OhlcDataPoint : IDataPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OhlcDataPoint"/> struct.
    /// </summary>
    /// <param name="x">The X-coordinate value (typically time).</param>
    /// <param name="open">The opening price.</param>
    /// <param name="high">The highest price.</param>
    /// <param name="low">The lowest price.</param>
    /// <param name="close">The closing price.</param>
    /// <param name="volume">The trading volume (optional).</param>
    public OhlcDataPoint(double x, double open, double high, double low, double close, double volume = 0)
    {
        X = x;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }

    /// <inheritdoc/>
    public double X { get; }

    /// <inheritdoc/>
    /// <remarks>Returns the Close price as the Y value.</remarks>
    public double Y => Close;

    /// <summary>
    /// Gets the opening price.
    /// </summary>
    public double Open { get; }

    /// <summary>
    /// Gets the highest price.
    /// </summary>
    public double High { get; }

    /// <summary>
    /// Gets the lowest price.
    /// </summary>
    public double Low { get; }

    /// <summary>
    /// Gets the closing price.
    /// </summary>
    public double Close { get; }

    /// <summary>
    /// Gets the trading volume.
    /// </summary>
    public double Volume { get; }

    /// <summary>
    /// Gets a value indicating whether this is a bullish candle (Close > Open).
    /// </summary>
    public bool IsBullish => Close >= Open;

    /// <summary>
    /// Returns a string representation of the OHLC data point.
    /// </summary>
    public override string ToString() => $"OHLC({X}: O={Open}, H={High}, L={Low}, C={Close}, V={Volume})";
}
