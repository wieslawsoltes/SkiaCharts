using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Radar chart (spider chart) for visualizing multi-dimensional data.
/// Each axis represents a different variable, arranged in a circular layout.
/// </summary>
public class RadarChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, RadarSeriesStyle> _seriesStyles = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RadarChart"/> class.
    /// </summary>
    public RadarChart()
    {
        DefaultStyle = new RadarSeriesStyle();
        Configuration = new RadarChartConfiguration();
    }

    /// <summary>
    /// Gets or sets the default style for series without explicit styles.
    /// </summary>
    public RadarSeriesStyle DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public RadarChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the style for a specific series.
    /// </summary>
    /// <param name="series">The series to style.</param>
    /// <param name="style">The style to apply.</param>
    public void SetSeriesStyle(IDataSeries<IDataPoint> series, RadarSeriesStyle style)
    {
        _seriesStyles[series] = style;
    }

    /// <summary>
    /// Gets the style for a specific series, or the default style if not set.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series style.</returns>
    public RadarSeriesStyle GetSeriesStyle(IDataSeries<IDataPoint> series)
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
        queue.Add(new RadarGridRenderer(this));

        // Add series renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                var style = GetSeriesStyle(series);
                queue.Add(new RadarSeriesRenderer(series, this, style));
            }
        }
    }

    private class RadarGridRenderer : ChartElement
    {
        private readonly RadarChart _chart;

        public RadarGridRenderer(RadarChart chart)
        {
            _chart = chart;
            Layer = RenderLayer.Grid;
        }

        public override void Render(IRenderContext context)
        {
            var center = GetCenter();
            var radius = GetRadius();

            // Draw circular grid levels
            using var gridPaint = new SKPaint
            {
                Color = _chart.Configuration.GridLineColor,
                StrokeWidth = _chart.Configuration.GridLineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            for (int i = 1; i <= _chart.Configuration.GridLevels; i++)
            {
                var levelRadius = radius * i / _chart.Configuration.GridLevels;
                context.DrawCircle(center.X, center.Y, levelRadius, gridPaint);
            }

            // Draw spoke lines (radial axes)
            if (_chart.Configuration.ShowSpokeLines)
            {
                var axisCount = GetAxisCount();

                using var spokePaint = new SKPaint
                {
                    Color = _chart.Configuration.SpokeLineColor,
                    StrokeWidth = _chart.Configuration.SpokeLineWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                for (int i = 0; i < axisCount; i++)
                {
                    var angle = GetAngleForAxis(i, axisCount);
                    var endPoint = GetPointOnCircle(center, radius, angle);

                    context.DrawLine(center.X, center.Y, endPoint.X, endPoint.Y, spokePaint);
                }
            }

            // Draw axis labels
            if (_chart.Configuration.ShowAxisLabels && _chart.Configuration.AxisLabels != null)
            {
                RenderAxisLabels(context, center, radius);
            }
        }

        private void RenderAxisLabels(IRenderContext context, SKPoint center, float radius)
        {
            var labels = _chart.Configuration.AxisLabels!;
            var axisCount = labels.Length;

            using var textPaint = new SKPaint
            {
                Color = _chart.Configuration.AxisLabelColor,
                TextSize = _chart.Configuration.AxisLabelFontSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            for (int i = 0; i < axisCount; i++)
            {
                var angle = GetAngleForAxis(i, axisCount);
                var labelRadius = radius + _chart.Configuration.LabelOffset;
                var labelPoint = GetPointOnCircle(center, labelRadius, angle);

                // Adjust text alignment based on position
                var radians = angle * Math.PI / 180.0;
                if (Math.Abs(Math.Cos(radians)) < 0.1)
                {
                    textPaint.TextAlign = SKTextAlign.Center;
                }
                else if (Math.Cos(radians) > 0)
                {
                    textPaint.TextAlign = SKTextAlign.Left;
                    labelPoint.X += 5;
                }
                else
                {
                    textPaint.TextAlign = SKTextAlign.Right;
                    labelPoint.X -= 5;
                }

                context.DrawText(labels[i], labelPoint.X, labelPoint.Y + textPaint.TextSize / 3, textPaint);
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

        private int GetAxisCount()
        {
            // Get the maximum number of data points across all series
            int maxCount = 0;
            foreach (var series in _chart.Series)
            {
                maxCount = Math.Max(maxCount, series.Count);
            }

            // If axis labels are provided, use that count
            if (_chart.Configuration.AxisLabels != null)
            {
                maxCount = Math.Max(maxCount, _chart.Configuration.AxisLabels.Length);
            }

            return Math.Max(3, maxCount); // Minimum 3 axes for a radar chart
        }

        private float GetAngleForAxis(int axisIndex, int totalAxes)
        {
            var angleStep = 360f / totalAxes;
            return _chart.Configuration.StartAngle + axisIndex * angleStep;
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

    private class RadarSeriesRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly RadarChart _chart;
        private readonly RadarSeriesStyle _style;

        public RadarSeriesRenderer(IDataSeries<IDataPoint> series, RadarChart chart, RadarSeriesStyle style)
        {
            _series = series;
            _chart = chart;
            _style = style;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count < 3)
            {
                return; // Need at least 3 points for a radar chart
            }

            var center = GetCenter();
            var radius = GetRadius();
            var maxValue = GetMaxValue();
            var minValue = _chart.Configuration.MinValue;

            // Create path for the series
            using var path = new SKPath();
            var points = new List<SKPoint>();

            for (int i = 0; i < _series.Count; i++)
            {
                var point = _series[i];
                var value = point.Y;

                // Normalize value to 0-1 range
                var normalizedValue = (value - minValue) / (maxValue - minValue);
                normalizedValue = Math.Max(0, Math.Min(1, normalizedValue));

                var pointRadius = radius * (float)normalizedValue;
                var angle = GetAngleForAxis(i, _series.Count);
                var screenPoint = GetPointOnCircle(center, pointRadius, angle);

                points.Add(screenPoint);

                if (i == 0)
                {
                    path.MoveTo(screenPoint);
                }
                else
                {
                    path.LineTo(screenPoint);
                }
            }

            // Close the path
            path.Close();

            // Fill area if enabled
            if (_style.FillArea)
            {
                using var fillPaint = new SKPaint
                {
                    Color = _style.FillColor.WithAlpha(_style.FillAlpha),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };

                context.DrawPath(path, fillPaint);
            }

            // Draw outline
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
            var markerColor = _style.MarkerFillColor ?? _style.LineColor;

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

        private float GetRadius()
        {
            var availableWidth = _chart.Viewport.ScreenRect.Width;
            var availableHeight = _chart.Viewport.ScreenRect.Height;
            var maxRadius = Math.Min(availableWidth, availableHeight) / 2f;

            return maxRadius * (1f - _chart.Configuration.PaddingRatio);
        }

        private double GetMaxValue()
        {
            if (_chart.Configuration.MaxValue.HasValue)
            {
                return _chart.Configuration.MaxValue.Value;
            }

            // Find max value across all series
            double max = double.MinValue;
            foreach (var series in _chart.Series)
            {
                max = Math.Max(max, series.MaxY);
            }

            return max;
        }

        private float GetAngleForAxis(int axisIndex, int totalAxes)
        {
            var angleStep = 360f / totalAxes;
            return _chart.Configuration.StartAngle + axisIndex * angleStep;
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
