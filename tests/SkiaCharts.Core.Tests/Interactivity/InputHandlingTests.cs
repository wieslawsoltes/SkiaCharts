using SkiaCharts.Core.Interactivity;
using SkiaSharp;
using Xunit;

namespace SkiaCharts.Core.Tests.Interactivity;

public class InputHandlingTests
{
    // Input Event Tests
    [Fact]
    public void MouseEvent_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var mouseEvent = new MouseEvent
        {
            Position = new SKPoint(100, 200),
            Button = MouseButton.Left,
            EventType = MouseEventType.Click,
            Modifiers = KeyModifiers.Control
        };

        // Assert
        Assert.Equal(100, mouseEvent.Position.X);
        Assert.Equal(200, mouseEvent.Position.Y);
        Assert.Equal(MouseButton.Left, mouseEvent.Button);
        Assert.Equal(MouseEventType.Click, mouseEvent.EventType);
        Assert.Equal(KeyModifiers.Control, mouseEvent.Modifiers);
        Assert.False(mouseEvent.Handled);
    }

    [Fact]
    public void TouchEvent_ShouldHavePrimaryTouchPoint()
    {
        // Arrange
        var touchPoints = new List<TouchPoint>
        {
            new TouchPoint { Id = 1, Position = new SKPoint(10, 20), State = TouchState.Began },
            new TouchPoint { Id = 2, Position = new SKPoint(30, 40), State = TouchState.Began }
        };

        // Act
        var touchEvent = new TouchEvent
        {
            TouchPoints = touchPoints,
            EventType = TouchEventType.Begin
        };

        // Assert
        Assert.NotNull(touchEvent.PrimaryTouchPoint);
        Assert.Equal(1, touchEvent.PrimaryTouchPoint!.Id);
        Assert.Equal(new SKPoint(10, 20), touchEvent.PrimaryTouchPoint.Position);
    }

    [Fact]
    public void KeyboardEvent_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var keyboardEvent = new KeyboardEvent
        {
            Key = Key.A,
            EventType = KeyboardEventType.KeyDown,
            Modifiers = KeyModifiers.Shift
        };

        // Assert
        Assert.Equal(Key.A, keyboardEvent.Key);
        Assert.Equal(KeyboardEventType.KeyDown, keyboardEvent.EventType);
        Assert.Equal(KeyModifiers.Shift, keyboardEvent.Modifiers);
    }

    [Fact]
    public void GestureEvent_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var gestureEvent = new GestureEvent
        {
            GestureType = GestureType.Pinch,
            Center = new SKPoint(100, 100),
            Scale = 1.5f,
            State = GestureState.Changed
        };

        // Assert
        Assert.Equal(GestureType.Pinch, gestureEvent.GestureType);
        Assert.Equal(100, gestureEvent.Center.X);
        Assert.Equal(1.5f, gestureEvent.Scale);
        Assert.Equal(GestureState.Changed, gestureEvent.State);
    }

    // Input Router Tests
    [Fact]
    public void InputRouter_ShouldRegisterAndUnregisterHandlers()
    {
        // Arrange
        var router = new InputRouter();
        var handler = new TestInputHandler();

        // Act
        router.RegisterHandler(handler);

        // Assert - Handler should be registered
        var mouseEvent = new MouseEvent
        {
            Position = new SKPoint(10, 10),
            EventType = MouseEventType.Click
        };
        router.RouteMouseEvent(mouseEvent);
        Assert.True(handler.MouseEventHandled);

        // Unregister
        router.UnregisterHandler(handler);
        handler.MouseEventHandled = false;
        router.RouteMouseEvent(mouseEvent);
        Assert.False(handler.MouseEventHandled);
    }

    [Fact]
    public void InputRouter_ShouldDetectDoubleClick()
    {
        // Arrange
        var router = new InputRouter();
        router.DoubleClickThreshold = 300;
        var handler = new TestInputHandler { ShouldHandleEvent = false }; // Don't consume events
        router.RegisterHandler(handler);

        var position = new SKPoint(100, 100);
        MouseEvent? doubleClickEvent = null;

        // Track all events
        var events = new List<MouseEvent>();
        handler.OnMouseEvent = (e) =>
        {
            events.Add(e);
            if (e.EventType == MouseEventType.DoubleClick)
                doubleClickEvent = e;
        };

        // Act - First click
        var click1 = new MouseEvent
        {
            Position = position,
            Button = MouseButton.Left,
            EventType = MouseEventType.Click
        };
        router.RouteMouseEvent(click1);

        // Second click (within threshold)
        var click2 = new MouseEvent
        {
            Position = position,
            Button = MouseButton.Left,
            EventType = MouseEventType.Click,
            Timestamp = click1.Timestamp.AddMilliseconds(200)
        };
        router.RouteMouseEvent(click2);

        // Assert
        Assert.NotNull(doubleClickEvent);
        Assert.Equal(MouseEventType.DoubleClick, doubleClickEvent!.EventType);
        Assert.Equal(position, doubleClickEvent.Position);
    }

    [Fact]
    public void InputRouter_ShouldTrackActiveTouches()
    {
        // Arrange
        var router = new InputRouter();

        // Act - Begin touch
        var touchBegin = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(10, 20), State = TouchState.Began }
            },
            EventType = TouchEventType.Begin
        };
        router.RouteTouchEvent(touchBegin);

        // Assert
        Assert.Single(router.ActiveTouches);
        Assert.True(router.ActiveTouches.ContainsKey(1));

        // Act - End touch
        var touchEnd = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(10, 20), State = TouchState.Ended }
            },
            EventType = TouchEventType.End
        };
        router.RouteTouchEvent(touchEnd);

        // Assert
        Assert.Empty(router.ActiveTouches);
    }

    [Fact]
    public void InputRouter_ShouldRespectHandlerPriority()
    {
        // Arrange
        var router = new InputRouter();
        var handler1 = new TestInputHandler { Priority = 1, ShouldHandleEvent = true };
        var handler2 = new TestInputHandler { Priority = 10, ShouldHandleEvent = true };

        router.RegisterHandler(handler1);
        router.RegisterHandler(handler2);

        // Act
        var mouseEvent = new MouseEvent
        {
            Position = new SKPoint(10, 10),
            EventType = MouseEventType.Click
        };
        router.RouteMouseEvent(mouseEvent);

        // Assert - Higher priority handler (handler2) should be called first
        Assert.True(handler2.MouseEventHandled);
        Assert.False(handler1.MouseEventHandled); // Not called because handler2 handled it
    }

    [Fact]
    public void InputRouter_HitTest_ShouldReturnCorrectHandler()
    {
        // Arrange
        var router = new InputRouter();
        var handler = new TestInputHandler
        {
            HitTestBounds = new SKRect(0, 0, 100, 100)
        };
        router.RegisterHandler(handler);

        // Act
        var hitInside = router.HitTest(new SKPoint(50, 50));
        var hitOutside = router.HitTest(new SKPoint(150, 150));

        // Assert
        Assert.Equal(handler, hitInside);
        Assert.Null(hitOutside);
    }

    // Gesture Recognizer Tests
    [Fact]
    public void PinchGestureRecognizer_ShouldRecognizePinch()
    {
        // Arrange
        var recognizer = new PinchGestureRecognizer();
        GestureEvent? capturedGesture = null;
        recognizer.GestureRecognized += (s, e) => capturedGesture = e;

        // Act - Begin with two touches
        var touchBegin = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(100, 100), State = TouchState.Began },
                new TouchPoint { Id = 2, Position = new SKPoint(200, 100), State = TouchState.Began }
            },
            EventType = TouchEventType.Begin
        };
        recognizer.ProcessTouchEvent(touchBegin);

        // Move - Pinch out (scale up)
        var touchMove = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(50, 100), State = TouchState.Moved },
                new TouchPoint { Id = 2, Position = new SKPoint(250, 100), State = TouchState.Moved }
            },
            EventType = TouchEventType.Move
        };
        recognizer.ProcessTouchEvent(touchMove);

        // Assert
        Assert.NotNull(capturedGesture);
        Assert.Equal(GestureType.Pinch, capturedGesture!.GestureType);
        Assert.True(capturedGesture.Scale > 1.0f); // Pinched out
    }

    [Fact]
    public void PanGestureRecognizer_ShouldRecognizePan()
    {
        // Arrange
        var recognizer = new PanGestureRecognizer();
        var gestures = new List<GestureEvent>();
        recognizer.GestureRecognized += (s, e) => gestures.Add(e);

        // Act - Begin touch
        var touchBegin = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(100, 100), State = TouchState.Began }
            },
            EventType = TouchEventType.Begin
        };
        recognizer.ProcessTouchEvent(touchBegin);

        // Move significantly
        var touchMove = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = new SKPoint(150, 120), State = TouchState.Moved }
            },
            EventType = TouchEventType.Move
        };
        recognizer.ProcessTouchEvent(touchMove);

        // Assert
        Assert.NotEmpty(gestures);
        Assert.Contains(gestures, g => g.GestureType == GestureType.Pan);
        var panGesture = gestures.First(g => g.GestureType == GestureType.Pan);
        Assert.Equal(50, panGesture.Delta.X);
        Assert.Equal(20, panGesture.Delta.Y);
    }

    [Fact]
    public void TapGestureRecognizer_ShouldRecognizeTap()
    {
        // Arrange
        var recognizer = new TapGestureRecognizer();
        GestureEvent? capturedGesture = null;
        recognizer.GestureRecognized += (s, e) => capturedGesture = e;

        var position = new SKPoint(100, 100);

        // Act - Begin and end quickly
        var touchBegin = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = position, State = TouchState.Began }
            },
            EventType = TouchEventType.Begin
        };
        recognizer.ProcessTouchEvent(touchBegin);

        var touchEnd = new TouchEvent
        {
            TouchPoints = new List<TouchPoint>
            {
                new TouchPoint { Id = 1, Position = position, State = TouchState.Ended }
            },
            EventType = TouchEventType.End,
            Timestamp = touchBegin.Timestamp.AddMilliseconds(100)
        };
        recognizer.ProcessTouchEvent(touchEnd);

        // Assert
        Assert.NotNull(capturedGesture);
        Assert.Equal(GestureType.Tap, capturedGesture!.GestureType);
        Assert.Equal(position, capturedGesture.Center);
    }

    // Keyboard Shortcuts Tests
    [Fact]
    public void KeyboardShortcutManager_ShouldRegisterAndExecuteShortcut()
    {
        // Arrange
        var manager = new KeyboardShortcutManager();
        bool actionExecuted = false;
        manager.Register(Key.A, KeyModifiers.Control, () => actionExecuted = true);

        // Act
        var keyEvent = new KeyboardEvent
        {
            Key = Key.A,
            Modifiers = KeyModifiers.Control,
            EventType = KeyboardEventType.KeyDown
        };
        var handled = manager.ProcessKeyboardEvent(keyEvent);

        // Assert
        Assert.True(handled);
        Assert.True(actionExecuted);
        Assert.True(keyEvent.Handled);
    }

    [Fact]
    public void KeyboardShortcutManager_ShouldNotExecuteWithWrongModifiers()
    {
        // Arrange
        var manager = new KeyboardShortcutManager();
        bool actionExecuted = false;
        manager.Register(Key.A, KeyModifiers.Control, () => actionExecuted = true);

        // Act - Press A without Control
        var keyEvent = new KeyboardEvent
        {
            Key = Key.A,
            Modifiers = KeyModifiers.None,
            EventType = KeyboardEventType.KeyDown
        };
        var handled = manager.ProcessKeyboardEvent(keyEvent);

        // Assert
        Assert.False(handled);
        Assert.False(actionExecuted);
    }

    [Fact]
    public void KeyboardShortcut_ShouldFormatCorrectly()
    {
        // Arrange
        var shortcut = new KeyboardShortcut(Key.A, KeyModifiers.Control | KeyModifiers.Shift);

        // Act
        var str = shortcut.ToString();

        // Assert
        Assert.Contains("Ctrl", str);
        Assert.Contains("Shift", str);
        Assert.Contains("A", str);
    }

    [Fact]
    public void CommonShortcuts_ShouldBeDefined()
    {
        // Arrange & Act & Assert
        Assert.Equal(Key.Plus, CommonShortcuts.ZoomIn.Key);
        Assert.Equal(Key.Minus, CommonShortcuts.ZoomOut.Key);
        Assert.Equal(Key.Left, CommonShortcuts.PanLeft.Key);
        Assert.Equal(Key.Right, CommonShortcuts.PanRight.Key);
        Assert.Equal(Key.Home, CommonShortcuts.Home.Key);
        Assert.Equal(Key.End, CommonShortcuts.End.Key);
    }

    // Hit Testing Tests
    [Fact]
    public void HitTesting_HitTestRect_ShouldDetectHit()
    {
        // Arrange
        var rect = new SKRect(10, 10, 100, 100);

        // Act & Assert
        Assert.True(HitTesting.HitTestRect(new SKPoint(50, 50), rect));
        Assert.False(HitTesting.HitTestRect(new SKPoint(5, 5), rect));
        Assert.False(HitTesting.HitTestRect(new SKPoint(150, 150), rect));
    }

    [Fact]
    public void HitTesting_HitTestCircle_ShouldDetectHit()
    {
        // Arrange
        var center = new SKPoint(100, 100);
        var radius = 50f;

        // Act & Assert
        Assert.True(HitTesting.HitTestCircle(new SKPoint(100, 100), center, radius)); // Center
        Assert.True(HitTesting.HitTestCircle(new SKPoint(120, 100), center, radius)); // Inside
        Assert.False(HitTesting.HitTestCircle(new SKPoint(200, 100), center, radius)); // Outside
    }

    [Fact]
    public void HitTesting_HitTestLine_ShouldDetectNearbyPoint()
    {
        // Arrange
        var lineStart = new SKPoint(0, 0);
        var lineEnd = new SKPoint(100, 0);
        var threshold = 10f;

        // Act & Assert
        Assert.True(HitTesting.HitTestLine(new SKPoint(50, 0), lineStart, lineEnd, threshold)); // On line
        Assert.True(HitTesting.HitTestLine(new SKPoint(50, 5), lineStart, lineEnd, threshold)); // Near line
        Assert.False(HitTesting.HitTestLine(new SKPoint(50, 20), lineStart, lineEnd, threshold)); // Far from line
    }

    [Fact]
    public void HitTesting_ExpandRect_ShouldExpandCorrectly()
    {
        // Arrange
        var rect = new SKRect(10, 10, 20, 20);
        var margin = 5f;

        // Act
        var expanded = HitTesting.ExpandRect(rect, margin);

        // Assert
        Assert.Equal(5, expanded.Left);
        Assert.Equal(5, expanded.Top);
        Assert.Equal(25, expanded.Right);
        Assert.Equal(25, expanded.Bottom);
    }

    [Fact]
    public void HitTesting_EnsureMinimumTouchTarget_ShouldExpandSmallTargets()
    {
        // Arrange
        var center = new SKPoint(100, 100);
        var smallSize = 10f;

        // Act
        var touchTarget = HitTesting.EnsureMinimumTouchTarget(center, smallSize);

        // Assert
        Assert.True(touchTarget.Width >= HitTesting.MinimumTouchTargetSize);
        Assert.True(touchTarget.Height >= HitTesting.MinimumTouchTargetSize);
        Assert.Equal(100, touchTarget.MidX);
        Assert.Equal(100, touchTarget.MidY);
    }

    [Fact]
    public void TouchHitAreaManager_ShouldRegisterAndHitTest()
    {
        // Arrange
        var manager = new TouchHitAreaManager();
        var obj1 = new object();
        var obj2 = new object();

        manager.RegisterHitArea(obj1, new SKRect(0, 0, 100, 100));
        manager.RegisterHitArea(obj2, new SKRect(200, 200, 300, 300));

        // Act
        var hit1 = manager.HitTest(new SKPoint(50, 50));
        var hit2 = manager.HitTest(new SKPoint(250, 250));
        var miss = manager.HitTest(new SKPoint(500, 500));

        // Assert
        Assert.Equal(obj1, hit1);
        Assert.Equal(obj2, hit2);
        Assert.Null(miss);
    }

    [Fact]
    public void TouchHitAreaManager_ShouldExpandSmallAreas()
    {
        // Arrange
        var manager = new TouchHitAreaManager();
        var obj = new object();

        // Register very small hit area (10x10) centered at (105, 105)
        manager.RegisterHitArea(obj, new SKRect(100, 100, 110, 110));

        // Act - Test outside the original small area but within expanded area
        // With 44x44 minimum size, the expanded rect is approximately (83, 83, 127, 127)
        var hitInside = manager.HitTest(new SKPoint(90, 105)); // Within expanded area
        var hitOutside = manager.HitTest(new SKPoint(80, 80)); // Outside expanded area

        // Assert - Should hit because area was expanded to minimum touch target size
        Assert.Equal(obj, hitInside);
        Assert.Null(hitOutside);
    }

    [Fact]
    public void TouchHitAreaManager_HitTestAll_ShouldReturnAllHits()
    {
        // Arrange
        var manager = new TouchHitAreaManager();
        var obj1 = new object();
        var obj2 = new object();

        // Overlapping areas
        manager.RegisterHitArea(obj1, new SKRect(0, 0, 100, 100));
        manager.RegisterHitArea(obj2, new SKRect(50, 50, 150, 150));

        // Act
        var hits = manager.HitTestAll(new SKPoint(75, 75)); // In both areas

        // Assert
        Assert.Equal(2, hits.Count);
        Assert.Contains(obj1, hits);
        Assert.Contains(obj2, hits);
    }

    // Test Helper Classes
    private class TestInputHandler : InputHandlerBase
    {
        public bool MouseEventHandled { get; set; }
        public bool TouchEventHandled { get; set; }
        public bool KeyboardEventHandled { get; set; }
        public bool GestureEventHandled { get; set; }
        public bool ShouldHandleEvent { get; set; }
        public SKRect HitTestBounds { get; set; }
        public MouseEvent? LastMouseEvent { get; private set; }
        public Action<MouseEvent>? OnMouseEvent { get; set; }

        public override bool HandleMouseEvent(MouseEvent mouseEvent)
        {
            MouseEventHandled = true;
            LastMouseEvent = mouseEvent;
            OnMouseEvent?.Invoke(mouseEvent);
            if (ShouldHandleEvent)
            {
                mouseEvent.Handled = true;
                return true;
            }
            return false;
        }

        public override bool HandleTouchEvent(TouchEvent touchEvent)
        {
            TouchEventHandled = true;
            if (ShouldHandleEvent)
            {
                touchEvent.Handled = true;
                return true;
            }
            return false;
        }

        public override bool HandleKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            KeyboardEventHandled = true;
            if (ShouldHandleEvent)
            {
                keyboardEvent.Handled = true;
                return true;
            }
            return false;
        }

        public override bool HandleGestureEvent(GestureEvent gestureEvent)
        {
            GestureEventHandled = true;
            if (ShouldHandleEvent)
            {
                gestureEvent.Handled = true;
                return true;
            }
            return false;
        }

        public override bool HitTest(SKPoint position)
        {
            return HitTesting.HitTestRect(position, HitTestBounds);
        }
    }
}
