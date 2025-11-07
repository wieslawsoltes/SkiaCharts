using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Routes input events to appropriate handlers.
/// Manages event bubbling and hit testing.
/// </summary>
public class InputRouter
{
    private readonly List<IInputHandler> _handlers;
    private readonly Dictionary<long, TouchPoint> _activeTouches;
    private SKPoint? _lastMousePosition;
    private DateTime _lastClickTime;
    private int _consecutiveClicks;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputRouter"/> class.
    /// </summary>
    public InputRouter()
    {
        _handlers = new List<IInputHandler>();
        _activeTouches = new Dictionary<long, TouchPoint>();
        _lastClickTime = DateTime.MinValue;
        _consecutiveClicks = 0;
    }

    /// <summary>
    /// Gets the threshold for double-click detection in milliseconds.
    /// </summary>
    public int DoubleClickThreshold { get; set; } = 300;

    /// <summary>
    /// Gets the threshold for double-click distance in pixels.
    /// </summary>
    public float DoubleClickDistance { get; set; } = 5.0f;

    /// <summary>
    /// Registers an input handler.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public void RegisterHandler(IInputHandler handler)
    {
        if (!_handlers.Contains(handler))
        {
            _handlers.Add(handler);
            // Sort by priority (higher priority first)
            _handlers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
    }

    /// <summary>
    /// Unregisters an input handler.
    /// </summary>
    /// <param name="handler">The handler to unregister.</param>
    public void UnregisterHandler(IInputHandler handler)
    {
        _handlers.Remove(handler);
    }

    /// <summary>
    /// Routes a mouse event to handlers.
    /// </summary>
    /// <param name="mouseEvent">The mouse event to route.</param>
    /// <returns>True if the event was handled.</returns>
    public bool RouteMouseEvent(MouseEvent mouseEvent)
    {
        // Detect double-click
        if (mouseEvent.EventType == MouseEventType.Click)
        {
            var timeSinceLastClick = (mouseEvent.Timestamp - _lastClickTime).TotalMilliseconds;
            var distance = _lastMousePosition.HasValue
                ? SKPoint.Distance(mouseEvent.Position, _lastMousePosition.Value)
                : float.MaxValue;

            if (timeSinceLastClick < DoubleClickThreshold && distance < DoubleClickDistance)
            {
                _consecutiveClicks++;
                if (_consecutiveClicks == 2)
                {
                    var doubleClickEvent = new MouseEvent
                    {
                        Position = mouseEvent.Position,
                        Button = mouseEvent.Button,
                        EventType = MouseEventType.DoubleClick,
                        Modifiers = mouseEvent.Modifiers,
                        Timestamp = mouseEvent.Timestamp,
                        ClickCount = 2
                    };
                    RouteEvent(doubleClickEvent);
                    _consecutiveClicks = 0;
                }
            }
            else
            {
                _consecutiveClicks = 1;
            }

            _lastClickTime = mouseEvent.Timestamp;
            _lastMousePosition = mouseEvent.Position;
        }

        // Update last mouse position
        if (mouseEvent.EventType == MouseEventType.Move)
        {
            _lastMousePosition = mouseEvent.Position;
        }

        return RouteEvent(mouseEvent);
    }

    /// <summary>
    /// Routes a touch event to handlers.
    /// </summary>
    /// <param name="touchEvent">The touch event to route.</param>
    /// <returns>True if the event was handled.</returns>
    public bool RouteTouchEvent(TouchEvent touchEvent)
    {
        // Update active touches
        foreach (var touchPoint in touchEvent.TouchPoints)
        {
            switch (touchPoint.State)
            {
                case TouchState.Began:
                    _activeTouches[touchPoint.Id] = touchPoint;
                    break;
                case TouchState.Ended:
                case TouchState.Cancelled:
                    _activeTouches.Remove(touchPoint.Id);
                    break;
                default:
                    _activeTouches[touchPoint.Id] = touchPoint;
                    break;
            }
        }

        return RouteEvent(touchEvent);
    }

    /// <summary>
    /// Routes a keyboard event to handlers.
    /// </summary>
    /// <param name="keyboardEvent">The keyboard event to route.</param>
    /// <returns>True if the event was handled.</returns>
    public bool RouteKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        return RouteEvent(keyboardEvent);
    }

