using SkiaSharp;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Base class for gesture recognizers.
/// </summary>
public abstract class GestureRecognizer
{
    /// <summary>
    /// Gets or sets whether this recognizer is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the current state of the gesture.
    /// </summary>
    public GestureState State { get; protected set; } = GestureState.Ended;

    /// <summary>
    /// Event raised when a gesture is recognized.
    /// </summary>
    public event EventHandler<GestureEvent>? GestureRecognized;

    /// <summary>
    /// Processes a touch event.
    /// </summary>
    /// <param name="touchEvent">The touch event.</param>
    public abstract void ProcessTouchEvent(TouchEvent touchEvent);

    /// <summary>
    /// Resets the recognizer state.
    /// </summary>
    public virtual void Reset()
    {
        State = GestureState.Ended;
    }

    /// <summary>
    /// Raises the gesture recognized event.
    /// </summary>
    protected void RaiseGestureRecognized(GestureEvent gestureEvent)
    {
        GestureRecognized?.Invoke(this, gestureEvent);
    }
}

/// <summary>
/// Recognizes pinch-to-zoom gestures.
/// </summary>
public class PinchGestureRecognizer : GestureRecognizer
{
    private SKPoint? _touch1Start;
    private SKPoint? _touch2Start;
    private float _initialDistance;
    private float _currentScale = 1.0f;

    /// <summary>
    /// Gets the minimum distance between touches to recognize a pinch.
    /// </summary>
    public float MinimumDistance { get; set; } = 20.0f;

    /// <inheritdoc/>
    public override void ProcessTouchEvent(TouchEvent touchEvent)
    {
        if (!IsEnabled || touchEvent.TouchPoints.Count < 2)
        {
            if (State != GestureState.Ended)
            {
                EndGesture();
            }
            return;
        }

        var touch1 = touchEvent.TouchPoints[0];
        var touch2 = touchEvent.TouchPoints[1];

        switch (touchEvent.EventType)
        {
            case TouchEventType.Begin:
                if (_touch1Start == null)
                {
                    _touch1Start = touch1.Position;
                    _touch2Start = touch2.Position;
                    _initialDistance = SKPoint.Distance(_touch1Start.Value, _touch2Start.Value);

                    if (_initialDistance >= MinimumDistance)
                    {
                        State = GestureState.Began;
                        _currentScale = 1.0f;
                        RaiseGesture(touch1, touch2, GestureState.Began);
                    }
                }
                break;

            case TouchEventType.Move:
                if (_touch1Start.HasValue && _touch2Start.HasValue)
                {
                    var currentDistance = SKPoint.Distance(touch1.Position, touch2.Position);
                    var scale = currentDistance / _initialDistance;

                    if (Math.Abs(scale - 1.0f) > 0.01f) // Threshold to avoid jitter
                    {
                        if (State == GestureState.Ended)
                        {
                            State = GestureState.Began;
                            RaiseGesture(touch1, touch2, GestureState.Began);
                        }
                        else
                        {
                            State = GestureState.Changed;
                            _currentScale = scale;
                            RaiseGesture(touch1, touch2, GestureState.Changed);
                        }
                    }
                }
                break;

            case TouchEventType.End:
            case TouchEventType.Cancel:
                EndGesture();
                break;
        }
    }

    private void RaiseGesture(TouchPoint touch1, TouchPoint touch2, GestureState state)
    {
        var center = new SKPoint(
            (touch1.Position.X + touch2.Position.X) / 2,
            (touch1.Position.Y + touch2.Position.Y) / 2
        );

        var gestureEvent = new GestureEvent
        {
            GestureType = GestureType.Pinch,
            Center = center,
            Scale = _currentScale,
            State = state
        };

        RaiseGestureRecognized(gestureEvent);
    }

    private void EndGesture()
    {
        if (State != GestureState.Ended)
        {
            State = GestureState.Ended;
            _touch1Start = null;
            _touch2Start = null;
        }
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        _touch1Start = null;
        _touch2Start = null;
        _initialDistance = 0;
        _currentScale = 1.0f;
    }
}

