using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Scatter chart with support for variable marker sizes, colors, and shapes.
/// </summary>
public class ScatterChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, ScatterSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterChart"/> class.
    /// </summary>
    public ScatterChart()
    {
        DefaultStyle = new ScatterSeriesStyle();
        Configuration = new ScatterChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public ScatterSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public ScatterChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, ScatterSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public ScatterSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add scatter renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new ScatterRenderer(series, this, style));
            }
        }
    }

    private class ScatterRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly ScatterChart _chart;
        private readonly ScatterSeriesStyle _style;

        public ScatterRenderer(IDataSeries<IDataPoint> series, ScatterChart chart, ScatterSeriesStyle style)
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

            // Render connecting lines if enabled
            if (_chart.Configuration.ShowConnectingLines && _series.Count >= 2)
            {
                RenderConnectingLines(context);
            }

            // Calculate value ranges for variable sizing and color mapping
            double minSize = double.MaxValue;
            double maxSize = double.MinValue;
            double minColorValue = double.MaxValue;
            double maxColorValue = double.MinValue;

            if (_style.UseVariableSizes || _style.UseColorMapping)
            {
                foreach (var point in _series)
                {
                    if (point is ScatterDataPoint scp)
                    {
                        if (_style.UseVariableSizes)
                        {
                            minSize = Math.Min(minSize, scp.Size);
                            maxSize = Math.Max(maxSize, scp.Size);
                        }
                        if (_style.UseColorMapping)
                        {
                            minColorValue = Math.Min(minColorValue, scp.ColorValue);
                            maxColorValue = Math.Max(maxColorValue, scp.ColorValue);
                        }
                    }
                }

                // Handle edge cases
                if (minSize == maxSize) maxSize = minSize + 1;
                if (minColorValue == maxColorValue) maxColorValue = minColorValue + 1;
            }

            // Render markers
            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);

                float markerSize = _style.MarkerSize;
                SKColor fillColor = _style.FillColor;

                // Apply variable sizing
                if (_style.UseVariableSizes && point is ScatterDataPoint scp)
                {
                    var normalizedSize = (scp.Size - minSize) / (maxSize - minSize);
                    markerSize = _style.MinMarkerSize +
                                (float)(normalizedSize * (_style.MaxMarkerSize - _style.MinMarkerSize));
                }

                // Apply color mapping
                if (_style.UseColorMapping && point is ScatterDataPoint scpColor)
                {
                    fillColor = MapValueToColor(scpColor.ColorValue, minColorValue, maxColorValue, _style);
                }

                RenderMarker(context, screenPoint, markerSize, fillColor);
            }
        }

        private void RenderConnectingLines(IRenderContext context)
        {
            using var path = new SKPath();
            bool isFirst = true;

            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);

                if (isFirst)
                {
                    path.MoveTo(screenPoint);
                    isFirst = false;
                }
                else
                {
                    path.LineTo(screenPoint);
                }
            }

            using var linePaint = new SKPaint
            {
                Color = _chart.Configuration.ConnectingLineColor.WithAlpha(_chart.Configuration.ConnectingLineAlpha),
                StrokeWidth = _chart.Configuration.ConnectingLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            context.DrawPath(path, linePaint);
        }

        private SKColor MapValueToColor(double value, double minValue, double maxValue, ScatterSeriesStyle style)
        {
            var normalizedValue = (value - minValue) / (maxValue - minValue);

            var colorScale = style.ColorScale ?? new[]
            {
                new SKColor(0, 0, 255),      // Blue (low)
                new SKColor(0, 255, 255),    // Cyan
                new SKColor(0, 255, 0),      // Green
                new SKColor(255, 255, 0),    // Yellow
                new SKColor(255, 0, 0)       // Red (high)
            };

            // Find the two colors to interpolate between
            var scaledValue = normalizedValue * (colorScale.Length - 1);
            var index = (int)Math.Floor(scaledValue);
            var t = scaledValue - index;

            if (index >= colorScale.Length - 1)
            {
                return colorScale[colorScale.Length - 1];
            }

            var color1 = colorScale[index];
            var color2 = colorScale[index + 1];

            // Interpolate
            var r = (byte)(color1.Red + (color2.Red - color1.Red) * t);
            var g = (byte)(color1.Green + (color2.Green - color1.Green) * t);
            var b = (byte)(color1.Blue + (color2.Blue - color1.Blue) * t);

            return new SKColor(r, g, b);
        }

        private void RenderMarker(IRenderContext context, SKPoint center, float size, SKColor fillColor)
        {
            var halfSize = size / 2f;

            // Fill
            using var fillPaint = new SKPaint
            {
                Color = fillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            switch (_style.MarkerShape)
            {
                case MarkerShape.Circle:
                    context.DrawCircle(center.X, center.Y, halfSize, fillPaint);
                    break;

                case MarkerShape.Square:
                    context.DrawRect(new SKRect(center.X - halfSize, center.Y - halfSize,
                                                center.X + halfSize, center.Y + halfSize), fillPaint);
                    break;

                case MarkerShape.Diamond:
                    using (var path = new SKPath())
                    {
                        path.MoveTo(center.X, center.Y - halfSize); // Top
                        path.LineTo(center.X + halfSize, center.Y); // Right
                        path.LineTo(center.X, center.Y + halfSize); // Bottom
                        path.LineTo(center.X - halfSize, center.Y); // Left
                        path.Close();
                        context.DrawPath(path, fillPaint);
                    }
                    break;

                case MarkerShape.Triangle:
                    using (var path = new SKPath())
                    {
                        var height = size * 0.866f; // sqrt(3)/2
                        path.MoveTo(center.X, center.Y - height / 2); // Top
                        path.LineTo(center.X + halfSize, center.Y + height / 2); // Bottom right
                        path.LineTo(center.X - halfSize, center.Y + height / 2); // Bottom left
                        path.Close();
                        context.DrawPath(path, fillPaint);
                    }
                    break;

                case MarkerShape.TriangleDown:
                    using (var path = new SKPath())
                    {
                        var height = size * 0.866f;
                        path.MoveTo(center.X, center.Y + height / 2); // Bottom
                        path.LineTo(center.X + halfSize, center.Y - height / 2); // Top right
                        path.LineTo(center.X - halfSize, center.Y - height / 2); // Top left
                        path.Close();
                        context.DrawPath(path, fillPaint);
                    }
                    break;

                case MarkerShape.Cross:
                    using (var path = new SKPath())
                    {
                        var offset = halfSize * 0.707f; // sqrt(2)/2
                        path.MoveTo(center.X - offset, center.Y - offset);
                        path.LineTo(center.X + offset, center.Y + offset);
                        path.MoveTo(center.X + offset, center.Y - offset);
                        path.LineTo(center.X - offset, center.Y + offset);
                        fillPaint.Style = SKPaintStyle.Stroke;
                        fillPaint.StrokeWidth = _style.BorderWidth;
                        context.DrawPath(path, fillPaint);
                    }
                    break;

                case MarkerShape.Plus:
                    using (var path = new SKPath())
                    {
                        path.MoveTo(center.X, center.Y - halfSize);
                        path.LineTo(center.X, center.Y + halfSize);
                        path.MoveTo(center.X - halfSize, center.Y);
                        path.LineTo(center.X + halfSize, center.Y);
                        fillPaint.Style = SKPaintStyle.Stroke;
                        fillPaint.StrokeWidth = _style.BorderWidth;
                        context.DrawPath(path, fillPaint);
                    }
                    break;

                case MarkerShape.None:
                    return;
            }

            // Border (for filled shapes only)
            if (_style.BorderColor.HasValue &&
                _style.MarkerShape != MarkerShape.Cross &&
                _style.MarkerShape != MarkerShape.Plus &&
                _style.MarkerShape != MarkerShape.None)
            {
                using var borderPaint = new SKPaint
                {
                    Color = _style.BorderColor.Value,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = _style.BorderWidth,
                    IsAntialias = true
                };

                switch (_style.MarkerShape)
                {
                    case MarkerShape.Circle:
                        context.DrawCircle(center.X, center.Y, halfSize, borderPaint);
                        break;

                    case MarkerShape.Square:
                        context.DrawRect(new SKRect(center.X - halfSize, center.Y - halfSize,
                                                    center.X + halfSize, center.Y + halfSize), borderPaint);
                        break;

                    case MarkerShape.Diamond:
                        using (var path = new SKPath())
                        {
                            path.MoveTo(center.X, center.Y - halfSize);
                            path.LineTo(center.X + halfSize, center.Y);
                            path.LineTo(center.X, center.Y + halfSize);
                            path.LineTo(center.X - halfSize, center.Y);
                            path.Close();
                            context.DrawPath(path, borderPaint);
                        }
                        break;

                    case MarkerShape.Triangle:
                        using (var path = new SKPath())
                        {
                            var height = size * 0.866f;
                            path.MoveTo(center.X, center.Y - height / 2);
                            path.LineTo(center.X + halfSize, center.Y + height / 2);
                            path.LineTo(center.X - halfSize, center.Y + height / 2);
                            path.Close();
                            context.DrawPath(path, borderPaint);
                        }
                        break;

                    case MarkerShape.TriangleDown:
                        using (var path = new SKPath())
                        {
                            var height = size * 0.866f;
                            path.MoveTo(center.X, center.Y + height / 2);
                            path.LineTo(center.X + halfSize, center.Y - height / 2);
                            path.LineTo(center.X - halfSize, center.Y - height / 2);
                            path.Close();
                            context.DrawPath(path, borderPaint);
                        }
                        break;
                }
            }
        }
    }
}
