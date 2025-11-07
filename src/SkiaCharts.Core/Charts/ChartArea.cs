using SkiaSharp;

namespace SkiaCharts.Core.Charts;

/// <summary>
/// Represents the plotting area of a chart with margins and padding.
/// </summary>
public class ChartArea
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartArea"/> class.
    /// </summary>
    public ChartArea()
    {
        Margin = new ChartMargin(40, 40, 60, 80);
        Padding = new ChartMargin(10, 10, 10, 10);
    }

    /// <summary>
    /// Gets or sets the outer margin (space for axes, labels, title).
    /// </summary>
    public ChartMargin Margin { get; set; }

    /// <summary>
    /// Gets or sets the inner padding (space between axis and data).
    /// </summary>
    public ChartMargin Padding { get; set; }

    /// <summary>
    /// Calculates the plot area rectangle given the total chart bounds.
    /// </summary>
    /// <param name="totalBounds">The total chart bounds.</param>
    /// <returns>The plot area rectangle.</returns>
    public SKRect CalculatePlotArea(SKRect totalBounds)
    {
        var left = totalBounds.Left + Margin.Left + Padding.Left;
        var top = totalBounds.Top + Margin.Top + Padding.Top;
        var right = totalBounds.Right - Margin.Right - Padding.Right;
        var bottom = totalBounds.Bottom - Margin.Bottom - Padding.Bottom;

        return new SKRect(left, top, right, bottom);
    }

    /// <summary>
    /// Calculates the rectangle available for the left axis.
    /// </summary>
    public SKRect CalculateLeftAxisArea(SKRect totalBounds)
    {
        return new SKRect(
            totalBounds.Left,
            totalBounds.Top + Margin.Top,
            totalBounds.Left + Margin.Left,
            totalBounds.Bottom - Margin.Bottom);
    }

    /// <summary>
    /// Calculates the rectangle available for the right axis.
    /// </summary>
    public SKRect CalculateRightAxisArea(SKRect totalBounds)
    {
        return new SKRect(
            totalBounds.Right - Margin.Right,
            totalBounds.Top + Margin.Top,
            totalBounds.Right,
            totalBounds.Bottom - Margin.Bottom);
    }

    /// <summary>
    /// Calculates the rectangle available for the bottom axis.
    /// </summary>
    public SKRect CalculateBottomAxisArea(SKRect totalBounds)
    {
        return new SKRect(
            totalBounds.Left + Margin.Left,
            totalBounds.Bottom - Margin.Bottom,
            totalBounds.Right - Margin.Right,
            totalBounds.Bottom);
    }

    /// <summary>
    /// Calculates the rectangle available for the top axis.
    /// </summary>
    public SKRect CalculateTopAxisArea(SKRect totalBounds)
    {
        return new SKRect(
            totalBounds.Left + Margin.Left,
            totalBounds.Top,
            totalBounds.Right - Margin.Right,
            totalBounds.Top + Margin.Top);
    }
}

/// <summary>
/// Represents margins for chart areas.
/// </summary>
public struct ChartMargin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartMargin"/> struct.
    /// </summary>
    public ChartMargin(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Gets or sets the left margin.
    /// </summary>
    public float Left { get; set; }

    /// <summary>
    /// Gets or sets the top margin.
    /// </summary>
    public float Top { get; set; }

    /// <summary>
    /// Gets or sets the right margin.
    /// </summary>
    public float Right { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin.
    /// </summary>
    public float Bottom { get; set; }
}
