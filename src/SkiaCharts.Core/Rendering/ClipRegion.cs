using SkiaSharp;

namespace SkiaCharts.Core.Rendering;

/// <summary>
/// Represents a rectangular clipping region for efficient viewport culling.
/// </summary>
public readonly struct ClipRegion
{
    /// <summary>
    /// Gets the X coordinate of the clip region.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Gets the Y coordinate of the clip region.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Gets the width of the clip region.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Gets the height of the clip region.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Gets the left edge of the clip region.
    /// </summary>
    public double Left => X;

    /// <summary>
    /// Gets the right edge of the clip region.
    /// </summary>
    public double Right => X + Width;

    /// <summary>
    /// Gets the top edge of the clip region.
    /// </summary>
    public double Top => Y;

    /// <summary>
    /// Gets the bottom edge of the clip region.
    /// </summary>
    public double Bottom => Y + Height;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClipRegion"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public ClipRegion(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a clip region from a rectangle.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <returns>A new clip region.</returns>
    public static ClipRegion FromRect(SKRect rect)
    {
        return new ClipRegion(rect.Left, rect.Top, rect.Width, rect.Height);
    }

    /// <summary>
    /// Converts this clip region to an SKRect.
    /// </summary>
    /// <returns>An SKRect.</returns>
    public SKRect ToSKRect()
    {
        return new SKRect((float)X, (float)Y, (float)(X + Width), (float)(Y + Height));
    }

    /// <summary>
    /// Determines if a point is within the clip region.
    /// </summary>
    /// <param name="x">The X coordinate of the point.</param>
    /// <param name="y">The Y coordinate of the point.</param>
    /// <returns>True if the point is inside the clip region.</returns>
    public bool Contains(double x, double y)
    {
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    /// <summary>
    /// Determines if a rectangle intersects with the clip region.
    /// </summary>
    /// <param name="x">The X coordinate of the rectangle.</param>
    /// <param name="y">The Y coordinate of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <returns>True if the rectangle intersects with the clip region.</returns>
    public bool Intersects(double x, double y, double width, double height)
    {
        return !(x + width < Left || x > Right || y + height < Top || y > Bottom);
    }

    /// <summary>
    /// Determines if another clip region intersects with this one.
    /// </summary>
    /// <param name="other">The other clip region.</param>
    /// <returns>True if the regions intersect.</returns>
    public bool Intersects(ClipRegion other)
    {
        return Intersects(other.X, other.Y, other.Width, other.Height);
    }

    /// <summary>
    /// Expands the clip region by a margin on all sides.
    /// </summary>
    /// <param name="margin">The margin to add.</param>
    /// <returns>A new expanded clip region.</returns>
    public ClipRegion Expand(double margin)
    {
        return new ClipRegion(
            X - margin,
            Y - margin,
            Width + 2 * margin,
            Height + 2 * margin
        );
    }

    /// <summary>
    /// Creates the intersection of two clip regions.
    /// </summary>
    /// <param name="other">The other clip region.</param>
    /// <returns>The intersection region, or an empty region if they don't intersect.</returns>
    public ClipRegion Intersect(ClipRegion other)
    {
        var left = Math.Max(Left, other.Left);
        var top = Math.Max(Top, other.Top);
        var right = Math.Min(Right, other.Right);
        var bottom = Math.Min(Bottom, other.Bottom);

        if (right <= left || bottom <= top)
        {
            return new ClipRegion(0, 0, 0, 0); // Empty region
        }

        return new ClipRegion(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Gets whether this clip region is empty.
    /// </summary>
    public bool IsEmpty => Width <= 0 || Height <= 0;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"ClipRegion(X={X:F1}, Y={Y:F1}, W={Width:F1}, H={Height:F1})";
    }
}
