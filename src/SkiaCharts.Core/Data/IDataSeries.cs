namespace SkiaCharts.Core.Data;

/// <summary>
/// Represents a series of data points with efficient indexing and enumeration.
/// </summary>
/// <typeparam name="T">The type of data points in the series.</typeparam>
public interface IDataSeries<out T> : IReadOnlyList<T> where T : IDataPoint
{
    /// <summary>
    /// Gets the name or identifier of the data series.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the minimum X value in the series.
    /// </summary>
    double MinX { get; }

    /// <summary>
    /// Gets the maximum X value in the series.
    /// </summary>
    double MaxX { get; }

    /// <summary>
    /// Gets the minimum Y value in the series.
    /// </summary>
    double MinY { get; }

    /// <summary>
    /// Gets the maximum Y value in the series.
    /// </summary>
    double MaxY { get; }

    /// <summary>
    /// Gets a value indicating whether the bounds have been calculated.
    /// </summary>
    bool AreBoundsValid { get; }

    /// <summary>
    /// Forces recalculation of min/max bounds.
    /// </summary>
    void InvalidateBounds();
}
