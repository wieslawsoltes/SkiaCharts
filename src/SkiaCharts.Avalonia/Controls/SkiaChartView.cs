using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Avalonia.Controls;

/// <summary>
/// Avalonia control for rendering SkiaCharts.
/// </summary>
public partial class SkiaChartView : Control
{
    private ChartBase? _chart;
    private ChartTheme? _theme;
    private SKSize _lastSize;
    private SKBitmap? _cachedBitmap;
    private bool _isCacheDirty = true;

    static SkiaChartView()
    {
        AffectsRender<SkiaChartView>(
            ChartProperty,
            ChartThemeProperty,
            BackgroundProperty,
            TitleProperty,
            SubtitleProperty,
            SeriesProperty,
            ShowLegendProperty,
            LegendPositionProperty,
            ShowGridProperty,
            ShowMinorGridProperty,
            XAxisLabelProperty,
            YAxisLabelProperty,
            EnableAnimationsProperty,
            AnimationDurationProperty,
            EnableTooltipsProperty,
            EnableZoomProperty,
            EnablePanProperty,
            LineWidthProperty,
            MarkerSizeProperty,
            ShowMarkersProperty,
            EnableAntiAliasingProperty,
            DpiScaleProperty
        );

        AffectsMeasure<SkiaChartView>(
            ChartProperty
        );
    }

    #region Avalonia Properties

    /// <summary>
    /// Defines the <see cref="Chart"/> property.
    /// </summary>
    public static readonly StyledProperty<ChartBase?> ChartProperty =
        AvaloniaProperty.Register<SkiaChartView, ChartBase?>(nameof(Chart));

    /// <summary>
    /// Gets or sets the chart to display.
    /// </summary>
    public ChartBase? Chart
    {
        get => GetValue(ChartProperty);
        set => SetValue(ChartProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="ChartTheme"/> property.
    /// </summary>
    public static readonly StyledProperty<ChartTheme?> ChartThemeProperty =
        AvaloniaProperty.Register<SkiaChartView, ChartTheme?>(
            nameof(ChartTheme),
            ThemePresets.Light);

    /// <summary>
    /// Gets or sets the chart theme.
    /// </summary>
    public ChartTheme? ChartTheme
    {
        get => GetValue(ChartThemeProperty);
        set => SetValue(ChartThemeProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Background"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<SkiaChartView>();

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="EnableAntiAliasing"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableAntiAliasingProperty =
        AvaloniaProperty.Register<SkiaChartView, bool>(
            nameof(EnableAntiAliasing),
            defaultValue: true);

    /// <summary>
    /// Gets or sets whether anti-aliasing is enabled.
    /// </summary>
    public bool EnableAntiAliasing
    {
        get => GetValue(EnableAntiAliasingProperty);
        set => SetValue(EnableAntiAliasingProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="DpiScale"/> property.
    /// </summary>
    public static readonly StyledProperty<double> DpiScaleProperty =
        AvaloniaProperty.Register<SkiaChartView, double>(
            nameof(DpiScale),
            defaultValue: 1.0);

    /// <summary>
    /// Gets or sets the DPI scale factor.
    /// </summary>
    public double DpiScale
    {
        get => GetValue(DpiScaleProperty);
        set => SetValue(DpiScaleProperty, value);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SkiaChartView"/> class.
    /// </summary>
    public SkiaChartView()
    {
        ClipToBounds = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ChartProperty)
        {
            _chart = change.GetNewValue<ChartBase?>();
            _isCacheDirty = true;
            InvalidateVisual();
        }
        else if (change.Property == ChartThemeProperty)
        {
            _theme = change.GetNewValue<ChartTheme?>();
            _isCacheDirty = true;
            InvalidateVisual();
        }
        else if (change.Property == SeriesProperty ||
                 change.Property == TitleProperty ||
                 change.Property == SubtitleProperty ||
                 change.Property == ShowLegendProperty ||
                 change.Property == LegendPositionProperty ||
                 change.Property == ShowGridProperty ||
                 change.Property == ShowMinorGridProperty ||
                 change.Property == XAxisLabelProperty ||
                 change.Property == YAxisLabelProperty ||
                 change.Property == LineWidthProperty ||
                 change.Property == MarkerSizeProperty ||
                 change.Property == ShowMarkersProperty)
        {
            _isCacheDirty = true;
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Draw background
        if (Background != null)
        {
            context.DrawRectangle(Background, null, new Rect(Bounds.Size));
        }

        // Create and draw custom operation
        if (_chart != null)
        {
            var size = new SKSize((float)Bounds.Width, (float)Bounds.Height);
            _lastSize = size;

            var operation = new SkiaChartDrawOperation(
                new Rect(Bounds.Size),
                _chart,
                _theme ?? ThemePresets.Light,
                EnableAntiAliasing,
                (float)DpiScale);

            context.Custom(operation);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // Default size if no constraints
        var width = double.IsInfinity(availableSize.Width) ? 400 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height;

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    /// <summary>
    /// Invalidates the chart and requests a re-render.
    /// </summary>
    public void InvalidateChart()
    {
        _isCacheDirty = true;
        InvalidateVisual();
    }

    /// <summary>
    /// Gets the current rendering size.
    /// </summary>
    public SKSize RenderSize => _lastSize;

    /// <summary>
    /// Clears the render cache.
    /// </summary>
    public void ClearCache()
    {
        _cachedBitmap?.Dispose();
        _cachedBitmap = null;
        _isCacheDirty = true;
    }
}

/// <summary>
/// Custom draw operation for rendering charts with SkiaSharp.
/// </summary>
internal class SkiaChartDrawOperation : ICustomDrawOperation
{
    private readonly Rect _bounds;
    private readonly ChartBase _chart;
    private readonly ChartTheme _theme;
    private readonly bool _enableAntiAliasing;
    private readonly float _dpiScale;

    public SkiaChartDrawOperation(
        Rect bounds,
        ChartBase chart,
        ChartTheme theme,
        bool enableAntiAliasing,
        float dpiScale)
    {
        _bounds = bounds;
        _chart = chart;
        _theme = theme;
        _enableAntiAliasing = enableAntiAliasing;
        _dpiScale = dpiScale;
    }

    public Rect Bounds => _bounds;

    public void Dispose()
    {
        // Nothing to dispose
    }

    public bool Equals(ICustomDrawOperation? other)
    {
        // Always return false to force redraw
        // In production, implement proper equality checking for performance
        return false;
    }

    public bool HitTest(Point p) => _bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature == null)
            return;

        using var lease = leaseFeature.Lease();
        var canvas = lease?.SkCanvas;
        if (canvas == null)
            return;

        // Save canvas state
        canvas.Save();

        var previousTheme = _chart.Theme;

        try
        {
            _chart.Theme = _theme ?? ThemePresets.Light;

            // Apply DPI scaling if needed
            if (_dpiScale != 1.0f)
            {
                canvas.Scale(_dpiScale);
            }

            // Create render bounds
            var width = (float)_bounds.Width;
            var height = (float)_bounds.Height;

            if (_dpiScale != 1.0f)
            {
                width /= _dpiScale;
                height /= _dpiScale;
            }

            // Render the chart using the core rendering engine
            _chart.Render(canvas, width, height);
        }
        finally
        {
            _chart.Theme = previousTheme;
            canvas.Restore();
        }
    }
}
