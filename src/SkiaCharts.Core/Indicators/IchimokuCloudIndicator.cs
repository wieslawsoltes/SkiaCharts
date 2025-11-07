using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Represents a single Ichimoku Cloud data point with all five lines.
/// </summary>
public class IchimokuDataPoint : IDataPoint
{
    public IchimokuDataPoint(double x, double tenkan, double kijun, double senkouA, double senkouB, double chikou)
    {
        X = x;
        Tenkan = tenkan;
        Kijun = kijun;
        SenkouA = senkouA;
        SenkouB = senkouB;
        Chikou = chikou;
    }

    public double X { get; }
    public double Y => Tenkan; // Y is the Tenkan line

    public double Tenkan { get; } // Conversion Line
    public double Kijun { get; }  // Base Line
    public double SenkouA { get; } // Leading Span A
    public double SenkouB { get; } // Leading Span B
    public double Chikou { get; }  // Lagging Span
}

/// <summary>
/// Ichimoku Cloud indicator.
/// A comprehensive indicator that defines support/resistance, trend direction, and momentum.
/// </summary>
public class IchimokuCloudIndicator : OverlayIndicatorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IchimokuCloudIndicator"/> class.
    /// </summary>
    /// <param name="tenkanPeriod">The Tenkan-sen (Conversion Line) period.</param>
    /// <param name="kijunPeriod">The Kijun-sen (Base Line) period.</param>
    /// <param name="senkouBPeriod">The Senkou Span B period.</param>
    /// <param name="displacement">The displacement for Senkou spans.</param>
    public IchimokuCloudIndicator(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouBPeriod = 52, int displacement = 26)
    {
        TenkanPeriod = tenkanPeriod;
        KijunPeriod = kijunPeriod;
        SenkouBPeriod = senkouBPeriod;
        Displacement = displacement;
    }

    /// <inheritdoc/>
    public override string Name => $"Ichimoku({TenkanPeriod},{KijunPeriod},{SenkouBPeriod})";

    /// <summary>
    /// Gets or sets the Tenkan-sen period.
    /// </summary>
    public int TenkanPeriod { get; set; }

    /// <summary>
    /// Gets or sets the Kijun-sen period.
    /// </summary>
    public int KijunPeriod { get; set; }

    /// <summary>
    /// Gets or sets the Senkou Span B period.
    /// </summary>
    public int SenkouBPeriod { get; set; }

    /// <summary>
    /// Gets or sets the displacement for Senkou spans.
    /// </summary>
    public int Displacement { get; set; }

    /// <summary>
    /// Gets or sets the Tenkan line color.
    /// </summary>
    public SKColor TenkanColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets the Kijun line color.
    /// </summary>
    public SKColor KijunColor { get; set; } = SKColors.Blue;

    /// <summary>
    /// Gets or sets the Senkou Span A color.
    /// </summary>
    public SKColor SenkouAColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the Senkou Span B color.
    /// </summary>
    public SKColor SenkouBColor { get; set; } = SKColors.Orange;

    /// <summary>
    /// Gets or sets the Chikou Span color.
    /// </summary>
    public SKColor ChikouColor { get; set; } = SKColors.Purple;

    /// <summary>
    /// Gets or sets whether to fill the cloud (Kumo).
    /// </summary>
    public bool FillCloud { get; set; } = true;

    /// <summary>
    /// Gets or sets the bullish cloud fill color (when Senkou A > Senkou B).
    /// </summary>
    public SKColor BullishCloudColor { get; set; } = new SKColor(0, 255, 0, 50);

    /// <summary>
    /// Gets or sets the bearish cloud fill color (when Senkou A < Senkou B).
    /// </summary>
    public SKColor BearishCloudColor { get; set; } = new SKColor(255, 0, 0, 50);

    /// <inheritdoc/>
    public override IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series)
    {
        var result = new List<IDataPoint>();

        if (series.Count < SenkouBPeriod)
        {
            return new DataSeries<IDataPoint>(result);
        }

        for (int i = 0; i < series.Count; i++)
        {
            // Calculate Tenkan-sen (Conversion Line) = (highest high + lowest low) / 2 over tenkanPeriod
            double tenkan = double.NaN;
            if (i >= TenkanPeriod - 1)
            {
                double high = double.MinValue;
                double low = double.MaxValue;
                for (int j = 0; j < TenkanPeriod; j++)
                {
                    high = Math.Max(high, GetHigh(series[i - j]));
                    low = Math.Min(low, GetLow(series[i - j]));
                }
                tenkan = (high + low) / 2;
            }

            // Calculate Kijun-sen (Base Line) = (highest high + lowest low) / 2 over kijunPeriod
            double kijun = double.NaN;
            if (i >= KijunPeriod - 1)
            {
                double high = double.MinValue;
                double low = double.MaxValue;
                for (int j = 0; j < KijunPeriod; j++)
                {
                    high = Math.Max(high, GetHigh(series[i - j]));
                    low = Math.Min(low, GetLow(series[i - j]));
                }
                kijun = (high + low) / 2;
            }

            // Calculate Senkou Span A = (Tenkan + Kijun) / 2, projected forward by displacement
            double senkouA = double.NaN;
            if (!double.IsNaN(tenkan) && !double.IsNaN(kijun))
            {
                senkouA = (tenkan + kijun) / 2;
            }

            // Calculate Senkou Span B = (highest high + lowest low) / 2 over senkouBPeriod, projected forward
            double senkouB = double.NaN;
            if (i >= SenkouBPeriod - 1)
            {
                double high = double.MinValue;
                double low = double.MaxValue;
                for (int j = 0; j < SenkouBPeriod; j++)
                {
                    high = Math.Max(high, GetHigh(series[i - j]));
                    low = Math.Min(low, GetLow(series[i - j]));
                }
                senkouB = (high + low) / 2;
            }

            // Calculate Chikou Span = close price, projected backward by displacement
            double chikou = GetClose(series[i]);

            result.Add(new IchimokuDataPoint(series[i].X, tenkan, kijun, senkouA, senkouB, chikou));
        }

        return new DataSeries<IDataPoint>(result);
    }
}
