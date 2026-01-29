using SkiaCharts.Core.Legend;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Legend;

public class LegendAndLabelTests
{
    // Legend Tests
    [Fact]
    public void LegendManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new LegendManager();

        // Assert
        Assert.True(manager.IsVisible);
        Assert.Equal(LegendPosition.TopRight, manager.Position);
        Assert.Equal(LegendOrientation.Vertical, manager.Orientation);
        Assert.Empty(manager.Items);
        Assert.True(manager.IsInteractive);
    }

    [Fact]
    public void LegendManager_AddItem_ShouldAddToCollection()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem
        {
            Text = "Series 1",
            Color = SKColors.Red
        };

        // Act
        manager.AddItem(item);

        // Assert
        Assert.Single(manager.Items);
        Assert.Contains(item, manager.Items);
    }

    [Fact]
    public void LegendManager_RemoveItem_ShouldRemoveFromCollection()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem
        {
            Text = "Series 1",
            Color = SKColors.Red
        };
        manager.AddItem(item);

        // Act
        var removed = manager.RemoveItem(item);

        // Assert
        Assert.True(removed);
        Assert.Empty(manager.Items);
    }

    [Fact]
    public void LegendManager_Clear_ShouldRemoveAllItems()
    {
        // Arrange
        var manager = new LegendManager();
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });
        manager.AddItem(new LegendItem { Text = "Series 2", Color = SKColors.Blue });

        // Act
        manager.Clear();

        // Assert
        Assert.Empty(manager.Items);
    }

    [Fact]
    public void LegendManager_CalculateLayout_ShouldSetBounds()
    {
        // Arrange
        var manager = new LegendManager();
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });
        manager.AddItem(new LegendItem { Text = "Series 2", Color = SKColors.Blue });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        Assert.NotEqual(SKRect.Empty, manager.Bounds);
        Assert.True(manager.Bounds.Width > 0);
        Assert.True(manager.Bounds.Height > 0);
    }

    [Fact]
    public void LegendManager_CalculateLayout_TopLeft_ShouldPositionCorrectly()
    {
        // Arrange
        var manager = new LegendManager
        {
            Position = LegendPosition.TopLeft
        };
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        Assert.True(manager.Bounds.Left < chartBounds.MidX);
        Assert.True(manager.Bounds.Top < chartBounds.MidY);
    }

    [Fact]
    public void LegendManager_CalculateLayout_BottomRight_ShouldPositionCorrectly()
    {
        // Arrange
        var manager = new LegendManager
        {
            Position = LegendPosition.BottomRight
        };
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        Assert.True(manager.Bounds.Right > chartBounds.MidX);
        Assert.True(manager.Bounds.Bottom > chartBounds.MidY);
    }

    [Fact]
    public void LegendManager_HitTest_ShouldDetectClick()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem { Text = "Series 1", Color = SKColors.Red };
        manager.AddItem(item);

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        // Act
        var hitItem = manager.HitTest(item.Bounds.Location);

        // Assert
        Assert.Equal(item, hitItem);
    }

    [Fact]
    public void LegendManager_HitTest_OutsideBounds_ShouldReturnNull()
    {
        // Arrange
        var manager = new LegendManager();
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        // Act
        var hitItem = manager.HitTest(new SKPoint(-100, -100));

        // Assert
        Assert.Null(hitItem);
    }

    [Fact]
    public void LegendManager_ToggleItem_ShouldChangeVisibility()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem { Text = "Series 1", Color = SKColors.Red };
        manager.AddItem(item);

        var originalVisibility = item.IsVisible;

        // Act
        manager.ToggleItem(item);

        // Assert
        Assert.NotEqual(originalVisibility, item.IsVisible);
    }

    [Fact]
    public void LegendManager_HandleClick_ShouldToggleVisibility()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem { Text = "Series 1", Color = SKColors.Red };
        manager.AddItem(item);

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        var originalVisibility = item.IsVisible;

        // Act
        var handled = manager.HandleClick(item.Bounds.Location);

        // Assert
        Assert.True(handled);
        Assert.NotEqual(originalVisibility, item.IsVisible);
    }

    [Fact]
    public void LegendManager_ShouldFireItemClickedEvent()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem { Text = "Series 1", Color = SKColors.Red };
        manager.AddItem(item);

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        bool eventFired = false;
        LegendItem? clickedItem = null;

        manager.ItemClicked += (s, e) =>
        {
            eventFired = true;
            clickedItem = e.Item;
        };

        // Act
        manager.HandleClick(item.Bounds.Location);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(item, clickedItem);
    }

    [Fact]
    public void LegendManager_ShouldFireItemVisibilityChangedEvent()
    {
        // Arrange
        var manager = new LegendManager();
        var item = new LegendItem { Text = "Series 1", Color = SKColors.Red };
        manager.AddItem(item);

        bool eventFired = false;

        manager.ItemVisibilityChanged += (s, e) =>
        {
            eventFired = true;
        };

        // Act
        manager.ToggleItem(item);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void LegendManager_Render_ShouldNotThrow()
    {
        // Arrange
        var manager = new LegendManager();
        manager.AddItem(new LegendItem { Text = "Series 1", Color = SKColors.Red });
        manager.AddItem(new LegendItem { Text = "Series 2", Color = SKColors.Blue });

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        manager.Render(canvas, chartBounds);
    }

    [Fact]
    public void LegendItem_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var item = new LegendItem
        {
            Text = "Test Series",
            Color = SKColors.Red,
            SymbolType = LegendSymbolType.Circle,
            Data = new { X = 10, Y = 20 }
        };

        // Assert
        Assert.Equal("Test Series", item.Text);
        Assert.Equal(SKColors.Red, item.Color);
        Assert.Equal(LegendSymbolType.Circle, item.SymbolType);
        Assert.True(item.IsVisible);
        Assert.NotNull(item.Data);
    }

    // Data Label Tests
    [Fact]
    public void DataLabelManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new DataLabelManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.True(manager.EnableCollisionDetection);
        Assert.Empty(manager.Labels);
        Assert.Equal(100, manager.MaxLabels);
    }

    [Fact]
    public void DataLabelManager_AddLabel_ShouldAddToCollection()
    {
        // Arrange
        var manager = new DataLabelManager();
        var label = new DataLabel
        {
            Value = 42.5,
            Position = new SKPoint(100, 100)
        };

        // Act
        manager.AddLabel(label);

        // Assert
        Assert.Single(manager.Labels);
        Assert.Contains(label, manager.Labels);
    }

    [Fact]
    public void DataLabelManager_Clear_ShouldRemoveAllLabels()
    {
        // Arrange
        var manager = new DataLabelManager();
        manager.AddLabel(new DataLabel { Value = 10, Position = new SKPoint(100, 100) });
        manager.AddLabel(new DataLabel { Value = 20, Position = new SKPoint(150, 150) });

        // Act
        manager.Clear();

        // Assert
        Assert.Empty(manager.Labels);
    }

    [Fact]
    public void DataLabelManager_MaxLabels_ShouldLimitCount()
    {
        // Arrange
        var manager = new DataLabelManager { MaxLabels = 2 };

        // Act
        manager.AddLabel(new DataLabel { Value = 1, Position = new SKPoint(100, 100) });
        manager.AddLabel(new DataLabel { Value = 2, Position = new SKPoint(150, 150) });
        manager.AddLabel(new DataLabel { Value = 3, Position = new SKPoint(200, 200) }); // Should not be added

        // Assert
        Assert.Equal(2, manager.Labels.Count);
    }

    [Fact]
    public void DataLabelManager_CalculateLayout_ShouldSetBounds()
    {
        // Arrange
        var manager = new DataLabelManager();
        manager.AddLabel(new DataLabel
        {
            Value = 42.5,
            Position = new SKPoint(100, 100)
        });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        var label = manager.Labels.First();
        Assert.NotEqual(SKRect.Empty, label.Bounds);
    }

    [Fact]
    public void DataLabelManager_WithFormatter_ShouldFormatText()
    {
        // Arrange
        var manager = new DataLabelManager
        {
            Formatter = value => string.Format(System.Globalization.CultureInfo.InvariantCulture, "${0:F2}", value)
        };

        manager.AddLabel(new DataLabel
        {
            Value = 42.5,
            Position = new SKPoint(100, 100)
        });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        var label = manager.Labels.First();
        Assert.Equal("$42.50", label.FormattedText);
    }

    [Fact]
    public void DataLabelManager_CollisionDetection_ShouldRepositionOrHideOverlappingLabels()
    {
        // Arrange
        var manager = new DataLabelManager
        {
            EnableCollisionDetection = true
        };

        // Add two labels at the same position with the same placement
        manager.AddLabel(new DataLabel
        {
            Value = 10,
            Position = new SKPoint(100, 100),
            Placement = LabelPlacement.Top
        });

        manager.AddLabel(new DataLabel
        {
            Value = 20,
            Position = new SKPoint(100, 100),
            Placement = LabelPlacement.Top
        });

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert - First label should be at preferred position (Top)
        var firstLabel = manager.Labels.First();
        Assert.True(firstLabel.IsVisible);

        // Second label should either be repositioned to a different placement or hidden
        var secondLabel = manager.Labels.Last();

        if (secondLabel.IsVisible)
        {
            // If visible, bounds should not overlap with first label
            Assert.False(firstLabel.Bounds.IntersectsWith(secondLabel.Bounds));
        }

        // At minimum, collision detection should prevent exact overlap
        Assert.NotEqual(firstLabel.Bounds, secondLabel.Bounds);
    }

    [Fact]
    public void DataLabelManager_Render_ShouldNotThrow()
    {
        // Arrange
        var manager = new DataLabelManager();
        manager.AddLabel(new DataLabel { Value = 42, Position = new SKPoint(100, 100) });

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        manager.Render(canvas);
    }

    [Fact]
    public void DataLabel_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var label = new DataLabel
        {
            Value = 123.45,
            Position = new SKPoint(100, 100),
            Placement = LabelPlacement.Right,
            Data = new { Name = "Test" }
        };

        // Assert
        Assert.Equal(123.45, label.Value);
        Assert.Equal(new SKPoint(100, 100), label.Position);
        Assert.Equal(LabelPlacement.Right, label.Placement);
        Assert.NotNull(label.Data);
    }

    // Title Tests
    [Fact]
    public void TitleManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new TitleManager();

        // Assert
        Assert.Null(manager.Title);
        Assert.Null(manager.Subtitle);
        Assert.False(manager.HasTitle);
        Assert.False(manager.HasSubtitle);
        Assert.Equal(16.0f, manager.TitleFontSize);
        Assert.Equal(12.0f, manager.SubtitleFontSize);
    }

    [Fact]
    public void TitleManager_HasTitle_ShouldReturnTrueWhenSet()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Chart Title"
        };

        // Assert
        Assert.True(manager.HasTitle);
    }

    [Fact]
    public void TitleManager_HasSubtitle_ShouldReturnTrueWhenSet()
    {
        // Arrange
        var manager = new TitleManager
        {
            Subtitle = "Chart Subtitle"
        };

        // Assert
        Assert.True(manager.HasSubtitle);
    }

    [Fact]
    public void TitleManager_CalculateTotalHeight_WithTitle_ShouldReturnHeight()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Test Title"
        };

        // Act
        var height = manager.CalculateTotalHeight();

        // Assert
        Assert.True(height > 0);
    }

    [Fact]
    public void TitleManager_CalculateTotalHeight_WithTitleAndSubtitle_ShouldIncludeBoth()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Test Title",
            Subtitle = "Test Subtitle"
        };

        // Act
        var height = manager.CalculateTotalHeight();

        // Assert
        Assert.True(height > manager.TitleFontSize + manager.SubtitleFontSize);
    }

    [Fact]
    public void TitleManager_CalculateLayout_ShouldSetBounds()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Test Title",
            Subtitle = "Test Subtitle"
        };

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        Assert.NotEqual(SKRect.Empty, manager.TitleBounds);
        Assert.NotEqual(SKRect.Empty, manager.SubtitleBounds);
    }

    [Fact]
    public void TitleManager_TitleAlignment_Center_ShouldCenterTitle()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Test",
            TitleAlignment = TextAlignment.Center
        };

        var chartBounds = new SKRect(0, 0, 400, 300);

        // Act
        manager.CalculateLayout(chartBounds);

        // Assert
        var titleCenter = manager.TitleBounds.MidX;
        var chartCenter = chartBounds.MidX;
        Assert.True(Math.Abs(titleCenter - chartCenter) < 50); // Allow some tolerance
    }

    [Fact]
    public void TitleManager_Render_ShouldNotThrow()
    {
        // Arrange
        var manager = new TitleManager
        {
            Title = "Test Title",
            Subtitle = "Test Subtitle"
        };

        var chartBounds = new SKRect(0, 0, 400, 300);
        manager.CalculateLayout(chartBounds);

        using var surface = SKSurface.Create(new SKImageInfo(400, 300));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        manager.Render(canvas, chartBounds);
    }
}
