using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Volume chart for displaying trading volume as bars.
/// Typically used alongside candlestick charts for financial data visualization.
/// </summary>
public class VolumeChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, VolumeSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeChart"/> class.
    /// </summary>
    public VolumeChart()
    {
        DefaultStyle = new VolumeSeriesStyle();
        Configuration = new VolumeChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public VolumeSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public VolumeChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, VolumeSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public VolumeSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add volume renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new VolumeRenderer(series, this, style));
            }
        }
    }

    private class VolumeRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly VolumeChart _chart;
        private readonly VolumeSeriesStyle _style;

        public VolumeRenderer(IDataSeries<IDataPoint> series, VolumeChart chart, VolumeSeriesStyle style)
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

            // Calculate bar width
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var barSpacing = _series.Count > 0 ? availableWidth / _series.Count : availableWidth;
            var barWidth = (float)(barSpacing * _style.BarWidthRatio);

            // Apply min/max constraints
            barWidth = Math.Max(barWidth, _style.MinimumBarWidth);
            if (_style.MaximumBarWidth > 0)
            {
                barWidth = Math.Min(barWidth, _style.MaximumBarWidth);
            }

            // Render each volume bar
            int index = 0;
            IDataPoint? prevPoint = null;

            foreach (var point in _series)
            {
                var volume = GetVolume(point);
                if (volume > 0)
                {
                    var color = DetermineBarColor(point, prevPoint);
                    RenderVolumeBar(context, point, volume, barWidth, color);
                }

                prevPoint = point;
                index++;
            }
        }

        private double GetVolume(IDataPoint point)
        {
            if (point is OhlcDataPoint ohlc)
            {
                return ohlc.Volume;
            }
            // For regular data points, use Y value as volume
            return point.Y;
        }

        private SKColor DetermineBarColor(IDataPoint point, IDataPoint? prevPoint)
        {
            var color = _style.DefaultColor;

            switch (_style.ColorMode)
            {
                case VolumeColorMode.Single:
                    color = _style.DefaultColor;
                    break;

                case VolumeColorMode.PriceDirection:
                    if (point is OhlcDataPoint ohlc)
                    {
                        color = ohlc.IsBullish ? _style.BullishColor : _style.BearishColor;
                    }
                    else
                    {
                        // For non-OHLC data, use default
                        color = _style.DefaultColor;
                    }
                    break;

                case VolumeColorMode.VolumeDirection:
                    if (prevPoint != null)
                    {
                        var currentVolume = GetVolume(point);
                        var previousVolume = GetVolume(prevPoint);
                        color = currentVolume >= previousVolume ? _style.IncreasingColor : _style.DecreasingColor;
                    }
                    else
                    {
                        color = _style.IncreasingColor;
                    }
                    break;
            }

            // Apply opacity
            return color.WithAlpha(_style.Opacity);
        }

        private void RenderVolumeBar(IRenderContext context, IDataPoint point, double volume, float barWidth, SKColor color)
        {
            // Convert to screen coordinates
            var x = _chart.Viewport.DataToScreen(point.X, 0).X;
            var volumeTop = _chart.Viewport.DataToScreen(0, volume).Y;
            var baseline = _chart.Viewport.DataToScreen(0, _chart.Configuration.Baseline).Y;

            var top = Math.Min(volumeTop, baseline);
            var bottom = Math.Max(volumeTop, baseline);

            var barRect = new SKRect(
                x - barWidth / 2f,
                top,
                x + barWidth / 2f,
                bottom
            );

            // Draw filled bar
            using var fillPaint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            context.DrawRect(barRect, fillPaint);

            // Draw border if specified
            if (_style.BorderColor.HasValue)
            {
                using var borderPaint = new SKPaint
                {
                    Color = _style.BorderColor.Value,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = _style.BorderWidth,
                    IsAntialias = true
                };
                context.DrawRect(barRect, borderPaint);
            }
        }
    }
}
