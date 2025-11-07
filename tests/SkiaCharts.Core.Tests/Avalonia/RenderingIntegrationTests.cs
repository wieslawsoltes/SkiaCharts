using Xunit;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaCharts.Core.Theming;

namespace SkiaCharts.Core.Tests.Avalonia;

/// <summary>
/// Integration tests for Avalonia rendering.
/// Note: These are basic tests since we cannot fully test rendering without Avalonia runtime.
/// </summary>
public class RenderingIntegrationTests
{
    [Fact]
    public void LineChart_CanBeCreated()
    {
        // Arrange & Act
        var chart = new LineChart();

        // Assert
        Assert.NotNull(chart);
        Assert.NotNull(chart.Series);
        Assert.Empty(chart.Series);
    }

    [Fact]
    public void LineChart_CanAddSeries()
    {
        // Arrange
        var chart = new LineChart();
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150),
            new DataPoint(3, 120)
        };
        var series = new DataSeries<IDataPoint>(points, "Test Series");

        // Act
        chart.Series.Add(series);

        // Assert
        Assert.Single(chart.Series);
        Assert.Equal("Test Series", chart.Series[0].Name);
    }

    [Fact]
    public void BarChart_CanBeCreated()
    {
        // Arrange & Act
        var chart = new BarChart();

        // Assert
        Assert.NotNull(chart);
        Assert.NotNull(chart.Series);
    }

    [Fact]
    public void PieChart_CanBeCreated()
    {
        // Arrange & Act
        var chart = new PieChart();

        // Assert
        Assert.NotNull(chart);
        Assert.NotNull(chart.Series);
    }

    [Fact]
    public void Chart_HasValidTheme()
    {
        // Arrange
        var chart = new LineChart();
        var theme = ThemePresets.Light;

        // Assert
        Assert.NotNull(theme);
        Assert.NotNull(theme.Background);
        Assert.NotNull(theme.Title);
        Assert.NotNull(theme.Grid);
    }

    [Fact]
    public void Chart_CanSetTitle()
    {
        // Arrange
        var chart = new LineChart();
        var title = "Test Chart Title";

        // Act
        chart.Title = title;

        // Assert
        Assert.Equal(title, chart.Title);
    }

    [Fact]
    public void Chart_HasAxes()
    {
        // Arrange & Act
        var chart = new LineChart();

        // Assert
        Assert.NotNull(chart.XAxis);
        Assert.NotNull(chart.YAxis);
    }

    [Fact]
    public void DataSeries_CalculatesBounds()
    {
        // Arrange
        var points = new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 200),
            new DataPoint(3, 150)
        };
        var series = new DataSeries<IDataPoint>(points, "Test");

        // Act & Assert
        Assert.Equal(1, series.MinX);
        Assert.Equal(3, series.MaxX);
        Assert.Equal(100, series.MinY);
        Assert.Equal(200, series.MaxY);
    }

    [Fact]
    public void ThemePresets_AllPresetsAvailable()
    {
        // Assert
        Assert.NotNull(ThemePresets.Light);
        Assert.NotNull(ThemePresets.Dark);
        Assert.NotNull(ThemePresets.Professional);
        Assert.NotNull(ThemePresets.HighContrast);
    }

    [Fact]
    public void Chart_SupportsMultipleSeries()
    {
        // Arrange
        var chart = new LineChart();
        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(1, 100),
            new DataPoint(2, 150)
        }, "Series 1");
        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(1, 80),
            new DataPoint(2, 120)
        }, "Series 2");

        // Act
        chart.Series.Add(series1);
        chart.Series.Add(series2);

        // Assert
        Assert.Equal(2, chart.Series.Count);
    }

    [Fact]
    public void Chart_SeriesCanBeCleared()
    {
        // Arrange
        var chart = new LineChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(1, 100)
        }, "Test");
        chart.Series.Add(series);

        // Act
        chart.Series.Clear();

        // Assert
        Assert.Empty(chart.Series);
    }

    [Fact]
    public void DataPoint_HasValidValues()
    {
        // Arrange & Act
        var point = new DataPoint(10.5, 20.7);

        // Assert
        Assert.Equal(10.5, point.X);
        Assert.Equal(20.7, point.Y);
    }

    [Fact]
    public void Chart_BackgroundColorCanBeSet()
    {
        // Arrange
        var chart = new LineChart();
        var color = SkiaSharp.SKColors.Blue;

        // Act
        chart.BackgroundColor = color;

        // Assert
        Assert.Equal(color, chart.BackgroundColor);
    }

    [Fact]
    public void Chart_HasChartArea()
    {
        // Arrange & Act
        var chart = new LineChart();

        // Assert
        Assert.NotNull(chart.ChartArea);
    }

    [Fact]
    public void Chart_HasViewport()
    {
        // Arrange & Act
        var chart = new LineChart();

        // Assert
        Assert.NotNull(chart.Viewport);
    }
}
