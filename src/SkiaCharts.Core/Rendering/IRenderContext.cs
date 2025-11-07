using SkiaSharp;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Provides an abstraction over the SkiaSharp canvas for rendering operations.
/// </summary>
public interface IRenderContext
{
    /// <summary>
    /// Gets the underlying SkiaSharp canvas.
    /// </summary>
    SKCanvas Canvas { get; }

    /// <summary>
    /// Gets the width of the rendering surface in pixels.
    /// </summary>
    float Width { get; }

    /// <summary>
    /// Gets the height of the rendering surface in pixels.
    /// </summary>
    float Height { get; }

    /// <summary>
    /// Gets the bounds of the rendering surface.
    /// </summary>
    SKRect Bounds { get; }

    /// <summary>
    /// Saves the current canvas state.
    /// </summary>
    /// <returns>A count representing the saved state.</returns>
    int Save();

    /// <summary>
    /// Restores the canvas to a previously saved state.
    /// </summary>
    void Restore();

    /// <summary>
    /// Clips the rendering region to the specified rectangle.
    /// </summary>
    /// <param name="rect">The clipping rectangle.</param>
    void ClipRect(SKRect rect);

    /// <summary>
    /// Translates the canvas coordinate system.
    /// </summary>
    /// <param name="dx">The horizontal translation.</param>
    /// <param name="dy">The vertical translation.</param>
    void Translate(float dx, float dy);

    /// <summary>
    /// Scales the canvas coordinate system.
    /// </summary>
    /// <param name="sx">The horizontal scale factor.</param>
    /// <param name="sy">The vertical scale factor.</param>
    void Scale(float sx, float sy);

    /// <summary>
    /// Clears the entire surface with the specified color.
    /// </summary>
    /// <param name="color">The color to clear with.</param>
    void Clear(SKColor color);

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    /// <param name="x0">The starting X coordinate.</param>
    /// <param name="y0">The starting Y coordinate.</param>
    /// <param name="x1">The ending X coordinate.</param>
    /// <param name="y1">The ending Y coordinate.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint);

    /// <summary>
    /// Draws a path.
    /// </summary>
    /// <param name="path">The path to draw.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawPath(SKPath path, SKPaint paint);

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawRect(SKRect rect, SKPaint paint);

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="rx">The X radius of the rounded corners.</param>
    /// <param name="ry">The Y radius of the rounded corners.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint);

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="cx">The X coordinate of the center.</param>
    /// <param name="cy">The Y coordinate of the center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawCircle(float cx, float cy, float radius, SKPaint paint);

    /// <summary>
    /// Draws text at the specified position.
    /// </summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="paint">The paint to use for drawing.</param>
    void DrawText(string text, float x, float y, SKPaint paint);

    /// <summary>
    /// Measures the bounds of the specified text.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="paint">The paint to use for measurement.</param>
    /// <returns>The bounds of the text.</returns>
    SKRect MeasureText(string text, SKPaint paint);
}
