using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Implements the Largest Triangle Three Buckets (LTTB) downsampling algorithm.
/// LTTB is specifically designed for time-series data visualization, preserving
/// visual characteristics better than simple averaging or decimation.
///
/// Reference: Sveinn Steinarsson (2013) "Downsampling Time Series for Visual Representation"
/// </summary>
public static class LargestTriangleThreeBuckets
{
    /// <summary>
    /// Downsamples data using the LTTB algorithm.
    /// The algorithm divides data into buckets and selects the point that creates
    /// the largest triangle with points from adjacent buckets.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data points (must be sorted by X).</param>
    /// <param name="threshold">The desired number of output points.</param>
    /// <returns>Downsampled data points.</returns>
    public static List<DataPoint> Downsample<T>(IReadOnlyList<T> data, int threshold) where T : IDataPoint
    {
        if (data.Count <= threshold || threshold < 3)
        {
            return data.Select(p => new DataPoint(p.X, p.Y)).ToList();
        }

        var result = new List<DataPoint>(threshold);

        // Bucket size. Leave room for start and end data points
        double bucketSize = (double)(data.Count - 2) / (threshold - 2);

        // Always add the first point
        result.Add(new DataPoint(data[0].X, data[0].Y));

        int a = 0; // Initially a is the first point in the triangle

        for (int i = 0; i < threshold - 2; i++)
        {
            // Calculate point average for next bucket (containing c)
            double avgX = 0;
            double avgY = 0;

            int avgRangeStart = (int)(Math.Floor((i + 1) * bucketSize) + 1);
            int avgRangeEnd = (int)(Math.Floor((i + 2) * bucketSize) + 1);
            avgRangeEnd = Math.Min(avgRangeEnd, data.Count);

            int avgRangeLength = avgRangeEnd - avgRangeStart;

            for (int j = avgRangeStart; j < avgRangeEnd; j++)
            {
                avgX += data[j].X;
                avgY += data[j].Y;
            }

            if (avgRangeLength > 0)
            {
                avgX /= avgRangeLength;
                avgY /= avgRangeLength;
            }

            // Get the range for this bucket
            int rangeOffs = (int)(Math.Floor(i * bucketSize) + 1);
            int rangeTo = (int)(Math.Floor((i + 1) * bucketSize) + 1);

            // Point a (the previous selected point)
            double pointAX = data[a].X;
            double pointAY = data[a].Y;

            double maxArea = -1;
            int maxAreaPoint = rangeOffs;

            for (int j = rangeOffs; j < rangeTo; j++)
            {
                // Calculate triangle area over three buckets
                double area = Math.Abs(
                    (pointAX - avgX) * (data[j].Y - pointAY) -
                    (pointAX - data[j].X) * (avgY - pointAY)
                ) * 0.5;

                if (area > maxArea)
                {
                    maxArea = area;
                    maxAreaPoint = j;
                }
            }

            // Pick point with the largest triangle area
            result.Add(new DataPoint(data[maxAreaPoint].X, data[maxAreaPoint].Y));
            a = maxAreaPoint; // This point is the next 'a' (previous selected point)
        }

        // Always add the last point
        result.Add(new DataPoint(data[data.Count - 1].X, data[data.Count - 1].Y));

        return result;
    }

    /// <summary>
    /// Downsamples data using LTTB with custom point selector.
    /// Allows using LTTB with custom data types that may have additional properties.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data points (must be sorted by X).</param>
    /// <param name="threshold">The desired number of output points.</param>
    /// <param name="createPoint">Function to create output point from selected input point.</param>
    /// <returns>Downsampled data points.</returns>
    public static List<TResult> DownsampleCustom<T, TResult>(
        IReadOnlyList<T> data,
        int threshold,
        Func<T, TResult> createPoint) where T : IDataPoint
    {
        if (data.Count <= threshold || threshold < 3)
        {
            return data.Select(createPoint).ToList();
        }

        var result = new List<TResult>(threshold);
        double bucketSize = (double)(data.Count - 2) / (threshold - 2);

        result.Add(createPoint(data[0]));

        int a = 0;

        for (int i = 0; i < threshold - 2; i++)
        {
            double avgX = 0;
            double avgY = 0;

            int avgRangeStart = (int)(Math.Floor((i + 1) * bucketSize) + 1);
            int avgRangeEnd = (int)(Math.Floor((i + 2) * bucketSize) + 1);
            avgRangeEnd = Math.Min(avgRangeEnd, data.Count);

            int avgRangeLength = avgRangeEnd - avgRangeStart;

            for (int j = avgRangeStart; j < avgRangeEnd; j++)
            {
                avgX += data[j].X;
                avgY += data[j].Y;
            }

            if (avgRangeLength > 0)
            {
                avgX /= avgRangeLength;
                avgY /= avgRangeLength;
            }

            int rangeOffs = (int)(Math.Floor(i * bucketSize) + 1);
            int rangeTo = (int)(Math.Floor((i + 1) * bucketSize) + 1);

            double pointAX = data[a].X;
            double pointAY = data[a].Y;

            double maxArea = -1;
            int maxAreaPoint = rangeOffs;

            for (int j = rangeOffs; j < rangeTo; j++)
            {
                double area = Math.Abs(
                    (pointAX - avgX) * (data[j].Y - pointAY) -
                    (pointAX - data[j].X) * (avgY - pointAY)
                ) * 0.5;

                if (area > maxArea)
                {
                    maxArea = area;
                    maxAreaPoint = j;
                }
            }

            result.Add(createPoint(data[maxAreaPoint]));
            a = maxAreaPoint;
        }

        result.Add(createPoint(data[data.Count - 1]));

        return result;
    }

    /// <summary>
    /// Calculates the effective downsampling ratio achieved.
    /// </summary>
    /// <param name="originalCount">Original number of points.</param>
    /// <param name="downsampledCount">Number of points after downsampling.</param>
    /// <returns>Downsampling ratio (e.g., 0.1 means 10% of original points retained).</returns>
    public static double CalculateDownsamplingRatio(int originalCount, int downsampledCount)
    {
        if (originalCount == 0)
            return 0;

        return (double)downsampledCount / originalCount;
    }

    /// <summary>
    /// Estimates the appropriate threshold for a given target downsampling ratio.
    /// </summary>
    /// <param name="dataCount">Number of data points.</param>
    /// <param name="targetRatio">Target ratio (e.g., 0.1 for 10% retention).</param>
    /// <returns>Suggested threshold value.</returns>
    public static int EstimateThreshold(int dataCount, double targetRatio)
    {
        return Math.Max(3, (int)(dataCount * targetRatio));
    }

    /// <summary>
    /// Estimates the appropriate threshold based on available screen width.
    /// </summary>
    /// <param name="screenWidth">Available screen width in pixels.</param>
    /// <param name="pointsPerPixel">Desired points per pixel (default: 2).</param>
    /// <returns>Suggested threshold value.</returns>
    public static int EstimateThresholdFromScreen(int screenWidth, double pointsPerPixel = 2.0)
    {
        return Math.Max(3, (int)(screenWidth * pointsPerPixel));
    }
}
