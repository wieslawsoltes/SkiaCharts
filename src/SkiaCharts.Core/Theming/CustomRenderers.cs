using SkiaSharp;
using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Interactivity;

namespace SkiaCharts.Core.Theming;

/// <summary>
/// Context information passed to custom renderers.
/// </summary>
public class RenderContext
{
    /// <summary>
    /// Gets the SKCanvas to draw on.
    /// </summary>
    public required SKCanvas Canvas { get; init; }

    /// <summary>
    /// Gets the current chart theme.
    /// </summary>
    public required ChartTheme Theme { get; init; }

    /// <summary>
    /// Gets the viewport for coordinate transformation.
    /// </summary>
    public required Viewport Viewport { get; init; }

    /// <summary>
    /// Gets the available drawing bounds.
    /// </summary>
    public required SKRect Bounds { get; init; }

    /// <summary>
    /// Gets additional custom data.
    /// </summary>
    public Dictionary<string, object> CustomData { get; } = new();
}

/// <summary>
/// Interface for custom marker renderers.
/// </summary>
public interface IMarkerRenderer
{
    /// <summary>
    /// Renders a marker at the specified position.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="position">The marker position in screen coordinates.</param>
    /// <param name="size">The marker size.</param>
    /// <param name="color">The marker color.</param>
    void RenderMarker(RenderContext context, SKPoint position, float size, SKColor color);
}

/// <summary>
/// Interface for custom line renderers.
/// </summary>
public interface ILineRenderer
{
    /// <summary>
    /// Renders a line between points.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="points">The line points in screen coordinates.</param>
    /// <param name="lineWidth">The line width.</param>
    /// <param name="color">The line color.</param>
    void RenderLine(RenderContext context, SKPoint[] points, float lineWidth, SKColor color);
}

/// <summary>
/// Interface for custom area renderers.
/// </summary>
public interface IAreaRenderer
{
    /// <summary>
    /// Renders a filled area.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="points">The area boundary points in screen coordinates.</param>
    /// <param name="color">The fill color.</param>
    /// <param name="opacity">The fill opacity (0-1).</param>
    void RenderArea(RenderContext context, SKPoint[] points, SKColor color, float opacity);
}

/// <summary>
/// Interface for custom bar renderers.
/// </summary>
public interface IBarRenderer
{
    /// <summary>
    /// Renders a bar.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="bounds">The bar bounds in screen coordinates.</param>
    /// <param name="color">The bar color.</param>
    /// <param name="borderColor">The border color.</param>
    /// <param name="borderWidth">The border width.</param>
    void RenderBar(RenderContext context, SKRect bounds, SKColor color, SKColor borderColor, float borderWidth);
}

/// <summary>
/// Interface for custom grid renderers.
/// </summary>
public interface IGridRenderer
{
    /// <summary>
    /// Renders the chart grid.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="horizontalLines">Horizontal grid line positions (Y coordinates).</param>
    /// <param name="verticalLines">Vertical grid line positions (X coordinates).</param>
    void RenderGrid(RenderContext context, float[] horizontalLines, float[] verticalLines);
}

/// <summary>
/// Interface for custom axis renderers.
/// </summary>
public interface IAxisRenderer
{
    /// <summary>
    /// Renders an axis.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="axis">The axis to render.</param>
    /// <param name="isHorizontal">True if the axis is horizontal.</param>
    void RenderAxis(RenderContext context, IAxis axis, bool isHorizontal);
}

/// <summary>
/// Interface for custom legend renderers.
/// </summary>
public interface ILegendRenderer
{
    /// <summary>
    /// Renders the chart legend.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="items">The legend items.</param>
    /// <param name="bounds">The legend bounds.</param>
    void RenderLegend(RenderContext context, LegendItem[] items, SKRect bounds);
}

/// <summary>
/// Represents a legend item.
/// </summary>
public class LegendItem
{
    /// <summary>
    /// Gets or sets the item label.
    /// </summary>
    public required string Label { get; init; }

    /// <summary>
    /// Gets or sets the item color.
    /// </summary>
    public required SKColor Color { get; init; }

    /// <summary>
    /// Gets or sets the item marker shape.
    /// </summary>
    public MarkerShape Shape { get; init; } = MarkerShape.Circle;

    /// <summary>
    /// Gets or sets whether the item is visible.
    /// </summary>
    public bool IsVisible { get; init; } = true;
}

/// <summary>
/// Marker shape enumeration.
/// </summary>
public enum MarkerShape
{
    /// <summary>Circle marker.</summary>
    Circle,
    /// <summary>Square marker.</summary>
    Square,
    /// <summary>Triangle marker.</summary>
    Triangle,
    /// <summary>Diamond marker.</summary>
    Diamond,
    /// <summary>Cross marker.</summary>
    Cross,
    /// <summary>Plus marker.</summary>
    Plus,
    /// <summary>Star marker.</summary>
    Star
}

