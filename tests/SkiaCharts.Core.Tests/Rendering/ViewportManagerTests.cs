using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaSharp;

namespace SkiaCharts.Core.Tests.Rendering;

public class ViewportManagerTests
{
    [Fact]
    public void ViewportManager_ShouldTransformDataToScreen()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            XDataRange = new DataRange(0, 100),
            YDataRange = new DataRange(0, 100),
            ScreenRect = new SKRect(0, 0, 200, 200)
        };

        // Act
        var screenPoint = viewport.DataToScreen(50, 50);

        // Assert
        Assert.Equal(100, screenPoint.X, 1);
        Assert.Equal(100, screenPoint.Y, 1);
    }

    [Fact]
    public void ViewportManager_ShouldTransformScreenToData()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            XDataRange = new DataRange(0, 100),
            YDataRange = new DataRange(0, 100),
            ScreenRect = new SKRect(0, 0, 200, 200)
        };

        // Act
        var (dataX, dataY) = viewport.ScreenToData(100, 100);

        // Assert
        Assert.Equal(50, dataX, 1);
        Assert.Equal(50, dataY, 1);
    }

    [Fact]
    public void ViewportManager_ShouldZoomCorrectly()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            XDataRange = new DataRange(0, 100),
            YDataRange = new DataRange(0, 100),
            ScreenRect = new SKRect(0, 0, 200, 200)
        };

        // Act - Zoom in by 2x around center
        viewport.Zoom(2.0, 50, 50);

        // Assert
        Assert.True(viewport.XDataRange.Span < 100);
        Assert.True(viewport.YDataRange.Span < 100);
    }

    [Fact]
    public void ViewportManager_ShouldPanCorrectly()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            XDataRange = new DataRange(0, 100),
            YDataRange = new DataRange(0, 100),
            ScreenRect = new SKRect(0, 0, 200, 200)
        };

        // Act
        viewport.Pan(10, 20);

        // Assert
        Assert.Equal(10, viewport.XDataRange.Min);
        Assert.Equal(110, viewport.XDataRange.Max);
        Assert.Equal(20, viewport.YDataRange.Min);
        Assert.Equal(120, viewport.YDataRange.Max);
    }

    [Fact]
    public void ViewportManager_ShouldTransformLogarithmicAxis()
    {
        // Arrange
        var viewport = new ViewportManager
        {
            XDataRange = new DataRange(1, 1000),
            YDataRange = new DataRange(1, 1000),
            ScreenRect = new SKRect(0, 0, 300, 300)
        };

        viewport.SetXTransform(Math.Log10, value => Math.Pow(10, value));
        viewport.SetYTransform(Math.Log10, value => Math.Pow(10, value));

        // Act
        var screenPoint = viewport.DataToScreen(10, 10);
        var (dataX, dataY) = viewport.ScreenToData(screenPoint.X, screenPoint.Y);

        // Assert
        Assert.Equal(100, screenPoint.X, 1);
        Assert.Equal(200, screenPoint.Y, 1);
        Assert.InRange(dataX, 9.9, 10.1);
        Assert.InRange(dataY, 9.9, 10.1);
    }
}
