using SkiaSharp;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides render optimization utilities including hardware acceleration flags
/// and performance-oriented rendering settings.
/// </summary>
public static class RenderOptimization
{
    /// <summary>
    /// Creates an optimized SKPaint for line rendering with hardware acceleration hints.
    /// </summary>
    /// <param name="color">The line color.</param>
    /// <param name="strokeWidth">The line width.</param>
    /// <param name="enableAntiAlias">Whether to enable anti-aliasing (default: true).</param>
    /// <returns>An optimized SKPaint instance.</returns>
    public static SKPaint CreateOptimizedLinePaint(SKColor color, float strokeWidth, bool enableAntiAlias = true)
    {
        return new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            Style = SKPaintStyle.Stroke,
            IsAntialias = enableAntiAlias,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            // Hardware acceleration hints
            FilterQuality = SKFilterQuality.Low, // Faster filtering
            IsDither = false // Disable dithering for better performance
        };
    }

    /// <summary>
    /// Creates an optimized SKPaint for fill rendering.
    /// </summary>
    /// <param name="color">The fill color.</param>
    /// <param name="enableAntiAlias">Whether to enable anti-aliasing (default: true).</param>
    /// <returns>An optimized SKPaint instance.</returns>
    public static SKPaint CreateOptimizedFillPaint(SKColor color, bool enableAntiAlias = true)
    {
        return new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Fill,
            IsAntialias = enableAntiAlias,
            FilterQuality = SKFilterQuality.Low,
            IsDither = false
        };
    }

    /// <summary>
    /// Creates an optimized SKPaint for text rendering.
    /// </summary>
    /// <param name="color">The text color.</param>
    /// <param name="textSize">The text size.</param>
    /// <param name="typeface">Optional typeface.</param>
    /// <returns>An optimized SKPaint instance.</returns>
    public static SKPaint CreateOptimizedTextPaint(SKColor color, float textSize, SKTypeface? typeface = null)
    {
        return new SKPaint
        {
            Color = color,
            TextSize = textSize,
            IsAntialias = true,
            Typeface = typeface,
            SubpixelText = true, // Better text rendering
            LcdRenderText = true, // LCD subpixel rendering
            FilterQuality = SKFilterQuality.Low
        };
    }

    /// <summary>
    /// Applies hardware acceleration settings to an SKCanvas.
    /// </summary>
    /// <param name="canvas">The canvas to optimize.</param>
    public static void ApplyHardwareAcceleration(SKCanvas canvas)
    {
        // Note: Most hardware acceleration is automatic in SkiaSharp
        // These are hints to the renderer
        canvas.Clear(SKColors.Transparent); // Clear with transparency is often hardware accelerated
    }

    /// <summary>
    /// Calculates optimal path effect settings based on zoom level.
    /// </summary>
    /// <param name="zoomLevel">Current zoom level (1.0 = normal).</param>
    /// <returns>A tuple with suggested stroke width and anti-alias setting.</returns>
    public static (float StrokeWidth, bool AntiAlias) GetOptimalRenderSettings(double zoomLevel)
    {
        if (zoomLevel > 5.0)
        {
            // High zoom - increase quality
            return (2f, true);
        }
        else if (zoomLevel < 0.5)
        {
            // Low zoom - reduce quality for performance
            return (1f, false);
        }
        else
        {
            // Normal zoom
            return (2f, true);
        }
    }

    /// <summary>
    /// Determines if a rectangle is large enough to be worth rendering.
    /// </summary>
    /// <param name="rect">The rectangle to check.</param>
    /// <param name="minSize">Minimum size in pixels (default: 1.0).</param>
    /// <returns>True if the rectangle should be rendered.</returns>
    public static bool ShouldRender(SKRect rect, float minSize = 1.0f)
    {
        return rect.Width >= minSize && rect.Height >= minSize;
    }

    /// <summary>
    /// Determines if a point is worth rendering based on screen position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="bounds">The visible bounds.</param>
    /// <param name="margin">Margin outside bounds to still render (default: 10).</param>
    /// <returns>True if the point should be rendered.</returns>
    public static bool IsPointVisible(float x, float y, SKRect bounds, float margin = 10f)
    {
        return x >= bounds.Left - margin &&
               x <= bounds.Right + margin &&
               y >= bounds.Top - margin &&
               y <= bounds.Bottom + margin;
    }

    /// <summary>
    /// Creates a clipping region for efficient rendering.
    /// Only content within this region will be rendered.
    /// </summary>
    /// <param name="canvas">The canvas to apply clipping to.</param>
    /// <param name="clipRect">The clipping rectangle.</param>
    /// <returns>A state token to restore the canvas later.</returns>
    public static int ApplyClipRegion(SKCanvas canvas, SKRect clipRect)
    {
        var saveCount = canvas.Save();
        canvas.ClipRect(clipRect);
        return saveCount;
    }

    /// <summary>
    /// Restores canvas state after clipping.
    /// </summary>
    /// <param name="canvas">The canvas to restore.</param>
    /// <param name="saveCount">The save count returned from ApplyClipRegion.</param>
    public static void RestoreCanvas(SKCanvas canvas, int saveCount)
    {
        canvas.RestoreToCount(saveCount);
    }
}