/// <summary>
/// Registry for custom renderers.
/// </summary>
public class RendererRegistry
{
    private IMarkerRenderer? _markerRenderer;
    private ILineRenderer? _lineRenderer;
    private IAreaRenderer? _areaRenderer;
    private IBarRenderer? _barRenderer;
    private IGridRenderer? _gridRenderer;
    private IAxisRenderer? _axisRenderer;
    private ILegendRenderer? _legendRenderer;

    /// <summary>
    /// Registers a custom marker renderer.
    /// </summary>
    public void RegisterMarkerRenderer(IMarkerRenderer renderer) => _markerRenderer = renderer;

    /// <summary>
    /// Registers a custom line renderer.
    /// </summary>
    public void RegisterLineRenderer(ILineRenderer renderer) => _lineRenderer = renderer;

    /// <summary>
    /// Registers a custom area renderer.
    /// </summary>
    public void RegisterAreaRenderer(IAreaRenderer renderer) => _areaRenderer = renderer;

    /// <summary>
    /// Registers a custom bar renderer.
    /// </summary>
    public void RegisterBarRenderer(IBarRenderer renderer) => _barRenderer = renderer;

    /// <summary>
    /// Registers a custom grid renderer.
    /// </summary>
    public void RegisterGridRenderer(IGridRenderer renderer) => _gridRenderer = renderer;

    /// <summary>
    /// Registers a custom axis renderer.
    /// </summary>
    public void RegisterAxisRenderer(IAxisRenderer renderer) => _axisRenderer = renderer;

    /// <summary>
    /// Registers a custom legend renderer.
    /// </summary>
    public void RegisterLegendRenderer(ILegendRenderer renderer) => _legendRenderer = renderer;

    /// <summary>
    /// Gets the registered marker renderer, or null if using default.
    /// </summary>
    public IMarkerRenderer? GetMarkerRenderer() => _markerRenderer;

    /// <summary>
    /// Gets the registered line renderer, or null if using default.
    /// </summary>
    public ILineRenderer? GetLineRenderer() => _lineRenderer;

    /// <summary>
    /// Gets the registered area renderer, or null if using default.
    /// </summary>
    public IAreaRenderer? GetAreaRenderer() => _areaRenderer;

    /// <summary>
    /// Gets the registered bar renderer, or null if using default.
    /// </summary>
    public IBarRenderer? GetBarRenderer() => _barRenderer;

    /// <summary>
    /// Gets the registered grid renderer, or null if using default.
    /// </summary>
    public IGridRenderer? GetGridRenderer() => _gridRenderer;

    /// <summary>
    /// Gets the registered axis renderer, or null if using default.
    /// </summary>
    public IAxisRenderer? GetAxisRenderer() => _axisRenderer;

    /// <summary>
    /// Gets the registered legend renderer, or null if using default.
    /// </summary>
    public ILegendRenderer? GetLegendRenderer() => _legendRenderer;

    /// <summary>
    /// Clears all registered custom renderers.
    /// </summary>
    public void ClearAll()
    {
        _markerRenderer = null;
        _lineRenderer = null;
        _areaRenderer = null;
        _barRenderer = null;
        _gridRenderer = null;
        _axisRenderer = null;
        _legendRenderer = null;
    }
}

/// <summary>
/// Example: Default circle marker renderer.
/// </summary>
public class DefaultMarkerRenderer : IMarkerRenderer
{
    public void RenderMarker(RenderContext context, SKPoint position, float size, SKColor color)
    {
        using var paint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        context.Canvas.DrawCircle(position, size / 2, paint);

        // Draw border
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        paint.Color = color.WithAlpha((byte)(color.Alpha * 0.7));
        context.Canvas.DrawCircle(position, size / 2, paint);
    }
}

/// <summary>
/// Example: Square marker renderer.
/// </summary>
public class SquareMarkerRenderer : IMarkerRenderer
{
    public void RenderMarker(RenderContext context, SKPoint position, float size, SKColor color)
    {
        using var paint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var halfSize = size / 2;
        var rect = new SKRect(
            position.X - halfSize,
            position.Y - halfSize,
            position.X + halfSize,
            position.Y + halfSize
        );

        context.Canvas.DrawRect(rect, paint);

        // Draw border
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        paint.Color = color.WithAlpha((byte)(color.Alpha * 0.7));
        context.Canvas.DrawRect(rect, paint);
    }
}

/// <summary>
/// Example: Diamond marker renderer.
/// </summary>
public class DiamondMarkerRenderer : IMarkerRenderer
{
    public void RenderMarker(RenderContext context, SKPoint position, float size, SKColor color)
    {
        using var paint = new SKPaint
        {
            Color = color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        var halfSize = size / 2;
        using var path = new SKPath();
        path.MoveTo(position.X, position.Y - halfSize);  // Top
        path.LineTo(position.X + halfSize, position.Y);  // Right
        path.LineTo(position.X, position.Y + halfSize);  // Bottom
        path.LineTo(position.X - halfSize, position.Y);  // Left
        path.Close();

        context.Canvas.DrawPath(path, paint);

        // Draw border
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 1;
        paint.Color = color.WithAlpha((byte)(color.Alpha * 0.7));
        context.Canvas.DrawPath(path, paint);
    }
}
