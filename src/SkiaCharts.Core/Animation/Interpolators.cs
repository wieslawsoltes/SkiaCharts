using SkiaCharts.Core.Utilities;
using SkiaSharp;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Provides interpolation functions for common types.
/// </summary>
public static class Interpolators
{
    /// <summary>
    /// Interpolates between two double values.
    /// </summary>
    public static double Double(double from, double to, double progress)
    {
        return MathHelper.Lerp(from, to, progress);
    }

    /// <summary>
    /// Interpolates between two float values.
    /// </summary>
    public static float Float(float from, float to, double progress)
    {
        return (float)MathHelper.Lerp(from, to, progress);
    }

    /// <summary>
    /// Interpolates between two integer values.
    /// </summary>
    public static int Int(int from, int to, double progress)
    {
        return (int)Math.Round(MathHelper.Lerp(from, to, progress));
    }

    /// <summary>
    /// Interpolates between two colors (RGB).
    /// </summary>
    public static SKColor Color(SKColor from, SKColor to, double progress)
    {
        var r = (byte)MathHelper.Lerp(from.Red, to.Red, progress);
        var g = (byte)MathHelper.Lerp(from.Green, to.Green, progress);
        var b = (byte)MathHelper.Lerp(from.Blue, to.Blue, progress);
        var a = (byte)MathHelper.Lerp(from.Alpha, to.Alpha, progress);
        return new SKColor(r, g, b, a);
    }

    /// <summary>
    /// Interpolates between two points.
    /// </summary>
    public static SKPoint Point(SKPoint from, SKPoint to, double progress)
    {
        var x = (float)MathHelper.Lerp(from.X, to.X, progress);
        var y = (float)MathHelper.Lerp(from.Y, to.Y, progress);
        return new SKPoint(x, y);
    }

    /// <summary>
    /// Interpolates between two rectangles.
    /// </summary>
    public static SKRect Rect(SKRect from, SKRect to, double progress)
    {
        var left = (float)MathHelper.Lerp(from.Left, to.Left, progress);
        var top = (float)MathHelper.Lerp(from.Top, to.Top, progress);
        var right = (float)MathHelper.Lerp(from.Right, to.Right, progress);
        var bottom = (float)MathHelper.Lerp(from.Bottom, to.Bottom, progress);
        return new SKRect(left, top, right, bottom);
    }

    /// <summary>
    /// Interpolates between two sizes.
    /// </summary>
    public static SKSize Size(SKSize from, SKSize to, double progress)
    {
        var width = (float)MathHelper.Lerp(from.Width, to.Width, progress);
        var height = (float)MathHelper.Lerp(from.Height, to.Height, progress);
        return new SKSize(width, height);
    }
}
