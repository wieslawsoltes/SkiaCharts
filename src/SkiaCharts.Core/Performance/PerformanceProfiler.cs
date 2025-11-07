using System.Diagnostics;

namespace SkiaCharts.Core.Performance;

/// <summary>
/// Provides performance profiling and monitoring capabilities for chart rendering.
/// </summary>
public class PerformanceProfiler
{
    private readonly Dictionary<string, ProfilerEntry> _entries = new();
    private readonly Stopwatch _globalStopwatch = new();

    /// <summary>
    /// Gets whether the profiler is currently active.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Starts profiling a specific operation.
    /// </summary>
    /// <param name="operationName">Name of the operation to profile.</param>
    /// <returns>A stopwatch for the operation.</returns>
    public Stopwatch StartOperation(string operationName)
    {
        if (!IsEnabled)
            return Stopwatch.StartNew();

        if (!_entries.ContainsKey(operationName))
        {
            _entries[operationName] = new ProfilerEntry { Name = operationName };
        }

        var sw = Stopwatch.StartNew();
        _entries[operationName].ActiveStopwatch = sw;
        return sw;
    }

    /// <summary>
    /// Ends profiling of a specific operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    public void EndOperation(string operationName)
    {
        if (!IsEnabled || !_entries.ContainsKey(operationName))
            return;

        var entry = _entries[operationName];
        if (entry.ActiveStopwatch != null)
        {
            entry.ActiveStopwatch.Stop();
            entry.TotalTime += entry.ActiveStopwatch.Elapsed;
            entry.CallCount++;
            entry.LastDuration = entry.ActiveStopwatch.Elapsed;

            // Track min/max
            if (entry.CallCount == 1 || entry.ActiveStopwatch.Elapsed < entry.MinDuration)
            {
                entry.MinDuration = entry.ActiveStopwatch.Elapsed;
            }
            if (entry.CallCount == 1 || entry.ActiveStopwatch.Elapsed > entry.MaxDuration)
            {
                entry.MaxDuration = entry.ActiveStopwatch.Elapsed;
            }

            entry.ActiveStopwatch = null;
        }
    }

    /// <summary>
    /// Profiles an operation using a disposable scope.
    /// </summary>
    /// <param name="operationName">Name of the operation to profile.</param>
    /// <returns>A disposable profiler scope.</returns>
    public ProfilerScope Profile(string operationName)
    {
        return new ProfilerScope(this, operationName);
    }

    /// <summary>
    /// Gets profiling results for a specific operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <returns>Profiling results or null if not found.</returns>
    public ProfilerResult? GetResults(string operationName)
    {
        if (!_entries.ContainsKey(operationName))
            return null;

        var entry = _entries[operationName];
        return new ProfilerResult
        {
            Name = operationName,
            TotalTime = entry.TotalTime,
            CallCount = entry.CallCount,
            AverageTime = entry.CallCount > 0 ? entry.TotalTime / entry.CallCount : TimeSpan.Zero,
            MinDuration = entry.MinDuration,
            MaxDuration = entry.MaxDuration,
            LastDuration = entry.LastDuration
        };
    }

    /// <summary>
    /// Gets all profiling results.
    /// </summary>
    /// <returns>A collection of profiling results.</returns>
    public IEnumerable<ProfilerResult> GetAllResults()
    {
        return _entries.Values.Select(e => new ProfilerResult
        {
            Name = e.Name,
            TotalTime = e.TotalTime,
            CallCount = e.CallCount,
            AverageTime = e.CallCount > 0 ? e.TotalTime / e.CallCount : TimeSpan.Zero,
            MinDuration = e.MinDuration,
            MaxDuration = e.MaxDuration,
            LastDuration = e.LastDuration
        });
    }

    /// <summary>
    /// Resets all profiling data.
    /// </summary>
    public void Reset()
    {
        _entries.Clear();
        _globalStopwatch.Reset();
    }

