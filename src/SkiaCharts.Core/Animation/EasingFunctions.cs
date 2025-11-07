namespace SkiaCharts.Core.Animation;

/// <summary>
/// Provides common easing functions for animations.
/// </summary>
public static class EasingFunctions
{
    /// <summary>
    /// Linear easing (no easing).
    /// </summary>
    public static readonly IEasingFunction Linear = new LinearEasing();

    /// <summary>
    /// Quadratic ease-in.
    /// </summary>
    public static readonly IEasingFunction QuadIn = new QuadraticEasing(EasingMode.In);

    /// <summary>
    /// Quadratic ease-out.
    /// </summary>
    public static readonly IEasingFunction QuadOut = new QuadraticEasing(EasingMode.Out);

    /// <summary>
    /// Quadratic ease-in-out.
    /// </summary>
    public static readonly IEasingFunction QuadInOut = new QuadraticEasing(EasingMode.InOut);

    /// <summary>
    /// Cubic ease-in.
    /// </summary>
    public static readonly IEasingFunction CubicIn = new CubicEasing(EasingMode.In);

    /// <summary>
    /// Cubic ease-out.
    /// </summary>
    public static readonly IEasingFunction CubicOut = new CubicEasing(EasingMode.Out);

    /// <summary>
    /// Cubic ease-in-out.
    /// </summary>
    public static readonly IEasingFunction CubicInOut = new CubicEasing(EasingMode.InOut);

    /// <summary>
    /// Sinusoidal ease-in.
    /// </summary>
    public static readonly IEasingFunction SineIn = new SinusoidalEasing(EasingMode.In);

    /// <summary>
    /// Sinusoidal ease-out.
    /// </summary>
    public static readonly IEasingFunction SineOut = new SinusoidalEasing(EasingMode.Out);

    /// <summary>
    /// Sinusoidal ease-in-out.
    /// </summary>
    public static readonly IEasingFunction SineInOut = new SinusoidalEasing(EasingMode.InOut);

    /// <summary>
    /// Exponential ease-in.
    /// </summary>
    public static readonly IEasingFunction ExpoIn = new ExponentialEasing(EasingMode.In);

    /// <summary>
    /// Exponential ease-out.
    /// </summary>
    public static readonly IEasingFunction ExpoOut = new ExponentialEasing(EasingMode.Out);

    /// <summary>
    /// Exponential ease-in-out.
    /// </summary>
    public static readonly IEasingFunction ExpoInOut = new ExponentialEasing(EasingMode.InOut);

    /// <summary>
    /// Elastic ease-in.
    /// </summary>
    public static readonly IEasingFunction ElasticIn = new ElasticEasing(EasingMode.In);

    /// <summary>
    /// Elastic ease-out.
    /// </summary>
    public static readonly IEasingFunction ElasticOut = new ElasticEasing(EasingMode.Out);

    /// <summary>
    /// Elastic ease-in-out.
    /// </summary>
    public static readonly IEasingFunction ElasticInOut = new ElasticEasing(EasingMode.InOut);

    /// <summary>
    /// Bounce ease-in.
    /// </summary>
    public static readonly IEasingFunction BounceIn = new BounceEasing(EasingMode.In);

    /// <summary>
    /// Bounce ease-out.
    /// </summary>
    public static readonly IEasingFunction BounceOut = new BounceEasing(EasingMode.Out);

    /// <summary>
    /// Bounce ease-in-out.
    /// </summary>
    public static readonly IEasingFunction BounceInOut = new BounceEasing(EasingMode.InOut);

    /// <summary>
    /// Back ease-in (overshoots before settling).
    /// </summary>
    public static readonly IEasingFunction BackIn = new BackEasing(EasingMode.In);

    /// <summary>
    /// Back ease-out (overshoots before settling).
    /// </summary>
    public static readonly IEasingFunction BackOut = new BackEasing(EasingMode.Out);

    /// <summary>
    /// Back ease-in-out (overshoots on both ends).
    /// </summary>
    public static readonly IEasingFunction BackInOut = new BackEasing(EasingMode.InOut);

