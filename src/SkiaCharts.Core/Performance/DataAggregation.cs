using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides data aggregation and binning strategies for large datasets.
/// Aggregation reduces data volume while preserving statistical characteristics.
/// </summary>
public static class DataAggregation
{
    /// <summary>
    /// Aggregates data points into bins using various aggregation methods.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to aggregate.</param>
    /// <param name="binCount">Number of bins to create.</param>
    /// <param name="method">Aggregation method to use.</param>
    /// <returns>Aggregated data points.</returns>
    public static List<DataPoint> Aggregate<T>(
        IReadOnlyList<T> points,
        int binCount,
        AggregationMethod method = AggregationMethod.Average) where T : IDataPoint
    {
        if (points.Count == 0 || binCount <= 0)
            return new List<DataPoint>();

        if (points.Count <= binCount)
            return points.Select(p => new DataPoint(p.X, p.Y)).ToList();

        var result = new List<DataPoint>(binCount);
        double binSize = (double)points.Count / binCount;

        for (int i = 0; i < binCount; i++)
        {
            int startIndex = (int)(i * binSize);
            int endIndex = (int)Math.Min((i + 1) * binSize, points.Count);

            var binPoints = points.Skip(startIndex).Take(endIndex - startIndex).ToList();
            if (binPoints.Count == 0)
                continue;

            var aggregatedPoint = AggregateBin(binPoints, method);
            result.Add(aggregatedPoint);
        }

        return result;
    }

    /// <summary>
    /// Aggregates data points into fixed-width X-axis bins.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to aggregate.</param>
    /// <param name="binWidth">Width of each bin on the X axis.</param>
    /// <param name="method">Aggregation method to use.</param>
    /// <returns>Aggregated data points.</returns>
    public static List<DataPoint> AggregateByXWidth<T>(
        IReadOnlyList<T> points,
        double binWidth,
        AggregationMethod method = AggregationMethod.Average) where T : IDataPoint
    {
        if (points.Count == 0 || binWidth <= 0)
            return new List<DataPoint>();

        var result = new List<DataPoint>();
        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);

        for (double x = minX; x < maxX; x += binWidth)
        {
            var binPoints = points.Where(p => p.X >= x && p.X < x + binWidth).ToList();
            if (binPoints.Count == 0)
                continue;

            var aggregatedPoint = AggregateBin(binPoints, method);
            // Use bin center as X coordinate
            aggregatedPoint = new DataPoint(x + binWidth / 2, aggregatedPoint.Y);
            result.Add(aggregatedPoint);
        }

