using SkiaSharp;

namespace SkiaCharts.Core.Layout;

/// <summary>
/// Manages automatic layout calculation for chart components.
/// </summary>
public class LayoutEngine
{
    /// <summary>
    /// Gets or sets the total available width.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the total available height.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the padding around the chart.
    /// </summary>
    public Padding Padding { get; set; } = new Padding(10);

    /// <summary>
    /// Gets or sets the spacing between components.
    /// </summary>
    public float Spacing { get; set; } = 5;

    /// <summary>
    /// Gets the calculated plot area bounds.
    /// </summary>
    public SKRect PlotArea { get; private set; }

    /// <summary>
    /// Gets the calculated title area bounds.
    /// </summary>
    public SKRect? TitleArea { get; private set; }

    /// <summary>
    /// Gets the calculated legend area bounds.
    /// </summary>
    public SKRect? LegendArea { get; private set; }

    /// <summary>
    /// Gets the calculated left axis area bounds.
    /// </summary>
    public SKRect? LeftAxisArea { get; private set; }

    /// <summary>
    /// Gets the calculated right axis area bounds.
    /// </summary>
    public SKRect? RightAxisArea { get; private set; }

    /// <summary>
    /// Gets the calculated top axis area bounds.
    /// </summary>
    public SKRect? TopAxisArea { get; private set; }

    /// <summary>
    /// Gets the calculated bottom axis area bounds.
    /// </summary>
    public SKRect? BottomAxisArea { get; private set; }

    /// <summary>
    /// Calculates the layout for all chart components.
    /// </summary>
    /// <param name="config">The layout configuration.</param>
    public void Calculate(LayoutConfiguration config)
    {
        if (Width <= 0 || Height <= 0)
        {
            PlotArea = SKRect.Empty;
            return;
        }

        float currentTop = Padding.Top;
        float currentBottom = Height - Padding.Bottom;
        float currentLeft = Padding.Left;
        float currentRight = Width - Padding.Right;

        // Title area (if present)
        if (config.HasTitle && config.TitleHeight > 0)
        {
            TitleArea = new SKRect(currentLeft, currentTop, currentRight, currentTop + config.TitleHeight);
            currentTop += config.TitleHeight + Spacing;
        }

        // Top axis area (if present)
        if (config.HasTopAxis && config.TopAxisHeight > 0)
        {
            TopAxisArea = new SKRect(currentLeft, currentTop, currentRight, currentTop + config.TopAxisHeight);
            currentTop += config.TopAxisHeight + Spacing;
        }

        // Bottom axis area (if present)
        if (config.HasBottomAxis && config.BottomAxisHeight > 0)
        {
            currentBottom -= config.BottomAxisHeight + Spacing;
            BottomAxisArea = new SKRect(currentLeft, currentBottom, currentRight, currentBottom + config.BottomAxisHeight);
        }

        // Left axis area (if present)
        if (config.HasLeftAxis && config.LeftAxisWidth > 0)
        {
            LeftAxisArea = new SKRect(currentLeft, currentTop, currentLeft + config.LeftAxisWidth, currentBottom);
            currentLeft += config.LeftAxisWidth + Spacing;
        }

        // Right axis area (if present)
        if (config.HasRightAxis && config.RightAxisWidth > 0)
        {
            currentRight -= config.RightAxisWidth + Spacing;
            RightAxisArea = new SKRect(currentRight, currentTop, currentRight + config.RightAxisWidth, currentBottom);
        }

        // Legend area (if present)
        if (config.HasLegend)
        {
            switch (config.LegendPosition)
            {
                case LegendPosition.Right:
                    if (config.LegendWidth > 0)
                    {
                        currentRight -= config.LegendWidth + Spacing;
                        LegendArea = new SKRect(currentRight, currentTop, currentRight + config.LegendWidth, currentBottom);
                    }
                    break;

                case LegendPosition.Left:
                    if (config.LegendWidth > 0)
                    {
                        LegendArea = new SKRect(currentLeft, currentTop, currentLeft + config.LegendWidth, currentBottom);
                        currentLeft += config.LegendWidth + Spacing;
                    }
                    break;

                case LegendPosition.Top:
                    if (config.LegendHeight > 0)
                    {
                        LegendArea = new SKRect(currentLeft, currentTop, currentRight, currentTop + config.LegendHeight);
                        currentTop += config.LegendHeight + Spacing;
                    }
                    break;

                case LegendPosition.Bottom:
                    if (config.LegendHeight > 0)
                    {
                        currentBottom -= config.LegendHeight + Spacing;
                        LegendArea = new SKRect(currentLeft, currentBottom, currentRight, currentBottom + config.LegendHeight);
                    }
                    break;
            }
        }

        // Plot area (remaining space)
        PlotArea = new SKRect(currentLeft, currentTop, currentRight, currentBottom);
    }

