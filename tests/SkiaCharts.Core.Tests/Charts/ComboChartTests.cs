using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class ComboChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new ComboChart();

        // Assert
        Assert.NotNull(chart.Configuration);
        Assert.NotNull(chart.Configuration.PrimaryYAxis);
        Assert.NotNull(chart.Configuration.SecondaryYAxis);
        Assert.False(chart.Configuration.ShowSecondaryYAxis);
        Assert.False(chart.Configuration.SynchronizeYAxes);
    }

    [Fact]
    public void SingleLineSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedLineAndBar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var lineSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        var barSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 15),
            new DataPoint(2, 10),
            new DataPoint(3, 20)
        });

        chart.Series.Add(lineSeries);
        chart.Series.Add(barSeries);

        chart.SetSeriesConfiguration(lineSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue }
        });

        chart.SetSeriesConfiguration(barSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            BarStyle = new BarSeriesStyle { FillColor = SKColors.Green }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedLineAndArea_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var lineSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        var areaSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 15),
            new DataPoint(2, 10),
            new DataPoint(3, 20)
        });

        chart.Series.Add(areaSeries); // Add area first so it renders behind
        chart.Series.Add(lineSeries);

        chart.SetSeriesConfiguration(lineSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue, LineWidth = 2f }
        });

        chart.SetSeriesConfiguration(areaSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Area,
            AreaStyle = new AreaSeriesStyle { FillColor = SKColors.Green, FillAlpha = 100 }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void DualYAxis_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();
        chart.Configuration.ShowSecondaryYAxis = true;

        var leftSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        var rightSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1000),
            new DataPoint(1, 2000),
            new DataPoint(2, 1500),
            new DataPoint(3, 2500)
        });

        chart.Series.Add(leftSeries);
        chart.Series.Add(rightSeries);

        chart.SetSeriesConfiguration(leftSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Left,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue }
        });

        chart.SetSeriesConfiguration(rightSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Right,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Red }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void DualYAxis_MixedTypes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();
        chart.Configuration.ShowSecondaryYAxis = true;

        var lineSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        var barSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1000),
            new DataPoint(1, 2000),
            new DataPoint(2, 1500),
            new DataPoint(3, 2500)
        });

        chart.Series.Add(barSeries);
        chart.Series.Add(lineSeries);

        chart.SetSeriesConfiguration(lineSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Left,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue }
        });

        chart.SetSeriesConfiguration(barSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            YAxisSide = YAxisSide.Right,
            BarStyle = new BarSeriesStyle { FillColor = SKColors.Green }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void GetSeriesConfiguration_ShouldReturnDefaultForUnconfiguredSeries()
    {
        // Arrange
        var chart = new ComboChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10)
        });

        // Act
        var config = chart.GetSeriesConfiguration(series);

        // Assert
        Assert.NotNull(config);
        Assert.Equal(ComboSeriesType.Line, config.ChartType);
        Assert.Equal(YAxisSide.Left, config.YAxisSide);
    }

    [Fact]
    public void SetSeriesConfiguration_ShouldStoreConfiguration()
    {
        // Arrange
        var chart = new ComboChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10)
        });

        var config = new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            YAxisSide = YAxisSide.Right,
            BarStyle = new BarSeriesStyle { FillColor = SKColors.Red }
        };

        // Act
        chart.SetSeriesConfiguration(series, config);
        var retrievedConfig = chart.GetSeriesConfiguration(series);

        // Assert
        Assert.Equal(ComboSeriesType.Bar, retrievedConfig.ChartType);
        Assert.Equal(YAxisSide.Right, retrievedConfig.YAxisSide);
        Assert.Equal(SKColors.Red, retrievedConfig.BarStyle!.FillColor);
    }

    [Fact]
    public void LineWithMarkers_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle
            {
                LineColor = SKColors.Blue,
                MarkerShape = MarkerShape.Circle,
                MarkerSize = 8f
            }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BarWithBorder_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            BarStyle = new BarSeriesStyle
            {
                FillColor = SKColors.Green,
                BorderColor = SKColors.DarkGreen,
                BorderWidth = 2f
            }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void AreaWithBoundaryLine_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Area,
            AreaStyle = new AreaSeriesStyle
            {
                FillColor = SKColors.Blue,
                FillAlpha = 100,
                ShowLine = true,
                LineColor = SKColors.DarkBlue,
                LineWidth = 2f
            }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ScatterSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25),
            new DataPoint(4, 18)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Scatter,
            ScatterStyle = new ScatterSeriesStyle
            {
                MarkerShape = MarkerShape.Circle,
                MarkerSize = 10f,
                FillColor = SKColors.Red
            }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void AllChartTypes_ShouldRenderTogether()
    {
        // Arrange
        var chart = new ComboChart();

        var lineSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        var barSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 15),
            new DataPoint(2, 10),
            new DataPoint(3, 20)
        });

        var areaSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 3),
            new DataPoint(1, 8),
            new DataPoint(2, 6),
            new DataPoint(3, 12)
        });

        var scatterSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0.5, 12),
            new DataPoint(1.5, 18),
            new DataPoint(2.5, 14)
        });

        chart.Series.Add(areaSeries);
        chart.Series.Add(barSeries);
        chart.Series.Add(lineSeries);
        chart.Series.Add(scatterSeries);

        chart.SetSeriesConfiguration(areaSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Area,
            AreaStyle = new AreaSeriesStyle { FillColor = SKColors.LightBlue, FillAlpha = 80 }
        });

        chart.SetSeriesConfiguration(barSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            BarStyle = new BarSeriesStyle { FillColor = SKColors.Green }
        });

        chart.SetSeriesConfiguration(lineSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue, LineWidth = 2f }
        });

        chart.SetSeriesConfiguration(scatterSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Scatter,
            ScatterStyle = new ScatterSeriesStyle { MarkerShape = MarkerShape.Diamond, MarkerSize = 12f, FillColor = SKColors.Red }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void HorizontalBars_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();
        chart.Configuration.BarOrientation = BarOrientation.Horizontal;

        var barSeries = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        chart.Series.Add(barSeries);
        chart.SetSeriesConfiguration(barSeries, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Bar,
            BarStyle = new BarSeriesStyle { FillColor = SKColors.Blue }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new ComboChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new ComboChart();

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MultipleLinesOnDifferentAxes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();
        chart.Configuration.ShowSecondaryYAxis = true;

        var leftLine1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        var leftLine2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 12),
            new DataPoint(1, 18),
            new DataPoint(2, 14)
        });

        var rightLine = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1000),
            new DataPoint(1, 2000),
            new DataPoint(2, 1500)
        });

        chart.Series.Add(leftLine1);
        chart.Series.Add(leftLine2);
        chart.Series.Add(rightLine);

        chart.SetSeriesConfiguration(leftLine1, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Left,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Blue }
        });

        chart.SetSeriesConfiguration(leftLine2, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Left,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Green }
        });

        chart.SetSeriesConfiguration(rightLine, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            YAxisSide = YAxisSide.Right,
            LineStyle = new LineSeriesStyle { LineColor = SKColors.Red }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SteppedLineMode_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ComboChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });

        chart.Series.Add(series);
        chart.SetSeriesConfiguration(series, new ComboSeriesConfiguration
        {
            ChartType = ComboSeriesType.Line,
            LineStyle = new LineSeriesStyle
            {
                LineColor = SKColors.Blue,
                LineMode = LineMode.Stepped
            }
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }
}
