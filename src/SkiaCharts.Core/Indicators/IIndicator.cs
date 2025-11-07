using SkiaCharts.Core.Data;

namespace SkiaCharts.Core.Indicators;

/// <summary>
/// Base interface for all technical indicators.
/// </summary>
public interface IIndicator
{
    /// <summary>
    /// Gets the name of the indicator.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Calculates the indicator values for the given data series.
    /// </summary>
    /// <param name="series">The input data series.</param>
    /// <returns>The calculated indicator values.</returns>
    IDataSeries<IDataPoint> Calculate(IDataSeries<IDataPoint> series);
}

/// <summary>
/// Base interface for overlay indicators that render on the same scale as price data.
/// </summary>
public interface IOverlayIndicator : IIndicator
{
}

/// <summary>
/// Base interface for panel indicators that render in a separate panel.
/// </summary>
public interface IPanelIndicator : IIndicator
{
    /// <summary>
    /// Gets the minimum Y-axis value for the indicator.
    /// </summary>
    double MinValue { get; }

    /// <summary>
    /// Gets the maximum Y-axis value for the indicator.
    /// </summary>
    double MaxValue { get; }
}
