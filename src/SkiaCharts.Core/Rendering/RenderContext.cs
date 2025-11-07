using SkiaSharp;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Default implementation of <see cref="IRenderContext"/> that wraps a SkiaSharp canvas.
/// </summary>
public class RenderContext : IRenderContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderContext"/> class.
    /// </summary>
    /// <param name="canvas">The SkiaSharp canvas.</param>
    /// <param name="width">The width of the rendering surface.</param>
    /// <param name="height">The height of the rendering surface.</param>
    public RenderContext(SKCanvas canvas, float width, float height)
    {
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        Width = width;
        Height = height;
        Bounds = new SKRect(0, 0, width, height);
    }

    /// <inheritdoc/>
    public SKCanvas Canvas { get; }

    /// <inheritdoc/>
    public float Width { get; }

    /// <inheritdoc/>
    public float Height { get; }

    /// <inheritdoc/>
    public SKRect Bounds { get; }

    /// <inheritdoc/>
    public int Save() => Canvas.Save();

    /// <inheritdoc/>
    public void Restore() => Canvas.Restore();

    /// <inheritdoc/>
    public void ClipRect(SKRect rect) => Canvas.ClipRect(rect);

    /// <inheritdoc/>
    public void Translate(float dx, float dy) => Canvas.Translate(dx, dy);

    /// <inheritdoc/>
    public void Scale(float sx, float sy) => Canvas.Scale(sx, sy);

    /// <inheritdoc/>
    public void Clear(SKColor color) => Canvas.Clear(color);

    /// <inheritdoc/>
    public void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint) =>
        Canvas.DrawLine(x0, y0, x1, y1, paint);

    /// <inheritdoc/>
    public void DrawPath(SKPath path, SKPaint paint) => Canvas.DrawPath(path, paint);

    /// <inheritdoc/>
    public void DrawRect(SKRect rect, SKPaint paint) => Canvas.DrawRect(rect, paint);

    /// <inheritdoc/>
    public void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint) =>
        Canvas.DrawRoundRect(rect, rx, ry, paint);

    /// <inheritdoc/>
    public void DrawCircle(float cx, float cy, float radius, SKPaint paint) =>
        Canvas.DrawCircle(cx, cy, radius, paint);

    /// <inheritdoc/>
    public void DrawText(string text, float x, float y, SKPaint paint)
    {
        using var font = new SKFont
        {
            Size = paint.TextSize,
            Typeface = paint.Typeface ?? SKTypeface.Default
        };
        Canvas.DrawText(text, x, y, font, paint);
    }

    /// <inheritdoc/>
    public SKRect MeasureText(string text, SKPaint paint)
    {
        using var font = new SKFont
        {
            Size = paint.TextSize,
            Typeface = paint.Typeface ?? SKTypeface.Default
        };
        font.MeasureText(text, out var bounds);
        return bounds;
    }
}
