using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents a line chart that renders data series as connected line segments.
/// </summary>
public class LineChart : ChartBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LineChart"/> class.
    /// </summary>
    public LineChart()
    {
        LineColor = SKColors.Blue;
        LineWidth = 2f;
        ShowMarkers = true;
        MarkerSize = 4f;
    }

    /// <summary>
    /// Gets or sets the color of the line.
    /// </summary>
    public SKColor LineColor { get; set; }

    /// <summary>
    /// Gets or sets the width of the line in pixels.
    /// </summary>
    public float LineWidth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether markers should be shown at data points.
    /// </summary>
    public bool ShowMarkers { get; set; }

    /// <summary>
    /// Gets or sets the size of markers in pixels.
    /// </summary>
    public float MarkerSize { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        // Add line series renderers
        foreach (var series in Series)
        {
            if (series.Count > 0)
            {
                queue.Add(new LineSeriesRenderer(series, this));
            }
        }
    }

    private class LineSeriesRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly LineChart _chart;

        public LineSeriesRenderer(IDataSeries<IDataPoint> series, LineChart chart)
        {
            _series = series;
            _chart = chart;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count < 2)
            {
                return;
            }

            using var path = new SKPath();
            using var linePaint = new SKPaint
            {
                Color = _chart.LineColor,
                StrokeWidth = _chart.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Build path
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

            // Draw line
            context.DrawPath(path, linePaint);

            // Draw markers if enabled
            if (_chart.ShowMarkers)
            {
                using var markerPaint = new SKPaint
                {
                    Color = _chart.LineColor,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };

                foreach (var point in _series)
                {
                    var screenPoint = _chart.Viewport.DataToScreen(point.X, point.Y);
                    context.DrawCircle(screenPoint.X, screenPoint.Y, _chart.MarkerSize, markerPaint);
                }
            }
        }
    }
}
