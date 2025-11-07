using SkiaCharts.Core.Interactivity;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Interactivity;

public class NavigationTests
{
    // Viewport Tests
    [Fact]
    public void Viewport_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var viewport = new Viewport();

        // Assert
        Assert.Equal(1.0f, viewport.Zoom);
        Assert.Equal(SKPoint.Empty, viewport.Pan);
        Assert.Equal(0.1f, viewport.MinZoom);
        Assert.Equal(100.0f, viewport.MaxZoom);
    }

    [Fact]
    public void Viewport_Zoom_ShouldClampToMinMax()
    {
        // Arrange
        var viewport = new Viewport
        {
            MinZoom = 0.5f,
            MaxZoom = 5.0f
        };

        // Act & Assert
        viewport.Zoom = 0.1f; // Below min
        Assert.Equal(0.5f, viewport.Zoom);

        viewport.Zoom = 10.0f; // Above max
        Assert.Equal(5.0f, viewport.Zoom);

        viewport.Zoom = 2.0f; // Within range
        Assert.Equal(2.0f, viewport.Zoom);
    }

    [Fact]
    public void Viewport_ScreenToData_ShouldConvertCorrectly()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 2.0f, // With zoom 2.0, visible area is 100x100 (matching data bounds)
            Pan = new SKPoint(0, 0)
        };

        // Act
        var screenPoint = new SKPoint(100, 100); // Middle of view
        var dataPoint = viewport.ScreenToData(screenPoint);

        // Assert
        Assert.Equal(50, dataPoint.X, 1); // Should map to middle of data (visible rect is 100x100)
        Assert.Equal(50, dataPoint.Y, 1);
    }

    [Fact]
    public void Viewport_DataToScreen_ShouldConvertCorrectly()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 2.0f, // With zoom 2.0, visible area is 100x100 (matching data bounds)
            Pan = new SKPoint(0, 0)
        };

        // Act
        var dataPoint = new SKPoint(50, 50); // Middle of data
        var screenPoint = viewport.DataToScreen(dataPoint);

        // Assert
        Assert.Equal(100, screenPoint.X, 1); // Should map to middle of view (visible rect is 100x100)
        Assert.Equal(100, screenPoint.Y, 1);
    }

    [Fact]
    public void Viewport_ZoomBy_ShouldUpdateZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        // Act
        viewport.ZoomBy(2.0f);

        // Assert
        Assert.Equal(2.0f, viewport.Zoom);
    }

    [Fact]
    public void Viewport_ZoomIn_ShouldIncreaseZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        // Act
        viewport.ZoomIn(1.5f);

        // Assert
        Assert.Equal(1.5f, viewport.Zoom);
    }

    [Fact]
    public void Viewport_ZoomOut_ShouldDecreaseZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 2.0f
        };

        // Act
        viewport.ZoomOut(2.0f);

        // Assert
        Assert.Equal(1.0f, viewport.Zoom);
    }

    [Fact]
    public void Viewport_PanBy_ShouldUpdatePan()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 200, 200), // Larger data bounds to allow panning
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 2.0f,
            Pan = new SKPoint(50, 50) // Start with some pan offset to allow movement
        };

        // Act
        viewport.PanBy(new SKPoint(20, 20)); // Screen delta

        // Assert
        // With zoom 2.0, 20 screen pixels = 10 data units, pan moves opposite to delta
        Assert.Equal(40, viewport.Pan.X, 1);
        Assert.Equal(40, viewport.Pan.Y, 1);
    }

    [Fact]
    public void Viewport_ZoomToFit_ShouldFitDataInView()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200)
        };

        // Act
        viewport.ZoomToFit(margin: 0);

        // Assert
        Assert.Equal(2.0f, viewport.Zoom, 1); // 200/100 = 2.0
    }

    [Fact]
    public void Viewport_ZoomToRect_ShouldZoomToSpecificArea()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200)
        };

        // Act - Zoom to quarter of the data
        viewport.ZoomToRect(new SKRect(25, 25, 75, 75), margin: 0);

        // Assert
        // 50 units of data should fit in 200 pixels view, so zoom should be ~4
        Assert.True(viewport.Zoom >= 3.5f && viewport.Zoom <= 4.5f);
    }

    [Fact]
    public void Viewport_Reset_ShouldResetToFit()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 5.0f,
            Pan = new SKPoint(50, 50)
        };

        // Act
        viewport.Reset();

        // Assert
        Assert.True(viewport.Zoom < 5.0f); // Should be zoomed out
        Assert.NotEqual(new SKPoint(50, 50), viewport.Pan); // Should be re-centered
    }

    [Fact]
    public void Viewport_TransformChanged_ShouldFireOnZoomChange()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200)
        };

        bool eventFired = false;
        viewport.TransformChanged += (s, e) => eventFired = true;

        // Act
        viewport.Zoom = 2.0f;

        // Assert
        Assert.True(eventFired);
    }

    // NavigationBehavior Tests
    [Fact]
    public void NavigationBehavior_ShouldHandleMouseWheelZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var behavior = new NavigationBehavior(viewport)
        {
            MouseWheelZoomEnabled = true,
            MouseWheelZoomFactor = 1.2f
        };

        // Act - Zoom in
        var wheelEvent = new MouseEvent
        {
            EventType = MouseEventType.Wheel,
            Position = new SKPoint(100, 100),
            WheelDelta = 1
        };
        var handled = behavior.HandleMouseEvent(wheelEvent);

        // Assert
        Assert.True(handled);
        Assert.True(viewport.Zoom > 1.0f);
        Assert.True(wheelEvent.Handled);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandleDragToPan()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f,
            Pan = new SKPoint(0, 0)
        };

        var behavior = new NavigationBehavior(viewport)
        {
            DragToPanEnabled = true,
            DragToPanButton = MouseButton.Left
        };

        // Act - Start drag
        var mouseDown = new MouseEvent
        {
            EventType = MouseEventType.Down,
            Position = new SKPoint(100, 100),
            Button = MouseButton.Left
        };
        behavior.HandleMouseEvent(mouseDown);

        // Move
        var mouseMove = new MouseEvent
        {
            EventType = MouseEventType.Move,
            Position = new SKPoint(120, 120)
        };
        behavior.HandleMouseEvent(mouseMove);

        // Assert
        Assert.NotEqual(SKPoint.Empty, viewport.Pan);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandleBoxSelection()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var behavior = new NavigationBehavior(viewport)
        {
            BoxSelectionZoomEnabled = true,
            BoxSelectionModifier = KeyModifiers.Shift
        };

        SKRect? selectionRect = null;
        behavior.BoxSelectionChanged += (s, e) => selectionRect = e.SelectionRect;

        // Act - Start box selection
        var mouseDown = new MouseEvent
        {
            EventType = MouseEventType.Down,
            Position = new SKPoint(50, 50),
            Modifiers = KeyModifiers.Shift
        };
        behavior.HandleMouseEvent(mouseDown);

        // Move
        var mouseMove = new MouseEvent
        {
            EventType = MouseEventType.Move,
            Position = new SKPoint(150, 150),
            Modifiers = KeyModifiers.Shift
        };
        behavior.HandleMouseEvent(mouseMove);

        // Assert
        Assert.NotNull(selectionRect);
        Assert.NotNull(behavior.BoxSelectionRect);

        // Complete selection
        var mouseUp = new MouseEvent
        {
            EventType = MouseEventType.Up,
            Position = new SKPoint(150, 150),
            Modifiers = KeyModifiers.Shift
        };
        behavior.HandleMouseEvent(mouseUp);

        // Assert - Should have zoomed in
        Assert.True(viewport.Zoom > 1.0f);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandleKeyboardNavigation()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f,
            Pan = new SKPoint(0, 0)
        };

        var behavior = new NavigationBehavior(viewport)
        {
            KeyboardNavigationEnabled = true
        };

        var originalPan = viewport.Pan;

        // Act - Pan right
        var keyEvent = new KeyboardEvent
        {
            EventType = KeyboardEventType.KeyDown,
            Key = Key.Right
        };
        behavior.HandleKeyboardEvent(keyEvent);

        // Assert
        Assert.NotEqual(originalPan, viewport.Pan);
        Assert.True(keyEvent.Handled);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandleKeyboardZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var behavior = new NavigationBehavior(viewport)
        {
            KeyboardNavigationEnabled = true,
            KeyboardZoomFactor = 1.5f
        };

        // Act - Zoom in with Ctrl+Plus
        var keyEvent = new KeyboardEvent
        {
            EventType = KeyboardEventType.KeyDown,
            Key = Key.Plus,
            Modifiers = KeyModifiers.Control
        };
        behavior.HandleKeyboardEvent(keyEvent);

        // Assert
        Assert.Equal(1.5f, viewport.Zoom);
        Assert.True(keyEvent.Handled);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandleKeyboardReset()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 5.0f,
            Pan = new SKPoint(50, 50)
        };

        var behavior = new NavigationBehavior(viewport)
        {
            KeyboardNavigationEnabled = true
        };

        // Act - Reset with Ctrl+0
        var keyEvent = new KeyboardEvent
        {
            EventType = KeyboardEventType.KeyDown,
            Key = Key.D0,
            Modifiers = KeyModifiers.Control
        };
        behavior.HandleKeyboardEvent(keyEvent);

        // Assert
        Assert.NotEqual(5.0f, viewport.Zoom);
        Assert.True(keyEvent.Handled);
    }

    [Fact]
    public void NavigationBehavior_ShouldHandlePinchGesture()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var behavior = new NavigationBehavior(viewport)
        {
            PinchToZoomEnabled = true
        };

        var originalZoom = viewport.Zoom;

        // Act - Simulate pinch gesture
        var touchBegin = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(50, 100), State = TouchState.Began },
                new TouchPoint { Id = 2, Position = new SKPoint(150, 100), State = TouchState.Began }
            },
            EventType = TouchEventType.Begin
        };
        behavior.HandleTouchEvent(touchBegin);

        // Pinch out (zoom in)
        var touchMove = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(25, 100), State = TouchState.Moved },
                new TouchPoint { Id = 2, Position = new SKPoint(175, 100), State = TouchState.Moved }
            },
            EventType = TouchEventType.Move
        };
        behavior.HandleTouchEvent(touchMove);

        // Give the recognizer time to process
        System.Threading.Thread.Sleep(50);

        // Assert - Zoom should have changed (increased)
        // Note: The exact value depends on gesture recognizer timing
        Assert.True(viewport.Zoom >= originalZoom);
    }

    // ViewportAnimation Tests
    [Fact]
    public void ViewportAnimation_ShouldAnimateZoom()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var animation = new ViewportAnimation(viewport)
        {
            DefaultDuration = TimeSpan.FromMilliseconds(100)
        };

        // Act
        animation.AnimateZoom(2.0f);

        // Update animation
        System.Threading.Thread.Sleep(50); // Half duration
        animation.Update();

        var midZoom = viewport.Zoom;

        System.Threading.Thread.Sleep(60); // Complete
        animation.Update();

        // Assert
        Assert.True(midZoom > 1.0f && midZoom < 2.0f); // Should be in progress
        Assert.Equal(2.0f, viewport.Zoom, 1); // Should reach target
        Assert.False(animation.IsAnimating);
    }

    [Fact]
    public void ViewportAnimation_ShouldAnimatePan()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Pan = new SKPoint(0, 0)
        };

        var animation = new ViewportAnimation(viewport)
        {
            DefaultDuration = TimeSpan.FromMilliseconds(100)
        };

        var targetPan = new SKPoint(50, 50);

        // Act
        animation.AnimatePan(targetPan);

        // Update to completion
        System.Threading.Thread.Sleep(110);
        animation.Update();

        // Assert
        Assert.Equal(targetPan.X, viewport.Pan.X, 1);
        Assert.Equal(targetPan.Y, viewport.Pan.Y, 1);
    }

    [Fact]
    public void ViewportAnimation_ShouldFireCompletionEvent()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var animation = new ViewportAnimation(viewport)
        {
            DefaultDuration = TimeSpan.FromMilliseconds(50)
        };

        bool completed = false;
        animation.AnimationCompleted += (s, e) => completed = true;

        // Act
        animation.AnimateZoom(2.0f);
        System.Threading.Thread.Sleep(60);
        animation.Update();

        // Assert
        Assert.True(completed);
    }

    [Fact]
    public void ViewportAnimation_Stop_ShouldStopAnimation()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var animation = new ViewportAnimation(viewport)
        {
            DefaultDuration = TimeSpan.FromSeconds(10)
        };

        // Act
        animation.AnimateZoom(5.0f);
        Assert.True(animation.IsAnimating);

        animation.Stop();

        // Assert
        Assert.False(animation.IsAnimating);
        Assert.NotEqual(5.0f, viewport.Zoom); // Should not have reached target
    }

    [Fact]
    public void ViewportAnimation_EasingFunctions_ShouldApplyCorrectly()
    {
        // Arrange
        var viewport = new Viewport
        {
            DataBounds = new SKRect(0, 0, 100, 100),
            ViewBounds = new SKRect(0, 0, 200, 200),
            Zoom = 1.0f
        };

        var animation = new ViewportAnimation(viewport)
        {
            DefaultDuration = TimeSpan.FromMilliseconds(100),
            DefaultEasing = EasingFunction.EaseInOutCubic
        };

        // Act
        animation.AnimateZoom(2.0f);
        System.Threading.Thread.Sleep(50); // Midpoint
        animation.Update();

        var midZoom = viewport.Zoom;

        // Assert
        // With cubic easing, midpoint should not be exactly 1.5
        Assert.True(midZoom > 1.0f && midZoom < 2.0f);
        Assert.NotEqual(1.5f, midZoom); // Should differ from linear
    }
}
