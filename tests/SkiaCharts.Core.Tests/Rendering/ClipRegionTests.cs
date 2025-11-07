using SkiaCharts.Core.Rendering;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Rendering;

public class ClipRegionTests
{
    [Fact]
    public void ClipRegion_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var region = new ClipRegion(10, 20, 100, 200);

        // Assert
        Assert.Equal(10, region.X);
        Assert.Equal(20, region.Y);
        Assert.Equal(100, region.Width);
        Assert.Equal(200, region.Height);
        Assert.Equal(10, region.Left);
        Assert.Equal(110, region.Right);
        Assert.Equal(20, region.Top);
        Assert.Equal(220, region.Bottom);
    }

    [Fact]
    public void ClipRegion_FromRect_CreatesCorrectRegion()
    {
        // Arrange
        var skRect = new SKRect(10, 20, 110, 220);

        // Act
        var region = ClipRegion.FromRect(skRect);

        // Assert
        Assert.Equal(10, region.X);
        Assert.Equal(20, region.Y);
        Assert.Equal(100, region.Width);
        Assert.Equal(200, region.Height);
    }

    [Fact]
    public void ClipRegion_ToSKRect_ConvertsCorrectly()
    {
        // Arrange
        var region = new ClipRegion(10, 20, 100, 200);

        // Act
        var skRect = region.ToSKRect();

        // Assert
        Assert.Equal(10, skRect.Left);
        Assert.Equal(20, skRect.Top);
        Assert.Equal(110, skRect.Right);
        Assert.Equal(220, skRect.Bottom);
    }

    [Fact]
    public void ClipRegion_Contains_DetectsPointInside()
    {
        // Arrange
        var region = new ClipRegion(0, 0, 100, 100);

        // Act & Assert
        Assert.True(region.Contains(50, 50));
        Assert.True(region.Contains(0, 0));
        Assert.True(region.Contains(100, 100));
    }

    [Fact]
    public void ClipRegion_Contains_DetectsPointOutside()
    {
        // Arrange
        var region = new ClipRegion(0, 0, 100, 100);

        // Act & Assert
        Assert.False(region.Contains(-1, 50));
        Assert.False(region.Contains(50, -1));
        Assert.False(region.Contains(101, 50));
        Assert.False(region.Contains(50, 101));
    }

    [Fact]
    public void ClipRegion_Intersects_DetectsIntersection()
    {
        // Arrange
        var region1 = new ClipRegion(0, 0, 100, 100);
        var region2 = new ClipRegion(50, 50, 100, 100);

        // Act & Assert
        Assert.True(region1.Intersects(region2));
        Assert.True(region2.Intersects(region1));
    }

    [Fact]
    public void ClipRegion_Intersects_DetectsNoIntersection()
    {
        // Arrange
        var region1 = new ClipRegion(0, 0, 100, 100);
        var region2 = new ClipRegion(200, 200, 100, 100);

        // Act & Assert
        Assert.False(region1.Intersects(region2));
        Assert.False(region2.Intersects(region1));
    }

    [Fact]
    public void ClipRegion_Intersects_DetectsRectIntersection()
    {
        // Arrange
        var region = new ClipRegion(0, 0, 100, 100);

        // Act & Assert
        Assert.True(region.Intersects(50, 50, 100, 100));
        Assert.True(region.Intersects(-50, -50, 100, 100));
        Assert.False(region.Intersects(200, 200, 100, 100));
    }

    [Fact]
    public void ClipRegion_Expand_ExpandsCorrectly()
    {
        // Arrange
        var region = new ClipRegion(10, 10, 80, 80);

        // Act
        var expanded = region.Expand(10);

        // Assert
        Assert.Equal(0, expanded.X);
        Assert.Equal(0, expanded.Y);
        Assert.Equal(100, expanded.Width);
        Assert.Equal(100, expanded.Height);
    }

    [Fact]
    public void ClipRegion_Intersect_CalculatesIntersection()
    {
        // Arrange
        var region1 = new ClipRegion(0, 0, 100, 100);
        var region2 = new ClipRegion(50, 50, 100, 100);

        // Act
        var intersection = region1.Intersect(region2);

        // Assert
        Assert.Equal(50, intersection.X);
        Assert.Equal(50, intersection.Y);
        Assert.Equal(50, intersection.Width);
        Assert.Equal(50, intersection.Height);
    }

    [Fact]
    public void ClipRegion_Intersect_ReturnsEmptyForNoIntersection()
    {
        // Arrange
        var region1 = new ClipRegion(0, 0, 100, 100);
        var region2 = new ClipRegion(200, 200, 100, 100);

        // Act
        var intersection = region1.Intersect(region2);

        // Assert
        Assert.True(intersection.IsEmpty);
    }

    [Fact]
    public void ClipRegion_IsEmpty_DetectsEmptyRegion()
    {
        // Arrange & Act & Assert
        Assert.True(new ClipRegion(0, 0, 0, 0).IsEmpty);
        Assert.True(new ClipRegion(0, 0, -10, 10).IsEmpty);
        Assert.True(new ClipRegion(0, 0, 10, -10).IsEmpty);
        Assert.False(new ClipRegion(0, 0, 10, 10).IsEmpty);
    }

    [Fact]
    public void ClipRegion_ToString_ReturnsFormattedString()
    {
        // Arrange
        var region = new ClipRegion(10.5, 20.5, 100.5, 200.5);

        // Act
        var result = region.ToString();

        // Assert
        Assert.Contains("10.5", result);
        Assert.Contains("20.5", result);
        Assert.Contains("100.5", result);
        Assert.Contains("200.5", result);
    }
}
