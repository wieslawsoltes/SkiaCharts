using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// RSI (Relative Strength Index) indicator.
/// Measures the magnitude of recent price changes to evaluate overbought or oversold conditions.
/// </summary>
public class RsiIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RsiIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for RSI calculation.</param>
    public RsiIndicator(int period = 14)
    {
        Period = period;
        MinValue = 0;
        MaxValue = 100;
    }

    /// <inheritdoc/>
    public override string Name => $"RSI({Period})";

    /// <summary>
    /// Gets or sets the period for RSI calculation.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the overbought level (default: 70).
    /// </summary>
    public double OverboughtLevel { get; set; } = 70;

    /// <summary>
    /// Gets or sets the oversold level (default: 30).
    /// </summary>
    public double OversoldLevel { get; set; } = 30;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < Period + 1)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate price changes
        var gains = new List<double>();
        var losses = new List<double>();

        for (int i = 1; i < series.Count; i++)
        {
            double change = GetClose(series[i]) - GetClose(series[i - 1]);
            gains.Add(change > 0 ? change : 0);
            losses.Add(change < 0 ? -change : 0);
        }

        // Calculate initial average gain and loss (SMA)
        double avgGain = 0;
        double avgLoss = 0;
        for (int i = 0; i < Period; i++)
        {
            avgGain += gains[i];
            avgLoss += losses[i];
        }
        avgGain /= Period;
        avgLoss /= Period;

        // Add NaN values for insufficient data
        for (int i = 0; i < Period; i++)
        {
            result.Add(new DataPoint(series[i].X, double.NaN));
        }

        // Calculate RSI for first valid point
        double rs = avgLoss == 0 ? 100 : avgGain / avgLoss;
        double rsi = 100 - (100 / (1 + rs));
        result.Add(new DataPoint(series[Period].X, rsi));

        // Calculate RSI for remaining points using Wilder's smoothing
        for (int i = Period; i < gains.Count; i++)
        {
            avgGain = (avgGain * (Period - 1) + gains[i]) / Period;
            avgLoss = (avgLoss * (Period - 1) + losses[i]) / Period;

            rs = avgLoss == 0 ? 100 : avgGain / avgLoss;
            rsi = 100 - (100 / (1 + rs));
            result.Add(new DataPoint(series[i + 1].X, rsi));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
