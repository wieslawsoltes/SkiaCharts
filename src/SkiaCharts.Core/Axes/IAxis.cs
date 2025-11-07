using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents an axis on a chart.
/// </summary>
public interface IAxis : IRenderable
{
    /// <summary>
    /// Gets or sets the title of the axis.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Gets or sets the position of the axis.
    /// </summary>
    AxisPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the visible range of values on this axis.
    /// </summary>
    DataRange VisibleRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the axis automatically scales to fit the data.
    /// </summary>
    bool AutoScale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether grid lines should be drawn.
    /// </summary>
    bool ShowGridLines { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tick labels should be drawn.
    /// </summary>
    bool ShowLabels { get; set; }

    /// <summary>
    /// Gets or sets the minimum value for the axis (when not auto-scaling).
    /// </summary>
    double? MinValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for the axis (when not auto-scaling).
    /// </summary>
    double? MaxValue { get; set; }

    /// <summary>
    /// Generates tick marks for the current visible range.
    /// </summary>
    /// <returns>A collection of tick information.</returns>
    IReadOnlyList<TickInfo> GenerateTicks();

    /// <summary>
    /// Formats a value for display on this axis.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted string.</returns>
    string FormatValue(double value);

    /// <summary>
    /// Calculates the optimal range for the given data range.
    /// </summary>
    /// <param name="dataRange">The data range to fit.</param>
    /// <returns>The optimal axis range.</returns>
    DataRange CalculateOptimalRange(DataRange dataRange);
}
