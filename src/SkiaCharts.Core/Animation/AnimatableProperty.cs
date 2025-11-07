using System.ComponentModel;

namespace SkiaCharts.Core.Animation;

/// <summary>
/// Wraps a property value to make it animatable with change notifications.
/// </summary>
/// <typeparam name="T">The type of the property value.</typeparam>
public class AnimatableProperty<T> : INotifyPropertyChanged
{
    private T _value;
    private Animation<T>? _currentAnimation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatableProperty{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value.</param>
    public AnimatableProperty(T initialValue)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this property is currently being animated.
    /// </summary>
    public bool IsAnimating => _currentAnimation?.State == AnimationState.Running;

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Animates the property to a new value.
    /// </summary>
    /// <param name="targetValue">The target value.</param>
    /// <param name="duration">The animation duration in seconds.</param>
    /// <param name="interpolator">The interpolation function.</param>
    /// <param name="easingFunction">The easing function (optional).</param>
    /// <returns>The animation object.</returns>
    public Animation<T> AnimateTo(
        T targetValue,
        double duration,
        Func<T, T, double, T> interpolator,
        IEasingFunction? easingFunction = null)
    {
        // Cancel any existing animation
        _currentAnimation?.Stop();

        // Create new animation
        _currentAnimation = new Animation<T>(
            from: _value,
            to: targetValue,
            duration: duration,
            interpolator: interpolator
        )
        {
            EasingFunction = easingFunction ?? EasingFunctions.Linear
        };

        // Update value as animation progresses
        _currentAnimation.Updated += (s, e) =>
        {
            Value = e.Value;
        };

        // Clear reference when completed
        _currentAnimation.Completed += (s, e) =>
        {
            _currentAnimation = null;
        };

        _currentAnimation.Start();
        return _currentAnimation;
    }

    /// <summary>
    /// Cancels any current animation.
    /// </summary>
    public void CancelAnimation()
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.Stop();
            _currentAnimation = null;
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Implicit conversion from AnimatableProperty to its value.
    /// </summary>
    public static implicit operator T(AnimatableProperty<T> property) => property.Value;
}
