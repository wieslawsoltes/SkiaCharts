using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Simple Moving Average (SMA) indicator.
/// Calculates the arithmetic mean of closing prices over a specified period.
/// </summary>
public class SmaIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmaIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for the moving average.</param>
    public SmaIndicator(int period = 20)
    {
        Period = period;
    }

    /// <inheritdoc/>
    public override string Name => $"SMA({Period})";

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

        for (int i = 0; i < series.Count; i++)
        {
            if (i < Period - 1)
            {
                // Not enough data yet - add placeholder with NaN
                result.Add(new DataPoint(series[i].X, double.NaN));
                continue;
            }

            // Calculate SMA for the current window
            double sum = 0;
            for (int j = 0; j < Period; j++)
            {
                sum += GetClose(series[i - j]);
            }

            double sma = sum / Period;
            result.Add(new DataPoint(series[i].X, sma));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
