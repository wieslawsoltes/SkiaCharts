namespace SkiaCharts.Core.Animation;

/// <summary>
/// Represents an easing function that controls the rate of change of a parameter over time.
/// </summary>
public interface IEasingFunction
{
    /// <summary>
    /// Transforms normalized time (0-1) using the easing function.
    /// </summary>
    /// <param name="normalizedTime">The normalized time value (0 = start, 1 = end).</param>
    /// <returns>The eased value.</returns>
    double Ease(double normalizedTime);
}
