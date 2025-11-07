using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class ScatterChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new ScatterChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(MarkerShape.Circle, chart.DefaultStyle.MarkerShape);
        Assert.Equal(8f, chart.DefaultStyle.MarkerSize);
        Assert.Equal(SKColors.Blue, chart.DefaultStyle.FillColor);
        Assert.False(chart.Configuration.ShowConnectingLines);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new ScatterChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10)
        });
        var style = new ScatterSeriesStyle
        {
            FillColor = SKColors.Red,
            MarkerSize = 12f
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Red, retrievedStyle.FillColor);
        Assert.Equal(12f, retrievedStyle.MarkerSize);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new ScatterChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void BasicScatter_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();

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

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AllMarkerShapes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var shape in Enum.GetValues<MarkerShape>())
        {
            if (shape == MarkerShape.None) continue;

            var chart = new ScatterChart();
            chart.DefaultStyle.MarkerShape = shape;

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
    public void MarkerBorder_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 2f;

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
    public void VariableMarkerSizes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseVariableSizes = true;
        chart.DefaultStyle.MinMarkerSize = 4f;
        chart.DefaultStyle.MaxMarkerSize = 20f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 5),   // Small
            new ScatterDataPoint(1, 20, 15),  // Medium
            new ScatterDataPoint(2, 15, 25)   // Large
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ColorMapping_DefaultScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseColorMapping = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 0, 0),    // Low value (blue)
            new ScatterDataPoint(1, 20, 0, 50),   // Medium value (green)
            new ScatterDataPoint(2, 15, 0, 100)   // High value (red)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ColorMapping_CustomScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseColorMapping = true;
        chart.DefaultStyle.ColorScale = new[]
        {
            SKColors.Purple,
            SKColors.Yellow,
            SKColors.Orange
        };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 0, 0),
            new ScatterDataPoint(1, 20, 0, 50),
            new ScatterDataPoint(2, 15, 0, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void CombinedFeatures_VariableSizeAndColor_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseVariableSizes = true;
        chart.DefaultStyle.UseColorMapping = true;
        chart.DefaultStyle.MinMarkerSize = 6f;
        chart.DefaultStyle.MaxMarkerSize = 18f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 5, 10),
            new ScatterDataPoint(1, 20, 15, 50),
            new ScatterDataPoint(2, 15, 25, 90),
            new ScatterDataPoint(3, 25, 10, 30),
            new ScatterDataPoint(4, 18, 20, 70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ConnectingLines_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.Configuration.ShowConnectingLines = true;
        chart.Configuration.ConnectingLineColor = SKColors.Gray;
        chart.Configuration.ConnectingLineWidth = 1f;

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
    public void MultiSeries_DifferentStyles_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();

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

        chart.SetSeriesStyle(series1, new ScatterSeriesStyle
        {
            FillColor = SKColors.Red,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 8f
        });

        chart.SetSeriesStyle(series2, new ScatterSeriesStyle
        {
            FillColor = SKColors.Blue,
            MarkerShape = MarkerShape.Square,
            MarkerSize = 10f
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
        var chart = new ScatterChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new ScatterChart();

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SinglePoint_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();

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
    public void LargeDataset_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 1000; i++)
        {
            points.Add(new DataPoint(i, Math.Sin(i * 0.1) * 100 + 200));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NegativeValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(-10, -5),
            new DataPoint(-5, 10),
            new DataPoint(0, 0),
            new DataPoint(5, -10),
            new DataPoint(10, 5)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MarkerShape_None_ShouldNotRenderMarkers()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.MarkerShape = MarkerShape.None;

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
    public void VariableSizes_SameValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseVariableSizes = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 10),
            new ScatterDataPoint(1, 20, 10),
            new ScatterDataPoint(2, 15, 10)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should handle edge case of all same sizes)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ColorMapping_SameValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseColorMapping = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(0, 10, 0, 50),
            new ScatterDataPoint(1, 20, 0, 50),
            new ScatterDataPoint(2, 15, 0, 50)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should handle edge case of all same color values)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MixedDataPointTypes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new ScatterChart();
        chart.DefaultStyle.UseVariableSizes = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10),                 // Regular point
            new ScatterDataPoint(1, 20, 15),      // Scatter point with size
            new DataPoint(2, 15),                 // Regular point
            new ScatterDataPoint(3, 25, 20)       // Scatter point with size
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should handle mixed point types)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void BorderWithDifferentShapes_ShouldRenderWithoutErrors()
    {
        // Arrange
        foreach (var shape in new[] { MarkerShape.Circle, MarkerShape.Square, MarkerShape.Diamond, MarkerShape.Triangle })
        {
            var chart = new ScatterChart();
            chart.DefaultStyle.MarkerShape = shape;
            chart.DefaultStyle.FillColor = SKColors.White;
            chart.DefaultStyle.BorderColor = SKColors.Black;
            chart.DefaultStyle.BorderWidth = 2f;

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
    }
}
