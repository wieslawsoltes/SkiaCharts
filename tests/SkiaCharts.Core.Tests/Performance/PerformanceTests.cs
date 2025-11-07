using SkiaCharts.Core.Data;
using SkiaCharts.Core.Performance;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Performance;

public class PerformanceTests
{
    // ViewportCulling Tests
    [Fact]
    public void ViewportCulling_GetVisiblePoints_ShouldFilterCorrectly()
    {
        // Arrange
        var points = GenerateTestPoints(100);

        // Act
        var visiblePoints = ViewportCulling.GetVisiblePoints(points, 25, 75).ToList();

        // Assert
        Assert.True(visiblePoints.Count > 0);
        Assert.True(visiblePoints.Count < 100);
        Assert.All(visiblePoints, p => Assert.InRange(p.X, 24, 76)); // Includes padding
    }

    [Fact]
    public void ViewportCulling_GetVisiblePointsOptimized_ShouldUseBinarySearch()
    {
        // Arrange
        var points = GenerateTestPoints(1000);

        // Act
        var visiblePoints = ViewportCulling.GetVisiblePointsOptimized(points, 250, 750).ToList();

        // Assert
        Assert.True(visiblePoints.Count > 0);
        Assert.True(visiblePoints.Count < 1000);
        Assert.All(visiblePoints.Skip(1).Take(visiblePoints.Count - 2),
            p => Assert.InRange(p.X, 250, 750));
    }

    [Fact]
    public void ViewportCulling_EstimateVisibleCount_ShouldReturnReasonableEstimate()
    {
        // Arrange
        var points = GenerateTestPoints(1000);

        // Act
        var estimate = ViewportCulling.EstimateVisibleCount(points, 250, 750);

        // Assert
        Assert.InRange(estimate, 400, 600); // Should be around 500
    }

    // LevelOfDetail Tests
    [Fact]
    public void LevelOfDetail_Decimate_ShouldReducePointCount()
    {
        // Arrange
        var points = GenerateTestPoints(1000);

        // Act
        var decimated = LevelOfDetail.Decimate(points, 100);

        // Assert
        Assert.Equal(100, decimated.Count);
        Assert.Equal(points[0].X, decimated[0].X); // First point preserved
        Assert.Equal(points[999].X, decimated[99].X); // Last point preserved
    }

    [Fact]
    public void LevelOfDetail_DecimateByScreenWidth_ShouldLimitPointsPerPixel()
    {
        // Arrange
        var points = GenerateTestPoints(10000);

        // Act
        var decimated = LevelOfDetail.DecimateByScreenWidth(points, 800, 2.0);

        // Assert
        Assert.True(decimated.Count <= 1600); // 800 * 2
    }

    [Fact]
    public void LevelOfDetail_DecimatePreserveFeatures_ShouldKeepPeaks()
    {
        // Arrange
        var points = GenerateTestPointsWithPeaks(1000);

        // Act
        var decimated = LevelOfDetail.DecimatePreserveFeatures(points, 100);

        // Assert
        Assert.True(decimated.Count <= 100);
        Assert.Contains(decimated, p => Math.Abs(p.Y - 100) < 1); // Peak should be preserved
    }

    [Fact]
    public void LevelOfDetail_CalculateLodLevel_ShouldReturnAppropriateLevel()
    {
        // Arrange & Act
        var level0 = LevelOfDetail.CalculateLodLevel(1000, 800);   // ratio 1.25
        var level2 = LevelOfDetail.CalculateLodLevel(8000, 800);   // ratio 10
        var level4 = LevelOfDetail.CalculateLodLevel(20000, 800);  // ratio 25

        // Assert
        Assert.Equal(0, level0);
        Assert.Equal(2, level2);
        Assert.Equal(4, level4);
    }

    // PathSimplification Tests
    [Fact]
    public void PathSimplification_DouglasPeucker_ShouldSimplifyPath()
    {
        // Arrange
        var points = GenerateZigzagPoints(1000);

        // Act
        var simplified = PathSimplification.DouglasPeucker(points, 5.0);

        // Assert
        Assert.True(simplified.Count < points.Count);
        Assert.Equal(points[0].X, simplified[0].X); // First point preserved
        Assert.Equal(points[points.Count - 1].X, simplified[simplified.Count - 1].X); // Last point preserved
    }

