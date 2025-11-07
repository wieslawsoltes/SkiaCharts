using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single MACD data point with MACD line, signal line, and histogram.
/// </summary>
public class MacdDataPoint : IDataPoint
{
    public MacdDataPoint(double x, double macd, double signal, double histogram)
    {
        X = x;
        Macd = macd;
        Signal = signal;
        Histogram = histogram;
    }

    public double X { get; }
    public double Y => Macd; // Y is the MACD line

    public double Macd { get; }
    public double Signal { get; }
    public double Histogram { get; }
}

/// <summary>
/// MACD (Moving Average Convergence Divergence) indicator.
/// Shows the relationship between two moving averages of prices.
/// </summary>
public class MacdIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MacdIndicator"/> class.
    /// </summary>
    /// <param name="fastPeriod">The fast EMA period.</param>
    /// <param name="slowPeriod">The slow EMA period.</param>
    /// <param name="signalPeriod">The signal line EMA period.</param>
    public MacdIndicator(int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
    {
        FastPeriod = fastPeriod;
        SlowPeriod = slowPeriod;
        SignalPeriod = signalPeriod;
        MinValue = double.NaN; // Will be calculated dynamically
        MaxValue = double.NaN;
    }

    /// <inheritdoc/>
    public override string Name => $"MACD({FastPeriod},{SlowPeriod},{SignalPeriod})";

    /// <summary>
    /// Gets or sets the fast EMA period.
    /// </summary>
    public int FastPeriod { get; set; }

    /// <summary>
    /// Gets or sets the slow EMA period.
    /// </summary>
    public int SlowPeriod { get; set; }

    /// <summary>
    /// Gets or sets the signal line EMA period.
    /// </summary>
    public int SignalPeriod { get; set; }

    /// <summary>
    /// Gets or sets the MACD line color.
    /// </summary>
    public SKColor MacdLineColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the signal line color.
    /// </summary>
    public SKColor SignalLineColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets the histogram color for positive values.
    /// </summary>
    public SKColor HistogramPositiveColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the histogram color for negative values.
    /// </summary>
    public SKColor HistogramNegativeColor { get; set; } = SKColors.Red;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < SlowPeriod)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate fast EMA
        var fastEma = CalculateEma(series, FastPeriod);

        // Calculate slow EMA
        var slowEma = CalculateEma(series, SlowPeriod);

        // Calculate MACD line (fast EMA - slow EMA)
        var macdLine = new List<double>();
        for (int i = 0; i < series.Count; i++)
        {
            if (i < SlowPeriod - 1)
            {
                macdLine.Add(double.NaN);
            }
            else
            {
                macdLine.Add(fastEma[i] - slowEma[i]);
            }
        }

        // Calculate signal line (EMA of MACD line)
        var signalLine = CalculateEmaFromValues(macdLine, SignalPeriod);

        // Calculate histogram (MACD - Signal)
        double minVal = double.MaxValue;
        double maxVal = double.MinValue;

        for (int i = 0; i < series.Count; i++)
        {
            double macd = macdLine[i];
            double signal = signalLine[i];
            double histogram = double.IsNaN(macd) || double.IsNaN(signal) ? double.NaN : macd - signal;

            result.Add(new MacdDataPoint(series[i].X, macd, signal, histogram));

            // Track min/max for panel bounds
            if (!double.IsNaN(histogram))
            {
                minVal = Math.Min(minVal, histogram);
                maxVal = Math.Max(maxVal, histogram);
            }
        }

        MinValue = minVal == double.MaxValue ? 0 : minVal;
        MaxValue = maxVal == double.MinValue ? 0 : maxVal;

        return new DataSeries<IDataPoint>(result);
    }

    private List<double> CalculateEma(IDataSeries<IDataPoint> series, int period)
    {
        var result = new List<double>();
        double multiplier = 2.0 / (period + 1);

        // Calculate initial SMA
        double sum = 0;
        for (int i = 0; i < period && i < series.Count; i++)
        {
            sum += GetClose(series[i]);
            result.Add(double.NaN);
        }

        if (series.Count < period)
        {
            return result;
        }

        double ema = sum / period;
        result[period - 1] = ema;

        // Calculate EMA for remaining points
        for (int i = period; i < series.Count; i++)
        {
            double close = GetClose(series[i]);
            ema = (close - ema) * multiplier + ema;
            result.Add(ema);
        }

        return result;
    }

    private List<double> CalculateEmaFromValues(List<double> values, int period)
    {
        var result = new List<double>();
        double multiplier = 2.0 / (period + 1);

        // Find first valid value index
        int firstValidIndex = -1;
        for (int i = 0; i < values.Count; i++)
        {
            if (!double.IsNaN(values[i]))
            {
                firstValidIndex = i;
                break;
            }
        }

        if (firstValidIndex == -1 || firstValidIndex + period > values.Count)
        {
            // Not enough data
            for (int i = 0; i < values.Count; i++)
            {
                result.Add(double.NaN);
            }
            return result;
        }

        // Add NaN values before we have enough data
        for (int i = 0; i < firstValidIndex + period - 1; i++)
        {
            result.Add(double.NaN);
        }

        // Calculate initial SMA
        double sum = 0;
        for (int i = 0; i < period; i++)
        {
            sum += values[firstValidIndex + i];
        }
        double ema = sum / period;
        result.Add(ema);

        // Calculate EMA for remaining points
        for (int i = firstValidIndex + period; i < values.Count; i++)
        {
            if (!double.IsNaN(values[i]))
            {
                ema = (values[i] - ema) * multiplier + ema;
                result.Add(ema);
            }
            else
            {
                result.Add(double.NaN);
            }
        }

        return result;
    }
}
