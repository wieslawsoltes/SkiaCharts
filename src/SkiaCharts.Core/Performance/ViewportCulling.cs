using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides viewport culling functionality to render only visible data points.
/// This significantly improves performance when dealing with large datasets.
/// </summary>
public static class ViewportCulling
{
    /// <summary>
    /// Filters a data series to include only points visible within the specified viewport range.
    /// Includes one point before and after the visible range for smooth edge rendering.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="series">The data series to filter.</param>
    /// <param name="minX">The minimum X value of the viewport.</param>
    /// <param name="maxX">The maximum X value of the viewport.</param>
    /// <returns>An enumerable of visible data points with one point padding on each side.</returns>
    public static IEnumerable<T> GetVisiblePoints<T>(IEnumerable<T> series, double minX, double maxX)
        where T : IDataPoint
    {
        T? previousPoint = default;
        bool foundFirst = false;
        bool addedLastPoint = false;

        foreach (var point in series)
        {
            if (point.X >= minX && point.X <= maxX)
            {
                // Add the previous point once when we find the first visible point
                if (!foundFirst && previousPoint != null)
                {
                    yield return previousPoint;
                }
                foundFirst = true;
                yield return point;
            }
            else if (foundFirst && !addedLastPoint)
            {
                // Add one point after the visible range for smooth edge rendering
                yield return point;
                addedLastPoint = true;
                yield break; // No need to continue
            }

            previousPoint = point;
        }
    }

    /// <summary>
    /// Filters a data series to include only points visible within the specified viewport range.
    /// Optimized for IReadOnlyList with binary search for faster access.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="series">The data series to filter.</param>
    /// <param name="minX">The minimum X value of the viewport.</param>
    /// <param name="maxX">The maximum X value of the viewport.</param>
    /// <returns>An enumerable of visible data points with one point padding on each side.</returns>
    public static IEnumerable<T> GetVisiblePointsOptimized<T>(IReadOnlyList<T> series, double minX, double maxX)
        where T : IDataPoint
    {
        if (series.Count == 0)
            yield break;

        // Binary search for the start index
        int startIndex = BinarySearchFloor(series, minX);

        // Include one point before for smooth edge rendering
        if (startIndex > 0)
            startIndex--;

        // Iterate from start index until we exceed maxX
        for (int i = startIndex; i < series.Count; i++)
        {
            var point = series[i];
            yield return point;

            // Include one point after the visible range for smooth edge rendering
            if (point.X > maxX)
                yield break;
        }
    }

    /// <summary>
    /// Performs binary search to find the index of the largest element less than or equal to the target.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="series">The sorted data series.</param>
    /// <param name="targetX">The target X value to search for.</param>
    /// <returns>The index of the largest element less than or equal to targetX, or 0 if all elements are greater.</returns>
    private static int BinarySearchFloor<T>(IReadOnlyList<T> series, double targetX) where T : IDataPoint
    {
        int left = 0;
        int right = series.Count - 1;
        int result = 0;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            double midValue = series[mid].X;

            if (midValue <= targetX)
            {
                result = mid;
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return result;
    }

    /// <summary>
    /// Estimates the number of visible points without creating a new collection.
    /// Useful for pre-allocating collections.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="series">The data series.</param>
    /// <param name="minX">The minimum X value of the viewport.</param>
    /// <param name="maxX">The maximum X value of the viewport.</param>
    /// <returns>The estimated number of visible points.</returns>
    public static int EstimateVisibleCount<T>(IReadOnlyList<T> series, double minX, double maxX)
        where T : IDataPoint
    {
        if (series.Count == 0)
            return 0;

        // If the entire series is outside the viewport
        if (series[series.Count - 1].X < minX || series[0].X > maxX)
            return 0;

        // Quick estimate using binary search
        int startIndex = BinarySearchFloor(series, minX);
        int endIndex = BinarySearchFloor(series, maxX);

        // Add padding for edge points
        return Math.Min(series.Count, endIndex - startIndex + 3);
    }
}
