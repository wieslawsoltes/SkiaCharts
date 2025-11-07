using System.Diagnostics;
using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides memory profiling and monitoring capabilities.
/// Tracks memory usage, GC collections, and allocation patterns.
/// </summary>
public class MemoryProfiler
{
    private long _initialMemory;
    private long _peakMemory;
    private readonly List<MemorySnapshot> _snapshots;
    private int _gen0Collections;
    private int _gen1Collections;
    private int _gen2Collections;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryProfiler"/> class.
    /// </summary>
    public MemoryProfiler()
    {
        _snapshots = new List<MemorySnapshot>();
        Reset();
    }

    /// <summary>
    /// Resets the profiler and takes an initial snapshot.
    /// </summary>
    public void Reset()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        _initialMemory = GC.GetTotalMemory(false);
        _peakMemory = _initialMemory;
        _snapshots.Clear();

        _gen0Collections = GC.CollectionCount(0);
        _gen1Collections = GC.CollectionCount(1);
        _gen2Collections = GC.CollectionCount(2);

        TakeSnapshot("Initial");
    }

    /// <summary>
    /// Takes a memory snapshot with a label.
    /// </summary>
    /// <param name="label">Label for this snapshot.</param>
    public void TakeSnapshot(string label)
    {
        var currentMemory = GC.GetTotalMemory(false);

        if (currentMemory > _peakMemory)
        {
            _peakMemory = currentMemory;
        }

        var snapshot = new MemorySnapshot
        {
            Label = label,
            Timestamp = DateTime.Now,
            TotalMemory = currentMemory,
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            AllocatedMemory = currentMemory - _initialMemory
        };

        _snapshots.Add(snapshot);
    }

    /// <summary>
    /// Forces garbage collection and takes a snapshot.
    /// </summary>
    /// <param name="label">Label for this snapshot.</param>
    public void ForceGCAndSnapshot(string label)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        TakeSnapshot(label);
    }

    /// <summary>
    /// Gets the current memory usage in bytes.
    /// </summary>
    public long CurrentMemory => GC.GetTotalMemory(false);

    /// <summary>
    /// Gets the peak memory usage in bytes.
    /// </summary>
    public long PeakMemory => _peakMemory;

    /// <summary>
    /// Gets the memory allocated since reset.
    /// </summary>
    public long AllocatedMemory => CurrentMemory - _initialMemory;

    /// <summary>
    /// Gets the number of Gen 0 collections since reset.
    /// </summary>
    public int Gen0Collections => GC.CollectionCount(0) - _gen0Collections;

    /// <summary>
    /// Gets the number of Gen 1 collections since reset.
    /// </summary>
    public int Gen1Collections => GC.CollectionCount(1) - _gen1Collections;

    /// <summary>
    /// Gets the number of Gen 2 collections since reset.
    /// </summary>
    public int Gen2Collections => GC.CollectionCount(2) - _gen2Collections;

    /// <summary>
    /// Gets all snapshots.
    /// </summary>
    public IReadOnlyList<MemorySnapshot> Snapshots => _snapshots.AsReadOnly();

    /// <summary>
    /// Gets a formatted report of memory usage.
    /// </summary>
    public string GetReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("Memory Profile Report");
        report.AppendLine("====================");
        report.AppendLine();
        report.AppendLine($"Initial Memory:  {FormatBytes(_initialMemory)}");
        report.AppendLine($"Current Memory:  {FormatBytes(CurrentMemory)}");
        report.AppendLine($"Peak Memory:     {FormatBytes(_peakMemory)}");
        report.AppendLine($"Allocated:       {FormatBytes(AllocatedMemory)}");
        report.AppendLine();
        report.AppendLine($"GC Collections:");
        report.AppendLine($"  Gen 0: {Gen0Collections}");
        report.AppendLine($"  Gen 1: {Gen1Collections}");
        report.AppendLine($"  Gen 2: {Gen2Collections}");
        report.AppendLine();
        report.AppendLine("Snapshots:");
        report.AppendLine("-----------");

        foreach (var snapshot in _snapshots)
        {
            report.AppendLine($"{snapshot.Label,-20} {FormatBytes(snapshot.TotalMemory),12} " +
                            $"(+{FormatBytes(snapshot.AllocatedMemory)})");
        }

        return report.ToString();
    }

    /// <summary>
    /// Estimates memory usage for a given number of data points.
    /// </summary>
    /// <param name="pointCount">Number of data points.</param>
    /// <param name="bytesPerPoint">Bytes per data point (default: 16 for DataPoint).</param>
    /// <returns>Estimated memory usage in bytes.</returns>
    public static long EstimateMemoryUsage(int pointCount, int bytesPerPoint = 16)
    {
        // Account for list overhead (approximately 1.5x the raw data size)
        return (long)(pointCount * bytesPerPoint * 1.5);
    }

    /// <summary>
    /// Estimates the maximum number of points that can fit in available memory.
    /// </summary>
    /// <param name="availableMemoryMB">Available memory in MB.</param>
    /// <param name="bytesPerPoint">Bytes per data point.</param>
    /// <returns>Estimated maximum point count.</returns>
    public static int EstimateMaxPoints(double availableMemoryMB, int bytesPerPoint = 16)
    {
        long availableBytes = (long)(availableMemoryMB * 1024 * 1024);
        return (int)(availableBytes / (bytesPerPoint * 1.5));
    }

    /// <summary>
    /// Formats bytes into a human-readable string.
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Represents a memory snapshot at a point in time.
/// </summary>
public class MemorySnapshot
{
    /// <summary>Gets or sets the label for this snapshot.</summary>
    public required string Label { get; set; }

