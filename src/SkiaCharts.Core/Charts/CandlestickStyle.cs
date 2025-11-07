namespace SkiaCharts.Core.Charts;

/// <summary>
/// Defines the candle type for candlestick charts.
/// </summary>
public enum CandleType
{
    /// <summary>
    /// Traditional candlestick with hollow/filled bodies.
    /// </summary>
    Candlestick,

    /// <summary>
    /// OHLC bars (vertical line with left/right ticks).
    /// </summary>
    OhlcBar
}

/// <summary>
/// Configuration for candlestick/OHLC chart appearance.
/// </summary>
public class CandlestickSeriesStyle
{
    /// <summary>
    /// Gets or sets the candle type.
    /// </summary>
    public CandleType CandleType { get; set; } = CandleType.Candlestick;

    /// <summary>
    /// Gets or sets the bullish (up) candle color.
    /// </summary>
    public SkiaSharp.SKColor BullishColor { get; set; } = new SkiaSharp.SKColor(38, 166, 91); // Green

    /// <summary>
    /// Gets or sets the bearish (down) candle color.
    /// </summary>
    public SkiaSharp.SKColor BearishColor { get; set; } = new SkiaSharp.SKColor(239, 83, 80); // Red

    /// <summary>
    /// Gets or sets whether to use hollow candles for bullish moves.
    /// When true, bullish candles are hollow (outline only), bearish are filled.
    /// When false, all candles are filled.
    /// </summary>
    public bool UseHollowCandles { get; set; } = true;

    /// <summary>
    /// Gets or sets the wick (shadow) width in pixels.
    /// </summary>
    public float WickWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the body border width for hollow candles.
    /// </summary>
    public float BodyBorderWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the candle width ratio (0-1, relative to available space).
    /// Default is 0.7, leaving 30% spacing between candles.
    /// </summary>
    public double CandleWidthRatio { get; set; } = 0.7;

    /// <summary>
    /// Gets or sets the minimum candle width in pixels.
    /// </summary>
    public float MinimumCandleWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the maximum candle width in pixels (0 for unlimited).
    /// </summary>
    public float MaximumCandleWidth { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the OHLC bar tick width ratio (0-1, relative to candle width).
    /// Only applies when CandleType is OhlcBar.
    /// </summary>
    public double OhlcTickRatio { get; set; } = 0.5;
}

/// <summary>
/// Configuration for candlestick/OHLC chart layout.
/// </summary>
public class CandlestickChartConfiguration
{
    /// <summary>
    /// Gets or sets whether to show volume bars below the chart.
    /// </summary>
    public bool ShowVolume { get; set; } = false;

    /// <summary>
    /// Gets or sets the volume panel height ratio (0-1, relative to total height).
    /// Only applies when ShowVolume is true.
    /// </summary>
    public double VolumePanelRatio { get; set; } = 0.2;

    /// <summary>
    /// Gets or sets the volume bar color for up days.
    /// </summary>
    public SkiaSharp.SKColor VolumeUpColor { get; set; } = new SkiaSharp.SKColor(38, 166, 91, 100); // Semi-transparent green

    /// <summary>
    /// Gets or sets the volume bar color for down days.
    /// </summary>
    public SkiaSharp.SKColor VolumeDownColor { get; set; } = new SkiaSharp.SKColor(239, 83, 80, 100); // Semi-transparent red

    /// <summary>
    /// Gets or sets whether to show high/low labels on candles.
    /// </summary>
    public bool ShowHighLowLabels { get; set; } = false;

    /// <summary>
    /// Gets or sets the high/low label font size.
    /// </summary>
    public float HighLowLabelFontSize { get; set; } = 8f;

    /// <summary>
    /// Gets or sets the high/low label color.
    /// </summary>
    public SkiaSharp.SKColor HighLowLabelColor { get; set; } = SkiaSharp.SKColors.Gray;
}
