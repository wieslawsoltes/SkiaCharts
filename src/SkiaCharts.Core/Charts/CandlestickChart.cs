using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Candlestick and OHLC chart for financial data visualization.
/// </summary>
public class CandlestickChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, CandlestickSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CandlestickChart"/> class.
    /// </summary>
    public CandlestickChart()
    {
        DefaultStyle = new CandlestickSeriesStyle();
        Configuration = new CandlestickChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public CandlestickSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public CandlestickChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, CandlestickSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public CandlestickSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add candlestick renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new CandlestickRenderer(series, this, style));
            }
        }
    }

    private class CandlestickRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly CandlestickChart _chart;
        private readonly CandlestickSeriesStyle _style;

        public CandlestickRenderer(IDataSeries<IDataPoint> series, CandlestickChart chart, CandlestickSeriesStyle style)
        {
            _series = series;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0)
            {
                return;
            }

            // Calculate candle width
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var candleSpacing = _series.Count > 0 ? availableWidth / _series.Count : availableWidth;
            var candleWidth = (float)(candleSpacing * _style.CandleWidthRatio);

            // Apply min/max constraints
            candleWidth = Math.Max(candleWidth, _style.MinimumCandleWidth);
            if (_style.MaximumCandleWidth > 0)
            {
                candleWidth = Math.Min(candleWidth, _style.MaximumCandleWidth);
            }

            var halfCandleWidth = candleWidth / 2f;

            // Render each candle
            int index = 0;
            foreach (var point in _series)
            {
                if (point is OhlcDataPoint ohlc)
                {
                    RenderCandle(context, ohlc, index, candleWidth, halfCandleWidth);
                }
                index++;
            }
        }

        private void RenderCandle(IRenderContext context, OhlcDataPoint ohlc, int index, float candleWidth, float halfCandleWidth)
        {
            // Determine if bullish or bearish
            var isBullish = ohlc.Close >= ohlc.Open;
            var color = isBullish ? _style.BullishColor : _style.BearishColor;

            // Convert OHLC values to screen coordinates
            var x = _chart.Viewport.DataToScreen(ohlc.X, 0).X;
            var highY = _chart.Viewport.DataToScreen(0, ohlc.High).Y;
            var lowY = _chart.Viewport.DataToScreen(0, ohlc.Low).Y;
            var openY = _chart.Viewport.DataToScreen(0, ohlc.Open).Y;
            var closeY = _chart.Viewport.DataToScreen(0, ohlc.Close).Y;

            if (_style.CandleType == CandleType.Candlestick)
            {
                RenderCandlestick(context, x, highY, lowY, openY, closeY, candleWidth, halfCandleWidth, color, isBullish);
            }
            else
            {
                RenderOhlcBar(context, x, highY, lowY, openY, closeY, candleWidth, halfCandleWidth, color);
            }
        }

        private void RenderCandlestick(IRenderContext context, float x, float highY, float lowY,
            float openY, float closeY, float candleWidth, float halfCandleWidth, SKColor color, bool isBullish)
        {
            // Draw wick (high-low line)
            using (var wickPaint = new SKPaint
            {
                Color = color,
                StrokeWidth = _style.WickWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            })
            {
                context.DrawLine(x, highY, x, lowY, wickPaint);
            }

            // Determine body top and bottom
            var bodyTop = Math.Min(openY, closeY);
            var bodyBottom = Math.Max(openY, closeY);
            var bodyHeight = bodyBottom - bodyTop;

            // Handle doji (open == close)
            if (Math.Abs(bodyHeight) < 0.5f)
            {
                bodyHeight = 1f;
            }

            var bodyRect = new SKRect(
                x - halfCandleWidth,
                bodyTop,
                x + halfCandleWidth,
                bodyBottom
            );

            // Draw body
            if (_style.UseHollowCandles && isBullish)
            {
                // Hollow candle (outline only)
                using var borderPaint = new SKPaint
                {
                    Color = color,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = _style.BodyBorderWidth,
                    IsAntialias = true
                };
                context.DrawRect(bodyRect, borderPaint);
            }
            else
            {
                // Filled candle
                using var fillPaint = new SKPaint
                {
                    Color = color,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                context.DrawRect(bodyRect, fillPaint);
            }
        }

        private void RenderOhlcBar(IRenderContext context, float x, float highY, float lowY,
            float openY, float closeY, float candleWidth, float halfCandleWidth, SKColor color)
        {
            var tickWidth = (float)(halfCandleWidth * _style.OhlcTickRatio);

            using var paint = new SKPaint
            {
                Color = color,
                StrokeWidth = _style.WickWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Draw high-low line
            context.DrawLine(x, highY, x, lowY, paint);

            // Draw open tick (left)
            context.DrawLine(x - tickWidth, openY, x, openY, paint);

            // Draw close tick (right)
            context.DrawLine(x, closeY, x + tickWidth, closeY, paint);
        }
    }
}
