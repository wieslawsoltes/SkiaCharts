using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single Pivot Points data point with all levels.
/// </summary>
public class PivotPointsDataPoint : IDataPoint
{
    public PivotPointsDataPoint(double x, double pivot, double r1, double r2, double r3, double s1, double s2, double s3)
    {
        X = x;
        Pivot = pivot;
        R1 = r1;
        R2 = r2;
        R3 = r3;
        S1 = s1;
        S2 = s2;
        S3 = s3;
    }

    public double X { get; }
    public double Y => Pivot; // Y is the pivot point

    public double Pivot { get; } // Main pivot point
    public double R1 { get; }    // Resistance 1
    public double R2 { get; }    // Resistance 2
    public double R3 { get; }    // Resistance 3
    public double S1 { get; }    // Support 1
    public double S2 { get; }    // Support 2
    public double S3 { get; }    // Support 3
}

/// <summary>
/// Pivot point calculation method.
/// </summary>
public enum PivotPointMethod
{
    /// <summary>
    /// Standard/Classic pivot points.
    /// </summary>
    Standard,

    /// <summary>
    /// Fibonacci pivot points.
    /// </summary>
    Fibonacci,

    /// <summary>
    /// Woodie pivot points.
    /// </summary>
    Woodie,

    /// <summary>
    /// Camarilla pivot points.
    /// </summary>
    Camarilla,

    /// <summary>
    /// DeMark pivot points.
    /// </summary>
    DeMark
}

/// <summary>
/// Pivot Points indicator.
/// Calculates support and resistance levels based on previous period's high, low, and close.
/// </summary>
public class PivotPointsIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotPointsIndicator"/> class.
    /// </summary>
    /// <param name="method">The calculation method.</param>
    public PivotPointsIndicator(PivotPointMethod method = PivotPointMethod.Standard)
    {
        Method = method;
    }

    /// <inheritdoc/>
    public override string Name => $"Pivot({Method})";

    /// <summary>
    /// Gets or sets the calculation method.
    /// </summary>
    public PivotPointMethod Method { get; set; }

    /// <summary>
    /// Gets or sets the pivot point color.
    /// </summary>
    public SKColor PivotColor { get; set; } = SKColors.Yellow;

    /// <summary>
    /// Gets or sets the resistance levels color.
    /// </summary>
    public SKColor ResistanceColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets the support levels color.
    /// </summary>
    public SKColor SupportColor { get; set; } = SKColors.Green;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < 1)
        {
            return new DataSeries<IDataPoint>(result);
        }

        for (int i = 0; i < series.Count; i++)
        {
            var point = series[i];
            double high = GetHigh(point);
            double low = GetLow(point);
            double close = GetClose(point);

            double pivot, r1, r2, r3, s1, s2, s3;

            switch (Method)
            {
                case PivotPointMethod.Standard:
                    pivot = (high + low + close) / 3;
                    r1 = (2 * pivot) - low;
                    r2 = pivot + (high - low);
                    r3 = high + 2 * (pivot - low);
                    s1 = (2 * pivot) - high;
                    s2 = pivot - (high - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                case PivotPointMethod.Fibonacci:
                    pivot = (high + low + close) / 3;
                    r1 = pivot + 0.382 * (high - low);
                    r2 = pivot + 0.618 * (high - low);
                    r3 = pivot + 1.000 * (high - low);
                    s1 = pivot - 0.382 * (high - low);
                    s2 = pivot - 0.618 * (high - low);
                    s3 = pivot - 1.000 * (high - low);
                    break;

                case PivotPointMethod.Woodie:
                    pivot = (high + low + 2 * close) / 4;
                    r1 = (2 * pivot) - low;
                    r2 = pivot + (high - low);
                    r3 = high + 2 * (pivot - low);
                    s1 = (2 * pivot) - high;
                    s2 = pivot - (high - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                case PivotPointMethod.Camarilla:
                    pivot = (high + low + close) / 3;
                    r1 = close + (high - low) * 1.1 / 12;
                    r2 = close + (high - low) * 1.1 / 6;
                    r3 = close + (high - low) * 1.1 / 4;
                    s1 = close - (high - low) * 1.1 / 12;
                    s2 = close - (high - low) * 1.1 / 6;
                    s3 = close - (high - low) * 1.1 / 4;
                    break;

                case PivotPointMethod.DeMark:
                    double x;
                    if (close < point.Y) // Compare close with open (using Y as approximation)
                        x = high + 2 * low + close;
                    else if (close > point.Y)
                        x = 2 * high + low + close;
                    else
                        x = high + low + 2 * close;

                    pivot = x / 4;
                    r1 = x / 2 - low;
                    s1 = x / 2 - high;
                    r2 = pivot + (high - low);
                    s2 = pivot - (high - low);
                    r3 = high + 2 * (pivot - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                default:
                    pivot = r1 = r2 = r3 = s1 = s2 = s3 = double.NaN;
                    break;
            }

            result.Add(new PivotPointsDataPoint(point.X, pivot, r1, r2, r3, s1, s2, s3));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
