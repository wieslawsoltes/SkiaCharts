using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaCharts.Core.Axes;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Combo chart that combines multiple chart types with dual Y-axis support.
/// </summary>
public class ComboChart : ChartBase
{
    private readonly Dictionary<IDataSeries<IDataPoint>, ComboSeriesConfiguration> _seriesConfigurations = new();
    private ViewportManager? _secondaryViewport;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComboChart"/> class.
    /// </summary>
    public ComboChart()
    {
        Configuration = new ComboChartConfiguration();

        // Initialize default axes
        Configuration.PrimaryYAxis = new LinearAxis { Position = AxisPosition.Left };
        Configuration.SecondaryYAxis = new LinearAxis { Position = AxisPosition.Right };
    }

    /// <summary>
    /// Gets or sets the chart configuration.
    /// </summary>
    public ComboChartConfiguration Configuration { get; set; }

    /// <summary>
    /// Sets the configuration for a specific series.
    /// </summary>
    /// <param name="series">The series to configure.</param>
    /// <param name="config">The configuration to apply.</param>
    public void SetSeriesConfiguration(IDataSeries<IDataPoint> series, ComboSeriesConfiguration config)
    {
        _seriesConfigurations[series] = config;
    }

    /// <summary>
    /// Gets the configuration for a specific series, or creates a default configuration.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>The series configuration.</returns>
    public ComboSeriesConfiguration GetSeriesConfiguration(IDataSeries<IDataPoint> series)
    {
        if (_seriesConfigurations.TryGetValue(series, out var config))
        {
            return config;
        }

        // Return default configuration
        return new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Left,
            LineStyle = new LineSeriesStyle()
        };
    }

    /// <inheritdoc/>
    protected override void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        base.BuildRenderQueue(queue, context);

        if (Series.Count == 0)
        {
            return;
        }

        // Create secondary viewport if needed
        if (Configuration.ShowSecondaryYAxis && _secondaryViewport == null)
        {
            _secondaryViewport = new ViewportManager();
        }

        // Setup viewports for dual Y-axis if needed
        SetupViewports();

        // Group series by Y-axis and chart type
        var leftSeries = new List<(IDataSeries<IDataPoint> series, ComboSeriesConfiguration config)>();
        var rightSeries = new List<(IDataSeries<IDataPoint> series, ComboSeriesConfiguration config)>();

        foreach (var series in Series)
        {
            var config = GetSeriesConfiguration(series);
            if (config.YAxisSide == YAxisSide.Left)
            {
                leftSeries.Add((series, config));
            }
            else
            {
                rightSeries.Add((series, config));
            }
        }

        // Render left axis series
        foreach (var (series, config) in leftSeries)
        {
            AddSeriesRenderer(queue, series, config, Viewport, isSecondary: false);
        }

        // Render right axis series (if secondary axis is enabled)
        if (Configuration.ShowSecondaryYAxis && _secondaryViewport != null)
        {
            foreach (var (series, config) in rightSeries)
            {
                AddSeriesRenderer(queue, series, config, _secondaryViewport, isSecondary: true);
            }
        }
    }

    private void SetupViewports()
    {
        // Calculate Y ranges for each axis
        var leftYMin = double.MaxValue;
        var leftYMax = double.MinValue;
        var rightYMin = double.MaxValue;
        var rightYMax = double.MinValue;
        var hasRightSeries = false;

        foreach (var series in Series)
        {
            if (series.Count == 0) continue;

            var config = GetSeriesConfiguration(series);

            if (config.YAxisSide == YAxisSide.Left)
            {
                leftYMin = Math.Min(leftYMin, series.MinY);
                leftYMax = Math.Max(leftYMax, series.MaxY);
            }
            else
            {
                rightYMin = Math.Min(rightYMin, series.MinY);
                rightYMax = Math.Max(rightYMax, series.MaxY);
                hasRightSeries = true;
            }
        }

        // Setup primary viewport (left Y-axis)
        if (leftYMin != double.MaxValue && leftYMax != double.MinValue)
        {
            Viewport.YDataRange = new DataRange(leftYMin, leftYMax);
        }

        // Setup secondary viewport (right Y-axis) if needed
        if (Configuration.ShowSecondaryYAxis && hasRightSeries && _secondaryViewport != null)
        {
            if (rightYMin != double.MaxValue && rightYMax != double.MinValue)
            {
                _secondaryViewport.YDataRange = new DataRange(rightYMin, rightYMax);
            }

            // Synchronize screen rect with primary viewport
            _secondaryViewport.ScreenRect = Viewport.ScreenRect;
            _secondaryViewport.XDataRange = Viewport.XDataRange;
        }
    }

    private void AddSeriesRenderer(RenderQueue queue, IDataSeries<IDataPoint> series,
        ComboSeriesConfiguration config, ViewportManager viewport, bool isSecondary)
    {
        switch (config.ChartType)
        {
            case ComboSeriesType.Line:
                var lineStyle = config.LineStyle ?? new LineSeriesStyle();
                queue.Add(new ComboLineRenderer(series, this, lineStyle, viewport));
                break;

            case ComboSeriesType.Bar:
                var barStyle = config.BarStyle ?? new BarSeriesStyle();
                queue.Add(new ComboBarRenderer(series, this, barStyle, viewport, Configuration.BarOrientation));
                break;

            case ComboSeriesType.Area:
                var areaStyle = config.AreaStyle ?? new AreaSeriesStyle();
                queue.Add(new ComboAreaRenderer(series, this, areaStyle, viewport));
                break;

            case ComboSeriesType.Scatter:
                var scatterStyle = config.ScatterStyle ?? new ScatterSeriesStyle();
                queue.Add(new ComboScatterRenderer(series, this, scatterStyle, viewport));
                break;
        }
    }

    // Simplified renderers that delegate to existing chart rendering logic

    private class ComboLineRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly ComboChart _chart;
        private readonly LineSeriesStyle _style;
        private readonly ViewportManager _viewport;

        public ComboLineRenderer(IDataSeries<IDataPoint> series, ComboChart chart,
            LineSeriesStyle style, ViewportManager viewport)
        {
            _series = series;
            _chart = chart;
            _style = style;
            _viewport = viewport;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0) return;

            using var path = new SKPath();
            bool isFirst = true;
            IDataPoint? prevPoint = null;

            foreach (var point in _series)
            {
                var screenPoint = _viewport.DataToScreen(point.X, point.Y);

                if (isFirst)
                {
                    path.MoveTo(screenPoint);
                    isFirst = false;
                }
                else
                {
                    if (_style.LineMode == LineMode.Stepped && prevPoint != null)
                    {
                        var stepPoint = _viewport.DataToScreen(prevPoint.X, point.Y);
                        path.LineTo(stepPoint);
                        path.LineTo(screenPoint);
                    }
                    else
                    {
                        path.LineTo(screenPoint);
                    }
                }

                prevPoint = point;
            }

            using var linePaint = new SKPaint
            {
                Color = _style.LineColor,
                StrokeWidth = _style.LineWidth,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            context.DrawPath(path, linePaint);

            // Draw markers if enabled
            if (_style.MarkerShape != MarkerShape.None)
            {
                RenderMarkers(context);
            }
        }

        private void RenderMarkers(IRenderContext context)
        {
            var markerSize = _style.MarkerSize / 2f;

            using var fillPaint = new SKPaint
            {
                Color = _style.MarkerFillColor ?? _style.LineColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            foreach (var point in _series)
            {
                var screenPoint = _viewport.DataToScreen(point.X, point.Y);

                switch (_style.MarkerShape)
                {
                    case MarkerShape.Circle:
                        context.DrawCircle(screenPoint.X, screenPoint.Y, markerSize, fillPaint);
                        break;

                    case MarkerShape.Square:
                        context.DrawRect(new SKRect(
                            screenPoint.X - markerSize, screenPoint.Y - markerSize,
                            screenPoint.X + markerSize, screenPoint.Y + markerSize), fillPaint);
                        break;

                    case MarkerShape.Diamond:
                        using (var path = new SKPath())
                        {
                            path.MoveTo(screenPoint.X, screenPoint.Y - markerSize);
                            path.LineTo(screenPoint.X + markerSize, screenPoint.Y);
                            path.LineTo(screenPoint.X, screenPoint.Y + markerSize);
                            path.LineTo(screenPoint.X - markerSize, screenPoint.Y);
                            path.Close();
                            context.DrawPath(path, fillPaint);
                        }
                        break;
                }
            }
        }
    }

    private class ComboBarRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly ComboChart _chart;
        private readonly BarSeriesStyle _style;
        private readonly ViewportManager _viewport;
        private readonly BarOrientation _orientation;

        public ComboBarRenderer(IDataSeries<IDataPoint> series, ComboChart chart,
            BarSeriesStyle style, ViewportManager viewport, BarOrientation orientation)
        {
            _series = series;
            _chart = chart;
            _style = style;
            _viewport = viewport;
            _orientation = orientation;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0) return;

            var availableWidth = _viewport.ScreenRect.Width;
            var barSpacing = _series.Count > 0 ? availableWidth / _series.Count : availableWidth;
            var barWidth = (float)(barSpacing * _style.BarWidthRatio);

            using var fillPaint = new SKPaint
            {
                Color = _style.FillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            foreach (var point in _series)
            {
                var screenPoint = _viewport.DataToScreen(point.X, point.Y);
                var baselineScreen = _viewport.DataToScreen(point.X, 0);

                SKRect barRect;
                if (_orientation == BarOrientation.Vertical)
                {
                    var top = Math.Min(screenPoint.Y, baselineScreen.Y);
                    var bottom = Math.Max(screenPoint.Y, baselineScreen.Y);
                    barRect = new SKRect(
                        screenPoint.X - barWidth / 2f,
                        top,
                        screenPoint.X + barWidth / 2f,
                        bottom
                    );
                }
                else
                {
                    var left = Math.Min(screenPoint.X, baselineScreen.X);
                    var right = Math.Max(screenPoint.X, baselineScreen.X);
                    barRect = new SKRect(
                        left,
                        screenPoint.Y - barWidth / 2f,
                        right,
                        screenPoint.Y + barWidth / 2f
                    );
                }

                context.DrawRect(barRect, fillPaint);

                // Draw border if specified
                if (_style.BorderColor.HasValue)
                {
                    using var borderPaint = new SKPaint
                    {
                        Color = _style.BorderColor.Value,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = _style.BorderWidth,
                        IsAntialias = true
                    };
                    context.DrawRect(barRect, borderPaint);
                }
            }
        }
    }

    private class ComboAreaRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly ComboChart _chart;
        private readonly AreaSeriesStyle _style;
        private readonly ViewportManager _viewport;

        public ComboAreaRenderer(IDataSeries<IDataPoint> series, ComboChart chart,
            AreaSeriesStyle style, ViewportManager viewport)
        {
            _series = series;
            _chart = chart;
            _style = style;
            _viewport = viewport;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0) return;

            using var path = new SKPath();

            // Start from baseline
            var firstPoint = _series[0];
            var baselineY = _style.Baseline;
            var startScreen = _viewport.DataToScreen(firstPoint.X, baselineY);
            path.MoveTo(startScreen);

            // Draw top boundary
            foreach (var point in _series)
            {
                var screenPoint = _viewport.DataToScreen(point.X, point.Y);
                path.LineTo(screenPoint);
            }

            // Close path along baseline
            var lastPoint = _series[_series.Count - 1];
            var endScreen = _viewport.DataToScreen(lastPoint.X, baselineY);
            path.LineTo(endScreen);
            path.Close();

            // Fill area
            using var fillPaint = new SKPaint
            {
                Color = _style.FillColor.WithAlpha(_style.FillAlpha),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            context.DrawPath(path, fillPaint);

            // Draw boundary line if specified
            if (_style.ShowLine)
            {
                using var boundaryPath = new SKPath();
                bool isFirst = true;

                foreach (var point in _series)
                {
                    var screenPoint = _viewport.DataToScreen(point.X, point.Y);
                    if (isFirst)
                    {
                        boundaryPath.MoveTo(screenPoint);
                        isFirst = false;
                    }
                    else
                    {
                        boundaryPath.LineTo(screenPoint);
                    }
                }

                using var linePaint = new SKPaint
                {
                    Color = _style.LineColor,
                    StrokeWidth = _style.LineWidth,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                context.DrawPath(boundaryPath, linePaint);
            }
        }
    }

    private class ComboScatterRenderer : ChartElement
    {
        private readonly IDataSeries<IDataPoint> _series;
        private readonly ComboChart _chart;
        private readonly ScatterSeriesStyle _style;
        private readonly ViewportManager _viewport;

        public ComboScatterRenderer(IDataSeries<IDataPoint> series, ComboChart chart,
            ScatterSeriesStyle style, ViewportManager viewport)
        {
            _series = series;
            _chart = chart;
            _style = style;
            _viewport = viewport;
            Layer = RenderLayer.Data;
        }

        public override void Render(IRenderContext context)
        {
            if (_series.Count == 0) return;

            var markerSize = _style.MarkerSize / 2f;

            using var fillPaint = new SKPaint
            {
                Color = _style.FillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            foreach (var point in _series)
            {
                var screenPoint = _viewport.DataToScreen(point.X, point.Y);

                switch (_style.MarkerShape)
                {
                    case MarkerShape.Circle:
                        context.DrawCircle(screenPoint.X, screenPoint.Y, markerSize, fillPaint);
                        break;

                    case MarkerShape.Square:
                        context.DrawRect(new SKRect(
                            screenPoint.X - markerSize, screenPoint.Y - markerSize,
                            screenPoint.X + markerSize, screenPoint.Y + markerSize), fillPaint);
                        break;
                }

                // Draw border if specified
                if (_style.BorderColor.HasValue)
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
                            context.DrawCircle(screenPoint.X, screenPoint.Y, markerSize, borderPaint);
                            break;

                        case MarkerShape.Square:
                            context.DrawRect(new SKRect(
                                screenPoint.X - markerSize, screenPoint.Y - markerSize,
                                screenPoint.X + markerSize, screenPoint.Y + markerSize), borderPaint);
                            break;
                    }
                }
            }
        }
    }
}
