using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a Renko brick for the Renko chart.
/// </summary>
public readonly struct RenkoBrick
{
    public RenkoBrick(int index, double open, double close, bool isBullish)
    {
        Index = index;
        Open = open;
        Close = close;
        IsBullish = isBullish;
    }

    public int Index { get; }
    public double Open { get; }
    public double Close { get; }
    public bool IsBullish { get; }
}

/// <summary>
/// Configuration for Renko chart.
/// </summary>
public class RenkoChartConfiguration
{
    /// <summary>
    /// Gets or sets the brick size (price movement required to create a new brick).
    /// If null, will be calculated automatically based on ATR.
    /// </summary>
    public double? BrickSize { get; set; }

    /// <summary>
    /// Gets or sets whether to use ATR (Average True Range) for automatic brick sizing.
    /// </summary>
    public bool UseAtr { get; set; } = false;

    /// <summary>
    /// Gets or sets the ATR period for automatic brick sizing.
    /// </summary>
    public int AtrPeriod { get; set; } = 14;

    /// <summary>
    /// Gets or sets the brick width in pixels.
    /// </summary>
    public float BrickWidth { get; set; } = 20f;

    /// <summary>
    /// Gets or sets the brick spacing in pixels.
    /// </summary>
    public float BrickSpacing { get; set; } = 2f;

    /// <summary>
    /// Gets or sets the bullish brick color.
    /// </summary>
    public SKColor BullishColor { get; set; } = SKColors.Green;

    /// <summary>
    /// Gets or sets the bearish brick color.
    /// </summary>
    public SKColor BearishColor { get; set; } = SKColors.Red;

    /// <summary>
    /// Gets or sets whether bearish bricks should be hollow.
    /// </summary>
    public bool HollowBearishBricks { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show brick borders.
    /// </summary>
    public bool ShowBorders { get; set; } = true;

    /// <summary>
    /// Gets or sets the border color.
    /// </summary>
    public SKColor BorderColor { get; set; } = SKColors.Black;

    /// <summary>
    /// Gets or sets the border width.
    /// </summary>
    public float BorderWidth { get; set; } = 1f;
}

/// <summary>
/// Renko chart - Price chart that uses bricks of fixed size to filter out noise
/// and focus on significant price movements. Time is not considered.
/// </summary>
public class RenkoChart : ChartBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoChart"/> class.
    /// </summary>
    public RenkoChart()
    {
        Configuration = new RenkoChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public RenkoChartConfiguration Configuration { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add Renko renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                queue.Add(new RenkoRenderer(series, this));
            }
        }
    }

    private class RenkoRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly RenkoChart _chart;

        public RenkoRenderer(IDataSeries<IDataPoint> series, RenkoChart chart)
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

            // Determine brick size
            var brickSize = _chart.Configuration.BrickSize ?? CalculateDefaultBrickSize();

            // Convert to Renko bricks
            var bricks = ConvertToRenkoBricks(_series, brickSize);

            // Render each brick
            for (int i = 0; i < bricks.Count; i++)
            {
                RenderBrick(context, bricks[i]);
            }
        }

        private double CalculateDefaultBrickSize()
        {
            if (_chart.Configuration.UseAtr)
            {
                return CalculateAtr(_series, _chart.Configuration.AtrPeriod);
            }

            // Simple default: 1% of price range
            var priceRange = _series.MaxY - _series.MinY;
            return priceRange * 0.01;
        }

        private double CalculateAtr(IDataSeries<IDataPoint> series, int period)
        {
            if (series.Count < period)
            {
                period = series.Count;
            }

            double atr = 0;
            var ohlcSeries = series.Select(p => p as OhlcDataPoint? ?? new OhlcDataPoint(p.X, p.Y, p.Y, p.Y, p.Y)).ToList();

            for (int i = 1; i < period; i++)
            {
                var tr = Math.Max(
                    ohlcSeries[i].High - ohlcSeries[i].Low,
                    Math.Max(
                        Math.Abs(ohlcSeries[i].High - ohlcSeries[i - 1].Close),
                        Math.Abs(ohlcSeries[i].Low - ohlcSeries[i - 1].Close)
                    )
                );
                atr += tr;
            }

            return atr / period;
        }

        private List<RenkoBrick> ConvertToRenkoBricks(IDataSeries<IDataPoint> series, double brickSize)
        {
            var bricks = new List<RenkoBrick>();

            if (series.Count == 0 || brickSize <= 0)
            {
                return bricks;
            }

            // Get first close price
            var firstPoint = series[0];
            var ohlc = firstPoint as OhlcDataPoint? ?? new OhlcDataPoint(firstPoint.X, firstPoint.Y, firstPoint.Y, firstPoint.Y, firstPoint.Y);
            var currentBrickBase = Math.Floor(ohlc.Close / brickSize) * brickSize;

            int brickIndex = 0;
            bool? lastDirection = null;

            foreach (var point in series)
            {
                var ohlcPoint = point as OhlcDataPoint? ?? new OhlcDataPoint(point.X, point.Y, point.Y, point.Y, point.Y);
                var close = ohlcPoint.Close;

                // Check for bullish bricks
                while (close >= currentBrickBase + brickSize)
                {
                    bricks.Add(new RenkoBrick(
                        brickIndex++,
                        currentBrickBase,
                        currentBrickBase + brickSize,
                        true
                    ));
                    currentBrickBase += brickSize;
                    lastDirection = true;
                }

                // Check for bearish bricks
                while (close <= currentBrickBase - brickSize)
                {
                    bricks.Add(new RenkoBrick(
                        brickIndex++,
                        currentBrickBase,
                        currentBrickBase - brickSize,
                        false
                    ));
                    currentBrickBase -= brickSize;
                    lastDirection = false;
                }
            }

            return bricks;
        }

        private void RenderBrick(IRenderContext context, RenkoBrick brick)
        {
            var config = _chart.Configuration;

            // Calculate brick position
            var xPos = brick.Index * (config.BrickWidth + config.BrickSpacing);
            var topY = _chart.Viewport.DataToScreen(0, Math.Max(brick.Open, brick.Close)).Y;
            var bottomY = _chart.Viewport.DataToScreen(0, Math.Min(brick.Open, brick.Close)).Y;

            var rect = new SKRect(xPos, topY, xPos + config.BrickWidth, bottomY);

            // Determine color
            var color = brick.IsBullish ? config.BullishColor : config.BearishColor;

            // Fill brick
            using var fillPaint = new SKPaint
            {
                Color = color,
                Style = (!brick.IsBullish && config.HollowBearishBricks) ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
                StrokeWidth = config.BorderWidth,
                IsAntialias = true
            };

            context.DrawRect(rect, fillPaint);

            // Draw border if configured
            if (config.ShowBorders && !(! brick.IsBullish && config.HollowBearishBricks))
            {
                using var borderPaint = new SKPaint
                {
                    Color = config.BorderColor,
                    StrokeWidth = config.BorderWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                context.DrawRect(rect, borderPaint);
            }
        }
    }
}
