using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Exponential Moving Average (EMA) indicator.
/// Gives more weight to recent prices using exponential smoothing.
/// </summary>
public class EmaIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmaIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for the moving average.</param>
    public EmaIndicator(int period = 20)
    {
        Period = period;
    }

    /// <inheritdoc/>
    public override string Name => $"EMA({Period})";

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

        // Calculate multiplier: 2 / (period + 1)
        double multiplier = 2.0 / (Period + 1);

        // Calculate initial SMA as starting point for EMA
        double sum = 0;
        for (int i = 0; i < Period; i++)
        {
            if (i < series.Count)
            {
                sum += GetClose(series[i]);
                result.Add(new DataPoint(series[i].X, double.NaN));
            }
        }

        double ema = sum / Period;
        result[Period - 1] = new DataPoint(series[Period - 1].X, ema);

        // Calculate EMA for remaining points
        for (int i = Period; i < series.Count; i++)
        {
            double close = GetClose(series[i]);
            ema = (close - ema) * multiplier + ema;
            result.Add(new DataPoint(series[i].X, ema));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
