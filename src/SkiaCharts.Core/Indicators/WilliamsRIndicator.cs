using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Williams %R indicator.
/// Measures overbought and oversold levels similar to Stochastic Oscillator.
/// </summary>
public class WilliamsRIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRIndicator"/> class.
    /// </summary>
    /// <param name="period">The lookback period.</param>
    public WilliamsRIndicator(int period = 14)
    {
        Period = period;
        MinValue = -100;
        MaxValue = 0;
    }

    /// <inheritdoc/>
    public override string Name => $"Williams %R({Period})";

    /// <summary>
    /// Gets or sets the lookback period.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the overbought level (default: -20).
    /// </summary>
    public double OverboughtLevel { get; set; } = -20;

    /// <summary>
    /// Gets or sets the oversold level (default: -80).
    /// </summary>
    public double OversoldLevel { get; set; } = -80;

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
                result.Add(new DataPoint(series[i].X, double.NaN));
                continue;
            }

            // Find highest high and lowest low over period
            double highest = double.MinValue;
            double lowest = double.MaxValue;

            for (int j = 0; j < Period; j++)
            {
                highest = Math.Max(highest, GetHigh(series[i - j]));
                lowest = Math.Min(lowest, GetLow(series[i - j]));
            }

            double close = GetClose(series[i]);

            // Williams %R = (Highest High - Close) / (Highest High - Lowest Low) * -100
            double williamsR;
            if (highest == lowest)
            {
                williamsR = -50; // Neutral value when range is zero
            }
            else
            {
                williamsR = ((highest - close) / (highest - lowest)) * -100;
            }

            result.Add(new DataPoint(series[i].X, williamsR));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