/// <summary>
/// Recognizes pan/swipe gestures.
/// </summary>
public class PanGestureRecognizer : GestureRecognizer
{
    private SKPoint? _startPosition;
    private SKPoint _lastPosition;
    private DateTime _startTime;
    private readonly List<(SKPoint Position, DateTime Time)> _velocityBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PanGestureRecognizer"/> class.
    /// </summary>
    public PanGestureRecognizer()
    {
        _velocityBuffer = new List<(SKPoint, DateTime)>();
    }

    /// <summary>
    /// Gets the minimum distance to recognize a pan.
    /// </summary>
    public float MinimumDistance { get; set; } = 10.0f;

    /// <summary>
    /// Gets the velocity threshold for swipe recognition.
    /// </summary>
    public float SwipeVelocityThreshold { get; set; } = 1000.0f;

    /// <summary>
    /// Gets the maximum number of touches (1 = single-finger pan).
    /// </summary>
    public int MaximumTouches { get; set; } = 1;

    /// <inheritdoc/>
    public override void ProcessTouchEvent(TouchEvent touchEvent)
    {
        if (!IsEnabled || touchEvent.TouchPoints.Count == 0 || touchEvent.TouchPoints.Count > MaximumTouches)
        {
            if (State != GestureState.Ended)
            {
                EndGesture();
            }
            return;
        }

        var touch = touchEvent.PrimaryTouchPoint!;

        switch (touchEvent.EventType)
        {
            case TouchEventType.Begin:
                _startPosition = touch.Position;
                _lastPosition = touch.Position;
                _startTime = touchEvent.Timestamp;
                _velocityBuffer.Clear();
                _velocityBuffer.Add((touch.Position, touchEvent.Timestamp));
                State = GestureState.Began;
                break;

            case TouchEventType.Move:
                if (_startPosition.HasValue)
                {
                    var delta = new SKPoint(
                        touch.Position.X - _lastPosition.X,
                        touch.Position.Y - _lastPosition.Y
                    );

                    var totalDistance = SKPoint.Distance(_startPosition.Value, touch.Position);

                    if (totalDistance >= MinimumDistance || State == GestureState.Changed)
                    {
                        State = GestureState.Changed;
                        _velocityBuffer.Add((touch.Position, touchEvent.Timestamp));

                        // Keep only recent positions for velocity calculation
                        if (_velocityBuffer.Count > 10)
                        {
                            _velocityBuffer.RemoveAt(0);
                        }

                        var gestureEvent = new GestureEvent
                        {
                            GestureType = GestureType.Pan,
                            Center = touch.Position,
                            Delta = delta,
                            State = GestureState.Changed
                        };

                        RaiseGestureRecognized(gestureEvent);
                        _lastPosition = touch.Position;
                    }
                }
                break;

            case TouchEventType.End:
                if (_startPosition.HasValue)
                {
                    var velocity = CalculateVelocity();
                    var velocityMagnitude = (float)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);

                    // Recognize swipe if velocity is high enough
                    if (velocityMagnitude >= SwipeVelocityThreshold)
                    {
                        var swipeEvent = new GestureEvent
                        {
                            GestureType = GestureType.Swipe,
                            Center = touch.Position,
                            Velocity = velocity,
                            State = GestureState.Ended
                        };
                        RaiseGestureRecognized(swipeEvent);
                    }

                    var endEvent = new GestureEvent
                    {
                        GestureType = GestureType.Pan,
                        Center = touch.Position,
                        Velocity = velocity,
                        State = GestureState.Ended
                    };
                    RaiseGestureRecognized(endEvent);
                }
                EndGesture();
                break;

            case TouchEventType.Cancel:
                EndGesture();
                break;
        }
    }

    private SKPoint CalculateVelocity()
    {
        if (_velocityBuffer.Count < 2)
            return SKPoint.Empty;

        var first = _velocityBuffer[0];
        var last = _velocityBuffer[_velocityBuffer.Count - 1];

        var deltaTime = (last.Time - first.Time).TotalSeconds;
        if (deltaTime <= 0)
            return SKPoint.Empty;

        var deltaX = last.Position.X - first.Position.X;
        var deltaY = last.Position.Y - first.Position.Y;

        return new SKPoint(
            (float)(deltaX / deltaTime),
            (float)(deltaY / deltaTime)
        );
    }

    private void EndGesture()
    {
        State = GestureState.Ended;
        _startPosition = null;
        _velocityBuffer.Clear();
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        _startPosition = null;
        _velocityBuffer.Clear();
    }
}

