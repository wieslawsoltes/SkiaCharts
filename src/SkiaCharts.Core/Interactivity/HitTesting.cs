using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Provides hit testing utilities for interactive elements.
/// </summary>
public static class HitTesting
{
    /// <summary>
    /// Tests if a point is within a rectangle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="rect">The rectangle.</param>
    /// <returns>True if the point is within the rectangle.</returns>
    public static bool HitTestRect(SKPoint point, SKRect rect)
    {
        return rect.Contains(point.X, point.Y);
    }

    /// <summary>
    /// Tests if a point is within a circle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="center">The circle center.</param>
    /// <param name="radius">The circle radius.</param>
    /// <returns>True if the point is within the circle.</returns>
    public static bool HitTestCircle(SKPoint point, SKPoint center, float radius)
    {
        var distance = SKPoint.Distance(point, center);
        return distance <= radius;
    }

    /// <summary>
    /// Tests if a point is near a line segment.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="lineStart">The line start point.</param>
    /// <param name="lineEnd">The line end point.</param>
    /// <param name="threshold">The distance threshold.</param>
    /// <returns>True if the point is near the line.</returns>
    public static bool HitTestLine(SKPoint point, SKPoint lineStart, SKPoint lineEnd, float threshold)
    {
        var distance = PointToLineDistance(point, lineStart, lineEnd);
        return distance <= threshold;
    }

    /// <summary>
    /// Tests if a point is within a path.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="path">The path.</param>
    /// <returns>True if the point is within the path.</returns>
    public static bool HitTestPath(SKPoint point, SKPath path)
    {
        return path.Contains(point.X, point.Y);
    }

    /// <summary>
    /// Calculates the distance from a point to a line segment.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="lineStart">The line start point.</param>
    /// <param name="lineEnd">The line end point.</param>
    /// <returns>The distance.</returns>
    public static float PointToLineDistance(SKPoint point, SKPoint lineStart, SKPoint lineEnd)
    {
        var lineLength = SKPoint.Distance(lineStart, lineEnd);
        if (lineLength == 0)
            return SKPoint.Distance(point, lineStart);

        var t = Math.Max(0, Math.Min(1,
            ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) +
             (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / (lineLength * lineLength)));

        var projection = new SKPoint(
            lineStart.X + t * (lineEnd.X - lineStart.X),
            lineStart.Y + t * (lineEnd.Y - lineStart.Y)
        );

        return SKPoint.Distance(point, projection);
    }

    /// <summary>
    /// Expands a rectangle by a margin (for touch-optimized hit areas).
    /// </summary>
    /// <param name="rect">The rectangle to expand.</param>
    /// <param name="margin">The margin to add on all sides.</param>
    /// <returns>The expanded rectangle.</returns>
    public static SKRect ExpandRect(SKRect rect, float margin)
    {
        return new SKRect(
            rect.Left - margin,
            rect.Top - margin,
            rect.Right + margin,
            rect.Bottom + margin
        );
    }

    /// <summary>
    /// Gets the minimum touch target size for mobile devices.
    /// </summary>
    /// <remarks>
    /// iOS Human Interface Guidelines recommend 44x44 points.
    /// Material Design recommends 48x48 dp.
    /// </remarks>
    public const float MinimumTouchTargetSize = 44.0f;

    /// <summary>
    /// Ensures a hit area is at least the minimum touch target size.
    /// </summary>
    /// <param name="center">The center of the hit area.</param>
    /// <param name="currentSize">The current size.</param>
    /// <returns>A rectangle with minimum touch target size.</returns>
    public static SKRect EnsureMinimumTouchTarget(SKPoint center, float currentSize)
    {
        var size = Math.Max(currentSize, MinimumTouchTargetSize);
        var halfSize = size / 2;

        return new SKRect(
            center.X - halfSize,
            center.Y - halfSize,
            center.X + halfSize,
            center.Y + halfSize
        );
    }
}

/// <summary>
/// Represents a hit test result.
/// </summary>
public class HitTestResult
{
    /// <summary>
    /// Gets whether the hit test succeeded.
    /// </summary>
    public bool IsHit { get; init; }

    /// <summary>
    /// Gets the hit object.
    /// </summary>
    public object? HitObject { get; init; }

    /// <summary>
    /// Gets the hit point.
    /// </summary>
    public SKPoint HitPoint { get; init; }

