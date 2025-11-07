namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a range of data values with minimum and maximum bounds.
/// </summary>
public readonly struct DataRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataRange"/> struct.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public DataRange(double min, double max)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum value cannot be greater than maximum value.");
        }

        Min = min;
        Max = max;
    }

    /// <summary>
    /// Gets the minimum value of the range.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// Gets the maximum value of the range.
    /// </summary>
    public double Max { get; }

    /// <summary>
    /// Gets the span (difference) between the maximum and minimum values.
    /// </summary>
    public double Span => Max - Min;

    /// <summary>
    /// Gets the center point of the range.
    /// </summary>
    public double Center => (Min + Max) / 2.0;

    /// <summary>
    /// Gets a value indicating whether the range is valid (non-empty and finite).
    /// </summary>
    public bool IsValid => !double.IsNaN(Min) && !double.IsNaN(Max) &&
                           !double.IsInfinity(Min) && !double.IsInfinity(Max) &&
                           Min <= Max;

    /// <summary>
    /// Determines whether the specified value is within this range.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is within the range; otherwise, false.</returns>
    public bool Contains(double value) => value >= Min && value <= Max;

    /// <summary>
    /// Creates a new range with additional padding applied as a percentage of the span.
    /// </summary>
    /// <param name="paddingFraction">The padding fraction (e.g., 0.1 for 10% padding).</param>
    /// <returns>A new range with padding applied.</returns>
    public DataRange WithPadding(double paddingFraction)
    {
        var padding = Span * paddingFraction;
        return new DataRange(Min - padding, Max + padding);
    }

    /// <summary>
    /// Creates a new range expanded to include the specified value.
    /// </summary>
    /// <param name="value">The value to include.</param>
    /// <returns>A new range that includes the value.</returns>
    public DataRange ExpandTo(double value)
    {
        return new DataRange(Math.Min(Min, value), Math.Max(Max, value));
    }

    /// <summary>
    /// Creates a new range that is the union of this range and another.
    /// </summary>
    /// <param name="other">The other range.</param>
    /// <returns>A new range that encompasses both ranges.</returns>
    public DataRange Union(DataRange other)
    {
        return new DataRange(Math.Min(Min, other.Min), Math.Max(Max, other.Max));
    }

    /// <summary>
    /// Returns a string representation of the range.
    /// </summary>
    public override string ToString() => $"[{Min}, {Max}] (Span: {Span})";

    /// <summary>
    /// Creates an empty/invalid range.
    /// </summary>
    public static DataRange Empty => new(double.NaN, double.NaN);

    /// <summary>
    /// Creates a range from a collection of values.
    /// </summary>
    /// <param name="values">The values to create a range from.</param>
    /// <returns>A range spanning all values.</returns>
    public static DataRange FromValues(IEnumerable<double> values)
    {
        var valueList = values.Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).ToList();

        if (valueList.Count == 0)
        {
            return Empty;
        }

        return new DataRange(valueList.Min(), valueList.Max());
    }
}
