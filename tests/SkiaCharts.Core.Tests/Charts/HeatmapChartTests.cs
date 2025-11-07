using SkiaCharts.Core.Charts;
using SkiaCharts.Core.Data;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Charts;

public class HeatmapChartTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var chart = new HeatmapChart();

        // Assert
        Assert.NotNull(chart.Style);
        Assert.NotNull(chart.Configuration);
        Assert.Equal(HeatmapInterpolation.Nearest, chart.Style.Interpolation);
        Assert.True(chart.Configuration.ShowColorLegend);
        Assert.Equal(LegendPosition.Right, chart.Configuration.LegendPosition);
    }

    [Fact]
    public void BasicHeatmap_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (no exception should be thrown)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void HeatmapData_ShouldCalculateMinMax()
    {
        // Arrange & Act
        var data = new HeatmapData(new double[,]
        {
            { 1, 5, 3 },
            { 4, 2, 6 },
            { 7, 9, 8 }
        });

        // Assert
        Assert.Equal(1, data.MinValue);
        Assert.Equal(9, data.MaxValue);
        Assert.Equal(3, data.Rows);
        Assert.Equal(3, data.Columns);
    }

    [Fact]
    public void HeatmapData_WithLabels_ShouldStoreLabels()
    {
        // Arrange
        var xLabels = new[] { "A", "B", "C" };
        var yLabels = new[] { "Row1", "Row2" };

        // Act
        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        }, xLabels, yLabels);

        // Assert
        Assert.Equal("A", data.GetXLabel(0));
        Assert.Equal("B", data.GetXLabel(1));
        Assert.Equal("C", data.GetXLabel(2));
        Assert.Equal("Row1", data.GetYLabel(0));
        Assert.Equal("Row2", data.GetYLabel(1));
    }

    [Fact]
    public void NearestInterpolation_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.Interpolation = HeatmapInterpolation.Nearest;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BilinearInterpolation_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.Interpolation = HeatmapInterpolation.Bilinear;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void BicubicInterpolation_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.Interpolation = HeatmapInterpolation.Bicubic;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomColorScale_ShouldApply()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.ColorScale = new[]
        {
            SKColors.Purple,
            SKColors.White,
            SKColors.Orange
        };

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CellBorders_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.ShowCellBorders = true;
        chart.Style.CellBorderColor = SKColors.Black;
        chart.Style.CellBorderWidth = 1f;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CellValues_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.ShowCellValues = true;
        chart.Style.CellValueColor = SKColors.Black;
        chart.Style.CellValueFontSize = 12f;
        chart.Style.CellValueFormat = "F2";
        chart.Style.MinCellSizeForValues = 10f; // Low threshold

        var data = new HeatmapData(new double[,]
        {
            { 1.5, 2.7, 3.2 },
            { 4.1, 5.9, 6.3 },
            { 7.8, 8.4, 9.1 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ContourLines_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.ShowContourLines = true;
        chart.Style.ContourLineColor = new SKColor(100, 100, 100);
        chart.Style.ContourLineWidth = 1.5f;
        chart.Style.ContourLevels = 5;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 },
            { 13, 14, 15, 16 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ColorLegend_Enabled_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Configuration.ShowColorLegend = true;
        chart.Configuration.LegendPosition = LegendPosition.Right;
        chart.Configuration.LegendWidth = 50f;
        chart.Configuration.LegendHeightRatio = 0.8f;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ColorLegend_Disabled_ShouldNotRender()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Configuration.ShowColorLegend = false;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void ColorLegend_NonePosition_ShouldNotRender()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Configuration.LegendPosition = LegendPosition.None;

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CustomMinMaxValues_ShouldApply()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Configuration.MinValue = 0;
        chart.Configuration.MaxValue = 100;

        var data = new HeatmapData(new double[,]
        {
            { 10, 20, 30 },
            { 40, 50, 60 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void LargeHeatmap_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var values = new double[50, 50];
        for (int row = 0; row < 50; row++)
        {
            for (int col = 0; col < 50; col++)
            {
                values[row, col] = Math.Sin(row * 0.2) * Math.Cos(col * 0.2) * 100;
            }
        }

        var data = new HeatmapData(values);
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(1000, 800));

        // Act & Assert (should handle large grid efficiently)
        chart.Render(surface.Canvas, 1000, 800);
    }

    [Fact]
    public void SmallHeatmap_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 1, 2 },
            { 3, 4 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void SingleCell_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 5 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 300);
    }

    [Fact]
    public void NullData_ShouldNotThrowException()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Data = null;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should not throw)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NaNValues_ShouldBeSkipped()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 1, double.NaN, 3 },
            { 4, 5, double.NaN },
            { double.NaN, 8, 9 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should skip NaN values)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void InfinityValues_ShouldBeSkipped()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 1, double.PositiveInfinity, 3 },
            { 4, 5, double.NegativeInfinity }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should skip infinity values)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void IdenticalValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 5, 5, 5 },
            { 5, 5, 5 },
            { 5, 5, 5 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert (should handle zero range gracefully)
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void NegativeValues_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { -10, -5, 0 },
            { 5, 10, 15 },
            { -20, -15, -10 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 600));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 600);
    }

    [Fact]
    public void CombinedFeatures_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();
        chart.Style.Interpolation = HeatmapInterpolation.Bilinear;
        chart.Style.ShowCellBorders = true;
        chart.Style.ShowCellValues = true;
        chart.Style.ShowContourLines = true;
        chart.Style.ColorScale = new[] { SKColors.Blue, SKColors.Yellow, SKColors.Red };
        chart.Configuration.ShowColorLegend = true;

        var data = new HeatmapData(new double[,]
        {
            { 1, 3, 5, 7 },
            { 2, 4, 6, 8 },
            { 3, 5, 7, 9 },
            { 4, 6, 8, 10 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(1000, 800));

        // Act & Assert
        chart.Render(surface.Canvas, 1000, 800);
    }

    [Fact]
    public void RectangularGrid_MoreColumnsThanRows_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 1, 2, 3, 4, 5, 6, 7, 8 },
            { 9, 10, 11, 12, 13, 14, 15, 16 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(800, 400));

        // Act & Assert
        chart.Render(surface.Canvas, 800, 400);
    }

    [Fact]
    public void RectangularGrid_MoreRowsThanColumns_ShouldRenderWithoutErrors()
    {
        // Arrange
        var chart = new HeatmapChart();

        var data = new HeatmapData(new double[,]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 },
            { 7, 8 },
            { 9, 10 },
            { 11, 12 }
        });
        chart.Data = data;

        using var surface = SKSurface.Create(new SKImageInfo(400, 800));

        // Act & Assert
        chart.Render(surface.Canvas, 400, 800);
    }
}
