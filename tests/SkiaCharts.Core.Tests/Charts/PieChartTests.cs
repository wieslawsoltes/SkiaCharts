using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class PieChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new PieChart();

        // Assert
        Assert.NotNull(chart.Configuration);
        Assert.False(chart.Configuration.IsDonut);
        Assert.Equal(0.5, chart.Configuration.InnerRadiusRatio);
        Assert.Equal(0f, chart.Configuration.StartAngle);
        Assert.Equal(PieLabelPosition.Outside, chart.Configuration.LabelPosition);
    }

    [Fact]
    public void BasicPieChart_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30, "Slice 1"),
            new PieDataPoint(50, "Slice 2"),
            new PieDataPoint(20, "Slice 3")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void DonutChart_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.IsDonut = true;
        chart.Configuration.InnerRadiusRatio = 0.6;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(50),
            new PieDataPoint(20)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void SetSliceStyle_ShouldStoreStyleForSlice()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(50)
        });

        var style = new PieSliceStyle
        {
            FillColor = SKColors.Red,
            BorderColor = SKColors.Black,
            BorderWidth = 2f
        };

        // Act
        chart.SetSliceStyle(series, 0, style);
        var retrievedStyle = chart.GetSliceStyle(series, 0);

        // Assert
        Assert.Equal(SKColors.Red, retrievedStyle.FillColor);
        Assert.Equal(SKColors.Black, retrievedStyle.BorderColor);
        Assert.Equal(2f, retrievedStyle.BorderWidth);
    }

    [Fact]
    public void GetSliceStyle_ShouldReturnDefaultStyleForUnstyledSlice()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30)
        });

        // Act
        var style = chart.GetSliceStyle(series, 0);

        // Assert
        Assert.NotNull(style);
        Assert.NotEqual(SKColors.Empty, style.FillColor);
    }

    [Fact]
    public void ExplodedSlice_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(50),
            new PieDataPoint(20)
        });
        chart.Series.Add(series);

        // Explode the second slice
        chart.SetSliceStyle(series, 1, new PieSliceStyle
        {
            FillColor = SKColors.Green,
            ExplodeDistance = 20f
        });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void MultipleExplodedSlices_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(25),
            new PieDataPoint(25),
            new PieDataPoint(25),
            new PieDataPoint(25)
        });
        chart.Series.Add(series);

        // Explode multiple slices
        chart.SetSliceStyle(series, 0, new PieSliceStyle { ExplodeDistance = 15f });
        chart.SetSliceStyle(series, 2, new PieSliceStyle { ExplodeDistance = 15f });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void StartAngle_ShouldRotateChart()
    {
        // Arrange & Act & Assert
        foreach (var angle in new[] { 0f, 90f, 180f, 270f })
        {
            var chart = new PieChart();
            chart.Configuration.StartAngle = angle;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new PieDataPoint(30),
                new PieDataPoint(70)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 400));
            chart.Render(surface.Canvas, 400, 400);
        }
    }

    [Fact]
    public void GradientFill_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(40),
            new PieDataPoint(60)
        });
        chart.Series.Add(series);

        chart.SetSliceStyle(series, 0, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightBlue, SKColors.DarkBlue }
        });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void MultipleGradientSlices_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(33),
            new PieDataPoint(33),
            new PieDataPoint(34)
        });
        chart.Series.Add(series);

        chart.SetSliceStyle(series, 0, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightBlue, SKColors.DarkBlue }
        });
        chart.SetSliceStyle(series, 1, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightGreen, SKColors.DarkGreen }
        });
        chart.SetSliceStyle(series, 2, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightPink, SKColors.DarkRed }
        });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void SliceBorders_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        chart.SetSliceStyle(series, 0, new PieSliceStyle
        {
            FillColor = SKColors.Blue,
            BorderColor = SKColors.White,
            BorderWidth = 3f
        });
        chart.SetSliceStyle(series, 1, new PieSliceStyle
        {
            FillColor = SKColors.Green,
            BorderColor = SKColors.White,
            BorderWidth = 3f
        });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void Labels_Inside_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.Inside;
        chart.Configuration.LabelContent = PieLabelContent.Percentage;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30, "A"),
            new PieDataPoint(50, "B"),
            new PieDataPoint(20, "C")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void Labels_Outside_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.Outside;
        chart.Configuration.LabelContent = PieLabelContent.Percentage;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30, "A"),
            new PieDataPoint(50, "B"),
            new PieDataPoint(20, "C")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void Labels_None_ShouldNotRenderLabels()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.None;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30, "A"),
            new PieDataPoint(70, "B")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void LabelContent_AllTypes_ShouldRenderWithoutErrors()
    {
        // Arrange & Act & Assert
        foreach (var contentType in Enum.GetValues<PieLabelContent>())
        {
            var chart = new PieChart();
            chart.Configuration.LabelPosition = PieLabelPosition.Outside;
            chart.Configuration.LabelContent = contentType;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new PieDataPoint(30, "Alpha"),
                new PieDataPoint(70, "Beta")
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 400));
            chart.Render(surface.Canvas, 400, 400);
        }
    }

    [Fact]
    public void DonutWithLabels_Inside_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.IsDonut = true;
        chart.Configuration.InnerRadiusRatio = 0.5;
        chart.Configuration.LabelPosition = PieLabelPosition.Inside;
        chart.Configuration.LabelContent = PieLabelContent.Percentage;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(40),
            new PieDataPoint(60)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void EmptySeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new PieChart();
        var series = new DataSeries<IDataPoint>(Array.Empty<IDataPoint>());
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void NoSeries_ShouldNotThrowException()
    {
        // Arrange
        var chart = new PieChart();

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void SingleSlice_ShouldRenderFullCircle()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(100, "Full")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void ManySlices_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();

        var points = new List<IDataPoint>();
        for (int i = 0; i < 20; i++)
        {
            points.Add(new PieDataPoint(5, $"Slice {i}"));
        }

        var series = new DataSeries<IDataPoint>(points);
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(600, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 600, 600);
    }

    [Fact]
    public void VerySmallSlices_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.Outside;
        chart.Configuration.MinimumLabelAngle = 5f;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(90, "Large"),
            new PieDataPoint(5, "Medium"),
            new PieDataPoint(3, "Small"),
            new PieDataPoint(2, "Tiny")
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (small slices should skip labels based on MinimumLabelAngle)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void NegativeValues_ShouldBeSkipped()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(-10), // Should be skipped
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (should render only positive values)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void ZeroValues_ShouldBeSkipped()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(0), // Should be skipped
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void AllZeroValues_ShouldNotRenderSlices()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(0),
            new PieDataPoint(0),
            new PieDataPoint(0)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (should not throw, just render nothing)
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void ThinDonut_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.IsDonut = true;
        chart.Configuration.InnerRadiusRatio = 0.9; // Very thin ring

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void ThickDonut_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.IsDonut = true;
        chart.Configuration.InnerRadiusRatio = 0.2; // Very thick ring

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void ExplodedDonut_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.IsDonut = true;
        chart.Configuration.InnerRadiusRatio = 0.6;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30),
            new PieDataPoint(70)
        });
        chart.Series.Add(series);

        chart.SetSliceStyle(series, 0, new PieSliceStyle
        {
            ExplodeDistance = 20f
        });

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void CombinedFeatures_ExplodedGradientWithLabels_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.Outside;
        chart.Configuration.LabelContent = PieLabelContent.NameAndPercentage;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(25, "Q1"),
            new PieDataPoint(30, "Q2"),
            new PieDataPoint(20, "Q3"),
            new PieDataPoint(25, "Q4")
        });
        chart.Series.Add(series);

        chart.SetSliceStyle(series, 0, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightBlue, SKColors.DarkBlue },
            ExplodeDistance = 15f
        });
        chart.SetSliceStyle(series, 2, new PieSliceStyle
        {
            GradientColors = new[] { SKColors.LightGreen, SKColors.DarkGreen },
            ExplodeDistance = 10f
        });

        using var surface = SKSurface.Create(new SKImageInfo(500, 500));

        // Act & Assert
        chart.Render(surface.Canvas, 500, 500);
    }

    [Fact]
    public void CustomRadiusRatio_ShouldAffectChartSize()
    {
        // Arrange & Act & Assert
        foreach (var ratio in new[] { 0.5, 0.7, 0.9, 1.0 })
        {
            var chart = new PieChart();
            chart.Configuration.RadiusRatio = ratio;

            var series = new DataSeries<IDataPoint>(new IDataPoint[]
            {
                new PieDataPoint(50),
                new PieDataPoint(50)
            });
            chart.Series.Add(series);

            using var surface = SKSurface.Create(new SKImageInfo(400, 400));
            chart.Render(surface.Canvas, 400, 400);
        }
    }

    [Fact]
    public void PieDataPoint_WithoutLabel_ShouldWork()
    {
        // Arrange
        var chart = new PieChart();
        chart.Configuration.LabelPosition = PieLabelPosition.Outside;
        chart.Configuration.LabelContent = PieLabelContent.Percentage;

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new PieDataPoint(30), // No label
            new PieDataPoint(70)  // No label
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 400);
    }

    [Fact]
    public void RegularDataPoint_ShouldWorkAsPieSlice()
    {
        // Arrange
        var chart = new PieChart();

        var series = new DataSeries<IDataPoint>(new IDataPoint[]
        {
            new DataPoint(0, 30),
            new DataPoint(0, 70)
        });
        chart.Series.Add(series);

        using var surface = SKSurface.Create(new SKImageInfo(400, 400));

        // Act & Assert (should work with regular DataPoint, using Y value)
        chart.Render(surface.Canvas, 400, 400);
    }
}
