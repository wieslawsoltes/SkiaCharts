using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Bubble chart with XY positioning and Z-dimension (size) visualization.
/// Supports variable bubble colors, opacity control, and label collision detection.
/// </summary>
public class BubbleChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, BubbleSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BubbleChart"/> class.
    /// </summary>
    public BubbleChart()
    {
        DefaultStyle = new BubbleSeriesStyle();
        Configuration = new BubbleChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public BubbleSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public BubbleChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, BubbleSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public BubbleSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add bubble renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new BubbleRenderer(series, this, style));
            }
        }
    }

    private class BubbleRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly BubbleChart _chart;
        private readonly BubbleSeriesStyle _style;

        public BubbleRenderer(IDataSeries<IDataPoint> series, BubbleChart chart, BubbleSeriesStyle style)
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

            // Calculate size and color ranges
            double minSize = double.MaxValue;
            double maxSize = double.MinValue;
            double minColorValue = double.MaxValue;
            double maxColorValue = double.MinValue;

            var bubbleData = new List<BubbleInfo>();

            foreach (var point in _series)
            {
                double size = GetSize(point);
                double colorValue = GetColorValue(point);

                minSize = Math.Min(minSize, size);
                maxSize = Math.Max(maxSize, size);
                minColorValue = Math.Min(minColorValue, colorValue);
                maxColorValue = Math.Max(maxColorValue, colorValue);

                bubbleData.Add(new BubbleInfo
                {
                    Point = point,
                    Size = size,
                    ColorValue = colorValue
                });
            }

            // Handle edge cases
            if (minSize == maxSize) maxSize = minSize + 1;
            if (minColorValue == maxColorValue) maxColorValue = minColorValue + 1;

            // Render bubbles
            var labelRects = new List<SKRect>();

            foreach (var bubble in bubbleData)
            {
                var screenPoint = _chart.Viewport.DataToScreen(bubble.Point.X, bubble.Point.Y);
                var bubbleRadius = CalculateBubbleRadius(bubble.Size, minSize, maxSize);
                var fillColor = CalculateFillColor(bubble.ColorValue, minColorValue, maxColorValue);

                RenderBubble(context, screenPoint, bubbleRadius, fillColor);

                // Render label if enabled and bubble is large enough
                if (_style.ShowLabels && bubbleRadius >= _style.MinLabelSize)
                {
                    var labelRect = RenderLabel(context, screenPoint, bubble, bubbleRadius, labelRects);
                    if (labelRect.HasValue)
                    {
                        labelRects.Add(labelRect.Value);
                    }
                }
            }
        }

        private double GetSize(IDataPoint point)
        {
            if (point is Charts.ScatterDataPoint scp)
            {
                return scp.Size;
            }
            // For regular data points, use a constant size
            return 1.0;
        }

        private double GetColorValue(IDataPoint point)
        {
            if (point is Charts.ScatterDataPoint scp)
            {
                return scp.ColorValue;
            }
            return 0.0;
        }

        private float CalculateBubbleRadius(double size, double minSize, double maxSize)
        {
            // Normalize size to 0-1 range
            var normalizedSize = (size - minSize) / (maxSize - minSize);

            float radius;
            switch (_style.SizeScale)
            {
                case BubbleSizeScale.Linear:
                    radius = _style.MinBubbleSize +
                            (float)(normalizedSize * (_style.MaxBubbleSize - _style.MinBubbleSize));
                    break;

                case BubbleSizeScale.Area:
                    // For area scaling, we want the area to be proportional to the value
                    // Area = π * r^2, so r = sqrt(Area / π)
                    // We scale the area linearly, then take sqrt for radius
                    var normalizedArea = normalizedSize;
                    var minArea = _style.MinBubbleSize * _style.MinBubbleSize;
                    var maxArea = _style.MaxBubbleSize * _style.MaxBubbleSize;
                    var targetArea = minArea + normalizedArea * (maxArea - minArea);
                    radius = (float)Math.Sqrt(targetArea);
                    break;

                case BubbleSizeScale.Logarithmic:
                    // For log scale, we need the original value, not normalized
                    // For now, use a simple log of the normalized value + 1 to avoid log(0)
                    var logValue = Math.Log(normalizedSize * 9 + 1) / Math.Log(10); // 0-1 range
                    radius = _style.MinBubbleSize +
                            (float)(logValue * (_style.MaxBubbleSize - _style.MinBubbleSize));
                    break;

                default:
                    radius = _style.MinBubbleSize;
                    break;
            }

            return radius;
        }

        private SKColor CalculateFillColor(double colorValue, double minColorValue, double maxColorValue)
        {
            if (!_style.UseColorMapping)
            {
                return _style.FillColor.WithAlpha(_style.Opacity);
            }

            // Normalize color value to 0-1 range
            var normalizedValue = (colorValue - minColorValue) / (maxColorValue - minColorValue);

            // Use custom color scale or default
            var colorScale = _style.ColorScale ?? new[]
            {
                new SKColor(0, 0, 255),      // Blue
                new SKColor(0, 255, 255),    // Cyan
                new SKColor(0, 255, 0),      // Green
                new SKColor(255, 255, 0),    // Yellow
                new SKColor(255, 0, 0)       // Red
            };

            var color = InterpolateColor(normalizedValue, colorScale);
            return color.WithAlpha(_style.Opacity);
        }

        private SKColor InterpolateColor(double t, SKColor[] colors)
        {
            if (colors.Length == 0) return SKColors.Black;
            if (colors.Length == 1) return colors[0];

            // Clamp t to 0-1
            t = Math.Max(0, Math.Min(1, t));

            // Find the two colors to interpolate between
            var scaledT = t * (colors.Length - 1);
            var index = (int)scaledT;
            var localT = scaledT - index;

            if (index >= colors.Length - 1)
            {
                return colors[colors.Length - 1];
            }

            var color1 = colors[index];
            var color2 = colors[index + 1];

            // Lerp between the two colors
            var r = (byte)(color1.Red + (color2.Red - color1.Red) * localT);
            var g = (byte)(color1.Green + (color2.Green - color1.Green) * localT);
            var b = (byte)(color1.Blue + (color2.Blue - color1.Blue) * localT);

            return new SKColor(r, g, b);
        }

        private void RenderBubble(IRenderContext context, SKPoint screenPoint, float radius, SKColor fillColor)
        {
            // Draw filled circle
            using var fillPaint = new SKPaint
            {
                Color = fillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            context.DrawCircle(screenPoint.X, screenPoint.Y, radius, fillPaint);

            // Draw border if specified
            if (_style.BorderColor.HasValue)
            {
                using var borderPaint = new SKPaint
                {
                    Color = _style.BorderColor.Value.WithAlpha(_style.BorderOpacity),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = _style.BorderWidth,
                    IsAntialias = true
                };

                context.DrawCircle(screenPoint.X, screenPoint.Y, radius, borderPaint);
            }
        }

        private SKRect? RenderLabel(IRenderContext context, SKPoint screenPoint, BubbleInfo bubble,
            float bubbleRadius, List<SKRect> existingLabels)
        {
            var labelText = FormatLabel(bubble);

            using var textPaint = new SKPaint
            {
                Color = _style.LabelColor,
                TextSize = _style.LabelFontSize,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            // Measure text
            var textBounds = new SKRect();
            textPaint.MeasureText(labelText, ref textBounds);

            // Position label in center of bubble
            var labelRect = new SKRect(
                screenPoint.X - textBounds.Width / 2 - _chart.Configuration.LabelCollisionPadding,
                screenPoint.Y - textBounds.Height / 2 - _chart.Configuration.LabelCollisionPadding,
                screenPoint.X + textBounds.Width / 2 + _chart.Configuration.LabelCollisionPadding,
                screenPoint.Y + textBounds.Height / 2 + _chart.Configuration.LabelCollisionPadding
            );

            // Check for collision if enabled
            if (_chart.Configuration.EnableLabelCollisionDetection)
            {
                foreach (var existingRect in existingLabels)
                {
                    if (labelRect.IntersectsWith(existingRect))
                    {
                        return null; // Skip this label
                    }
                }
            }

            // Draw label
            context.DrawText(labelText, screenPoint.X, screenPoint.Y + textBounds.Height / 2, textPaint);

            return labelRect;
        }

        private string FormatLabel(BubbleInfo bubble)
        {
            try
            {
                return string.Format(_style.LabelFormat, bubble.Point.X, bubble.Point.Y, bubble.Size);
            }
            catch
            {
                return bubble.Size.ToString("F1");
            }
        }

        private class BubbleInfo
        {
            public IDataPoint Point { get; set; } = null!;
            public double Size { get; set; }
            public double ColorValue { get; set; }
        }
    }
}
