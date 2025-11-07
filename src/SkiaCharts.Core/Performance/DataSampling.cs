using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides various data sampling strategies for large datasets.
/// </summary>
public static class DataSampling
{
    /// <summary>
    /// Performs uniform sampling - selects every Nth point.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <returns>Uniformly sampled data points.</returns>
    public static List<T> UniformSample<T>(IReadOnlyList<T> data, int targetCount) where T : IDataPoint
    {
        if (data.Count <= targetCount)
            return new List<T>(data);

        var result = new List<T>(targetCount);
        int step = data.Count / targetCount;

        for (int i = 0; i < data.Count; i += step)
        {
            if (result.Count >= targetCount)
                break;
            result.Add(data[i]);
        }

        return result;
    }

    /// <summary>
    /// Performs random sampling - selects random points.
    /// Useful for statistical analysis but may miss important features.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    /// <returns>Randomly sampled data points.</returns>
    public static List<T> RandomSample<T>(IReadOnlyList<T> data, int targetCount, int? seed = null)
        where T : IDataPoint
    {
        if (data.Count <= targetCount)
            return new List<T>(data);

        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var indices = new HashSet<int>();

        while (indices.Count < targetCount)
        {
            indices.Add(random.Next(data.Count));
        }

        return indices.OrderBy(i => i).Select(i => data[i]).ToList();
    }

    /// <summary>
    /// Performs stratified sampling - ensures even distribution across the X range.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <returns>Stratified sampled data points.</returns>
    public static List<T> StratifiedSample<T>(IReadOnlyList<T> data, int targetCount) where T : IDataPoint
    {
        if (data.Count <= targetCount)
            return new List<T>(data);

        var result = new List<T>(targetCount);
        double strataSize = (double)data.Count / targetCount;

        for (int i = 0; i < targetCount; i++)
        {
            int strataStart = (int)(i * strataSize);
            int strataEnd = (int)((i + 1) * strataSize);

            // Take the middle point of each strata
            int middleIndex = (strataStart + strataEnd) / 2;
            result.Add(data[Math.Min(middleIndex, data.Count - 1)]);
        }

        return result;
    }

    /// <summary>
    /// Performs importance sampling - weights points by their Y value.
    /// Useful for emphasizing significant values.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    /// <returns>Importance sampled data points.</returns>
    public static List<T> ImportanceSample<T>(IReadOnlyList<T> data, int targetCount, int? seed = null)
        where T : IDataPoint
    {
        if (data.Count <= targetCount)
            return new List<T>(data);

        var random = seed.HasValue ? new Random(seed.Value) : new Random();

        // Calculate weights based on absolute Y values
        var weights = data.Select(p => Math.Abs(p.Y)).ToList();
        var totalWeight = weights.Sum();

        if (totalWeight == 0)
            return UniformSample(data, targetCount);

        var normalizedWeights = weights.Select(w => w / totalWeight).ToList();

        // Cumulative distribution function
        var cdf = new List<double>(data.Count);
        double cumulative = 0;
        foreach (var weight in normalizedWeights)
        {
            cumulative += weight;
            cdf.Add(cumulative);
        }

        var selectedIndices = new HashSet<int>();

        while (selectedIndices.Count < targetCount)
        {
            double r = random.NextDouble();
            int index = cdf.BinarySearch(r);
            if (index < 0)
                index = ~index;

            index = Math.Min(index, data.Count - 1);
            selectedIndices.Add(index);
        }

        return selectedIndices.OrderBy(i => i).Select(i => data[i]).ToList();
    }

    /// <summary>
    /// Performs reservoir sampling - online algorithm that samples N items from stream.
    /// Memory efficient for very large datasets.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points (reservoir size).</param>
    /// <param name="seed">Random seed for reproducibility.</param>
    /// <returns>Reservoir sampled data points.</returns>
    public static List<T> ReservoirSample<T>(IEnumerable<T> data, int targetCount, int? seed = null)
        where T : IDataPoint
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var reservoir = new List<T>(targetCount);
        int count = 0;

        foreach (var item in data)
        {
            count++;

            if (reservoir.Count < targetCount)
            {
                reservoir.Add(item);
            }
            else
            {
                int j = random.Next(count);
                if (j < targetCount)
                {
                    reservoir[j] = item;
                }
            }
        }