/// <summary>
/// Recognizes tap gestures.
/// </summary>
public class TapGestureRecognizer : GestureRecognizer
{
    private SKPoint? _startPosition;
    private DateTime _startTime;

    /// <summary>
    /// Gets the maximum duration for a tap in milliseconds.
    /// </summary>
    public int MaximumDuration { get; set; } = 300;

    /// <summary>
    /// Gets the maximum distance the touch can move.
    /// </summary>
    public float MaximumDistance { get; set; } = 10.0f;

    /// <summary>
    /// Gets the number of taps required (1 for single tap, 2 for double tap).
    /// </summary>
    public int NumberOfTapsRequired { get; set; } = 1;

    /// <inheritdoc/>
    public override void ProcessTouchEvent(TouchEvent touchEvent)
    {
        if (!IsEnabled || touchEvent.TouchPoints.Count != 1)
        {
            Reset();
            return;
        }

        var touch = touchEvent.PrimaryTouchPoint!;

        switch (touchEvent.EventType)
        {
            case TouchEventType.Begin:
                _startPosition = touch.Position;
                _startTime = touchEvent.Timestamp;
                State = GestureState.Began;
                break;

            case TouchEventType.End:
                if (_startPosition.HasValue)
                {
                    var duration = (touchEvent.Timestamp - _startTime).TotalMilliseconds;
                    var distance = SKPoint.Distance(_startPosition.Value, touch.Position);

                    if (duration <= MaximumDuration && distance <= MaximumDistance)
                    {
                        var gestureType = NumberOfTapsRequired == 2 ? GestureType.DoubleTap : GestureType.Tap;

                        var gestureEvent = new GestureEvent
                        {
                            GestureType = gestureType,
                            Center = touch.Position,
                            State = GestureState.Ended
                        };

                        RaiseGestureRecognized(gestureEvent);
                    }
                }
                Reset();
                break;

            case TouchEventType.Cancel:
                Reset();
                break;
        }
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        _startPosition = null;
    }
}

/// <summary>
/// Recognizes long press gestures.
/// </summary>
public class LongPressGestureRecognizer : GestureRecognizer
{
    private SKPoint? _startPosition;
    private DateTime _startTime;
    private System.Threading.Timer? _timer;

    /// <summary>
    /// Gets the minimum duration for a long press in milliseconds.
    /// </summary>
    public int MinimumDuration { get; set; } = 500;

    /// <summary>
    /// Gets the maximum distance the touch can move.
    /// </summary>
    public float MaximumDistance { get; set; } = 10.0f;

    /// <inheritdoc/>
    public override void ProcessTouchEvent(TouchEvent touchEvent)
    {
        if (!IsEnabled || touchEvent.TouchPoints.Count != 1)
        {
            CancelTimer();
            Reset();
            return;
        }

        var touch = touchEvent.PrimaryTouchPoint!;

        switch (touchEvent.EventType)
        {
            case TouchEventType.Begin:
                _startPosition = touch.Position;
                _startTime = touchEvent.Timestamp;
                State = GestureState.Began;

                // Start timer for long press
                _timer = new System.Threading.Timer(_ =>
                {
                    if (State == GestureState.Began && _startPosition.HasValue)
                    {
                        var gestureEvent = new GestureEvent
                        {
                            GestureType = GestureType.LongPress,
                            Center = _startPosition.Value,
                            State = GestureState.Ended
                        };

                        State = GestureState.Ended;
                        RaiseGestureRecognized(gestureEvent);
                    }
                }, null, MinimumDuration, Timeout.Infinite);
                break;

            case TouchEventType.Move:
                if (_startPosition.HasValue)
                {
                    var distance = SKPoint.Distance(_startPosition.Value, touch.Position);
                    if (distance > MaximumDistance)
                    {
                        CancelTimer();
                        Reset();
                    }
                }
                break;

            case TouchEventType.End:
            case TouchEventType.Cancel:
                CancelTimer();
                Reset();
                break;
        }
    }

    private void CancelTimer()
    {
        _timer?.Dispose();
        _timer = null;
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        CancelTimer();
        _startPosition = null;
    }
}
