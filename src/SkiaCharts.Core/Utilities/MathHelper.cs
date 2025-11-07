namespace SkiaCharts.Core.Utilities;

/// <summary>
/// Provides mathematical utility functions for chart calculations.
/// </summary>
public static class MathHelper
{
    /// <summary>
    /// Clamps a value between a minimum and maximum.
    /// </summary>
    public static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Linearly interpolates between two values.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="t">The interpolation factor (0-1).</param>
    public static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

    /// <summary>
    /// Calculates the "nice" number close to the specified value.
    /// Used for axis tick calculations.
    /// </summary>
    public static double NiceNumber(double value, bool round)
    {
        var exponent = Math.Floor(Math.Log10(value));
        var fraction = value / Math.Pow(10, exponent);
        double niceFraction;

        if (round)
        {
            if (fraction < 1.5) niceFraction = 1;
            else if (fraction < 3) niceFraction = 2;
            else if (fraction < 7) niceFraction = 5;
            else niceFraction = 10;
        }
        else
        {
            if (fraction <= 1) niceFraction = 1;
            else if (fraction <= 2) niceFraction = 2;
            else if (fraction <= 5) niceFraction = 5;
            else niceFraction = 10;
        }

        return niceFraction * Math.Pow(10, exponent);
    }

    /// <summary>
    /// Rounds a value to a specified number of significant figures.
    /// </summary>
    public static double RoundToSignificantFigures(double value, int significantFigures)
    {
        if (value == 0) return 0;

        var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
        return scale * Math.Round(value / scale, significantFigures);
    }
}
