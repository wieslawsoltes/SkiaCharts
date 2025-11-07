using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a recognized candlestick pattern.
/// </summary>
public enum CandlestickPattern
{
    None,
    Doji,
    Hammer,
    InvertedHammer,
    ShootingStar,
    HangingMan,
    BullishEngulfing,
    BearishEngulfing,
    MorningStar,
    EveningStar,
    ThreeWhiteSoldiers,
    ThreeBlackCrows
}

/// <summary>
/// Represents a detected pattern with its location.
/// </summary>
public class PatternDetection
{
    public CandlestickPattern Pattern { get; set; }
    public int Index { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsBullish { get; set; }
}

/// <summary>
/// Configuration for pattern recognition visual markers.
/// </summary>
public class PatternRecognitionConfiguration
{
    /// <summary>
    /// Gets or sets whether to show pattern markers.
    /// </summary>
    public bool ShowPatternMarkers { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show pattern labels.
    /// </summary>
    public bool ShowPatternLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets the marker size.
    /// </summary>
    public float MarkerSize { get; set; } = 8f;

    /// <summary>
    /// Gets or sets the bullish pattern color.
    /// </summary>
    public SKColor BullishPatternColor { get; set; } = SKColors.LimeGreen;

    /// <summary>
    /// Gets or sets the bearish pattern color.
    /// </summary>
    public SKColor BearishPatternColor { get; set; } = SKColors.OrangeRed;

    /// <summary>
    /// Gets or sets the label font size.
    /// </summary>
    public float LabelFontSize { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the label offset from candle.
    /// </summary>
    public float LabelOffset { get; set; } = 15f;
}

/// <summary>
/// Candlestick pattern recognition engine.
/// </summary>
public static class CandlestickPatternRecognizer
{
    /// <summary>
    /// Detects patterns in a series of OHLC data points.
    /// </summary>
    public static List<PatternDetection> DetectPatterns(IDataSeries<IDataPoint> series)
    {
        var patterns = new List<PatternDetection>();

        if (series.Count < 3)
        {
            return patterns;
        }

        // Convert to OHLC points
        var candles = new List<OhlcDataPoint>();
        foreach (var point in series)
        {
            candles.Add(point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y));
        }

        // Detect single-candle patterns
        for (int i = 0; i < candles.Count; i++)
        {
            var pattern = DetectSingleCandlePattern(candles, i);
            if (pattern != CandlestickPattern.None)
            {
                patterns.Add(new PatternDetection
                {
                    Pattern = pattern,
                    Index = i,
                    Label = GetPatternLabel(pattern),
                    IsBullish = IsBullishPattern(pattern)
                });
            }
        }

        // Detect two-candle patterns
        for (int i = 1; i < candles.Count; i++)
        {
            var pattern = DetectTwoCandlePattern(candles, i);
            if (pattern != CandlestickPattern.None)
            {
                patterns.Add(new PatternDetection
                {
                    Pattern = pattern,
                    Index = i,
                    Label = GetPatternLabel(pattern),
                    IsBullish = IsBullishPattern(pattern)
                });
            }
        }

        // Detect three-candle patterns
        for (int i = 2; i < candles.Count; i++)
        {
            var pattern = DetectThreeCandlePattern(candles, i);
            if (pattern != CandlestickPattern.None)
            {
                patterns.Add(new PatternDetection
                {
                    Pattern = pattern,
                    Index = i,
                    Label = GetPatternLabel(pattern),
                    IsBullish = IsBullishPattern(pattern)
                });
            }
        }

        return patterns;
    }

    private static CandlestickPattern DetectSingleCandlePattern(List<OhlcDataPoint> candles, int index)
    {
        var candle = candles[index];
        var body = Math.Abs(candle.Close - candle.Open);
        var range = candle.High - candle.Low;
        var upperShadow = candle.High - Math.Max(candle.Open, candle.Close);
        var lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;

        // Doji: Very small body relative to range
        if (body < range * 0.1)
        {
            return CandlestickPattern.Doji;
        }

        // Hammer: Small body at top, long lower shadow
        if (body < range * 0.3 && lowerShadow > body * 2 && upperShadow < body * 0.5 && candle.IsBullish)
        {
            return CandlestickPattern.Hammer;
        }

        // Inverted Hammer: Small body at bottom, long upper shadow
        if (body < range * 0.3 && upperShadow > body * 2 && lowerShadow < body * 0.5 && candle.IsBullish)
        {
            return CandlestickPattern.InvertedHammer;
        }

        // Shooting Star: Small body at bottom, long upper shadow
        if (body < range * 0.3 && upperShadow > body * 2 && lowerShadow < body * 0.5 && !candle.IsBullish)
        {
            return CandlestickPattern.ShootingStar;
        }

        // Hanging Man: Small body at top, long lower shadow
        if (body < range * 0.3 && lowerShadow > body * 2 && upperShadow < body * 0.5 && !candle.IsBullish)
        {
            return CandlestickPattern.HangingMan;
        }

        return CandlestickPattern.None;
    }

    private static CandlestickPattern DetectTwoCandlePattern(List<OhlcDataPoint> candles, int index)
    {
        if (index < 1) return CandlestickPattern.None;

        var prev = candles[index - 1];
        var curr = candles[index];

        var prevBody = Math.Abs(prev.Close - prev.Open);
        var currBody = Math.Abs(curr.Close - curr.Open);

        // Bullish Engulfing: Bearish candle followed by larger bullish candle
        if (!prev.IsBullish && curr.IsBullish &&
            curr.Open <= prev.Close && curr.Close > prev.Open &&
            currBody > prevBody)
        {
            return CandlestickPattern.BullishEngulfing;
        }

        // Bearish Engulfing: Bullish candle followed by larger bearish candle
        if (prev.IsBullish && !curr.IsBullish &&
            curr.Open >= prev.Close && curr.Close < prev.Open &&
            currBody > prevBody)
        {
            return CandlestickPattern.BearishEngulfing;
        }

        return CandlestickPattern.None;
    }

    private static CandlestickPattern DetectThreeCandlePattern(List<OhlcDataPoint> candles, int index)
    {
        if (index < 2) return CandlestickPattern.None;

        var first = candles[index - 2];
        var second = candles[index - 1];
        var third = candles[index];

        // Morning Star: Bearish candle, small candle (doji), bullish candle
        if (!first.IsBullish && third.IsBullish)
        {
            var secondBody = Math.Abs(second.Close - second.Open);
            var secondRange = second.High - second.Low;
            if (secondBody < secondRange * 0.3 && third.Close > (first.Open + first.Close) / 2)
            {
                return CandlestickPattern.MorningStar;
            }
        }

        // Evening Star: Bullish candle, small candle (doji), bearish candle
        if (first.IsBullish && !third.IsBullish)
        {
            var secondBody = Math.Abs(second.Close - second.Open);
            var secondRange = second.High - second.Low;
            if (secondBody < secondRange * 0.3 && third.Close < (first.Open + first.Close) / 2)
            {
                return CandlestickPattern.EveningStar;
            }
        }

        // Three White Soldiers: Three consecutive bullish candles with higher closes
        if (first.IsBullish && second.IsBullish && third.IsBullish &&
            second.Close > first.Close && third.Close > second.Close)
        {
            return CandlestickPattern.ThreeWhiteSoldiers;
        }

        // Three Black Crows: Three consecutive bearish candles with lower closes
        if (!first.IsBullish && !second.IsBullish && !third.IsBullish &&
            second.Close < first.Close && third.Close < second.Close)
        {
            return CandlestickPattern.ThreeBlackCrows;
        }

        return CandlestickPattern.None;
    }

    private static string GetPatternLabel(CandlestickPattern pattern)
    {
        return pattern switch
        {
            CandlestickPattern.Doji => "Doji",
            CandlestickPattern.Hammer => "Hammer",
            CandlestickPattern.InvertedHammer => "Inv. Hammer",
            CandlestickPattern.ShootingStar => "Shooting Star",
            CandlestickPattern.HangingMan => "Hanging Man",
            CandlestickPattern.BullishEngulfing => "Bull Engulf",
            CandlestickPattern.BearishEngulfing => "Bear Engulf",
            CandlestickPattern.MorningStar => "Morning Star",
            CandlestickPattern.EveningStar => "Evening Star",
            CandlestickPattern.ThreeWhiteSoldiers => "3 White Soldiers",
            CandlestickPattern.ThreeBlackCrows => "3 Black Crows",
            _ => ""
        };
    }

    private static bool IsBullishPattern(CandlestickPattern pattern)
    {
        return pattern switch
        {
            CandlestickPattern.Hammer => true,
            CandlestickPattern.InvertedHammer => true,
            CandlestickPattern.BullishEngulfing => true,
            CandlestickPattern.MorningStar => true,
            CandlestickPattern.ThreeWhiteSoldiers => true,
            CandlestickPattern.ShootingStar => false,
            CandlestickPattern.HangingMan => false,
            CandlestickPattern.BearishEngulfing => false,
            CandlestickPattern.EveningStar => false,
            CandlestickPattern.ThreeBlackCrows => false,
            _ => true // Neutral patterns default to bullish color
        };
    }
}
