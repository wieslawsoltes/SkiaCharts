using SkiaCharts.Core.Data;
using SkiaCharts.Core.Rendering;
using SkiaCharts.Core.Utilities;

namespace SkiaCharts.Core.Axes;

/// <summary>
/// Represents a linear (numeric) axis with even spacing.
/// </summary>
public class LinearAxis : IAxis
{
    private DataRange _visibleRange;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearAxis"/> class.
    /// </summary>
    public LinearAxis()
    {
        _visibleRange = new DataRange(0, 10);
        Position = AxisPosition.Bottom;
        AutoScale = true;
        ShowGridLines = true;
        ShowLabels = true;
        IsVisible = true;
        Layer = RenderLayer.Grid;
        TargetTickCount = 10;
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
    /// Gets or sets the desired number of tick marks.
    /// The actual number may differ to achieve "nice" numbers.
    /// </summary>
    public int TargetTickCount { get; set; }

    /// <summary>
    /// Gets or sets the format string for tick labels.
    /// </summary>
    public string? LabelFormat { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<TickInfo> GenerateTicks()
    {
        var ticks = new List<TickInfo>();
        var range = _visibleRange.Span;

        if (range <= 0)
        {
            return ticks;
        }

        // Calculate nice tick interval
        var roughInterval = range / (TargetTickCount - 1);
        var niceInterval = MathHelper.NiceNumber(roughInterval, true);

        // Calculate nice min/max
        var niceMin = Math.Floor(_visibleRange.Min / niceInterval) * niceInterval;
        var niceMax = Math.Ceiling(_visibleRange.Max / niceInterval) * niceInterval;

        // Generate ticks
        for (double value = niceMin; value <= niceMax; value += niceInterval)
        {
            // Handle floating point precision issues
            var roundedValue = Math.Round(value / niceInterval) * niceInterval;

            if (roundedValue >= _visibleRange.Min && roundedValue <= _visibleRange.Max)
            {
                ticks.Add(new TickInfo(roundedValue, FormatValue(roundedValue), true));
            }
        }

        return ticks;
    }

    /// <inheritdoc/>
    public string FormatValue(double value)
    {
        if (!string.IsNullOrEmpty(LabelFormat))
        {
            return value.ToString(LabelFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        // Auto-format based on magnitude
        var absValue = Math.Abs(value);

        // Special case for zero
        if (absValue == 0)
        {
            return "0";
        }

        if (absValue < 0.01 || absValue >= 10000)
        {
            return value.ToString("E2", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (absValue < 1)
        {
            return value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (absValue < 100)
        {
            return value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            return value.ToString("F0", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    /// <inheritdoc/>
    public DataRange CalculateOptimalRange(DataRange dataRange)
    {
        if (!dataRange.IsValid)
        {
            return new DataRange(0, 10);
        }

        var range = dataRange.Span;
        if (range <= 0)
        {
            // Data has no span, create artificial range
            var center = dataRange.Min;
            return new DataRange(center - 5, center + 5);
        }

        // Add 5% padding
        var paddedRange = dataRange.WithPadding(0.05);

        // Round to nice numbers
        var roughInterval = range / (TargetTickCount - 1);
        var niceInterval = MathHelper.NiceNumber(roughInterval, false);

        var niceMin = Math.Floor(paddedRange.Min / niceInterval) * niceInterval;
        var niceMax = Math.Ceiling(paddedRange.Max / niceInterval) * niceInterval;

        return new DataRange(niceMin, niceMax);
    }

    /// <inheritdoc/>
    public void Render(IRenderContext context)
    {
        // Basic rendering implementation will be enhanced later
        // For now, this is a placeholder
    }
}