    /// <summary>
    /// Resets all calculated areas.
    /// </summary>
    public void Reset()
    {
        PlotArea = SKRect.Empty;
        TitleArea = null;
        LegendArea = null;
        LeftAxisArea = null;
        RightAxisArea = null;
        TopAxisArea = null;
        BottomAxisArea = null;
    }
}

/// <summary>
/// Configuration for layout calculation.
/// </summary>
public class LayoutConfiguration
{
    /// <summary>
    /// Gets or sets whether a title is present.
    /// </summary>
    public bool HasTitle { get; set; }

    /// <summary>
    /// Gets or sets the title height.
    /// </summary>
    public float TitleHeight { get; set; }

    /// <summary>
    /// Gets or sets whether a legend is present.
    /// </summary>
    public bool HasLegend { get; set; }

    /// <summary>
    /// Gets or sets the legend position.
    /// </summary>
    public LegendPosition LegendPosition { get; set; } = LegendPosition.Right;

    /// <summary>
    /// Gets or sets the legend width (for left/right positions).
    /// </summary>
    public float LegendWidth { get; set; }

    /// <summary>
    /// Gets or sets the legend height (for top/bottom positions).
    /// </summary>
    public float LegendHeight { get; set; }

    /// <summary>
    /// Gets or sets whether a left axis is present.
    /// </summary>
    public bool HasLeftAxis { get; set; }

    /// <summary>
    /// Gets or sets the left axis width.
    /// </summary>
    public float LeftAxisWidth { get; set; }

    /// <summary>
    /// Gets or sets whether a right axis is present.
    /// </summary>
    public bool HasRightAxis { get; set; }

    /// <summary>
    /// Gets or sets the right axis width.
    /// </summary>
    public float RightAxisWidth { get; set; }

    /// <summary>
    /// Gets or sets whether a top axis is present.
    /// </summary>
    public bool HasTopAxis { get; set; }

    /// <summary>
    /// Gets or sets the top axis height.
    /// </summary>
    public float TopAxisHeight { get; set; }

    /// <summary>
    /// Gets or sets whether a bottom axis is present.
    /// </summary>
    public bool HasBottomAxis { get; set; }

    /// <summary>
    /// Gets or sets the bottom axis height.
    /// </summary>
    public float BottomAxisHeight { get; set; }
}

/// <summary>
/// Defines legend position options.
/// </summary>
public enum LegendPosition
{
    /// <summary>
    /// Legend on the right side.
    /// </summary>
    Right,

    /// <summary>
    /// Legend on the left side.
    /// </summary>
    Left,

    /// <summary>
    /// Legend on the top.
    /// </summary>
    Top,

    /// <summary>
    /// Legend on the bottom.
    /// </summary>
    Bottom
}

/// <summary>
/// Represents padding values for all four sides.
/// </summary>
public struct Padding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> struct with uniform padding.
    /// </summary>
    /// <param name="all">The padding for all sides.</param>
    public Padding(float all)
    {
        Left = Right = Top = Bottom = all;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> struct with separate values.
    /// </summary>
    /// <param name="left">The left padding.</param>
    /// <param name="top">The top padding.</param>
    /// <param name="right">The right padding.</param>
    /// <param name="bottom">The bottom padding.</param>
    public Padding(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Gets or sets the left padding.
    /// </summary>
    public float Left { get; set; }

    /// <summary>
    /// Gets or sets the top padding.
    /// </summary>
    public float Top { get; set; }

    /// <summary>
    /// Gets or sets the right padding.
    /// </summary>
    public float Right { get; set; }

    /// <summary>
    /// Gets or sets the bottom padding.
    /// </summary>
    public float Bottom { get; set; }

    /// <summary>
    /// Gets the horizontal padding (left + right).
    /// </summary>
    public float Horizontal => Left + Right;

    /// <summary>
    /// Gets the vertical padding (top + bottom).
    /// </summary>
    public float Vertical => Top + Bottom;
}
