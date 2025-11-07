using SkiaSharp;
using System.Diagnostics;

namespace SkiaCharts.Core.Interactivity;

/// <summary>
/// Provides animated transitions for viewport changes.
/// </summary>
public class ViewportAnimation
{
    private readonly Viewport _viewport;
    private AnimationState? _currentAnimation;
    private readonly Stopwatch _stopwatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewportAnimation"/> class.
    /// </summary>
    /// <param name="viewport">The viewport to animate.</param>
    public ViewportAnimation(Viewport viewport)
    {
        _viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
        _stopwatch = new Stopwatch();
        DefaultDuration = TimeSpan.FromMilliseconds(300);
        DefaultEasing = EasingFunction.EaseInOutCubic;
    }

    /// <summary>
    /// Gets or sets the default animation duration.
    /// </summary>
    public TimeSpan DefaultDuration { get; set; }

    /// <summary>
    /// Gets or sets the default easing function.
    /// </summary>
    public EasingFunction DefaultEasing { get; set; }

    /// <summary>
    /// Gets whether an animation is currently running.
    /// </summary>
    public bool IsAnimating => _currentAnimation != null;

    /// <summary>
    /// Event raised when an animation completes.
    /// </summary>
    public event EventHandler? AnimationCompleted;

    /// <summary>
    /// Animates a zoom change.
    /// </summary>
    /// <param name="targetZoom">The target zoom level.</param>
    /// <param name="duration">The animation duration (null uses default).</param>
    /// <param name="easing">The easing function (null uses default).</param>
    public void AnimateZoom(float targetZoom, TimeSpan? duration = null, EasingFunction? easing = null)
    {
        var startZoom = _viewport.Zoom;
        var startPan = _viewport.Pan;

        StartAnimation(new AnimationState
        {
            StartZoom = startZoom,
            TargetZoom = targetZoom,
            StartPan = startPan,
            TargetPan = startPan,
            Duration = duration ?? DefaultDuration,
            Easing = easing ?? DefaultEasing
        });
    }

    /// <summary>
    /// Animates a pan change.
    /// </summary>
    /// <param name="targetPan">The target pan offset.</param>
    /// <param name="duration">The animation duration (null uses default).</param>
    /// <param name="easing">The easing function (null uses default).</param>
    public void AnimatePan(SKPoint targetPan, TimeSpan? duration = null, EasingFunction? easing = null)
    {
        var startZoom = _viewport.Zoom;
        var startPan = _viewport.Pan;

        StartAnimation(new AnimationState
        {
            StartZoom = startZoom,
            TargetZoom = startZoom,
            StartPan = startPan,
            TargetPan = targetPan,
            Duration = duration ?? DefaultDuration,
            Easing = easing ?? DefaultEasing
        });
    }

    /// <summary>
    /// Animates both zoom and pan.
    /// </summary>
    /// <param name="targetZoom">The target zoom level.</param>
    /// <param name="targetPan">The target pan offset.</param>
    /// <param name="duration">The animation duration (null uses default).</param>
    /// <param name="easing">The easing function (null uses default).</param>
    public void AnimateZoomAndPan(float targetZoom, SKPoint targetPan, TimeSpan? duration = null, EasingFunction? easing = null)
    {
        var startZoom = _viewport.Zoom;
        var startPan = _viewport.Pan;

        StartAnimation(new AnimationState
        {
            StartZoom = startZoom,
            TargetZoom = targetZoom,
            StartPan = startPan,
            TargetPan = targetPan,
            Duration = duration ?? DefaultDuration,
            Easing = easing ?? DefaultEasing
        });
    }

    /// <summary>
    /// Animates to fit the data bounds.
    /// </summary>
    /// <param name="duration">The animation duration (null uses default).</param>
    /// <param name="easing">The easing function (null uses default).</param>
    public void AnimateToFit(TimeSpan? duration = null, EasingFunction? easing = null)
    {
        // Calculate target zoom and pan for fit
        var currentZoom = _viewport.Zoom;
        var currentPan = _viewport.Pan;

        // Temporarily apply zoom-to-fit to get target values
        _viewport.ZoomToFit();
        var targetZoom = _viewport.Zoom;
        var targetPan = _viewport.Pan;

        // Restore current values
        _viewport.Zoom = currentZoom;
        _viewport.Pan = currentPan;

        // Animate to target
        AnimateZoomAndPan(targetZoom, targetPan, duration, easing);
    }

