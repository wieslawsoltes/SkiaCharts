# SkiaCharts Performance Tuning Guide

This guide provides practical advice for optimizing chart performance in SkiaCharts applications.

## Table of Contents

1. [Quick Wins](#quick-wins)
2. [Data Optimization](#data-optimization)
3. [Rendering Optimization](#rendering-optimization)
4. [Memory Optimization](#memory-optimization)
5. [Streaming Data Optimization](#streaming-data-optimization)
6. [Profiling and Diagnostics](#profiling-and-diagnostics)
7. [Common Performance Issues](#common-performance-issues)
8. [Platform-Specific Tips](#platform-specific-tips)

---

## Quick Wins

These optimizations provide immediate performance improvements with minimal code changes:

### 1. Enable Data Downsampling

**Problem**: Rendering 100K+ points causes low FPS

**Solution**: Use LTTB downsampling

```csharp
// Before (slow)
chart.Data = largeDataset; // 100K points

// After (fast)
var downsampled = LargestTriangleThreeBuckets.Downsample(largeDataset, 2000);
chart.Data = downsampled; // 2K points, visually identical
```

**Impact**: 10-50x rendering speed improvement

### 2. Use Circular Buffers for Streaming

**Problem**: Memory grows unbounded with streaming data

**Solution**: Use fixed-size circular buffer

```csharp
// Before (memory leak)
var data = new List<DataPoint>();
// data.Add() grows forever

// After (fixed memory)
var buffer = new CircularBuffer<DataPoint>(10000); // Max 10K points
buffer.Add(newPoint); // Automatically removes oldest
```

**Impact**: Constant memory usage, prevents memory leaks

### 3. Enable Background Processing

**Problem**: UI freezes during heavy data processing

**Solution**: Offload to background thread

```csharp
var processor = new BackgroundDataProcessor<DataPoint>();
processor.Start();

processor.ProcessingCompleted += (s, e) =>
{
    // Update chart on UI thread
    chart.Data = e.Result;
};

processor.QueueLttbDownsampling(largeDataset, 2000);
```

**Impact**: Responsive UI during processing

### 4. Reduce Anti-Aliasing for Static Charts

**Problem**: Anti-aliasing is expensive for complex paths

**Solution**: Disable for static/non-interactive charts

```csharp
var paint = new SKPaint
{
    IsAntialias = false, // Disable for better performance
    StrokeWidth = 2
};
```

**Impact**: 2-3x rendering speed improvement

### 5. Use Virtual Data Providers

**Problem**: Loading millions of points into memory

**Solution**: Use virtual scrolling with lazy loading

```csharp
var dataProvider = new InMemoryDataProvider<DataPoint>(allData);
var virtualProvider = new VirtualDataProvider<DataPoint>(dataProvider, pageSize: 1000);

// Only loads visible range
var visibleData = virtualProvider.GetRange(startIndex, endIndex);
```

**Impact**: Constant memory usage regardless of dataset size

---

## Data Optimization

### Choosing the Right Downsampling Strategy

Different scenarios require different strategies:

#### Large Static Datasets (> 10K points)

**Use LTTB**: Best for visualization, preserves shape

```csharp
var downsampled = LargestTriangleThreeBuckets.Downsample(data, threshold: 2000);
```

- **Pros**: Preserves visual characteristics, fast (O(n))
- **Cons**: Requires full dataset in memory
- **Best for**: Financial charts, sensor data, time series

#### Real-Time Streaming Data

**Use Circular Buffer + Aggregation**: Efficient for live updates

```csharp
var buffer = new CircularBuffer<DataPoint>(10000);
// ... add streaming data

// Optionally aggregate for display
var aggregated = DataAggregation.Aggregate(buffer.GetAll(), 500);
```

- **Pros**: Constant memory, O(1) updates
- **Cons**: Fixed window size
- **Best for**: Live monitoring, real-time dashboards

#### Statistical Analysis

**Use Aggregation**: Groups data into bins

```csharp
var aggregated = DataAggregation.Aggregate(
    data,
    binCount: 100,
    method: AggregationMethod.Average
);
```

- **Pros**: Reduces noise, shows trends
- **Cons**: Loses detail
- **Best for**: Trend analysis, summary views

#### Preserving Extremes

**Use Min-Max Sampling**: Keeps important values

```csharp
var sampled = DataSampling.MinMaxSample(data, windowCount: 100);
```

- **Pros**: Preserves peaks and valleys
- **Cons**: May include outliers
- **Best for**: Range charts, volatility analysis

### Automatic Strategy Selection

Let the library choose:

```csharp
var strategy = DataSampling.GetRecommendedStrategy(
    dataCount: data.Count,
    targetCount: 1000,
    preserveFeatures: true
);

// Use the recommended strategy
switch (strategy)
{
    case SamplingStrategy.LTTB:
        result = LargestTriangleThreeBuckets.Downsample(data, 1000);
        break;
    case SamplingStrategy.Adaptive:
        result = DataSampling.AdaptiveSample(data, 1000);
        break;
    // ... etc
}
```

---

## Rendering Optimization

### Path Simplification

For line charts with many points:

```csharp
// Simplify path before rendering
var simplified = PathSimplification.DouglasPeucker(
    points,
    tolerance: 1.0 // Adjust based on zoom level
);

// Render simplified path
foreach (var point in simplified)
{
    canvas.DrawLine(/* ... */);
}
```

**Tolerance Guidelines**:
- Zoomed out: 5.0-10.0 (aggressive simplification)
- Normal view: 1.0-2.0 (balanced)
- Zoomed in: 0.1-0.5 (preserve detail)

### Reduce Overdraw

**Problem**: Drawing the same pixels multiple times

**Solution**: Clip to visible region

```csharp
// Cull points outside visible range
var visiblePoints = data
    .Where(p => p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY)
    .ToList();

// Only render visible points
RenderPoints(visiblePoints);
```

### Batch Drawing Operations

**Problem**: Many small draw calls are expensive

**Solution**: Batch into single path

```csharp
// Before (slow)
foreach (var point in points)
{
    canvas.DrawCircle(point.X, point.Y, 3, paint);
}

// After (fast)
using var path = new SKPath();
foreach (var point in points)
{
    path.AddCircle(point.X, point.Y, 3);
}
canvas.DrawPath(path, paint);
```

### Reuse Paint Objects

**Problem**: Creating paint objects is expensive

**Solution**: Reuse and cache paint objects

```csharp
// Before (slow)
foreach (var series in chartSeries)
{
    var paint = new SKPaint { Color = series.Color }; // ❌ Created every frame
    canvas.DrawLine(/* ... */, paint);
    paint.Dispose();
}

// After (fast)
private readonly Dictionary<SKColor, SKPaint> _paintCache = new();

private SKPaint GetPaint(SKColor color)
{
    if (!_paintCache.TryGetValue(color, out var paint))
    {
        paint = new SKPaint { Color = color, IsAntialias = true };
        _paintCache[color] = paint;
    }
    return paint;
}
```

---

## Memory Optimization

### Memory Profiling

Use the built-in memory profiler:

```csharp
var profiler = new MemoryProfiler();

// Before loading data
profiler.TakeSnapshot("Before");

// Load data
var data = LoadLargeDataset();

// After loading
profiler.TakeSnapshot("After");

// Check memory usage
Console.WriteLine(profiler.GetReport());

// Estimate maximum points
var maxPoints = MemoryProfiler.EstimateMaxPoints(
    availableMemoryMB: 100,
    bytesPerPoint: 16
);
```

### Memory-Aware Data Loading

Automatically downsample if memory limit would be exceeded:

```csharp
var loader = new MemoryAwareDataLoader(maxMemoryMB: 100);

// Automatically downsamples if needed
var data = loader.LoadDataWithMemoryLimit(
    dataProvider: (count) => LoadData(count),
    totalCount: 1_000_000,
    bytesPerPoint: 16
);
```

### Dispose Resources Properly

**Critical**: Always dispose SkiaSharp objects

```csharp
// Use 'using' statements
using var surface = SKSurface.Create(imageInfo);
using var canvas = surface.Canvas;
using var paint = new SKPaint();
using var path = new SKPath();

// Or dispose manually in finally block
SKPaint paint = null;
try
{
    paint = new SKPaint();
    // ... use paint
}
finally
{
    paint?.Dispose();
}
```

### Clear Caches Periodically

For long-running applications:

```csharp
// Clear virtual data provider cache
virtualProvider.ClearCache();

// Clear background processor queue
backgroundProcessor.ClearQueue();

// Force garbage collection (use sparingly!)
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
```

---

## Streaming Data Optimization

### Throttle Updates

**Problem**: Too many updates overwhelm the UI

**Solution**: Batch updates or throttle refresh rate

```csharp
private readonly Queue<DataPoint> _pendingUpdates = new();
private DateTime _lastRender = DateTime.MinValue;
private const int RenderIntervalMs = 16; // ~60 FPS

public void AddDataPoint(DataPoint point)
{
    _pendingUpdates.Enqueue(point);

    // Only render at most 60 times per second
    var now = DateTime.Now;
    if ((now - _lastRender).TotalMilliseconds >= RenderIntervalMs)
    {
        ProcessPendingUpdates();
        _lastRender = now;
    }
}

private void ProcessPendingUpdates()
{
    while (_pendingUpdates.Count > 0)
    {
        var point = _pendingUpdates.Dequeue();
        _buffer.Add(point);
    }
    InvalidateChart();
}
```

### Use Background Processing for Heavy Work

```csharp
var processor = new BackgroundDataProcessor<DataPoint>();
processor.Start();

processor.ProcessingCompleted += (s, e) =>
{
    if (e.Success)
    {
        // Update chart on UI thread
        Dispatcher.Invoke(() =>
        {
            chart.Data = e.Result;
        });
    }
};

// Queue processing in background
processor.QueueLttbDownsampling(streamBuffer.GetAll(), 1000);
```

### Incremental Rendering

Only redraw changed portions:

```csharp
private SKBitmap _cachedBackground;
private bool _backgroundDirty = true;

private void Render(SKCanvas canvas)
{
    // Render expensive background once
    if (_backgroundDirty || _cachedBackground == null)
    {
        _cachedBackground = RenderBackground();
        _backgroundDirty = false;
    }

    // Draw cached background
    canvas.DrawBitmap(_cachedBackground, 0, 0);

    // Only render dynamic data
    RenderStreamingData(canvas);
}
```

---

## Profiling and Diagnostics

### Use PerformanceBenchmark

Measure actual performance:

```csharp
var benchmark = new PerformanceBenchmark();

// Benchmark rendering
var result = benchmark.BenchmarkRendering(
    renderAction: () => RenderChart(),
    pointCount: data.Count,
    duration: 5
);

Console.WriteLine($"FPS: {result.FPS:F2}");
Console.WriteLine($"Avg Frame Time: {result.AverageFrameTime:F2}ms");
Console.WriteLine($"P99 Frame Time: {result.PercentileFrameTime99:F2}ms");

// Check if targets are met
if (!result.Meets60FpsTarget)
{
    Console.WriteLine("⚠️ Warning: FPS below 60!");
}
```

### Profile with Diagnostics Tools

**.NET Profiling**:
```bash
# CPU profiling
dotnet trace collect --process-id <pid> --profile cpu-sampling

# Memory profiling
dotnet-dump collect --process-id <pid>
dotnet-dump analyze <dump-file>
```

**Visual Studio Profiler**:
- Debug → Performance Profiler
- Enable CPU Usage, Memory Usage, and Events

**JetBrains dotMemory/dotTrace**:
- Detailed memory and performance analysis
- Timeline view for identifying bottlenecks

### Identify Bottlenecks

Common bottleneck patterns:

1. **High Frame Time**: Rendering is slow
   - Solution: Reduce point count, simplify paths

2. **High Update Time**: Data processing is slow
   - Solution: Use background processing

3. **Memory Spikes**: Allocating too much memory
   - Solution: Use circular buffers, virtual providers

4. **GC Pauses**: Too much garbage collection
   - Solution: Reduce allocations, reuse objects

---

## Common Performance Issues

### Issue 1: Chart Freezes with Large Datasets

**Symptoms**: UI becomes unresponsive when loading data

**Diagnosis**:
```csharp
var profiler = new MemoryProfiler();
var stopwatch = Stopwatch.StartNew();
LoadData();
stopwatch.Stop();
Console.WriteLine($"Load time: {stopwatch.ElapsedMilliseconds}ms");
```

**Solutions**:
1. Downsample data before rendering
2. Use virtual data provider
3. Load data in background thread

### Issue 2: Memory Grows Unbounded

**Symptoms**: Memory usage increases over time

**Diagnosis**:
```csharp
var profiler = new MemoryProfiler();
for (int i = 0; i < 1000; i++)
{
    AddDataPoint(newPoint);
    if (i % 100 == 0)
    {
        profiler.TakeSnapshot($"Iteration {i}");
    }
}
Console.WriteLine(profiler.GetReport());
```

**Solutions**:
1. Use circular buffer instead of list
2. Clear old data periodically
3. Use virtual data provider with cache limits

### Issue 3: Low FPS Even with Few Points

**Symptoms**: Frame rate is low despite small dataset

**Diagnosis**:
```csharp
var benchmark = new PerformanceBenchmark();
var result = benchmark.BenchmarkRendering(RenderChart, data.Count, 5);
Console.WriteLine($"FPS: {result.FPS}, Frame Time: {result.AverageFrameTime}ms");
```

**Solutions**:
1. Reduce anti-aliasing
2. Batch drawing operations
3. Reuse paint objects
4. Check for GPU acceleration issues

### Issue 4: Updates Are Slow

**Symptoms**: Chart lags behind real-time data

**Diagnosis**:
```csharp
var benchmark = new PerformanceBenchmark();
var result = benchmark.BenchmarkUpdates(UpdateChart, targetRate: 1000, duration: 5);
Console.WriteLine($"Updates/sec: {result.UpdatesPerSecond}");
```

**Solutions**:
1. Use circular buffer for O(1) insertions
2. Throttle render rate (60 FPS max)
3. Batch multiple updates
4. Use background processing

---

## Platform-Specific Tips

### Windows

**Enable Hardware Acceleration**:
```csharp
// Use GPU-accelerated rendering
var grContext = GRContext.CreateGl();
var surface = SKSurface.Create(grContext, imageInfo);
```

**Reduce GDI+ Overhead**:
- Use SkiaSharp directly instead of System.Drawing wrappers

### macOS

**Use Metal Backend**:
```csharp
// Enable Metal for better performance
var grContext = GRContext.CreateMetal(metalDevice, metalQueue);
```

**Retina Display Optimization**:
```csharp
// Account for high DPI
var scale = NSScreen.MainScreen.BackingScaleFactor;
var imageInfo = new SKImageInfo(
    (int)(width * scale),
    (int)(height * scale)
);
```

### Linux

**Use Vulkan or OpenGL**:
```csharp
// Vulkan backend (best performance)
var grContext = GRContext.CreateVulkan(vkInterface);

// Or OpenGL fallback
var grContext = GRContext.CreateGl();
```

### Mobile (Xamarin/MAUI)

**Reduce Memory Footprint**:
- Aggressively downsample (500-1000 points max)
- Use smaller textures
- Disable anti-aliasing on low-end devices

**Touch Optimization**:
```csharp
// Increase touch target sizes
const int MinTouchSize = 44; // iOS HIG recommendation

// Throttle touch events
private DateTime _lastTouch = DateTime.MinValue;
private const int TouchThrottleMs = 16;

private void OnTouch(TouchEventArgs e)
{
    var now = DateTime.Now;
    if ((now - _lastTouch).TotalMilliseconds < TouchThrottleMs)
        return;

    _lastTouch = now;
    HandleTouch(e);
}
```

### WebAssembly (Blazor)

**Limit Dataset Size**:
- Max 10K points for acceptable performance
- Downsample aggressively

**Use SkiaSharp Blazor**:
```csharp
<SKCanvasView OnPaintSurface="OnPaintSurface" />

@code {
    private void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        RenderChart(canvas);
    }
}
```

---

## Performance Checklist

Before deploying, verify:

- [ ] Data downsampling enabled for large datasets (> 10K points)
- [ ] Circular buffers used for streaming data
- [ ] Background processing for heavy operations
- [ ] Paint objects cached and reused
- [ ] Resources disposed properly (using statements)
- [ ] Virtual data providers for very large datasets (> 1M points)
- [ ] Rendering throttled to 60 FPS max
- [ ] Path simplification enabled for line charts
- [ ] Hardware acceleration enabled (GPU)
- [ ] Memory profiling shows no leaks
- [ ] Benchmarks meet targets (60 FPS, 10K updates/sec)

---

## Next Steps

- Read [PERFORMANCE_BENCHMARKS.md](PERFORMANCE_BENCHMARKS.md) for detailed metrics
- Review [ARCHITECTURE.md](ARCHITECTURE.md) for system design
- Check [API_REFERENCE.md](API_REFERENCE.md) for optimization APIs
- Join discussions in GitHub Issues with `performance` label

## Support

For performance-related questions:
- GitHub Issues: Tag with `performance`
- Include: dataset size, platform, profiling results
- Provide: minimal reproducible example
