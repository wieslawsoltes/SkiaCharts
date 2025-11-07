using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Provides navigation behaviors for charts (pan, zoom, etc.).
/// </summary>
public class NavigationBehavior : InputHandlerBase
{
    private readonly Viewport _viewport;
    private SKPoint? _panStartPosition;
    private SKRect? _boxSelectionStart;
    private readonly PinchGestureRecognizer _pinchRecognizer;
    private readonly PanGestureRecognizer _panRecognizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationBehavior"/> class.
    /// </summary>
    /// <param name="viewport">The viewport to control.</param>
    public NavigationBehavior(Viewport viewport)
    {
        _viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
        Priority = 100; // High priority for navigation

        // Setup gesture recognizers
        _pinchRecognizer = new PinchGestureRecognizer();
        _pinchRecognizer.GestureRecognized += OnPinchGesture;

        _panRecognizer = new PanGestureRecognizer { MaximumTouches = 2 };
        _panRecognizer.GestureRecognized += OnPanGesture;

        // Default settings
        MouseWheelZoomEnabled = true;
        MouseWheelZoomFactor = 1.2f;
        DragToPanEnabled = true;
        DragToPanButton = MouseButton.Left;
        PinchToZoomEnabled = true;
        BoxSelectionZoomEnabled = true;
        BoxSelectionModifier = KeyModifiers.Shift;
        KeyboardNavigationEnabled = true;
        KeyboardPanStep = 0.1f; // 10% of view
        KeyboardZoomFactor = 1.2f;
    }

    /// <summary>
    /// Gets or sets whether mouse wheel zoom is enabled.
    /// </summary>
    public bool MouseWheelZoomEnabled { get; set; }

    /// <summary>
    /// Gets or sets the mouse wheel zoom factor.
    /// </summary>
    public float MouseWheelZoomFactor { get; set; }

    /// <summary>
    /// Gets or sets whether drag-to-pan is enabled.
    /// </summary>
    public bool DragToPanEnabled { get; set; }

    /// <summary>
    /// Gets or sets the mouse button for drag-to-pan.
    /// </summary>
    public MouseButton DragToPanButton { get; set; }

    /// <summary>
    /// Gets or sets whether pinch-to-zoom is enabled.
    /// </summary>
    public bool PinchToZoomEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether box selection zoom is enabled.
    /// </summary>
    public bool BoxSelectionZoomEnabled { get; set; }

    /// <summary>
    /// Gets or sets the modifier key for box selection.
    /// </summary>
    public KeyModifiers BoxSelectionModifier { get; set; }

    /// <summary>
    /// Gets or sets whether keyboard navigation is enabled.
    /// </summary>
    public bool KeyboardNavigationEnabled { get; set; }

    /// <summary>
    /// Gets or sets the keyboard pan step (as fraction of view).
    /// </summary>
    public float KeyboardPanStep { get; set; }

    /// <summary>
    /// Gets or sets the keyboard zoom factor.
    /// </summary>
    public float KeyboardZoomFactor { get; set; }

    /// <summary>
    /// Gets or sets whether to animate transitions.
    /// </summary>
    public bool AnimateTransitions { get; set; }

    /// <summary>
    /// Event raised when box selection is in progress.
    /// </summary>
    public event EventHandler<BoxSelectionEventArgs>? BoxSelectionChanged;

    /// <inheritdoc/>
    public override bool HandleMouseEvent(MouseEvent mouseEvent)
    {
        switch (mouseEvent.EventType)
        {
            case MouseEventType.Wheel:
                return HandleMouseWheel(mouseEvent);

            case MouseEventType.Down:
                return HandleMouseDown(mouseEvent);

            case MouseEventType.Move:
                return HandleMouseMove(mouseEvent);

            case MouseEventType.Up:
                return HandleMouseUp(mouseEvent);
        }

        return false;
    }

    /// <inheritdoc/>
    public override bool HandleTouchEvent(TouchEvent touchEvent)
    {
        // Process gestures
        _pinchRecognizer.ProcessTouchEvent(touchEvent);
        _panRecognizer.ProcessTouchEvent(touchEvent);

        return false;
    }

    /// <inheritdoc/>
    public override bool HandleKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        if (!KeyboardNavigationEnabled || keyboardEvent.EventType != KeyboardEventType.KeyDown)
            return false;

        switch (keyboardEvent.Key)
        {
            case Key.Left:
                PanLeft();
                keyboardEvent.Handled = true;
                return true;

            case Key.Right:
                PanRight();
                keyboardEvent.Handled = true;
                return true;

            case Key.Up:
                PanUp();
                keyboardEvent.Handled = true;
                return true;

            case Key.Down:
                PanDown();
                keyboardEvent.Handled = true;
                return true;

            case Key.Plus when keyboardEvent.Modifiers.HasFlag(KeyModifiers.Control):
                _viewport.ZoomIn(KeyboardZoomFactor);
                keyboardEvent.Handled = true;
                return true;

            case Key.Minus when keyboardEvent.Modifiers.HasFlag(KeyModifiers.Control):
                _viewport.ZoomOut(KeyboardZoomFactor);
                keyboardEvent.Handled = true;
                return true;

            case Key.D0 when keyboardEvent.Modifiers.HasFlag(KeyModifiers.Control):
                _viewport.Reset();
                keyboardEvent.Handled = true;
                return true;

            case Key.Home:
                PanToStart();
                keyboardEvent.Handled = true;
                return true;

            case Key.End:
                PanToEnd();
                keyboardEvent.Handled = true;
                return true;

            case Key.PageUp:
                _viewport.ZoomIn(KeyboardZoomFactor);
                keyboardEvent.Handled = true;
                return true;

            case Key.PageDown:
                _viewport.ZoomOut(KeyboardZoomFactor);
                keyboardEvent.Handled = true;
                return true;
        }

