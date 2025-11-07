using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Area chart with support for stacking, transparency, gradient fills,
/// and smooth/stepped modes.
/// </summary>
public class AreaChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, AreaSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AreaChart"/> class.
    /// </summary>
    public AreaChart()
    {
        DefaultStyle = new AreaSeriesStyle();
        Configuration = new AreaChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public AreaSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public AreaChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, AreaSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public AreaSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add area renderers
        if (Series.Count > 0)
        {
            if (Configuration.StackMode == AreaStackMode.Stacked)
            {
                queue.Add(new StackedAreaRenderer(Series, this));
            }
            else
            {
                foreach (var series in Series)
                {
                    if (series.Count > 0)
                    {
                        var style = GetSeriesStyle(series);
                        queue.Add(new AreaRenderer(series, this, style));
                    }
                }
            }
        }
    }

    private class AreaRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly AreaChart _chart;
        private readonly AreaSeriesStyle _style;

        public AreaRenderer(IDataSeries<IDataPoint> series, AreaChart chart, AreaSeriesStyle style)
        {
            _series = series;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count < 2)
            {
                return;
            }

            // Render filled area
            RenderArea(context);

            // Render boundary line if enabled
            if (_style.ShowLine)
            {
                RenderLine(context);
            }
        }

        private void RenderArea(IRenderContext context)
        {
            using var path = BuildAreaPath();
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            // Apply gradient or solid fill
            if (_style.GradientColors != null && _style.GradientColors.Length >= 2)
            {
                ApplyGradient(fillPaint, context);
            }
            else
            {
                fillPaint.Color = _style.FillColor.WithAlpha(_style.FillAlpha);
            }

            context.DrawPath(path, fillPaint);
        }

        private void ApplyGradient(SKPaint paint, IRenderContext context)
        {
            var bounds = context.Canvas.DeviceClipBounds;

            switch (_style.GradientDirection)
            {
                case GradientDirection.Vertical:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(bounds.MidX, bounds.Top),
                        new SKPoint(bounds.MidX, bounds.Bottom),
                        _style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;

                case GradientDirection.Horizontal:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(bounds.Left, bounds.MidY),
                        new SKPoint(bounds.Right, bounds.MidY),
                        _style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;

                case GradientDirection.Radial:
                    var radius = Math.Max(bounds.Width, bounds.Height) / 2;
                    paint.Shader = SKShader.CreateRadialGradient(
                        new SKPoint(bounds.MidX, bounds.MidY),
                        radius,
                        _style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;
            }
        }

        private SKPath BuildAreaPath()
        {
            var path = new SKPath();

            // Start at baseline
            var firstPoint = _series[0];
            var baselineStart = _chart.Viewport.DataToScreen(firstPoint.X, _style.Baseline);
            path.MoveTo(baselineStart);

            // Build top boundary
            switch (_style.AreaMode)
            {
                case AreaMode.Linear:
                    BuildLinearBoundary(path);
                    break;
                case AreaMode.Stepped:
                    BuildSteppedBoundary(path);
                    break;
                case AreaMode.Smooth:
                    BuildSmoothBoundary(path);
                    break;
            }

            // Close to baseline
            var lastPoint = _series[_series.Count - 1];
            var baselineEnd = _chart.Viewport.DataToScreen(lastPoint.X, _style.Baseline);
            path.LineTo(baselineEnd);
            path.Close();

            return path;
        }

        private void BuildLinearBoundary(SKPath path)
        {
            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);
                path.LineTo(screenPoint);
            }
        }

        private void BuildSteppedBoundary(SKPath path)
        {
            SKPoint previousPoint = default;
            bool isFirst = true;

            foreach (var point in _series)
            {
                var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);

                if (!isFirst)
                {
                    // Horizontal then vertical
                    path.LineTo(screenPoint.X, previousPoint.Y);
                }

                path.LineTo(screenPoint);
                previousPoint = screenPoint;
                isFirst = false;
            }
        }

        private void BuildSmoothBoundary(SKPath path)
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
                path.LineTo(points[0]);
                path.LineTo(points[1]);
                return;
            }

            // Calculate control points for cubic Bezier
            var tension = Math.Clamp(_style.SmoothTension, 0f, 1f);
            path.LineTo(points[0]);

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

            switch (_style.AreaMode)
            {
                case AreaMode.Linear:
                    BuildLinearLinePath(path);
                    break;
                case AreaMode.Stepped:
                    BuildSteppedLinePath(path);
                    break;
                case AreaMode.Smooth:
                    BuildSmoothLinePath(path);
                    break;
            }

            return path;
        }

        private void BuildLinearLinePath(SKPath path)
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

        private void BuildSteppedLinePath(SKPath path)
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
                    path.LineTo(screenPoint.X, previousPoint.Y);
                    path.LineTo(screenPoint);
                }

                previousPoint = screenPoint;
            }
        }

        private void BuildSmoothLinePath(SKPath path)
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

                var cp1X = p1.X + (p2.X - p0.X) * tension / 6f;
                var cp1Y = p1.Y + (p2.Y - p0.Y) * tension / 6f;
                var cp2X = p2.X - (p3.X - p1.X) * tension / 6f;
                var cp2Y = p2.Y - (p3.Y - p1.Y) * tension / 6f;

                path.CubicTo(cp1X, cp1Y, cp2X, cp2Y, p2.X, p2.Y);
            }
        }
    }

    private class StackedAreaRenderer : ChartElement
    {
        private readonly DataSeriesCollection _allSeries;
        private readonly AreaChart _chart;

        public StackedAreaRenderer(DataSeriesCollection allSeries, AreaChart chart)
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

            // Build cumulative data for stacking
            var cumulativeData = new Dictionary<double, double>();
            foreach (var x in sortedX)
            {
                cumulativeData[x] = 0;
            }

            // Render each area stacked on previous
            foreach (var series in _allSeries)
            {
                if (series.Count < 2) continue;

                var style = _chart.GetSeriesStyle(series);
                RenderStackedArea(context, series, sortedX, cumulativeData, style);

                // Update cumulative values
                foreach (var point in series)
                {
                    if (cumulativeData.ContainsKey(point.X))
                    {
                        cumulativeData[point.X] += point.Y;
                    }
                }
            }
        }

        private void RenderStackedArea(
            IRenderContext context,
            IDataSeries<IDataPoint> series,
            List<double> sortedX,
            Dictionary<double, double> cumulativeData,
            AreaSeriesStyle style)
        {
            using var path = new SKPath();

            // Build bottom boundary (cumulative from previous areas)
            var firstX = sortedX[0];
            var firstCumulative = cumulativeData[firstX];
            var startPoint = _chart.Viewport.DataToScreen(firstX, firstCumulative);
            path.MoveTo(startPoint);

            foreach (var x in sortedX)
            {
                var y = cumulativeData[x];
                var screenPoint = _chart.Viewport.DataToScreen(x, y);
                path.LineTo(screenPoint);
            }

            // Build top boundary (current series values + cumulative)
            for (int i = sortedX.Count - 1; i >= 0; i--)
            {
                var x = sortedX[i];
                var point = series.FirstOrDefault(p => Math.Abs(p.X - x) < 0.0001);
                var y = cumulativeData[x] + (point?.Y ?? 0);
                var screenPoint = _chart.Viewport.DataToScreen(x, y);
                path.LineTo(screenPoint);
            }

            path.Close();

            // Fill
            using var fillPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            if (style.GradientColors != null && style.GradientColors.Length >= 2)
            {
                ApplyGradient(fillPaint, context, style);
            }
            else
            {
                fillPaint.Color = style.FillColor.WithAlpha(style.FillAlpha);
            }

            context.DrawPath(path, fillPaint);

            // Render boundary line if enabled
            if (style.ShowLine)
            {
                RenderStackedLine(context, series, cumulativeData, style);
            }
        }

        private void RenderStackedLine(
            IRenderContext context,
            IDataSeries<IDataPoint> series,
            Dictionary<double, double> cumulativeData,
            AreaSeriesStyle style)
        {
            using var path = new SKPath();
            bool isFirst = true;

            foreach (var point in series)
            {
                var y = cumulativeData[point.X] + point.Y;
                var screenPoint = _chart.Viewport.DataToScreen(point.X, y);

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
                Color = style.LineColor,
                StrokeWidth = style.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            if (style.DashPattern != null && style.DashPattern.Length > 0)
            {
                linePaint.PathEffect = SKPathEffect.CreateDash(style.DashPattern, 0);
            }

            context.DrawPath(path, linePaint);
        }

        private void ApplyGradient(SKPaint paint, IRenderContext context, AreaSeriesStyle style)
        {
            var bounds = context.Canvas.DeviceClipBounds;

            switch (style.GradientDirection)
            {
                case GradientDirection.Vertical:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(bounds.MidX, bounds.Top),
                        new SKPoint(bounds.MidX, bounds.Bottom),
                        style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;

                case GradientDirection.Horizontal:
                    paint.Shader = SKShader.CreateLinearGradient(
                        new SKPoint(bounds.Left, bounds.MidY),
                        new SKPoint(bounds.Right, bounds.MidY),
                        style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;

                case GradientDirection.Radial:
                    var radius = Math.Max(bounds.Width, bounds.Height) / 2;
                    paint.Shader = SKShader.CreateRadialGradient(
                        new SKPoint(bounds.MidX, bounds.MidY),
                        radius,
                        style.GradientColors!,
                        null,
                        SKShaderTileMode.Clamp
                    );
                    break;
            }
        }
    }
}
