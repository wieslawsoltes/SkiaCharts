namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents information about an axis tick.
/// </summary>
public readonly struct TickInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TickInfo"/> struct.
    /// </summary>
    /// <param name="value">The data value at this tick.</param>
    /// <param name="label">The formatted label for this tick.</param>
    /// <param name="isMajor">Whether this is a major tick.</param>
    public TickInfo(double value, string label, bool isMajor = true)
    {
        Value = value;
        Label = label;
        IsMajor = isMajor;
    }

    /// <summary>
    /// Gets the data value at this tick position.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Gets the formatted label text for this tick.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets a value indicating whether this is a major tick (as opposed to a minor tick).
    /// </summary>
    public bool IsMajor { get; }

    /// <summary>
    /// Returns a string representation of the tick.
    /// </summary>
    public override string ToString() => $"{Value}: {Label} ({(IsMajor ? "Major" : "Minor")})";
}
