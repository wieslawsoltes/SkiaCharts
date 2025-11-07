using SkiaCharts.Core.Data;
using SkiaCharts.Core.Performance;
using Xunit;

namespace SkiaCharts.Core.Tests.Performance;

public class LargeDatasetTests
{
    // Data Aggregation Tests
    [Fact]
    public void DataAggregation_Aggregate_ShouldReduceDataCount()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);

        // Act
        var aggregated = DataAggregation.Aggregate(points, 100, AggregationMethod.Average);

        // Assert
        Assert.Equal(100, aggregated.Count);
    }

    [Fact]
    public void DataAggregation_AggregateOHLC_ShouldPreserveExtremes()
    {
        // Arrange
        var points = GenerateLargeDataset(1000);

        // Act
        var ohlc = DataAggregation.AggregateOHLC(points, 10);

        // Assert
        Assert.Equal(10, ohlc.Count);
        Assert.All(ohlc, candle =>
        {
            Assert.True(candle.High >= candle.Low);
            Assert.True(candle.Open >= candle.Low && candle.Open <= candle.High);
            Assert.True(candle.Close >= candle.Low && candle.Close <= candle.High);
        });
    }

    [Fact]
    public void DataAggregation_MinMaxAggregate_ShouldReturnPairs()
    {
        // Arrange
        var points = GenerateLargeDataset(1000);

        // Act
        var minMax = DataAggregation.AggregateMinMax(points, 50);

        // Assert
        Assert.Equal(100, minMax.Count); // 50 bins * 2 points per bin
    }

    // LTTB Tests
    [Fact]
    public void LTTB_Downsample_ShouldReduceDataCount()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);

        // Act
        var downsampled = LargestTriangleThreeBuckets.Downsample(points, 500);

        // Assert
        Assert.Equal(500, downsampled.Count);
        Assert.Equal(points[0].X, downsampled[0].X); // First point preserved
        Assert.Equal(points[9999].X, downsampled[499].X); // Last point preserved
    }

    [Fact]
    public void LTTB_Downsample_LargeDataset_ShouldComplete()
    {
        // Arrange - 1 million points
        var points = GenerateLargeDataset(1_000_000);

        // Act
        var start = DateTime.Now;
        var downsampled = LargestTriangleThreeBuckets.Downsample(points, 1000);
        var duration = DateTime.Now - start;

        // Assert
        Assert.Equal(1000, downsampled.Count);
        Assert.True(duration.TotalSeconds < 5, $"Processing took {duration.TotalSeconds}s, expected < 5s");
    }

    [Fact]
    public void LTTB_EstimateThreshold_ShouldReturnReasonableValue()
    {
        // Arrange & Act
        var threshold = LargestTriangleThreeBuckets.EstimateThreshold(100000, 0.01);

        // Assert
        Assert.Equal(1000, threshold);
    }

    // Virtual Data Provider Tests
    [Fact]
    public void VirtualDataProvider_GetRange_ShouldReturnCorrectSubset()
    {
        // Arrange
        var allData = GenerateLargeDataset(100000);
        var provider = new InMemoryDataProvider<DataPoint>(allData);
        var virtualProvider = new VirtualDataProvider<DataPoint>(provider, 1000);

        // Act
        var range = virtualProvider.GetRange(5000, 5100);

        // Assert
        Assert.Equal(100, range.Count);
        Assert.Equal(5000, range[0].X);
        Assert.Equal(5099, range[99].X);
    }

    [Fact]
    public void VirtualDataProvider_ShouldCachePages()
    {
        // Arrange
        var allData = GenerateLargeDataset(10000);
        var provider = new InMemoryDataProvider<DataPoint>(allData);
        var virtualProvider = new VirtualDataProvider<DataPoint>(provider, 1000);

        // Act
        _ = virtualProvider.GetRange(0, 2500); // Loads 3 pages

        // Assert
        Assert.Equal(3, virtualProvider.CachedPageCount);
    }

    // Background Data Processor Tests
    [Fact]
    public async Task BackgroundDataProcessor_QueueLttbDownsampling_ShouldProcess()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);
        var processor = new BackgroundDataProcessor<DataPoint>();
        processor.Start();

        List<DataPoint>? result = null;
        processor.ProcessingCompleted += (s, e) =>
        {
            result = e.Result;
        };

        // Act
        processor.QueueLttbDownsampling(points, 500);

        // Wait for processing
        await Task.Delay(500);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.Count);

        await processor.StopAsync();
    }

    [Fact]
    public async Task BackgroundDataProcessor_MultipleQueued_ShouldProcessAll()
    {
        // Arrange
        var processor = new BackgroundDataProcessor<DataPoint>();
        processor.Start();

        int completedCount = 0;
        processor.ProcessingCompleted += (s, e) =>
        {
            Interlocked.Increment(ref completedCount);
        };

        // Act
        for (int i = 0; i < 5; i++)
        {
            var points = GenerateLargeDataset(1000);
            processor.QueueLttbDownsampling(points, 100);
        }

        // Wait for all processing
        await Task.Delay(1000);

        // Assert
        Assert.Equal(5, completedCount);

        await processor.StopAsync();
    }

    // Memory Profiler Tests
    [Fact]
    public void MemoryProfiler_EstimateMemoryUsage_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var estimated = MemoryProfiler.EstimateMemoryUsage(1_000_000, 16);

        // Assert
        Assert.InRange(estimated, 20_000_000, 30_000_000); // ~24MB for 1M points
    }

    [Fact]
    public void MemoryProfiler_TakeSnapshot_ShouldRecordMemory()
    {
        // Arrange
        var profiler = new MemoryProfiler();
        // Reset creates an "Initial" snapshot, so we already have 1

        // Act
        var _ = GenerateLargeDataset(100000); // Allocate memory
        profiler.TakeSnapshot("After");

        // Assert
        Assert.Equal(2, profiler.Snapshots.Count); // Initial + After
        Assert.True(profiler.Snapshots[1].TotalMemory > profiler.Snapshots[0].TotalMemory);
    }

    [Fact]
    public void MemoryAwareDataLoader_WouldExceedMemoryLimit_ShouldDetect()
    {
        // Arrange
        var loader = new MemoryAwareDataLoader(10); // 10 MB limit

        // Act
        bool wouldExceed = loader.WouldExceedMemoryLimit(10_000_000); // 10M points

        // Assert
        Assert.True(wouldExceed);
    }

    // Data Sampling Tests
    [Fact]
    public void DataSampling_UniformSample_ShouldReduceEvenly()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);

        // Act
        var sampled = DataSampling.UniformSample(points, 1000);

        // Assert
        Assert.Equal(1000, sampled.Count);
    }

    [Fact]
    public void DataSampling_RandomSample_WithSeed_ShouldBeReproducible()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);

        // Act
        var sample1 = DataSampling.RandomSample(points, 1000, seed: 42);
        var sample2 = DataSampling.RandomSample(points, 1000, seed: 42);

        // Assert
        Assert.Equal(sample1.Count, sample2.Count);
        for (int i = 0; i < sample1.Count; i++)
        {
            Assert.Equal(sample1[i].X, sample2[i].X);
        }
    }

    [Fact]
    public void DataSampling_StratifiedSample_ShouldDistributeEvenly()
    {
        // Arrange
        var points = GenerateLargeDataset(10000);

        // Act
        var sampled = DataSampling.StratifiedSample(points, 100);

        // Assert
        Assert.Equal(100, sampled.Count);
        // Check even distribution
        double expectedGap = 10000.0 / 100;
        for (int i = 1; i < sampled.Count; i++)
        {
            double gap = sampled[i].X - sampled[i - 1].X;
            Assert.InRange(gap, expectedGap * 0.5, expectedGap * 1.5);
        }
    }

    [Fact]
    public void DataSampling_MinMaxSample_ShouldPreserveExtremes()
    {
        // Arrange
        var points = GenerateTestPointsWithExtremes(1000);

        // Act
        var sampled = DataSampling.MinMaxSample(points, 10);

        // Assert
        // Should include the extreme high point
        Assert.Contains(sampled, p => Math.Abs(p.Y - 1000) < 0.1);
        // Should include the extreme low point
        Assert.Contains(sampled, p => Math.Abs(p.Y - (-1000)) < 0.1);
    }

    [Fact]
    public void DataSampling_AdaptiveSample_ShouldVaryDensity()
    {
        // Arrange - data with varying complexity
        var points = GenerateVariableComplexityData(10000);

        // Act
        var sampled = DataSampling.AdaptiveSample(points, 1000);

        // Assert
        Assert.Equal(1000, sampled.Count);
        Assert.Equal(points[0].X, sampled[0].X); // First preserved
        Assert.Equal(points[9999].X, sampled[999].X); // Last preserved
    }

    [Fact]
    public void DataSampling_GetRecommendedStrategy_ShouldReturnAppropriate()
    {
        // Arrange & Act
        var lightSampling = DataSampling.GetRecommendedStrategy(1000, 600, true);
        var mediumSampling = DataSampling.GetRecommendedStrategy(10000, 1000, true);
        var heavySampling = DataSampling.GetRecommendedStrategy(100000, 500, true);

        // Assert
        Assert.Equal(SamplingStrategy.Uniform, lightSampling);
        Assert.Equal(SamplingStrategy.Adaptive, mediumSampling);
        Assert.Equal(SamplingStrategy.LTTB, heavySampling);
    }

    // Stress Tests
    [Fact]
    public void StressTest_ProcessOneMillion_AllStrategies()
    {
        // Arrange
        var points = GenerateLargeDataset(1_000_000);

        // Act & Assert - All should complete without error
        var lttb = LargestTriangleThreeBuckets.Downsample(points, 1000);
        Assert.Equal(1000, lttb.Count);

        var aggregated = DataAggregation.Aggregate(points, 1000);
        Assert.Equal(1000, aggregated.Count);

        var uniform = DataSampling.UniformSample(points, 1000);
        Assert.Equal(1000, uniform.Count);
    }

    // Helper methods
    private static List<DataPoint> GenerateLargeDataset(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            points.Add(new DataPoint(i, Math.Sin(i * 0.01) * 100));
        }
        return points;
    }

    private static List<DataPoint> GenerateTestPointsWithExtremes(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            double y = Math.Sin(i * 0.1) * 50;
            if (i == count / 3)
                y = 1000; // Extreme high
            if (i == 2 * count / 3)
                y = -1000; // Extreme low
            points.Add(new DataPoint(i, y));
        }
        return points;
    }

    private static List<DataPoint> GenerateVariableComplexityData(int count)
    {
        var points = new List<DataPoint>(count);
        for (int i = 0; i < count; i++)
        {
            // First half: high frequency
            // Second half: low frequency
            double frequency = i < count / 2 ? 0.1 : 0.01;
            points.Add(new DataPoint(i, Math.Sin(i * frequency) * 100));
        }
        return points;
    }
}
