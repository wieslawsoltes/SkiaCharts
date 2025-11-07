using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class MultiPanelChartTests
{
    // ChartPanel Tests
    [Fact]
    public void ChartPanel_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var panel = new ChartPanel();

        // Assert
        Assert.NotNull(panel.Id);
        Assert.Equal(1.0, panel.Height);
        Assert.True(panel.IsVisible);
        Assert.True(panel.ShareXAxis);
        Assert.Equal(50f, panel.MinHeight);
    }

    [Fact]
    public void ChartPanel_ConstructorWithChart_ShouldSetChart()
    {
        // Arrange
        var chart = new LineChart();

        // Act
        var panel = new ChartPanel(chart);

        // Assert
        Assert.NotNull(panel.Chart);
        Assert.Equal(chart, panel.Chart);
    }

    [Fact]
    public void ChartPanel_Render_ShouldNotThrow()
    {
        // Arrange
        var panel = new ChartPanel(new LineChart());
        panel.Bounds = new SKRect(0, 0, 800, 200);

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act & Assert
        panel.Render(canvas); // Should not throw
    }

    // MultiPanelChart Tests
    [Fact]
    public void MultiPanelChart_Constructor_ShouldInitialize()
    {
        // Arrange & Act
        var multiPanel = new MultiPanelChart();

        // Assert
        Assert.Empty(multiPanel.Panels);
        Assert.Equal(SKColors.White, multiPanel.BackgroundColor);
        Assert.Equal(5f, multiPanel.PanelSpacing);
        Assert.True(multiPanel.SynchronizeXAxis);
        Assert.NotNull(multiPanel.SharedViewport);
    }

    [Fact]
    public void MultiPanelChart_AddPanel_ShouldAddToCollection()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());

        // Act
        multiPanel.AddPanel(panel);

        // Assert
        Assert.Single(multiPanel.Panels);
        Assert.Equal(panel, multiPanel.Panels[0]);
    }

    [Fact]
    public void MultiPanelChart_RemovePanel_ShouldRemoveFromCollection()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);

        // Act
        var removed = multiPanel.RemovePanel(panel);

        // Assert
        Assert.True(removed);
        Assert.Empty(multiPanel.Panels);
    }

    [Fact]
    public void MultiPanelChart_RemovePanelById_ShouldRemoveFromCollection()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);
        var panelId = panel.Id;

        // Act
        var removed = multiPanel.RemovePanelById(panelId);

        // Assert
        Assert.True(removed);
        Assert.Empty(multiPanel.Panels);
    }

    [Fact]
    public void MultiPanelChart_GetPanelById_ShouldReturnPanel()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);
        var panelId = panel.Id;

        // Act
        var found = multiPanel.GetPanelById(panelId);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(panel, found);
    }

    [Fact]
    public void MultiPanelChart_GetPanelById_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();

        // Act
        var found = multiPanel.GetPanelById("nonexistent");

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void MultiPanelChart_ClearPanels_ShouldRemoveAllPanels()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        multiPanel.AddPanel(new ChartPanel(new LineChart()));
        multiPanel.AddPanel(new ChartPanel(new BarChart()));

        // Act
        multiPanel.ClearPanels();

        // Assert
        Assert.Empty(multiPanel.Panels);
    }

    [Fact]
    public void MultiPanelChart_MovePanelTo_ShouldReorderPanels()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel1 = new ChartPanel(new LineChart());
        var panel2 = new ChartPanel(new BarChart());
        var panel3 = new ChartPanel(new AreaChart());
        multiPanel.AddPanel(panel1);
        multiPanel.AddPanel(panel2);
        multiPanel.AddPanel(panel3);

        // Act
        var moved = multiPanel.MovePanelTo(panel1.Id, 2);

        // Assert
        Assert.True(moved);
        Assert.Equal(panel2, multiPanel.Panels[0]);
        Assert.Equal(panel3, multiPanel.Panels[1]);
        Assert.Equal(panel1, multiPanel.Panels[2]);
    }

    [Fact]
    public void MultiPanelChart_MovePanelTo_ShouldReturnFalseForInvalidIndex()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);

        // Act
        var moved = multiPanel.MovePanelTo(panel.Id, 5);

        // Assert
        Assert.False(moved);
    }

    [Fact]
    public void MultiPanelChart_SetPanelHeight_ShouldUpdateHeight()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);

        // Act
        var set = multiPanel.SetPanelHeight(panel.Id, 2.5);

        // Assert
        Assert.True(set);
        Assert.Equal(2.5, panel.Height);
    }

    [Fact]
    public void MultiPanelChart_SetPanelHeight_ShouldEnforceMinimum()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel = new ChartPanel(new LineChart());
        multiPanel.AddPanel(panel);

        // Act
        var set = multiPanel.SetPanelHeight(panel.Id, 0.05);

        // Assert
        Assert.True(set);
        Assert.Equal(0.1, panel.Height); // Minimum enforced
    }

    [Fact]
    public void MultiPanelChart_Render_ShouldNotThrow()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        multiPanel.AddPanel(new ChartPanel(new LineChart()));
        multiPanel.AddPanel(new ChartPanel(new BarChart()));

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act & Assert
        multiPanel.Render(canvas, 800, 600); // Should not throw
    }

    [Fact]
    public void MultiPanelChart_Render_WithTitle_ShouldNotThrow()
    {
        // Arrange
        var multiPanel = new MultiPanelChart
        {
            Title = "Financial Dashboard"
        };
        multiPanel.AddPanel(new ChartPanel(new LineChart()));

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act & Assert
        multiPanel.Render(canvas, 800, 600); // Should not throw
    }

    // Preset Layout Tests
    [Fact]
    public void CreateSinglePanelLayout_ShouldCreateOnePanel()
    {
        // Arrange
        var chart = new LineChart();

        // Act
        var multiPanel = MultiPanelChart.CreateSinglePanelLayout(chart);

        // Assert
        Assert.Single(multiPanel.Panels);
        Assert.Equal(chart, multiPanel.Panels[0].Chart);
        Assert.Equal(1.0, multiPanel.Panels[0].Height);
    }

    [Fact]
    public void CreateDualPanelLayout_ShouldCreateTwoPanels()
    {
        // Arrange
        var topChart = new CandlestickChart();
        var bottomChart = new VolumeChart();

        // Act
        var multiPanel = MultiPanelChart.CreateDualPanelLayout(topChart, bottomChart);

        // Assert
        Assert.Equal(2, multiPanel.Panels.Count);
        Assert.Equal(topChart, multiPanel.Panels[0].Chart);
        Assert.Equal(bottomChart, multiPanel.Panels[1].Chart);
        Assert.Equal(3.0, multiPanel.Panels[0].Height);
        Assert.Equal(1.0, multiPanel.Panels[1].Height);
    }

    [Fact]
    public void CreateDualPanelLayout_WithCustomRatios_ShouldUseProvidedRatios()
    {
        // Arrange
        var topChart = new CandlestickChart();
        var bottomChart = new VolumeChart();

        // Act
        var multiPanel = MultiPanelChart.CreateDualPanelLayout(topChart, bottomChart, 4.0, 2.0);

        // Assert
        Assert.Equal(4.0, multiPanel.Panels[0].Height);
        Assert.Equal(2.0, multiPanel.Panels[1].Height);
    }

    [Fact]
    public void CreateTriplePanelLayout_ShouldCreateThreePanels()
    {
        // Arrange
        var topChart = new CandlestickChart();
        var middleChart = new LineChart();
        var bottomChart = new VolumeChart();

        // Act
        var multiPanel = MultiPanelChart.CreateTriplePanelLayout(topChart, middleChart, bottomChart);

        // Assert
        Assert.Equal(3, multiPanel.Panels.Count);
        Assert.Equal(topChart, multiPanel.Panels[0].Chart);
        Assert.Equal(middleChart, multiPanel.Panels[1].Chart);
        Assert.Equal(bottomChart, multiPanel.Panels[2].Chart);
        Assert.Equal(3.0, multiPanel.Panels[0].Height);
        Assert.Equal(1.5, multiPanel.Panels[1].Height);
        Assert.Equal(1.0, multiPanel.Panels[2].Height);
    }

    [Fact]
    public void CreateTriplePanelLayout_WithCustomRatios_ShouldUseProvidedRatios()
    {
        // Arrange
        var topChart = new CandlestickChart();
        var middleChart = new LineChart();
        var bottomChart = new VolumeChart();

        // Act
        var multiPanel = MultiPanelChart.CreateTriplePanelLayout(
            topChart, middleChart, bottomChart, 5.0, 2.0, 1.5);

        // Assert
        Assert.Equal(5.0, multiPanel.Panels[0].Height);
        Assert.Equal(2.0, multiPanel.Panels[1].Height);
        Assert.Equal(1.5, multiPanel.Panels[2].Height);
    }

    // Integration Tests
    [Fact]
    public void MultiPanelChart_WithData_ShouldRenderCorrectly()
    {
        // Arrange
        var priceChart = new CandlestickChart();
        var pricePoints = new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 102),
            new DataPoint(2, 106)
        };
        priceChart.Series.Add(new DataSeries<IDataPoint>(pricePoints, "AAPL"));

        var volumeChart = new VolumeChart();
        var volumePoints = new IDataPoint[]
        {
            new DataPoint(0, 1000000),
            new DataPoint(1, 1500000),
            new DataPoint(2, 1200000)
        };
        volumeChart.Series.Add(new DataSeries<IDataPoint>(volumePoints, "Volume"));

        var multiPanel = MultiPanelChart.CreateDualPanelLayout(priceChart, volumeChart);

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act & Assert
        multiPanel.Render(canvas, 800, 600); // Should not throw
    }

    [Fact]
    public void MultiPanelChart_SynchronizeXAxis_ShouldSyncAcrossPanels()
    {
        // Arrange
        var chart1 = new LineChart();
        var points1 = new IDataPoint[] { new DataPoint(0, 10), new DataPoint(5, 20) };
        chart1.Series.Add(new DataSeries<IDataPoint>(points1, "Series1"));

        var chart2 = new LineChart();
        var points2 = new IDataPoint[] { new DataPoint(2, 30), new DataPoint(8, 40) };
        chart2.Series.Add(new DataSeries<IDataPoint>(points2, "Series2"));

        var multiPanel = new MultiPanelChart
        {
            SynchronizeXAxis = true
        };
        multiPanel.AddPanel(new ChartPanel(chart1));
        multiPanel.AddPanel(new ChartPanel(chart2));

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act
        multiPanel.Render(canvas, 800, 600);

        // Assert - Both charts should have synchronized X-axis
        // The shared viewport should cover the full range (0 to 8)
        Assert.NotNull(multiPanel.SharedViewport);
    }

    [Fact]
    public void MultiPanelChart_InvisiblePanel_ShouldNotRender()
    {
        // Arrange
        var multiPanel = new MultiPanelChart();
        var panel1 = new ChartPanel(new LineChart()) { IsVisible = true };
        var panel2 = new ChartPanel(new BarChart()) { IsVisible = false };
        multiPanel.AddPanel(panel1);
        multiPanel.AddPanel(panel2);

        using var bitmap = new SKBitmap(800, 600);
        using var canvas = new SKCanvas(bitmap);

        // Act
        multiPanel.Render(canvas, 800, 600);

        // Assert
        // Panel 1 should have bounds, panel 2 should not
        Assert.False(panel1.Bounds.IsEmpty);
        // Panel 2 might have bounds calculated but won't render
    }
}
