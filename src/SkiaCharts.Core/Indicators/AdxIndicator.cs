using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single ADX data point with ADX, +DI, and -DI lines.
/// </summary>
public class AdxDataPoint : IDataPoint
{
    public AdxDataPoint(double x, double adx, double plusDi, double minusDi)
    {
        X = x;
        Adx = adx;
        PlusDi = plusDi;
        MinusDi = minusDi;
    }

    public double X { get; }
    public double Y => Adx; // Y is the ADX line

    public double Adx { get; }
    public double PlusDi { get; }
    public double MinusDi { get; }
}

/// <summary>
/// ADX (Average Directional Index) indicator.
/// Measures the strength of a trend, regardless of direction.
/// </summary>
public class AdxIndicator : PanelIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdxIndicator"/> class.
    /// </summary>
    /// <param name="period">The period for ADX calculation.</param>
    public AdxIndicator(int period = 14)
    {
        Period = period;
        MinValue = 0;
        MaxValue = 100;
    }

    /// <inheritdoc/>
    public override string Name => $"ADX({Period})";

    /// <summary>
    /// Gets or sets the period for ADX calculation.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the ADX line color.
    /// </summary>
    public SKColor AdxLineColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the +DI line color.
    /// </summary>
    public SKColor PlusDiColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the -DI line color.
    /// </summary>
    public SKColor MinusDiColor { get; set; } = SKColors.Red;

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < Period * 2)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Calculate True Range and Directional Movement
        var tr = new List<double>();
        var plusDM = new List<double>();
        var minusDM = new List<double>();

        // First point
        result.Add(new AdxDataPoint(series[0].X, double.NaN, double.NaN, double.NaN));

        for (int i = 1; i < series.Count; i++)
        {
            double high = GetHigh(series[i]);
            double low = GetLow(series[i]);
            double prevHigh = GetHigh(series[i - 1]);
            double prevLow = GetLow(series[i - 1]);
            double prevClose = GetClose(series[i - 1]);

            // True Range
            double trValue = Math.Max(
                high - low,
                Math.Max(
                    Math.Abs(high - prevClose),
                    Math.Abs(low - prevClose)
                )
            );
            tr.Add(trValue);

            // Directional Movement
            double highDiff = high - prevHigh;
            double lowDiff = prevLow - low;

            double plusDMValue = (highDiff > lowDiff && highDiff > 0) ? highDiff : 0;
            double minusDMValue = (lowDiff > highDiff && lowDiff > 0) ? lowDiff : 0;

            plusDM.Add(plusDMValue);
            minusDM.Add(minusDMValue);
        }

        // Calculate smoothed TR, +DM, -DM
        if (tr.Count < Period)
        {
            return new DataSeries<IDataPoint>(result);
        }

        // Initial smoothed values (sum of first Period values)
        double smoothedTR = 0;
        double smoothedPlusDM = 0;
        double smoothedMinusDM = 0;

        for (int i = 0; i < Period; i++)
        {
            smoothedTR += tr[i];
            smoothedPlusDM += plusDM[i];
            smoothedMinusDM += minusDM[i];
        }

        // Add NaN values for insufficient data
        for (int i = 1; i < Period; i++)
        {
            result.Add(new AdxDataPoint(series[i].X, double.NaN, double.NaN, double.NaN));
        }

        // Calculate +DI and -DI
        var plusDI = new List<double>();
        var minusDI = new List<double>();

        double plusDIValue = smoothedTR == 0 ? 0 : (smoothedPlusDM / smoothedTR) * 100;
        double minusDIValue = smoothedTR == 0 ? 0 : (smoothedMinusDM / smoothedTR) * 100;
        plusDI.Add(plusDIValue);
        minusDI.Add(minusDIValue);

        result.Add(new AdxDataPoint(series[Period].X, double.NaN, plusDIValue, minusDIValue));

        for (int i = Period; i < tr.Count; i++)
        {
            // Wilder's smoothing
            smoothedTR = smoothedTR - (smoothedTR / Period) + tr[i];
            smoothedPlusDM = smoothedPlusDM - (smoothedPlusDM / Period) + plusDM[i];
            smoothedMinusDM = smoothedMinusDM - (smoothedMinusDM / Period) + minusDM[i];

            plusDIValue = smoothedTR == 0 ? 0 : (smoothedPlusDM / smoothedTR) * 100;
            minusDIValue = smoothedTR == 0 ? 0 : (smoothedMinusDM / smoothedTR) * 100;

            plusDI.Add(plusDIValue);
            minusDI.Add(minusDIValue);
        }

        // Calculate DX (Directional Index)
        var dx = new List<double>();
        for (int i = 0; i < plusDI.Count; i++)
        {
            double sum = plusDI[i] + minusDI[i];
            double dxValue = sum == 0 ? 0 : (Math.Abs(plusDI[i] - minusDI[i]) / sum) * 100;
            dx.Add(dxValue);
        }

        // Calculate ADX (average of DX)
        if (dx.Count < Period)
        {
            // Not enough data for ADX
            for (int i = result.Count; i < series.Count; i++)
            {
                int diIndex = i - Period;
                if (diIndex >= 0 && diIndex < plusDI.Count)
                {
                    result.Add(new AdxDataPoint(series[i].X, double.NaN, plusDI[diIndex], minusDI[diIndex]));
                }
            }
            return new DataSeries<IDataPoint>(result);
        }

        // Initial ADX (simple average)
        double adx = 0;
        for (int i = 0; i < Period; i++)
        {
            adx += dx[i];
        }
        adx /= Period;

        // Update the result at index Period with ADX
        result[Period] = new AdxDataPoint(series[Period].X, adx, plusDI[0], minusDI[0]);

        // Calculate smoothed ADX for remaining points
        for (int i = Period; i < dx.Count; i++)
        {
            adx = ((adx * (Period - 1)) + dx[i]) / Period;
            result.Add(new AdxDataPoint(series[i + 1].X, adx, plusDI[i], minusDI[i]));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
