using SkiaCharts.Core.Charts;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Animates the scale of a chart element from 0 to 1.
/// </summary>
public class GrowAnimation : ChartAnimation
{
    private Animation<double>? _scaleAnimation;
    private ChartElement? _element;

    /// <summary>
    /// Gets or sets the origin point for scaling (0-1, where 0.5 is center).
    /// </summary>
    public (double X, double Y) ScaleOrigin { get; set; } = (0.5, 0.5);

    /// <inheritdoc/>
    public override void Start(ChartElement element)
    {
        _element = element;
        _scaleAnimation = new Animation<double>(
            from: 0,
            to: 1,
            duration: Duration,
            Interpolators.Double
        )
        {
            EasingFunction = EasingFunction,
            Delay = Delay
        };

        _scaleAnimation.Updated += (s, e) =>
        {
            Progress = e.Progress;
            // Scale value available via e.Value
            // Would need to apply transform in rendering
        };

        _scaleAnimation.Completed += (s, e) =>
        {
            State = AnimationState.Completed;
        };

        _scaleAnimation.Start();
        State = AnimationState.Running;
    }

    /// <inheritdoc/>
    public override bool Update(double deltaTime)
    {
        if (_scaleAnimation == null || State != AnimationState.Running)
        {
            return false;
        }

        return _scaleAnimation.Update(deltaTime);
    }

    /// <inheritdoc/>
    public override void Stop()
    {
        base.Stop();
        _scaleAnimation?.Stop();
    }

    /// <summary>
    /// Gets the current scale value.
    /// </summary>
    public double CurrentScale => _scaleAnimation?.CurrentValue ?? 1.0;
}
