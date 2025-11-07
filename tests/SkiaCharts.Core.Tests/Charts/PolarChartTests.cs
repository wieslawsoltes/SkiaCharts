using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class PolarChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new PolarChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(12, chart.Configuration.AngleGridLines);
        Assert.Equal(5, chart.Configuration.RadiusGridCircles);
        Assert.True(chart.Configuration.AngleInDegrees);
        Assert.Equal(90f, chart.Configuration.StartAngle);
    }

    [Fact]
    public void BasicPolar_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(45, 2),
            new DataPoint(90, 3),
            new DataPoint(135, 2.5),
            new DataPoint(180, 1.5),
            new DataPoint(225, 2),
            new DataPoint(270, 2.5),
            new DataPoint(315, 1.8)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void Spiral_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i <= 360; i += 10)
        {
            points.Add(new DataPoint(i, i / 360.0 * 5)); // Spiral outward
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void Circle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 360; i += 15)
        {
            points.Add(new DataPoint(i, 3)); // Constant radius = circle
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void RosePattern_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 360; i += 5)
        {
            var radius = 2 + Math.Sin(i * 5 * Math.PI / 180) * 1.5; // Rose pattern
            points.Add(new DataPoint(i, radius));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void Markers_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.DefaultStyle.ShowMarkers = true;
        chart.DefaultStyle.MarkerSize = 8f;
        chart.DefaultStyle.MarkerColor = SKColors.Red;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
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
        var chart = new PolarChart();
        chart.DefaultStyle.DashPattern = new[] { 10f, 5f };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
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
        var chart = new PolarChart();

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(45, 1.8),
            new DataPoint(135, 2.2),
            new DataPoint(225, 1.3),
            new DataPoint(315, 2.1)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        chart.SetSeriesStyle(series2, new PolarSeriesStyle
        {
            LineColor = SKColors.Red,
            LineWidth = 3f
        });

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void AngleLabels_Disabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.Configuration.ShowAngleLabels = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void Clockwise_False_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.Configuration.Clockwise = false; // Counterclockwise

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
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
        var chart = new PolarChart();
        chart.Configuration.StartAngle = 0f; // Start from right (east)

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void RadiansInput_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.Configuration.AngleInDegrees = false;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),                    // 0 radians
            new DataPoint(Math.PI / 2, 2),          // π/2 radians (90°)
            new DataPoint(Math.PI, 1.5),            // π radians (180°)
            new DataPoint(3 * Math.PI / 2, 2.5)     // 3π/2 radians (270°)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void CustomMaxRadius_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.Configuration.MaxRadius = 5;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 3),
            new DataPoint(180, 2),
            new DataPoint(270, 4)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void CustomGridLines_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();
        chart.Configuration.AngleGridLines = 8; // 45-degree increments
        chart.Configuration.RadiusGridCircles = 10;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2),
            new DataPoint(180, 1.5),
            new DataPoint(270, 2.5)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new PolarChart();
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
        var chart = new PolarChart();

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void SinglePoint_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PolarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(45, 2)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new PolarChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2)
        });

        var style = new PolarSeriesStyle
        {
            LineColor = SKColors.Purple,
            LineWidth = 4f,
            ShowMarkers = true
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Purple, retrievedStyle.LineColor);
        Assert.Equal(4f, retrievedStyle.LineWidth);
        Assert.True(retrievedStyle.ShowMarkers);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new PolarChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 1),
            new DataPoint(90, 2)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }
}
