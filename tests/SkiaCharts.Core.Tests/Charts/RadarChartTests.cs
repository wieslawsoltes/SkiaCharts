using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class RadarChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new RadarChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.True(chart.DefaultStyle.FillArea);
        Assert.Equal(5, chart.Configuration.GridLevels);
        Assert.Equal(-90f, chart.Configuration.StartAngle);
    }

    [Fact]
    public void BasicRadar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85),
            new DataPoint(4, 75)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void RadarWithLabels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.Configuration.AxisLabels = new[] { "Speed", "Power", "Accuracy", "Defense", "Stamina" };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85),
            new DataPoint(4, 75)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void FilledArea_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.DefaultStyle.FillArea = true;
        chart.DefaultStyle.FillColor = SKColors.Blue;
        chart.DefaultStyle.FillAlpha = 128;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void FilledArea_Disabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.DefaultStyle.FillArea = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void Markers_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.DefaultStyle.ShowMarkers = true;
        chart.DefaultStyle.MarkerSize = 8f;
        chart.DefaultStyle.MarkerFillColor = SKColors.Red;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void DashedLine_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.DefaultStyle.DashPattern = new[] { 10f, 5f };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void MultipleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 60),
            new DataPoint(1, 70),
            new DataPoint(2, 85),
            new DataPoint(3, 75)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        chart.SetSeriesStyle(series2, new RadarSeriesStyle
        {
            LineColor = SKColors.Red,
            FillColor = SKColors.Red,
            FillAlpha = 100
        });

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void CustomGridLevels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.Configuration.GridLevels = 10;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void SpokeLines_Disabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.Configuration.ShowSpokeLines = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void CustomStartAngle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.Configuration.StartAngle = 0f; // Start from right

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void CustomMinMaxValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();
        chart.Configuration.MinValue = 0;
        chart.Configuration.MaxValue = 100;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 50),
            new DataPoint(1, 75),
            new DataPoint(2, 60),
            new DataPoint(3, 85)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void ThreePoints_MinimumForRadar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void TwoPoints_ShouldNotRenderSeries()
    {
        // Arrange
        var chart = new RadarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (should render grid but not series)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new RadarChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new RadarChart();

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void ManyAxes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new RadarChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 12; i++)
        {
            points.Add(new DataPoint(i, 50 + Math.Sin(i) * 30));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new RadarChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70)
        });

        var style = new RadarSeriesStyle
        {
            LineColor = SKColors.Green,
            FillColor = SKColors.Green,
            LineWidth = 3f
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Green, retrievedStyle.LineColor);
        Assert.Equal(SKColors.Green, retrievedStyle.FillColor);
        Assert.Equal(3f, retrievedStyle.LineWidth);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new RadarChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 90),
            new DataPoint(2, 70)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }
}
