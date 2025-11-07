using SkiaCharts.Core.Charts;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Base class for chart-specific animations.
/// </summary>
public abstract class ChartAnimation
{
    /// <summary>
    /// Gets or sets the duration in seconds.
    /// </summary>
    public double Duration { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the delay before starting (in seconds).
    /// </summary>
    public double Delay { get; set; }

    /// <summary>
    /// Gets or sets the easing function.
    /// </summary>
    public IEasingFunction EasingFunction { get; set; } = EasingFunctions.CubicOut;

    /// <summary>
    /// Gets the current progress (0-1).
    /// </summary>
    public double Progress { get; protected set; }

    /// <summary>
    /// Gets the animation state.
    /// </summary>
    public AnimationState State { get; protected set; } = AnimationState.NotStarted;

    /// <summary>
    /// Starts the animation on the specified chart element.
    /// </summary>
    /// <param name="element">The chart element to animate.</param>
    public abstract void Start(ChartElement element);

    /// <summary>
    /// Updates the animation.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since last update (in seconds).</param>
    /// <returns>True if animation is still running; otherwise, false.</returns>
    public abstract bool Update(double deltaTime);

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public virtual void Stop()
    {
        State = AnimationState.Cancelled;
    }
}
