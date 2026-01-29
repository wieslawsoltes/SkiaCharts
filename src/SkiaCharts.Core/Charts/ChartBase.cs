using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Legend;
using SkiaCharts.Core.Rendering;
using SkiaCharts.Core.Theming;
using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Abstract base class for all chart types.
/// Provides common functionality for data series, axes, and rendering.
/// </summary>
public abstract class ChartBase
{
    private readonly RenderQueue _renderQueue;
    private readonly ViewportManager _viewportManager;
    private readonly RenderCache _renderCache;
    private readonly DirtyRegionTracker _dirtyRegions;
    private ChartTheme _theme = ThemePresets.Light;
    private AccessibilityOptions _accessibility = AccessibilityOptions.Default;
    private SKRect _chartBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartBase"/> class.
    /// </summary>
    protected ChartBase()
    {
        _renderQueue = new RenderQueue();
        _viewportManager = new ViewportManager();
        _renderCache = new RenderCache();
        _dirtyRegions = new DirtyRegionTracker();

        Series = new DataSeriesCollection();
        ChartArea = new ChartArea();

        XAxis = new LinearAxis { Position = AxisPosition.Bottom };
        YAxis = new LinearAxis { Position = AxisPosition.Left };

        BackgroundColor = SKColors.White;
        Theme = ThemePresets.Light;

        Legend = new LegendManager();
        TitleManager = new TitleManager();
        DataLabels = new DataLabelManager();
        AutoGenerateLegend = true;
        Accessibility = AccessibilityOptions.Default;
    }

    /// <summary>
    /// Gets the collection of data series in this chart.
    /// </summary>
    public DataSeriesCollection Series { get; }

    /// <summary>
    /// Gets or sets the X axis.
    /// </summary>
    public IAxis XAxis { get; set; }

    /// <summary>
    /// Gets or sets the Y axis.
    /// </summary>
    public IAxis YAxis { get; set; }

    /// <summary>
    /// Gets the chart area (margins and padding).
    /// </summary>
    public ChartArea ChartArea { get; }

    /// <summary>
    /// Gets the legend manager.
    /// </summary>
    public LegendManager Legend { get; }

    /// <summary>
    /// Gets the title manager.
    /// </summary>
    public TitleManager TitleManager { get; }

    /// <summary>
    /// Gets the data label manager.
    /// </summary>
    public DataLabelManager DataLabels { get; }

    /// <summary>
    /// Gets the viewport manager for coordinate transformations.
    /// </summary>
    public ViewportManager Viewport => _viewportManager;

    /// <summary>
    /// Gets the render cache used for layer caching.
    /// </summary>
    public RenderCache RenderCache => _renderCache;

    /// <summary>
    /// Gets the dirty region tracker for fine-grained invalidation.
    /// </summary>
    public DirtyRegionTracker DirtyRegions => _dirtyRegions;

    /// <summary>
    /// Gets the current chart bounds used for layout (excluding title area).
    /// </summary>
    protected SKRect ChartBounds => _chartBounds;

    /// <summary>
    /// Gets or sets the background color of the chart.
    /// </summary>
    public SKColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the chart theme used for rendering.
    /// </summary>
    public ChartTheme Theme
    {
        get => _theme;
        set
        {
            _theme = value ?? ThemePresets.Light;
            BackgroundColor = _theme.Background.Color;
        }
    }

    /// <summary>
    /// Gets or sets the title of the chart.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the subtitle of the chart.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets whether to auto-generate legend items.
    /// </summary>
    public bool AutoGenerateLegend { get; set; }

    /// <summary>
    /// Gets or sets accessibility options for rendering.
    /// </summary>
    public AccessibilityOptions Accessibility
    {
        get => _accessibility;
        set => _accessibility = value ?? AccessibilityOptions.Default;
    }

