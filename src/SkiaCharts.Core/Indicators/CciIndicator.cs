using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// CCI (Commodity Channel Index) indicator.
/// Measures the deviation of the price from its statistical mean.
/// </summary>
public class CciIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CciIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for CCI calculation.</param>
    public CciIndicator(int period = 20)
    {
        Period = period;
        MinValue = -200;
        MaxValue = 200;
    }

    /// <inheritdoc/>
    public override string Name => $"CCI({Period})";

    /// <summary>
    /// Gets or sets the period for CCI calculation.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the constant multiplier (default: 0.015).
    /// </summary>
    public double Constant { get; set; } = 0.015;

    /// <summary>
    /// Gets or sets the overbought level (default: 100).
    /// </summary>
    public double OverboughtLevel { get; set; } = 100;

    /// <summary>
    /// Gets or sets the oversold level (default: -100).
    /// </summary>
    public double OversoldLevel { get; set; } = -100;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < Period)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate typical price for each point
        var typicalPrices = new List<double>();
        for (int i = 0; i < series.Count; i++)
        {
            double tp = (GetHigh(series[i]) + GetLow(series[i]) + GetClose(series[i])) / 3;
            typicalPrices.Add(tp);
        }

        // Calculate CCI
        for (int i = 0; i < series.Count; i++)
        {
            if (i < Period - 1)
            {
                result.Add(new DataPoint(series[i].X, double.NaN));
                continue;
            }

            // Calculate SMA of typical price over period
            double sum = 0;
            for (int j = 0; j < Period; j++)
            {
                sum += typicalPrices[i - j];
            }
            double sma = sum / Period;

            // Calculate mean deviation
            double deviationSum = 0;
            for (int j = 0; j < Period; j++)
            {
                deviationSum += Math.Abs(typicalPrices[i - j] - sma);
            }
            double meanDeviation = deviationSum / Period;

            // Calculate CCI
            // CCI = (Typical Price - SMA) / (Constant * Mean Deviation)
            double cci = meanDeviation == 0 ? 0 : (typicalPrices[i] - sma) / (Constant * meanDeviation);

            result.Add(new DataPoint(series[i].X, cci));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
