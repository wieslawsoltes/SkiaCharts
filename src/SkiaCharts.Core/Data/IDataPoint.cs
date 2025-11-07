namespace SkiaCharts.Core.Data;

/// <summary>
/// Base interface for all data points in the charting framework.
/// </summary>
public interface IDataPoint
{
    /// <summary>
    /// Gets the X-coordinate value of the data point.
    /// </summary>
    double X { get; }

    /// <summary>
    /// Gets the Y-coordinate value of the data point.
    /// </summary>
    double Y { get; }
}