    /// <summary>
    /// Renders the entire chart to the specified canvas.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to render to.</param>
    /// <param name="width">The width of the canvas.</param>
    /// <param name="height">The height of the canvas.</param>
    public void Render(SKCanvas canvas, float width, float height)
    {
        var context = new SkiaCharts.Core.Rendering.RenderContext(canvas, width, height);

        ApplyTheme(Theme);

        // Clear background
        context.Clear(BackgroundColor);

        // Calculate layout
        var totalBounds = new SKRect(0, 0, width, height);
        TitleManager.Title = Title;
        TitleManager.Subtitle = Subtitle;
        TitleManager.CalculateLayout(totalBounds);

        var chartBounds = totalBounds;
        var titleHeight = TitleManager.CalculateTotalHeight();
        if (titleHeight > 0)
        {
            chartBounds = new SKRect(
                totalBounds.Left,
                totalBounds.Top + titleHeight,
                totalBounds.Right,
                totalBounds.Bottom);
        }

        _chartBounds = chartBounds;

        if (AutoGenerateLegend)
        {
            PopulateLegendItems(Theme);
        }

        Legend.CalculateLayout(chartBounds);

        var plotArea = ChartArea.CalculatePlotArea(chartBounds);

        // Update viewport
        _viewportManager.ScreenRect = plotArea;

        // Auto-scale axes if needed
        var xRange = XAxis.VisibleRange;
        if (XAxis.AutoScale)
        {
            xRange = XAxis.CalculateOptimalRange(Series.XRange);
            XAxis.VisibleRange = xRange;
        }
        _viewportManager.XDataRange = xRange;

        var yRange = YAxis.VisibleRange;
        if (YAxis.AutoScale)
        {
            yRange = YAxis.CalculateOptimalRange(Series.YRange);
            YAxis.VisibleRange = yRange;
        }
        _viewportManager.YDataRange = yRange;

        ApplyAxisTransforms(_viewportManager, XAxis, YAxis);

        DataLabels.CalculateLayout(plotArea);

        // Build render queue
        _renderQueue.Clear();
        BuildRenderQueue(_renderQueue, context);

        // Add overlays
        if (TitleManager.HasTitle || TitleManager.HasSubtitle)
        {
            _renderQueue.Add(new TitleRenderElement(TitleManager, totalBounds));
        }

        if (Legend.IsVisible && Legend.Items.Count > 0)
        {
            _renderQueue.Add(new LegendRenderElement(Legend, chartBounds));
        }

        if (DataLabels.IsEnabled && DataLabels.Labels.Count > 0)
        {
            _renderQueue.Add(new DataLabelRenderElement(DataLabels));
        }

        // Render all elements
        if (_renderCache.IsEnabled)
        {
            RenderWithCache(context);
        }
        else
        {
            _renderQueue.RenderAll(context);
            _dirtyRegions.Clear();
        }
    }

    /// <summary>
    /// Builds the render queue with all elements that should be drawn.
    /// Override this method in derived classes to add chart-specific rendering.
    /// </summary>
    /// <param name="queue">The render queue to populate.</param>
    /// <param name="context">The render context.</param>
    protected virtual void BuildRenderQueue(RenderQueue queue, IRenderContext context)
    {
        // Add axes
        queue.Add(new AxisRenderElement(ChartArea, Viewport, XAxis, Theme, _chartBounds));
        queue.Add(new AxisRenderElement(ChartArea, Viewport, YAxis, Theme, _chartBounds));

        // Derived classes will add their specific renderable elements
    }

    /// <summary>
    /// Builds legend items for the current chart.
    /// </summary>
    /// <param name="theme">The theme to use for colors.</param>
    /// <returns>Legend items.</returns>
    protected virtual IEnumerable<SkiaCharts.Core.Legend.LegendItem> BuildLegendItems(ChartTheme theme)
    {
        var palette = theme.ColorPalette;
        var index = 0;

        foreach (var series in Series)
        {
            var name = string.IsNullOrWhiteSpace(series.Name)
                ? $"Series {index + 1}"
                : series.Name!;

            yield return new SkiaCharts.Core.Legend.LegendItem
            {
                Text = name,
                Color = palette.GetColor(index),
                SymbolType = SkiaCharts.Core.Legend.LegendSymbolType.Line,
                Data = series
            };

            index++;
        }
    }

    private void PopulateLegendItems(ChartTheme theme)
    {
        Legend.Clear();
        foreach (var item in BuildLegendItems(theme))
        {
            Legend.AddItem(item);
        }
    }

    private void ApplyTheme(ChartTheme theme)
    {
        BackgroundColor = theme.Background.Color;

        TitleManager.TitleColor = theme.Title.Color;
        TitleManager.TitleFontSize = theme.Title.FontSize;
        TitleManager.TitleFontFamily = theme.Fonts.TitleFontFamily;
        TitleManager.TitleFontStyle = theme.Title.FontStyle;
        TitleManager.SubtitleColor = theme.Title.SubtitleColor;
        TitleManager.SubtitleFontSize = theme.Title.SubtitleFontSize;
        TitleManager.SubtitleFontFamily = theme.Fonts.LabelFontFamily;

        Legend.BackgroundColor = theme.Legend.BackgroundColor;
        Legend.BorderColor = theme.Legend.BorderColor;
        Legend.BorderWidth = theme.Legend.BorderWidth;
        Legend.TextColor = theme.Legend.TextColor;
        Legend.FontSize = theme.Legend.FontSize;
        Legend.Padding = theme.Legend.Padding;
        Legend.CornerRadius = theme.Legend.CornerRadius;
        Legend.FontFamily = theme.Fonts.LabelFontFamily;
        Legend.SymbolSize = theme.Series.MarkerSize;

        DataLabels.TextColor = theme.Axis.LabelColor;
        DataLabels.FontSize = theme.Axis.LabelFontSize;
        DataLabels.FontFamily = theme.Fonts.LabelFontFamily;
    }

    /// <summary>
    /// Invalidates the chart, causing it to be redrawn on the next render cycle.
    /// </summary>
    public virtual void Invalidate()
    {
        _dirtyRegions.MarkAllDirty();
        _renderCache.InvalidateAll();
    }

