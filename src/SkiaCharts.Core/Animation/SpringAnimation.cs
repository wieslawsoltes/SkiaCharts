namespace SkiaCharts.Core.Animation;

/// <summary>
/// Physics-based spring animation for natural motion.
/// </summary>
/// <typeparam name="T">The type of value being animated.</typeparam>
public class SpringAnimation<T>
{
    private readonly Func<T, T, double, T> _interpolator;
    private T _currentValue;
    private double _velocity;
    private AnimationState _state = AnimationState.NotStarted;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpringAnimation{T}"/> class.
    /// </summary>
    /// <param name="from">The starting value.</param>
    /// <param name="to">The target value.</param>
    /// <param name="interpolator">The interpolation function.</param>
    public SpringAnimation(T from, T to, Func<T, T, double, T> interpolator)
    {
        From = from;
        To = to;
        _interpolator = interpolator;
        _currentValue = from;
    }

    /// <summary>
    /// Gets the starting value.
    /// </summary>
    public T From { get; }

    /// <summary>
    /// Gets or sets the target value.
    /// </summary>
    public T To { get; set; }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    public T CurrentValue => _currentValue;

    /// <summary>
    /// Gets the animation state.
    /// </summary>
    public AnimationState State => _state;

    /// <summary>
    /// Gets or sets the spring stiffness (higher = faster). Default is 100.
    /// </summary>
    public double Stiffness { get; set; } = 100.0;

    /// <summary>
    /// Gets or sets the damping coefficient (higher = less bouncy). Default is 10.
    /// </summary>
    public double Damping { get; set; } = 10.0;

    /// <summary>
    /// Gets or sets the mass. Default is 1.0.
    /// </summary>
    public double Mass { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the threshold for considering the animation complete. Default is 0.001.
    /// </summary>
    public double Threshold { get; set; } = 0.001;

    /// <summary>
    /// Event raised when the animation completes.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Starts the animation.
    /// </summary>
    public void Start()
    {
        _state = AnimationState.Running;
        _velocity = 0;
    }

    /// <summary>
    /// Updates the spring animation using Hooke's law.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since last update (in seconds).</param>
    /// <returns>True if still animating; otherwise, false.</returns>
    public bool Update(double deltaTime)
    {
        if (_state != AnimationState.Running)
        {
            return false;
        }

        // Simplified spring physics (works for double values)
        // For complex types, this would need adaptation

        // Calculate current progress (0-1)
        // This is a simplified version - real spring would track position/velocity
        var currentProgress = GetProgress(_currentValue);
        var targetProgress = 1.0;

        // Spring force: F = -k * x
        var displacement = targetProgress - currentProgress;
        var springForce = Stiffness * displacement;

        // Damping force: F = -c * v
        var dampingForce = Damping * _velocity;

        // Net force
        var force = springForce - dampingForce;

        // Acceleration: a = F / m
        var acceleration = force / Mass;

        // Update velocity
        _velocity += acceleration * deltaTime;

        // Update position
        var newProgress = currentProgress + _velocity * deltaTime;
        newProgress = Math.Clamp(newProgress, 0, 1);

        // Interpolate value
        _currentValue = _interpolator(From, To, newProgress);

        // Check if animation should complete (velocity near zero and close to target)
        if (Math.Abs(_velocity) < Threshold && Math.Abs(displacement) < Threshold)
        {
            _currentValue = To;
            _state = AnimationState.Completed;
            Completed?.Invoke(this, EventArgs.Empty);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public void Stop()
    {
        _state = AnimationState.Cancelled;
    }

    private double GetProgress(T current)
    {
        // This is a simplified version for demonstration
        // Real implementation would need to compare current with From/To
        // For now, return 0.5 as placeholder
        return 0.5;
    }
}
