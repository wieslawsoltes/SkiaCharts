using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Bar/Column chart with support for grouped, stacked (absolute and percentage),
/// rounded corners, gradient fills, and borders.
/// </summary>
public class BarChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, BarSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BarChart"/> class.
    /// </summary>
    public BarChart()
    {
        DefaultStyle = new BarSeriesStyle();
        Configuration = new BarChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public BarSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public BarChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, BarSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public BarSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add bar renderers
        if (Series.Count > 0)
        {
            queue.Add(new BarRenderer(Series, this));
        }
    }

    private class BarRenderer : ChartElement
    {
        private readonly DataSeriesCollection _allSeries;
        private readonly BarChart _chart;

        public BarRenderer(DataSeriesCollection allSeries, BarChart chart)
        {
            _allSeries = allSeries;
            _chart = chart;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_allSeries.Count == 0)
            {
                return;
            }

            var config = _chart.Configuration;

            switch (config.StackMode)
            {
                case BarStackMode.None:
                    RenderGroupedBars(context);
                    break;
                case BarStackMode.Absolute:
                    RenderStackedBars(context, false);
                    break;
                case BarStackMode.Percentage:
                    RenderStackedBars(context, true);
                    break;
            }
        }

        private void RenderGroupedBars(IRenderContext context)
        {
            var config = _chart.Configuration;
            var seriesCount = _allSeries.Count;
            if (seriesCount == 0) return;

            // Get all unique X values across all series
            var allXValues = new HashSet<double>();
            foreach (var series in _allSeries)
            {
                foreach (var point in series)
                {
                    allXValues.Add(point.X);
                }
            }
            var sortedX = allXValues.OrderBy(x => x).ToList();

            // Calculate bar width
            var categoryWidth = 1.0; // Assuming unit spacing between categories
            var groupWidth = categoryWidth * (1 - config.GroupSpacing);
            var barWidth = groupWidth / seriesCount;

            for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
            {
                var series = _allSeries[seriesIndex];
                var style = _chart.GetSeriesStyle(series);

                foreach (var point in series)
                {
                    var categoryIndex = sortedX.IndexOf(point.X);
                    if (categoryIndex < 0) continue;

                    // Calculate bar position
                    var categoryCenter = point.X;
                    var groupStart = categoryCenter - groupWidth / 2;
                    var barX = groupStart + seriesIndex * barWidth;

                    var barRect = CalculateBarRect(barX, 0, point.Y, barWidth, style);
                    RenderBar(context, barRect, style, point.Y);
                }
            }
        }

        private void RenderStackedBars(IRenderContext context, bool percentage)
        {
            var config = _chart.Configuration;
            var seriesCount = _allSeries.Count;
            if (seriesCount == 0) return;

            // Get all unique X values
            var allXValues = new HashSet<double>();
            foreach (var series in _allSeries)
            {
                foreach (var point in series)
                {
                    allXValues.Add(point.X);
                }
            }
            var sortedX = allXValues.OrderBy(x => x).ToList();

            // Calculate totals for percentage mode
            var totals = new Dictionary<double, double>();
            if (percentage)
            {
                foreach (var x in sortedX)
                {
                    double total = 0;
                    foreach (var series in _allSeries)
                    {
                        var point = series.FirstOrDefault(p => Math.Abs(p.X - x) < 0.0001);
                        if (point != null)
                        {
                            total += Math.Abs(point.Y);
                        }
                    }
                    totals[x] = total > 0 ? total : 1; // Avoid division by zero
                }
            }

            // Calculate bar width
            var categoryWidth = 1.0;
            var firstSeriesStyle = _chart.GetSeriesStyle(_allSeries[0]);
            var barWidth = categoryWidth * firstSeriesStyle.BarWidthRatio;

            // Render stacked bars
            foreach (var x in sortedX)
            {
                double cumulativeY = 0;

                for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
                {
                    var series = _allSeries[seriesIndex];
                    var style = _chart.GetSeriesStyle(series);

                    var point = series.FirstOrDefault(p => Math.Abs(p.X - x) < 0.0001);
                    if (point == null) continue;

                    double value = point.Y;
                    if (percentage && totals[x] > 0)
                    {
                        value = (value / totals[x]) * 100;
                    }

                    var barRect = CalculateBarRect(x - barWidth / 2, cumulativeY, cumulativeY + value, barWidth, style);
                    RenderBar(context, barRect, style, value);

                    cumulativeY += value;
                }
            }
        }

        private SKRect CalculateBarRect(double x, double y1, double y2, double width, BarSeriesStyle style)
        {
            var viewport = _chart.Viewport;
            var config = _chart.Configuration;

            SKPoint p1, p2;

            if (config.Orientation == BarOrientation.Vertical)
            {
                // Vertical bars (column chart)
                p1 = viewport.DataToScreen(x, y1);
                p2 = viewport.DataToScreen(x + width, y2);

                // Ensure minimum bar size
                var height = Math.Abs(p2.Y - p1.Y);
                if (height < style.MinimumBarSize && y2 != y1)
                {
                    if (y2 > y1)
                    {
                        p2.Y = p1.Y + style.MinimumBarSize;
                    }
                    else
                    {
                        p2.Y = p1.Y - style.MinimumBarSize;
                    }
                }

                return new SKRect(p1.X, Math.Min(p1.Y, p2.Y), p2.X, Math.Max(p1.Y, p2.Y));
            }
            else
            {
                // Horizontal bars (bar chart)
                p1 = viewport.DataToScreen(y1, x);
                p2 = viewport.DataToScreen(y2, x + width);

                // Ensure minimum bar size
                var barWidth = Math.Abs(p2.X - p1.X);
                if (barWidth < style.MinimumBarSize && y2 != y1)
                {
                    if (y2 > y1)
                    {
                        p2.X = p1.X + style.MinimumBarSize;
                    }
                    else
                    {
                        p2.X = p1.X - style.MinimumBarSize;
                    }
                }

                return new SKRect(Math.Min(p1.X, p2.X), p1.Y, Math.Max(p1.X, p2.X), p2.Y);
            }
        }

        private void RenderBar(IRenderContext context, SKRect rect, BarSeriesStyle style, double value)
        {
            // Fill
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // Apply gradient or solid fill
            if (style.GradientColors != null && style.GradientColors.Length >= 2)
            {
                var angle = style.GradientAngle * (float)Math.PI / 180f;
                var dx = (float)Math.Cos(angle);
                var dy = (float)Math.Sin(angle);

                var startPoint = new SKPoint(
                    rect.MidX - dx * rect.Width / 2,
                    rect.MidY - dy * rect.Height / 2
                );
                var endPoint = new SKPoint(
                    rect.MidX + dx * rect.Width / 2,
                    rect.MidY + dy * rect.Height / 2
                );

                fillPaint.Shader = SKShader.CreateLinearGradient(
                    startPoint,
                    endPoint,
                    style.GradientColors,
                    null,
                    SKShaderTileMode.Clamp
                );
            }
            else
            {
                fillPaint.Color = style.FillColor;
            }

            // Draw with rounded corners if specified
            if (style.CornerRadius > 0)
            {
                context.Canvas.DrawRoundRect(rect, style.CornerRadius, style.CornerRadius, fillPaint);
            }
            else
            {
                context.DrawRect(rect, fillPaint);
            }

            // Border
            if (style.BorderColor.HasValue && style.BorderWidth > 0)
            {
                using var borderPaint = new SKPaint
                {
                    Color = style.BorderColor.Value,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = style.BorderWidth,
                    IsAntialias = true
                };

                if (style.CornerRadius > 0)
                {
                    context.Canvas.DrawRoundRect(rect, style.CornerRadius, style.CornerRadius, borderPaint);
                }
                else
                {
                    context.DrawRect(rect, borderPaint);
                }
            }

            // Value label
            if (_chart.Configuration.ShowValueLabels)
            {
                RenderValueLabel(context, rect, value);
            }
        }

        private void RenderValueLabel(IRenderContext context, SKRect rect, double value)
        {
            var config = _chart.Configuration;
            var labelText = value.ToString(config.ValueLabelFormat);

            using var textPaint = new SKPaint
            {
                Color = config.ValueLabelColor,
                TextSize = config.ValueLabelFontSize,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var textBounds = new SKRect();
            textPaint.MeasureText(labelText, ref textBounds);

            float x = rect.MidX;
            float y = rect.MidY - textBounds.MidY;

            context.DrawText(labelText, x, y, textPaint);
        }
    }
}
