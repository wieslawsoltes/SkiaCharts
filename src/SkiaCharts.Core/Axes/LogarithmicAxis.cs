using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;

namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents a logarithmic axis for scientific data visualization.
/// </summary>
public class LogarithmicAxis : IAxis
{
    private DataRange _visibleRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogarithmicAxis"/> class.
    /// </summary>
    public LogarithmicAxis()
    {
        _visibleRange = new DataRange(1, 1000); // Default: 1 to 1000
        Position = AxisPosition.Bottom;
        AutoScale = true;
        ShowGridLines = true;
        ShowLabels = true;
        IsVisible = true;
        Layer = RenderLayer.Grid;
        Base = 10; // Default to base 10 (common for scientific data)
    }

    /// <inheritdoc/>
    public string? Title { get; set; }

    /// <inheritdoc/>
    public AxisPosition Position { get; set; }

    /// <inheritdoc/>
    public DataRange VisibleRange
    {
        get => _visibleRange;
        set => _visibleRange = value;
    }

    /// <inheritdoc/>
    public bool AutoScale { get; set; }

    /// <inheritdoc/>
    public bool ShowGridLines { get; set; }

    /// <inheritdoc/>
    public bool ShowLabels { get; set; }

    /// <inheritdoc/>
    public double? MinValue { get; set; }

    /// <inheritdoc/>
    public double? MaxValue { get; set; }

    /// <inheritdoc/>
    public bool IsVisible { get; set; }

    /// <inheritdoc/>
    public RenderLayer Layer { get; }

    /// <summary>
    /// Gets or sets the logarithm base (default is 10).
    /// </summary>
    public double Base { get; set; }

    /// <summary>
    /// Gets or sets the format string for tick labels.
    /// </summary>
    public string? LabelFormat { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<TickInfo> GenerateTicks()
    {
        var ticks = new List<TickInfo>();

        if (_visibleRange.Min <= 0 || _visibleRange.Max <= 0)
        {
            // Logarithmic axis requires positive values
            return ticks;
        }

        // Calculate log range
        var logMin = Math.Log(_visibleRange.Min, Base);
        var logMax = Math.Log(_visibleRange.Max, Base);

        // Generate major ticks at powers of the base
        var startPower = (int)Math.Floor(logMin);
        var endPower = (int)Math.Ceiling(logMax);

        for (int power = startPower; power <= endPower; power++)
        {
            var value = Math.Pow(Base, power);

            if (value >= _visibleRange.Min && value <= _visibleRange.Max)
            {
                ticks.Add(new TickInfo(value, FormatValue(value), true));
            }

            // Add minor ticks between major ticks (if range is small enough)
            if (endPower - startPower <= 4) // Only show minor ticks for small ranges
            {
                for (int i = 2; i < Base; i++)
                {
                    var minorValue = i * Math.Pow(Base, power);
                    if (minorValue >= _visibleRange.Min && minorValue <= _visibleRange.Max)
                    {
                        ticks.Add(new TickInfo(minorValue, FormatValue(minorValue), false));
                    }
                }
            }
        }

        return ticks;
    }

    /// <inheritdoc/>
    public string FormatValue(double value)
    {
        if (value <= 0)
        {
            return "0";
        }

        if (!string.IsNullOrEmpty(LabelFormat))
        {
            return value.ToString(LabelFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        // Check if value is a power of the base
        var logValue = Math.Log(value, Base);
        var power = Math.Round(logValue);

        if (Math.Abs(logValue - power) < 0.001)
        {
            // It's a power of the base, format as such
            if (Base == 10)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "10^{0:F0}", power);
            }
            else if (Base == Math.E)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "e^{0:F0}", power);
            }
            else
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}^{1:F0}", Base, power);
            }
        }

        // Not a clean power, use scientific notation for large/small values
        if (value >= 10000 || value < 0.01)
        {
            return value.ToString("E1", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Use decimal notation
        if (value < 1)
        {
            return value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (value < 10)
        {
            return value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (value < 100)
        {
            return value.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            return value.ToString("F0", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    /// <inheritdoc/>
    public DataRange CalculateOptimalRange(DataRange dataRange)
    {
        if (!dataRange.IsValid || dataRange.Min <= 0 || dataRange.Max <= 0)
        {
            return new DataRange(1, 1000); // Default range
        }

        // Calculate log range
        var logMin = Math.Log(dataRange.Min, Base);
        var logMax = Math.Log(dataRange.Max, Base);

        // Round to nearest powers
        var startPower = Math.Floor(logMin);
        var endPower = Math.Ceiling(logMax);

        // Add one power on each side for padding
        startPower -= 0.5;
        endPower += 0.5;

        var niceMin = Math.Pow(Base, startPower);
        var niceMax = Math.Pow(Base, endPower);

        return new DataRange(niceMin, niceMax);
    }

    /// <inheritdoc/>
    public void Render(IRenderContext context)
    {
        // Basic rendering implementation will be enhanced later
        // For now, this is a placeholder
    }

    /// <summary>
    /// Converts a linear value to logarithmic scale.
    /// </summary>
    /// <param name="value">The linear value.</param>
    /// <returns>The logarithmic value.</returns>
    public double ToLog(double value)
    {
        return value > 0 ? Math.Log(value, Base) : double.NegativeInfinity;
    }

    /// <summary>
    /// Converts a logarithmic value to linear scale.
    /// </summary>
    /// <param name="logValue">The logarithmic value.</param>
    /// <returns>The linear value.</returns>
    public double FromLog(double logValue)
    {
        return Math.Pow(Base, logValue);
    }
}
