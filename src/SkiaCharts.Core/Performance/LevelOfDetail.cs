using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides Level-of-Detail (LOD) functionality with adaptive point decimation.
/// Reduces the number of rendered points based on available screen pixels.
/// </summary>
public static class LevelOfDetail
{
    /// <summary>
    /// Decimates a data series to a target point count using adaptive sampling.
    /// Ensures important features are preserved while reducing overall point count.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to decimate.</param>
    /// <param name="targetCount">The target number of points after decimation.</param>
    /// <returns>A decimated list of data points.</returns>
    public static List<T> Decimate<T>(IReadOnlyList<T> points, int targetCount) where T : IDataPoint
    {
        if (points.Count <= targetCount || targetCount < 2)
            return new List<T>(points);

        var result = new List<T>(targetCount);

        // Always include first point
        result.Add(points[0]);

        // Calculate how many intermediate points we need (excluding first and last)
        int intermediateCount = targetCount - 2;
        double step = (double)(points.Count - 1) / (targetCount - 1);

        // Add intermediate points at calculated intervals
        for (int i = 1; i < targetCount - 1; i++)
        {
            int index = (int)Math.Round(i * step);
            result.Add(points[index]);
        }

        // Always include last point
        result.Add(points[points.Count - 1]);

        return result;
    }

    /// <summary>
    /// Decimates points based on available screen width.
    /// Calculates optimal point count to avoid rendering more points than pixels.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to decimate.</param>
    /// <param name="screenWidth">Available screen width in pixels.</param>
    /// <param name="maxPointsPerPixel">Maximum points to render per pixel (default: 2).</param>
    /// <returns>A decimated list of data points.</returns>
    public static List<T> DecimateByScreenWidth<T>(IReadOnlyList<T> points, int screenWidth, double maxPointsPerPixel = 2.0)
        where T : IDataPoint
    {
        int targetCount = (int)(screenWidth * maxPointsPerPixel);
        return Decimate(points, targetCount);
    }

    /// <summary>
    /// Adaptive decimation that preserves peaks and valleys.
    /// Uses a sliding window to identify significant points.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to decimate.</param>
    /// <param name="targetCount">The target number of points after decimation.</param>
    /// <returns>A decimated list of data points with preserved features.</returns>
    public static List<T> DecimatePreserveFeatures<T>(IReadOnlyList<T> points, int targetCount) where T : IDataPoint
    {
        if (points.Count <= targetCount || targetCount < 2)
            return new List<T>(points);

        var result = new List<T>(targetCount);

        // Always include first point
        result.Add(points[0]);

        int windowSize = Math.Max(3, points.Count / targetCount);
        int currentIndex = 0;

        while (result.Count < targetCount - 1 && currentIndex < points.Count - windowSize)
        {
            // Find the most significant point in the window
            var significantPoint = FindMostSignificantPoint(points, currentIndex, windowSize);
            result.Add(significantPoint.point);
            currentIndex = significantPoint.index + 1;
        }

        // Always include last point
        result.Add(points[points.Count - 1]);

        return result;
    }

    /// <summary>
    /// Finds the most significant point in a window (furthest from the line between window endpoints).
    /// </summary>
    private static (T point, int index) FindMostSignificantPoint<T>(IReadOnlyList<T> points, int startIndex, int windowSize)
        where T : IDataPoint
    {
        int endIndex = Math.Min(startIndex + windowSize, points.Count - 1);

        double maxDistance = 0;
        int maxIndex = startIndex;
        T maxPoint = points[startIndex];

        var startPoint = points[startIndex];
        var endPoint = points[endIndex];

        for (int i = startIndex; i <= endIndex; i++)
        {
            var point = points[i];
            double distance = PerpendicularDistance(
                point.X, point.Y,
                startPoint.X, startPoint.Y,
                endPoint.X, endPoint.Y
            );

            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
                maxPoint = point;
            }
        }

        return (maxPoint, maxIndex);
    }

    /// <summary>
    /// Calculates perpendicular distance from a point to a line.
    /// </summary>
    private static double PerpendicularDistance(
        double px, double py,
        double x1, double y1,
        double x2, double y2)
    {
        double dx = x2 - x1;
        double dy = y2 - y1;

        if (dx == 0 && dy == 0)
        {
            // The line is actually a point
            dx = px - x1;
            dy = py - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        double numerator = Math.Abs(dy * px - dx * py + x2 * y1 - y2 * x1);
        double denominator = Math.Sqrt(dx * dx + dy * dy);

        return numerator / denominator;
    }

    /// <summary>
    /// Determines the appropriate LOD level based on point count and screen space.
    /// </summary>
    /// <param name="pointCount">The total number of points.</param>
    /// <param name="screenWidth">Available screen width in pixels.</param>
    /// <returns>LOD level (0 = full detail, higher = more decimation).</returns>
    public static int CalculateLodLevel(int pointCount, int screenWidth)
    {
        double ratio = (double)pointCount / screenWidth;

        if (ratio <= 2) return 0;      // Full detail
        if (ratio <= 5) return 1;      // Light decimation
        if (ratio <= 10) return 2;     // Medium decimation
        if (ratio <= 20) return 3;     // Heavy decimation
        return 4;                       // Very heavy decimation
    }

    /// <summary>
    /// Gets the decimation factor for a given LOD level.
    /// </summary>
    /// <param name="lodLevel">The LOD level.</param>
    /// <returns>The decimation factor (points to keep per pixel).</returns>
    public static double GetDecimationFactor(int lodLevel)
    {
        return lodLevel switch
        {
            0 => double.MaxValue,  // No decimation
            1 => 5.0,
            2 => 3.0,
            3 => 2.0,
            4 => 1.5,
            _ => 1.0
        };
    }
}
