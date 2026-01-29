using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Parabolic SAR (Stop and Reverse) indicator.
/// Provides potential reversal points in price trends.
/// </summary>
public class ParabolicSarIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParabolicSarIndicator"/> class.
    /// </summary>
    /// <param name="accelerationFactor">The initial acceleration factor.</param>
    /// <param name="maxAcceleration">The maximum acceleration factor.</param>
    public ParabolicSarIndicator(double accelerationFactor = 0.02, double maxAcceleration = 0.2)
    {
        AccelerationFactor = accelerationFactor;
        MaxAcceleration = maxAcceleration;
    }

    /// <inheritdoc/>
    public override string Name => string.Format(System.Globalization.CultureInfo.InvariantCulture,
        "SAR({0},{1})", AccelerationFactor, MaxAcceleration);

    /// <summary>
    /// Gets or sets the initial acceleration factor.
    /// </summary>
    public double AccelerationFactor { get; set; }

    /// <summary>
    /// Gets or sets the maximum acceleration factor.
    /// </summary>
    public double MaxAcceleration { get; set; }

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < 2)
        {
            return new DataSeries<IDataPoint>(result);
        }

        bool isUpTrend = true;
        double sar = GetLow(series[0]);
        double extremePoint = GetHigh(series[0]);
        double af = AccelerationFactor;

        result.Add(new DataPoint(series[0].X, sar));

        for (int i = 1; i < series.Count; i++)
        {
            var current = series[i];
            var currentHigh = GetHigh(current);
            var currentLow = GetLow(current);

            // Calculate new SAR
            sar = sar + af * (extremePoint - sar);

            // Check for reversal
            bool reversal = false;
            if (isUpTrend)
            {
                // In uptrend, SAR should be below price
                if (currentLow < sar)
                {
                    reversal = true;
                    isUpTrend = false;
                    sar = extremePoint;
                    extremePoint = currentLow;
                    af = AccelerationFactor;
                }
                else
                {
                    // Update extreme point if new high
                    if (currentHigh > extremePoint)
                    {
                        extremePoint = currentHigh;
                        af = Math.Min(af + AccelerationFactor, MaxAcceleration);
                    }

                    // SAR can't be above prior two lows in uptrend
                    if (i >= 2)
                    {
                        sar = Math.Min(sar, GetLow(series[i - 1]));
                        sar = Math.Min(sar, GetLow(series[i - 2]));
                    }
                }
            }
            else
            {
                // In downtrend, SAR should be above price
                if (currentHigh > sar)
                {
                    reversal = true;
                    isUpTrend = true;
                    sar = extremePoint;
                    extremePoint = currentHigh;
                    af = AccelerationFactor;
                }
                else
                {
                    // Update extreme point if new low
                    if (currentLow < extremePoint)
                    {
                        extremePoint = currentLow;
                        af = Math.Min(af + AccelerationFactor, MaxAcceleration);
                    }

                    // SAR can't be below prior two highs in downtrend
                    if (i >= 2)
                    {
                        sar = Math.Max(sar, GetHigh(series[i - 1]));
                        sar = Math.Max(sar, GetHigh(series[i - 2]));
                    }
                }
            }

            result.Add(new DataPoint(current.X, sar));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