    [Fact]
    public void PathSimplification_DouglasPeucker_WithLowTolerance_ShouldPreserveMorePoints()
    {
        // Arrange
        var points = GenerateZigzagPoints(100);

        // Act
        var simplified1 = PathSimplification.DouglasPeucker(points, 1.0);
        var simplified2 = PathSimplification.DouglasPeucker(points, 10.0);

        // Assert
        Assert.True(simplified1.Count > simplified2.Count);
    }

    [Fact]
    public void PathSimplification_CalculateReductionRatio_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var ratio = PathSimplification.CalculateReductionRatio(1000, 100);

        // Assert
        Assert.Equal(0.9, ratio); // 90% reduction
    }

    [Fact]
    public void PathSimplification_EstimateTolerance_ShouldReturnReasonableValue()
    {
        // Arrange & Act
        var tolerance = PathSimplification.EstimateTolerance(100.0, 1000, 100);

        // Assert
        Assert.True(tolerance > 0);
        Assert.True(tolerance < 100.0);
    }

    // ObjectPooling Tests
    [Fact]
    public void ObjectPooling_PathPool_ShouldReuseObjects()
    {
        // Arrange
        var pool = new SKPathPool();

        // Act
        var path1 = pool.Rent();
        pool.Return(path1);
        var path2 = pool.Rent();

        // Assert
        Assert.Same(path1, path2); // Should be the same instance
    }

    [Fact]
    public void ObjectPooling_PathPool_ShouldTrackStatistics()
    {
        // Arrange
        var pool = new SKPathPool();

        // Act
        var path1 = pool.Rent();
        var path2 = pool.Rent();
        pool.Return(path1);
        pool.Return(path2);

        // Assert
        Assert.Equal(2, pool.TotalCreated);
        Assert.Equal(2, pool.CurrentPooled);
    }

    [Fact]
    public void ObjectPooling_PaintPool_ShouldReuseObjects()
    {
        // Arrange
        var pool = new SKPaintPool();

        // Act
        var paint1 = pool.Rent();
        pool.Return(paint1);
        var paint2 = pool.Rent();

        // Assert
        Assert.Same(paint1, paint2);
    }

    [Fact]
    public void ObjectPooling_PooledObject_ShouldReturnOnDispose()
    {
        // Arrange
        var pool = new SKPathPool();

        // Act
        using (var pooled = new PooledObject<SKPath>(pool.Rent(), pool.Return))
        {
            Assert.NotNull(pooled.Object);
        }

        // Assert
        Assert.Equal(1, pool.CurrentPooled);
    }

    // RenderOptimization Tests
    [Fact]
    public void RenderOptimization_CreateOptimizedLinePaint_ShouldSetCorrectProperties()
    {
        // Arrange & Act
        using var paint = RenderOptimization.CreateOptimizedLinePaint(SKColors.Blue, 2f);

        // Assert
        Assert.Equal(SKPaintStyle.Stroke, paint.Style);
        Assert.Equal(2f, paint.StrokeWidth);
        Assert.True(paint.IsAntialias);
        Assert.Equal(SKFilterQuality.Low, paint.FilterQuality);
    }

    [Fact]
    public void RenderOptimization_GetOptimalRenderSettings_ShouldAdjustByZoom()
    {
        // Arrange & Act
        var highZoom = RenderOptimization.GetOptimalRenderSettings(10.0);
        var lowZoom = RenderOptimization.GetOptimalRenderSettings(0.3);
        var normalZoom = RenderOptimization.GetOptimalRenderSettings(1.0);

        // Assert
        Assert.True(highZoom.AntiAlias);
        Assert.False(lowZoom.AntiAlias);
        Assert.True(normalZoom.AntiAlias);
    }

    [Fact]
    public void RenderOptimization_ShouldRender_ShouldFilterSmallRects()
    {
        // Arrange
        var largeRect = new SKRect(0, 0, 100, 100);
        var smallRect = new SKRect(0, 0, 0.5f, 0.5f);

        // Act & Assert
        Assert.True(RenderOptimization.ShouldRender(largeRect));
        Assert.False(RenderOptimization.ShouldRender(smallRect));
    }

    [Fact]
    public void RenderOptimization_IsPointVisible_ShouldCheckBounds()
    {
        // Arrange
        var bounds = new SKRect(0, 0, 100, 100);

        // Act & Assert
        Assert.True(RenderOptimization.IsPointVisible(50, 50, bounds));
        Assert.False(RenderOptimization.IsPointVisible(200, 200, bounds));
        Assert.True(RenderOptimization.IsPointVisible(105, 50, bounds, 10)); // Within margin
    }

    // RenderBatcher Tests
    [Fact]
    public void RenderBatcher_ShouldBatchDrawCalls()
    {
        // Arrange
        var batcher = new RenderBatcher();
        var callCount = 0;

        // Act
        batcher.BeginBatch();
        batcher.AddDrawCall(_ => callCount++, 0);
        batcher.AddDrawCall(_ => callCount++, 1);
        batcher.AddDrawCall(_ => callCount++, 2);

        using var surface = SKSurface.Create(new SKImageInfo(100, 100));
        batcher.ExecuteBatch(surface.Canvas);

        // Assert
        Assert.Equal(3, callCount);
    }

    // PerformanceProfiler Tests
    [Fact]
    public void PerformanceProfiler_ShouldTrackOperations()
    {
        // Arrange
        var profiler = new PerformanceProfiler();

        // Act
        profiler.StartOperation("TestOp");
        Thread.Sleep(10);
        profiler.EndOperation("TestOp");

        // Assert
        var result = profiler.GetResults("TestOp");
        Assert.NotNull(result);
        Assert.Equal("TestOp", result.Name);
        Assert.Equal(1, result.CallCount);
        Assert.True(result.TotalTime.TotalMilliseconds >= 10);
    }

    [Fact]
    public void PerformanceProfiler_ProfileScope_ShouldAutoTrack()
    {
        // Arrange
        var profiler = new PerformanceProfiler();

        // Act
        using (profiler.Profile("ScopedOp"))
        {
            Thread.Sleep(5);
        }

        // Assert
        var result = profiler.GetResults("ScopedOp");
        Assert.NotNull(result);
        Assert.Equal(1, result.CallCount);
    }

    [Fact]
    public void PerformanceProfiler_ShouldTrackMinMaxAverage()
    {
        // Arrange
        var profiler = new PerformanceProfiler();

        // Act
        for (int i = 0; i < 5; i++)
        {
            profiler.StartOperation("MultiOp");
            Thread.Sleep(i * 2); // Varying durations
            profiler.EndOperation("MultiOp");
        }

        // Assert
        var result = profiler.GetResults("MultiOp");
        Assert.NotNull(result);
        Assert.Equal(5, result.CallCount);
        Assert.True(result.MinDuration < result.MaxDuration);
        Assert.True(result.AverageTime.TotalMilliseconds > 0);
    }

    [Fact]
    public void FrameRateMonitor_ShouldTrackFps()
    {
        // Arrange
        var monitor = new FrameRateMonitor();

        // Act
        for (int i = 0; i < 10; i++)
        {
            monitor.RecordFrame();
            Thread.Sleep(16); // ~60 FPS
        }

        // Assert
        Assert.InRange(monitor.CurrentFps, 30, 70); // Allow some variance
        Assert.InRange(monitor.AverageFrameTime, 10, 30);
    }

    // Helper methods
    private static List<DataPoint> GenerateTestPoints(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            points.Add(new DataPoint(i, Math.Sin(i * 0.1) * 50));
        }
        return points;
    }

    private static List<DataPoint> GenerateTestPointsWithPeaks(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            double y = Math.Sin(i * 0.1) * 50;
            if (i == count / 2)
                y = 100; // Add a peak in the middle
            points.Add(new DataPoint(i, y));
        }
        return points;
    }

    private static List<DataPoint> GenerateZigzagPoints(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            double y = i % 2 == 0 ? 0 : 10;
            points.Add(new DataPoint(i, y));
        }
        return points;
    }
}
