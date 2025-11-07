using System.Diagnostics;
using SkiaCharts.Core.Data;
using SkiaSharp;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides performance benchmarking capabilities for chart operations.
/// Measures rendering speed, memory usage, and update throughput.
/// </summary>
public class PerformanceBenchmark
{
    private readonly List<BenchmarkResult> _results;
    private readonly MemoryProfiler _memoryProfiler;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBenchmark"/> class.
    /// </summary>
    public PerformanceBenchmark()
    {
        _results = new List<BenchmarkResult>();
        _memoryProfiler = new MemoryProfiler();
    }

    /// <summary>
    /// Gets all benchmark results.
    /// </summary>
    public IReadOnlyList<BenchmarkResult> Results => _results.AsReadOnly();

    /// <summary>
    /// Benchmarks rendering performance (FPS test).
    /// </summary>
    /// <param name="renderAction">The rendering action to benchmark.</param>
    /// <param name="pointCount">Number of data points.</param>
    /// <param name="duration">Duration to run the benchmark in seconds.</param>
    /// <returns>Benchmark result with FPS metrics.</returns>
    public BenchmarkResult BenchmarkRendering(
        Action renderAction,
        int pointCount,
        int duration = 5)
    {
        _memoryProfiler.Reset();

        var stopwatch = Stopwatch.StartNew();
        int frameCount = 0;
        var frameTimes = new List<double>();

        // Warm up
        for (int i = 0; i < 10; i++)
        {
            renderAction();
        }

        // Actual benchmark
        var benchmarkStopwatch = Stopwatch.StartNew();
        var frameStopwatch = Stopwatch.StartNew();

        while (benchmarkStopwatch.Elapsed.TotalSeconds < duration)
        {
            frameStopwatch.Restart();
            renderAction();
            frameStopwatch.Stop();

            frameTimes.Add(frameStopwatch.Elapsed.TotalMilliseconds);
            frameCount++;
        }

        stopwatch.Stop();
        _memoryProfiler.TakeSnapshot("After Rendering");

        var result = new BenchmarkResult
        {
            TestName = $"Rendering {pointCount:N0} points",
            PointCount = pointCount,
            Duration = stopwatch.Elapsed,
            FrameCount = frameCount,
            FPS = frameCount / stopwatch.Elapsed.TotalSeconds,
            AverageFrameTime = frameTimes.Average(),
            MinFrameTime = frameTimes.Min(),
            MaxFrameTime = frameTimes.Max(),
            PercentileFrameTime99 = CalculatePercentile(frameTimes, 0.99),
            MemoryUsed = _memoryProfiler.AllocatedMemory,
            PeakMemory = _memoryProfiler.PeakMemory
        };

        _results.Add(result);
        return result;
    }

    /// <summary>
    /// Benchmarks data update throughput (streaming test).
    /// </summary>
    /// <param name="updateAction">The update action to benchmark.</param>
    /// <param name="targetUpdatesPerSecond">Target updates per second.</param>
    /// <param name="duration">Duration to run the benchmark in seconds.</param>
    /// <returns>Benchmark result with update throughput metrics.</returns>
    public BenchmarkResult BenchmarkUpdates(
        Action updateAction,
        int targetUpdatesPerSecond,
        int duration = 5)
    {
        _memoryProfiler.Reset();

        var stopwatch = Stopwatch.StartNew();
        int updateCount = 0;
        var updateTimes = new List<double>();

        // Calculate delay between updates
        double delayMs = 1000.0 / targetUpdatesPerSecond;
        var updateStopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed.TotalSeconds < duration)
        {
            updateStopwatch.Restart();
            updateAction();
            updateStopwatch.Stop();

            updateTimes.Add(updateStopwatch.Elapsed.TotalMilliseconds);
            updateCount++;

            // Maintain target update rate
            double elapsed = updateStopwatch.Elapsed.TotalMilliseconds;
            if (elapsed < delayMs)
            {
                Thread.Sleep((int)(delayMs - elapsed));
            }
        }

        stopwatch.Stop();
        _memoryProfiler.TakeSnapshot("After Updates");

        var result = new BenchmarkResult
        {
            TestName = $"Updates {targetUpdatesPerSecond:N0}/sec",
            Duration = stopwatch.Elapsed,
            UpdateCount = updateCount,
            UpdatesPerSecond = updateCount / stopwatch.Elapsed.TotalSeconds,
            AverageUpdateTime = updateTimes.Average(),
            MinUpdateTime = updateTimes.Min(),
            MaxUpdateTime = updateTimes.Max(),
            PercentileUpdateTime99 = CalculatePercentile(updateTimes, 0.99),
            MemoryUsed = _memoryProfiler.AllocatedMemory,
            PeakMemory = _memoryProfiler.PeakMemory
        };

