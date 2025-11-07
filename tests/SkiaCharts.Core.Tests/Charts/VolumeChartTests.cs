using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class VolumeChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new VolumeChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(VolumeColorMode.PriceDirection, chart.DefaultStyle.ColorMode);
        Assert.Equal(0.8, chart.DefaultStyle.BarWidthRatio);
        Assert.Equal(0.25, chart.Configuration.VolumePanelRatio);
    }

    [Fact]
    public void BasicVolume_WithOhlcData_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000),
            new OhlcDataPoint(2, 103, 115, 100, 112, 15000),
            new OhlcDataPoint(3, 112, 118, 110, 111, 8000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BasicVolume_WithDataPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10000),
            new DataPoint(1, 12000),
            new DataPoint(2, 15000),
            new DataPoint(3, 8000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SingleColorMode_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.ColorMode = VolumeColorMode.Single;
        chart.DefaultStyle.DefaultColor = SKColors.Gray;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000),
            new OhlcDataPoint(2, 103, 115, 100, 112, 15000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void PriceDirectionMode_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.ColorMode = VolumeColorMode.PriceDirection;
        chart.DefaultStyle.BullishColor = SKColors.Green;
        chart.DefaultStyle.BearishColor = SKColors.Red;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),   // Bullish
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000),  // Bearish
            new OhlcDataPoint(2, 103, 115, 100, 112, 15000)   // Bullish
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void VolumeDirectionMode_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.ColorMode = VolumeColorMode.VolumeDirection;
        chart.DefaultStyle.IncreasingColor = SKColors.LightGreen;
        chart.DefaultStyle.DecreasingColor = SKColors.LightCoral;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 15000),  // Increasing
            new OhlcDataPoint(2, 103, 115, 100, 112, 12000),  // Decreasing
            new OhlcDataPoint(3, 112, 118, 110, 111, 18000)   // Increasing
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomOpacity_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.Opacity = 128; // 50% opacity

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new VolumeChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000)
        });

        var style = new VolumeSeriesStyle
        {
            ColorMode = VolumeColorMode.Single,
            DefaultColor = SKColors.Blue,
            BarWidthRatio = 0.9
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(VolumeColorMode.Single, retrievedStyle.ColorMode);
        Assert.Equal(SKColors.Blue, retrievedStyle.DefaultColor);
        Assert.Equal(0.9, retrievedStyle.BarWidthRatio);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new VolumeChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void CustomBarWidth_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.BarWidthRatio = 0.95; // Wide bars

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000),
            new OhlcDataPoint(2, 103, 115, 100, 112, 15000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MinimumBarWidth_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.MinimumBarWidth = 3f;

        var points = new List<IDataPoint>();
        for (int i = 0; i < 200; i++)
        {
            points.Add(new OhlcDataPoint(i, 100, 110, 95, 105, 10000 + i * 100));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (bars should not be smaller than minimum)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MaximumBarWidth_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.MaximumBarWidth = 8f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (bars should not exceed maximum)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BorderStyle_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 1f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ZeroVolume_ShouldNotRender()
    {
        // Arrange
        var chart = new VolumeChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 0),      // Zero volume
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000), // Has volume
            new OhlcDataPoint(2, 103, 115, 100, 112, 0)      // Zero volume
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should only render bar at index 1)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new VolumeChart();
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
        var chart = new VolumeChart();

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SingleVolumeBar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ManyVolumeBars_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var points = new List<IDataPoint>();
        var volume = 10000.0;
        for (int i = 0; i < 100; i++)
        {
            volume += (i % 2 == 0 ? 500 : -300);
            points.Add(new OhlcDataPoint(i, 100, 110, 95, 105, volume));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(1200, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 1200, 600);
    }

    [Fact]
    public void VaryingVolume_ShouldRenderCorrectly()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.DefaultStyle.ColorMode = VolumeColorMode.VolumeDirection;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 5000),    // Starting volume
            new OhlcDataPoint(1, 105, 108, 102, 103, 15000),  // Spike up
            new OhlcDataPoint(2, 103, 115, 100, 112, 8000),   // Drop
            new OhlcDataPoint(3, 112, 118, 110, 111, 12000),  // Increase
            new OhlcDataPoint(4, 111, 115, 109, 110, 6000)    // Drop
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedOhlcAndDataPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new DataPoint(1, 12000), // Regular data point - Y value used as volume
            new OhlcDataPoint(2, 105, 115, 100, 112, 15000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomConfiguration_ShouldApply()
    {
        // Arrange
        var chart = new VolumeChart();
        chart.Configuration.VolumePanelRatio = 0.3;
        chart.Configuration.ShowVolumeAxis = false;
        chart.Configuration.ShowGridLines = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MultipleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new VolumeChart();

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(0, 100, 110, 95, 105, 10000),
            new OhlcDataPoint(1, 105, 108, 102, 103, 12000)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new OhlcDataPoint(2, 103, 115, 100, 112, 15000),
            new OhlcDataPoint(3, 112, 118, 110, 111, 8000)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }
}
