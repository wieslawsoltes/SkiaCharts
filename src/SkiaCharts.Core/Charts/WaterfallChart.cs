using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Waterfall chart for visualizing cumulative effects of sequential values.
/// Shows how an initial value is affected by a series of positive and negative values.
/// </summary>
public class WaterfallChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, WaterfallSeriesStyle> _seriesStyles = new();
    private readonly Dictionary<IDataSeries<IDataPoint>, List<WaterfallBarConfiguration>> _barConfigurations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WaterfallChart"/> class.
    /// </summary>
    public WaterfallChart()
    {
        DefaultStyle = new WaterfallSeriesStyle();
        Configuration = new WaterfallChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public WaterfallSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public WaterfallChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, WaterfallSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public WaterfallSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <summary>
    /// Sets the bar configurations for a series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="configurations">The bar configurations.</param>
    public void SetBarConfigurations(IDataSeries<IDataPoint> series, List<WaterfallBarConfiguration> configurations)
    {
        _barConfigurations[series] = configurations;
    }

    /// <summary>
    /// Gets the bar configurations for a series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The bar configurations.</returns>
    public List<WaterfallBarConfiguration>? GetBarConfigurations(IDataSeries<IDataPoint> series)
    {
        return _barConfigurations.TryGetValue(series, out var configs) ? configs : null;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        if (Series.Count == 0)
        {
            return;
        }

        // Add series renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                var configs = GetBarConfigurations(series);
                queue.Add(new WaterfallSeriesRenderer(series, this, style, configs));
            }
        }

        // Add category labels renderer if enabled
        if (Configuration.ShowCategoryLabels)
        {
            queue.Add(new CategoryLabelsRenderer(this));
        }
    }

    private class WaterfallSeriesRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly WaterfallChart _chart;
        private readonly WaterfallSeriesStyle _style;
        private readonly List<WaterfallBarConfiguration>? _configs;

        public WaterfallSeriesRenderer(
            IDataSeries<IDataPoint> series,
            WaterfallChart chart,
            WaterfallSeriesStyle style,
            List<WaterfallBarConfiguration>? configs)
        {
            _series = series;
            _chart = chart;
            _style = style;
            _configs = configs;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0)
            {
                return;
            }

            var barWidth = CalculateBarWidth();
            var cumulativeValue = _chart.Configuration.StartFromZero ? 0.0 : _series[0].Y;
            var previousTop = cumulativeValue;

            var connectorPoints = new List<(SKPoint Start, SKPoint End)>();

            // Render bars
            for (int i = 0; i < _series.Count; i++)
            {
                var point = _series[i];
                var config = _configs != null && i < _configs.Count ? _configs[i] : new WaterfallBarConfiguration();

                var barType = DetermineBarType(point, config);
                var barColor = GetBarColor(barType);

                double barStart, barEnd;

                if (config.IsTotal || barType == WaterfallBarType.Total)
                {
                    // Total bars start from zero to cumulative value
                    barStart = 0;
                    barEnd = cumulativeValue;
                }
                else
                {
                    // Regular bars show the change
                    barStart = previousTop;
                    barEnd = previousTop + point.Y;
                    cumulativeValue = barEnd;
                    previousTop = barEnd;
                }

                // Draw bar
                RenderBar(context, i, barStart, barEnd, barWidth, barColor);

                // Store connector line endpoints
                if (_style.ShowConnectorLines && i < _series.Count - 1)
                {
                    var nextPoint = _series[i + 1];
                    var nextConfig = _configs != null && i + 1 < _configs.Count ? _configs[i + 1] : new WaterfallBarConfiguration();

                    // Don't draw connector from this bar if next is a total
                    if (!nextConfig.IsTotal && DetermineBarType(nextPoint, nextConfig) != WaterfallBarType.Total)
                    {
                        var currentRight = _chart.Viewport.DataToScreen(i + barWidth / 2, barEnd);
                        var nextLeft = _chart.Viewport.DataToScreen(i + 1 - barWidth / 2, barEnd);
                        connectorPoints.Add((currentRight, nextLeft));
                    }
                }

                // Draw value label if enabled
                if (_style.ShowValueLabels)
                {
                    RenderValueLabel(context, i, point.Y, barEnd, barWidth);
                }
            }

            // Draw connector lines
            if (_style.ShowConnectorLines)
            {
                RenderConnectorLines(context, connectorPoints);
            }
        }

        private void RenderBar(IRenderContext context, int index, double startValue, double endValue, double barWidth, SKColor color)
        {
            var left = _chart.Viewport.DataToScreen(index - barWidth / 2, 0);
            var right = _chart.Viewport.DataToScreen(index + barWidth / 2, 0);
            var bottom = _chart.Viewport.DataToScreen(index, Math.Min(startValue, endValue));
            var top = _chart.Viewport.DataToScreen(index, Math.Max(startValue, endValue));

            var rect = new SKRect(left.X, top.Y, right.X, bottom.Y);

            using var fillPaint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            if (_style.CornerRadius > 0)
            {
                context.DrawRoundRect(rect, _style.CornerRadius, _style.CornerRadius, fillPaint);
            }
            else
            {
                context.DrawRect(rect, fillPaint);
            }

            // Draw border if configured
            if (_style.BorderColor.HasValue)
            {
                using var borderPaint = new SKPaint
                {
                    Color = _style.BorderColor.Value,
                    StrokeWidth = _style.BorderWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                if (_style.CornerRadius > 0)
                {
                    context.DrawRoundRect(rect, _style.CornerRadius, _style.CornerRadius, borderPaint);
                }
                else
                {
                    context.DrawRect(rect, borderPaint);
                }
            }
        }

        private void RenderConnectorLines(IRenderContext context, List<(SKPoint Start, SKPoint End)> connectorPoints)
        {
            using var linePaint = new SKPaint
            {
                Color = _style.ConnectorLineColor,
                StrokeWidth = _style.ConnectorLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            if (_style.ConnectorDashPattern != null)
            {
                linePaint.PathEffect = SKPathEffect.CreateDash(_style.ConnectorDashPattern, 0);
            }

            foreach (var (start, end) in connectorPoints)
            {
                context.DrawLine(start.X, start.Y, end.X, end.Y, linePaint);
            }
        }

        private void RenderValueLabel(IRenderContext context, int index, double value, double barTop, double barWidth)
        {
            using var textPaint = new SKPaint
            {
                Color = _style.ValueLabelColor,
                TextSize = _style.ValueLabelFontSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            var centerX = _chart.Viewport.DataToScreen(index, 0).X;
            var labelY = _chart.Viewport.DataToScreen(index, barTop).Y - 5;

            var labelText = value.ToString("F2");
            context.DrawText(labelText, centerX, labelY, textPaint);
        }

        private double CalculateBarWidth()
        {
            // Bar width as a ratio of the available space between points
            return _style.BarWidthRatio * 0.8;
        }

        private WaterfallBarType DetermineBarType(IDataPoint point, WaterfallBarConfiguration config)
        {
            if (config.IsTotal)
            {
                return WaterfallBarType.Total;
            }

            if (config.BarType != WaterfallBarType.Automatic)
            {
                return config.BarType;
            }

            return point.Y >= 0 ? WaterfallBarType.Positive : WaterfallBarType.Negative;
        }

        private SKColor GetBarColor(WaterfallBarType barType)
        {
            return barType switch
            {
                WaterfallBarType.Positive => _style.PositiveColor,
                WaterfallBarType.Negative => _style.NegativeColor,
                WaterfallBarType.Total => _style.TotalColor,
                _ => _style.PositiveColor
            };
        }
    }

    private class CategoryLabelsRenderer : ChartElement
    {
        private readonly WaterfallChart _chart;

        public CategoryLabelsRenderer(WaterfallChart chart)
        {
            _chart = chart;
            Layer = RenderLayer.Overlay;
        }

        public override void Render(IRenderContext context)
        {
            if (_chart.Configuration.CategoryLabels == null || _chart.Series.Count == 0)
            {
                return;
            }

            var series = _chart.Series[0];
            var labels = _chart.Configuration.CategoryLabels;

            using var textPaint = new SKPaint
            {
                Color = _chart.Configuration.CategoryLabelColor,
                TextSize = _chart.Configuration.CategoryLabelFontSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            for (int i = 0; i < Math.Min(series.Count, labels.Length); i++)
            {
                var position = _chart.Viewport.DataToScreen(i, 0);
                var labelY = position.Y + 15; // Below the X-axis

                if (_chart.Configuration.CategoryLabelRotation != 0)
                {
                    context.Canvas.Save();
                    context.Canvas.Translate(position.X, labelY);
                    context.Canvas.RotateDegrees(_chart.Configuration.CategoryLabelRotation);
                    context.DrawText(labels[i], 0, 0, textPaint);
                    context.Canvas.Restore();
                }
                else
                {
                    context.DrawText(labels[i], position.X, labelY, textPaint);
                }
            }
        }
    }
}
