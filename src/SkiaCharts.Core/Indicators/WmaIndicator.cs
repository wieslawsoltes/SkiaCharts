using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Weighted Moving Average (WMA) indicator.
/// Gives linearly increasing weight to more recent prices.
/// </summary>
public class WmaIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WmaIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for the moving average.</param>
    public WmaIndicator(int period = 20)
    {
        Period = period;
    }

    /// <inheritdoc/>
    public override string Name => $"WMA({Period})";

    /// <summary>
    /// Gets or sets the period for the moving average.
    /// </summary>
    public int Period { get; set; }

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < Period)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate weight sum: 1 + 2 + 3 + ... + period = period * (period + 1) / 2
        double weightSum = Period * (Period + 1) / 2.0;

        for (int i = 0; i < series.Count; i++)
        {
            if (i < Period - 1)
            {
                // Not enough data yet
                result.Add(new DataPoint(series[i].X, double.NaN));
                continue;
            }

            // Calculate WMA for the current window
            double weightedSum = 0;
            for (int j = 0; j < Period; j++)
            {
                double weight = Period - j; // Weight decreases as we go back in time
                weightedSum += GetClose(series[i - j]) * weight;
            }

            double wma = weightedSum / weightSum;
            result.Add(new DataPoint(series[i].X, wma));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
