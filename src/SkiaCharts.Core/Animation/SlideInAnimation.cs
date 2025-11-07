using SkiaCharts.Core.Charts;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Animation that slides chart elements in from a direction.
/// </summary>
public class SlideInAnimation : ChartAnimation
{
    private double _elapsedTime;

    /// <summary>
    /// Gets or sets the direction to slide from.
    /// </summary>
    public SlideDirection Direction { get; set; } = SlideDirection.Left;

    /// <summary>
    /// Gets or sets the distance to slide (as a multiplier of the element size).
    /// </summary>
    public double Distance { get; set; } = 1.0;

    /// <summary>
    /// Gets the current offset based on animation progress.
    /// </summary>
    public (double X, double Y) CurrentOffset
    {
        get
        {
            var t = 1.0 - Progress; // Invert so we start offset and move to 0

            return Direction switch
            {
                SlideDirection.Left => (-Distance * t, 0),
                SlideDirection.Right => (Distance * t, 0),
                SlideDirection.Top => (0, -Distance * t),
                SlideDirection.Bottom => (0, Distance * t),
                _ => (0, 0)
            };
        }
    }

    /// <inheritdoc/>
    public override void Start(ChartElement element)
    {
        State = AnimationState.Running;
        Progress = 0;
        _elapsedTime = 0;
    }

    /// <inheritdoc/>
    public override bool Update(double deltaTime)
    {
        if (State != AnimationState.Running)
            return false;

        _elapsedTime += deltaTime;

        if (_elapsedTime < Delay)
            return true;

        var adjustedTime = _elapsedTime - Delay;
        Progress = Math.Min(adjustedTime / Duration, 1.0);

        var easedProgress = EasingFunction.Ease(Progress);
        Progress = easedProgress;

        if (Progress >= 1.0)
        {
            Progress = 1.0;
            State = AnimationState.Completed;
            return false;
        }

        return true;
    }
}

/// <summary>
/// Defines the direction for slide animations.
/// </summary>
public enum SlideDirection
{
    /// <summary>
    /// Slide from the left.
    /// </summary>
    Left,

    /// <summary>
    /// Slide from the right.
    /// </summary>
    Right,

    /// <summary>
    /// Slide from the top.
    /// </summary>
    Top,

    /// <summary>
    /// Slide from the bottom.
    /// </summary>
    Bottom
}