    /// <summary>
    /// Routes a gesture event to handlers.
    /// </summary>
    /// <param name="gestureEvent">The gesture event to route.</param>
    /// <returns>True if the event was handled.</returns>
    public bool RouteGestureEvent(GestureEvent gestureEvent)
    {
        return RouteEvent(gestureEvent);
    }

    /// <summary>
    /// Routes an event to registered handlers.
    /// </summary>
    private bool RouteEvent(InputEvent inputEvent)
    {
        // Route to handlers in priority order
        foreach (var handler in _handlers)
        {
            if (!handler.IsEnabled)
                continue;

            // Check if handler can handle this event type
            bool handled = inputEvent switch
            {
                MouseEvent me => handler.HandleMouseEvent(me),
                TouchEvent te => handler.HandleTouchEvent(te),
                KeyboardEvent ke => handler.HandleKeyboardEvent(ke),
                GestureEvent ge => handler.HandleGestureEvent(ge),
                _ => false
            };

            if (handled && inputEvent.Handled)
            {
                return true;
            }
        }

        return inputEvent.Handled;
    }

    /// <summary>
    /// Performs hit testing at the specified position.
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <returns>The handler that was hit, or null.</returns>
    public IInputHandler? HitTest(SKPoint position)
    {
        // Test handlers in priority order
        foreach (var handler in _handlers)
        {
            if (!handler.IsEnabled)
                continue;

            if (handler.HitTest(position))
            {
                return handler;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all active touch points.
    /// </summary>
    public IReadOnlyDictionary<long, TouchPoint> ActiveTouches => _activeTouches;

    /// <summary>
    /// Gets the last known mouse position.
    /// </summary>
    public SKPoint? LastMousePosition => _lastMousePosition;

    /// <summary>
    /// Clears all state.
    /// </summary>
    public void Clear()
    {
        _activeTouches.Clear();
        _lastMousePosition = null;
        _consecutiveClicks = 0;
    }
}

/// <summary>
/// Interface for input handlers.
/// </summary>
public interface IInputHandler
{
    /// <summary>
    /// Gets the priority of this handler (higher = processed first).
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets whether this handler is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Handles a mouse event.
    /// </summary>
    /// <param name="mouseEvent">The mouse event.</param>
    /// <returns>True if the event was handled.</returns>
    bool HandleMouseEvent(MouseEvent mouseEvent);

    /// <summary>
    /// Handles a touch event.
    /// </summary>
    /// <param name="touchEvent">The touch event.</param>
    /// <returns>True if the event was handled.</returns>
    bool HandleTouchEvent(TouchEvent touchEvent);

    /// <summary>
    /// Handles a keyboard event.
    /// </summary>
    /// <param name="keyboardEvent">The keyboard event.</param>
    /// <returns>True if the event was handled.</returns>
    bool HandleKeyboardEvent(KeyboardEvent keyboardEvent);

    /// <summary>
    /// Handles a gesture event.
    /// </summary>
    /// <param name="gestureEvent">The gesture event.</param>
    /// <returns>True if the event was handled.</returns>
    bool HandleGestureEvent(GestureEvent gestureEvent);

    /// <summary>
    /// Performs hit testing at the specified position.
    /// </summary>
    /// <param name="position">The position to test.</param>
    /// <returns>True if the position hits this handler.</returns>
    bool HitTest(SKPoint position);
}

/// <summary>
/// Base implementation of an input handler.
/// </summary>
public abstract class InputHandlerBase : IInputHandler
{
    /// <inheritdoc/>
    public virtual int Priority { get; set; } = 0;

    /// <inheritdoc/>
    public virtual bool IsEnabled { get; set; } = true;

    /// <inheritdoc/>
    public virtual bool HandleMouseEvent(MouseEvent mouseEvent) => false;

    /// <inheritdoc/>
    public virtual bool HandleTouchEvent(TouchEvent touchEvent) => false;

    /// <inheritdoc/>
    public virtual bool HandleKeyboardEvent(KeyboardEvent keyboardEvent) => false;

    /// <inheritdoc/>
    public virtual bool HandleGestureEvent(GestureEvent gestureEvent) => false;

    /// <inheritdoc/>
    public virtual bool HitTest(SKPoint position) => false;
}
