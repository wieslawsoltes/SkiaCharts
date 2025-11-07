using SkiaCharts.Core.Axes;
using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class BarChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new BarChart();

        // Assert
        Assert.NotNull(chart.DefaultStyle);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(SKColors.Blue, chart.DefaultStyle.FillColor);
        Assert.Equal(BarStackMode.None, chart.Configuration.StackMode);
        Assert.Equal(BarOrientation.Vertical, chart.Configuration.Orientation);
    }

    [Fact]
    public void SetSeriesStyle_ShouldStoreStyleForSeries()
    {
        // Arrange
        var chart = new BarChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 10)
        });
        var style = new BarSeriesStyle
        {
            FillColor = SKColors.Red,
            BorderColor = SKColors.Black
        };

        // Act
        chart.SetSeriesStyle(series, style);
        var retrievedStyle = chart.GetSeriesStyle(series);

        // Assert
        Assert.Equal(SKColors.Red, retrievedStyle.FillColor);
        Assert.Equal(SKColors.Black, retrievedStyle.BorderColor);
    }

    [Fact]
    public void GetSeriesStyle_ShouldReturnDefaultStyleForUnstyledSeries()
    {
        // Arrange
        var chart = new BarChart();
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
    public void VerticalBars_SingleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.Orientation = BarOrientation.Vertical;
        chart.XAxis = new CategoryAxis(new[] { "Q1", "Q2", "Q3", "Q4" });

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 150),
            new DataPoint(2, 120),
            new DataPoint(3, 180)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void HorizontalBars_SingleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.Orientation = BarOrientation.Horizontal;
        chart.YAxis = new CategoryAxis(new[] { "Q1", "Q2", "Q3", "Q4" });

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 150),
            new DataPoint(2, 120),
            new DataPoint(3, 180)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GroupedBars_MultipleSeries_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.StackMode = BarStackMode.None;
        chart.XAxis = new CategoryAxis(new[] { "Jan", "Feb", "Mar" });

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 50),
            new DataPoint(1, 60),
            new DataPoint(2, 55)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 45),
            new DataPoint(1, 70),
            new DataPoint(2, 50)
        });

        chart.SetSeriesStyle(series1, new BarSeriesStyle { FillColor = SKColors.Blue });
        chart.SetSeriesStyle(series2, new BarSeriesStyle { FillColor = SKColors.Red });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void StackedBars_Absolute_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.StackMode = BarStackMode.Absolute;
        chart.XAxis = new CategoryAxis(new[] { "Product A", "Product B" });

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 30),
            new DataPoint(1, 40)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 20),
            new DataPoint(1, 30)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void StackedBars_Percentage_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.StackMode = BarStackMode.Percentage;
        chart.XAxis = new CategoryAxis(new[] { "North", "South", "East", "West" });

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 30),
            new DataPoint(1, 40),
            new DataPoint(2, 35),
            new DataPoint(3, 25)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 70),
            new DataPoint(1, 60),
            new DataPoint(2, 65),
            new DataPoint(3, 75)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void RoundedCorners_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.CornerRadius = 8f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 150),
            new DataPoint(2, 120)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void BorderStyle_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.BorderColor = SKColors.Black;
        chart.DefaultStyle.BorderWidth = 2f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 120),
            new DataPoint(2, 90)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GradientFill_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.GradientColors = new[]
        {
            SKColors.Blue,
            SKColors.LightBlue
        };
        chart.DefaultStyle.GradientAngle = 90f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 50),
            new DataPoint(1, 100),
            new DataPoint(2, 75)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ValueLabels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.ShowValueLabels = true;
        chart.Configuration.ValueLabelFormat = "0.0";

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 45.5),
            new DataPoint(1, 78.2),
            new DataPoint(2, 62.8)
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
        var chart = new BarChart();
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
        var chart = new BarChart();

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void BarWidthRatio_ShouldAffectBarWidth()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.BarWidthRatio = 0.5; // Narrower bars

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 100),
            new DataPoint(1, 150)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void MinimumBarSize_ShouldPreventTinyBars()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.MinimumBarSize = 5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0.1), // Very small value
            new DataPoint(1, 100)
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
        var chart = new BarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, -50),
            new DataPoint(1, 100),
            new DataPoint(2, -30),
            new DataPoint(3, 80)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void AllStackModes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var stackMode in Enum.GetValues<BarStackMode>())
        {
            var chart = new BarChart();
            chart.Configuration.StackMode = stackMode;

            var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 30),
                new DataPoint(1, 40)
            });

            var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 20),
                new DataPoint(1, 30)
            });

            chart.Series.Add(series1);
            chart.Series.Add(series2);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }

    [Fact]
    public void AllOrientations_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var orientation in Enum.GetValues<BarOrientation>())
        {
            var chart = new BarChart();
            chart.Configuration.Orientation = orientation;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new DataPoint(0, 50),
                new DataPoint(1, 100),
                new DataPoint(2, 75)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 300));
            chart.Render(surface.Canvas, 400, 300);
        }
    }

    [Fact]
    public void CombinedFeatures_RoundedCornersAndBorderAndGradient_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.DefaultStyle.CornerRadius = 10f;
        chart.DefaultStyle.BorderColor = SKColors.DarkBlue;
        chart.DefaultStyle.BorderWidth = 2f;
        chart.DefaultStyle.GradientColors = new[]
        {
            new SKColor(0, 100, 255),
            new SKColor(0, 200, 255)
        };

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 80),
            new DataPoint(1, 120),
            new DataPoint(2, 100)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void GroupSpacing_ShouldAffectBarPlacement()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.GroupSpacing = 0.5; // Larger spacing between groups

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 50),
            new DataPoint(1, 60)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 45),
            new DataPoint(1, 55)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ThreeSeries_Grouped_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.StackMode = BarStackMode.None;

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 30),
            new DataPoint(1, 40)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 35),
            new DataPoint(1, 45)
        });

        var series3 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 25),
            new DataPoint(1, 50)
        });

        chart.SetSeriesStyle(series1, new BarSeriesStyle { FillColor = SKColors.Red });
        chart.SetSeriesStyle(series2, new BarSeriesStyle { FillColor = SKColors.Green });
        chart.SetSeriesStyle(series3, new BarSeriesStyle { FillColor = SKColors.Blue });

        chart.Series.Add(series1);
        chart.Series.Add(series2);
        chart.Series.Add(series3);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ThreeSeries_Stacked_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();
        chart.Configuration.StackMode = BarStackMode.Absolute;

        var series1 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 20),
            new DataPoint(1, 30)
        });

        var series2 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 30),
            new DataPoint(1, 40)
        });

        var series3 = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 50),
            new DataPoint(1, 30)
        });

        chart.Series.Add(series1);
        chart.Series.Add(series2);
        chart.Series.Add(series3);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void ZeroValue_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new BarChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 0),
            new DataPoint(1, 100),
            new DataPoint(2, 0)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }
}