        return reservoir;
    }

    /// <summary>
    /// Performs min-max sampling - preserves extreme values in each window.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="windowCount">Number of windows.</param>
    /// <returns>Min-max sampled data points.</returns>
    public static List<T> MinMaxSample<T>(IReadOnlyList<T> data, int windowCount) where T : IDataPoint
    {
        if (data.Count <= windowCount * 2)
            return new List<T>(data);

        var result = new List<T>(windowCount * 2);
        int windowSize = data.Count / windowCount;

        for (int i = 0; i < windowCount; i++)
        {
            int start = i * windowSize;
            int end = Math.Min((i + 1) * windowSize, data.Count);

            T? minPoint = default;
            T? maxPoint = default;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            for (int j = start; j < end; j++)
            {
                if (data[j].Y < minY)
                {
                    minY = data[j].Y;
                    minPoint = data[j];
                }
                if (data[j].Y > maxY)
                {
                    maxY = data[j].Y;
                    maxPoint = data[j];
                }
            }

            if (minPoint != null)
                result.Add(minPoint);
            if (maxPoint != null && !EqualityComparer<T>.Default.Equals(minPoint, maxPoint))
                result.Add(maxPoint);
        }

        return result.OrderBy(p => p.X).ToList();
    }

    /// <summary>
    /// Adaptive sampling - varies sampling density based on data complexity.
    /// More points in high-variation areas, fewer in flat areas.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="data">The source data.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <returns>Adaptively sampled data points.</returns>
    public static List<T> AdaptiveSample<T>(IReadOnlyList<T> data, int targetCount) where T : IDataPoint
    {
        if (data.Count <= targetCount || data.Count < 3)
            return new List<T>(data);

        var result = new List<T>(targetCount);

        // Always include first point
        result.Add(data[0]);

        // Calculate local variations
        var variations = new double[data.Count - 2];
        for (int i = 1; i < data.Count - 1; i++)
        {
            double derivative = Math.Abs(data[i + 1].Y - data[i - 1].Y);
            variations[i - 1] = derivative;
        }

        // Normalize variations
        double maxVariation = variations.Max();
        if (maxVariation > 0)
        {
            for (int i = 0; i < variations.Length; i++)
            {
                variations[i] /= maxVariation;
            }
        }

        // Sample based on variations
        double step = (double)data.Count / targetCount;
        double position = step;

        while (result.Count < targetCount - 1 && position < data.Count - 1)
        {
            int index = (int)position;
            result.Add(data[index]);

            // Adjust step based on local variation
            if (index - 1 >= 0 && index - 1 < variations.Length)
            {
                double variationFactor = 1.0 - (variations[index - 1] * 0.5);
                position += step * variationFactor;
            }
            else
            {
                position += step;
            }
        }

        // Always include last point
        result.Add(data[data.Count - 1]);

        return result;
    }

    /// <summary>
    /// Gets the recommended sampling strategy for a dataset.
    /// </summary>
    /// <param name="dataCount">Number of data points.</param>
    /// <param name="targetCount">Target number of points.</param>
    /// <param name="preserveFeatures">Whether to preserve important features.</param>
    /// <returns>Recommended sampling strategy.</returns>
    public static SamplingStrategy GetRecommendedStrategy(
        int dataCount,
        int targetCount,
        bool preserveFeatures = true)
    {
        double ratio = (double)targetCount / dataCount;

        if (ratio >= 0.5)
        {
            // Light sampling
            return SamplingStrategy.Uniform;
        }
        else if (ratio >= 0.1)
        {
            // Medium sampling
            return preserveFeatures ? SamplingStrategy.Adaptive : SamplingStrategy.Stratified;
        }
        else
        {
            // Heavy sampling
            return preserveFeatures ? SamplingStrategy.LTTB : SamplingStrategy.MinMax;
        }
    }
}

/// <summary>
/// Sampling strategy types.
/// </summary>
public enum SamplingStrategy
{
    /// <summary>Uniform sampling (every Nth point).</summary>
    Uniform,

    /// <summary>Random sampling.</summary>
    Random,

    /// <summary>Stratified sampling (even distribution).</summary>
    Stratified,

    /// <summary>Importance sampling (weighted by Y values).</summary>
    Importance,

    /// <summary>Reservoir sampling (online algorithm).</summary>
    Reservoir,

    /// <summary>Min-max sampling (preserves extremes).</summary>
    MinMax,

    /// <summary>Adaptive sampling (density varies with complexity).</summary>
    Adaptive,

    /// <summary>LTTB algorithm (best for visualization).</summary>
    LTTB
}
