using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Layout;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Tests.Integration;

/// <summary>
/// Integration tests for complete chart workflows.
/// </summary>
public class IntegrationTests
{
    [Fact]
    public void LinearAxis_ShouldWorkWithDataSeries()
    {
        // Arrange
        var points = new List<DataPoint>();
        for (int i = 0; i < 10; i++)
        {
            points.Add(new DataPoint(i, i * i));
        }
        var series = new DataSeries<DataPoint>(points);

        var axis = new LinearAxis
        {
            Title = "X Axis",
            Position = AxisPosition.Bottom
        };

        // Act
        var dataRange = new DataRange(series.MinX, series.MaxX);
        var optimalRange = axis.CalculateOptimalRange(dataRange);
        axis.VisibleRange = optimalRange;
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.True(optimalRange.Min <= series.MinX);
        Assert.True(optimalRange.Max >= series.MaxX);
    }

    [Fact]
    public void DateTimeAxis_ShouldWorkWithTimeSeriesData()
    {
        // Arrange
        var points = new List<DataPoint>();
        var startDate = new DateTime(2024, 1, 1);
        for (int i = 0; i < 30; i++)
        {
            var date = startDate.AddDays(i);
            points.Add(new DataPoint(date.ToOADate(), Math.Sin(i * 0.1) * 100));
        }
        var series = new DataSeries<DataPoint>(points);

        var axis = new DateTimeAxis
        {
            Title = "Date",
            Position = AxisPosition.Bottom
        };

        // Act
        var dataRange = new DataRange(series.MinX, series.MaxX);
        axis.VisibleRange = dataRange;
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.NotEmpty(ticks);
        Assert.Equal(30, series.Count);

        // Verify tick formatting includes date information
        Assert.All(ticks, tick => Assert.NotEmpty(tick.Label));
    }

    [Fact]
    public void ObservableDataSeries_ShouldUpdateBoundsOnAdd()
    {
        // Arrange
        var series = new ObservableDataSeries<DataPoint>();
        bool boundsChanged = false;
        series.CollectionChanged += (s, e) => boundsChanged = true;

        // Act
        series.Add(new DataPoint(0, 0));
        series.Add(new DataPoint(10, 100));

        // Assert
        Assert.True(boundsChanged);
        Assert.Equal(2, series.Count);
        Assert.InRange(series.MinX, -1, 1);
        Assert.InRange(series.MaxY, 99, 101);
    }

