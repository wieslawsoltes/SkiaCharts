using SkiaCharts.Core.Data;
using SkiaCharts.Core.Streaming;
using Xunit;

namespace SkiaCharts.Core.Tests.Streaming;

public class StreamingTests
{
    // CircularDataBuffer Tests
    [Fact]
    public void CircularDataBuffer_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var buffer = new CircularDataBuffer<DataPoint>(10);

        // Assert
        Assert.Equal(10, buffer.Capacity);
        Assert.Equal(0, buffer.Count);
        Assert.True(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public void CircularDataBuffer_Add_ShouldAddPoints()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);

        // Act
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));

        // Assert
        Assert.Equal(2, buffer.Count);
        Assert.False(buffer.IsEmpty);
        Assert.False(buffer.IsFull);
    }

    [Fact]
    public void CircularDataBuffer_Add_WhenFull_ShouldOverwriteOldest()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(3);
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));
        buffer.Add(new DataPoint(2, 30));

        // Act
        buffer.Add(new DataPoint(3, 40)); // Should overwrite (0, 10)

        // Assert
        Assert.Equal(3, buffer.Count);
        Assert.True(buffer.IsFull);

        var all = buffer.GetAll();
        Assert.Equal(3, all.Count);
        Assert.Equal(1, all[0].X); // Oldest is now (1, 20)
        Assert.Equal(3, all[2].X); // Newest is (3, 40)
    }

    [Fact]
    public void CircularDataBuffer_GetAll_ShouldReturnInChronologicalOrder()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));
        buffer.Add(new DataPoint(2, 30));

        // Act
        var all = buffer.GetAll();

        // Assert
        Assert.Equal(3, all.Count);
        Assert.Equal(0, all[0].X);
        Assert.Equal(1, all[1].X);
        Assert.Equal(2, all[2].X);
    }

    [Fact]
    public void CircularDataBuffer_GetLast_ShouldReturnMostRecent()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);
        for (int i = 0; i < 5; i++)
        {
            buffer.Add(new DataPoint(i, i * 10));
        }

        // Act
        var last2 = buffer.GetLast(2);

        // Assert
        Assert.Equal(2, last2.Count);
        Assert.Equal(3, last2[0].X);
        Assert.Equal(4, last2[1].X);
    }

    [Fact]
    public void CircularDataBuffer_GetWindow_ShouldReturnPointsInRange()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(10);
        for (int i = 0; i < 10; i++)
        {
            buffer.Add(new DataPoint(i, i * 10));
        }

        // Act
        var window = buffer.GetWindow(3, 6);

        // Assert
        Assert.Equal(4, window.Count); // Points at X=3,4,5,6
        Assert.Equal(3, window[0].X);
        Assert.Equal(6, window[3].X);
    }

    [Fact]
    public void CircularDataBuffer_GetNewest_ShouldReturnLastAdded()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));

        // Act
        var newest = buffer.GetNewest();

        // Assert
        Assert.NotNull(newest);
        Assert.Equal(1, newest.X);
        Assert.Equal(20, newest.Y);
    }

    [Fact]
    public void CircularDataBuffer_GetOldest_ShouldReturnFirstInBuffer()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));

        // Act
        var oldest = buffer.GetOldest();

        // Assert
        Assert.NotNull(oldest);
        Assert.Equal(0, oldest.X);
        Assert.Equal(10, oldest.Y);
    }

    [Fact]
    public void CircularDataBuffer_Clear_ShouldRemoveAllData()
    {
        // Arrange
        var buffer = new CircularDataBuffer<DataPoint>(5);
        buffer.Add(new DataPoint(0, 10));
        buffer.Add(new DataPoint(1, 20));

        // Act
        buffer.Clear();

        // Assert
        Assert.Equal(0, buffer.Count);
        Assert.True(buffer.IsEmpty);
    }

    // StreamingDataSeries Tests
    [Fact]
    public void StreamingDataSeries_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var series = new StreamingDataSeries<DataPoint>(100, "Test");

        // Assert
        Assert.Equal("Test", series.Name);
        Assert.Equal(0, series.Count);
        Assert.Equal(100, series.Capacity);
    }

    [Fact]
    public void StreamingDataSeries_AddPoint_ShouldAddToBuffer()
    {
        // Arrange
        var series = new StreamingDataSeries<DataPoint>(10);

        // Act
        series.AddPoint(new DataPoint(0, 10));
        series.AddPoint(new DataPoint(1, 20));

        // Assert
        Assert.Equal(2, series.Count);
    }

    [Fact]
    public void StreamingDataSeries_TimeWindow_ShouldFilterPoints()
    {
        // Arrange
        var series = new StreamingDataSeries<DataPoint>(100)
        {
            TimeWindow = TimeSpan.FromSeconds(5)
        };

        // Add points spanning 10 seconds
        for (int i = 0; i < 11; i++)
        {
            series.AddPoint(new DataPoint(i, i * 10));
        }

        // Act
        var visiblePoints = series.ToList();

        // Assert
        // Should only show points from X=6 to X=10 (last 5 seconds)
        Assert.True(visiblePoints.Count <= 6);
        Assert.All(visiblePoints, p => Assert.True(p.X >= 5));
    }

    [Fact]
    public void StreamingDataSeries_PointsChanged_ShouldFireOnAdd()
    {
        // Arrange
        var series = new StreamingDataSeries<DataPoint>(10);
        bool eventFired = false;
        series.PointsChanged += (s, e) => eventFired = true;

        // Act
        series.AddPoint(new DataPoint(0, 10));

        // Assert
        Assert.True(eventFired);
    }

    // SimulatedStreamingDataSource Tests
    [Fact]
    public async Task SimulatedStreamingDataSource_Start_ShouldGenerateData()
    {
        // Arrange
        var source = new SimulatedStreamingDataSource
        {
            UpdateInterval = TimeSpan.FromMilliseconds(50),
            MaxUpdateFrequency = 0 // No throttling for test
        };

        int receivedCount = 0;
        source.DataReceived += (s, e) => receivedCount++;

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        await source.StartAsync(cts.Token);

        // Wait for streaming to complete
        await Task.Delay(250);

        // Assert
        Assert.True(receivedCount > 0);
        Assert.Equal(StreamingState.Connected, source.State);
    }

    [Fact]
    public async Task SimulatedStreamingDataSource_Pause_ShouldStopDataFlow()
    {
        // Arrange
        var source = new SimulatedStreamingDataSource
        {
            UpdateInterval = TimeSpan.FromMilliseconds(50)
        };

        int receivedCount = 0;
        source.DataReceived += (s, e) => receivedCount++;

        using var cts = new CancellationTokenSource();

        // Act
        await source.StartAsync(cts.Token);
        await Task.Delay(100);
        var countBeforePause = receivedCount;

        source.Pause();
        await Task.Delay(100);
        var countAfterPause = receivedCount;

        // Assert
        Assert.Equal(StreamingState.Paused, source.State);
        // Count should be same or very similar after pause
        Assert.True(Math.Abs(countAfterPause - countBeforePause) <= 1);

        cts.Cancel();
        await source.StopAsync();
    }

    [Fact]
    public async Task SimulatedStreamingDataSource_RateLimiting_ShouldThrottle()
    {
        // Arrange
        var source = new SimulatedStreamingDataSource
        {
            UpdateInterval = TimeSpan.FromMilliseconds(10), // Try to send 100/sec
            MaxUpdateFrequency = 10 // But limit to 10/sec
        };

        int receivedCount = 0;
        source.DataReceived += (s, e) => receivedCount++;

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        await source.StartAsync(cts.Token);
        await Task.Delay(1100);

        // Assert
        // Should receive ~10 updates in 1 second due to throttling
        Assert.InRange(receivedCount, 8, 12);

        await source.StopAsync();
    }

    // AsyncDataLoader Tests
    [Fact]
    public async Task AsyncDataLoader_LoadAsync_ShouldLoadData()
    {
        // Arrange
        var loader = new AsyncDataLoader();

        // Act
        var data = await loader.LoadAsync(async (progress, ct) =>
        {
            await Task.Delay(10, ct);
            progress.Report(0.5);
            await Task.Delay(10, ct);
            progress.Report(1.0);

            return new List<IDataPoint>
            {
                new DataPoint(0, 10),
                new DataPoint(1, 20),
                new DataPoint(2, 30)
            };
        });

        // Assert
        Assert.Equal(3, data.Count);
    }

    [Fact]
    public async Task AsyncDataLoader_ProgressChanged_ShouldFireEvents()
    {
        // Arrange
        var loader = new AsyncDataLoader();
        var progressReports = new List<double>();
        loader.ProgressChanged += (s, e) => progressReports.Add(e.ProgressPercent);

        // Act
        await loader.LoadAsync(async (progress, ct) =>
        {
            progress.Report(0.25);
            await Task.Delay(10, ct);
            progress.Report(0.50);
            await Task.Delay(10, ct);
            progress.Report(0.75);
            await Task.Delay(10, ct);
            progress.Report(1.0);

            return new List<IDataPoint>();
        });

        // Assert
        Assert.Contains(0.25, progressReports);
        Assert.Contains(0.50, progressReports);
        Assert.Contains(0.75, progressReports);
        Assert.Contains(1.0, progressReports);
    }

    [Fact]
    public async Task AsyncDataLoader_LoadCompleted_ShouldFireOnSuccess()
    {
        // Arrange
        var loader = new AsyncDataLoader();
        bool completedEventFired = false;
        int pointsLoaded = 0;

        loader.LoadCompleted += (s, e) =>
        {
            completedEventFired = true;
            pointsLoaded = e.PointsLoaded;
        };

        // Act
        await loader.LoadAsync(async (progress, ct) =>
        {
            await Task.Delay(10, ct);
            return new List<IDataPoint>
            {
                new DataPoint(0, 10),
                new DataPoint(1, 20)
            };
        });

        // Assert
        Assert.True(completedEventFired);
        Assert.Equal(2, pointsLoaded);
    }
}
