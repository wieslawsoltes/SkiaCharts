using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class WaterfallChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new WaterfallChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.True(chart.Configuration.StartFromZero);
        Assert.True(chart.DefaultStyle.ShowConnectorLines);
        Assert.Equal(0.7f, chart.DefaultStyle.BarWidthRatio);
    }

    [Fact]
    public void BasicWaterfall_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),   // Starting value
            new DataPoint(1, 50),    // Increase
            new DataPoint(2, -30),   // Decrease
            new DataPoint(3, 20),    // Increase
            new DataPoint(4, -10)    // Decrease
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void WaterfallWithTotalBars_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),   // Starting value
            new DataPoint(1, 50),    // Increase
            new DataPoint(2, -30),   // Decrease
            new DataPoint(3, 0),     // Subtotal (configured as total)
            new DataPoint(4, 20),    // Increase
            new DataPoint(5, 0)      // Final total (configured as total)
        });
        chart.Series.Add(series);

        var configs = new List<WaterfallBarConfiguration>
        {
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Negative },
            new() { IsTotal = true, Label = "Subtotal" },
            new() { BarType = WaterfallBarType.Positive },
            new() { IsTotal = true, Label = "Total" }
        };
        chart.SetBarConfigurations(series, configs);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void WaterfallWithCategoryLabels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.Configuration.CategoryLabels = new[] { "Start", "Sales", "Returns", "End" };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1000),
            new DataPoint(1, 500),
            new DataPoint(2, -200),
            new DataPoint(3, 0)
        });
        chart.Series.Add(series);

        var configs = new List<WaterfallBarConfiguration>
        {
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Negative },
            new() { IsTotal = true }
        };
        chart.SetBarConfigurations(series, configs);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ConnectorLines_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.ShowConnectorLines = true;
        chart.DefaultStyle.ConnectorLineColor = SKColors.Gray;
        chart.DefaultStyle.ConnectorLineWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ConnectorLines_Disabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.ShowConnectorLines = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ConnectorLines_DashedPattern_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.ShowConnectorLines = true;
        chart.DefaultStyle.ConnectorDashPattern = new[] { 10f, 5f };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomColors_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.PositiveColor = SKColors.LightGreen;
        chart.DefaultStyle.NegativeColor = SKColors.LightCoral;
        chart.DefaultStyle.TotalColor = SKColors.LightBlue;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30),
            new DataPoint(3, 0)
        });
        chart.Series.Add(series);

        var configs = new List<WaterfallBarConfiguration>
        {
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Negative },
            new() { IsTotal = true }
        };
        chart.SetBarConfigurations(series, configs);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void RoundedCorners_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.CornerRadius = 5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BarBorder_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ValueLabels_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.ShowValueLabels = true;
        chart.DefaultStyle.ValueLabelFontSize = 12f;
        chart.DefaultStyle.ValueLabelColor = SKColors.Black;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void StartFromZero_True_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.Configuration.StartFromZero = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void StartFromZero_False_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.Configuration.StartFromZero = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomBarWidth_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.DefaultStyle.BarWidthRatio = 0.5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void RotatedCategoryLabels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();
        chart.Configuration.CategoryLabels = new[] { "January", "February", "March" };
        chart.Configuration.CategoryLabelRotation = -45f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, -30)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void OnlyPositiveValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, 30),
            new DataPoint(3, 20)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void OnlyNegativeValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, -100),
            new DataPoint(1, -50),
            new DataPoint(2, -30),
            new DataPoint(3, -20)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedPositiveNegative_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, -50),
            new DataPoint(2, 30),
            new DataPoint(3, -20),
            new DataPoint(4, 40)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SingleBar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new WaterfallChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new WaterfallChart();

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void LargeDataset_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 50; i++)
        {
            points.Add(new DataPoint(i, (i % 2 == 0 ? 1 : -1) * (10 + i * 2)));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(1600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 1600, 600);
    }

    [Fact]
    public void MultipleTotalBars_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new WaterfallChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50),
            new DataPoint(2, 0),    // First total
            new DataPoint(3, -30),
            new DataPoint(4, 20),
            new DataPoint(5, 0)     // Second total
        });
        chart.Series.Add(series);

        var configs = new List<WaterfallBarConfiguration>
        {
            new() { BarType = WaterfallBarType.Positive },
            new() { BarType = WaterfallBarType.Positive },
            new() { IsTotal = true, Label = "Q1" },
            new() { BarType = WaterfallBarType.Negative },
            new() { BarType = WaterfallBarType.Positive },
            new() { IsTotal = true, Label = "Q2" }
        };
        chart.SetBarConfigurations(series, configs);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new WaterfallChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50)
        });

        var style = new WaterfallSeriesStyle
        {
            PositiveColor = SKColors.DarkGreen,
            NegativeColor = SKColors.DarkRed,
            BarWidthRatio = 0.8f
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.DarkGreen, retrievedStyle.PositiveColor);
        Assert.Equal(SKColors.DarkRed, retrievedStyle.NegativeColor);
        Assert.Equal(0.8f, retrievedStyle.BarWidthRatio);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new WaterfallChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void SetBarConfigurations_ShouldStoreConfigurations()
    {
        // Arrange
        var chart = new WaterfallChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50)
        });

        var configs = new List<WaterfallBarConfiguration>
        {
            new() { BarType = WaterfallBarType.Positive, Label = "Sales" },
            new() { BarType = WaterfallBarType.Positive, Label = "Revenue" }
        };

        // Act
        chart.SetBarConfigurations(series, configs);
        var retrievedConfigs = chart.GetBarConfigurations(series);

        // Assert
        Assert.NotNull(retrievedConfigs);
        Assert.Equal(2, retrievedConfigs.Count);
        Assert.Equal("Sales", retrievedConfigs[0].Label);
        Assert.Equal("Revenue", retrievedConfigs[1].Label);
    }

    [Fact]
    public void GetBarConfigurations_ShouldReturnNullForUnconfiguredSeries()
    {
        // Arrange
        var chart = new WaterfallChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 50)
        });

        // Act
        var configs = chart.GetBarConfigurations(series);

        // Assert
        Assert.Null(configs);
    }
}
