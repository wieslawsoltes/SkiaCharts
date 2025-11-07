using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Polar chart for visualizing data in polar coordinates (angle, radius).
/// X values represent angles, Y values represent distances from center.
/// </summary>
public class PolarChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, PolarSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    public PolarChart()
    {
        DefaultStyle = new PolarSeriesStyle();
        Configuration = new PolarChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public PolarSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public PolarChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, PolarSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public PolarSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
    {
        return _seriesStyles.TryGetValue(series, out var style) ? style : DefaultStyle;
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        if (Series.Count == 0)
        {
            return;
        }

        // Add grid renderer
        queue.Add(new PolarGridRenderer(this));

        // Add series renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new PolarSeriesRenderer(series, this, style));
            }
        }
    }

    private class PolarGridRenderer : ChartElement
    {
        private readonly PolarChart _chart;

        public PolarGridRenderer(PolarChart chart)
        {
            _chart = chart;
            Layer = RenderLayer.Grid;
        }

        public override void Render(IRenderContext context)
        {
            var center = GetCenter();
            var radius = GetRadius();

            using var gridPaint = new SKPaint
            {
                Color = _chart.Configuration.GridLineColor,
                StrokeWidth = _chart.Configuration.GridLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Draw circular grid (radius circles)
            for (int i = 1; i <= _chart.Configuration.RadiusGridCircles; i++)
            {
                var circleRadius = radius * i / _chart.Configuration.RadiusGridCircles;
                context.DrawCircle(center.X, center.Y, circleRadius, gridPaint);
            }

            // Draw radial grid lines (angle spokes)
            for (int i = 0; i < _chart.Configuration.AngleGridLines; i++)
            {
                var angle = 360f * i / _chart.Configuration.AngleGridLines;
                var screenAngle = ConvertToScreenAngle(angle);
                var endPoint = GetPointOnCircle(center, radius, screenAngle);

                context.DrawLine(center.X, center.Y, endPoint.X, endPoint.Y, gridPaint);
            }

            // Draw angle labels
            if (_chart.Configuration.ShowAngleLabels)
            {
                RenderAngleLabels(context, center, radius);
            }
        }

        private void RenderAngleLabels(IRenderContext context, SKPoint center, float radius)
        {
            using var textPaint = new SKPaint
            {
                Color = _chart.Configuration.AngleLabelColor,
                TextSize = _chart.Configuration.AngleLabelFontSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            var labelRadius = radius + 15f;

            for (int i = 0; i < _chart.Configuration.AngleGridLines; i++)
            {
                var angle = 360f * i / _chart.Configuration.AngleGridLines;
                var screenAngle = ConvertToScreenAngle(angle);
                var labelPoint = GetPointOnCircle(center, labelRadius, screenAngle);

                var labelText = $"{(int)angle}°";
                context.DrawText(labelText, labelPoint.X, labelPoint.Y + textPaint.TextSize / 3, textPaint);
            }
        }

        private SKPoint GetCenter()
        {
            return new SKPoint(
                _chart.Viewport.ScreenRect.MidX,
                _chart.Viewport.ScreenRect.MidY
            );
        }

        private float GetRadius()
        {
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var availableHeight = _chart.Viewport.ScreenRect.Height;
            var maxRadius = Math.Min(availableWidth, availableHeight) / 2f;

            return maxRadius * (1f - _chart.Configuration.PaddingRatio);
        }

        private float ConvertToScreenAngle(float dataAngle)
        {
            // Convert data angle to screen angle based on configuration
            var angle = _chart.Configuration.StartAngle + dataAngle;
            if (!_chart.Configuration.Clockwise)
            {
                angle = _chart.Configuration.StartAngle - dataAngle;
            }
            return angle;
        }

        private SKPoint GetPointOnCircle(SKPoint center, float radius, float angleDegrees)
        {
            var radians = angleDegrees * Math.PI / 180.0;
            return new SKPoint(
                center.X + radius * (float)Math.Cos(radians),
                center.Y + radius * (float)Math.Sin(radians)
            );
        }
    }

    private class PolarSeriesRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly PolarChart _chart;
        private readonly PolarSeriesStyle _style;

        public PolarSeriesRenderer(IDataSeries<IDataPoint> series, PolarChart chart, PolarSeriesStyle style)
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

            var center = GetCenter();
            var chartRadius = GetChartRadius();
            var maxRadius = GetMaxRadius();
            var minRadius = _chart.Configuration.MinRadius;

            using var path = new SKPath();
            var points = new List<SKPoint>();
            bool isFirst = true;

            foreach (var point in _series)
            {
                // X = angle, Y = radius
                var angle = ConvertAngle(point.X);
                var radiusValue = point.Y;

                // Normalize radius to 0-1 range
                var normalizedRadius = (radiusValue - minRadius) / (maxRadius - minRadius);
                normalizedRadius = Math.Max(0, Math.Min(1, normalizedRadius));

                var screenRadius = chartRadius * (float)normalizedRadius;
                var screenAngle = ConvertToScreenAngle(angle);
                var screenPoint = GetPointOnCircle(center, screenRadius, screenAngle);

                points.Add(screenPoint);

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

            // Draw line
            using var linePaint = new SKPaint
            {
                Color = _style.LineColor,
                StrokeWidth = _style.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            if (_style.DashPattern != null)
            {
                linePaint.PathEffect = SKPathEffect.CreateDash(_style.DashPattern, 0);
            }

            context.DrawPath(path, linePaint);

            // Draw markers if enabled
            if (_style.ShowMarkers)
            {
                RenderMarkers(context, points);
            }
        }

        private void RenderMarkers(IRenderContext context, List<SKPoint> points)
        {
            var markerColor = _style.MarkerColor ?? _style.LineColor;

            using var markerPaint = new SKPaint
            {
                Color = markerColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            foreach (var point in points)
            {
                context.DrawCircle(point.X, point.Y, _style.MarkerSize / 2f, markerPaint);
            }
        }

        private SKPoint GetCenter()
        {
            return new SKPoint(
                _chart.Viewport.ScreenRect.MidX,
                _chart.Viewport.ScreenRect.MidY
            );
        }

        private float GetChartRadius()
        {
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var availableHeight = _chart.Viewport.ScreenRect.Height;
            var maxRadius = Math.Min(availableWidth, availableHeight) / 2f;

            return maxRadius * (1f - _chart.Configuration.PaddingRatio);
        }

        private double GetMaxRadius()
        {
            if (_chart.Configuration.MaxRadius.HasValue)
            {
                return _chart.Configuration.MaxRadius.Value;
            }

            // Find max radius across all series
            double max = double.MinValue;
            foreach (var series in _chart.Series)
            {
                max = Math.Max(max, series.MaxY);
            }

            return max;
        }

        private float ConvertAngle(double dataAngle)
        {
            // Convert from degrees or radians
            if (!_chart.Configuration.AngleInDegrees)
            {
                dataAngle = dataAngle * 180.0 / Math.PI;
            }

            return (float)dataAngle;
        }

        private float ConvertToScreenAngle(float dataAngle)
        {
            // Convert data angle to screen angle based on configuration
            var angle = _chart.Configuration.StartAngle + dataAngle;
            if (!_chart.Configuration.Clockwise)
            {
                angle = _chart.Configuration.StartAngle - dataAngle;
            }
            return angle;
        }

        private SKPoint GetPointOnCircle(SKPoint center, float radius, float angleDegrees)
        {
            var radians = angleDegrees * Math.PI / 180.0;
            return new SKPoint(
                center.X + radius * (float)Math.Cos(radians),
                center.Y + radius * (float)Math.Sin(radians)
            );
        }
    }
}
