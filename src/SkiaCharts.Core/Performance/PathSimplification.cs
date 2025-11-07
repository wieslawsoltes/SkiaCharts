using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides path simplification algorithms to reduce the number of points
/// while maintaining visual fidelity. Implements the Douglas-Peucker algorithm.
/// </summary>
public static class PathSimplification
{
    /// <summary>
    /// Simplifies a path using the Douglas-Peucker algorithm.
    /// This algorithm recursively subdivides the path and removes points
    /// that are within a specified tolerance distance from the line segment.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points representing the path.</param>
    /// <param name="tolerance">The perpendicular distance tolerance. Points within this distance are removed.</param>
    /// <returns>A simplified list of data points.</returns>
    public static List<T> DouglasPeucker<T>(IReadOnlyList<T> points, double tolerance) where T : IDataPoint
    {
        if (points.Count < 3)
            return new List<T>(points);

        var result = new List<T>();
        DouglasPeuckerRecursive(points, 0, points.Count - 1, tolerance, result);
        return result;
    }

    /// <summary>
    /// Recursive implementation of the Douglas-Peucker algorithm.
    /// </summary>
    private static void DouglasPeuckerRecursive<T>(
        IReadOnlyList<T> points,
        int startIndex,
        int endIndex,
        double tolerance,
        List<T> result) where T : IDataPoint
    {
        // Always add the start point
        if (result.Count == 0 || !EqualityComparer<T>.Default.Equals(result[result.Count - 1], points[startIndex]))
        {
            result.Add(points[startIndex]);
        }

        if (endIndex - startIndex <= 1)
        {
            // Add end point if it's different from start
            if (endIndex != startIndex)
            {
                result.Add(points[endIndex]);
            }
            return;
        }

        // Find the point with maximum distance from the line segment
        double maxDistance = 0;
        int maxIndex = startIndex;

        var startPoint = points[startIndex];
        var endPoint = points[endIndex];

        for (int i = startIndex + 1; i < endIndex; i++)
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
            }
        }

        // If max distance is greater than tolerance, recursively simplify
        if (maxDistance > tolerance)
        {
            // Recursively simplify the two segments
            DouglasPeuckerRecursive(points, startIndex, maxIndex, tolerance, result);
            // Remove the duplicate point that was added
            if (result.Count > 0 && EqualityComparer<T>.Default.Equals(result[result.Count - 1], points[maxIndex]))
            {
                result.RemoveAt(result.Count - 1);
            }
            DouglasPeuckerRecursive(points, maxIndex, endIndex, tolerance, result);
        }
        else
        {
            // All points in between can be removed, just add the end point
            result.Add(points[endIndex]);
        }
    }

    /// <summary>
    /// Calculates the perpendicular distance from a point to a line segment.
    /// </summary>
    /// <param name="px">Point X coordinate.</param>
    /// <param name="py">Point Y coordinate.</param>
    /// <param name="x1">Line start X coordinate.</param>
    /// <param name="y1">Line start Y coordinate.</param>
    /// <param name="x2">Line end X coordinate.</param>
    /// <param name="y2">Line end Y coordinate.</param>
    /// <returns>The perpendicular distance.</returns>
    private static double PerpendicularDistance(
        double px, double py,
        double x1, double y1,
        double x2, double y2)
    {
        double dx = x2 - x1;
        double dy = y2 - y1;

        if (dx == 0 && dy == 0)
        {
            // The line segment is actually a point
            dx = px - x1;
            dy = py - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Calculate perpendicular distance using cross product
        double numerator = Math.Abs(dy * px - dx * py + x2 * y1 - y2 * x1);
        double denominator = Math.Sqrt(dx * dx + dy * dy);

        return numerator / denominator;
    }

    /// <summary>
    /// Simplifies a path using the Ramer-Douglas-Peucker algorithm with screen space tolerance.
    /// Automatically calculates tolerance based on screen pixel dimensions.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points representing the path.</param>
    /// <param name="minPixelDistance">Minimum pixel distance between points (default: 1.0).</param>
    /// <param name="dataToScreenScaleX">Scale factor from data space to screen space X.</param>
    /// <param name="dataToScreenScaleY">Scale factor from data space to screen space Y.</param>
    /// <returns>A simplified list of data points.</returns>
    public static List<T> SimplifyScreenSpace<T>(
        IReadOnlyList<T> points,
        double minPixelDistance,
        double dataToScreenScaleX,
        double dataToScreenScaleY) where T : IDataPoint
    {
        if (points.Count < 3)
            return new List<T>(points);

        // Calculate tolerance in data space
        // Use the smaller scale factor to be conservative
        double scale = Math.Min(Math.Abs(dataToScreenScaleX), Math.Abs(dataToScreenScaleY));
        double tolerance = minPixelDistance / scale;

        return DouglasPeucker(points, tolerance);
    }

    /// <summary>
    /// Calculates the reduction ratio achieved by simplification.
    /// </summary>
    /// <param name="originalCount">Original number of points.</param>
    /// <param name="simplifiedCount">Number of points after simplification.</param>
    /// <returns>Reduction ratio (0.0 to 1.0, where 1.0 means 100% reduction).</returns>
    public static double CalculateReductionRatio(int originalCount, int simplifiedCount)
    {
        if (originalCount == 0)
            return 0.0;

        return 1.0 - ((double)simplifiedCount / originalCount);
    }

    /// <summary>
    /// Estimates appropriate tolerance based on data range and target point count.
    /// </summary>
    /// <param name="dataRangeY">The Y range of the data.</param>
    /// <param name="currentPointCount">Current number of points.</param>
    /// <param name="targetPointCount">Desired number of points after simplification.</param>
    /// <returns>Suggested tolerance value.</returns>
    public static double EstimateTolerance(double dataRangeY, int currentPointCount, int targetPointCount)
    {
        if (currentPointCount <= targetPointCount || currentPointCount == 0)
            return 0.0;

        // Estimate reduction needed
        double reductionFactor = (double)targetPointCount / currentPointCount;

        // Start with a small fraction of the data range
        // and adjust based on how much reduction we need
        double baseTolerance = dataRangeY * 0.001; // 0.1% of range

        // Increase tolerance for more aggressive reduction
        double aggressiveness = Math.Log(1.0 / reductionFactor);
        return baseTolerance * aggressiveness;
    }
}
