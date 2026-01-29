using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Pie and donut chart with support for exploded slices, labels, and gradients.
/// </summary>
public class PieChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, Dictionary<int, PieSliceStyle>> _sliceStyles = new();
    private readonly List<SKColor> _defaultColors = new()
    {
        new SKColor(52, 152, 219),   // Blue
        new SKColor(46, 204, 113),   // Green
        new SKColor(155, 89, 182),   // Purple
        new SKColor(241, 196, 15),   // Yellow
        new SKColor(231, 76, 60),    // Red
        new SKColor(26, 188, 156),   // Teal
        new SKColor(230, 126, 34),   // Orange
        new SKColor(149, 165, 166),  // Gray
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart()
    {
        Configuration = new PieChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public PieChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific slice in a series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="sliceIndex">The slice index.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSliceStyle(IDataSeries<IDataPoint> series, int sliceIndex, PieSliceStyle style)
    {
        if (!_sliceStyles.ContainsKey(series))
        {
            _sliceStyles[series] = new Dictionary<int, PieSliceStyle>();
        }
        _sliceStyles[series][sliceIndex] = style;
    }

    /// <summary>
    /// Gets the style for a specific slice, or creates a default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="sliceIndex">The slice index.</param>
    /// <returns>The slice style.</returns>
    public PieSliceStyle GetSliceStyle(IDataSeries<IDataPoint> series, int sliceIndex)
    {
        if (_sliceStyles.TryGetValue(series, out var styles) &&
            styles.TryGetValue(sliceIndex, out var style))
        {
            return style;
        }

        // Return default style with color from palette
        return new PieSliceStyle
        {
            FillColor = _defaultColors[sliceIndex % _defaultColors.Count]
        };
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add pie renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                queue.Add(new PieRenderer(series, this));
            }
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<SkiaCharts.Core.Legend.LegendItem> BuildLegendItems(Theming.ChartTheme theme)
    {
        var items = new List<Legend.LegendItem>();
        var palette = theme.ColorPalette;
        var colorIndex = 0;

        foreach (var series in Series)
        {
            var sliceIndex = 0;
            foreach (var point in series)
            {
                if (point.Y <= 0)
                {
                    sliceIndex++;
                    continue;
                }

                var style = GetSliceStyle(series, sliceIndex);
                var label = point is PieDataPoint piePoint && !string.IsNullOrWhiteSpace(piePoint.Label)
                    ? piePoint.Label!
                    : !string.IsNullOrWhiteSpace(style.Label)
                        ? style.Label!
                        : $"Slice {sliceIndex + 1}";

                var color = style.FillColor;
                if (color == SKColors.Empty)
                {
                    color = palette.GetColor(colorIndex);
                }

                items.Add(new SkiaCharts.Core.Legend.LegendItem
                {
                    Text = label,
                    Color = color,
                    SymbolType = SkiaCharts.Core.Legend.LegendSymbolType.Rectangle,
                    Data = point
                });

                sliceIndex++;
                colorIndex++;
            }
        }

        return items;
    }

    private class PieRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly PieChart _chart;

        public PieRenderer(IDataSeries<IDataPoint> series, PieChart chart)
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

            var config = _chart.Configuration;
            var bounds = _chart.Viewport.ScreenRect;

            // Calculate center and radius
            var centerX = bounds.Left + bounds.Width / 2f;
            var centerY = bounds.Top + bounds.Height / 2f;
            var availableSize = Math.Min(bounds.Width, bounds.Height);
            var outerRadius = (float)(availableSize / 2f * config.RadiusRatio);
            var innerRadius = config.IsDonut ? (float)(outerRadius * config.InnerRadiusRatio) : 0f;

            // Calculate total value
            double totalValue = 0;
            foreach (var point in _series)
            {
                if (point.Y > 0) // Only positive values
                {
                    totalValue += point.Y;
                }
            }

            if (totalValue == 0)
            {
                return; // Nothing to render
            }

            // Render slices
            var currentAngle = config.StartAngle;
            var sliceIndex = 0;
            var sliceInfo = new List<(SKRect rect, float startAngle, float sweepAngle, PieSliceStyle style, double value, string? label)>();

            foreach (var point in _series)
            {
                if (point.Y <= 0) // Skip non-positive values
                {
                    sliceIndex++;
                    continue;
                }

                var percentage = point.Y / totalValue;
                var sweepAngle = (float)(percentage * 360);
                var style = _chart.GetSliceStyle(_series, sliceIndex);

                // Get label
                string? label = null;
                if (point is PieDataPoint piePoint)
                {
                    label = piePoint.Label;
                }
                else
                {
                    label = style.Label;
                }

                // Calculate slice center for explosion
                var sliceCenterAngle = currentAngle + sweepAngle / 2f;
                var sliceCenterRad = sliceCenterAngle * (float)Math.PI / 180f;
                var explodeOffsetX = (float)Math.Cos(sliceCenterRad) * style.ExplodeDistance;
                var explodeOffsetY = (float)Math.Sin(sliceCenterRad) * style.ExplodeDistance;

                var sliceRect = new SKRect(
                    centerX - outerRadius + explodeOffsetX,
                    centerY - outerRadius + explodeOffsetY,
                    centerX + outerRadius + explodeOffsetX,
                    centerY + outerRadius + explodeOffsetY
                );

                sliceInfo.Add((sliceRect, currentAngle, sweepAngle, style, point.Y, label));

                // Render slice
                RenderSlice(context, sliceRect, currentAngle, sweepAngle, style, outerRadius, innerRadius,
                    new SKPoint(centerX + explodeOffsetX, centerY + explodeOffsetY));

                currentAngle += sweepAngle;
                sliceIndex++;
            }

            // Render labels
            if (config.LabelPosition != PieLabelPosition.None)
            {
                RenderLabels(context, sliceInfo, centerX, centerY, outerRadius, innerRadius, totalValue);
            }
        }

        private void RenderSlice(IRenderContext context, SKRect rect, float startAngle, float sweepAngle,
            PieSliceStyle style, float outerRadius, float innerRadius, SKPoint center)
        {
            using var path = new SKPath();

            if (innerRadius > 0)
            {
                // Donut slice
                path.AddArc(rect, startAngle, sweepAngle);

                var innerRect = new SKRect(
                    center.X - innerRadius,
                    center.Y - innerRadius,
                    center.X + innerRadius,
                    center.Y + innerRadius
                );

                var endAngle = startAngle + sweepAngle;
                var endRad = endAngle * (float)Math.PI / 180f;
                var endX = center.X + (float)Math.Cos(endRad) * innerRadius;
                var endY = center.Y + (float)Math.Sin(endRad) * innerRadius;

                path.LineTo(endX, endY);
                path.AddArc(innerRect, endAngle, -sweepAngle);
                path.Close();
            }
            else
            {
                // Pie slice
                path.MoveTo(center);
                path.AddArc(rect, startAngle, sweepAngle);
                path.Close();
            }

            // Fill
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            if (style.GradientColors != null && style.GradientColors.Length > 0)
            {
                // Radial gradient from center to edge
                fillPaint.Shader = SKShader.CreateRadialGradient(
                    center,
                    outerRadius,
                    style.GradientColors,
                    null,
                    SKShaderTileMode.Clamp
                );
            }
            else
            {
                fillPaint.Color = style.FillColor;
            }

            context.DrawPath(path, fillPaint);

            // Border
            if (style.BorderColor.HasValue)
            {
                using var borderPaint = new SKPaint
                {
                    Color = style.BorderColor.Value,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = style.BorderWidth,
                    IsAntialias = true
                };

                context.DrawPath(path, borderPaint);
            }
        }

        private void RenderLabels(IRenderContext context,
            List<(SKRect rect, float startAngle, float sweepAngle, PieSliceStyle style, double value, string? label)> slices,
            float centerX, float centerY, float outerRadius, float innerRadius, double totalValue)
        {
            var config = _chart.Configuration;

            using var textPaint = new SKPaint
            {
                Color = config.LabelColor,
                TextSize = config.LabelFontSize,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            using var linePaint = new SKPaint
            {
                Color = config.LeaderLineColor,
                StrokeWidth = config.LeaderLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            foreach (var (rect, startAngle, sweepAngle, style, value, label) in slices)
            {
                // Skip labels for very small slices
                if (sweepAngle < config.MinimumLabelAngle)
                {
                    continue;
                }

                var percentage = value / totalValue;
                var labelText = FormatLabel(config.LabelContent, value, percentage, label, config);

                if (string.IsNullOrEmpty(labelText))
                {
                    continue;
                }

                var midAngle = startAngle + sweepAngle / 2f;
                var midRad = midAngle * (float)Math.PI / 180f;

                var explodeOffsetX = (float)Math.Cos(midRad) * style.ExplodeDistance;
                var explodeOffsetY = (float)Math.Sin(midRad) * style.ExplodeDistance;

                if (config.LabelPosition == PieLabelPosition.Inside)
                {
                    // Position label inside the slice
                    var labelRadius = config.IsDonut
                        ? innerRadius + (outerRadius - innerRadius) / 2f
                        : outerRadius * 0.6f;

                    var labelX = centerX + (float)Math.Cos(midRad) * labelRadius + explodeOffsetX;
                    var labelY = centerY + (float)Math.Sin(midRad) * labelRadius + explodeOffsetY;

                    // Measure text to draw background
                    var textBounds = new SKRect();
                    textPaint.MeasureText(labelText, ref textBounds);

                    context.DrawText(labelText, labelX, labelY - textBounds.Height / 2f, textPaint);
                }
                else // Outside with leader line
                {
                    var innerPointRadius = outerRadius;
                    var innerX = centerX + (float)Math.Cos(midRad) * innerPointRadius + explodeOffsetX;
                    var innerY = centerY + (float)Math.Sin(midRad) * innerPointRadius + explodeOffsetY;

                    var outerPointRadius = outerRadius + config.LeaderLineLength;
                    var outerX = centerX + (float)Math.Cos(midRad) * outerPointRadius + explodeOffsetX;
                    var outerY = centerY + (float)Math.Sin(midRad) * outerPointRadius + explodeOffsetY;

                    // Draw leader line
                    context.DrawLine(innerX, innerY, outerX, outerY, linePaint);

                    // Adjust text alignment based on angle
                    var normalizedAngle = midAngle % 360;
                    if (normalizedAngle < 0) normalizedAngle += 360;

                    if (normalizedAngle > 90 && normalizedAngle < 270)
                    {
                        textPaint.TextAlign = SKTextAlign.Right;
                    }
                    else
                    {
                        textPaint.TextAlign = SKTextAlign.Left;
                    }

                    context.DrawText(labelText, outerX, outerY, textPaint);
                }
            }
        }

        private string FormatLabel(PieLabelContent content, double value, double percentage, string? label, PieChartConfiguration config)
        {
            return content switch
            {
                PieLabelContent.Percentage => percentage.ToString(config.PercentageFormat),
                PieLabelContent.Value => value.ToString(config.ValueFormat),
                PieLabelContent.Both => $"{value.ToString(config.ValueFormat)} ({percentage.ToString(config.PercentageFormat)})",
                PieLabelContent.Name => label ?? "",
                PieLabelContent.NameAndPercentage => string.IsNullOrEmpty(label)
                    ? percentage.ToString(config.PercentageFormat)
                    : $"{label}\n{percentage.ToString(config.PercentageFormat)}",
                PieLabelContent.NameAndValue => string.IsNullOrEmpty(label)
                    ? value.ToString(config.ValueFormat)
                    : $"{label}\n{value.ToString(config.ValueFormat)}",
                _ => ""
            };
        }
    }
}
