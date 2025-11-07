namespace SkiaCharts.Core.Animation;

/// <summary>
/// Provides preset animation configurations.
/// </summary>
public static class AnimationPresets
{
    /// <summary>
    /// Fast animation (0.2 seconds with QuadOut easing).
    /// </summary>
    public static AnimationPreset Fast => new()
    {
        Duration = 0.2,
        EasingFunction = EasingFunctions.QuadOut
    };

    /// <summary>
    /// Normal animation (0.5 seconds with CubicOut easing).
    /// </summary>
    public static AnimationPreset Normal => new()
    {
        Duration = 0.5,
        EasingFunction = EasingFunctions.CubicOut
    };

    /// <summary>
    /// Slow animation (1.0 second with CubicInOut easing).
    /// </summary>
    public static AnimationPreset Slow => new()
    {
        Duration = 1.0,
        EasingFunction = EasingFunctions.CubicInOut
    };

    /// <summary>
    /// Smooth animation (0.6 seconds with SineInOut easing).
    /// </summary>
    public static AnimationPreset Smooth => new()
    {
        Duration = 0.6,
        EasingFunction = EasingFunctions.SineInOut
    };

    /// <summary>
    /// Bouncy animation (0.8 seconds with BounceOut easing).
    /// </summary>
    public static AnimationPreset Bouncy => new()
    {
        Duration = 0.8,
        EasingFunction = EasingFunctions.BounceOut
    };

    /// <summary>
    /// Elastic animation (1.0 second with ElasticOut easing).
    /// </summary>
    public static AnimationPreset Elastic => new()
    {
        Duration = 1.0,
        EasingFunction = EasingFunctions.ElasticOut
    };

    /// <summary>
    /// Snappy animation (0.3 seconds with ExpoOut easing).
    /// </summary>
    public static AnimationPreset Snappy => new()
    {
        Duration = 0.3,
        EasingFunction = EasingFunctions.ExpoOut
    };
}

/// <summary>
/// Represents a preset animation configuration.
/// </summary>
public class AnimationPreset
{
    /// <summary>
    /// Gets or sets the duration in seconds.
    /// </summary>
    public double Duration { get; set; }

    /// <summary>
    /// Gets or sets the easing function.
    /// </summary>
    public IEasingFunction EasingFunction { get; set; } = EasingFunctions.Linear;

    /// <summary>
    /// Creates an animation with this preset.
    /// </summary>
    public Animation<T> Create<T>(T from, T to, Func<T, T, double, T> interpolator)
    {
        return new Animation<T>(from, to, Duration, interpolator)
        {
            EasingFunction = EasingFunction
        };
    }
}
