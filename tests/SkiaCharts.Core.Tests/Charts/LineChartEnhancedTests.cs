using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class LineChartEnhancedTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new LineChartEnhanced();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.Equal(SKColors.Blue, chart.DefaultStyle.LineColor);
        Assert.Equal(2f, chart.DefaultStyle.LineWidth);
        Assert.Equal(LineMode.Linear, chart.DefaultStyle.LineMode);
        Assert.Equal(MarkerShape.Circle, chart.DefaultStyle.MarkerShape);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0)
        });
        var style = new LineSeriesStyle
        {
            LineColor = SKColors.Red,
            LineWidth = 3f
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Red, retrievedStyle.LineColor);
        Assert.Equal(3f, retrievedStyle.LineWidth);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void LineMode_Linear_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.LineMode = LineMode.Linear;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void LineMode_Stepped_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.LineMode = LineMode.Stepped;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void LineMode_Smooth_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.LineMode = LineMode.Smooth;
        chart.DefaultStyle.SmoothTension = 0.5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Circle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Circle;
        chart.DefaultStyle.MarkerSize = 8f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Square_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Square;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Diamond_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Diamond;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Triangle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Triangle;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_TriangleDown_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.TriangleDown;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Cross_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Cross;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_Plus_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Plus;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void FillArea_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.FillArea = true;
        chart.DefaultStyle.FillAlpha = 100;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void FillArea_WithCustomColor_ShouldUseCustomColor()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.FillArea = true;
        chart.DefaultStyle.FillColor = SKColors.LightBlue;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void DashPattern_ShouldRenderDashedLine()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.DashPattern = new[] { 10f, 5f };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10),
            new DataPoint(2, 7)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MultiSeries_WithDifferentStyles_ShouldRenderBothSeries()
    {
        // Arrange
        var chart = new LineChartEnhanced();

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10),
            new DataPoint(2, 7)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 3),
            new DataPoint(1, 8),
            new DataPoint(2, 12)
        });

        chart.SetSeriesStyle(series1, new LineSeriesStyle
        {
            LineColor = SKColors.Red,
            MarkerShape = MarkerShape.Circle
        });

        chart.SetSeriesStyle(series2, new LineSeriesStyle
        {
            LineColor = SKColors.Green,
            MarkerShape = MarkerShape.Square,
            LineMode = LineMode.Stepped
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SinglePoint_ShouldRenderMarkerOnly()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Circle;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(5, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SmoothTension_ShouldClampToValidRange()
    {
        // Arrange
        var style = new LineSeriesStyle
        {
            SmoothTension = 1.5f, // Out of range
            LineMode = LineMode.Smooth
        };

        var chart = new LineChartEnhanced
        {
            DefaultStyle = style
        };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 10),
            new DataPoint(2, 5),
            new DataPoint(3, 15)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should clamp internally and not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerStroke_ShouldRenderMarkerOutline()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.Circle;
        chart.DefaultStyle.MarkerFillColor = SKColors.White;
        chart.DefaultStyle.MarkerStrokeColor = SKColors.Black;
        chart.DefaultStyle.MarkerStrokeWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void NoMarkers_ShouldRenderLineOnly()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.MarkerShape = MarkerShape.None;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 5),
            new DataPoint(1, 10),
            new DataPoint(2, 7)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SmoothCurve_WithTwoPoints_ShouldRenderStraightLine()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.LineMode = LineMode.Smooth;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(10, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw with only 2 points)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void FillArea_WithSinglePoint_ShouldNotThrow()
    {
        // Arrange
        var chart = new LineChartEnhanced();
        chart.DefaultStyle.FillArea = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(5, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw with single point)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AllMarkerShapes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var shape in Enum.GetValues<MarkerShape>())
        {
            if (shape == MarkerShape.None) continue;

            var chart = new LineChartEnhanced();
            chart.DefaultStyle.MarkerShape = shape;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 5),
                new DataPoint(1, 10),
                new DataPoint(2, 7)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }

    [Fact]
    public void AllLineModes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var mode in Enum.GetValues<LineMode>())
        {
            var chart = new LineChartEnhanced();
            chart.DefaultStyle.LineMode = mode;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 0),
                new DataPoint(1, 10),
                new DataPoint(2, 5),
                new DataPoint(3, 15),
                new DataPoint(4, 8)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }
}
