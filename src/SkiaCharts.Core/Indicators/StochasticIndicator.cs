using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single Stochastic data point with %K and %D lines.
/// </summary>
public class StochasticDataPoint : IDataPoint
{
    public StochasticDataPoint(double x, double k, double d)
    {
        X = x;
        K = k;
        D = d;
    }

    public double X { get; }
    public double Y => K; // Y is the %K line

    public double K { get; } // Fast line
    public double D { get; } // Slow line (signal)
}

/// <summary>
/// Stochastic Oscillator indicator.
/// Compares a closing price to its price range over a given time period.
/// </summary>
public class StochasticIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StochasticIndicator"/> class.
    /// </summary>
    /// <param name="kPeriod">The %K period.</param>
    /// <param name="dPeriod">The %D period (SMA of %K).</param>
    /// <param name="smooth">The smoothing period for %K.</param>
    public StochasticIndicator(int kPeriod = 14, int dPeriod = 3, int smooth = 3)
    {
        KPeriod = kPeriod;
        DPeriod = dPeriod;
        Smooth = smooth;
        MinValue = 0;
        MaxValue = 100;
    }

    /// <inheritdoc/>
    public override string Name => $"Stoch({KPeriod},{DPeriod},{Smooth})";

    /// <summary>
    /// Gets or sets the %K period.
    /// </summary>
    public int KPeriod { get; set; }

    /// <summary>
    /// Gets or sets the %D period.
    /// </summary>
    public int DPeriod { get; set; }

    /// <summary>
    /// Gets or sets the smoothing period.
    /// </summary>
    public int Smooth { get; set; }

    /// <summary>
    /// Gets or sets the overbought level (default: 80).
    /// </summary>
    public double OverboughtLevel { get; set; } = 80;

    /// <summary>
    /// Gets or sets the oversold level (default: 20).
    /// </summary>
    public double OversoldLevel { get; set; } = 20;

    /// <summary>
    /// Gets or sets the %K line color.
    /// </summary>
    public SKColor KLineColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the %D line color.
    /// </summary>
    public SKColor DLineColor { get; set; } = SKColors.Red;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < KPeriod)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate raw %K values
        var rawK = new List<double>();
        for (int i = 0; i < series.Count; i++)
        {
            if (i < KPeriod - 1)
            {
                rawK.Add(double.NaN);
                continue;
            }

            // Find highest high and lowest low over K period
            double highest = double.MinValue;
            double lowest = double.MaxValue;
            for (int j = 0; j < KPeriod; j++)
            {
                highest = Math.Max(highest, GetHigh(series[i - j]));
                lowest = Math.Min(lowest, GetLow(series[i - j]));
            }

            double close = GetClose(series[i]);
            double k = highest == lowest ? 50 : ((close - lowest) / (highest - lowest)) * 100;
            rawK.Add(k);
        }

        // Smooth %K if smoothing period > 1
        var smoothedK = new List<double>();
        if (Smooth > 1)
        {
            for (int i = 0; i < rawK.Count; i++)
            {
                if (double.IsNaN(rawK[i]) || i < Smooth - 1)
                {
                    smoothedK.Add(double.NaN);
                    continue;
                }

                double sum = 0;
                int count = 0;
                for (int j = 0; j < Smooth; j++)
                {
                    if (!double.IsNaN(rawK[i - j]))
                    {
                        sum += rawK[i - j];
                        count++;
                    }
                }
                smoothedK.Add(count > 0 ? sum / count : double.NaN);
            }
        }
        else
        {
            smoothedK = rawK;
        }

        // Calculate %D (SMA of smoothed %K)
        var dLine = new List<double>();
        for (int i = 0; i < smoothedK.Count; i++)
        {
            if (double.IsNaN(smoothedK[i]) || i < DPeriod - 1)
            {
                dLine.Add(double.NaN);
                continue;
            }

            double sum = 0;
            int count = 0;
            for (int j = 0; j < DPeriod; j++)
            {
                if (!double.IsNaN(smoothedK[i - j]))
                {
                    sum += smoothedK[i - j];
                    count++;
                }
            }
            dLine.Add(count > 0 ? sum / count : double.NaN);
        }

        // Build result
        for (int i = 0; i < series.Count; i++)
        {
            result.Add(new StochasticDataPoint(series[i].X, smoothedK[i], dLine[i]));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
