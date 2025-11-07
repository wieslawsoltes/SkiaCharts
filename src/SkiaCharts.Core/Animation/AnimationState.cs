namespace SkiaCharts.Core.Animation;

/// <summary>
/// Represents the state of an animation.
/// </summary>
public enum AnimationState
{
    /// <summary>
    /// Animation has not started yet.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Animation is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// Animation is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// Animation has completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Animation was cancelled.
    /// </summary>
    Cancelled
}
