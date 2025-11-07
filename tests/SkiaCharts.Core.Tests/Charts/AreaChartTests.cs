using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class AreaChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new AreaChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(SKColors.Blue, chart.DefaultStyle.FillColor);
        Assert.Equal(100, chart.DefaultStyle.FillAlpha);
        Assert.Equal(AreaStackMode.None, chart.Configuration.StackMode);
        Assert.True(chart.DefaultStyle.ShowLine);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new AreaChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20)
        });
        var style = new AreaSeriesStyle
        {
            FillColor = SKColors.Red,
            FillAlpha = 150
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Red, retrievedStyle.FillColor);
        Assert.Equal(150, retrievedStyle.FillAlpha);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new AreaChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void AreaMode_Linear_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.AreaMode = AreaMode.Linear;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AreaMode_Stepped_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.AreaMode = AreaMode.Stepped;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AreaMode_Smooth_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.AreaMode = AreaMode.Smooth;
        chart.DefaultStyle.SmoothTension = 0.5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void StackMode_None_MultipleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.Configuration.StackMode = AreaStackMode.None;

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 15),
            new DataPoint(1, 10),
            new DataPoint(2, 20)
        });

        chart.SetSeriesStyle(series1, new AreaSeriesStyle
        {
            FillColor = SKColors.Blue,
            FillAlpha = 100
        });

        chart.SetSeriesStyle(series2, new AreaSeriesStyle
        {
            FillColor = SKColors.Red,
            FillAlpha = 100
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void StackMode_Stacked_MultipleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.Configuration.StackMode = AreaStackMode.Stacked;

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 15),
            new DataPoint(1, 10),
            new DataPoint(2, 20)
        });

        chart.SetSeriesStyle(series1, new AreaSeriesStyle { FillColor = SKColors.Blue, FillAlpha = 150 });
        chart.SetSeriesStyle(series2, new AreaSeriesStyle { FillColor = SKColors.Red, FillAlpha = 150 });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void TransparencyAlpha_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.FillAlpha = 50; // Very transparent

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GradientFill_Vertical_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.GradientColors = new[]
        {
            SKColors.Blue,
            SKColors.LightBlue
        };
        chart.DefaultStyle.GradientDirection = GradientDirection.Vertical;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GradientFill_Horizontal_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.GradientColors = new[]
        {
            SKColors.Red,
            SKColors.Orange
        };
        chart.DefaultStyle.GradientDirection = GradientDirection.Horizontal;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GradientFill_Radial_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.GradientColors = new[]
        {
            SKColors.Green,
            SKColors.LightGreen
        };
        chart.DefaultStyle.GradientDirection = GradientDirection.Radial;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void BoundaryLine_Disabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.ShowLine = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void DashedBoundaryLine_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.DashPattern = new[] { 10f, 5f };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void Baseline_CustomValue_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.Baseline = 10.0; // Custom baseline

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 15),
            new DataPoint(1, 25),
            new DataPoint(2, 20)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void NegativeValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, -10),
            new DataPoint(1, 20),
            new DataPoint(2, -5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new AreaChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SinglePoint_ShouldNotRender()
    {
        // Arrange
        var chart = new AreaChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(5, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw, but area needs at least 2 points)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void TwoPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AllAreaModes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var mode in Enum.GetValues<AreaMode>())
        {
            var chart = new AreaChart();
            chart.DefaultStyle.AreaMode = mode;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 10),
                new DataPoint(1, 20),
                new DataPoint(2, 15),
                new DataPoint(3, 25),
                new DataPoint(4, 18)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }

    [Fact]
    public void AllGradientDirections_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var direction in Enum.GetValues<GradientDirection>())
        {
            var chart = new AreaChart();
            chart.DefaultStyle.GradientColors = new[]
            {
                SKColors.Blue,
                SKColors.LightBlue
            };
            chart.DefaultStyle.GradientDirection = direction;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 10),
                new DataPoint(1, 20),
                new DataPoint(2, 15)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }

    [Fact]
    public void ThreeSeries_Stacked_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.Configuration.StackMode = AreaStackMode.Stacked;

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10),
            new DataPoint(2, 8)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 8),
            new DataPoint(1, 12),
            new DataPoint(2, 10)
        });

        var series3 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 8),
            new DataPoint(2, 12)
        });

        chart.SetSeriesStyle(series1, new AreaSeriesStyle { FillColor = SKColors.Red, FillAlpha = 150 });
        chart.SetSeriesStyle(series2, new AreaSeriesStyle { FillColor = SKColors.Green, FillAlpha = 150 });
        chart.SetSeriesStyle(series3, new AreaSeriesStyle { FillColor = SKColors.Blue, FillAlpha = 150 });

        chart.Series.Add(series1);
        chart.Series.Add(series2);
        chart.Series.Add(series3);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SmoothTension_ShouldClampToValidRange()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.AreaMode = AreaMode.Smooth;
        chart.DefaultStyle.SmoothTension = 1.5f; // Out of range

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should clamp internally and not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void CombinedFeatures_GradientAndDashedLine_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();
        chart.DefaultStyle.GradientColors = new[]
        {
            new SKColor(0, 100, 255),
            new SKColor(0, 200, 255)
        };
        chart.DefaultStyle.GradientDirection = GradientDirection.Vertical;
        chart.DefaultStyle.DashPattern = new[] { 5f, 3f };
        chart.DefaultStyle.LineColor = SKColors.Navy;
        chart.DefaultStyle.LineWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),
            new DataPoint(1, 20),
            new DataPoint(2, 15),
            new DataPoint(3, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new AreaChart();

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ZeroValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new AreaChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 20),
            new DataPoint(2, 0),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }
}
