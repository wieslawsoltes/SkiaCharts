namespace SkiaCharts.Core.Animation;

/// <summary>
/// Represents an object that can be animated.
/// </summary>
public interface IAnimatable
{
    /// <summary>
    /// Gets a value indicating whether this object can currently be animated.
    /// </summary>
    bool CanAnimate { get; }

    /// <summary>
    /// Called when an animation starts on this object.
    /// </summary>
    void OnAnimationStarted();

    /// <summary>
    /// Called when an animation completes on this object.
    /// </summary>
    void OnAnimationCompleted();

    /// <summary>
    /// Requests the object to invalidate and redraw.
    /// </summary>
    void Invalidate();
}