        return result;
    }

    /// <summary>
    /// Creates OHLC (Open-High-Low-Close) aggregation for time series data.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to aggregate.</param>
    /// <param name="binCount">Number of bins to create.</param>
    /// <returns>OHLC aggregated data points.</returns>
    public static List<OhlcDataPoint> AggregateOHLC<T>(
        IReadOnlyList<T> points,
        int binCount) where T : IDataPoint
    {
        if (points.Count == 0 || binCount <= 0)
            return new List<OhlcDataPoint>();

        if (points.Count <= binCount)
            return points.Select(p => new OhlcDataPoint(p.X, p.Y, p.Y, p.Y, p.Y)).ToList();

        var result = new List<OhlcDataPoint>(binCount);
        double binSize = (double)points.Count / binCount;

        for (int i = 0; i < binCount; i++)
        {
            int startIndex = (int)(i * binSize);
            int endIndex = (int)Math.Min((i + 1) * binSize, points.Count);

            var binPoints = points.Skip(startIndex).Take(endIndex - startIndex).ToList();
            if (binPoints.Count == 0)
                continue;

            var open = binPoints.First().Y;
            var close = binPoints.Last().Y;
            var high = binPoints.Max(p => p.Y);
            var low = binPoints.Min(p => p.Y);
            var centerX = binPoints[binPoints.Count / 2].X;

            result.Add(new OhlcDataPoint(centerX, open, high, low, close));
        }

        return result;
    }

    /// <summary>
    /// Aggregates a single bin of points using the specified method.
    /// </summary>
    private static DataPoint AggregateBin<T>(List<T> binPoints, AggregationMethod method) where T : IDataPoint
    {
        if (binPoints.Count == 0)
            return new DataPoint(0, 0);

        // Use middle point's X coordinate
        var x = binPoints[binPoints.Count / 2].X;
        double y;

        switch (method)
        {
            case AggregationMethod.Average:
                y = binPoints.Average(p => p.Y);
                break;

            case AggregationMethod.Min:
                y = binPoints.Min(p => p.Y);
                break;

            case AggregationMethod.Max:
                y = binPoints.Max(p => p.Y);
                break;

            case AggregationMethod.First:
                y = binPoints.First().Y;
                break;

            case AggregationMethod.Last:
                y = binPoints.Last().Y;
                break;

            case AggregationMethod.Median:
                var sorted = binPoints.Select(p => p.Y).OrderBy(v => v).ToList();
                y = sorted[sorted.Count / 2];
                break;

            case AggregationMethod.Sum:
                y = binPoints.Sum(p => p.Y);
                break;

            default:
                y = binPoints.Average(p => p.Y);
                break;
        }

        return new DataPoint(x, y);
    }

    /// <summary>
    /// Creates a min-max aggregation that preserves the range of values in each bin.
    /// Returns pairs of points (min, max) for each bin.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="points">The data points to aggregate.</param>
    /// <param name="binCount">Number of bins to create.</param>
    /// <returns>Min-max aggregated data points.</returns>
    public static List<DataPoint> AggregateMinMax<T>(
        IReadOnlyList<T> points,
        int binCount) where T : IDataPoint
    {
        if (points.Count == 0 || binCount <= 0)
            return new List<DataPoint>();

        if (points.Count <= binCount * 2)
            return points.Select(p => new DataPoint(p.X, p.Y)).ToList();

        var result = new List<DataPoint>(binCount * 2);
        double binSize = (double)points.Count / binCount;

        for (int i = 0; i < binCount; i++)
        {
            int startIndex = (int)(i * binSize);
            int endIndex = (int)Math.Min((i + 1) * binSize, points.Count);

            var binPoints = points.Skip(startIndex).Take(endIndex - startIndex).ToList();
            if (binPoints.Count == 0)
                continue;

            var x = binPoints[binPoints.Count / 2].X;
            var min = binPoints.Min(p => p.Y);
            var max = binPoints.Max(p => p.Y);

            // Add both min and max to preserve range
            result.Add(new DataPoint(x, min));
            result.Add(new DataPoint(x, max));
        }

        return result;
    }
}

/// <summary>
/// Aggregation methods for data binning.
/// </summary>
public enum AggregationMethod
{
    /// <summary>Average of all points in the bin.</summary>
    Average,

    /// <summary>Minimum value in the bin.</summary>
    Min,

    /// <summary>Maximum value in the bin.</summary>
    Max,

    /// <summary>First point in the bin.</summary>
    First,

    /// <summary>Last point in the bin.</summary>
    Last,

    /// <summary>Median value in the bin.</summary>
    Median,

    /// <summary>Sum of all points in the bin.</summary>
    Sum
}

/// <summary>
/// Represents an OHLC (Open-High-Low-Close) data point.
/// </summary>
public class OhlcDataPoint : IDataPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OhlcDataPoint"/> class.
    /// </summary>
    public OhlcDataPoint(double x, double open, double high, double low, double close)
    {
        X = x;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Y = close; // Y represents close price
    }

    /// <inheritdoc/>
    public double X { get; }

    /// <inheritdoc/>
    public double Y { get; }

    /// <summary>Gets the opening price.</summary>
    public double Open { get; }

    /// <summary>Gets the highest price.</summary>
    public double High { get; }

    /// <summary>Gets the lowest price.</summary>
    public double Low { get; }

    /// <summary>Gets the closing price.</summary>
    public double Close { get; }
}
