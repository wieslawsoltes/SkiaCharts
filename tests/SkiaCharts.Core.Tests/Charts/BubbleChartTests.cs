using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;
using ScatterDataPoint = SkiaCharts.Core.Charts.ScatterDataPoint;

namespace SkiaCharts.Core.Tests.Charts;

public class BubbleChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new BubbleChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(BubbleSizeScale.Area, chart.DefaultStyle.SizeScale);
        Assert.Equal(3f, chart.DefaultStyle.MinBubbleSize);
        Assert.Equal(40f, chart.DefaultStyle.MaxBubbleSize);
        Assert.Equal(180, chart.DefaultStyle.Opacity);
    }

    [Fact]
    public void BasicBubble_WithScatterDataPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 200),
            new ScatterDataPoint(3, 15, 150),
            new ScatterDataPoint(4, 25, 250)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BasicBubble_WithDataPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(1, 10),
            new DataPoint(2, 20),
            new DataPoint(3, 15),
            new DataPoint(4, 25)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (regular points should render with constant size)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void LinearSizeScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.SizeScale = BubbleSizeScale.Linear;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 50),
            new ScatterDataPoint(2, 20, 100),
            new ScatterDataPoint(3, 15, 150),
            new ScatterDataPoint(4, 25, 200)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void AreaSizeScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.SizeScale = BubbleSizeScale.Area;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 50),
            new ScatterDataPoint(2, 20, 100),
            new ScatterDataPoint(3, 15, 150),
            new ScatterDataPoint(4, 25, 200)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void LogarithmicSizeScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.SizeScale = BubbleSizeScale.Logarithmic;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 1),
            new ScatterDataPoint(2, 20, 10),
            new ScatterDataPoint(3, 15, 100),
            new ScatterDataPoint(4, 25, 1000)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ColorMapping_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.UseColorMapping = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100, 0),
            new ScatterDataPoint(2, 20, 150, 25),
            new ScatterDataPoint(3, 15, 120, 50),
            new ScatterDataPoint(4, 25, 180, 75),
            new ScatterDataPoint(5, 30, 200, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ColorMapping_CustomScale_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.UseColorMapping = true;
        chart.DefaultStyle.ColorScale = new[]
        {
            SKColors.Purple,
            SKColors.Pink,
            SKColors.Orange
        };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100, 0),
            new ScatterDataPoint(2, 20, 150, 50),
            new ScatterDataPoint(3, 15, 120, 100)
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
        var chart = new BubbleChart();
        chart.DefaultStyle.Opacity = 100; // More transparent

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 150),
            new ScatterDataPoint(3, 15, 120)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BorderStyle_ShouldApply()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 2f;
        chart.DefaultStyle.BorderOpacity = 255;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 150),
            new ScatterDataPoint(3, 15, 120)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Labels_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.ShowLabels = true;
        chart.DefaultStyle.MinLabelSize = 10f; // Show labels on most bubbles
        chart.DefaultStyle.LabelFormat = "{2:F0}"; // Show size as integer

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 150),
            new ScatterDataPoint(3, 15, 120),
            new ScatterDataPoint(4, 25, 180)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Labels_WithCollisionDetection_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.ShowLabels = true;
        chart.DefaultStyle.MinLabelSize = 5f;
        chart.Configuration.EnableLabelCollisionDetection = true;
        chart.Configuration.LabelCollisionPadding = 5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(1.1, 10.1, 110),  // Close to previous - may hide label
            new ScatterDataPoint(2, 20, 150),
            new ScatterDataPoint(3, 15, 120)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void Labels_WithCustomFormat_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.ShowLabels = true;
        chart.DefaultStyle.LabelFormat = "({0:F1}, {1:F1})"; // Show X,Y coordinates
        chart.DefaultStyle.MinLabelSize = 10f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 150)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomBubbleSize_ShouldApply()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.MinBubbleSize = 10f;
        chart.DefaultStyle.MaxBubbleSize = 60f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 50),
            new ScatterDataPoint(2, 20, 100),
            new ScatterDataPoint(3, 15, 200)
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
        var chart = new BubbleChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100)
        });

        var style = new BubbleSeriesStyle
        {
            FillColor = SKColors.Purple,
            Opacity = 200,
            SizeScale = BubbleSizeScale.Linear
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Purple, retrievedStyle.FillColor);
        Assert.Equal(200, retrievedStyle.Opacity);
        Assert.Equal(BubbleSizeScale.Linear, retrievedStyle.SizeScale);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new BubbleChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100)
        });

        // Act
        var style = chart.GetSeriesStyle(series);

        // Assert
        Assert.Same(chart.DefaultStyle, style);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new BubbleChart();
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
        var chart = new BubbleChart();

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void SingleBubble_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ManyBubbles_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var points = new List<IDataPoint>();
        var random = new Random(42);
        for (int i = 0; i < 100; i++)
        {
            points.Add(new ScatterDataPoint(
                random.NextDouble() * 100,
                random.NextDouble() * 100,
                random.NextDouble() * 200 + 10
            ));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(1200, 800));

        // Act & Assert
        chart.Render(surface.Canvas, 1200, 800);
    }

    [Fact]
    public void IdenticalSizes_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 100),
            new ScatterDataPoint(3, 15, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should handle identical sizes gracefully)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void MixedScatterAndDataPoints_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new DataPoint(2, 20), // Regular data point - constant size
            new ScatterDataPoint(3, 15, 150)
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
        var chart = new BubbleChart();

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 150)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(3, 15, 120),
            new ScatterDataPoint(4, 25, 180)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        chart.SetSeriesStyle(series2, new BubbleSeriesStyle
        {
            FillColor = SKColors.Orange,
            Opacity = 150
        });

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ExtremelySmallBubbles_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.MinBubbleSize = 1f;
        chart.DefaultStyle.MaxBubbleSize = 5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 1),
            new ScatterDataPoint(2, 20, 2),
            new ScatterDataPoint(3, 15, 3)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ExtremelyLargeBubbles_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.MinBubbleSize = 50f;
        chart.DefaultStyle.MaxBubbleSize = 100f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100),
            new ScatterDataPoint(2, 20, 200),
            new ScatterDataPoint(3, 15, 150)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CombinedFeatures_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BubbleChart();
        chart.DefaultStyle.UseColorMapping = true;
        chart.DefaultStyle.ShowLabels = true;
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 1.5f;
        chart.DefaultStyle.SizeScale = BubbleSizeScale.Area;
        chart.DefaultStyle.MinLabelSize = 15f;
        chart.Configuration.EnableLabelCollisionDetection = true;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new ScatterDataPoint(1, 10, 100, 0),
            new ScatterDataPoint(2, 20, 200, 33),
            new ScatterDataPoint(3, 15, 150, 66),
            new ScatterDataPoint(4, 25, 250, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }
}