    /// <summary>Gets or sets the timestamp.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Gets or sets the total memory at snapshot time.</summary>
    public long TotalMemory { get; set; }

    /// <summary>Gets or sets memory allocated since profiler reset.</summary>
    public long AllocatedMemory { get; set; }

    /// <summary>Gets or sets Gen 0 collection count.</summary>
    public int Gen0Collections { get; set; }

    /// <summary>Gets or sets Gen 1 collection count.</summary>
    public int Gen1Collections { get; set; }

    /// <summary>Gets or sets Gen 2 collection count.</summary>
    public int Gen2Collections { get; set; }
}

/// <summary>
/// Provides memory-aware data loading with automatic downsampling.
/// </summary>
public class MemoryAwareDataLoader
{
    private readonly long _maxMemoryBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryAwareDataLoader"/> class.
    /// </summary>
    /// <param name="maxMemoryMB">Maximum memory to use in MB (default: 100).</param>
    public MemoryAwareDataLoader(double maxMemoryMB = 100)
    {
        _maxMemoryBytes = (long)(maxMemoryMB * 1024 * 1024);
    }

    /// <summary>
    /// Loads data with automatic downsampling if needed to stay within memory limits.
    /// </summary>
    /// <typeparam name="T">The type of data points.</typeparam>
    /// <param name="dataProvider">Data provider.</param>
    /// <param name="totalCount">Total number of points available.</param>
    /// <param name="bytesPerPoint">Bytes per data point.</param>
    /// <returns>Loaded and potentially downsampled data.</returns>
    public List<DataPoint> LoadDataWithMemoryLimit<T>(
        Func<int, List<T>> dataProvider,
        int totalCount,
        int bytesPerPoint = 16) where T : IDataPoint
    {
        // Estimate how many points we can load
        int maxPoints = (int)(_maxMemoryBytes / (bytesPerPoint * 1.5));

        if (totalCount <= maxPoints)
        {
            // Load all data and convert to DataPoint
            var data = dataProvider(totalCount);
            return data.Select(p => new DataPoint(p.X, p.Y)).ToList();
        }
        else
        {
            // Load and downsample
            var allData = dataProvider(totalCount);
            return LargestTriangleThreeBuckets.Downsample(allData, maxPoints);
        }
    }

    /// <summary>
    /// Checks if loading the specified number of points would exceed memory limits.
    /// </summary>
    public bool WouldExceedMemoryLimit(int pointCount, int bytesPerPoint = 16)
    {
        long estimatedMemory = MemoryProfiler.EstimateMemoryUsage(pointCount, bytesPerPoint);
        return estimatedMemory > _maxMemoryBytes;
    }

    /// <summary>
    /// Gets the recommended downsampling threshold for the given point count.
    /// </summary>
    public int GetRecommendedThreshold(int pointCount, int bytesPerPoint = 16)
    {
        int maxPoints = (int)(_maxMemoryBytes / (bytesPerPoint * 1.5));
        return Math.Min(pointCount, maxPoints);
    }
}