    /// <summary>
    /// Circular ease-in (accelerates using circular function).
    /// </summary>
    public static readonly IEasingFunction CircIn = new CircularEasing(EasingMode.In);

    /// <summary>
    /// Circular ease-out (decelerates using circular function).
    /// </summary>
    public static readonly IEasingFunction CircOut = new CircularEasing(EasingMode.Out);

    /// <summary>
    /// Circular ease-in-out (accelerates and decelerates using circular function).
    /// </summary>
    public static readonly IEasingFunction CircInOut = new CircularEasing(EasingMode.InOut);

    /// <summary>
    /// Creates a custom cubic Bezier easing function.
    /// Common presets: ease = (0.25, 0.1, 0.25, 1.0), ease-in = (0.42, 0, 1.0, 1.0), ease-out = (0, 0, 0.58, 1.0), ease-in-out = (0.42, 0, 0.58, 1.0)
    /// </summary>
    /// <param name="x1">First control point X (0-1).</param>
    /// <param name="y1">First control point Y.</param>
    /// <param name="x2">Second control point X (0-1).</param>
    /// <param name="y2">Second control point Y.</param>
    /// <returns>A custom Bezier easing function.</returns>
    public static IEasingFunction CreateBezier(double x1, double y1, double x2, double y2)
    {
        return new BezierEasing(x1, y1, x2, y2);
    }

    private class LinearEasing : IEasingFunction
    {
        public double Ease(double t) => t;
    }