        _results.Add(result);
        return result;
    }

    /// <summary>
    /// Benchmarks memory usage for large datasets.
    /// </summary>
    /// <param name="loadDataAction">The data loading action.</param>
    /// <param name="pointCount">Number of data points.</param>
    /// <returns>Benchmark result with memory metrics.</returns>
    public BenchmarkResult BenchmarkMemory(
        Action loadDataAction,
        int pointCount)
    {
        _memoryProfiler.Reset();

        var stopwatch = Stopwatch.StartNew();
        loadDataAction();
        stopwatch.Stop();

        _memoryProfiler.TakeSnapshot("After Loading");

        var result = new BenchmarkResult
        {
            TestName = $"Memory {pointCount:N0} points",
            PointCount = pointCount,
            Duration = stopwatch.Elapsed,
            MemoryUsed = _memoryProfiler.AllocatedMemory,
            PeakMemory = _memoryProfiler.PeakMemory,
            MemoryPerPoint = _memoryProfiler.AllocatedMemory / (double)pointCount
        };

        _results.Add(result);
        return result;
    }

    /// <summary>
    /// Benchmarks cold start time (initialization performance).
    /// </summary>
    /// <param name="initAction">The initialization action.</param>
    /// <param name="testName">Name of the test.</param>
    /// <returns>Benchmark result with cold start metrics.</returns>
    public BenchmarkResult BenchmarkColdStart(
        Action initAction,
        string testName = "Cold Start")
    {
        _memoryProfiler.Reset();

        var stopwatch = Stopwatch.StartNew();
        initAction();
        stopwatch.Stop();

        _memoryProfiler.TakeSnapshot("After Init");

        var result = new BenchmarkResult
        {
            TestName = testName,
            Duration = stopwatch.Elapsed,
            ColdStartTime = stopwatch.Elapsed.TotalMilliseconds,
            MemoryUsed = _memoryProfiler.AllocatedMemory,
            PeakMemory = _memoryProfiler.PeakMemory
        };

        _results.Add(result);
        return result;
    }

    /// <summary>
    /// Runs a complete benchmark suite.
    /// </summary>
    /// <param name="suite">The benchmark suite to run.</param>
    /// <returns>Collection of benchmark results.</returns>
    public List<BenchmarkResult> RunSuite(BenchmarkSuite suite)
    {
        var suiteResults = new List<BenchmarkResult>();

        if (suite.ColdStartTest != null)
        {
            var result = BenchmarkColdStart(suite.ColdStartTest, suite.Name + " - Cold Start");
            suiteResults.Add(result);
        }

        if (suite.RenderingTest != null)
        {
            var result = BenchmarkRendering(
                suite.RenderingTest,
                suite.PointCount,
                suite.Duration);
            suiteResults.Add(result);
        }

        if (suite.UpdateTest != null)
        {
            var result = BenchmarkUpdates(
                suite.UpdateTest,
                suite.TargetUpdatesPerSecond,
                suite.Duration);
            suiteResults.Add(result);
        }

        if (suite.MemoryTest != null)
        {
            var result = BenchmarkMemory(suite.MemoryTest, suite.PointCount);
            suiteResults.Add(result);
        }

        return suiteResults;
    }

    /// <summary>
    /// Clears all benchmark results.
    /// </summary>
    public void Clear()
    {
        _results.Clear();
    }

    /// <summary>
    /// Gets a summary report of all benchmark results.
    /// </summary>
    public string GetSummaryReport()
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("Performance Benchmark Summary");
        report.AppendLine("=============================");
        report.AppendLine();

        foreach (var result in _results)
        {
            report.AppendLine($"Test: {result.TestName}");
            report.AppendLine($"  Duration: {result.Duration.TotalSeconds:F2}s");

            if (result.FPS > 0)
            {
                report.AppendLine($"  FPS: {result.FPS:F2} ({result.FrameCount} frames)");
                report.AppendLine($"  Frame Time: Avg={result.AverageFrameTime:F2}ms, Min={result.MinFrameTime:F2}ms, Max={result.MaxFrameTime:F2}ms, P99={result.PercentileFrameTime99:F2}ms");
            }

            if (result.UpdatesPerSecond > 0)
            {
                report.AppendLine($"  Updates/sec: {result.UpdatesPerSecond:F2} ({result.UpdateCount} updates)");
                report.AppendLine($"  Update Time: Avg={result.AverageUpdateTime:F2}ms, Min={result.MinUpdateTime:F2}ms, Max={result.MaxUpdateTime:F2}ms, P99={result.PercentileUpdateTime99:F2}ms");
            }

            if (result.ColdStartTime > 0)
            {
                report.AppendLine($"  Cold Start: {result.ColdStartTime:F2}ms");
            }

            if (result.MemoryUsed > 0)
            {
                report.AppendLine($"  Memory: {FormatBytes(result.MemoryUsed)} (Peak: {FormatBytes(result.PeakMemory)})");
                if (result.MemoryPerPoint > 0)
                {
                    report.AppendLine($"  Memory/Point: {result.MemoryPerPoint:F2} bytes");
                }
            }

            report.AppendLine();
        }

        return report.ToString();
    }

    /// <summary>
    /// Calculates the percentile value from a list of values.
    /// </summary>
    private static double CalculatePercentile(List<double> values, double percentile)
    {
        if (values.Count == 0)
            return 0;

        var sorted = values.OrderBy(v => v).ToList();
        int index = (int)Math.Ceiling(percentile * sorted.Count) - 1;
        return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
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
/// Represents a benchmark result.
/// </summary>
public class BenchmarkResult
{
    /// <summary>Gets or sets the test name.</summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of data points.</summary>
    public int PointCount { get; set; }

    /// <summary>Gets or sets the test duration.</summary>
    public TimeSpan Duration { get; set; }

    // Rendering metrics
    /// <summary>Gets or sets the frame count.</summary>
    public int FrameCount { get; set; }

    /// <summary>Gets or sets the frames per second.</summary>
    public double FPS { get; set; }

    /// <summary>Gets or sets the average frame time in milliseconds.</summary>
    public double AverageFrameTime { get; set; }

    /// <summary>Gets or sets the minimum frame time in milliseconds.</summary>
    public double MinFrameTime { get; set; }

    /// <summary>Gets or sets the maximum frame time in milliseconds.</summary>
    public double MaxFrameTime { get; set; }

    /// <summary>Gets or sets the 99th percentile frame time in milliseconds.</summary>
    public double PercentileFrameTime99 { get; set; }

    // Update metrics
    /// <summary>Gets or sets the update count.</summary>
    public int UpdateCount { get; set; }

    /// <summary>Gets or sets the updates per second.</summary>
    public double UpdatesPerSecond { get; set; }

    /// <summary>Gets or sets the average update time in milliseconds.</summary>
    public double AverageUpdateTime { get; set; }

    /// <summary>Gets or sets the minimum update time in milliseconds.</summary>
    public double MinUpdateTime { get; set; }

    /// <summary>Gets or sets the maximum update time in milliseconds.</summary>
    public double MaxUpdateTime { get; set; }

    /// <summary>Gets or sets the 99th percentile update time in milliseconds.</summary>
    public double PercentileUpdateTime99 { get; set; }

    // Memory metrics
    /// <summary>Gets or sets the memory used in bytes.</summary>
    public long MemoryUsed { get; set; }

    /// <summary>Gets or sets the peak memory in bytes.</summary>
    public long PeakMemory { get; set; }

    /// <summary>Gets or sets the memory per point in bytes.</summary>
    public double MemoryPerPoint { get; set; }

    // Cold start metrics
    /// <summary>Gets or sets the cold start time in milliseconds.</summary>
    public double ColdStartTime { get; set; }

    /// <summary>Gets whether the result meets 60 FPS target.</summary>
    public bool Meets60FpsTarget => FPS >= 60;

    /// <summary>Gets whether the result meets streaming target.</summary>
    public bool MeetsStreamingTarget(int targetUpdatesPerSecond) =>
        UpdatesPerSecond >= targetUpdatesPerSecond * 0.95; // Allow 5% tolerance

    /// <summary>Gets whether the result meets memory target.</summary>
    public bool MeetsMemoryTarget(long targetBytes) =>
        MemoryUsed <= targetBytes;

    /// <summary>Gets whether the result meets cold start target.</summary>
    public bool MeetsColdStartTarget(double targetMs) =>
        ColdStartTime <= targetMs;
}

/// <summary>
/// Represents a benchmark suite configuration.
/// </summary>
public class BenchmarkSuite
{
    /// <summary>Gets or sets the suite name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the point count for tests.</summary>
    public int PointCount { get; set; }

    /// <summary>Gets or sets the duration for timed tests.</summary>
    public int Duration { get; set; } = 5;

    /// <summary>Gets or sets the target updates per second.</summary>
    public int TargetUpdatesPerSecond { get; set; } = 10000;

    /// <summary>Gets or sets the cold start test action.</summary>
    public Action? ColdStartTest { get; set; }

    /// <summary>Gets or sets the rendering test action.</summary>
    public Action? RenderingTest { get; set; }

    /// <summary>Gets or sets the update test action.</summary>
    public Action? UpdateTest { get; set; }

    /// <summary>Gets or sets the memory test action.</summary>
    public Action? MemoryTest { get; set; }
}