    /// <summary>
    /// Invalidates a specific layer or region.
    /// </summary>
    /// <param name="layer">The render layer to invalidate.</param>
    /// <param name="region">Optional region in screen coordinates.</param>
    public void Invalidate(RenderLayer layer, SKRect? region = null)
    {
        if (region.HasValue)
        {
            _dirtyRegions.MarkDirty(layer, region.Value);
        }
        else
        {
            _dirtyRegions.MarkDirty(layer);
            _renderCache.Invalidate(layer);
        }
    }

    /// <summary>
    /// Performs hit testing at the specified screen coordinates.
    /// </summary>
    /// <param name="x">The X coordinate in screen space.</param>
    /// <param name="y">The Y coordinate in screen space.</param>
    /// <returns>The hit element, or null if nothing was hit.</returns>
    public virtual ChartElement? HitTest(float x, float y)
    {
        // Will be implemented when we add interactive elements
        return null;
    }

    /// <summary>
    /// Converts screen coordinates to data coordinates.
    /// </summary>
    /// <param name="screenX">The screen X coordinate.</param>
    /// <param name="screenY">The screen Y coordinate.</param>
    /// <returns>The data coordinates.</returns>
    public (double dataX, double dataY) ScreenToData(float screenX, float screenY)
    {
        return _viewportManager.ScreenToData(screenX, screenY);
    }

    /// <summary>
    /// Converts data coordinates to screen coordinates.
    /// </summary>
    /// <param name="dataX">The data X coordinate.</param>
    /// <param name="dataY">The data Y coordinate.</param>
    /// <returns>The screen coordinates.</returns>
    public SKPoint DataToScreen(double dataX, double dataY)
    {
        return _viewportManager.DataToScreen(dataX, dataY);
    }

    /// <summary>
    /// Applies axis-specific transforms to the specified viewport.
    /// </summary>
    /// <param name="viewport">The viewport to configure.</param>
    /// <param name="xAxis">The X axis.</param>
    /// <param name="yAxis">The Y axis.</param>
    protected void ApplyAxisTransforms(ViewportManager viewport, IAxis xAxis, IAxis yAxis)
    {
        var (xTransform, xInverse) = GetAxisTransforms(xAxis);
        var (yTransform, yInverse) = GetAxisTransforms(yAxis);

        viewport.SetXTransform(xTransform, xInverse);
        viewport.SetYTransform(yTransform, yInverse);
    }

    /// <summary>
    /// Applies pattern fills when accessibility options or series styles request it.
    /// </summary>
    /// <param name="paint">The paint to configure.</param>
    /// <param name="seriesIndex">The series index (used to pick a pattern).</param>
    /// <param name="foreground">The foreground color for the pattern.</param>
    /// <param name="patternOverride">Optional explicit pattern override.</param>
    /// <param name="patternScale">Optional pattern scale override.</param>
    /// <returns>True if a pattern fill was applied.</returns>
    protected bool TryApplyPatternFill(
        SKPaint paint,
        int seriesIndex,
        SKColor foreground,
        SkiaCharts.Core.Theming.PatternType? patternOverride = null,
        float? patternScale = null)
    {
        if (!Accessibility.UsePatternFills && !patternOverride.HasValue && !IsPatternFillTheme)
        {
            return false;
        }

        var pattern = patternOverride ?? SkiaCharts.Core.Theming.PatternFills.GetCategoricalPattern(seriesIndex);
        var scale = patternScale ?? Accessibility.PatternScale;
        if (scale <= 0)
        {
            scale = 1.0f;
        }

        paint.Shader = SkiaCharts.Core.Theming.PatternFills.CreatePattern(pattern, foreground, BackgroundColor, scale);
        return true;
    }

    /// <summary>
    /// Gets whether the current theme is the pattern-fill print theme.
    /// </summary>
    protected bool IsPatternFillTheme =>
        string.Equals(Theme.Name, SkiaCharts.Core.Theming.PrintThemes.PatternFill.Name, System.StringComparison.OrdinalIgnoreCase);

    private void RenderWithCache(IRenderContext context)
    {
        foreach (RenderLayer layer in System.Enum.GetValues(typeof(RenderLayer)))
        {
            if (_renderCache.CachedLayers.Contains(layer))
            {
                SKRect? dirtyRegion = null;
                bool fullDirty = false;

                if (_dirtyRegions.TryGetDirtyRegion(layer, out var region, out var isFull))
                {
                    fullDirty = isFull;
                    if (!isFull)
                    {
                        dirtyRegion = region;
                    }
                }

                _renderCache.RenderLayer(
                    context,
                    layer,
                    layerContext => _renderQueue.RenderLayer(layerContext, layer),
                    dirtyRegion,
                    fullDirty);
            }
            else
            {
                _renderQueue.RenderLayer(context, layer);
            }
        }

        _dirtyRegions.Clear();
    }

    private static (Func<double, double>? transform, Func<double, double>? inverse) GetAxisTransforms(IAxis axis)
    {
        if (axis is LogarithmicAxis logAxis)
        {
            return (logAxis.ToLog, logAxis.FromLog);
        }

        return (null, null);
    }
}
