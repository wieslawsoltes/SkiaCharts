using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartBase"/> class.
    /// </summary>
    protected ChartBase()
    {
        _renderQueue = new RenderQueue();
        _viewportManager = new ViewportManager();

        Series = new DataSeriesCollection();
        ChartArea = new ChartArea();

        XAxis = new LinearAxis { Position = AxisPosition.Bottom };
        YAxis = new LinearAxis { Position = AxisPosition.Left };

        BackgroundColor = SKColors.White;
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
    /// Gets the viewport manager for coordinate transformations.
    /// </summary>
    public ViewportManager Viewport => _viewportManager;

    /// <summary>
    /// Gets or sets the background color of the chart.
    /// </summary>
    public SKColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the title of the chart.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Renders the entire chart to the specified canvas.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas to render to.</param>
    /// <param name="width">The width of the canvas.</param>
    /// <param name="height">The height of the canvas.</param>
    public void Render(SKCanvas canvas, float width, float height)
    {
        var context = new RenderContext(canvas, width, height);

        // Clear background
        context.Clear(BackgroundColor);

        // Calculate layout
        var totalBounds = new SKRect(0, 0, width, height);
        var plotArea = ChartArea.CalculatePlotArea(totalBounds);

        // Update viewport
        _viewportManager.ScreenRect = plotArea;

        // Auto-scale axes if needed
        if (XAxis.AutoScale)
        {
            var xRange = XAxis.CalculateOptimalRange(Series.XRange);
            XAxis.VisibleRange = xRange;
            _viewportManager.XDataRange = xRange;
        }

        if (YAxis.AutoScale)
        {
            var yRange = YAxis.CalculateOptimalRange(Series.YRange);
            YAxis.VisibleRange = yRange;
            _viewportManager.YDataRange = yRange;
        }

        // Build render queue
        _renderQueue.Clear();
        BuildRenderQueue(_renderQueue, context);

        // Render all elements
        _renderQueue.RenderAll(context);
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
        queue.Add(XAxis);
        queue.Add(YAxis);

        // Derived classes will add their specific renderable elements
    }

    /// <summary>
    /// Invalidates the chart, causing it to be redrawn on the next render cycle.
    /// </summary>
    public virtual void Invalidate()
    {
        // This will be connected to UI invalidation in the Avalonia integration
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
}
