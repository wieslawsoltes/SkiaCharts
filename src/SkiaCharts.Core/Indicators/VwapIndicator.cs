using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// VWAP (Volume Weighted Average Price) indicator.
/// Calculates the average price weighted by volume, typically reset daily.
/// </summary>
public class VwapIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VwapIndicator"/> class.
    /// </summary>
    public VwapIndicator()
    {
    }

    /// <inheritdoc/>
    public override string Name => "VWAP";

    /// <summary>
    /// Gets or sets whether to use a rolling window instead of cumulative.
    /// </summary>
    public bool UseRollingWindow { get; set; } = false;

    /// <summary>
    /// Gets or sets the rolling window period (only used if UseRollingWindow is true).
    /// </summary>
    public int RollingPeriod { get; set; } = 20;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < 1)
        {
            return new DataSeries<IDataPoint>(result);
        }

        if (UseRollingWindow)
        {
            // Rolling VWAP
            for (int i = 0; i < series.Count; i++)
            {
                if (i < RollingPeriod - 1)
                {
                    result.Add(new DataPoint(series[i].X, double.NaN));
                    continue;
                }

                double cumulativePV = 0;
                double cumulativeVolume = 0;

                for (int j = 0; j < RollingPeriod; j++)
                {
                    var point = series[i - j];
                    double typicalPrice = (GetHigh(point) + GetLow(point) + GetClose(point)) / 3;
                    double volume = GetVolume(point);

                    cumulativePV += typicalPrice * volume;
                    cumulativeVolume += volume;
                }

                double vwap = cumulativeVolume > 0 ? cumulativePV / cumulativeVolume : double.NaN;
                result.Add(new DataPoint(series[i].X, vwap));
            }
        }
        else
        {
            // Cumulative VWAP
            double cumulativePV = 0;
            double cumulativeVolume = 0;

            for (int i = 0; i < series.Count; i++)
            {
                var point = series[i];
                double typicalPrice = (GetHigh(point) + GetLow(point) + GetClose(point)) / 3;
                double volume = GetVolume(point);

                cumulativePV += typicalPrice * volume;
                cumulativeVolume += volume;

                double vwap = cumulativeVolume > 0 ? cumulativePV / cumulativeVolume : double.NaN;
                result.Add(new DataPoint(point.X, vwap));
            }
        }

        return new DataSeries<IDataPoint>(result);
    }
}
