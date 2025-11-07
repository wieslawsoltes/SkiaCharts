using SkiaCharts.Core.Charts;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Animation that reveals chart elements with a wipe effect.
/// </summary>
public class WipeAnimation : ChartAnimation
{
    private double _elapsedTime;

    /// <summary>
    /// Gets or sets the wipe direction.
    /// </summary>
    public WipeDirection Direction { get; set; } = WipeDirection.LeftToRight;

    /// <summary>
    /// Gets the current clip percentage based on animation progress.
    /// </summary>
    public double ClipPercentage => Progress;

    /// <summary>
    /// Calculates the clip region for the current progress.
    /// </summary>
    /// <param name="totalWidth">The total width to clip.</param>
    /// <param name="totalHeight">The total height to clip.</param>
    /// <returns>The visible region as (x, y, width, height).</returns>
    public (double X, double Y, double Width, double Height) GetClipRegion(double totalWidth, double totalHeight)
    {
        return Direction switch
        {
            WipeDirection.LeftToRight => (0, 0, totalWidth * Progress, totalHeight),
            WipeDirection.RightToLeft => (totalWidth * (1 - Progress), 0, totalWidth * Progress, totalHeight),
            WipeDirection.TopToBottom => (0, 0, totalWidth, totalHeight * Progress),
            WipeDirection.BottomToTop => (0, totalHeight * (1 - Progress), totalWidth, totalHeight * Progress),
            _ => (0, 0, totalWidth, totalHeight)
        };
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
/// Defines the direction for wipe animations.
/// </summary>
public enum WipeDirection
{
    /// <summary>
    /// Wipe from left to right.
    /// </summary>
    LeftToRight,

    /// <summary>
    /// Wipe from right to left.
    /// </summary>
    RightToLeft,

    /// <summary>
    /// Wipe from top to bottom.
    /// </summary>
    TopToBottom,

    /// <summary>
    /// Wipe from bottom to top.
    /// </summary>
    BottomToTop
}
