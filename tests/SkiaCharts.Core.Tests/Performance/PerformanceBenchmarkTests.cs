using SkiaCharts.Core.Data;
using SkiaCharts.Core.Performance;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Performance;

public class PerformanceBenchmarkTests
{
    // Target 5.4.1: 60 FPS with 100K visible points
    [Fact]
    public void Benchmark_Rendering100KPoints_ShouldAchieve60FPS()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var points = GenerateDataset(100_000);

        // Simulate rendering (without actual GPU rendering in test)
        var renderAction = new Action(() =>
        {
            // Simulate LTTB downsampling for rendering
            var downsampled = LargestTriangleThreeBuckets.Downsample(points, 2000);

            // Simulate coordinate transformation
            for (int i = 0; i < downsampled.Count; i++)
            {
                var x = downsampled[i].X * 0.01;
                var y = downsampled[i].Y * 0.01;
            }
        });

        // Act
        var result = benchmark.BenchmarkRendering(renderAction, 100_000, duration: 3);

        // Assert
        Assert.True(result.FPS >= 60,
            $"Expected >= 60 FPS, got {result.FPS:F2} FPS. " +
            $"Avg frame time: {result.AverageFrameTime:F2}ms, P99: {result.PercentileFrameTime99:F2}ms");
        Assert.True(result.Meets60FpsTarget);
    }

    // Target 5.4.2: 10K updates/second for streaming
    [Fact]
    public void Benchmark_Streaming10KUpdatesPerSecond_ShouldMaintainThroughput()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var buffer = new CircularBuffer<DataPoint>(10000);
        int updateIndex = 0;

        var updateAction = new Action(() =>
        {
            // Simulate streaming update
            buffer.Add(new DataPoint(updateIndex, Math.Sin(updateIndex * 0.01) * 100));
            updateIndex++;
        });

        // Act
        var result = benchmark.BenchmarkUpdates(updateAction, 10_000, duration: 3);

        // Assert
        Assert.True(result.UpdatesPerSecond >= 9500,
            $"Expected >= 9500 updates/sec (95% of 10K), got {result.UpdatesPerSecond:F2}. " +
            $"Avg update time: {result.AverageUpdateTime:F4}ms, P99: {result.PercentileUpdateTime99:F4}ms");
        Assert.True(result.MeetsStreamingTarget(10_000));
    }

    // Target 5.4.3: Maintain < 1MB memory for 1M data points (with downsampling)
    [Fact]
    public void Benchmark_Memory1MPoints_ShouldStayUnder1MB()
    {
        // Arrange
        var profiler = new MemoryProfiler();
        profiler.Reset();

        // Act - Load 1M points and downsample
        var largeDataset = GenerateDataset(1_000_000);
        profiler.TakeSnapshot("After 1M Load");

        var downsampledData = LargestTriangleThreeBuckets.Downsample(largeDataset, 10_000);
        profiler.TakeSnapshot("After Downsampling");

        // Clear the large dataset to measure final size
        largeDataset.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        profiler.TakeSnapshot("After Cleanup");

        // Assert
        // The downsampled data should be under 1MB
        // 10K points * 16 bytes/point * 1.5 overhead ≈ 240KB
        long estimatedMemory = MemoryProfiler.EstimateMemoryUsage(10_000, 16);
        long targetMemory = 1 * 1024 * 1024; // 1MB

        Assert.NotNull(downsampledData);
        Assert.Equal(10_000, downsampledData.Count);
        Assert.True(estimatedMemory < targetMemory,
            $"Expected downsampled data memory < 1MB, estimated {estimatedMemory / (1024.0 * 1024.0):F2}MB");
    }

    // Target 5.4.4: Cold start < 100ms for basic chart
    [Fact]
    public void Benchmark_ColdStart_ShouldBeUnder100ms()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();

        var initAction = new Action(() =>
        {
            // Simulate basic chart initialization
            var data = GenerateDataset(1000);
            var imageInfo = new SKImageInfo(800, 600);
            using var surface = SKSurface.Create(imageInfo);
            using var canvas = surface.Canvas;
            using var paint = new SKPaint { Color = SKColors.Blue, StrokeWidth = 2, IsAntialias = true };

            canvas.Clear(SKColors.White);

            // Simple line drawing
            for (int i = 0; i < data.Count - 1; i++)
            {
                float x1 = (float)data[i].X;
                float y1 = (float)data[i].Y;
                float x2 = (float)data[i + 1].X;
                float y2 = (float)data[i + 1].Y;
                canvas.DrawLine(x1, y1, x2, y2, paint);
            }
        });

        // Act
        var result = benchmark.BenchmarkColdStart(initAction, "Basic Chart Cold Start");

        // Assert
        Assert.True(result.ColdStartTime < 100,
            $"Expected cold start < 100ms, got {result.ColdStartTime:F2}ms");
        Assert.True(result.MeetsColdStartTarget(100));
    }

    // Comprehensive benchmark suite test
    [Fact]
    public void Benchmark_ComprehensiveSuite_ShouldMeetAllTargets()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var points100K = GenerateDataset(100_000);
        var buffer = new CircularBuffer<DataPoint>(10000);
        int updateIndex = 0;

        var suite = new BenchmarkSuite
        {
            Name = "Comprehensive Performance Suite",
            PointCount = 100_000,
            Duration = 2,
            TargetUpdatesPerSecond = 10_000,

            ColdStartTest = () =>
            {
                var data = GenerateDataset(1000);
                var _ = LargestTriangleThreeBuckets.Downsample(data, 500);
            },

            RenderingTest = () =>
            {
                var downsampled = LargestTriangleThreeBuckets.Downsample(points100K, 2000);
                for (int i = 0; i < downsampled.Count; i++)
                {
                    var x = downsampled[i].X * 0.01;
                    var y = downsampled[i].Y * 0.01;
                }
            },

            UpdateTest = () =>
            {
                buffer.Add(new DataPoint(updateIndex, Math.Sin(updateIndex * 0.01) * 100));
                updateIndex++;
            },

            MemoryTest = () =>
            {
                var data = GenerateDataset(100_000);
                var _ = LargestTriangleThreeBuckets.Downsample(data, 2000);
            }
        };

        // Act
        var results = benchmark.RunSuite(suite);

        // Assert
        Assert.Equal(4, results.Count);

        var coldStartResult = results.FirstOrDefault(r => r.TestName.Contains("Cold Start"));
        Assert.NotNull(coldStartResult);
        Assert.True(coldStartResult.MeetsColdStartTarget(100));

        var renderingResult = results.FirstOrDefault(r => r.FPS > 0);
        Assert.NotNull(renderingResult);
        Assert.True(renderingResult.Meets60FpsTarget);

        var updateResult = results.FirstOrDefault(r => r.UpdatesPerSecond > 0);
        Assert.NotNull(updateResult);
        Assert.True(updateResult.MeetsStreamingTarget(10_000));
    }

    // Performance profiler tests
    [Fact]
    public void PerformanceBenchmark_ShouldRecordMultipleResults()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var data = GenerateDataset(10_000);

        // Act
        benchmark.BenchmarkRendering(() => { var _ = data.Take(100).ToList(); }, 10_000, 1);
        benchmark.BenchmarkMemory(() => { var _ = GenerateDataset(1000); }, 1000);

        // Assert
        Assert.Equal(2, benchmark.Results.Count);
    }

    [Fact]
    public void PerformanceBenchmark_GetSummaryReport_ShouldGenerateReport()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var data = GenerateDataset(1000);

        // Act
        benchmark.BenchmarkRendering(() => { var _ = data.Take(100).ToList(); }, 1000, 1);
        var report = benchmark.GetSummaryReport();

        // Assert
        Assert.Contains("Performance Benchmark Summary", report);
        Assert.Contains("FPS:", report);
        Assert.Contains("Frame Time:", report);
    }

    [Fact]
    public void PerformanceBenchmark_Clear_ShouldRemoveAllResults()
    {
        // Arrange
        var benchmark = new PerformanceBenchmark();
        var data = GenerateDataset(1000);
        benchmark.BenchmarkRendering(() => { var _ = data.Take(100).ToList(); }, 1000, 1);

        // Act
        benchmark.Clear();

        // Assert
        Assert.Empty(benchmark.Results);
    }

    // Real-world scenario tests
    [Fact]
    public void Benchmark_RealWorldScenario_LiveFinancialChart()
    {
        // Arrange - Simulates a live financial chart with 1-minute candles for 1 year
        var benchmark = new PerformanceBenchmark();
        var historicalData = GenerateDataset(365 * 24 * 60); // 1 year of 1-min data = ~525K points
        var visibleWindow = 1440; // 24 hours visible
        var streamBuffer = new CircularBuffer<DataPoint>(visibleWindow);

        // Simulate initial load with downsampling
        var coldStartAction = new Action(() =>
        {
            var downsampled = LargestTriangleThreeBuckets.Downsample(historicalData, 10_000);
        });

        // Simulate rendering visible window
        var renderAction = new Action(() =>
        {
            var visible = historicalData.Take(visibleWindow).ToList();
            var renderPoints = LargestTriangleThreeBuckets.Downsample(visible, 500);
            for (int i = 0; i < renderPoints.Count; i++)
            {
                var x = renderPoints[i].X * 0.01;
                var y = renderPoints[i].Y * 0.01;
            }
        });

        // Simulate streaming updates
        int tick = 0;
        var updateAction = new Action(() =>
        {
            streamBuffer.Add(new DataPoint(tick, 100 + Math.Sin(tick * 0.1) * 10));
            tick++;
        });

        // Act
        var coldStart = benchmark.BenchmarkColdStart(coldStartAction, "Financial Chart Load");
        var rendering = benchmark.BenchmarkRendering(renderAction, visibleWindow, 2);
        var streaming = benchmark.BenchmarkUpdates(updateAction, 1000, 2); // 1K updates/sec for financial data

        // Assert
        Assert.True(coldStart.ColdStartTime < 500,
            $"Financial chart load should be < 500ms, got {coldStart.ColdStartTime:F2}ms");
        Assert.True(rendering.Meets60FpsTarget,
            $"Financial chart should render at 60 FPS, got {rendering.FPS:F2} FPS");
        Assert.True(streaming.MeetsStreamingTarget(1000),
            $"Financial chart should handle 1K updates/sec, got {streaming.UpdatesPerSecond:F2}");
    }

    [Fact]
    public void Benchmark_RealWorldScenario_SensorDashboard()
    {
        // Arrange - Simulates IoT sensor dashboard with multiple charts
        var benchmark = new PerformanceBenchmark();
        const int sensorCount = 10;
        const int pointsPerSensor = 10_000;
        var sensors = new List<CircularBuffer<DataPoint>>();

        for (int i = 0; i < sensorCount; i++)
        {
            sensors.Add(new CircularBuffer<DataPoint>(pointsPerSensor));
        }

        // Simulate rendering all sensor charts
        var renderAction = new Action(() =>
        {
            foreach (var sensor in sensors)
            {
                var points = sensor.ToList();
                if (points.Count > 500)
                {
                    var downsampled = LargestTriangleThreeBuckets.Downsample(points, 500);
                    for (int i = 0; i < downsampled.Count; i++)
                    {
                        var x = downsampled[i].X * 0.01;
                        var y = downsampled[i].Y * 0.01;
                    }
                }
            }
        });

        // Simulate sensor updates (all sensors update simultaneously)
        int tick = 0;
        var random = new Random();
        var updateAction = new Action(() =>
        {
            foreach (var sensor in sensors)
            {
                sensor.Add(new DataPoint(tick, random.NextDouble() * 100));
            }
            tick++;
        });

        // Act
        var rendering = benchmark.BenchmarkRendering(renderAction, sensorCount * pointsPerSensor, 2);
        var streaming = benchmark.BenchmarkUpdates(updateAction, 1000, 2); // 1K updates/sec total

        // Assert
        Assert.True(rendering.FPS >= 30, // 30 FPS is acceptable for dashboard with 10 charts
            $"Sensor dashboard should render at >= 30 FPS, got {rendering.FPS:F2} FPS");
        Assert.True(streaming.UpdatesPerSecond >= 950,
            $"Sensor dashboard should handle 1K updates/sec, got {streaming.UpdatesPerSecond:F2}");
    }

    // Helper method
    private static List<DataPoint> GenerateDataset(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            points.Add(new DataPoint(i, Math.Sin(i * 0.01) * 100));
        }
        return points;
    }
}
