using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Enhanced line chart with support for multiple styles, markers, fills, and line modes.
/// </summary>
public class LineChartEnhanced : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, LineSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LineChartEnhanced"/> class.
    /// </summary>
    public LineChartEnhanced()
    {
        DefaultStyle = new LineSeriesStyle();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public LineSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, LineSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public LineSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add line series renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new LineSeriesRenderer(series, this, style));
            }
        }
    }

    private class LineSeriesRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly LineChartEnhanced _chart;
        private readonly LineSeriesStyle _style;

        public LineSeriesRenderer(IDataSeries<IDataPoint> series, LineChartEnhanced chart, LineSeriesStyle style)
        {
            _series = series;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count < 1)
            {
                return;
            }

            // Render filled area first (behind line)
            if (_style.FillArea && _series.Count >= 2)
            {
                RenderFill(context);
            }

            // Render line
            if (_series.Count >= 2)
            {
                RenderLine(context);
            }

            // Render markers last (on top)
            if (_style.MarkerShape != MarkerShape.None)
            {
                RenderMarkers(context);
            }
        }

        private void RenderFill(IRenderContext context)
        {
            using var path = new SKPath();
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // Determine fill color
            if (_style.FillColor.HasValue)
            {
                fillPaint.Color = _style.FillColor.Value;
            }
            else
            {
                fillPaint.Color = _style.LineColor.WithAlpha(_style.FillAlpha);
            }

            // Build fill path
            bool isFirst = true;
            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);

                if (isFirst)
                {
                    // Start from bottom of chart
                    var bottomPoint = _chart.Viewport.DataToScreen(point.X, _chart.YAxis!.VisibleRange.Min);
                    path.MoveTo(bottomPoint);
                    path.LineTo(screenPoint);
                    isFirst = false;
                }
                else
                {
                    path.LineTo(screenPoint);
                }
            }

            // Close path to bottom
            var lastPoint = _series[_series.Count - 1];
            var lastBottomPoint = _chart.Viewport.DataToScreen(lastPoint.X, _chart.YAxis!.VisibleRange.Min);
            path.LineTo(lastBottomPoint);
            path.Close();

            context.DrawPath(path, fillPaint);
        }

        private void RenderLine(IRenderContext context)
        {
            using var path = BuildLinePath();
            using var linePaint = new SKPaint
            {
                Color = _style.LineColor,
                StrokeWidth = _style.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            // Apply dash pattern if specified
            if (_style.DashPattern != null && _style.DashPattern.Length > 0)
            {
                linePaint.PathEffect = SKPathEffect.CreateDash(_style.DashPattern, 0);
            }

            context.DrawPath(path, linePaint);
        }

        private SKPath BuildLinePath()
        {
            var path = new SKPath();

            switch (_style.LineMode)
            {
                case LineMode.Linear:
                    BuildLinearPath(path);
                    break;
                case LineMode.Stepped:
                    BuildSteppedPath(path);
                    break;
                case LineMode.Smooth:
                    BuildSmoothPath(path);
                    break;
            }

            return path;
        }

        private void BuildLinearPath(SKPath path)
        {
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
        }

        private void BuildSteppedPath(SKPath path)
        {
            bool isFirst = true;
            SKPoint previousPoint = default;

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
                    // Horizontal then vertical
                    path.LineTo(screenPoint.X, previousPoint.Y);
                    path.LineTo(screenPoint);
                }

                previousPoint = screenPoint;
            }
        }

        private void BuildSmoothPath(SKPath path)
        {
            if (_series.Count < 2)
            {
                return;
            }

            // Convert to screen coordinates
            var points = new List<SKPoint>();
            foreach (var point in _series)
            {
                points.Add(_chart.Viewport.DataToScreen(point.X, point.Y));
            }

            if (points.Count == 2)
            {
                // Only two points, draw straight line
                path.MoveTo(points[0]);
                path.LineTo(points[1]);
                return;
            }

            // Calculate control points for cubic Bezier
            var tension = Math.Clamp(_style.SmoothTension, 0f, 1f);
            path.MoveTo(points[0]);

            for (int i = 0; i < points.Count - 1; i++)
            {
                var p0 = i > 0 ? points[i - 1] : points[i];
                var p1 = points[i];
                var p2 = points[i + 1];
                var p3 = i < points.Count - 2 ? points[i + 2] : p2;

                // Calculate control points using Catmull-Rom to Bezier conversion
                var cp1X = p1.X + (p2.X - p0.X) * tension / 6f;
                var cp1Y = p1.Y + (p2.Y - p0.Y) * tension / 6f;
                var cp2X = p2.X - (p3.X - p1.X) * tension / 6f;
                var cp2Y = p2.Y - (p3.Y - p1.Y) * tension / 6f;

                path.CubicTo(cp1X, cp1Y, cp2X, cp2Y, p2.X, p2.Y);
            }
        }

        private void RenderMarkers(IRenderContext context)
        {
            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);
                RenderMarker(context, screenPoint, _style);
            }
        }

        private void RenderMarker(IRenderContext context, SKPoint center, LineSeriesStyle style)
        {
            var size = style.MarkerSize;
            var halfSize = size / 2f;

            // Fill
            if (style.MarkerShape != MarkerShape.None)
            {
                using var fillPaint = new SKPaint
                {
                    Color = style.MarkerFillColor ?? style.LineColor,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };

                switch (style.MarkerShape)
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
                            fillPaint.StrokeWidth = style.MarkerStrokeWidth;
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
                            fillPaint.StrokeWidth = style.MarkerStrokeWidth;
                            context.DrawPath(path, fillPaint);
                        }
                        break;
                }

                // Stroke (outline)
                if (style.MarkerStrokeColor.HasValue &&
                    style.MarkerShape != MarkerShape.Cross &&
                    style.MarkerShape != MarkerShape.Plus)
                {
                    using var strokePaint = new SKPaint
                    {
                        Color = style.MarkerStrokeColor.Value,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = style.MarkerStrokeWidth,
                        IsAntialias = true
                    };

                    switch (style.MarkerShape)
                    {
                        case MarkerShape.Circle:
                            context.DrawCircle(center.X, center.Y, halfSize, strokePaint);
                            break;

                        case MarkerShape.Square:
                            context.DrawRect(new SKRect(center.X - halfSize, center.Y - halfSize,
                                                        center.X + halfSize, center.Y + halfSize), strokePaint);
                            break;

                        // Add stroke for other shapes similarly
                    }
                }
            }
        }
    }
}