    /// <summary>
    /// Gets the distance to the hit object.
    /// </summary>
    public float Distance { get; init; }

    /// <summary>
    /// Gets additional data about the hit.
    /// </summary>
    public Dictionary<string, object> Data { get; init; } = new();

    /// <summary>
    /// Creates a successful hit test result.
    /// </summary>
    public static HitTestResult Hit(object hitObject, SKPoint hitPoint, float distance = 0)
    {
        return new HitTestResult
        {
            IsHit = true,
            HitObject = hitObject,
            HitPoint = hitPoint,
            Distance = distance
        };
    }

    /// <summary>
    /// Creates a failed hit test result.
    /// </summary>
    public static HitTestResult Miss()
    {
        return new HitTestResult { IsHit = false };
    }
}

/// <summary>
/// Interface for objects that support hit testing.
/// </summary>
public interface IHitTestable
{
    /// <summary>
    /// Performs hit testing at the specified point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>The hit test result.</returns>
    HitTestResult HitTest(SKPoint point);

    /// <summary>
    /// Gets the bounds of this object.
    /// </summary>
    SKRect Bounds { get; }

    /// <summary>
    /// Gets whether this object is hit testable.
    /// </summary>
    bool IsHitTestVisible { get; }
}

/// <summary>
/// Manages touch-optimized hit areas.
/// </summary>
public class TouchHitAreaManager
{
    private readonly Dictionary<object, SKRect> _hitAreas;

    /// <summary>
    /// Initializes a new instance of the <see cref="TouchHitAreaManager"/> class.
    /// </summary>
    public TouchHitAreaManager()
    {
        _hitAreas = new Dictionary<object, SKRect>();
    }

    /// <summary>
    /// Registers a hit area for an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="hitArea">The hit area.</param>
    public void RegisterHitArea(object obj, SKRect hitArea)
    {
        // Ensure minimum touch target size
        var expandedArea = EnsureMinimumSize(hitArea);
        _hitAreas[obj] = expandedArea;
    }

    /// <summary>
    /// Registers a circular hit area for an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="center">The center point.</param>
    /// <param name="radius">The radius.</param>
    public void RegisterCircularHitArea(object obj, SKPoint center, float radius)
    {
        var expandedRadius = Math.Max(radius, HitTesting.MinimumTouchTargetSize / 2);
        var hitArea = new SKRect(
            center.X - expandedRadius,
            center.Y - expandedRadius,
            center.X + expandedRadius,
            center.Y + expandedRadius
        );
        _hitAreas[obj] = hitArea;
    }

    /// <summary>
    /// Unregisters a hit area.
    /// </summary>
    /// <param name="obj">The object.</param>
    public void UnregisterHitArea(object obj)
    {
        _hitAreas.Remove(obj);
    }

    /// <summary>
    /// Performs hit testing at the specified point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>The hit object, or null if no hit.</returns>
    public object? HitTest(SKPoint point)
    {
        // Test in reverse order (top to bottom)
        foreach (var kvp in _hitAreas.Reverse())
        {
            if (HitTesting.HitTestRect(point, kvp.Value))
            {
                return kvp.Key;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all hit areas that intersect the specified point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>List of hit objects.</returns>
    public List<object> HitTestAll(SKPoint point)
    {
        var results = new List<object>();

        foreach (var kvp in _hitAreas.Reverse())
        {
            if (HitTesting.HitTestRect(point, kvp.Value))
            {
                results.Add(kvp.Key);
            }
        }

        return results;
    }

    /// <summary>
    /// Clears all registered hit areas.
    /// </summary>
    public void Clear()
    {
        _hitAreas.Clear();
    }

    /// <summary>
    /// Gets the number of registered hit areas.
    /// </summary>
    public int Count => _hitAreas.Count;

    private SKRect EnsureMinimumSize(SKRect rect)
    {
        var width = rect.Width;
        var height = rect.Height;

        if (width >= HitTesting.MinimumTouchTargetSize && height >= HitTesting.MinimumTouchTargetSize)
            return rect;

        var expandWidth = Math.Max(0, (HitTesting.MinimumTouchTargetSize - width) / 2);
        var expandHeight = Math.Max(0, (HitTesting.MinimumTouchTargetSize - height) / 2);

        return new SKRect(
            rect.Left - expandWidth,
            rect.Top - expandHeight,
            rect.Right + expandWidth,
            rect.Bottom + expandHeight
        );
    }
}