    /// <summary>
    /// Updates the animation. Should be called each frame.
    /// </summary>
    /// <returns>True if animation is still running.</returns>
    public bool Update()
    {
        if (_currentAnimation == null)
            return false;

        var elapsed = _stopwatch.Elapsed;
        var progress = Math.Min(1.0, elapsed.TotalMilliseconds / _currentAnimation.Duration.TotalMilliseconds);

        // Apply easing
        var easedProgress = ApplyEasing(progress, _currentAnimation.Easing);

        // Interpolate zoom and pan
        var zoom = Lerp(_currentAnimation.StartZoom, _currentAnimation.TargetZoom, easedProgress);
        var pan = new SKPoint(
            Lerp(_currentAnimation.StartPan.X, _currentAnimation.TargetPan.X, easedProgress),
            Lerp(_currentAnimation.StartPan.Y, _currentAnimation.TargetPan.Y, easedProgress)
        );

        _viewport.Zoom = zoom;
        _viewport.Pan = pan;

        if (progress >= 1.0)
        {
            StopAnimation();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Stops the current animation.
    /// </summary>
    public void Stop()
    {
        StopAnimation();
    }

    private void StartAnimation(AnimationState state)
    {
        _currentAnimation = state;
        _stopwatch.Restart();
    }

    private void StopAnimation()
    {
        if (_currentAnimation != null)
        {
            _currentAnimation = null;
            _stopwatch.Stop();
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private static float Lerp(float start, float end, double progress)
    {
        return start + (end - start) * (float)progress;
    }

    private static double ApplyEasing(double progress, EasingFunction easing)
    {
        return easing switch
        {
            EasingFunction.Linear => progress,
            EasingFunction.EaseInQuad => progress * progress,
            EasingFunction.EaseOutQuad => progress * (2 - progress),
            EasingFunction.EaseInOutQuad => progress < 0.5
                ? 2 * progress * progress
                : 1 - Math.Pow(-2 * progress + 2, 2) / 2,
            EasingFunction.EaseInCubic => progress * progress * progress,
            EasingFunction.EaseOutCubic => 1 - Math.Pow(1 - progress, 3),
            EasingFunction.EaseInOutCubic => progress < 0.5
                ? 4 * progress * progress * progress
                : 1 - Math.Pow(-2 * progress + 2, 3) / 2,
            EasingFunction.EaseInQuart => progress * progress * progress * progress,
            EasingFunction.EaseOutQuart => 1 - Math.Pow(1 - progress, 4),
            EasingFunction.EaseInOutQuart => progress < 0.5
                ? 8 * progress * progress * progress * progress
                : 1 - Math.Pow(-2 * progress + 2, 4) / 2,
            _ => progress
        };
    }

    private class AnimationState
    {
        public float StartZoom { get; set; }
        public float TargetZoom { get; set; }
        public SKPoint StartPan { get; set; }
        public SKPoint TargetPan { get; set; }
        public TimeSpan Duration { get; set; }
        public EasingFunction Easing { get; set; }
    }
}

/// <summary>
/// Easing function enumeration for animations.
/// </summary>
public enum EasingFunction
{
    /// <summary>Linear interpolation (no easing).</summary>
    Linear,
    /// <summary>Quadratic ease-in.</summary>
    EaseInQuad,
    /// <summary>Quadratic ease-out.</summary>
    EaseOutQuad,
    /// <summary>Quadratic ease-in-out.</summary>
    EaseInOutQuad,
    /// <summary>Cubic ease-in.</summary>
    EaseInCubic,
    /// <summary>Cubic ease-out.</summary>
    EaseOutCubic,
    /// <summary>Cubic ease-in-out.</summary>
    EaseInOutCubic,
    /// <summary>Quartic ease-in.</summary>
    EaseInQuart,
    /// <summary>Quartic ease-out.</summary>
    EaseOutQuart,
    /// <summary>Quartic ease-in-out.</summary>
    EaseInOutQuart
}