    /// <summary>
    /// Gets a formatted report of profiling results.
    /// </summary>
    /// <returns>A formatted string containing profiling data.</returns>
    public string GetReport()
    {
        var results = GetAllResults().OrderByDescending(r => r.TotalTime).ToList();

        if (results.Count == 0)
            return "No profiling data available.";

        var report = new System.Text.StringBuilder();
        report.AppendLine("Performance Profile Report");
        report.AppendLine("=========================");
        report.AppendLine();

        foreach (var result in results)
        {
            report.AppendLine($"Operation: {result.Name}");
            report.AppendLine($"  Calls:       {result.CallCount}");
            report.AppendLine($"  Total Time:  {result.TotalTime.TotalMilliseconds:F2} ms");
            report.AppendLine($"  Average:     {result.AverageTime.TotalMilliseconds:F2} ms");
            report.AppendLine($"  Min:         {result.MinDuration.TotalMilliseconds:F2} ms");
            report.AppendLine($"  Max:         {result.MaxDuration.TotalMilliseconds:F2} ms");
            report.AppendLine($"  Last:        {result.LastDuration.TotalMilliseconds:F2} ms");
            report.AppendLine();
        }

        return report.ToString();
    }

    private class ProfilerEntry
    {
        public string Name { get; set; } = string.Empty;
        public TimeSpan TotalTime { get; set; }
        public int CallCount { get; set; }
        public TimeSpan MinDuration { get; set; }
        public TimeSpan MaxDuration { get; set; }
        public TimeSpan LastDuration { get; set; }
        public Stopwatch? ActiveStopwatch { get; set; }
    }
}

/// <summary>
/// Represents profiling results for an operation.
/// </summary>
public class ProfilerResult
{
    /// <summary>
    /// Gets the name of the operation.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the total time spent in this operation.
    /// </summary>
    public TimeSpan TotalTime { get; init; }

    /// <summary>
    /// Gets the number of times this operation was called.
    /// </summary>
    public int CallCount { get; init; }

    /// <summary>
    /// Gets the average time per call.
    /// </summary>
    public TimeSpan AverageTime { get; init; }

    /// <summary>
    /// Gets the minimum duration.
    /// </summary>
    public TimeSpan MinDuration { get; init; }

    /// <summary>
    /// Gets the maximum duration.
    /// </summary>
    public TimeSpan MaxDuration { get; init; }

    /// <summary>
    /// Gets the duration of the last call.
    /// </summary>
    public TimeSpan LastDuration { get; init; }
}

/// <summary>
/// A disposable profiler scope for automatic timing.
/// </summary>
public readonly struct ProfilerScope : IDisposable
{
    private readonly PerformanceProfiler _profiler;
    private readonly string _operationName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilerScope"/> struct.
    /// </summary>
    /// <param name="profiler">The profiler instance.</param>
    /// <param name="operationName">Name of the operation.</param>
    public ProfilerScope(PerformanceProfiler profiler, string operationName)
    {
        _profiler = profiler;
        _operationName = operationName;
        _profiler.StartOperation(_operationName);
    }

    /// <summary>
    /// Ends the profiling scope.
    /// </summary>
    public void Dispose()
    {
        _profiler.EndOperation(_operationName);
    }
}

/// <summary>
/// Tracks frame rate and rendering performance.
/// </summary>
public class FrameRateMonitor
{
    private readonly Queue<DateTime> _frameTimes = new();
    private readonly int _sampleSize;
    private DateTime _lastFrameTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameRateMonitor"/> class.
    /// </summary>
    /// <param name="sampleSize">Number of frames to average (default: 60).</param>
    public FrameRateMonitor(int sampleSize = 60)
    {
        _sampleSize = sampleSize;
        _lastFrameTime = DateTime.Now;
    }

    /// <summary>
    /// Records a frame render.
    /// </summary>
    public void RecordFrame()
    {
        var now = DateTime.Now;
        _frameTimes.Enqueue(now);

        // Keep only the last N frames
        while (_frameTimes.Count > _sampleSize)
        {
            _frameTimes.Dequeue();
        }

        _lastFrameTime = now;
    }

    /// <summary>
    /// Gets the current frame rate (FPS).
    /// </summary>
    public double CurrentFps
    {
        get
        {
            if (_frameTimes.Count < 2)
                return 0;

            var span = _lastFrameTime - _frameTimes.Peek();
            if (span.TotalSeconds == 0)
                return 0;

            return _frameTimes.Count / span.TotalSeconds;
        }
    }

    /// <summary>
    /// Gets the average frame time in milliseconds.
    /// </summary>
    public double AverageFrameTime
    {
        get
        {
            if (_frameTimes.Count < 2)
                return 0;

            var span = _lastFrameTime - _frameTimes.Peek();
            return span.TotalMilliseconds / _frameTimes.Count;
        }
    }

    /// <summary>
    /// Resets the frame rate monitor.
    /// </summary>
    public void Reset()
    {
        _frameTimes.Clear();
        _lastFrameTime = DateTime.Now;
    }
}
