using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Tests.Data;

public class DataSeriesTests
{
    [Fact]
    public void DataSeries_ShouldCalculateBounds()
    {
        // Arrange
        var points = new[]
        {
            new DataPoint(1, 10),
            new DataPoint(2, 20),
            new DataPoint(3, 15),
            new DataPoint(4, 25)
        };
        var series = new DataSeries<DataPoint>(points, "Test Series");

        // Act & Assert
        Assert.Equal(1, series.MinX);
        Assert.Equal(4, series.MaxX);
        Assert.Equal(10, series.MinY);
        Assert.Equal(25, series.MaxY);
    }

    [Fact]
    public void DataSeries_ShouldProvideCount()
    {
        // Arrange
        var points = new[]
        {
            new DataPoint(1, 10),
            new DataPoint(2, 20),
            new DataPoint(3, 15)
        };
        var series = new DataSeries<DataPoint>(points);

        // Act & Assert
        Assert.Equal(3, series.Count);
    }

    [Fact]
    public void DataSeries_ShouldAllowIndexing()
    {
        // Arrange
        var points = new[]
        {
            new DataPoint(1, 10),
            new DataPoint(2, 20),
            new DataPoint(3, 15)
        };
        var series = new DataSeries<DataPoint>(points);

        // Act & Assert
        Assert.Equal(1, series[0].X);
        Assert.Equal(20, series[1].Y);
        Assert.Equal(3, series[2].X);
    }

    [Fact]
    public void ObservableDataSeries_ShouldNotifyOnAdd()
    {
        // Arrange
        var series = new ObservableDataSeries<DataPoint>("Test");
        bool notified = false;
        series.CollectionChanged += (s, e) => notified = true;

        // Act
        series.Add(new DataPoint(1, 10));

        // Assert
        Assert.True(notified);
        Assert.Equal(1, series.Count);
    }

    [Fact]
    public void CircularBuffer_ShouldOverwriteOldData()
    {
        // Arrange
        var buffer = new CircularBuffer<int>(3);

        // Act
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);
        buffer.Add(4); // Should overwrite 1

        // Assert
        Assert.Equal(3, buffer.Count);
        Assert.Equal(2, buffer[0]);
        Assert.Equal(3, buffer[1]);
        Assert.Equal(4, buffer[2]);
    }

    [Fact]
    public void DataRange_ShouldCalculateSpan()
    {
        // Arrange
        var range = new DataRange(10, 20);

        // Act & Assert
        Assert.Equal(10, range.Span);
        Assert.Equal(15, range.Center);
    }

    [Fact]
    public void DataRange_ShouldExpandToValue()
    {
        // Arrange
        var range = new DataRange(10, 20);

        // Act
        var expanded = range.ExpandTo(25);

        // Assert
        Assert.Equal(10, expanded.Min);
        Assert.Equal(25, expanded.Max);
    }
}
