using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a Kagi line segment.
/// </summary>
public readonly struct KagiSegment
{
    public KagiSegment(int index, double startPrice, double endPrice, bool isThick)
    {
        Index = index;
        StartPrice = startPrice;
        EndPrice = endPrice;
        IsThick = isThick;
    }

    public int Index { get; }
    public double StartPrice { get; }
    public double EndPrice { get; }
    public bool IsThick { get; }
    public bool IsUp => EndPrice > StartPrice;
}

/// <summary>
/// Configuration for Kagi chart.
/// </summary>
public class KagiChartConfiguration
{
    /// <summary>
    /// Gets or sets the reversal amount (price change required to reverse direction).
    /// If null, will be calculated as a percentage of price.
    /// </summary>
    public double? ReversalAmount { get; set; }

    /// <summary>
    /// Gets or sets the reversal percentage (used if ReversalAmount is null).
    /// </summary>
    public double ReversalPercentage { get; set; } = 4.0; // 4%

    /// <summary>
    /// Gets or sets the horizontal spacing between direction changes.
    /// </summary>
    public float HorizontalSpacing { get; set; } = 30f;

    /// <summary>
    /// Gets or sets the thick line width (used when price exceeds previous high/low).
    /// </summary>
    public float ThickLineWidth { get; set; } = 3f;

    /// <summary>
    /// Gets or sets the thin line width (used during normal trends).
    /// </summary>
    public float ThinLineWidth { get; set; } = 1.5f;

    /// <summary>
    /// Gets or sets the color for upward (yang/thick) lines.
    /// </summary>
    public SKColor UpColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the color for downward (yin/thin) lines.
    /// </summary>
    public SKColor DownColor { get; set; } = SKColors.Red;
}

/// <summary>
/// Kagi chart - Japanese charting technique that uses thick and thin lines
/// to show supply and demand. Changes direction only on significant reversals.
/// </summary>
public class KagiChart : ChartBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KagiChart"/> class.
    /// </summary>
    public KagiChart()
    {
        Configuration = new KagiChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public KagiChartConfiguration Configuration { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add Kagi renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                queue.Add(new KagiRenderer(series, this));
            }
        }
    }

    private class KagiRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly KagiChart _chart;

        public KagiRenderer(IDataSeries<IDataPoint> series, KagiChart chart)
        {
            _series = series;
            _chart = chart;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0)
            {
                return;
            }

            // Calculate reversal amount
            var reversalAmount = _chart.Configuration.ReversalAmount
                ?? ((_series.MaxY - _series.MinY) * _chart.Configuration.ReversalPercentage / 100.0);

            // Convert to Kagi segments
            var segments = ConvertToKagiSegments(_series, reversalAmount);

            // Render each segment
            for (int i = 0; i < segments.Count; i++)
            {
                RenderSegment(context, segments[i], i > 0 ? segments[i - 1] : (KagiSegment?)null);
            }
        }

        private List<KagiSegment> ConvertToKagiSegments(IDataSeries<IDataPoint> series, double reversalAmount)
        {
            var segments = new List<KagiSegment>();

            if (series.Count == 0)
            {
                return segments;
            }

            // Get initial price
            var firstPoint = series[0];
            var ohlc = firstPoint as OhlcDataPoint? ?? new OhlcDataPoint(firstPoint.X, firstPoint.Y, firstPoint.Y, firstPoint.Y, firstPoint.Y);

            var currentPrice = ohlc.Close;
            var segmentStartPrice = currentPrice;
            var currentHigh = currentPrice;
            var currentLow = currentPrice;
            bool isUpTrend = true;
            int segmentIndex = 0;

            for (int i = 1; i < series.Count; i++)
            {
                var point = series[i];
                var ohlcPoint = point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y);
                var close = ohlcPoint.Close;

                if (isUpTrend)
                {
                    // Update high if we have a new high
                    if (close > currentHigh)
                    {
                        currentHigh = close;
                        currentPrice = close;
                    }
                    // Check for reversal
                    else if (currentPrice - close >= reversalAmount)
                    {
                        // Add upward segment
                        bool isThick = currentPrice > currentHigh;
                        segments.Add(new KagiSegment(segmentIndex++, segmentStartPrice, currentPrice, isThick));

                        // Start downward segment
                        isUpTrend = false;
                        segmentStartPrice = currentPrice;
                        currentLow = close;
                        currentPrice = close;
                    }
                }
                else
                {
                    // Update low if we have a new low
                    if (close < currentLow)
                    {
                        currentLow = close;
                        currentPrice = close;
                    }
                    // Check for reversal
                    else if (close - currentPrice >= reversalAmount)
                    {
                        // Add downward segment
                        bool isThick = currentPrice < currentLow;
                        segments.Add(new KagiSegment(segmentIndex++, segmentStartPrice, currentPrice, isThick));

                        // Start upward segment
                        isUpTrend = true;
                        segmentStartPrice = currentPrice;
                        currentHigh = close;
                        currentPrice = close;
                    }
                }
            }

            // Add final segment
            if (currentPrice != segmentStartPrice)
            {
                bool isThick = isUpTrend ? (currentPrice > currentHigh) : (currentPrice < currentLow);
                segments.Add(new KagiSegment(segmentIndex, segmentStartPrice, currentPrice, isThick));
            }

            return segments;
        }

        private void RenderSegment(IRenderContext context, KagiSegment segment, KagiSegment? prevSegment)
        {
            var config = _chart.Configuration;

            // Calculate x position
            var xPos = segment.Index * config.HorizontalSpacing;

            // Calculate y positions
            var startY = _chart.Viewport.DataToScreen(0, segment.StartPrice).Y;
            var endY = _chart.Viewport.DataToScreen(0, segment.EndPrice).Y;

            // Determine color and width
            var color = segment.IsUp ? config.UpColor : config.DownColor;
            var lineWidth = segment.IsThick ? config.ThickLineWidth : config.ThinLineWidth;

            // Draw vertical line
            using var linePaint = new SKPaint
            {
                Color = color,
                StrokeWidth = lineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            context.DrawLine(xPos, startY, xPos, endY, linePaint);

            // Draw horizontal connector to previous segment if exists
            if (prevSegment.HasValue)
            {
                var prevXPos = prevSegment.Value.Index * config.HorizontalSpacing;
                var prevEndY = _chart.Viewport.DataToScreen(0, prevSegment.Value.EndPrice).Y;

                context.DrawLine(prevXPos, prevEndY, xPos, startY, linePaint);
            }
        }
    }
}
