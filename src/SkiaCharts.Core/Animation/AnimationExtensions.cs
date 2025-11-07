using SkiaCharts.Core.Charts;
using SkiaSharp;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Provides extension methods for creating and chaining animations.
/// </summary>
public static class AnimationExtensions
{
    /// <summary>
    /// Creates a fluent animation builder for a value.
    /// </summary>
    public static AnimationBuilder<T> Animate<T>(this T startValue)
    {
        return new AnimationBuilder<T>(startValue);
    }

    /// <summary>
    /// Animates a double value.
    /// </summary>
    public static Animation<double> AnimateTo(
        this double from,
        double to,
        double duration,
        IEasingFunction? easing = null)
    {
        var animation = new Animation<double>(from, to, duration, Interpolators.Double)
        {
            EasingFunction = easing ?? EasingFunctions.Linear
        };
        return animation;
    }

    /// <summary>
    /// Animates a color value.
    /// </summary>
    public static Animation<SKColor> AnimateTo(
        this SKColor from,
        SKColor to,
        double duration,
        IEasingFunction? easing = null)
    {
        var animation = new Animation<SKColor>(from, to, duration, Interpolators.Color)
        {
            EasingFunction = easing ?? EasingFunctions.Linear
        };
        return animation;
    }

    /// <summary>
    /// Animates a point value.
    /// </summary>
    public static Animation<SKPoint> AnimateTo(
        this SKPoint from,
        SKPoint to,
        double duration,
        IEasingFunction? easing = null)
    {
        var animation = new Animation<SKPoint>(from, to, duration, Interpolators.Point)
        {
            EasingFunction = easing ?? EasingFunctions.Linear
        };
        return animation;
    }

    /// <summary>
    /// Adds a delay before the animation starts.
    /// </summary>
    public static Animation<T> WithDelay<T>(this Animation<T> animation, double delay)
    {
        animation.Delay = delay;
        return animation;
    }

    /// <summary>
    /// Sets the easing function for the animation.
    /// </summary>
    public static Animation<T> WithEasing<T>(this Animation<T> animation, IEasingFunction easing)
    {
        animation.EasingFunction = easing;
        return animation;
    }

    /// <summary>
    /// Makes the animation loop indefinitely.
    /// </summary>
    public static Animation<T> Repeat<T>(this Animation<T> animation, bool autoReverse = false)
    {
        animation.Loop = true;
        animation.AutoReverse = autoReverse;
        return animation;
    }

    /// <summary>
    /// Adds a callback when the animation starts.
    /// </summary>
    public static Animation<T> OnStart<T>(this Animation<T> animation, Action callback)
    {
        animation.Started += (s, e) => callback();
        return animation;
    }

    /// <summary>
    /// Adds a callback when the animation updates.
    /// </summary>
    public static Animation<T> OnUpdate<T>(this Animation<T> animation, Action<T> callback)
    {
        animation.Updated += (s, e) => callback(e.Value);
        return animation;
    }

    /// <summary>
    /// Adds a callback when the animation completes.
    /// </summary>
    public static Animation<T> OnComplete<T>(this Animation<T> animation, Action callback)
    {
        animation.Completed += (s, e) => callback();
        return animation;
    }

    /// <summary>
    /// Starts the animation immediately.
    /// </summary>
    public static Animation<T> StartAnimation<T>(this Animation<T> animation)
    {
        animation.Start();
        return animation;
    }
}

/// <summary>
/// Fluent animation builder.
/// </summary>
public class AnimationBuilder<T>
{
    private readonly T _startValue;
    private T _endValue = default!;
    private double _duration = 1.0;
    private IEasingFunction _easing = EasingFunctions.Linear;
    private double _delay;
    private bool _loop;
    private bool _autoReverse;
    private readonly List<Action<T>> _updateCallbacks = new();
    private readonly List<Action> _startCallbacks = new();
    private readonly List<Action> _completeCallbacks = new();

    internal AnimationBuilder(T startValue)
    {
        _startValue = startValue;
    }

    /// <summary>
    /// Sets the target value.
    /// </summary>
    public AnimationBuilder<T> To(T endValue)
    {
        _endValue = endValue;
        return this;
    }

    /// <summary>
    /// Sets the duration.
    /// </summary>
    public AnimationBuilder<T> For(double duration)
    {
        _duration = duration;
        return this;
    }

    /// <summary>
    /// Sets the easing function.
    /// </summary>
    public AnimationBuilder<T> With(IEasingFunction easing)
    {
        _easing = easing;
        return this;
    }

    /// <summary>
    /// Sets a delay before starting.
    /// </summary>
    public AnimationBuilder<T> After(double delay)
    {
        _delay = delay;
        return this;
    }

    /// <summary>
    /// Makes the animation loop.
    /// </summary>
    public AnimationBuilder<T> Loop(bool autoReverse = false)
    {
        _loop = true;
        _autoReverse = autoReverse;
        return this;
    }

    /// <summary>
    /// Adds an update callback.
    /// </summary>
    public AnimationBuilder<T> OnUpdate(Action<T> callback)
    {
        _updateCallbacks.Add(callback);
        return this;
    }

    /// <summary>
    /// Adds a start callback.
    /// </summary>
    public AnimationBuilder<T> OnStart(Action callback)
    {
        _startCallbacks.Add(callback);
        return this;
    }

    /// <summary>
    /// Adds a complete callback.
    /// </summary>
    public AnimationBuilder<T> OnComplete(Action callback)
    {
        _completeCallbacks.Add(callback);
        return this;
    }

    /// <summary>
    /// Builds and returns the animation.
    /// </summary>
    public Animation<T> Build(Func<T, T, double, T> interpolator)
    {
        var animation = new Animation<T>(_startValue, _endValue, _duration, interpolator)
        {
            EasingFunction = _easing,
            Delay = _delay,
            Loop = _loop,
            AutoReverse = _autoReverse
        };

        // Attach callbacks
        foreach (var callback in _startCallbacks)
        {
            animation.Started += (s, e) => callback();
        }

        foreach (var callback in _updateCallbacks)
        {
            animation.Updated += (s, e) => callback(e.Value);
        }

        foreach (var callback in _completeCallbacks)
        {
            animation.Completed += (s, e) => callback();
        }

        return animation;
    }

    /// <summary>
    /// Builds and starts the animation.
    /// </summary>
    public Animation<T> Start(Func<T, T, double, T> interpolator)
    {
        var animation = Build(interpolator);
        animation.Start();
        return animation;
    }
}
