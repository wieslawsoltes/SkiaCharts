using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single Bollinger Bands data point with upper, middle, and lower bands.
/// </summary>
public class BollingerBandsDataPoint : IDataPoint
{
    public BollingerBandsDataPoint(double x, double upper, double middle, double lower)
    {
        X = x;
        Upper = upper;
        Middle = middle;
        Lower = lower;
    }

    public double X { get; }
    public double Y => Middle; // Y is the middle band

    public double Upper { get; }
    public double Middle { get; }
    public double Lower { get; }
}

/// <summary>
/// Bollinger Bands indicator.
/// Shows volatility bands around a moving average (typically SMA).
/// </summary>
public class BollingerBandsIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for the moving average.</param>
    /// <param name="standardDeviations">The number of standard deviations for the bands.</param>
    public BollingerBandsIndicator(int period = 20, double standardDeviations = 2.0)
    {
        Period = period;
        StandardDeviations = standardDeviations;
    }

    /// <inheritdoc/>
    public override string Name => $"BB({Period},{StandardDeviations})";

    /// <summary>
    /// Gets or sets the period for the moving average.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the number of standard deviations for the bands.
    /// </summary>
    public double StandardDeviations { get; set; }

    /// <summary>
    /// Gets or sets the color for the upper band.
    /// </summary>
    public SKColor UpperBandColor { get; set; } = new SKColor(100, 100, 255);

    /// <summary>
    /// Gets or sets the color for the middle band (SMA).
    /// </summary>
    public SKColor MiddleBandColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the color for the lower band.
    /// </summary>
    public SKColor LowerBandColor { get; set; } = new SKColor(100, 100, 255);

    /// <summary>
    /// Gets or sets whether to fill the area between bands.
    /// </summary>
    public bool FillBands { get; set; } = true;

    /// <summary>
    /// Gets or sets the fill color between bands.
    /// </summary>
    public SKColor FillColor { get; set; } = new SKColor(100, 100, 255, 50);

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
                // Not enough data yet
                result.Add(new BollingerBandsDataPoint(series[i].X, double.NaN, double.NaN, double.NaN));
                continue;
            }

            // Calculate SMA
            double sum = 0;
            for (int j = 0; j < Period; j++)
            {
                sum += GetClose(series[i - j]);
            }
            double sma = sum / Period;

            // Calculate standard deviation
            double varianceSum = 0;
            for (int j = 0; j < Period; j++)
            {
                double deviation = GetClose(series[i - j]) - sma;
                varianceSum += deviation * deviation;
            }
            double stdDev = Math.Sqrt(varianceSum / Period);

            // Calculate bands
            double upper = sma + (StandardDeviations * stdDev);
            double lower = sma - (StandardDeviations * stdDev);

            result.Add(new BollingerBandsDataPoint(series[i].X, upper, sma, lower));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