    [Fact]
    public void ViewportManager_ShouldTransformCoordinatesCorrectly()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            ScreenRect = new SKRect(0, 0, 800, 600)
        };
        viewport.FitToRange(new DataRange(0, 100), new DataRange(0, 100));

        // Act
        var screenPoint = viewport.DataToScreen(50, 50);
        var (dataX, dataY) = viewport.ScreenToData(screenPoint.X, screenPoint.Y);

        // Assert - Round-trip should return original values
        Assert.InRange(dataX, 49, 51);
        Assert.InRange(dataY, 49, 51);
    }

    [Fact]
    public void MultiSeriesData_ShouldCalculateCombinedBounds()
    {
        // Arrange
        var points1 = new List<DataPoint>();
        for (int i = 0; i < 10; i++)
        {
            points1.Add(new DataPoint(i, i));
        }
        var series1 = new DataSeries<DataPoint>(points1);

        var points2 = new List<DataPoint>();
        for (int i = 0; i < 10; i++)
        {
            points2.Add(new DataPoint(i, i * 2));
        }
        var series2 = new DataSeries<DataPoint>(points2);

        // Act
        var xMin = Math.Min(series1.MinX, series2.MinX);
        var xMax = Math.Max(series1.MaxX, series2.MaxX);
        var yMin = Math.Min(series1.MinY, series2.MinY);
        var yMax = Math.Max(series1.MaxY, series2.MaxY);

        // Assert
        Assert.InRange(xMin, -1, 1);
        Assert.InRange(xMax, 9, 10);
        Assert.InRange(yMin, -1, 1);
        Assert.InRange(yMax, 18, 19); // Max from series2 (9 * 2)
    }

    [Fact]
    public void LayoutEngine_ShouldCalculateCorrectPlotArea()
    {
        // Arrange
        var layout = new LayoutEngine
        {
            Width = 800,
            Height = 600,
            Padding = new Padding(10)
        };

        var config = new LayoutConfiguration
        {
            HasTitle = true,
            TitleHeight = 40,
            HasLeftAxis = true,
            LeftAxisWidth = 60,
            HasBottomAxis = true,
            BottomAxisHeight = 50
        };

        // Act
        layout.Calculate(config);

        // Assert
        Assert.True(layout.PlotArea.Width > 0);
        Assert.True(layout.PlotArea.Height > 0);
        Assert.NotNull(layout.TitleArea);
        Assert.NotNull(layout.LeftAxisArea);
        Assert.NotNull(layout.BottomAxisArea);

        // Verify plot area is inside the canvas bounds
        Assert.InRange(layout.PlotArea.Left, 0, 800);
        Assert.InRange(layout.PlotArea.Top, 0, 600);
        Assert.InRange(layout.PlotArea.Right, 0, 800);
        Assert.InRange(layout.PlotArea.Bottom, 0, 600);
    }

    [Fact]
    public void CircularBuffer_ShouldWorkWithRealTimeData()
    {
        // Arrange
        var buffer = new CircularBuffer<DataPoint>(100);

        // Act - Simulate real-time data streaming
        for (int i = 0; i < 150; i++) // More than capacity
        {
            buffer.Add(new DataPoint(i, Math.Sin(i * 0.1)));
        }

        var points = new List<DataPoint>();
        foreach (var point in buffer)
        {
            points.Add(point);
        }
        var series = new DataSeries<DataPoint>(points);

        // Assert
        Assert.Equal(100, buffer.Count); // Should maintain capacity
        Assert.Equal(100, series.Count);

        // Should contain most recent data (50-149)
        Assert.InRange(series[0].X, 49, 51);
        Assert.InRange(series[99].X, 148, 150);
    }

    [Fact]
    public void AxisAutoScaling_ShouldProduceNiceBounds()
    {
        // Arrange
        var axis = new LinearAxis();
        var points = new List<DataPoint>();
        for (int i = 0; i < 20; i++)
        {
            points.Add(new DataPoint(i, i * 3.7 + 5.3)); // Irregular values
        }
        var data = new DataSeries<DataPoint>(points);

        // Act
        var dataRange = new DataRange(data.MinY, data.MaxY);
        var optimalRange = axis.CalculateOptimalRange(dataRange);
        axis.VisibleRange = optimalRange;
        var ticks = axis.GenerateTicks();

        // Assert
        Assert.True(optimalRange.Span >= dataRange.Span); // Should have padding
        Assert.NotEmpty(ticks);

        // Ticks should be at "nice" intervals
        if (ticks.Count > 1)
        {
            var interval = ticks[1].Value - ticks[0].Value;
            // Interval should be a nice number (1, 2, 5, 10, 20, 50, etc.)
            Assert.True(IsNiceNumber(interval));
        }
    }

    [Fact]
    public void CategoryAxis_ShouldWorkWithBarChartScenario()
    {
        // Arrange - Simulate bar chart categories
        var categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        var axis = new CategoryAxis(categories)
        {
            Position = AxisPosition.Bottom
        };

        var values = new[] { 45, 67, 89, 56, 78, 92 };
        var points = new List<DataPoint>();
        for (int i = 0; i < categories.Length; i++)
        {
            points.Add(new DataPoint(i, values[i]));
        }
        var series = new DataSeries<DataPoint>(points);

        // Act
        var ticks = axis.GenerateTicks();
        var optimalRange = axis.CalculateOptimalRange(new DataRange(0, categories.Length - 1));

        // Assert
        Assert.Equal(categories.Length, ticks.Count);
        Assert.Equal(6, series.Count);

        // Range should be centered for bars
        Assert.Equal(-0.5, optimalRange.Min);
        Assert.Equal(5.5, optimalRange.Max);
    }

    [Fact]
    public void RenderQueue_ShouldMaintainLayerOrder()
    {
        // Arrange
        var queue = new RenderQueue();
        var renderOrder = new List<RenderLayer>();

        var mockBackground = new MockRenderable(RenderLayer.Background);
        var mockData = new MockRenderable(RenderLayer.Data);
        var mockOverlay = new MockRenderable(RenderLayer.Overlay);
        var mockGrid = new MockRenderable(RenderLayer.Grid);

        // Act - Add in random order
        queue.Add(mockOverlay);
        queue.Add(mockBackground);
        queue.Add(mockData);
        queue.Add(mockGrid);

        // Render and track order
        using var surface = SKSurface.Create(new SKImageInfo(100, 100));
        var context = new RenderContext(surface.Canvas, 100, 100);

        mockBackground.OnRendered = () => renderOrder.Add(RenderLayer.Background);
        mockGrid.OnRendered = () => renderOrder.Add(RenderLayer.Grid);
        mockData.OnRendered = () => renderOrder.Add(RenderLayer.Data);
        mockOverlay.OnRendered = () => renderOrder.Add(RenderLayer.Overlay);

        queue.RenderAll(context);

        // Assert - Should render in layer order
        Assert.Equal(4, renderOrder.Count);
        Assert.Equal(RenderLayer.Background, renderOrder[0]);
        Assert.Equal(RenderLayer.Grid, renderOrder[1]);
        Assert.Equal(RenderLayer.Data, renderOrder[2]);
        Assert.Equal(RenderLayer.Overlay, renderOrder[3]);
    }

    private static bool IsNiceNumber(double value)
    {
        var absValue = Math.Abs(value);
        if (absValue == 0) return true;

        // Normalize to [1, 10) range
        var exponent = Math.Floor(Math.Log10(absValue));
        var mantissa = absValue / Math.Pow(10, exponent);

        // Check if mantissa is close to 1, 2, or 5
        return Math.Abs(mantissa - 1) < 0.01 ||
               Math.Abs(mantissa - 2) < 0.01 ||
               Math.Abs(mantissa - 5) < 0.01;
    }

    private class MockRenderable : IRenderable
    {
        public MockRenderable(RenderLayer layer)
        {
            Layer = layer;
        }

        public bool IsVisible { get; set; } = true;
        public RenderLayer Layer { get; }
        public Action? OnRendered { get; set; }

        public void Render(IRenderContext context)
        {
            OnRendered?.Invoke();
        }
    }
}
