using SkiaCharts.Core.Interactivity;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Interactivity;

public class InteractiveElementsTests
{
    // Tooltip Tests
    [Fact]
    public void TooltipManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new TooltipManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.Equal(TimeSpan.FromMilliseconds(500), manager.ShowDelay);
        Assert.Equal(TimeSpan.FromMilliseconds(200), manager.HideDelay);
        Assert.Null(manager.CurrentTooltip);
        Assert.False(manager.IsVisible);
    }

    [Fact]
    public void TooltipManager_Show_ShouldSetCurrentTooltip()
    {
        // Arrange
        var manager = new TooltipManager
        {
            ShowDelay = TimeSpan.Zero // No delay for testing
        };
        var data = new { X = 10, Y = 20 };
        var position = new SKPoint(100, 100);

        // Act
        manager.Show(data, position);

        // Assert
        Assert.NotNull(manager.CurrentTooltip);
        Assert.Equal(data, manager.CurrentTooltip!.Data);
    }

    [Fact]
    public void TooltipManager_Hide_ShouldClearTooltip()
    {
        // Arrange
        var manager = new TooltipManager();
        manager.Show(new { X = 10 }, new SKPoint(100, 100));

        // Act
        manager.Hide();

        // Assert
        Assert.Null(manager.CurrentTooltip);
        Assert.False(manager.IsVisible);
    }

    [Fact]
    public void TooltipManager_Update_ShouldChangeTooltipOnDifferentData()
    {
        // Arrange
        var manager = new TooltipManager
        {
            ShowDelay = TimeSpan.Zero
        };
        var data1 = new { X = 10 };
        var data2 = new { X = 20 };

        // Act
        manager.Update(data1, new SKPoint(100, 100));
        var firstTooltip = manager.CurrentTooltip;

        manager.Update(data2, new SKPoint(110, 110));
        var secondTooltip = manager.CurrentTooltip;

        // Assert
        Assert.NotNull(firstTooltip);
        Assert.NotNull(secondTooltip);
        Assert.NotEqual(firstTooltip!.Data, secondTooltip!.Data);
    }

    [Fact]
    public void TooltipManager_ShouldFireTooltipChangedEvent()
    {
        // Arrange
        var manager = new TooltipManager();
        bool eventFired = false;
        manager.TooltipChanged += (s, e) => eventFired = true;

        // Act
        manager.Show(new { X = 10 }, new SKPoint(100, 100));

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void DefaultTooltipRenderer_ShouldNotThrow()
    {
        // Arrange
        var renderer = new DefaultTooltipRenderer();
        var tooltip = new TooltipInfo
        {
            Data = new { X = 10 },
            Position = new SKPoint(100, 100),
            Content = "Test Tooltip"
        };

        using var surface = SKSurface.Create(new SKImageInfo(200, 200));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        renderer.Render(canvas, tooltip, new SKRect(0, 0, 200, 200));
    }

    // Crosshair Tests
    [Fact]
    public void CrosshairManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new CrosshairManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.False(manager.IsVisible);
        Assert.Null(manager.Position);
    }

    [Fact]
    public void CrosshairManager_Show_ShouldMakeVisible()
    {
        // Arrange
        var manager = new CrosshairManager();
        var position = new SKPoint(100, 100);

        // Act
        manager.Show(position);

        // Assert
        Assert.True(manager.IsVisible);
        Assert.Equal(position, manager.Position);
    }

    [Fact]
    public void CrosshairManager_Hide_ShouldMakeInvisible()
    {
        // Arrange
        var manager = new CrosshairManager();
        manager.Show(new SKPoint(100, 100));

        // Act
        manager.Hide();

        // Assert
        Assert.False(manager.IsVisible);
        Assert.Null(manager.Position);
    }

    [Fact]
    public void CrosshairManager_Update_ShouldChangePosition()
    {
        // Arrange
        var manager = new CrosshairManager();
        var pos1 = new SKPoint(100, 100);
        var pos2 = new SKPoint(150, 150);

        // Act
        manager.Update(pos1);
        var firstPos = manager.Position;

        manager.Update(pos2);
        var secondPos = manager.Position;

        // Assert
        Assert.Equal(pos1, firstPos);
        Assert.Equal(pos2, secondPos);
    }

    [Fact]
    public void CrosshairManager_ShouldFirePositionChangedEvent()
    {
        // Arrange
        var manager = new CrosshairManager();
        bool eventFired = false;
        manager.PositionChanged += (s, e) => eventFired = true;

        // Act
        manager.Show(new SKPoint(100, 100));

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void SynchronizedCrosshairManager_ShouldSyncCrosshairs()
    {
        // Arrange
        var syncManager = new SynchronizedCrosshairManager();
        var crosshair1 = new CrosshairManager();
        var crosshair2 = new CrosshairManager();

        syncManager.Register(crosshair1);
        syncManager.Register(crosshair2);

        var position = new SKPoint(100, 100);

        // Act
        syncManager.UpdateAll(position);

        // Assert
        Assert.Equal(position, crosshair1.Position);
        Assert.Equal(position, crosshair2.Position);
    }

    // Selection Tests
    [Fact]
    public void SelectionManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new SelectionManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.Equal(SelectionMode.Single, manager.SelectionMode);
        Assert.Empty(manager.SelectedItems);
        Assert.False(manager.HasSelection);
    }

    [Fact]
    public void SelectionManager_Select_SingleMode_ShouldSelectOneItem()
    {
        // Arrange
        var manager = new SelectionManager { SelectionMode = SelectionMode.Single };
        var item1 = new { X = 10 };
        var item2 = new { X = 20 };

        // Act
        manager.Select(item1);
        manager.Select(item2);

        // Assert
        Assert.Single(manager.SelectedItems);
        Assert.Contains(item2, manager.SelectedItems);
        Assert.DoesNotContain(item1, manager.SelectedItems);
    }

    [Fact]
    public void SelectionManager_Select_MultipleMode_ShouldSelectMultipleItems()
    {
        // Arrange
        var manager = new SelectionManager { SelectionMode = SelectionMode.Multiple };
        var item1 = new { X = 10 };
        var item2 = new { X = 20 };

        // Act
        manager.Select(item1, addToSelection: true);
        manager.Select(item2, addToSelection: true);

        // Assert
        Assert.Equal(2, manager.SelectedItems.Count);
        Assert.Contains(item1, manager.SelectedItems);
        Assert.Contains(item2, manager.SelectedItems);
    }

    [Fact]
    public void SelectionManager_Deselect_ShouldRemoveItem()
    {
        // Arrange
        var manager = new SelectionManager();
        var item = new { X = 10 };
        manager.Select(item);

        // Act
        var changed = manager.Deselect(item);

        // Assert
        Assert.True(changed);
        Assert.Empty(manager.SelectedItems);
    }

    [Fact]
    public void SelectionManager_Toggle_ShouldToggleSelection()
    {
        // Arrange
        var manager = new SelectionManager { SelectionMode = SelectionMode.Multiple };
        var item = new { X = 10 };

        // Act
        var selected1 = manager.Toggle(item); // Select
        var selected2 = manager.Toggle(item); // Deselect

        // Assert
        Assert.True(selected1);
        Assert.False(selected2);
        Assert.Empty(manager.SelectedItems);
    }

    [Fact]
    public void SelectionManager_Clear_ShouldRemoveAllSelections()
    {
        // Arrange
        var manager = new SelectionManager { SelectionMode = SelectionMode.Multiple };
        manager.Select(new { X = 10 }, addToSelection: true);
        manager.Select(new { X = 20 }, addToSelection: true);

        // Act
        var changed = manager.Clear();

        // Assert
        Assert.True(changed);
        Assert.Empty(manager.SelectedItems);
        Assert.False(manager.HasSelection);
    }

    [Fact]
    public void SelectionManager_SelectRange_ShouldSetRange()
    {
        // Arrange
        var manager = new SelectionManager();
        var range = new RectangleSelectionRange
        {
            Rectangle = new SKRect(0, 0, 100, 100)
        };

        // Act
        var changed = manager.SelectRange(range);

        // Assert
        Assert.True(changed);
        Assert.Equal(range, manager.SelectedRange);
        Assert.True(manager.HasSelection);
    }

    [Fact]
    public void SelectionManager_ShouldFireSelectionChangedEvent()
    {
        // Arrange
        var manager = new SelectionManager();
        bool eventFired = false;
        manager.SelectionChanged += (s, e) => eventFired = true;

        // Act
        manager.Select(new { X = 10 });

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void RectangleSelectionRange_Contains_ShouldDetectPoint()
    {
        // Arrange
        var range = new RectangleSelectionRange
        {
            Rectangle = new SKRect(10, 10, 100, 100)
        };

        // Act & Assert
        Assert.True(range.Contains(new SKPoint(50, 50))); // Inside
        Assert.False(range.Contains(new SKPoint(5, 5))); // Outside
        Assert.False(range.Contains(new SKPoint(150, 150))); // Outside
    }

    // Context Menu Tests
    [Fact]
    public void ContextMenuManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new ContextMenuManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.False(manager.IsVisible);
        Assert.Null(manager.CurrentMenu);
    }

    [Fact]
    public void ContextMenuManager_Show_ShouldDisplayMenu()
    {
        // Arrange
        var manager = new ContextMenuManager();
        var items = new List<ContextMenuItem>
        {
            new ContextMenuItem { Text = "Item 1", Action = () => { } },
            new ContextMenuItem { Text = "Item 2", Action = () => { } }
        };

        // Act
        manager.Show(new SKPoint(100, 100), null, items);

        // Assert
        Assert.True(manager.IsVisible);
        Assert.NotNull(manager.CurrentMenu);
        Assert.Equal(2, manager.CurrentMenu!.Items.Count);
    }

    [Fact]
    public void ContextMenuManager_Hide_ShouldHideMenu()
    {
        // Arrange
        var manager = new ContextMenuManager();
        var items = new List<ContextMenuItem>
        {
            new ContextMenuItem { Text = "Item 1", Action = () => { } }
        };
        manager.Show(new SKPoint(100, 100), null, items);

        // Act
        manager.Hide();

        // Assert
        Assert.False(manager.IsVisible);
        Assert.Null(manager.CurrentMenu);
    }

    [Fact]
    public void ContextMenuManager_SelectItem_ShouldExecuteAction()
    {
        // Arrange
        var manager = new ContextMenuManager();
        bool actionExecuted = false;
        var item = new ContextMenuItem
        {
            Text = "Test Item",
            Action = () => actionExecuted = true
        };

        var items = new List<ContextMenuItem> { item };
        manager.Show(new SKPoint(100, 100), null, items);

        // Act
        manager.SelectItem(item);

        // Assert
        Assert.True(actionExecuted);
        Assert.False(manager.IsVisible); // Menu should hide after selection
    }

    [Fact]
    public void ContextMenuManager_ShouldFireMenuRequestedEvent()
    {
        // Arrange
        var manager = new ContextMenuManager();
        bool eventFired = false;
        manager.MenuRequested += (s, e) => eventFired = true;

        // Act
        manager.RequestMenu(new SKPoint(100, 100), null);

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void ContextMenuItem_Separator_ShouldCreateSeparator()
    {
        // Arrange & Act
        var separator = ContextMenuItem.Separator();

        // Assert
        Assert.True(separator.IsSeparator);
        Assert.Empty(separator.Text);
    }

    // Hover Highlight Tests
    [Fact]
    public void HoverHighlightManager_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var manager = new HoverHighlightManager();

        // Assert
        Assert.True(manager.IsEnabled);
        Assert.False(manager.IsHighlighting);
        Assert.Null(manager.HoveredItem);
        Assert.Equal(HighlightStyle.Glow, manager.HighlightStyle);
    }

    [Fact]
    public void HoverHighlightManager_Update_ShouldSetHoveredItem()
    {
        // Arrange
        var manager = new HoverHighlightManager
        {
            HighlightDelay = TimeSpan.Zero // No delay for testing
        };
        var item = new { X = 10 };

        // Act
        manager.Update(item, new SKPoint(100, 100));

        // Assert
        Assert.Equal(item, manager.HoveredItem);
        Assert.True(manager.IsHighlighting);
    }

    [Fact]
    public void HoverHighlightManager_Clear_ShouldRemoveHover()
    {
        // Arrange
        var manager = new HoverHighlightManager();
        manager.Update(new { X = 10 }, new SKPoint(100, 100));

        // Act
        manager.Clear();

        // Assert
        Assert.Null(manager.HoveredItem);
        Assert.False(manager.IsHighlighting);
    }

    [Fact]
    public void HoverHighlightManager_IsHovered_ShouldDetectHoveredItem()
    {
        // Arrange
        var manager = new HoverHighlightManager
        {
            HighlightDelay = TimeSpan.Zero
        };
        var item1 = new { X = 10 };
        var item2 = new { X = 20 };

        // Act
        manager.Update(item1, new SKPoint(100, 100));

        // Assert
        Assert.True(manager.IsHovered(item1));
        Assert.False(manager.IsHovered(item2));
    }

    [Fact]
    public void HoverHighlightManager_GetScaleFactor_ShouldReturnScaleForHoveredItem()
    {
        // Arrange
        var manager = new HoverHighlightManager
        {
            HighlightDelay = TimeSpan.Zero,
            HighlightStyle = HighlightStyle.Scale,
            ScaleFactor = 1.5f
        };
        var item1 = new { X = 10 };
        var item2 = new { X = 20 };

        // Act
        manager.Update(item1, new SKPoint(100, 100));

        // Assert
        Assert.Equal(1.5f, manager.GetScaleFactor(item1));
        Assert.Equal(1.0f, manager.GetScaleFactor(item2));
    }

    [Fact]
    public void HoverHighlightManager_ShouldFireHoverChangedEvent()
    {
        // Arrange
        var manager = new HoverHighlightManager();
        bool eventFired = false;
        object? newItem = null;

        manager.HoverChanged += (s, e) =>
        {
            eventFired = true;
            newItem = e.NewItem;
        };

        var item = new { X = 10 };

        // Act
        manager.Update(item, new SKPoint(100, 100));

        // Assert
        Assert.True(eventFired);
        Assert.Equal(item, newItem);
    }

    [Fact]
    public void HoverHighlightManager_RenderHighlight_ShouldNotThrow()
    {
        // Arrange
        var manager = new HoverHighlightManager
        {
            HighlightDelay = TimeSpan.Zero
        };
        manager.Update(new { X = 10 }, new SKPoint(100, 100));

        using var surface = SKSurface.Create(new SKImageInfo(200, 200));
        var canvas = surface.Canvas;

        // Act & Assert - Should not throw
        manager.RenderHighlight(canvas, new SKRect(50, 50, 150, 150));
        manager.RenderCircleHighlight(canvas, new SKPoint(100, 100), 20);
    }
}
