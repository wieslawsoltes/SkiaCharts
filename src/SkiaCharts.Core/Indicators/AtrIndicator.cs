using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// ATR (Average True Range) indicator.
/// Measures market volatility by decomposing the entire range of an asset price for that period.
/// </summary>
public class AtrIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtrIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for ATR calculation.</param>
    public AtrIndicator(int period = 14)
    {
        Period = period;
        MinValue = 0;
        MaxValue = double.NaN; // Will be calculated dynamically
    }

    /// <inheritdoc/>
    public override string Name => $"ATR({Period})";

    /// <summary>
    /// Gets or sets the period for ATR calculation.
    /// </summary>
    public int Period { get; set; }

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < Period + 1)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate True Range for each period
        var trueRanges = new List<double>();

        // First point has no previous close, so use high - low
        double firstHigh = GetHigh(series[0]);
        double firstLow = GetLow(series[0]);
        trueRanges.Add(firstHigh - firstLow);
        result.Add(new DataPoint(series[0].X, double.NaN));

        for (int i = 1; i < series.Count; i++)
        {
            double high = GetHigh(series[i]);
            double low = GetLow(series[i]);
            double previousClose = GetClose(series[i - 1]);

            // True Range = max(high - low, |high - previousClose|, |low - previousClose|)
            double tr = Math.Max(
                high - low,
                Math.Max(
                    Math.Abs(high - previousClose),
                    Math.Abs(low - previousClose)
                )
            );

            trueRanges.Add(tr);
        }

        // Calculate initial ATR (simple average of first Period true ranges)
        if (trueRanges.Count < Period)
        {
            return new DataSeries<IDataPoint>(result);
        }

        double sum = 0;
        for (int i = 0; i < Period; i++)
        {
            sum += trueRanges[i];
            if (i > 0 && i < Period)
            {
                result.Add(new DataPoint(series[i].X, double.NaN));
            }
        }

        double atr = sum / Period;
        result.Add(new DataPoint(series[Period].X, atr));
        double maxAtr = atr;

        // Calculate ATR for remaining points using Wilder's smoothing
        // ATR = ((Prior ATR * (n-1)) + Current TR) / n
        for (int i = Period; i < trueRanges.Count; i++)
        {
            atr = ((atr * (Period - 1)) + trueRanges[i]) / Period;
            result.Add(new DataPoint(series[i].X, atr));
            maxAtr = Math.Max(maxAtr, atr);
        }

        MaxValue = maxAtr;

        return new DataSeries<IDataPoint>(result);
    }
}