    private class QuadraticEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public QuadraticEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => t * t,
                EasingMode.Out => t * (2 - t),
                EasingMode.InOut => t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t,
                _ => t
            };
        }
    }

    private class CubicEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public CubicEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => t * t * t,
                EasingMode.Out => (--t) * t * t + 1,
                EasingMode.InOut => t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1,
                _ => t
            };
        }
    }

    private class SinusoidalEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public SinusoidalEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => 1 - Math.Cos(t * Math.PI / 2),
                EasingMode.Out => Math.Sin(t * Math.PI / 2),
                EasingMode.InOut => -(Math.Cos(Math.PI * t) - 1) / 2,
                _ => t
            };
        }
    }

    private class ExponentialEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public ExponentialEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => t == 0 ? 0 : Math.Pow(2, 10 * (t - 1)),
                EasingMode.Out => t == 1 ? 1 : 1 - Math.Pow(2, -10 * t),
                EasingMode.InOut => t == 0 ? 0 : t == 1 ? 1 : t < 0.5
                    ? Math.Pow(2, 20 * t - 10) / 2
                    : (2 - Math.Pow(2, -20 * t + 10)) / 2,
                _ => t
            };
        }
    }

    private class ElasticEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public ElasticEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            const double c4 = (2 * Math.PI) / 3;

            return _mode switch
            {
                EasingMode.In => t == 0 ? 0 : t == 1 ? 1 : -Math.Pow(2, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * c4),
                EasingMode.Out => t == 0 ? 0 : t == 1 ? 1 : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1,
                EasingMode.InOut => t == 0 ? 0 : t == 1 ? 1 : t < 0.5
                    ? -(Math.Pow(2, 20 * t - 10) * Math.Sin((20 * t - 11.125) * ((2 * Math.PI) / 4.5))) / 2
                    : (Math.Pow(2, -20 * t + 10) * Math.Sin((20 * t - 11.125) * ((2 * Math.PI) / 4.5))) / 2 + 1,
                _ => t
            };
        }
    }

    private class BounceEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        public BounceEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => 1 - EaseOutBounce(1 - t),
                EasingMode.Out => EaseOutBounce(t),
                EasingMode.InOut => t < 0.5 ? (1 - EaseOutBounce(1 - 2 * t)) / 2 : (1 + EaseOutBounce(2 * t - 1)) / 2,
                _ => t
            };
        }

        private static double EaseOutBounce(double t)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5 / d1) * t + 0.75;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25 / d1) * t + 0.9375;
            }
            else
            {
                return n1 * (t -= 2.625 / d1) * t + 0.984375;
            }
        }
    }

    private class BackEasing : IEasingFunction
    {
        private readonly EasingMode _mode;
        private const double Overshoot = 1.70158; // Standard overshoot constant

        public BackEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => t * t * ((Overshoot + 1) * t - Overshoot),
                EasingMode.Out => (t - 1) * (t - 1) * ((Overshoot + 1) * (t - 1) + Overshoot) + 1,
                EasingMode.InOut => t < 0.5
                    ? (2 * t) * (2 * t) * ((Overshoot * 1.525 + 1) * 2 * t - Overshoot * 1.525) / 2
                    : ((2 * t - 2) * (2 * t - 2) * ((Overshoot * 1.525 + 1) * (2 * t - 2) + Overshoot * 1.525) + 2) / 2,
                _ => t
            };
        }
    }

    private class CircularEasing : IEasingFunction
    {
        private readonly EasingMode _mode;

        public CircularEasing(EasingMode mode) => _mode = mode;

        public double Ease(double t)
        {
            return _mode switch
            {
                EasingMode.In => 1 - Math.Sqrt(1 - t * t),
                EasingMode.Out => Math.Sqrt(1 - (t - 1) * (t - 1)),
                EasingMode.InOut => t < 0.5
                    ? (1 - Math.Sqrt(1 - 4 * t * t)) / 2
                    : (Math.Sqrt(1 - (-2 * t + 2) * (-2 * t + 2)) + 1) / 2,
                _ => t
            };
        }
    }

    private class BezierEasing : IEasingFunction
    {
        private readonly double _x1, _y1, _x2, _y2;

        public BezierEasing(double x1, double y1, double x2, double y2)
        {
            _x1 = Math.Clamp(x1, 0, 1);
            _y1 = y1;
            _x2 = Math.Clamp(x2, 0, 1);
            _y2 = y2;
        }

        public double Ease(double t)
        {
            // Use Newton-Raphson method to find the t value for the given x
            // For cubic Bezier: x(t) = 3(1-t)²t·x1 + 3(1-t)t²·x2 + t³

            if (t <= 0) return 0;
            if (t >= 1) return 1;

            // Find t for the given x using binary search (simpler and more stable)
            var t0 = 0.0;
            var t1 = 1.0;
            var currentT = t;

            // Binary search to find the t value for input x
            const int maxIterations = 10;
            for (int i = 0; i < maxIterations; i++)
            {
                var currentX = CalculateBezierX(currentT);
                if (Math.Abs(currentX - t) < 0.001)
                    break;

                if (currentX < t)
                    t0 = currentT;
                else
                    t1 = currentT;

                currentT = (t0 + t1) / 2;
            }

            // Calculate the corresponding y value
            return CalculateBezierY(currentT);
        }

        private double CalculateBezierX(double t)
        {
            // x(t) = 3(1-t)²t·x1 + 3(1-t)t²·x2 + t³
            var oneMinusT = 1 - t;
            return 3 * oneMinusT * oneMinusT * t * _x1 +
                   3 * oneMinusT * t * t * _x2 +
                   t * t * t;
        }

        private double CalculateBezierY(double t)
        {
            // y(t) = 3(1-t)²t·y1 + 3(1-t)t²·y2 + t³
            var oneMinusT = 1 - t;
            return 3 * oneMinusT * oneMinusT * t * _y1 +
                   3 * oneMinusT * t * t * _y2 +
                   t * t * t;
        }
    }
}

/// <summary>
/// Defines the easing mode (In, Out, InOut).
/// </summary>
public enum EasingMode
{
    /// <summary>
    /// Ease in (slow start, fast end).
    /// </summary>
    In,

    /// <summary>
    /// Ease out (fast start, slow end).
    /// </summary>
    Out,

    /// <summary>
    /// Ease in-out (slow start, fast middle, slow end).
    /// </summary>
    InOut
}