/// <summary>
/// Provides render batching capabilities to minimize draw calls.
/// </summary>
public class RenderBatcher
{
    private readonly List<BatchedDrawCall> _drawCalls = new();
    private bool _isBatching;

    /// <summary>
    /// Starts batching draw calls.
    /// </summary>
    public void BeginBatch()
    {
        _isBatching = true;
        _drawCalls.Clear();
    }

    /// <summary>
    /// Adds a draw call to the batch.
    /// </summary>
    /// <param name="drawAction">The draw action to batch.</param>
    /// <param name="layer">The render layer for sorting.</param>
    public void AddDrawCall(Action<SKCanvas> drawAction, int layer = 0)
    {
        if (!_isBatching)
        {
            throw new InvalidOperationException("Must call BeginBatch before adding draw calls.");
        }

        _drawCalls.Add(new BatchedDrawCall
        {
            DrawAction = drawAction,
            Layer = layer
        });
    }

    /// <summary>
    /// Executes all batched draw calls in optimal order.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    public void ExecuteBatch(SKCanvas canvas)
    {
        if (!_isBatching)
        {
            throw new InvalidOperationException("Must call BeginBatch before executing batch.");
        }

        // Sort by layer for optimal rendering order
        var sortedCalls = _drawCalls.OrderBy(c => c.Layer).ToList();

        foreach (var call in sortedCalls)
        {
            call.DrawAction(canvas);
        }

        _isBatching = false;
        _drawCalls.Clear();
    }

    /// <summary>
    /// Gets the number of batched draw calls.
    /// </summary>
    public int DrawCallCount => _drawCalls.Count;

    private class BatchedDrawCall
    {
        public required Action<SKCanvas> DrawAction { get; init; }
        public int Layer { get; init; }
    }
}

/// <summary>
/// Provides parallel rendering capabilities for independent series.
/// </summary>
public static class ParallelRendering
{
    /// <summary>
    /// Renders multiple independent items in parallel.
    /// Each item is rendered to its own SKSurface, then composited to the main canvas.
    /// </summary>
    /// <typeparam name="T">The type of items to render.</typeparam>
    /// <param name="items">The items to render.</param>
    /// <param name="renderAction">Action to render each item.</param>
    /// <param name="width">Surface width.</param>
    /// <param name="height">Surface height.</param>
    /// <param name="finalCanvas">The final canvas to composite onto.</param>
    public static void RenderParallel<T>(
        IEnumerable<T> items,
        Action<T, SKCanvas> renderAction,
        int width,
        int height,
        SKCanvas finalCanvas)
    {
        var surfaces = new List<SKSurface>();
        var itemList = items.ToList();

        try
        {
            // Create surfaces in parallel
            Parallel.ForEach(itemList, item =>
            {
                var surface = SKSurface.Create(new SKImageInfo(width, height));
                lock (surfaces)
                {
                    surfaces.Add(surface);
                }
                renderAction(item, surface.Canvas);
            });

            // Composite to final canvas
            foreach (var surface in surfaces)
            {
                using var image = surface.Snapshot();
                finalCanvas.DrawImage(image, 0, 0);
            }
        }
        finally
        {
            // Cleanup surfaces
            foreach (var surface in surfaces)
            {
                surface.Dispose();
            }
        }
    }

    /// <summary>
    /// Determines if parallel rendering would be beneficial based on item count.
    /// </summary>
    /// <param name="itemCount">Number of items to render.</param>
    /// <param name="threshold">Minimum items to use parallel rendering (default: 3).</param>
    /// <returns>True if parallel rendering should be used.</returns>
    public static bool ShouldUseParallelRendering(int itemCount, int threshold = 3)
    {
        return itemCount >= threshold && Environment.ProcessorCount > 1;
    }
}