        return false;
    }

    private bool HandleMouseWheel(MouseEvent mouseEvent)
    {
        if (!MouseWheelZoomEnabled)
            return false;

        var factor = mouseEvent.WheelDelta > 0 ? MouseWheelZoomFactor : 1.0f / MouseWheelZoomFactor;
        _viewport.ZoomBy(factor, mouseEvent.Position);

        mouseEvent.Handled = true;
        return true;
    }

    private bool HandleMouseDown(MouseEvent mouseEvent)
    {
        if (BoxSelectionZoomEnabled && mouseEvent.Modifiers.HasFlag(BoxSelectionModifier))
        {
            // Start box selection
            _boxSelectionStart = new SKRect(
                mouseEvent.Position.X,
                mouseEvent.Position.Y,
                mouseEvent.Position.X,
                mouseEvent.Position.Y
            );
            mouseEvent.Handled = true;
            return true;
        }

        if (DragToPanEnabled && mouseEvent.Button == DragToPanButton)
        {
            // Start drag-to-pan
            _panStartPosition = mouseEvent.Position;
            mouseEvent.Handled = true;
            return true;
        }

        return false;
    }

    private bool HandleMouseMove(MouseEvent mouseEvent)
    {
        if (_boxSelectionStart.HasValue)
        {
            // Update box selection
            var box = _boxSelectionStart.Value;
            box.Right = mouseEvent.Position.X;
            box.Bottom = mouseEvent.Position.Y;
            _boxSelectionStart = box;

            BoxSelectionChanged?.Invoke(this, new BoxSelectionEventArgs { SelectionRect = box });

            mouseEvent.Handled = true;
            return true;
        }

        if (_panStartPosition.HasValue)
        {
            // Update pan
            var delta = new SKPoint(
                mouseEvent.Position.X - _panStartPosition.Value.X,
                mouseEvent.Position.Y - _panStartPosition.Value.Y
            );

            _viewport.PanBy(delta);
            _panStartPosition = mouseEvent.Position;

            mouseEvent.Handled = true;
            return true;
        }

        return false;
    }

    private bool HandleMouseUp(MouseEvent mouseEvent)
    {
        if (_boxSelectionStart.HasValue)
        {
            // Complete box selection zoom
            var box = _boxSelectionStart.Value;
            box.Right = mouseEvent.Position.X;
            box.Bottom = mouseEvent.Position.Y;

            // Standardize the rectangle
            var rect = new SKRect(
                Math.Min(box.Left, box.Right),
                Math.Min(box.Top, box.Bottom),
                Math.Max(box.Left, box.Right),
                Math.Max(box.Top, box.Bottom)
            );

            // Only zoom if the selection is large enough
            if (rect.Width > 10 && rect.Height > 10)
            {
                var dataRect = new SKRect(
                    _viewport.ScreenToData(new SKPoint(rect.Left, rect.Top)).X,
                    _viewport.ScreenToData(new SKPoint(rect.Left, rect.Top)).Y,
                    _viewport.ScreenToData(new SKPoint(rect.Right, rect.Bottom)).X,
                    _viewport.ScreenToData(new SKPoint(rect.Right, rect.Bottom)).Y
                );

                _viewport.ZoomToRect(dataRect);
            }

            _boxSelectionStart = null;
            BoxSelectionChanged?.Invoke(this, new BoxSelectionEventArgs { SelectionRect = null });

            mouseEvent.Handled = true;
            return true;
        }

        if (_panStartPosition.HasValue)
        {
            _panStartPosition = null;
            mouseEvent.Handled = true;
            return true;
        }

        return false;
    }

    private void OnPinchGesture(object? sender, GestureEvent e)
    {
        if (!PinchToZoomEnabled)
            return;

        if (e.State == GestureState.Changed)
        {
            var factor = e.Scale;
            _viewport.ZoomBy(factor, e.Center);
        }
    }

    private void OnPanGesture(object? sender, GestureEvent e)
    {
        if (e.State == GestureState.Changed && e.GestureType == GestureType.Pan)
        {
            _viewport.PanBy(e.Delta);
        }
    }

    private void PanLeft()
    {
        var delta = new SKPoint(_viewport.ViewBounds.Width * KeyboardPanStep, 0);
        _viewport.PanBy(delta);
    }

    private void PanRight()
    {
        var delta = new SKPoint(-_viewport.ViewBounds.Width * KeyboardPanStep, 0);
        _viewport.PanBy(delta);
    }

    private void PanUp()
    {
        var delta = new SKPoint(0, _viewport.ViewBounds.Height * KeyboardPanStep);
        _viewport.PanBy(delta);
    }

    private void PanDown()
    {
        var delta = new SKPoint(0, -_viewport.ViewBounds.Height * KeyboardPanStep);
        _viewport.PanBy(delta);
    }

    private void PanToStart()
    {
        var visible = _viewport.VisibleDataRect;
        _viewport.Pan = new SKPoint(_viewport.DataBounds.Left, visible.Top);
    }

    private void PanToEnd()
    {
        var visible = _viewport.VisibleDataRect;
        _viewport.Pan = new SKPoint(
            _viewport.DataBounds.Right - visible.Width,
            visible.Top
        );
    }

    /// <summary>
    /// Gets the current box selection rectangle (if active).
    /// </summary>
    public SKRect? BoxSelectionRect => _boxSelectionStart;
}

/// <summary>
/// Event args for box selection changes.
/// </summary>
public class BoxSelectionEventArgs : EventArgs
{
    /// <summary>
    /// Gets the current selection rectangle (null if not active).
    /// </summary>
    public SKRect? SelectionRect { get; init; }
}
