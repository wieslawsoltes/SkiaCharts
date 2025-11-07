namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a basic 2D data point with X and Y coordinates.
/// </summary>
public readonly struct DataPoint : IDataPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataPoint"/> struct.
    /// </summary>
    /// <param name="x">The X-coordinate value.</param>
    /// <param name="y">The Y-coordinate value.</param>
    public DataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <inheritdoc/>
    public double X { get; }

    /// <inheritdoc/>
    public double Y { get; }

    /// <summary>
    /// Returns a string representation of the data point.
    /// </summary>
    public override string ToString() => $"({X}, {Y})";

    /// <summary>
    /// Deconstructs the data point into its X and Y components.
    /// </summary>
    public void Deconstruct(out double x, out double y)
    {
        x = X;
        y = Y;
    }
}
