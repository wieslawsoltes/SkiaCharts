using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Heiken-Ashi chart - A modified candlestick chart that uses averaged values
/// to smooth price action and identify trends more easily.
/// </summary>
public class HeikenAshiChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, CandlestickSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="HeikenAshiChart"/> class.
    /// </summary>
    public HeikenAshiChart()
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

        // Add Heiken-Ashi renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new HeikenAshiRenderer(series, this, style));
            }
        }
    }

    private class HeikenAshiRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly HeikenAshiChart _chart;
        private readonly CandlestickSeriesStyle _style;

        public HeikenAshiRenderer(IDataSeries<IDataPoint> series, HeikenAshiChart chart, CandlestickSeriesStyle style)
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
            var candleWidth = CalculateCandleWidth();

            // Convert to Heiken-Ashi candles
            var haCandles = ConvertToHeikenAshi(_series);

            // Render each Heiken-Ashi candle
            for (int i = 0; i < haCandles.Count; i++)
            {
                var candle = haCandles[i];
                RenderCandle(context, i, candle, candleWidth);
            }
        }

        private List<OhlcDataPoint> ConvertToHeikenAshi(IDataSeries<IDataPoint> series)
        {
            var haCandles = new List<OhlcDataPoint>();

            double prevHaOpen = 0;
            double prevHaClose = 0;

            for (int i = 0; i < series.Count; i++)
            {
                var point = series[i];
                var ohlc = point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y);

                // Heiken-Ashi calculations:
                // HA-Close = (Open + High + Low + Close) / 4
                var haClose = (ohlc.Open + ohlc.High + ohlc.Low + ohlc.Close) / 4;

                // HA-Open = (Previous HA-Open + Previous HA-Close) / 2
                var haOpen = i == 0
                    ? (ohlc.Open + ohlc.Close) / 2
                    : (prevHaOpen + prevHaClose) / 2;

                // HA-High = Max(High, HA-Open, HA-Close)
                var haHigh = Math.Max(ohlc.High, Math.Max(haOpen, haClose));

                // HA-Low = Min(Low, HA-Open, HA-Close)
                var haLow = Math.Min(ohlc.Low, Math.Min(haOpen, haClose));

                haCandles.Add(new OhlcDataPoint(
                    ohlc.X,
                    haOpen,
                    haHigh,
                    haLow,
                    haClose,
                    ohlc.Volume
                ));

                prevHaOpen = haOpen;
                prevHaClose = haClose;
            }

            return haCandles;
        }

        private void RenderCandle(IRenderContext context, int index, OhlcDataPoint candle, float candleWidth)
        {
            var centerX = _chart.Viewport.DataToScreen(index, 0).X;
            var openY = _chart.Viewport.DataToScreen(index, candle.Open).Y;
            var highY = _chart.Viewport.DataToScreen(index, candle.High).Y;
            var lowY = _chart.Viewport.DataToScreen(index, candle.Low).Y;
            var closeY = _chart.Viewport.DataToScreen(index, candle.Close).Y;

            var isBullish = candle.IsBullish;
            var color = isBullish ? _style.BullishColor : _style.BearishColor;

            // Draw wick (high-low line)
            using var wickPaint = new SKPaint
            {
                Color = color,
                StrokeWidth = _style.WickWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            context.DrawLine(centerX, highY, centerX, lowY, wickPaint);

            // Draw candle body
            var bodyTop = Math.Min(openY, closeY);
            var bodyBottom = Math.Max(openY, closeY);
            var bodyHeight = bodyBottom - bodyTop;

            if (bodyHeight < 0.5f)
            {
                bodyHeight = 0.5f; // Minimum visible height for doji
            }

            var bodyRect = new SKRect(
                centerX - candleWidth / 2,
                bodyTop,
                centerX + candleWidth / 2,
                bodyTop + bodyHeight
            );

            // Fill body
            using var bodyFillPaint = new SKPaint
            {
                Color = color,
                Style = (isBullish && _style.UseHollowCandles) ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
                StrokeWidth = _style.BodyBorderWidth,
                IsAntialias = true
            };

            context.DrawRect(bodyRect, bodyFillPaint);

            // Draw border for filled candles
            if (!isBullish || !_style.UseHollowCandles)
            {
                using var borderPaint = new SKPaint
                {
                    Color = color,
                    StrokeWidth = _style.BodyBorderWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                context.DrawRect(bodyRect, borderPaint);
            }
        }

        private float CalculateCandleWidth()
        {
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var candleSpacing = _series.Count > 0 ? availableWidth / _series.Count : availableWidth;
            var candleWidth = (float)(candleSpacing * _style.CandleWidthRatio);

            candleWidth = Math.Max(candleWidth, _style.MinimumCandleWidth);
            if (_style.MaximumCandleWidth > 0)
            {
                candleWidth = Math.Min(candleWidth, _style.MaximumCandleWidth);
            }

            return candleWidth;
        }
    }
}
