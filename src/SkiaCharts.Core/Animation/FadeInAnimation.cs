using SkiaCharts.Core.Charts;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Animates the opacity of a chart element from 0 to 1.
/// </summary>
public class FadeInAnimation : ChartAnimation
{
    private Animation<double>? _opacityAnimation;
    private ChartElement? _element;

    /// <summary>
    /// Gets or sets the target opacity (0-1). Default is 1.0.
    /// </summary>
    public double TargetOpacity { get; set; } = 1.0;

    /// <inheritdoc/>
    public override void Start(ChartElement element)
    {
        _element = element;
        _opacityAnimation = new Animation<double>(
            from: 0,
            to: TargetOpacity,
            duration: Duration,
            Interpolators.Double
        )
        {
            EasingFunction = EasingFunction,
            Delay = Delay
        };

        _opacityAnimation.Updated += (s, e) =>
        {
            Progress = e.Progress;
            // Note: Opacity property needs to be added to ChartElement
            // For now, this is a placeholder
        };

        _opacityAnimation.Completed += (s, e) =>
        {
            State = AnimationState.Completed;
        };

        _opacityAnimation.Start();
        State = AnimationState.Running;
    }

    /// <inheritdoc/>
    public override bool Update(double deltaTime)
    {
        if (_opacityAnimation == null || State != AnimationState.Running)
        {
            return false;
        }

        return _opacityAnimation.Update(deltaTime);
    }

    /// <inheritdoc/>
    public override void Stop()
    {
        base.Stop();
        _opacityAnimation?.Stop();
    }
}
